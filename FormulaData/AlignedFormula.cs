using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.FormulaData
{
    [Serializable]
    [DelimitedRecord(",")]
    public class AlignedFormula
    {
        public string formstr_neutral { get; set; }
        public string formstr_charge { get; set; }

        public double mass { get; set; }
        public double dbe { get; set; }
        public int charge { get; set; }
        public int c { get; set; }
        public int h { get; set; }
        public int n { get; set; }
        public int o { get; set; }
        public int p { get; set; }
        public int f { get; set; }
        public int cl { get; set; }
        public int br { get; set; }
        public int i { get; set; }
        public int s { get; set; }
        public int si { get; set; }
        public int b { get; set; }
        public int se { get; set; }
        public int na { get; set; }
        public int k { get; set; }

        public int PubChem { get; set; }
        public int ANPDB { get; set; }
        public int BLEXP { get; set; }
        public int BMDB { get; set; }
        public int ChEBI { get; set; }
        public int COCONUT { get; set; }
        public int DrugBank { get; set; }
        public int DSSTOX { get; set; }
        public int ECMDB { get; set; }
        public int FooDB { get; set; }
        public int HMDB { get; set; }
        public int HSDB { get; set; }
        public int KEGG { get; set; }
        public int LMSD { get; set; }
        public int MaConDa { get; set; }
        public int MarkerDB { get; set; }
        public int MCDB { get; set; }
        public int NORMAN { get; set; }
        public int NPASS { get; set; }
        public int Plantcyc { get; set; }
        public int SMPDB { get; set; }
        public int STOFF_IDENT { get; set; }
        public int T3DB { get; set; }
        public int TTD { get; set; }
        public int UNPD { get; set; }
        public int YMDB { get; set; }
        public double LogFreq { get; set; }

    }

    [DelimitedRecord(",")]
    public class AlignedFormulaCSV
    {
        public string formstr_neutral { get; set; }
        public string mass { get; set; }
        public string dbe { get; set; }
        public string charge { get; set; }
        public string c { get; set; }
        public string h { get; set; }
        public string n { get; set; }
        public string o { get; set; }
        public string p { get; set; }
        public string f { get; set; }
        public string cl { get; set; }
        public string br { get; set; }
        public string i { get; set; }
        public string s { get; set; }
        public string si { get; set; }
        public string b { get; set; }
        public string se { get; set; }
        public string na { get; set; }
        public string k { get; set; }

        public string PubChem { get; set; }
        public string ANPDB { get; set; }
        public string BLEXP { get; set; }
        public string BMDB { get; set; }
        public string ChEBI { get; set; }
        public string COCONUT { get; set; }
        public string DrugBank { get; set; }
        public string DSSTOX { get; set; }
        public string ECMDB { get; set; }
        public string FooDB { get; set; }
        public string HMDB { get; set; }
        public string HSDB { get; set; }
        public string KEGG { get; set; }
        public string LMSD { get; set; }
        public string MaConDa { get; set; }
        public string MarkerDB { get; set; }
        public string MCDB { get; set; }
        public string NORMAN { get; set; }
        public string NPASS { get; set; }
        public string Plantcyc { get; set; }
        public string SMPDB { get; set; }
        public string STOFF_IDENT { get; set; }
        public string T3DB { get; set; }
        public string TTD { get; set; }
        public string UNPD { get; set; }
        public string YMDB { get; set; }
        public string LogFreq { get; set; }

    }

    public class AlignedFormulaManager
    {
        public static List<AlignedFormula> GetFormula(string path)
        {
            var engine = new FileHelperEngine<AlignedFormulaCSV>();
            var records = engine.ReadFile(path);
            var formula_list = new List<AlignedFormula>();

            foreach (var record in records.Skip(1))
            {
                formula_list.Add(new AlignedFormula
                {
                    formstr_neutral = record.formstr_neutral,
                    mass = Double.Parse(record.mass),
                    dbe = Double.Parse(record.dbe),
                    charge = Int32.Parse(record.charge),
                    c = Int32.Parse(record.c),
                    h = Int32.Parse(record.h),
                    n = Int32.Parse(record.n),
                    o = Int32.Parse(record.o),
                    p = Int32.Parse(record.p),
                    f = Int32.Parse(record.f),
                    cl = Int32.Parse(record.cl),
                    br = Int32.Parse(record.br),
                    i = Int32.Parse(record.i),
                    s = Int32.Parse(record.s),
                    si = Int32.Parse(record.si),
                    b = Int32.Parse(record.b),
                    se = Int32.Parse(record.se),
                    na = Int32.Parse(record.na),
                    k = Int32.Parse(record.k),

                    PubChem = Int32.Parse(record.PubChem),
                    ANPDB = Int32.Parse(record.ANPDB),
                    BLEXP = Int32.Parse(record.BLEXP),
                    BMDB = Int32.Parse(record.BMDB),
                    ChEBI = Int32.Parse(record.ChEBI),
                    COCONUT = Int32.Parse(record.COCONUT),
                    DrugBank = Int32.Parse(record.DrugBank),
                    DSSTOX = Int32.Parse(record.DSSTOX),
                    ECMDB = Int32.Parse(record.ECMDB),
                    FooDB = Int32.Parse(record.FooDB),
                    HMDB = Int32.Parse(record.HMDB),
                    HSDB = Int32.Parse(record.HSDB),
                    KEGG = Int32.Parse(record.KEGG),
                    LMSD = Int32.Parse(record.LMSD),
                    MaConDa = Int32.Parse(record.MaConDa),
                    MarkerDB = Int32.Parse(record.MarkerDB),
                    MCDB = Int32.Parse(record.MCDB),
                    NORMAN = Int32.Parse(record.NORMAN),
                    NPASS = Int32.Parse(record.NPASS),
                    Plantcyc = Int32.Parse(record.Plantcyc),
                    SMPDB = Int32.Parse(record.SMPDB),
                    STOFF_IDENT = Int32.Parse(record.STOFF_IDENT),
                    T3DB = Int32.Parse(record.T3DB),
                    TTD = Int32.Parse(record.TTD),
                    UNPD = Int32.Parse(record.UNPD),
                    YMDB = Int32.Parse(record.YMDB),
                    LogFreq = Double.Parse(record.LogFreq)
                });
            }
            //formula_list.RemoveAt(0);

            return formula_list;
        }

    }

    public class AlignedFormulaEasyVersion
    {
        public string formstr_neutral { get; set; }
        public string formstr_charge { get; set; }

        public double mass { get; set; }
        public double dbe { get; set; }
        public int charge { get; set; }
        public int c { get; set; }
        public int h { get; set; }
        public int n { get; set; }
        public int o { get; set; }
        public int p { get; set; }
        public int f { get; set; }
        public int cl { get; set; }
        public int br { get; set; }
        public int i { get; set; }
        public int s { get; set; }
        public int si { get; set; }
        public int b { get; set; }
        public int se { get; set; }
        public int na { get; set; }
        public int k { get; set; }
        public double LogFreq { get; set; }

    }
}
