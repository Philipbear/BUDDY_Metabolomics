using BUDDY.BufpHelper;
using BUDDY.FormulaData;
using BUDDY.RawData;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;
using BUDDY.GlobalOptimizationMILP;
using BUDDY.MS2LibrarySearch;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.Json.Serialization;

namespace BUDDY
{
    [Serializable]
    public class Ms2Utility : INotifyPropertyChanged
    {
        public bool selected;
        public int fileindex;
        public double mz;
        public double rt;
        public string imageLink;
        public string polarity;
        public string filename;
        public int charge;
        public Adduct adduct;
        public List<RAW_PeakElement> oriSpectrum { get; set; } // original spectrum, for ms2 library search
        public List<RAW_PeakElement> spectrum { get; set; } // current spectrum after denoise, deisotope, precursor exclusion
        public int scanNumber;

        public string formula_pc; //precursor formula, neutral formula
        public string inchikey;
        public string inchikeyFirstHalf;
        public string metaboliteName;
        public bool seedMetabolite; // annotation provided by users, regarded as ground truth

        // for reload pubchem search
        public bool pubchemAccessedBefore;
        public string pubchemCID;
        public string pubchemDescription;
        public BitmapImage pubchemImage;

        public List<RAW_PeakElement> ms1 { get; set; }
        public List<int> mergedindex;
        public double ms2tol;
        public double ms1tol;
        public List<Feature> candidates { get; set; }
        public List<RAW_PeakElement> precursorFragments { get; set; }
        public List<RAW_PeakElement> isotopeFragments { get; set; }
        public List<RAW_PeakElement> noiseFragments { get; set; }

