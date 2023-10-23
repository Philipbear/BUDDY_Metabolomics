using NCDK;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BUDDY.FormulaData
{
    [Serializable]
    public class Adduct
    {
        public string Formula { get; set; } //full formula including square bracket
        public string Mode { get; set; }
        public int M { get; set; } //2 in 2M  [2M+H]+
        public int AbsCharge { get; set; } //absolute charge [M-2H]2-:2, [M+2H]2+:2
        public List<Formula> SubFormulae { get; set; }
        public Formula SumFormula { get; set; }
        public Formula SumFormulaNeg { get; set; }

        public Adduct(string formula)
        {
            // Function duplicated with the next line-----------------------------
            // string trimFormula = formula.Trim(); //trim leading and tailing whitespace
            string trimFormula = Regex.Replace(formula, @"\s+", ""); //remove all whitespace

            if (trimFormula == "[M]+")
            {
                this.Formula = trimFormula;
                trimFormula = "[M+H]+";
            }
            else
            {
                this.Formula = trimFormula;
            }

            string modePM = trimFormula.Substring(trimFormula.Length - 1); // + or - at end of formula
            if (modePM == "+")
            {
                this.Mode = "P";
            }
            else
            {
                this.Mode = "N";
            }

            this.AbsCharge = 1; // default: singly charged


            // Could be handled in one line ---------------------------------------
            // trimFormula = trimFormula.Remove(0, 1); //remove leading square bracket

            if (trimFormula.EndsWith("]+") || trimFormula.EndsWith("]-"))
            {
                // 0 would be the "[". So starting at 1.
                // 3 -> "[", "]", and "+/-"
                // -------------------------------------------------------
                trimFormula = trimFormula.Substring(1, trimFormula.Length - 3); //formula inside square bracket
            }
            else
            {
                int endBracketIndex = trimFormula.IndexOf("]");
                if (endBracketIndex > 1)
                {
                    // find absolute charge
                    string tmpForm = trimFormula.Substring(endBracketIndex + 1);
                    tmpForm = tmpForm.Replace("+", "");
                    tmpForm = tmpForm.Replace("-", "");

                    AbsCharge = int.Parse(tmpForm);

                    trimFormula = trimFormula.Substring(1, endBracketIndex - 1);
                }
            }




            // Get the subunit spliting by +/-
            // M+H-H2O -> M; +H ; -H2O
            string[] subFormulae = Regex.Split(trimFormula, @"(?=[+-])");

            string formulaSum = "";
            double massSum = 0.0;
            int cSum = 0;
            int hSum = 0;
            int nSum = 0;
            int oSum = 0;
            int pSum = 0;
            int fSum = 0;
            int clSum = 0;
            int brSum = 0;
            int iSum = 0;
            int sSum = 0;
            int siSum = 0;
            int bSum = 0;
            int seSum = 0;
            int naSum = 0;
            int kSum = 0;

            int cSumneg = 0;
            int hSumneg = 0;
            int nSumneg = 0;
            int oSumneg = 0;
            int pSumneg = 0;
            int fSumneg = 0;
            int clSumneg = 0;
            int brSumneg = 0;
            int iSumneg = 0;
            int sSumneg = 0;
            int siSumneg = 0;
            int bSumneg = 0;
            int seSumneg = 0;
            int naSumneg = 0;
            int kSumneg = 0;

            //if numeric preceeds M
            if (int.TryParse(subFormulae[0].Substring(0, 1), out int w))
            {
                this.M = int.Parse(Regex.Match(subFormulae[0].Substring(0), @"\d+").Value);
            }
            else
            {
                this.M = 1;
            }

            List<Formula> subFormList = new List<Formula>();
            for (int i = 1; i < subFormulae.Length; i++)
            {
                int multi = 1;
                int numLength = 0;
                //if subformula begins with a numeric
                if (int.TryParse(subFormulae[i].Substring(1, 1), out int k))
                {
                    multi = int.Parse(Regex.Match(subFormulae[i].Substring(1), @"\d+").Value);
                    numLength = multi.ToString().Length;
                }
                string currMode = subFormulae[i].Substring(0, 1); //+ or - of subformula
                IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(subFormulae[i].Substring(1 + numLength)); //subformula without mode or coefficient
                Formula currFormula;
                if (currMode == "+")
                {
                    currMode = "P";
                }
                else
                {
                    currMode = "N";
                    multi = multi * -1;
                }
                currFormula = new Formula
                {
                    formstr = subFormulae[i].Substring(1), //subformula including coefficient
                    mass = MolecularFormulaManipulator.GetMass(mf, MolecularWeightTypes.MostAbundant) * multi, //takes into consideration + or -
                    dbe = 0.0,
                    c = MolecularFormulaManipulator.GetElementCount(mf, "C") * multi,
                    h = MolecularFormulaManipulator.GetElementCount(mf, "H") * multi,
                    n = MolecularFormulaManipulator.GetElementCount(mf, "N") * multi,
                    o = MolecularFormulaManipulator.GetElementCount(mf, "O") * multi,
                    p = MolecularFormulaManipulator.GetElementCount(mf, "P") * multi,
                    f = MolecularFormulaManipulator.GetElementCount(mf, "F") * multi,
                    cl = MolecularFormulaManipulator.GetElementCount(mf, "Cl") * multi,
                    br = MolecularFormulaManipulator.GetElementCount(mf, "Br") * multi,
                    i = MolecularFormulaManipulator.GetElementCount(mf, "I") * multi,
                    s = MolecularFormulaManipulator.GetElementCount(mf, "S") * multi,
                    si = MolecularFormulaManipulator.GetElementCount(mf, "Si") * multi,
                    b = MolecularFormulaManipulator.GetElementCount(mf, "B") * multi,
                    se = MolecularFormulaManipulator.GetElementCount(mf, "Se") * multi,
                    na = MolecularFormulaManipulator.GetElementCount(mf, "Na") * multi,
                    k = MolecularFormulaManipulator.GetElementCount(mf, "K") * multi,
                    mode = currMode //+ or - of subformula
                };


                subFormList.Add(currFormula);

                formulaSum = formulaSum + currFormula.formstr;
                massSum += currFormula.mass; //accurate mass sum
                cSum += currFormula.c;
                hSum += currFormula.h;
                nSum += currFormula.n;
                oSum += currFormula.o;
                pSum += currFormula.p;
                fSum += currFormula.f;
                clSum += currFormula.cl;
                brSum += currFormula.br;
                iSum += currFormula.i;
                sSum += currFormula.s;
                siSum += currFormula.si;
                bSum += currFormula.b;
                seSum += currFormula.se;
                naSum += currFormula.na;
                kSum += currFormula.k;

                if (currFormula.mass < 0)
                {
                    cSumneg += currFormula.c;
                    hSumneg += currFormula.h;
                    nSumneg += currFormula.n;
                    oSumneg += currFormula.o;
                    pSumneg += currFormula.p;
                    fSumneg += currFormula.f;
                    clSumneg += currFormula.cl;
                    brSumneg += currFormula.br;
                    iSumneg += currFormula.i;
                    sSumneg += currFormula.s;
                    siSumneg += currFormula.si;
                    bSumneg += currFormula.b;
                    seSumneg += currFormula.se;
                    naSumneg += currFormula.na;
                    kSumneg += currFormula.k;
                }
            }
            this.SubFormulae = subFormList;
            if (this.Mode == "P")
            {
                massSum = massSum - 0.0005485 * this.AbsCharge;
            }
            else
            {
                massSum = massSum + 0.0005485 * this.AbsCharge;
            }
            this.SumFormula = new Formula
            {
                formstr = formulaSum,
                mass = massSum,
                dbe = 0.0,
                c = cSum,
                h = hSum,
                n = nSum,
                o = oSum,
                p = pSum,
                f = fSum,
                cl = clSum,
                br = brSum,
                i = iSum,
                s = sSum,
                si = siSum,
                b = bSum,
                se = seSum,
                na = naSum,
                k = kSum,
                mode = this.Mode
            };
            this.SumFormulaNeg = new Formula
            {
                formstr = "",
                mass = 0.0,
                dbe = 0.0,
                c = cSumneg,
                h = hSumneg,
                n = nSumneg,
                o = oSumneg,
                p = pSumneg,
                f = fSumneg,
                cl = clSumneg,
                br = brSumneg,
                i = iSumneg,
                s = sSumneg,
                si = siSumneg,
                b = bSumneg,
                se = seSumneg,
                na = naSumneg,
                k = kSumneg,
                mode = this.Mode
            };
        }
    }
}
