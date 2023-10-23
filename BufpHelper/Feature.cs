using BUDDY.FormulaData;
using FileHelpers;
using NCDK.Formula;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.BufpHelper
{
    [Serializable]
    [DelimitedRecord(",")]
    public class Feature
    {
        public Feature() //17-21
        {
            //UInt32 ind;
            //string p_formula; // precursor
            //double p_dbe;
            //double p_h2c;
            //double p_hetero2c;
            //double p_mzErrorRatio;
            //double expfNoRatio;
            //double expfIntRatio;
            //double wf_DBE; // weighted average
            //double wl_DBE;
            //double wfl_DBE;
            //double wf_H2C;
            //double wl_H2C;
            //double wfl_H2C;
            //double wf_Hetero2C;
            //double wl_Hetero2C;
            //double wfl_Hetero2C;
            //double waf_mzErrorRatio;
            //int ionmode;
            //int fragno;
            //double f_nonintDBENoRatio;
            //double mztol;
            //double fDBEsd;
            //double lDBEsd;
            //double flDBEsd;
            //double fH2Csd;
            //double lH2Csd;
            //double flH2Csd;
            //double fHetero2Csd;
            //double lHetero2Csd;
            //double flHetero2Csd;
            //double ispsim;
            //IsotopePattern theoMS1;
            //int relevance;
            //List<CFDF> cfdf;
            //double waf_logFreq;
            //double wal_logFreq;
            //double halogenAtomRatio;
            //double cAtomRatio;
            //double choAtomRatio;
            //double chnoAtomRatio;
            //double chnopsAtomRatio;
            //int choOnly;
            //int chnoOnly;
            //int chnopsOnly;
            //double atomNo2Mass;
            //AlignedFormula alignedFormula;
            //double score;
        }
        public Feature(UInt32 index, string preFormula, double preDBE, double preH2C, double preHetero2C, double preMzErrorRatio, //1-5
            double explainedFragNoRatio, double explainedFragIntRatio, //6-7
            double walpFragDBE,
            double walpLossDBE,
            double walpFragLossDBE,
            double walpFragH2C,
            double walpLossH2C,
            double walpFragLossH2C,
            double walpFragHetero2C,
            double walpLossHetero2C,
            double walpFragLossHetero2C, //8-16
            double waFragMzErrorRatio, int ionMode, int fragNo, double fragNonIntegerDBENoRatio, double mzTol,
            double fragDBEsd, double lossDBEsd, double fraglossDBEsd,
            double fragH2Csd, double lossH2Csd, double fraglossH2Csd,
            double fragHetero2Csd, double lossHetero2Csd, double fraglossHetero2Csd,
            double ispSim, IsotopePattern TheoMS1, int Relevance, List<CFDF> CFDF, double waFrag_LogFreq, double waLoss_LogFreq,
            double halogenNoRatio, double cNumRaio, double choNumRatio,
            double chnoNumRatio, double chnopsNumRatio, int choOnlyBool, int chnoOnlyBool, int chnopsOnlyBool, double atomNum2Mass, int modelTypeNo) //17-21
        {
            ind = index;
            p_formula = preFormula;
            p_dbe = preDBE;
            p_h2c = preH2C;
            p_hetero2c = preHetero2C;
            p_mzErrorRatio = preMzErrorRatio;
            expfNoRatio = explainedFragNoRatio;
            expfIntRatio = explainedFragIntRatio;
            wf_DBE = walpFragDBE;
            wl_DBE = walpLossDBE;
            wfl_DBE = walpFragLossDBE;
            wf_H2C = walpFragH2C;
            wl_H2C = walpLossH2C;
            wfl_H2C = walpFragLossH2C;
            wf_Hetero2C = walpFragHetero2C;
            wl_Hetero2C = walpLossHetero2C;
            wfl_Hetero2C = walpFragLossHetero2C;
            waf_mzErrorRatio = waFragMzErrorRatio;
            ionmode = ionMode;
            fragno = fragNo;
            f_nonintDBENoRatio = fragNonIntegerDBENoRatio;
            mztol = mzTol;
            fDBEsd = fragDBEsd;
            lDBEsd = lossDBEsd;
            flDBEsd = fraglossDBEsd;
            fH2Csd = fragH2Csd;
            lH2Csd = lossH2Csd;
            flH2Csd = fraglossH2Csd;
            fHetero2Csd = fragHetero2Csd;
            lHetero2Csd = lossHetero2Csd;
            flHetero2Csd = fraglossHetero2Csd;
            ispsim = ispSim;
            theoMS1 = TheoMS1;
            relevance = Relevance;
            cfdf = CFDF;
            waf_logFreq = waFrag_LogFreq;
            wal_logFreq = waLoss_LogFreq;
            halogenAtomRatio = halogenNoRatio;
            cAtomRatio = cNumRaio;
            choAtomRatio = choNumRatio;
            chnoAtomRatio = chnoNumRatio;
            chnopsAtomRatio = chnopsNumRatio;
            choOnly = choOnlyBool;
            chnoOnly = chnoOnlyBool;
            chnopsOnly = chnopsOnlyBool;
            atomNo2Mass = atomNum2Mass;
            modelType = modelTypeNo;
        }
        public UInt32 ind { get; set; }
        public string p_formula { get; set; } // neutral formula

        public double p_dbe { get; set; }
        public double p_h2c { get; set; }
        public double p_hetero2c { get; set; }
        public double p_mzErrorRatio { get; set; }
        public double expfNoRatio { get; set; }
        public double expfIntRatio { get; set; }
        public double wf_DBE { get; set; }
        public double wl_DBE { get; set; }
        public double wfl_DBE { get; set; }
        public double wf_H2C { get; set; }
        public double wl_H2C { get; set; }
        public double wfl_H2C { get; set; }
        public double wf_Hetero2C { get; set; }
        public double wl_Hetero2C { get; set; }
        public double wfl_Hetero2C { get; set; }
        public double waf_mzErrorRatio { get; set; }
        public int ionmode { get; set; }
        public int fragno { get; set; }
        public double f_nonintDBENoRatio { get; set; }
        public double mztol { get; set; }
        public double fDBEsd { get; set; }
        public double lDBEsd { get; set; }
        public double flDBEsd { get; set; }
        public double fH2Csd { get; set; }
        public double lH2Csd { get; set; }
        public double flH2Csd { get; set; }
        public double fHetero2Csd { get; set; }
        public double lHetero2Csd { get; set; }
        public double flHetero2Csd { get; set; }
        public double ispsim { get; set; }
        public IsotopePattern theoMS1 { get; set; }

        public int relevance { get; set; }  // ranking within the candidates, results obtained by plattProb
        public double MILPScore { get; set; }
        public int VariableIndexInMILP { get; set; }
        public List<CFDF> cfdf { get; set; }
        public double waf_logFreq { get; set; }
        public double wal_logFreq { get; set; }
        public AlignedFormula alignedFormula { get; set; }
        public double score { get; set; } // MLR score

        public double halogenAtomRatio { get; set; }
        public double cAtomRatio { get; set; }
        public double choAtomRatio { get; set; }
        public double chnoAtomRatio { get; set; }
        public double chnopsAtomRatio { get; set; }
        public int choOnly { get; set; }
        public int chnoOnly { get; set; }
        public int chnopsOnly { get; set; }
        public double atomNo2Mass { get; set; }
        public int modelType { get; set; }
        // modeType: 1: pos, HR, MS1; 2: pos, HR, noMS1; 3: neg, HR, MS1; 4: neg, HR, noMS1
        //           5: pos, LR, MS1; 6: pos, LR, noMS1; 7: neg, LR, MS1; 8: neg, LR, noMS1
        public double plattProb { get; set; }
        public List<string> fragmentFormulaList { get; set; } // this list only contains spectrum fragments (other than precursor, isotope, noise peaks)
        public List<bool> fragmentReannotated { get; set; } // whether fragment has gone thru reannotation, this list only contains spectrum fragments (other than precursor, isotope, noise peaks)
        public bool fragmentReannotatedCompleted { get; set; } // overall bool for this candidate
        public bool accessedBefore { get; set; }

        // list of chemical elements
        public FormulaElement formulaElement { get; set; }
    }
    public class FormulaElement
    {
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
        public FormulaElement(AlignedFormula form)
        {
            this.c = form.c;
            this.h = form.h;
            this.n = form.n;
            this.o = form.o;
            this.p = form.p;
            this.f = form.f;
            this.cl = form.cl;
            this.br = form.br;
            this.i = form.i;
            this.s = form.s;
            this.si = form.si;
            this.b = form.b;
            this.se = form.se;
            this.na = form.na;
            this.k = form.k;
        }
        public FormulaElement (){}
    }


    [Serializable]
    [DelimitedRecord(",")]
    public class MlModel
    {
        public MlModel() //17-21
        {
            UInt32 ind;
            string p_formula;
            double p_dbe;
            double p_h2c;
            double p_hetero2c;
            double p_mzErrorRatio;
            double expfNoRatio;
            double expfIntRatio;
            double wf_DBE;
            double wl_DBE;
            double wfl_DBE;
            double wf_H2C;
            double wl_H2C;
            double wfl_H2C;
            double wf_Hetero2C;
            double wl_Hetero2C;
            double wfl_Hetero2C;
            double waf_mzErrorRatio;
            int ionmode;
            int fragno;
            double f_nonintDBENoRatio;
            double mztol;
            double fDBEsd;
            double lDBEsd;
            double flDBEsd;
            double fH2Csd;
            double lH2Csd;
            double flH2Csd;
            double fHetero2Csd;
            double lHetero2Csd;
            double flHetero2Csd;
            double ispsim;
            int relevance;
            double waf_logFreq;
            double wal_logFreq;
            double halogenAtomRatio;
            double cAtomRatio;
            double choAtomRatio;
            double chnoAtomRatio;
            double chnopsAtomRatio;
            int choOnly;
            int chnoOnly;
            int chnopsOnly;
            double atomNo2Mass;
        }
        public MlModel(UInt32 index, string preFormula, double preDBE, double preH2C, double preHetero2C, double preMzErrorRatio, //1-5
            double explainedFragNoRatio, double explainedFragIntRatio, //6-7
            double walpFragDBE,
            double walpLossDBE,
            double walpFragLossDBE,
            double walpFragH2C,
            double walpLossH2C,
            double walpFragLossH2C,
            double walpFragHetero2C,
            double walpLossHetero2C,
            double walpFragLossHetero2C, //8-16
            double waFragMzErrorRatio, int ionMode, int fragNo, double fragNonIntegerDBENoRatio, double mzTol,
            double fragDBEsd, double lossDBEsd, double fraglossDBEsd,
            double fragH2Csd, double lossH2Csd, double fraglossH2Csd,
            double fragHetero2Csd, double lossHetero2Csd, double fraglossHetero2Csd,
            double ispSim, int Relevance, double waFrag_LogFreq, double waLoss_LogFreq, double halogenNumRatio, double cNumRaio, double choNumRatio,
            double chnoNumRatio, double chnopsNumRatio, int choOnlyBool, int chnoOnlyBool, int chnopsOnlyBool, double atomNum2Mass) //17-21
        {
            ind = index;
            p_formula = preFormula;
            p_dbe = preDBE;
            p_h2c = preH2C;
            p_hetero2c = preHetero2C;
            p_mzErrorRatio = preMzErrorRatio;
            expfNoRatio = explainedFragNoRatio;
            expfIntRatio = explainedFragIntRatio;
            wf_DBE = walpFragDBE;
            wl_DBE = walpLossDBE;
            wfl_DBE = walpFragLossDBE;
            wf_H2C = walpFragH2C;
            wl_H2C = walpLossH2C;
            wfl_H2C = walpFragLossH2C;
            wf_Hetero2C = walpFragHetero2C;
            wl_Hetero2C = walpLossHetero2C;
            wfl_Hetero2C = walpFragLossHetero2C;
            waf_mzErrorRatio = waFragMzErrorRatio;
            ionmode = ionMode;
            fragno = fragNo;
            f_nonintDBENoRatio = fragNonIntegerDBENoRatio;
            mztol = mzTol;
            fDBEsd = fragDBEsd;
            lDBEsd = lossDBEsd;
            flDBEsd = fraglossDBEsd;
            fH2Csd = fragH2Csd;
            lH2Csd = lossH2Csd;
            flH2Csd = fraglossH2Csd;
            fHetero2Csd = fragHetero2Csd;
            lHetero2Csd = lossHetero2Csd;
            flHetero2Csd = fraglossHetero2Csd;
            ispsim = ispSim;
            relevance = Relevance;
            waf_logFreq = waFrag_LogFreq;
            wal_logFreq = waLoss_LogFreq;
            halogenAtomRatio = halogenNumRatio;
            cAtomRatio = cNumRaio;
            choAtomRatio = choNumRatio;
            chnoAtomRatio = chnoNumRatio;
            chnopsAtomRatio = chnopsNumRatio;
            choOnly = choOnlyBool;
            chnoOnly = chnoOnlyBool;
            chnopsOnly = chnopsOnlyBool;
            atomNo2Mass = atomNum2Mass;
        }
        public UInt32 ind { get; set; }
        public string p_formula { get; set; }

        [LoadColumn(0)]
        public double p_dbe { get; set; }
        [LoadColumn(1)]
        public double p_h2c { get; set; }
        [LoadColumn(2)]
        public double p_hetero2c { get; set; }
        [LoadColumn(3)]
        public double p_mzErrorRatio { get; set; }
        [LoadColumn(4)]
        public double expfNoRatio { get; set; }
        [LoadColumn(5)]
        public double expfIntRatio { get; set; }
        [LoadColumn(6)]
        public double wf_DBE { get; set; }
        [LoadColumn(7)]
        public double wl_DBE { get; set; }
        [LoadColumn(8)]
        public double wfl_DBE { get; set; }
        [LoadColumn(9)]
        public double wf_H2C { get; set; }
        [LoadColumn(10)]
        public double wl_H2C { get; set; }
        [LoadColumn(11)]
        public double wfl_H2C { get; set; }
        [LoadColumn(12)]
        public double wf_Hetero2C { get; set; }
        [LoadColumn(13)]
        public double wl_Hetero2C { get; set; }
        [LoadColumn(14)]
        public double wfl_Hetero2C { get; set; }
        [LoadColumn(15)]
        public double waf_mzErrorRatio { get; set; }
        [LoadColumn(16)]
        public int ionmode { get; set; }
        [LoadColumn(17)]
        public int fragno { get; set; }
        [LoadColumn(18)]
        public double f_nonintDBENoRatio { get; set; }
        [LoadColumn(19)]
        public double mztol { get; set; }
        [LoadColumn(20)]
        public double fDBEsd { get; set; }
        [LoadColumn(21)]
        public double lDBEsd { get; set; }
        [LoadColumn(22)]
        public double flDBEsd { get; set; }
        [LoadColumn(23)]
        public double fH2Csd { get; set; }
        [LoadColumn(24)]
        public double lH2Csd { get; set; }
        [LoadColumn(25)]
        public double flH2Csd { get; set; }
        [LoadColumn(26)]
        public double fHetero2Csd { get; set; }
        [LoadColumn(27)]
        public double lHetero2Csd { get; set; }
        [LoadColumn(28)]
        public double flHetero2Csd { get; set; }
        [LoadColumn(29)]
        public double ispsim { get; set; }
        [LoadColumn(30)]
        [ColumnName("Label_int")]
        public int relevance { get; set; }
        [LoadColumn(31)]
        public double waf_logFreq { get; set; }
        [LoadColumn(32)]
        public double wal_logFreq { get; set; }
        [LoadColumn(33)]
        public double halogenAtomRatio { get; set; }
        [LoadColumn(34)]
        public double cAtomRatio { get; set; }
        [LoadColumn(35)]
        public double choAtomRatio { get; set; }
        [LoadColumn(36)]
        public double chnoAtomRatio { get; set; }
        [LoadColumn(37)]
        public double chnopsAtomRatio { get; set; }
        [LoadColumn(38)]
        public int choOnly { get; set; }
        [LoadColumn(39)]
        public int chnoOnly { get; set; }
        [LoadColumn(40)]
        public int chnopsOnly { get; set; }
        [LoadColumn(41)]
        public double atomNo2Mass { get; set; }
    }


    public class ValidFeature
    {
        public ValidFeature() //17-21
        {
            int ind;
            string formula;
            double expfNoRatio;
            double expfIntRatio;
            int relevance;
            double p_mz_error;
        }
        public ValidFeature(int index, string Formula, double explainedFragNoRatio, double explainedFragIntRatio, int Relevance, double MzError)
        {
            ind = index;
            formula = Formula;
            expfNoRatio = explainedFragNoRatio;
            expfIntRatio = explainedFragIntRatio;
            relevance = Relevance;
            p_mz_error = MzError;
        }
        public int ind { get; set; }
        public string formula { get; set; }
        public double expfNoRatio { get; set; }
        public double expfIntRatio { get; set; }
        public int relevance { get; set; }
        public double p_mz_error { get; set; }
    }
}
