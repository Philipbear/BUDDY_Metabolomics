using FileHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDDY.FormulaData
{
    [DelimitedRecord(",")]
    public class Database : INotifyPropertyChanged
    {
        string name;
        string icon;

        public Database()
        { }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public string Icon
        {
            get { return icon; }
            set { icon = value; OnPropertyChanged("Icon"); }
        }

        public Database(string dbName)
        {

            #region
            if (dbName == "PubChem_include")
            {
                this.Name = "PubChem";
                this.Icon = "ms-appx:///Image/pubchem.png";
            }
            else if (dbName == "ANPDB_include")
            {
                this.Name = "ANPDB";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "BLEXP_include")
            {
                this.Name = "BLEXP";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "BMDB_include")
            {
                this.Name = "BMDB";
                this.Icon = "ms-appx:///Image/bmdb.png";
            }
            else if (dbName == "ChEBI_include")
            {
                this.Name = "ChEBI";
                this.Icon = "ms-appx:///Image/chebi.png";
            }
            else if (dbName == "COCONUT_include")
            {
                this.Name = "COCONUT";
                this.Icon = "ms-appx:///Image/coconut.png";
            }
            else if (dbName == "DrugBank_include")
            {
                this.Name = "DrugBank";
                this.Icon = "ms-appx:///Image/drugbank.png";
            }
            else if (dbName == "DSSTOX_include")
            {
                this.Name = "DSSTOX";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "ECMDB_include")
            {
                this.Name = "ECMDB";
                this.Icon = "ms-appx:///Image/ecmbd.png";
            }
            else if (dbName == "FooDB_include")
            {
                this.Name = "FooDB";
                this.Icon = "ms-appx:///Image/foodb.png";
            }
            else if (dbName == "HMDB_include")
            {
                this.Name = "HMDB";
                this.Icon = "ms-appx:///Image/hmdb.png";
            }
            else if (dbName == "HSDB_include")
            {
                this.Name = "HSDB";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "KEGG_include")
            {
                this.Name = "KEGG";
                this.Icon = "ms-appx:///Image/kegg.png";
            }
            else if (dbName == "LMSD_include")
            {
                this.Name = "LMSD";
                this.Icon = "ms-appx:///Image/lmsd.png";
            }
            else if (dbName == "MaConDa_include")
            {
                this.Name = "MaConDa";
                this.Icon = "ms-appx:///Image/maconda.png";
            }
            else if (dbName == "MarkerDB_include")
            {
                this.Name = "MarkerDB";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "MCDB_include")
            {
                this.Name = "MCDB";
                this.Icon = "ms-appx:///Image/mcdb.png";
            }
            else if (dbName == "NORMAN_include")
            {
                this.Name = "NORMAN";
                this.Icon = "ms-appx:///Image/norman.png";
            }
            else if (dbName == "NPASS_include")
            {
                this.Name = "NPASS";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "Plantcyc_include")
            {
                this.Name = "Plantcyc";
                this.Icon = "ms-appx:///Image/plantcyc.png";
            }
            else if (dbName == "SMPDB_include")
            {
                this.Name = "SMPDB";
                this.Icon = "ms-appx:///Image/smpdb.png";
            }
            else if (dbName == "STF_IDENT_include")
            {
                this.Name = "STF-IDENT";
                this.Icon = "ms-appx:///Image/stoffident.png";
            }
            else if (dbName == "T3DB_include")
            {
                this.Name = "T3DB";
                this.Icon = "ms-appx:///Image/t3db.png";
            }
            else if (dbName == "TTD_include")
            {
                this.Name = "TTD";
                this.Icon = "ms-appx:///Image/ttd.png";
            }
            else if (dbName == "UNPD_include")
            {
                this.Name = "UNPD";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            else if (dbName == "YMDB_include")
            {
                this.Name = "YMDB";
                this.Icon = "ms-appx:///Image/genericDB.png";
            }
            #endregion


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
