using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY
{
    [Serializable]
    public class FileUtility : INotifyPropertyChanged
    {
        bool selected;
        string fileName;
        string filePath;
        int index;
        string ms2number;
        List<Ms2Utility> ms2list { get; set; }
        public string imageLink;
        bool loaded;


        public FileUtility()
        { }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged("ProductId"); }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged("ProductId"); }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; OnPropertyChanged("ProductId"); }
        }

        public int Index
        {
            get { return index; }
            set { index = value; OnPropertyChanged("Index"); }
        }
        public string Ms2number
        {
            get { return ms2number; }
            set { ms2number = value; OnPropertyChanged("Ms2number"); }
        }
        public List<Ms2Utility> MS2List
        {
            get { return ms2list; }
            set { ms2list = value; OnPropertyChanged("MS2List"); }
        }
        public string ImageLink
        {
            get { return imageLink; }
            set { imageLink = value; OnPropertyChanged("ImageLink"); }
        }
        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; OnPropertyChanged("ProductId"); }
        }
        public FileUtility(bool selected, string fileName, string filePath, int index, string ms2number, bool loaded)
        {
            this.selected = selected;
            this.fileName = fileName;
            this.filePath = filePath;
            this.index = index;
            this.ms2number = ms2number;
            this.loaded = loaded;
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
