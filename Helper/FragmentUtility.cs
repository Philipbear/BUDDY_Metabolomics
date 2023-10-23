using FileHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.Helper
{
    [DelimitedRecord(",")]
    public class FragmentUtility : INotifyPropertyChanged
    {
        int index;
        double mz;
        double abs_int;
        double rel_int;
        string formula;

        public FragmentUtility()
        { }

        public int Index
        {
            get { return index; }
            set { index = value; OnPropertyChanged("Index"); }
        }

        public double Mz
        {
            get { return mz; }
            set { mz = value; OnPropertyChanged("Mz"); }
        }

        public double Abs_int
        {
            get { return abs_int; }
            set { abs_int = value; OnPropertyChanged("Intensity"); }
        }

        public double Rel_int
        {
            get { return rel_int; }
            set { rel_int = value; OnPropertyChanged("Intensity"); }
        }

        public string Formula
        {
            get { return formula; }
            set { formula = value; OnPropertyChanged("Formula"); }
        }

        public FragmentUtility(int index, double mz, double abs_int, double rel_int, string formula)
        {
            this.index = index;
            this.mz = mz;
            this.abs_int = abs_int;
            this.rel_int = rel_int;
            this.formula = formula;

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
