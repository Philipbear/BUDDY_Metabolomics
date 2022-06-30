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
    class CandidateModel : INotifyPropertyChanged
    {
        public CandidateModel()
        {
            candidates = new ObservableCollection<CandidateUtility>();
        }
        /// <summary>
        /// Gets or sets the orders details.
        /// </summary>
        /// <value>The orders details.</value>
        private ObservableCollection<CandidateUtility> candidates;
        public ObservableCollection<CandidateUtility> Candidates
        {
            get
            {
                return candidates;
            }
            set
            {
                candidates = value;
                RaisePropertyChanged("Candidates");
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