        public List<FeatureConnection> featureConnections { get; set; }
        public Ms2MatchingResult ms2Matching { get; set; }
        public Ms2Utility()
        { }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged("Selected"); }
        }

        public int FileIndex
        {
            get { return fileindex; }
            set { fileindex = value; OnPropertyChanged("Index"); }
        }

        public double Mz
        {
            get { return mz; }
            set { mz = value; OnPropertyChanged("Mz"); }
        }
        public double Rt
        {
            get { return rt; }
            set { rt = value; OnPropertyChanged("Rt"); }
        }
        //public int AdductID
        //{
        //    get { return adductID; }
        //    set { adductID = value; OnPropertyChanged("AdductID"); }
        //}
        public string ImageLink
        {
            get { return imageLink; }
            set { imageLink = value; OnPropertyChanged("ImageLink"); }
        }
        public string Polarity // "P" or "N"
        {
            get { return polarity; }
            set { polarity = value; OnPropertyChanged("Polarity"); }
        }
        public string Filename
        {
            get { return filename; }
            set { filename = value; OnPropertyChanged("Filename"); }
        }
        public int Charge
        {
            get { return charge; }
            set { charge = value; OnPropertyChanged("Charge"); }
        }
        public Adduct Adduct
        {
            get { return adduct; }
            set { adduct = value; OnPropertyChanged("Adduct"); }
        }        
        public List<RAW_PeakElement> OriSpectrum // used for ms2 library search
        {
            get { return oriSpectrum; }
            set { oriSpectrum = value; OnPropertyChanged("OriSpectrum"); }
        }
        public List<RAW_PeakElement> Spectrum
        {
            get { return spectrum; }
            set { spectrum = value; OnPropertyChanged("Spectrum"); }
        }
        public int ScanNumber
        {
            get { return scanNumber; }
            set { scanNumber = value; OnPropertyChanged("ScanNumber"); }
        }
        public string Formula_PC // neutral formula
        {
            get { return formula_pc; }
            set { formula_pc = value; OnPropertyChanged("Formula_PC"); }
        }
        public string InChiKey
        {
            get { return inchikey; }
            set { inchikey = value; OnPropertyChanged("InChiKey"); }
        }
        public string InChiKeyFirstHalf
        {
            get { return inchikeyFirstHalf; }
            set { inchikeyFirstHalf = value; OnPropertyChanged("InChiKeyFirstHalf"); }
        }
        public string MetaboliteName
        {
            get { return metaboliteName; }
            set { metaboliteName = value; OnPropertyChanged("MetaboliteName"); }
        }
        public bool SeedMetabolite
        {
            get { return seedMetabolite; }
            set { seedMetabolite = value; OnPropertyChanged("SeedMetabolite"); }
        }
        public bool PubchemAccessedBefore
        {
            get { return pubchemAccessedBefore; }
            set { pubchemAccessedBefore = value; OnPropertyChanged("PubchemAccessedBefore"); }
        }
        public string PubchemCID
        {
            get { return pubchemCID; }
            set { pubchemCID = value; OnPropertyChanged("PubchemCID"); }
        }
        public string PubchemDescription
        {
            get { return pubchemDescription; }
            set { pubchemDescription = value; OnPropertyChanged("PubchemDescription"); }
        }
        public BitmapImage PubchemImage
        {
            get { return pubchemImage; }
            set { pubchemImage = value; OnPropertyChanged("PubchemImage"); }
        }

        public List<RAW_PeakElement> Ms1
        {
            get { return ms1; }
            set { ms1 = value; OnPropertyChanged("Ms1"); }
        }
        public List<int> MergedIndex
        {
            get { return mergedindex; }
            set { mergedindex = value; OnPropertyChanged("MergedIndex"); }
        }
        public double Ms2Tol
        {
            get { return ms2tol; }
            set { ms2tol = value; OnPropertyChanged("Ms2Tol"); }
        }
        public double Ms1Tol
        {
            get { return ms1tol; }
            set { ms1tol = value; OnPropertyChanged("Ms1Tol"); }
        }
        public List<Feature> Candidates
        {
            get { return candidates; }
            set { candidates = value; OnPropertyChanged("Candidates"); }
        }
        public List<RAW_PeakElement> PrecursorFragments
        {
            get { return precursorFragments; }
            set { precursorFragments = value; OnPropertyChanged("PrecursorFragments"); }
        }
        public List<RAW_PeakElement> IsotopeFragments
        {
            get { return isotopeFragments; }
            set { isotopeFragments = value; OnPropertyChanged("IsotopeFragments"); }
        }
        public List<RAW_PeakElement> NoiseFragments
        {
            get { return noiseFragments; }
            set { noiseFragments = value; OnPropertyChanged("NoiseFragments"); }
        }
        public List<FeatureConnection> FeatureConnections
        {
            get { return featureConnections; }
            set { featureConnections = value; OnPropertyChanged("FeatureConnections"); }
        }
        public Ms2MatchingResult Ms2Matching
        {
            get { return ms2Matching; }
            set { ms2Matching = value; OnPropertyChanged("Ms2Matching"); }
        }
  
        public Ms2Utility(double mz, string polarity, string inchikey, string formula_pc, List<RAW_PeakElement> spectrum, Adduct adduct, double ms2tol, double ms1tol)
        {
            this.mz = Math.Round(mz, 4);
            this.polarity = polarity;
            this.inchikey = inchikey;
            this.formula_pc = formula_pc;
            this.spectrum = spectrum;
            this.adduct = adduct;
            this.ms2tol = ms2tol;
            this.ms1tol = ms1tol;
        }
        public Ms2Utility(double mz, string polarity, List<RAW_PeakElement> spectrum, Adduct adduct, double ms2tol, double ms1tol, List<RAW_PeakElement> ms1)
        {
            this.mz = Math.Round(mz, 4);
            this.polarity = polarity;
            this.oriSpectrum = spectrum;
            this.adduct = adduct;
            this.ms2tol = ms2tol;
            this.ms1tol = ms1tol;
            this.ms1 = ms1;
        }
        public Ms2Utility(double mz, string polarity, List<RAW_PeakElement> orispectrum, Adduct adduct, double ms2tol, double ms1tol, List<RAW_PeakElement> ms1, List<RAW_PeakElement> spectrum)
        {
            this.mz = Math.Round(mz, 4);
            this.polarity = polarity;
            this.oriSpectrum = orispectrum;
            this.spectrum = spectrum;
            this.adduct = adduct;
            this.ms2tol = ms2tol;
            this.ms1tol = ms1tol;
            this.ms1 = ms1;
        }
        //public Ms2Utility(int scannumber, double mz, double rt, string polarity, Adduct adduct, List<RAW_PeakElement> ms1, List<RAW_PeakElement> spectrum)
        //{
        //    this.scanNumber = scannumber;
        //    this.mz = Math.Round(mz, 4);
        //    this.rt = Math.Round(rt, 2);
        //    this.polarity = polarity;
        //    this.adduct = adduct;
        //    this.ms1 = ms1;
        //    this.spectrum = spectrum;
        //}

        public Ms2Utility(bool selected, int fileindex, double mz, double rt, string imageLink, string polarity,
            string filename, int charge, Adduct adduct, List<RAW_PeakElement> spectrum, int scanNumber,
            string formula_pc, string inchikey, List<RAW_PeakElement> ms1)
        {
            this.selected = selected;
            this.fileindex = fileindex;
            this.mz = Math.Round(mz, 4);
            this.rt = Math.Round(rt, 2);
            this.imageLink = imageLink;
            this.polarity = polarity;
            this.filename = filename;
            this.charge = charge;
            this.adduct = adduct;
            this.spectrum = spectrum;
            this.scanNumber = scanNumber;
            this.formula_pc = formula_pc;
            this.inchikey = inchikey;
            this.ms1 = ms1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    [Serializable]
    public class Ms2MatchingResult
    {
        public string MS2DB { get; set; } // ms2DB name
        public int MS2MatchingAlgorithm { get; set; } // algorithm index: 0-2
        public List<MS2MatchResult> ms2MatchingReturns { get; set; } // all matchings to this MS2
}

    [DelimitedRecord(",")]
    public class MS2csv
    {
        public string index { get; set; }
        public string precursor_mz { get; set; }
        public string rt { get; set; }
        public string polarity { get; set; }
        public string adduct { get; set; }
        public string ms1mz { get; set; }
        public string ms1int { get; set; }
        public string ms2mz { get; set; }
        public string ms2int { get; set; }
    }

    public class MS2Manager
    {
        public static List<Ms2Utility> GetMS2(FileUtility file)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            // adduct 
            List<Adduct> adductList_Pos;
            List<Adduct> adductList_Neg;
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                adductList_Pos = new List<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
            }
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                adductList_Neg = new List<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
            }
            List<string> adductList_PosString = adductList_Pos.Select(o => o.Formula).ToList();
            List<string> adductList_NegString = adductList_Neg.Select(o => o.Formula).ToList();


            var engine = new FileHelperEngine<MS2csv>();
            var records = engine.ReadFile(file.FilePath);
            var ms2_db = new List<Ms2Utility>();

            for(int i = 1; i < records.Length; i++)
            {
                if(records[i].precursor_mz == "" || records[i].index == "" || records[i].polarity == "" || records[i].precursor_mz == "NA" || records[i].index == "NA" || records[i].polarity == "NA")
                {
                    continue;
                }

                List<RAW_PeakElement> currms1 = new List<RAW_PeakElement>();
                List<RAW_PeakElement> currms2 = new List<RAW_PeakElement>();
                if(records[i].ms1mz != "" && records[i].ms1int != "" && records[i].ms1mz != "NA" && records[i].ms1int != "NA")
                {
                    List<double> currms1mz = records[i].ms1mz.Split(';').Select(double.Parse).ToList();
                    List<double> currms1int = records[i].ms1int.Split(';').Select(double.Parse).ToList();
                    if(currms1mz.Count == currms1int.Count)
                    {
                        for (int j = 0; j < currms1mz.Count; j++)
                        {
                            currms1.Add(new RAW_PeakElement() { Mz = currms1mz[j], Intensity = currms1int[j] });
                        }
                    }
                }
                if (records[i].ms2mz != "" && records[i].ms2int != "" && records[i].ms2mz != "NA" && records[i].ms2int != "NA")
                {
                    List<double> currms2mz = records[i].ms2mz.Split(';').Select(double.Parse).ToList();
                    List<double> currms2int = records[i].ms2int.Split(';').Select(double.Parse).ToList();
                    if(currms2mz.Count == currms2int.Count)
                    {
                        for (int k = 0; k < currms2mz.Count; k++)
                        {
                            currms2.Add(new RAW_PeakElement() { Mz = currms2mz[k], Intensity = currms2int[k] });
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                //else
                //{
                //    continue;
                //}

                Ms2Utility newMS2 = new Ms2Utility();
                newMS2.Selected = false;
                newMS2.FileIndex = file.Index;

                double ms2premz = 0.0;
                if(double.TryParse(records[i].precursor_mz, out ms2premz))
                {
                    newMS2.Mz = Math.Round(ms2premz, 4);
                }
                else
                {
                    continue;
                }

                if(records[i].rt != "" && records[i].rt != "NA")
                {
                    double ms2rt = 0.0;
                    if(double.TryParse(records[i].rt, out ms2rt))
                    {
                        newMS2.Rt = Math.Round(ms2rt, 2);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    newMS2.Rt = 0.00;
                }

                newMS2.ImageLink = "open.png";
                newMS2.InChiKey = "Unknown";
                newMS2.Formula_PC = "Unknown";
                newMS2.Filename = file.FileName;
                if (records[i].adduct != "" && records[i].adduct != "NA")
                {
                    //if (records[i].polarity == "P" && adductList_PosString.Contains(records[i].adduct))
                    //{
                    //    newMS2.Adduct = new Adduct(records[i].adduct);
                    //}
                    //else if (records[i].polarity == "N" && adductList_NegString.Contains(records[i].adduct))
                    //{
                    //    newMS2.Adduct = new Adduct(records[i].adduct);
                    //}
                    //else
                    //{
                    //    continue;
                    //}

                    try
                    {
                        newMS2.Adduct = new Adduct(records[i].adduct);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    if (records[i].polarity == "P")
                    {
                        newMS2.Adduct = new Adduct("[M+H]+");
                    }
                    else
                    {
                        newMS2.Adduct = new Adduct("[M-H]-");
                    }
                }
                newMS2.ScanNumber = int.Parse(records[i].index, CultureInfo.InvariantCulture.NumberFormat);
                newMS2.OriSpectrum = currms2;
                newMS2.Ms1 = currms1;
                newMS2.Polarity = records[i].polarity;

                ms2_db.Add(newMS2);
            }
            return ms2_db;
        }

    }


    //public class TextUpdate : INotifyPropertyChanged
    //{
    //    public string text { get; set; }
    //    public string Text
    //    {
    //        get { return text; }
    //        set { text = value; OnPropertyChanged("Text"); }
    //    }
    //    public TextUpdate(string thisText)
    //    {
    //        this.text = thisText;
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private void OnPropertyChanged(String prop)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    //        }
    //    }
    //}


    //public class AdductDetails : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private void RaisePropertyChanged(string name)
    //    {
    //        if (PropertyChanged != null)
    //            PropertyChanged(this, new PropertyChangedEventArgs(name));
    //    }

    //    private int adductID;
    //    public int AdductID
    //    {
    //        get
    //        {
    //            return adductID;
    //        }
    //        set
    //        {
    //            adductID = value;
    //            RaisePropertyChanged("AdductID");
    //        }
    //    }

    //    private string adductName;
    //    public string AdductName
    //    {
    //        get
    //        {
    //            return adductName;
    //        }
    //        set
    //        {
    //            adductName = value;
    //            RaisePropertyChanged("AdductName");
    //        }
    //    }
    //}

    //public static class PolarityAdductRepository
    //{
    //    static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
    //    public static Dictionary<string, ObservableCollection<AdductDetails>> GetAdduct()
    //    {
    //        ObservableCollection<Adduct> adductList_Pos;
    //        ObservableCollection<Adduct> adductList_Neg;
    //        using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Open))
    //        {
    //            var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //            adductList_Pos = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
    //        }
    //        using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Open))
    //        {
    //            var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //            adductList_Neg = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
    //        }
    //        ObservableCollection<AdductDetails> positive = new ObservableCollection<AdductDetails>();
    //        int posID = 0;
    //        foreach (Adduct add in adductList_Pos)
    //        {
    //            posID += 1;
    //            positive.Add(new AdductDetails() { AdductName = add.Formula, AdductID = posID });
    //        }

    //        ObservableCollection<AdductDetails> negative = new ObservableCollection<AdductDetails>();
    //        int negID = 100;
    //        foreach (Adduct add in adductList_Neg)
    //        {
    //            negID += 1;
    //            negative.Add(new AdductDetails() { AdductName = add.Formula, AdductID = negID });
    //        }

    //        Dictionary<string, ObservableCollection<AdductDetails>> adductDictionary =
    //            new Dictionary<string, ObservableCollection<AdductDetails>>();
    //        adductDictionary.Add("P", positive);
    //        adductDictionary.Add("N", negative);
    //        return adductDictionary;
    //    }
    //}
}
