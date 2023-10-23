using BUDDY.GlobalOptimizationMILP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.Helper
{
    class FeatureConnectionUtility : INotifyPropertyChanged
    {
        public int pairedMs2ScanNumber { get; set; }
        public string pairedMs2MetaboliteName { get; set; }
        public string pairedMs2Formula { get; set; }
        public double ms2Similarity { get; set; }
        public double pairedMs2RT { get; set; }
        public string connectionType { get; set; }
        public string formulaChange { get; set; }
        public double massDiff { get; set; }
        public string description { get; set; }

        public FeatureConnectionUtility()
        { }


        public int PairedMs2ScanNumber
        {
            get { return pairedMs2ScanNumber; }
            set { pairedMs2ScanNumber = value; OnPropertyChanged("PairedMs2ScanNumber"); }
        }
        public string PairedMs2MetaboliteName
        {
            get { return pairedMs2MetaboliteName; }
            set { pairedMs2MetaboliteName = value; OnPropertyChanged("PairedMs2MetaboliteName"); }
        }
        public string PairedMs2Formula
        {
            get { return pairedMs2Formula; }
            set { pairedMs2Formula = value; OnPropertyChanged("PairedMs2Formula"); }
        }
        public double Ms2Similarity
        {
            get { return ms2Similarity; }
            set { ms2Similarity = value; OnPropertyChanged("Ms2Similarity"); }
        }
        public double PairedMs2RT
        {
            get { return pairedMs2RT; }
            set { pairedMs2RT = value; OnPropertyChanged("PairedMs2RT"); }
        }
        public string ConnectionType
        {
            get { return connectionType; }
            set { connectionType = value; OnPropertyChanged("ConnectionType"); }
        }
        public string FormulaChange
        {
            get { return formulaChange; }
            set { formulaChange = value; OnPropertyChanged("FormulaChange"); }
        }
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged("Description"); }
        }
        public double MassDiff
        {
            get { return massDiff; }
            set { massDiff = value; OnPropertyChanged("MassDiff"); }
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
