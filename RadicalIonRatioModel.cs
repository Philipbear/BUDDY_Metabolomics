using BUDDY.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY
{

    class RadicalIonRatioModel : INotifyPropertyChanged
    {
        public RadicalIonRatioModel()
        {
            radicalions = new ObservableCollection<RadicalIonRatioUtility>()
            {
                new RadicalIonRatioUtility("Count", 50, 100),
                new RadicalIonRatioUtility("Intensity", 100, 1000)
            };
        }


        private ObservableCollection<RadicalIonRatioUtility> radicalions;
        public ObservableCollection<RadicalIonRatioUtility> RadicalIons
        {
            get
            {
                return radicalions;
            }
            set
            {
                radicalions = value;
                RaisePropertyChanged("RadicalIons");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
