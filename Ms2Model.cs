using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY
{
    class Ms2Model : INotifyPropertyChanged
    {
        public Ms2Model()
        {
            this.PopulateData();
            //adducts = PolarityAdductRepository.GetAdduct();
            //PolarityList = new List<string>(Adducts.Keys);
        }
        /// <summary>
        /// Gets or sets the orders details.
        /// </summary>
        /// <value>The orders details.</value>
        private ObservableCollection<Ms2Utility> ms2s;
        public ObservableCollection<Ms2Utility> MS2s
        {
            get
            {
                return ms2s;
            }
            set
            {
                ms2s = value;
                RaisePropertyChanged("MS2s");
            }
        }

        //Dictionary<string, ObservableCollection<AdductDetails>> adducts;
        //public Dictionary<string, ObservableCollection<AdductDetails>> Adducts
        //{
        //    get
        //    {
        //        return adducts;
        //    }
        //    set
        //    {
        //        adducts = value;
        //        RaisePropertyChanged("Adducts");
        //    }
        //}

        //public List<string> PolarityList { get; set; }
        public void PopulateData()
        {
            ms2s = new ObservableCollection<Ms2Utility>();
            //ms2s.Add(new Ms2Utility(true, 1, "100.0104", "200", 001, "open.png", "P"));
            //ms2s.Add(new Ms2Utility(true, 2, "100.0104", "200", 001, "cancelled.png", "P"));
            //ms2s.Add(new Ms2Utility(false, 3, "100.0104", "200", 101, "complete.png", "N"));
            //ms2s.Add(new Ms2Utility(false, 4, "100.0104", "200", 101, "progress.png", "N"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
