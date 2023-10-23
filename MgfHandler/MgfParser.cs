using BUDDY.FormulaData;
using BUDDY.RawData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace BUDDY.MgfHandler
{
    public sealed class MgfParser
    {
        public MgfParser() { }

        public static List<Ms2Utility> ReadMgf(FileUtility file)
        {
            List<Ms2Utility> mgfRecords = new List<Ms2Utility>();
            int index = 1;

            using (StreamReader sr = new StreamReader(file.FilePath, Encoding.ASCII))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    if (line == "BEGIN IONS")
                    {
                        var record = getMgfRecord(sr, file.FileName, file.Index);
                        record.ScanNumber = index;
                        mgfRecords.Add(record);
                        index++;
                    }
                }
            }

            return mgfRecords;
        }

        public static Ms2Utility getMgfRecord(StreamReader sr, string fileName, int fileIndex)
        {
            Ms2Utility mgfRecord = new Ms2Utility();
            mgfRecord.Selected = false;
            mgfRecord.OriSpectrum = new List<RAW_PeakElement>();
            mgfRecord.FileIndex = fileIndex;
            mgfRecord.Filename = fileName;
            mgfRecord.InChiKey = "Unknown";
            mgfRecord.Formula_PC = "Unknown";
            mgfRecord.ImageLink = "open.png";
            mgfRecord.Polarity = "P";
            mgfRecord.Adduct = new Adduct("[M+H]+");

            while (sr.Peek() > -1)
            {
                var line = sr.ReadLine();
                if (line == "END IONS") return mgfRecord;

                var lineEqualSplit = line.Split('=');
                if (lineEqualSplit.Length > 1)
                {
                    var field = lineEqualSplit[0];
                    switch (field)
                    {
                        //case "TITLE": mgfRecord.Title = line.Substring(field.Length + 1); break;
                        case "PEPMASS":
                            var m = 0.0;
                            if (double.TryParse(lineEqualSplit[1], out m)) mgfRecord.Mz = Math.Round(m, 4);
                            break;
                        case "RTINSECONDS":
                            var rn = 0.0;
                            if (double.TryParse(lineEqualSplit[1], out rn)) mgfRecord.Rt = Math.Round(rn / 60.0, 2);
                            break;
                        case "RTINMINUTES":
                            var rm = 0.0;
                            if (double.TryParse(lineEqualSplit[1], out rm)) mgfRecord.Rt = Math.Round(rm, 2);
                            break;
                        case "CHARGE":
                            var chargeState = lineEqualSplit[1].Trim();
                            var chargeValue = 0;
                            if (int.TryParse(chargeState.Substring(0, chargeState.Length - 1), out chargeValue))
                                mgfRecord.Charge = chargeValue;

                            if (chargeState.Contains("+"))
                            {
                                mgfRecord.Polarity = "P";
                                //mgfRecord.Adduct = new Adduct("[M+H]+");
                            }
                            else if (chargeState.Contains("-"))
                            {
                                mgfRecord.Polarity = "N";
                                mgfRecord.Adduct = new Adduct("[M-H]-");
                            }

                            break;
                        case "ION":
                            var adduct = lineEqualSplit[1].Trim();
                            try
                            {
                                mgfRecord.Adduct = new Adduct(adduct);

                            }
                            catch
                            {
                                mgfRecord.Adduct = new Adduct("[M+H]+");
                            }
                            break;
                        //case "MSLEVEL":
                        //    var mslevelString = lineEqualSplit[1].Trim();
                        //    var mslevel = 1;
                        //    if (int.TryParse(mslevelString, out mslevel)) mgfRecord.Mslevel = mslevel;
                        //    break;
                        //case "SOURCE_INSTRUMENT":
                        //    mgfRecord.Source_instrument = line.Substring(field.Length + 1);
                        //    break;
                        //case "FILENAME":
                        //    mgfRecord.Filename = fileName;
                        //    break;
                        //case "SEQ":
                        //    mgfRecord.Seq = line.Substring(field.Length + 1);
                        //    break;
                        case "IONMODE":
                            if (lineEqualSplit[1].Contains("P") || lineEqualSplit[1].Contains("p"))
                            {
                                mgfRecord.Polarity = "P";
                                //mgfRecord.Adduct = new Adduct("[M+H]+");
                            }
                            else
                            {
                                mgfRecord.Polarity = "N";
                                mgfRecord.Adduct = new Adduct("[M-H]-");
                            }
                            break;
                        //case "ORGANISM":
                        //    mgfRecord.Organism = line.Substring(field.Length + 1);
                        //    break;
                        //case "NAME":
                        //    mgfRecord.Name = line.Substring(field.Length + 1);
                            //var adduct = line.Split(' ')[line.Split(' ').Length - 1];
                            //if (AdductConverter.GnpsAdductToMF.ContainsKey(adduct))
                            //{
                            //    var adductMF = AdductConverter.GnpsAdductToMF[adduct];
                            //    mgfRecord.Adduct = AdductIonParcer.GetAdductIonBean(adductMF);
                            //}
                            //else mgfRecord.Adduct = null;

                        //case "PI":
                        //    mgfRecord.Pi = line.Substring(field.Length + 1);
                        //    break;
                        //case "DATACOLLECTOR":
                        //    mgfRecord.Datacollector = line.Substring(field.Length + 1); ;
                        //    break;
                        //case "SMILES":
                        //    if (lineEqualSplit[1].Length > 0)
                        //        mgfRecord.Smiles = line.Substring(field.Length + 1); ;
                        //    break;
                        //case "LIBRARYQUALITY":
                        //    mgfRecord.LibraryQuality = line.Substring(field.Length + 1);
                        //    break;
                        //case "SPECTRUMID":
                        //    mgfRecord.SpectrumID = line.Substring(field.Length + 1);
                        //    break;
                        //case "SCAN":
                        //    var scan = 0;
                        //    var scanString = lineEqualSplit[1];
                        //    if (int.TryParse(scanString, out scan)) mgfRecord.Scan = scan;
                        //    break;
                    }
                }
                else
                {
                    double mz = 0.0, intensity = 0.0;
                    int charge = 1;
                    //var lineSpaceSplit = line.Split('\t');
                    var lineSpaceSplit = line.Split(' ', '\t');
                    var peak = new RAW_PeakElement();
                    if (lineSpaceSplit.Length > 0 && double.TryParse(lineSpaceSplit[0], out mz)) peak.Mz = mz;
                    if (lineSpaceSplit.Length > 1 && double.TryParse(lineSpaceSplit[1], out intensity)) peak.Intensity = intensity;
                    //if (lineSpaceSplit.Length > 2 && int.TryParse(lineSpaceSplit[2], out charge)) peak.Charge = charge;
                    mgfRecord.OriSpectrum.Add(peak);
                }
            }


            return mgfRecord;
        }
    }
}
