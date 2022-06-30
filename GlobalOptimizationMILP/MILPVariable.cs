using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BUDDY.BufpHelper;
using BUDDY.FormulaData;
using BUDDY.RawData;
using NCDK;
using NCDK.Tools.Manipulator;

namespace BUDDY.GlobalOptimizationMILP
{
    public class MILPVariable
    {
        //public int VariableIndex { get; set; }
        public bool Node { get; set; } // node: feature; edge: connection
        // for node:
        public int MetaboliteFeatureIndex { get; set; }
        public int CandidateIndex { get; set; } // candidate index within metabolite feature

        public double PlattProb { get; set; }
        public string NodeCandidateFormula { get; set; }

        // for edge:
        public int EdgeMetaboliteFeatureIndexA { get; set; }
        public int EdgeCandidateIndexA { get; set; } // candidate index in this metabolite feature
        public int EdgeCandidateTotalIndexA { get; set; } // cummulated index in MILP variable (for edge generation)
        public string EdgeCandidateFormulaA { get; set; } // neutral formula -- M
        public int EdgeMetaboliteFeatureIndexB { get; set; }
        public int EdgeCandidateIndexB { get; set; }
        public int EdgeCandidateTotalIndexB { get; set; } // cummulated index in MILP variable
        public string EdgeCandidateFormulaB { get; set; }
        public bool AbioticConnection { get; set; } // if abiotic connection: RT diff should be within RTtol
        public bool RtMatched { get; set; }
        public Connection EdgeConnection { get; set; }
        public double MS2SimilarityScore { get; set; } // GNPS

        // returns formula diff string, bool: whether A is a subformula of B (for potential ISF determination)
        public static FormulaDiffOutput FormulaCompare(string formulaA, string formulaB, FormulaElement formEleA, FormulaElement formEleB, bool AMzLarger) // bool: whether A has larger Mz, for formula subtraction
        {
            if (formEleA == null || formEleB == null) { return null; }

            if (formulaA == formulaB)
            {
                return new FormulaDiffOutput() { formulaSame = true, formulaDiff = "", subForm = false };
            }

            int[] formDiff = new int[15];
            if (AMzLarger)
            {
                formDiff[0] = formEleA.c - formEleB.c;
                formDiff[1] = formEleA.h - formEleB.h;
                formDiff[2] = formEleA.b - formEleB.b;
                formDiff[3] = formEleA.br - formEleB.br;
                formDiff[4] = formEleA.cl - formEleB.cl;
                formDiff[5] = formEleA.f - formEleB.f;
                formDiff[6] = formEleA.i - formEleB.i;
                formDiff[7] = formEleA.k - formEleB.k;
                formDiff[8] = formEleA.n - formEleB.n;
                formDiff[9] = formEleA.na - formEleB.na;
                formDiff[10] = formEleA.o - formEleB.o;
                formDiff[11] = formEleA.p - formEleB.p;
                formDiff[12] = formEleA.s - formEleB.s;
                formDiff[13] = formEleA.se - formEleB.se;
                formDiff[14] = formEleA.si - formEleB.si;
            }
            else
            {
                formDiff[0] = formEleB.c - formEleA.c;
                formDiff[1] = formEleB.h - formEleA.h;
                formDiff[2] = formEleB.b - formEleA.b;
                formDiff[3] = formEleB.br - formEleA.br;
                formDiff[4] = formEleB.cl - formEleA.cl;
                formDiff[5] = formEleB.f - formEleA.f;
                formDiff[6] = formEleB.i - formEleA.i;
                formDiff[7] = formEleB.k - formEleA.k;
                formDiff[8] = formEleB.n - formEleA.n;
                formDiff[9] = formEleB.na - formEleA.na;
                formDiff[10] = formEleB.o - formEleA.o;
                formDiff[11] = formEleB.p - formEleA.p;
                formDiff[12] = formEleB.s - formEleA.s;
                formDiff[13] = formEleB.se - formEleA.se;
                formDiff[14] = formEleB.si - formEleA.si;
            }
            bool subForm = true;
            for (int i = 0; i < 15; i++)
            {
                if (formDiff[i] < 0)
                {
                    subForm = false;
                    break;
                }
            }

            string outStr = "";
            string[] elementStr = new string[] { "C", "H", "B", "Br", "Cl", "F", "I", "K", "N", "Na", "O", "P", "S", "Se", "Si" };
            for (int i = 0; i < 15; i++)
            {
                if (formDiff[i] != 0)
                {
                    outStr = outStr + elementStr[i] + formDiff[i];
                }
            }

            //IMolecularFormula mfA = MolecularFormulaManipulator.GetMolecularFormula(formulaA);
            //IMolecularFormula mfB = MolecularFormulaManipulator.GetMolecularFormula(formulaB);
            //if (mfA == null || mfB == null) { return null; }

            //string[] elementStr = new string[] { "C", "H", "B", "Br", "Cl", "F", "I", "K", "N", "Na", "O", "P", "S", "Se", "Si" };

            //int[] formA = new int[15];
            //int[] formB = new int[15];
            //int[] formDiff = new int[15];

            //for (int i = 0; i < 15; i++)
            //{
            //    formA[i] = MolecularFormulaManipulator.GetElementCount(mfA, elementStr[i]);
            //    formB[i] = MolecularFormulaManipulator.GetElementCount(mfB, elementStr[i]);
            //}

            //if (AMzLarger)
            //{
            //    for (int i = 0; i < 15; i++)
            //    {
            //        formDiff[i] = formA[i] - formB[i];
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < 15; i++)
            //    {
            //        formDiff[i] = formB[i] - formA[i];
            //    }
            //}

