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

    class FeatureConnectionModel : INotifyPropertyChanged
    {
        public FeatureConnectionModel()
        {
            connections = new ObservableCollection<FeatureConnectionUtility>();
        }
        /// <summary>
        /// Gets or sets the orders details.
        /// </summary>
        /// <value>The orders details.</value>
        private ObservableCollection<FeatureConnectionUtility> connections;
        public ObservableCollection<FeatureConnectionUtility> Connections
        {
            get
            {
                return connections;
            }
            set
            {
                connections = value;
                RaisePropertyChanged("FeatureConnections");
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
