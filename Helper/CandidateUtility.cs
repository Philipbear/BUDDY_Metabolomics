using BUDDY.BufpHelper;
using BUDDY.FormulaData;
using NCDK.Formula;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.Helper
{
    class CandidateUtility : INotifyPropertyChanged
    {
        public int rank;
        public string formula;
        public double mzerror;
        public double expfragnum;
        public double expfragsumint;
        public double wafragmzerror;
        public double mlrScore;
        public double estimatedProbability;
        public double estimatedFDR;
        public double ispsim;
        public IsotopePattern theoMS1;
        public List<CFDF> cfdf;
        public AlignedFormula alignedFormula;

        public CandidateUtility()
        { }

        public int Rank
        {
            get { return rank; }
            set { rank = value; OnPropertyChanged("Rank"); }
        }

        public string Formula
        {
            get { return formula; }
            set { formula = value; OnPropertyChanged("Formula"); }
        }

        public double MzError
        {
            get { return mzerror; }
            set { mzerror = value; OnPropertyChanged("MzError"); }
        }
        public double ExpFragNum
        {
            get { return expfragnum; }
            set { expfragnum = value; OnPropertyChanged("ExpFragNum"); }
        }
        public double ExpFragSumInt
        {
            get { return expfragsumint; }
            set { expfragsumint = value; OnPropertyChanged("ExpFragSumInt"); }
        }
        public double IspSim
        {
            get { return ispsim; }
            set { ispsim = value; OnPropertyChanged("IspSim"); }
        }
        public double WaFragMzError
        {
            get { return wafragmzerror; }
            set { wafragmzerror = value; OnPropertyChanged("WaFragMzError"); }
        }
        public double EstimatedProbability
        {
            get { return estimatedProbability; }
            set { estimatedProbability = value; OnPropertyChanged("EstimatedProbability"); }
        }
        public double MLRScore
        {
            get { return mlrScore; }
            set { mlrScore = value; OnPropertyChanged("MLRScore"); }
        }
        public double EstimatedFDR
        {
            get { return estimatedFDR; }
            set { estimatedFDR = value; OnPropertyChanged("EstimatedFDR"); }
        }
        public IsotopePattern TheoMS1
        {
            get { return theoMS1; }
            set { theoMS1 = value; OnPropertyChanged("TheoMS1"); }
        }
        public List<CFDF> CFDF
        {
            get { return cfdf; }
            set { cfdf = value; OnPropertyChanged("CFDF"); }
        }
        public AlignedFormula AlignedFormula
        {
            get { return alignedFormula; }
            set { alignedFormula = value; OnPropertyChanged("AlignedFormula"); }
        }

        public CandidateUtility(int rank, string formula, double mzerror, double expfragnum, double expfragsumint, 
            double wafragmzerror, double estimatedProbability, double estimatedFDR, double ispsim, IsotopePattern theoMS1, List<CFDF> cfdf, AlignedFormula alignedFormula)
        {
            this.rank = rank;
            this.formula = formula;
            this.mzerror = mzerror;
            this.expfragnum = expfragnum;
            this.expfragsumint = expfragsumint;
            this.wafragmzerror = wafragmzerror;
            this.estimatedProbability = estimatedProbability;
            this.estimatedFDR = estimatedFDR;
            this.ispsim = ispsim;
            this.theoMS1 = theoMS1;
            this.cfdf = cfdf;
            this.alignedFormula = alignedFormula;
        }

        public CandidateUtility(int rank, string formula, double mzerror, double expfragnum, double expfragsumint, double wafragmzerror, double estimatedProbability, 
            double estimatedFDR, double ispsim, IsotopePattern theoMS1, List<CFDF> cfdf, AlignedFormula alignedFormula, double mlrScore)
        {
            this.rank = rank;
            this.formula = formula;
            this.mzerror = mzerror;
            this.expfragnum = expfragnum;
            this.expfragsumint = expfragsumint;
            this.wafragmzerror = wafragmzerror;
            this.estimatedProbability = estimatedProbability;
            this.estimatedFDR = estimatedFDR;
            this.ispsim = ispsim;
            this.theoMS1 = theoMS1;
            this.cfdf = cfdf;
            this.alignedFormula = alignedFormula;
            this.mlrScore = mlrScore;
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
}
