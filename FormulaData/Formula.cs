using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.FormulaData
{
    [DelimitedRecord(",")]
    [Serializable]
    public class Formula
    {
        public string formstr { get; set; }
        public double mass { get; set; }
        public double dbe { get; set; }
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
        public string mode { get; set; } //mode for adduct + -
    }

    [DelimitedRecord(",")]
    public class FormulaCSV
    {
        public string formstr { get; set; }
        public string mass { get; set; }
        public string dbe { get; set; }
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
        public string mode { get; set; }

    }

    public class FormulaManager
    {
        public static List<Formula> GetFormula(string path)
        {
            var engine = new FileHelperEngine<FormulaCSV>();
            var records = engine.ReadFile(path);
            var formula_list = new List<Formula>();

            foreach (var record in records)
            {
                formula_list.Add(new Formula
                {
                    formstr = record.formstr,
                    mass = Double.Parse(record.mass),
                    dbe = Double.Parse(record.dbe),
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
                    mode = ""
                });
            }
            //formula_list.RemoveAt(0);

            return formula_list;
        }

    }
}
