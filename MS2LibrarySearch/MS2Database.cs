using BUDDY.RawData;
using FileHelpers;
using NCDK;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.MS2LibrarySearch
{

    //public class MS2Database
    //{
    //    public string MS2DBName { get; set; }
    //    public List<MS2DBEntry> DBEntries { get; set; }
    //}
    [Serializable]
    public class MS2DBEntry
    {
        //public string MS2DBName { get; set; }
        public string MetaboliteName { get; set; }
        public string DBNumberString { get; set; }
        public string InChIKey { get; set; }
        public string Adduct { get; set; }
        public double PrecursorMz { get; set; }
        public string InstrumentType { get; set; }
        public string Instrument { get; set; }
        public string IonMode { get; set; } // P or N
        public string CollisionEnergy { get; set; }
        public string Formula { get; set; }
        public string Comments { get; set; }
        public List<RAW_PeakElement> MS2Spec { get; set; }

        public string InChIKeyFirstHalf { get; set; }
        public bool ValidRT { get; set; } // whether RT in this DB entry is valid (for public repository, false)
        public double RTminute { get; set; } 
    }

    //[Serializable]
    //[DelimitedRecord("\t")]
    //public class MS2DBEntryInput
    //{
    //    public string MetaboliteName { get; set; }
    //    public string DBNumberString { get; set; }
    //    public string InChIKey { get; set; }
    //    public string Adduct { get; set; }
    //    public string PrecursorMz { get; set; }
    //    public string InstrumentType { get; set; }
    //    public string Instrument { get; set; }
    //    public string IonMode { get; set; }
    //    public string CollisionEnergy { get; set; }
    //    public string Formula { get; set; }
    //    public string Comments { get; set; }
    //    public string MS2mz { get; set; }
    //    public string MS2int { get; set; }
    //    public string InChIKeyFirstHalf { get; set; }
    //    public string RTminute { get; set; }
    //}

    public class MS2DBManager
    {
        //public static List<MS2DBEntry> GetMS2DBEntry(string path)
        //{
        //    var engine = new FileHelperEngine<MS2DBEntryInput>();
        //    var records = engine.ReadFile(path);
        //    var ms2db_list = new List<MS2DBEntry>();

        //    for (int i = 1; i < records.Length; i++)
        //    {
        //        var record = records[i];
        //        List<RAW_PeakElement> ms2Spec = new List<RAW_PeakElement>();
        //        string[] mzStr = record.MS2mz.Split(";");
        //        string[] intStr = record.MS2int.Split(";");
        //        for (int j = 0; j < mzStr.Length; j++)
        //        {
        //            ms2Spec.Add(new RAW_PeakElement { Mz = Double.Parse(mzStr[j]), Intensity = Double.Parse(intStr[j]) });
        //        }
        //        bool validRT = true;
        //        double RT = 0;
        //        if (record.RTminute == "")
        //        {
        //            validRT = false;
        //        }
        //        else
        //        {
        //            RT = Double.Parse(record.RTminute);
        //        }
        //        ms2db_list.Add(new MS2DBEntry
        //        {
        //            //MS2DBName = DBName,
        //            MetaboliteName = record.MetaboliteName,
        //            DBNumberString = record.DBNumberString,
        //            InChIKey = record.InChIKey,
        //            Adduct = record.Adduct,
        //            PrecursorMz = Double.Parse(record.PrecursorMz),
        //            InstrumentType = record.InstrumentType,
        //            Instrument = record.Instrument,
        //            IonMode = record.IonMode,
        //            CollisionEnergy = record.CollisionEnergy,
        //            Formula = record.Formula,
        //            Comments = record.Comments,
        //            MS2Spec = ms2Spec,
        //            InChIKeyFirstHalf = record.InChIKeyFirstHalf,
        //            ValidRT = validRT,
        //            RTminute = RT
        //        }); ;
        //    }
        //    // removed, i = 1
        //    //ms2db_list.RemoveAt(0);
        //    return ms2db_list;
        //}

        public static List<MS2DBEntry> ReadMspMS2DB(string path)
        {
            List<MS2DBEntry> ms2db_list = new List<MS2DBEntry>();

            StreamReader reader = File.OpenText(path);

            string line;

            MS2DBEntry currEntry = new MS2DBEntry();
            string DBNumberStringStr = null;
            string InChIKeyStr = null;
            string AdductStr = null;
            double PrecursorMz = 0;
            string InstrumentTypeStr = null;
            string InstrumentStr = null;
            string IonModeStr = null;
            string CollisionEnergyStr = null;
            string FormulaStr = null;
            string CommentsStr = null;
            List<RAW_PeakElement> MS2Spec = null;
            bool ValidRT = false;
            double RTminute = 0;
            bool validEntry = false;
            bool ms2Line = false;
            bool FirstEntry = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (line == "")
                {
                    continue;
                }

                if (line.Contains("Name: ") || line.Contains("NAME: "))
                {
                    ms2Line = false;

                    // test whether this entry is valid
                    // precursormz : double; ms2spec count > 0; ion mode: P/N ; formula: parsable; inchikey: 27 characters
                    if (validEntry && FirstEntry == false && InChIKeyStr != null && FormulaStr != null && IonModeStr != null && MS2Spec != null)
                    {
                        currEntry.DBNumberString = DBNumberStringStr;
                        currEntry.InChIKey = InChIKeyStr;
                        currEntry.Adduct = AdductStr;
                        currEntry.PrecursorMz = PrecursorMz;
                        currEntry.InstrumentType = InstrumentTypeStr;
                        currEntry.Instrument = InstrumentStr;
                        currEntry.IonMode = IonModeStr;
                        currEntry.CollisionEnergy = CollisionEnergyStr;
                        currEntry.Formula = FormulaStr;
                        currEntry.Comments = CommentsStr;
                        currEntry.MS2Spec = MS2Spec;
                        currEntry.InChIKeyFirstHalf = InChIKeyStr.Substring(0, 14);
                        currEntry.ValidRT = ValidRT;
                        currEntry.RTminute = RTminute;

                        ms2db_list.Add(currEntry);

                        // debug
                        //Debug.WriteLine("DB total count: " + ms2db_list.Count);

                    }
                    FirstEntry = false;

                    //Debug.WriteLine("name: " + currEntry.MetaboliteName);

                    // a new MS2 entry
                    currEntry = new MS2DBEntry();
                    currEntry.MetaboliteName = line.Substring(6);
                    DBNumberStringStr = null;
                    InChIKeyStr = null;
                    AdductStr = null;
                    PrecursorMz = 0;
                    InstrumentTypeStr = null;
                    InstrumentStr = null;
                    IonModeStr = null;
                    CollisionEnergyStr = null;
                    FormulaStr = null;
                    CommentsStr = null;
                    MS2Spec = null;
                    ValidRT = false;
                    RTminute = 0;
                    validEntry = true;
                    continue;
                }

                if (ms2Line)
                {
                    string[] FragPairStr = null;
                    if (line.Contains(" "))
                    {
                        FragPairStr = line.Split(" ");
                    }
                    else if (line.Contains("\t"))
                    {
                        FragPairStr = line.Split("\t");
                    }

                    if (FragPairStr == null || FragPairStr.Length != 2)
                    {
                        continue;
                    }
                    else
                    {
                        MS2Spec.Add(new RAW_PeakElement { Mz = double.Parse(FragPairStr[0]), Intensity = double.Parse(FragPairStr[1]) });
                        continue;
                    }
                }

                if (line.Contains("DB#: "))
                {
                    DBNumberStringStr = line.Substring(5);
                    continue;
                }
                if (line.Contains("InChIKey: ") || line.Contains("INCHIKEY: "))
                {
                    InChIKeyStr = line.Substring(10);
                    if (InChIKeyStr.Length != 27)
                    {
                        validEntry = false;
                    }
                    continue;
                }
                if (line.Contains("Precursor_type: "))
                {
                    AdductStr = line.Substring(16);
                    continue;
                }
                if (line.Contains("PRECURSORTYPE: "))
                {
                    AdductStr = line.Substring(15);
                    continue;
                }
                if (line.Contains("PrecursorMZ: ") || line.Contains("PRECURSORMZ: "))
                {
                    if (double.TryParse(line.Substring(13), out PrecursorMz) == false)
                    {
                        validEntry = false;
                    }
                    continue;
                }
                if (line.Contains("Instrument_type: "))
                {
                    InstrumentTypeStr = line.Substring(17);
                    continue;
                }
                if (line.Contains("INSTRUMENTTYPE: "))
                {
                    InstrumentTypeStr = line.Substring(16);
                    continue;
                }
                if (line.Contains("Instrument: ") || line.Contains("INSTRUMENT: "))
                {
                    InstrumentStr = line.Substring(11);
                    continue;
                }
                if (line.Contains("Ion_mode: "))
                {
                    IonModeStr = line.Substring(10);
                    if (IonModeStr != "P" && IonModeStr != "N")
                    {
                        validEntry = false;
                    }
                    continue;
                }
                if (line.Contains("IONMODE: "))
                {
                    if (line.Substring(9) == "Positive")
                    {
                        IonModeStr = "P";
                    }
                    else if (line.Substring(9) == "Negative")
                    {
                        IonModeStr = "N";
                    }
                    else
                    {
                        validEntry = false;
                    }
                    continue;
                }
                if (line.Contains("Collision_energy: "))
                {
                    CollisionEnergyStr = line.Substring(18);
                    continue;
                }
                if (line.Contains("COLLISIONENERGY: "))
                {
                    CollisionEnergyStr = line.Substring(17);
                    continue;
                }
                if (line.Contains("Formula: ") || line.Contains("FORMULA: "))
                {
                    FormulaStr = line.Substring(9);
                    // when reading in MS2 database, we dont check for formula validility, we check it when doing global optimization (seed metabolites)
                    //var formCheck = FormulaValidilityCheck(FormulaStr);
                    //if (!formCheck.Item1)
                    //{
                    //    validEntry = false;
                    //}
                    //FormulaStr = formCheck.Item2;
                    continue;
                }
                if (line.Contains("Comment: "))
                {
                    CommentsStr = line.Substring(9);
                    continue;
                }
                if (line.Contains("Comments: "))
                {
                    CommentsStr = line.Substring(10);
                    continue;
                }
                if (line.Contains("RETENTIONTIME: "))
                {
                    if (double.TryParse(line.Substring(15), out RTminute))
                    {
                        ValidRT = true;
                    }
                    continue;
                }
                if (line.Contains("Num Peaks: "))
                {
                    ms2Line = true;
                    MS2Spec = new List<RAW_PeakElement>();
                    continue;
                }

            }

            if (validEntry && FirstEntry == false && InChIKeyStr != null && FormulaStr != null && IonModeStr != null && MS2Spec != null)
            {
                currEntry.DBNumberString = DBNumberStringStr;
                currEntry.InChIKey = InChIKeyStr;
                currEntry.Adduct = AdductStr;
                currEntry.PrecursorMz = PrecursorMz;
                currEntry.InstrumentType = InstrumentTypeStr;
                currEntry.Instrument = InstrumentStr;
                currEntry.IonMode = IonModeStr;
                currEntry.CollisionEnergy = CollisionEnergyStr;
                currEntry.Formula = FormulaStr;
                currEntry.Comments = CommentsStr;
                currEntry.MS2Spec = MS2Spec;
                currEntry.InChIKeyFirstHalf = InChIKeyStr.Substring(0, 14);
                currEntry.ValidRT = ValidRT;
                currEntry.RTminute = RTminute;

                ms2db_list.Add(currEntry);
            }
            //Debug.WriteLine("ms2DB: " + ms2db_list.Count);

            return ms2db_list;
        }

        public static Tuple<bool,string> FormulaValidilityCheck(string formula) // return bool, string: proton-adjusted formula
        {
            if (formula == "")
            {
                return new Tuple<bool, string>(false, "");
            }

            string[] elementStr = new string[] { "C", "H", "N", "O", "P", "F", "Cl", "Br", "I", "S", "Si", "B", "Se" };
            bool validFormula = true;

            try
            {
                IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(formula);
                //double mass1 = MolecularFormulaManipulator.GetMass(mf1, MolecularWeightTypes.MonoIsotopic);


                var ele = mf.Isotopes;
                foreach (var item in ele)
                {
                    if (!Array.Exists(elementStr, o => o == item.Symbol))
                    {
                        validFormula = false;
                        break;
                    }
                }

                int charge = 0;
                if (formula.Contains("+"))
                {
                    if (formula.Contains("]"))
                    {
                        if (formula.EndsWith("]+"))
                        {
                            charge = 1;
                        }
                        else if (int.TryParse(formula.Replace("+", "").Substring(formula.IndexOf("]") + 1), out charge) == false)
                        {
                            validFormula = false;
                        }
                    }
                    else
                    {
                        if (formula.EndsWith("+"))
                        {
                            charge = 1;
                        }
                        else if (int.TryParse(formula.Substring(formula.IndexOf("+")), out charge) == false)
                        {
                            validFormula = false;
                        }
                    }
                }
                else if (formula.Contains("-"))
                {
                    if (formula.Contains("]"))
                    {
                        int revCharge;
                        if (formula.EndsWith("]-"))
                        {
                            charge = -1;
                        }
                        else if (int.TryParse(formula.Replace("-", "").Substring(formula.IndexOf("]") + 1), out revCharge) == false)
                        {
                            validFormula = false;
                        }
                        else
                        {
                            charge = -1 * revCharge;
                        }
                    }
                    else
                    {
                        if (formula.EndsWith("-"))
                        {
                            charge = -1;
                        }
                        else if (int.TryParse(formula.Substring(formula.IndexOf("-")), out charge))
                        {
                            validFormula = false;
                        }
                    }
                }

                string newForm = formula;
                if (charge != 0)
                {
                    MolecularFormulaManipulator.AdjustProtonation(mf, -1 * charge);
                    newForm = MolecularFormulaManipulator.GetString(mf);
                }

                Tuple<bool, string> functionReturn = new Tuple<bool, string>(validFormula, newForm);
                return functionReturn;
            }
            catch
            {
                return new Tuple<bool, string>(false, "");
            }
        }
    }
}
