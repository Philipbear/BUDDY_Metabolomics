using BUDDY.BufpHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY
{
    class FragDbeModel : INotifyPropertyChanged
    {
        public FragDbeModel()
        {
            fragDBEs = new ObservableCollection<CFDF>();
            fragDBEs.Add(new CFDF(5.0));
            fragDBEs.Add(new CFDF(5.0));
            fragDBEs.Add(new CFDF(5.0));
            fragDBEs.Add(new CFDF(4.5));
            fragDBEs.Add(new CFDF(4.5));
            fragDBEs.Add(new CFDF(4.0));
            fragDBEs.Add(new CFDF(3.0));
            fragDBEs.Add(new CFDF(3.0));
            fragDBEs.Add(new CFDF(3.0));
            fragDBEs.Add(new CFDF(2.0));
            fragDBEs.Add(new CFDF(1.0));
            fragDBEs.Add(new CFDF(0.0));
            fragDBEs.Add(new CFDF(-1.0));
            fragDBEs.Add(new CFDF(-1.0));
        }
        /// <summary>
        /// Gets or sets the orders details.
        /// </summary>
        /// <value>The orders details.</value>
        private ObservableCollection<CFDF> fragDBEs;
        public ObservableCollection<CFDF> FragDBEs
        {
            get
            {
                return fragDBEs;
            }
            set
            {
                fragDBEs = value;
                RaisePropertyChanged("FragDBEs");
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
