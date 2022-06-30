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
    public class FragmentModel : INotifyPropertyChanged
    {
        public FragmentModel()
        {
            this.PopulateData();
        }

        private ObservableCollection<FragmentUtility> fragments;
        public ObservableCollection<FragmentUtility> Fragments
        {
            get
            {
                return fragments;
            }
            set
            {
                fragments = value;
                RaisePropertyChanged("Fragments");
            }
        }

        public void PopulateData()
        {
            fragments = new ObservableCollection<FragmentUtility>();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
