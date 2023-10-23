using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.Helper
{

    [Serializable]
    public class SaveProjectUtility : INotifyPropertyChanged
    {
        public ObservableCollection<FileUtility> filelist { get; set; }
        //public ObservableCollection<Ms2Utility> ms2list { get; set; }

        public ObservableCollection<FileUtility> FileList
        {
            get { return filelist; }
            set { filelist = value; OnPropertyChanged("FileList"); }
        }
        //public ObservableCollection<Ms2Utility> Ms2List
        //{
        //    get { return ms2list; }
        //    set { ms2list = value; OnPropertyChanged("Ms2List"); }
        //}

        //public SaveProjectUtility(ObservableCollection<FileUtility> filelist, ObservableCollection<Ms2Utility> ms2list)
        //{
        //    this.filelist = filelist;
        //    this.ms2list = ms2list;
        //}
        public SaveProjectUtility(ObservableCollection<FileUtility> filelist)
        {
            this.filelist = filelist;
        }


        public SaveProjectUtility() { }

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
