using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.Helper
{

    [Serializable]
    public class RadicalIonRatioUtility : INotifyPropertyChanged
    {
        public string type;
        public double radical;
        public double nonradical;

        public string Type
        {
            get { return type; }
            set { type = value; OnPropertyChanged("Type"); }
        }
        public double Radical
        {
            get { return radical; }
            set { radical = value; OnPropertyChanged("Radical"); }
        }
        public double NonRadical
        {
            get { return nonradical; }
            set { nonradical = value; OnPropertyChanged("NonRadical"); }
        }

        public RadicalIonRatioUtility(string type, double radical, double nonradical)
        {
            this.type = type;
            this.radical = radical;
            this.nonradical = nonradical;
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
