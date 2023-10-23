using BUDDY.FormulaData;
using BUDDY.RawData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace BUDDY.MsdialDataHandler
{
    public sealed class MsdialParser
    {
        private static bool alignedTXT;
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public MsdialParser() { }

        public static List<Ms2Utility> ReadMsdialTxt(FileUtility file, bool featureClusterBool)
        {
            List<Ms2Utility> MsdialRecords = new List<Ms2Utility>();

            StreamReader reader = File.OpenText(file.FilePath);

            string[] adductOrder = { "[M+K]+", "[2M+NH4]+", "[M+Na]+", "[2M+H]+", "[M+H-2H2O]+","[M+NH4]+", "[M+H-H2O]+", "[M+H]+",
                                      "[M+Na-2H]-","[2M-H]-", "[M+Br]-","[M+Cl]-","[M+CH3COO]-","[M+HCOO]-","[M-H2O-H]-","[M-H]-" };
            bool IndexFound = false;
            int MzColIndex = 2;
            int RtColIndex = 1;
            int AdductColIndex = 4;
            int MetaboliteNameColIndex = 3;
            int FormulaColIndex = 10;
            int INCHIKEYColIndex = 12;
            int MS1ColIndex = 30;
            int MS2ColIndex = 31;
            int CommentIndex = 5;
            string line;
            List<int> duplicateFeatureScanNo = new List<int>();

            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split('\t');

                //Debug.WriteLine(line);

                if (items[0] == "")
                {
                    continue;
                }

                if (!IndexFound)
                {
                    if (items[0] == "Alignment ID")
                    {
                        alignedTXT = true;
                        //  find the row index of the "name row"  (alignment ID, or peak ID)
                        MzColIndex = Array.FindIndex(items, o => o == "Average Mz");
                        RtColIndex = Array.FindIndex(items, o => o == "Average Rt(min)");
                        AdductColIndex = Array.FindIndex(items, o => o == "Adduct type");
                        MetaboliteNameColIndex = Array.FindIndex(items, o => o == "Metabolite name");
                        FormulaColIndex = Array.FindIndex(items, o => o == "Formula");
                        INCHIKEYColIndex = Array.FindIndex(items, o => o == "INCHIKEY");
                        MS1ColIndex = Array.FindIndex(items, o => o == "MS1 isotopic spectrum");
                        MS2ColIndex = Array.FindIndex(items, o => o == "MS/MS spectrum");
                        CommentIndex = Array.FindIndex(items, o => o == "Post curation result");
                        IndexFound = true;
                        continue;
                    }
                    if (items[0] == "PeakID")
                    {
                        alignedTXT = false;
                        //  find the row index of the "name row"  (alignment ID, or peak ID)
                        MzColIndex = Array.FindIndex(items, o => o == "Precursor m/z");
                        RtColIndex = Array.FindIndex(items, o => o == "RT (min)");
                        AdductColIndex = Array.FindIndex(items, o => o == "Adduct");
                        MetaboliteNameColIndex = Array.FindIndex(items, o => o == "Title");
                        FormulaColIndex = Array.FindIndex(items, o => o == "Formula");
                        INCHIKEYColIndex = Array.FindIndex(items, o => o == "InChIKey");
                        MS1ColIndex = Array.FindIndex(items, o => o == "MS1 isotopes");
                        MS2ColIndex = Array.FindIndex(items, o => o == "MSMS spectrum");
                        CommentIndex = Array.FindIndex(items, o => o == "Comment");
                        IndexFound = true;
                        continue;
                    }
                }

                Ms2Utility currFeature = new Ms2Utility();

                // scan number
                currFeature.ScanNumber = int.Parse(items[0]);
                if (featureClusterBool)
                {
                    if (duplicateFeatureScanNo.Contains(currFeature.ScanNumber)) { continue; }

                    // deal with comment
                    string comment = items[CommentIndex];
                    if (comment != "")
                    {
                        // isotope
                        if (comment.Contains("isotope")) { continue; }

                        string[] links = comment.Split(";");
                        List<FeatureGroup> featureGroup = new List<FeatureGroup>();
                        featureGroup.Add(new FeatureGroup { scanNo = currFeature.ScanNumber, adductOrder = Array.FindIndex(adductOrder, o => o == items[AdductColIndex]) });
                        foreach (string link in links)
                        {
                            if (link.Contains("adduct"))
                            {
                                string[] linkSplit = link.Substring(17).Split("_");
                                if (int.TryParse(linkSplit[0], out int scanNo))
                                {
                                    featureGroup.Add(new FeatureGroup { scanNo = scanNo, adductOrder = Array.FindIndex(adductOrder, o => o == linkSplit[1]) });
                                }
                            }
                        }
                        if (featureGroup.Count > 1)
                        {
                            featureGroup = featureGroup.OrderByDescending(o => o.adductOrder).ToList();
                            featureGroup.RemoveAt(0);
                            duplicateFeatureScanNo.AddRange(featureGroup.Select(o => o.scanNo));

                            if (duplicateFeatureScanNo.Contains(currFeature.ScanNumber)) { continue; }
                        }
                    }
                }
                
                currFeature.Selected = false;
                currFeature.OriSpectrum = new List<RAW_PeakElement>();
                currFeature.Ms1 = new List<RAW_PeakElement>();
                currFeature.FileIndex = file.Index;
                currFeature.Filename = file.FileName;
                currFeature.ImageLink = "open.png";

                // mz, rt
                var m = 0.0;
                if (double.TryParse(items[MzColIndex], out m)) currFeature.Mz = Math.Round(m, 4);
                var rm = 0.0;
                if (double.TryParse(items[RtColIndex], out rm)) currFeature.Rt = Math.Round(rm, 4);

                // adduct
                try
                {
                    currFeature.Adduct = new Adduct(items[AdductColIndex]);
                    currFeature.Polarity = currFeature.Adduct.Mode;
                }
                catch
                {
                    continue;
                }

                if (items[MetaboliteNameColIndex].Contains("Unknown") == false && items[MetaboliteNameColIndex].Contains("w/o MS2") == false && items[FormulaColIndex]!= "" && items[INCHIKEYColIndex] != "")
                {
                    // name
                    currFeature.MetaboliteName = items[MetaboliteNameColIndex];
                    // inchikey
                    // remove white space
                    currFeature.InChiKey = Regex.Replace(items[INCHIKEYColIndex], @"\s+", "");
                    currFeature.InChiKeyFirstHalf = currFeature.InChiKey.Substring(0, 14);
                    // formula
                    currFeature.Formula_PC = Regex.Replace(items[FormulaColIndex], @"\s+", "");
                    // seed metabolite
                    currFeature.seedMetabolite = true;
                }


                // MS1
                if (!string.IsNullOrEmpty(items[MS1ColIndex]))
                {
                    string[] ms1PKs = items[MS1ColIndex].Split(' ');

                    foreach (string currms1 in ms1PKs)
                    {
                        string[] MzInt = currms1.Split(':');
                        currFeature.Ms1.Add(new RAW_PeakElement() { Mz = double.Parse(MzInt[0]), Intensity = double.Parse(MzInt[1]) });
                    }
                }
                // MS2
                if (!string.IsNullOrEmpty(items[MS2ColIndex]))
                {
                    string[] ms2PKs = items[MS2ColIndex].Split(' ');
                    foreach (string currms2 in ms2PKs)
                    {
                        string[] MzInt = currms2.Split(':');
                        currFeature.OriSpectrum.Add(new RAW_PeakElement() { Mz = double.Parse(MzInt[0]), Intensity = double.Parse(MzInt[1]) });
                    }
                }


                //else
                //{
                //    currFeature.ScanNumber = int.Parse(items[0]);
                //    var m = 0.0;
                //    if (double.TryParse(items[6], out m)) currFeature.Mz = Math.Round(m, 4);
                //    var rm = 0.0;
                //    if (double.TryParse(items[4], out rm)) currFeature.Rt = Math.Round(rm, 4);
                //    try
                //    {
                //        currFeature.Adduct = new Adduct(items[10]);
                //        currFeature.Polarity = currFeature.Adduct.Mode;
                //    }
                //    catch
                //    {
                //        continue;
                //    }
                //    if (!string.IsNullOrEmpty(items[MS1ColIndex]))
                //    {
                //        string[] ms1PKs = items[MS1ColIndex].Split(' ');
                //        foreach (string currms1 in ms1PKs)
                //        {
                //            string[] MzInt = currms1.Split(':');
                //            currFeature.Ms1.Add(new RAW_PeakElement() { Mz = double.Parse(MzInt[0]), Intensity = double.Parse(MzInt[1]) });
                //        }
                //    }
                //    if (!string.IsNullOrEmpty(items[MS2ColIndex]))
                //    {
                //        string[] ms2PKs = items[MS2ColIndex].Split(' ');
                //        foreach (string currms2 in ms2PKs)
                //        {
                //            string[] MzInt = currms2.Split(':');
                //            currFeature.Spectrum.Add(new RAW_PeakElement() { Mz = double.Parse(MzInt[0]), Intensity = double.Parse(MzInt[1]) });
                //        }
                //    }
                //}

                //if(currFeature.Ms1.Count > 0)
                //{
                //    double maxMS1Xint = currFeature.Ms1.Max(o => o.Intensity);
                //    for (int i = 0; i < currFeature.Ms1.Count; i++)
                //    {
                //        RAW_PeakElement str = currFeature.Ms1[i];
                //        str.Intensity = currFeature.Ms1[i].Intensity / maxMS1Xint;
                //        currFeature.Ms1[i] = str;
                //    }

                //    //bool ppm = (bool)localSettings.Values["ms1tol_ppmON"];
                //    bool ppm = false;

                //    double M0Intensity = currFeature.Ms1[0].Intensity;
                //    double isotopeBinMzTol = 0.02;
                //    double tmpMzDiff = isotopeBinMzTol;
                //    double IsotopeCutoff = 0.0;
                //    List<RAW_PeakElement> isoy = new List<RAW_PeakElement>();
                //    isoy.Add(new RAW_PeakElement() { Mz = currFeature.Ms1[0].Mz, Intensity = currFeature.Ms1[0].Intensity });

                //    //binning
                //    double tmpMz = currFeature.Ms1[0].Mz;
                //    for (int i = 1; i < currFeature.Ms1.Count; i++)
                //    {
                //        bool M = false;

                //        List<double> MzList = new List<double>();
                //        List<double> IntList = new List<double>();

                //        if (ppm)
                //        {
                //            tmpMzDiff = (tmpMz + 1.003355) * isotopeBinMzTol * 1e-6;
                //        }
                //        for (int j = 1; j < currFeature.Ms1.Count; j++)
                //        {
                //            if (Math.Abs(currFeature.Ms1[j].Mz - (tmpMz + 1.003355)) <= tmpMzDiff) // include all the ions within the mz range
                //            {
                //                MzList.Add(currFeature.Ms1[j].Mz);
                //                IntList.Add(currFeature.Ms1[j].Intensity);
                //                M = true;
                //            }
                //        }
                //        if (M == false)
                //        {
                //            break;
                //        }
                //        else
                //        {    // for all the ions within the mz range, calculate weighted average m/z, sum intensity
                //            double weightedSumMz = 0;
                //            double sumInt = 0;
                //            for (int m = 0; m < MzList.Count; m++)
                //            {
                //                weightedSumMz += MzList[m] * IntList[m];
                //                sumInt += IntList[m];
                //            }
                //            tmpMz = weightedSumMz / sumInt;

                //            if ((100.0 * sumInt / M0Intensity) >= IsotopeCutoff)
                //            {
                //                isoy.Add(new RAW_PeakElement() { Mz = tmpMz, Intensity = sumInt });
                //            }
                //            else
                //            {
                //                continue;
                //            }
                //        }
                //    }

                //    currFeature.Ms1 = isoy;
                //}

                MsdialRecords.Add(currFeature);
               
            }
            return MsdialRecords;
        }

    }
    class FeatureGroup
    {
        public int scanNo { get; set; }
        public int adductOrder { get; set; }
    }
}