            //bool subForm = true;
            //for (int i = 0; i < 15; i++)
            //{
            //    if (formDiff[i] < 0)
            //    {
            //        subForm = false;
            //        break;
            //    }
            //}

            //string outStr = "";
            //for (int i = 0; i < 15; i++)
            //{
            //    if (formDiff[i] != 0)
            //    {
            //        outStr = outStr + elementStr[i] + formDiff[i];
            //    }
            //}
            return new FormulaDiffOutput() { formulaSame = false, formulaDiff = outStr, subForm = subForm };
        }
        //FormulaStrToList 
        public static FormulaElement FormulaParse(string formula)
        {
            FormulaElement output = new FormulaElement();
            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(formula);
            if (mf == null) { return null; }

            output.c = MolecularFormulaManipulator.GetElementCount(mf, "C");
            output.h = MolecularFormulaManipulator.GetElementCount(mf, "H");
            output.b = MolecularFormulaManipulator.GetElementCount(mf, "B");
            output.br = MolecularFormulaManipulator.GetElementCount(mf, "Br");
            output.cl = MolecularFormulaManipulator.GetElementCount(mf, "Cl");
            output.f = MolecularFormulaManipulator.GetElementCount(mf, "F");
            output.i = MolecularFormulaManipulator.GetElementCount(mf, "I");
            output.k = MolecularFormulaManipulator.GetElementCount(mf, "K");
            output.n = MolecularFormulaManipulator.GetElementCount(mf, "N");
            output.na = MolecularFormulaManipulator.GetElementCount(mf, "Na");
            output.o = MolecularFormulaManipulator.GetElementCount(mf, "O");
            output.p = MolecularFormulaManipulator.GetElementCount(mf, "P");
            output.s = MolecularFormulaManipulator.GetElementCount(mf, "S");
            output.se = MolecularFormulaManipulator.GetElementCount(mf, "Se");
            output.si = MolecularFormulaManipulator.GetElementCount(mf, "Si");

            return output;
        }
        public static bool MS2FragContain(double mzA, double mzB, List<RAW_PeakElement> ms2A, List<RAW_PeakElement> ms2B, bool AMzLarger, bool ppm, double ms1tol, double ms2tol, double intThreshold) // >= Max*intThreshold
        {
            bool returnBool = false;
            double allowedMS2Deviation = ms1tol + ms2tol;
            if (AMzLarger)
            {
                if (ms2A != null && ms2A.Count > 0)
                {
                    double maxInt = ms2A.Max(o => o.Intensity);
                    for (int m = 0; m < ms2A.Count; m++)
                    {
                        if (ppm)
                        {
                            allowedMS2Deviation = mzB * ms1tol * 1e-6 + ms2A[m].Mz * ms2tol * 1e-6;
                        }
                        if (Math.Abs(mzB - ms2A[m].Mz) <= allowedMS2Deviation && ms2A[m].Intensity >= maxInt * intThreshold)
                        {
                            returnBool = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (ms2B != null && ms2B.Count > 0)
                {
                    double maxInt = ms2B.Max(o => o.Intensity);
                    for (int m = 0; m < ms2B.Count; m++)
                    {
                        if (ppm)
                        {
                            allowedMS2Deviation = mzA * ms1tol * 1e-6 + ms2B[m].Mz * ms2tol * 1e-6;
                        }
                        if (Math.Abs(mzA - ms2B[m].Mz) <= allowedMS2Deviation && ms2B[m].Intensity >= maxInt * intThreshold)
                        {
                            returnBool = true;
                            break;
                        }
                    }
                }
            }
            return returnBool;
        }
    }
    public class FormulaDiffOutput
    {
        public bool formulaSame { get; set; } // whether formula are exactly the same
        public string formulaDiff { get; set; }
        public bool subForm { get; set; } // for in-source fragment
    }
    public class MILPFeaturePair
    {
        public int AFeatureIndex { get; set; }
        public int BFeatureIndex { get; set; }
        public bool RtMatched { get; set; }
        public bool FeatureAMzLarger { get; set; }
        public bool bothValidMS2 { get; set; }
        public double GnpsScore { get; set; }
        public double RdpScore { get; set; }
        public bool ValidPair { get; set; } // if both validMS2, GNPS >= 0.6 or (rDP >= 0.8 && rtMatched)
    }
}
