using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BUDDY
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicSetting : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        bool throwInvalidInputErrorBasicSettings = false;
        public BasicSetting()
        {
            this.InitializeComponent();
            this.Loaded += BasicSetting_Page_Loaded;

            MS2LibrarySearch_checkbox.IsChecked = (bool)localSettings.Values["MS2LibrarySearch_include"];
            BUDDY_checkbox.IsChecked = (bool)localSettings.Values["BUDDY_include"];
            ExpSpecificGlobalAnnotation_checkbox.IsChecked = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include"];
            //mass tol
            ms_instrument_selection.SelectedIndex = (int)localSettings.Values["selectedMSinstrument"];
            ms1tol_box.Text = localSettings.Values["ms1tol"].ToString();
            ms2tol_box.Text = localSettings.Values["ms2tol"].ToString();
            ms1tol_switch.IsOn = (bool)localSettings.Values["ms1tol_ppmON"];
            ms2tol_switch.IsOn = (bool)localSettings.Values["ms2tol_ppmON"];
            //raw data processing
            //ExpSpecificGlobalAnnotation_checkbox.IsChecked = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include"];
            //MetaScore_checkbox.IsChecked = (bool)localSettings.Values["MetaScore_include"];
            ////timeout
            //timeout_single_checkbox.IsChecked = (bool)localSettings.Values["timeout_single_include"];
            //timeout_single_box.Text = localSettings.Values["timeout_single"].ToString();
            //formula database
            noResDB_checkbox.IsChecked = (bool)localSettings.Values["NoResDB_include"];
            pubchem_checkbox.IsChecked = (bool)localSettings.Values["PubChem_include"];
            mtb_lpd_checkbox.IsChecked = (bool)localSettings.Values["mtb_lpd_include"];
            BMDB_checkbox.IsChecked = (bool)localSettings.Values["BMDB_include"];
            ChEBI_checkbox.IsChecked = (bool)localSettings.Values["ChEBI_include"];
            ECMDB_checkbox.IsChecked = (bool)localSettings.Values["ECMDB_include"];
            FooDB_checkbox.IsChecked = (bool)localSettings.Values["FooDB_include"];
            HMDB_checkbox.IsChecked = (bool)localSettings.Values["HMDB_include"];
            KEGG_checkbox.IsChecked = (bool)localSettings.Values["KEGG_include"];
            LMSD_checkbox.IsChecked = (bool)localSettings.Values["LMSD_include"];
            MarkerDB_checkbox.IsChecked = (bool)localSettings.Values["MarkerDB_include"];
            MCDB_checkbox.IsChecked = (bool)localSettings.Values["MCDB_include"];
            PlantCyc_checkbox.IsChecked = (bool)localSettings.Values["PlantCyc_include"];
            SMPDB_checkbox.IsChecked = (bool)localSettings.Values["SMPDB_include"];
            YMDB_checkbox.IsChecked = (bool)localSettings.Values["YMDB_include"];
            drug_txn_checkbox.IsChecked = (bool)localSettings.Values["drug_txn_include"];
            DrugBank_checkbox.IsChecked = (bool)localSettings.Values["DrugBank_include"];
            DSSTOX_checkbox.IsChecked = (bool)localSettings.Values["DSSTOX_include"];
            HSDB_checkbox.IsChecked = (bool)localSettings.Values["HSDB_include"];
            T3DB_checkbox.IsChecked = (bool)localSettings.Values["T3DB_include"];
            TTD_checkbox.IsChecked = (bool)localSettings.Values["TTD_include"];
            natProducts_checkbox.IsChecked = (bool)localSettings.Values["natProducts_include"];
            ANPDB_checkbox.IsChecked = (bool)localSettings.Values["ANPDB_include"];
            COCONUT_checkbox.IsChecked = (bool)localSettings.Values["COCONUT_include"];
            NPASS_checkbox.IsChecked = (bool)localSettings.Values["NPASS_include"];
            UNPD_checkbox.IsChecked = (bool)localSettings.Values["UNPD_include"];
            xbiotics_checkbox.IsChecked = (bool)localSettings.Values["xbiotics_include"];
            BLEXP_checkbox.IsChecked = (bool)localSettings.Values["BLEXP_include"];
            NORMAN_checkbox.IsChecked = (bool)localSettings.Values["NORMAN_include"];
            STF_IDENT_checkbox.IsChecked = (bool)localSettings.Values["STF_IDENT_include"];
            contaminants_checkbox.IsChecked = (bool)localSettings.Values["contaminants_include"];
            MaConDa_checkbox.IsChecked = (bool)localSettings.Values["MaConDa_include"];
        }

        private void BasicSetting_Page_Loaded(object sender, RoutedEventArgs e)
        {
            var s = ApplicationView.GetForCurrentView();
            s.TryResizeView(new Windows.Foundation.Size { Width = 850.0, Height = 800.0 });
        }
        private void BSC_Restore(object sender, RoutedEventArgs e)
        {
            ExpSpecificGlobalAnnotation_checkbox.IsChecked = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include_default"];
            MS2LibrarySearch_checkbox.IsChecked = (bool)localSettings.Values["MS2LibrarySearch_include_default"];
            BUDDY_checkbox.IsChecked = (bool)localSettings.Values["BUDDY_include_default"];

            ms_instrument_selection.SelectedIndex = (int)localSettings.Values["selectedMSinstrument_default"];
            ms1tol_box.Text = localSettings.Values["ms1tol_default"].ToString();
            ms2tol_box.Text = localSettings.Values["ms2tol_default"].ToString();
            ms1tol_switch.IsOn = (bool)localSettings.Values["ms1tol_ppmON_default"];
            ms2tol_switch.IsOn = (bool)localSettings.Values["ms2tol_ppmON_default"];
            //ExpSpecificGlobalAnnotation_checkbox.IsChecked = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include_default"];
            //MetaScore_checkbox.IsChecked = (bool)localSettings.Values["MetaScore_include_default"];
            //timeout_single_checkbox.IsChecked = (bool)localSettings.Values["timeout_single_include_default"];
            //timeout_single_box.Text = localSettings.Values["timeout_single_default"].ToString();

            noResDB_checkbox.IsChecked = (bool)localSettings.Values["NoResDB_include_default"];
            pubchem_checkbox.IsChecked = (bool)localSettings.Values["PubChem_include_default"];
            mtb_lpd_checkbox.IsChecked = (bool)localSettings.Values["mtb_lpd_include_default"];
            BMDB_checkbox.IsChecked = (bool)localSettings.Values["BMDB_include_default"];
            ChEBI_checkbox.IsChecked = (bool)localSettings.Values["ChEBI_include_default"];
            ECMDB_checkbox.IsChecked = (bool)localSettings.Values["ECMDB_include_default"];
            FooDB_checkbox.IsChecked = (bool)localSettings.Values["FooDB_include_default"];
            HMDB_checkbox.IsChecked = (bool)localSettings.Values["HMDB_include_default"];
            KEGG_checkbox.IsChecked = (bool)localSettings.Values["KEGG_include_default"];
            LMSD_checkbox.IsChecked = (bool)localSettings.Values["LMSD_include_default"];
            MarkerDB_checkbox.IsChecked = (bool)localSettings.Values["MarkerDB_include_default"];
            MCDB_checkbox.IsChecked = (bool)localSettings.Values["MCDB_include_default"];
            PlantCyc_checkbox.IsChecked = (bool)localSettings.Values["PlantCyc_include_default"];
            SMPDB_checkbox.IsChecked = (bool)localSettings.Values["SMPDB_include_default"];
            YMDB_checkbox.IsChecked = (bool)localSettings.Values["YMDB_include_default"];
            drug_txn_checkbox.IsChecked = (bool)localSettings.Values["drug_txn_include_default"];
            DrugBank_checkbox.IsChecked = (bool)localSettings.Values["DrugBank_include_default"];
            DSSTOX_checkbox.IsChecked = (bool)localSettings.Values["DSSTOX_include_default"];
            HSDB_checkbox.IsChecked = (bool)localSettings.Values["HSDB_include_default"];
            T3DB_checkbox.IsChecked = (bool)localSettings.Values["T3DB_include_default"];
            TTD_checkbox.IsChecked = (bool)localSettings.Values["TTD_include_default"];
            natProducts_checkbox.IsChecked = (bool)localSettings.Values["natProducts_include_default"];
            ANPDB_checkbox.IsChecked = (bool)localSettings.Values["ANPDB_include_default"];
            COCONUT_checkbox.IsChecked = (bool)localSettings.Values["COCONUT_include_default"];
            NPASS_checkbox.IsChecked = (bool)localSettings.Values["NPASS_include_default"];
            UNPD_checkbox.IsChecked = (bool)localSettings.Values["UNPD_include_default"];
            xbiotics_checkbox.IsChecked = (bool)localSettings.Values["xbiotics_include_default"];
            BLEXP_checkbox.IsChecked = (bool)localSettings.Values["BLEXP_include_default"];
            NORMAN_checkbox.IsChecked = (bool)localSettings.Values["NORMAN_include_default"];
            STF_IDENT_checkbox.IsChecked = (bool)localSettings.Values["STF_IDENT_include_default"];
            contaminants_checkbox.IsChecked = (bool)localSettings.Values["contaminants_include_default"];
            MaConDa_checkbox.IsChecked = (bool)localSettings.Values["MaConDa_include_default"];

        }
        private void BSC_Apply(object sender, RoutedEventArgs e)
        {
            throwInvalidInputErrorBasicSettings = false;

            localSettings.Values["ExpSpecificGlobalAnnotation_include"] = ExpSpecificGlobalAnnotation_checkbox.IsChecked;
            localSettings.Values["BUDDY_include"] = BUDDY_checkbox.IsChecked;
            localSettings.Values["MS2LibrarySearch_include"] = MS2LibrarySearch_checkbox.IsChecked;
            //mass tolerance
            localSettings.Values["selectedMSinstrument"] = ms_instrument_selection.SelectedIndex;

            double ms1tol;
            if (double.TryParse(ms1tol_box.Text, out ms1tol))
            {
                if(ms1tol > 0)
                {
                    localSettings.Values["ms1tol"] = ms1tol;
                }
                else
                {
                    throwInvalidInputErrorBasicSettings = true;
                    //return;
                }
            }
            else
            {
                throwInvalidInputErrorBasicSettings = true;
                //return;
            }

            double ms2tol;
            if (double.TryParse(ms2tol_box.Text, out ms2tol))
            {
                if (ms2tol > 0)
                {
                    localSettings.Values["ms2tol"] = ms2tol;
                }
                else
                {
                    throwInvalidInputErrorBasicSettings = true;
                    //return;
                }
            }
            else
            {
                throwInvalidInputErrorBasicSettings = true;
                //return;
            }

            localSettings.Values["ms1tol_ppmON"] = ms1tol_switch.IsOn;
            localSettings.Values["ms2tol_ppmON"] = ms2tol_switch.IsOn;
            //formula database
            localSettings.Values["NoResDB_include"] = noResDB_checkbox.IsChecked;
            localSettings.Values["PubChem_include"] = pubchem_checkbox.IsChecked;
            localSettings.Values["mtb_lpd_include"] = mtb_lpd_checkbox.IsChecked;
            localSettings.Values["BMDB_include"] = BMDB_checkbox.IsChecked;
            localSettings.Values["ChEBI_include"] = ChEBI_checkbox.IsChecked;
            localSettings.Values["ECMDB_include"] = ECMDB_checkbox.IsChecked;
            localSettings.Values["FooDB_include"] = FooDB_checkbox.IsChecked;
            localSettings.Values["HMDB_include"] = HMDB_checkbox.IsChecked;
            localSettings.Values["KEGG_include"] = KEGG_checkbox.IsChecked;
            localSettings.Values["LMSD_include"] = LMSD_checkbox.IsChecked;
            localSettings.Values["MarkerDB_include"] = MarkerDB_checkbox.IsChecked;
            localSettings.Values["MCDB_include"] = MCDB_checkbox.IsChecked;
            localSettings.Values["PlantCyc_include"] = PlantCyc_checkbox.IsChecked;
            localSettings.Values["SMPDB_include"] = SMPDB_checkbox.IsChecked;
            localSettings.Values["YMDB_include"] = YMDB_checkbox.IsChecked;
            localSettings.Values["drug_txn_include"] = drug_txn_checkbox.IsChecked;
            localSettings.Values["DrugBank_include"] = DrugBank_checkbox.IsChecked;
            localSettings.Values["DSSTOX_include"] = DSSTOX_checkbox.IsChecked;
            localSettings.Values["HSDB_include"] = HSDB_checkbox.IsChecked;
            localSettings.Values["T3DB_include"] = T3DB_checkbox.IsChecked;
            localSettings.Values["TTD_include"] = TTD_checkbox.IsChecked;
            localSettings.Values["natProducts_include"] = natProducts_checkbox.IsChecked;
            localSettings.Values["ANPDB_include"] = ANPDB_checkbox.IsChecked;
            localSettings.Values["COCONUT_include"] = COCONUT_checkbox.IsChecked;
            localSettings.Values["NPASS_include"] = NPASS_checkbox.IsChecked;
            localSettings.Values["UNPD_include"] = UNPD_checkbox.IsChecked;
            localSettings.Values["xbiotics_include"] = xbiotics_checkbox.IsChecked;
            localSettings.Values["BLEXP_include"] = BLEXP_checkbox.IsChecked;
            localSettings.Values["NORMAN_include"] = NORMAN_checkbox.IsChecked;
            localSettings.Values["STF_IDENT_include"] = STF_IDENT_checkbox.IsChecked;
            localSettings.Values["contaminants_include"] = contaminants_checkbox.IsChecked;
            localSettings.Values["MaConDa_include"] = MaConDa_checkbox.IsChecked;

            if (noResDB_checkbox.IsChecked == false && pubchem_checkbox.IsChecked == false && BMDB_checkbox.IsChecked == false && ChEBI_checkbox.IsChecked == false &&
                ECMDB_checkbox.IsChecked == false && FooDB_checkbox.IsChecked == false && HMDB_checkbox.IsChecked == false && KEGG_checkbox.IsChecked == false && LMSD_checkbox.IsChecked == false &&
                MarkerDB_checkbox.IsChecked == false && MCDB_checkbox.IsChecked == false && PlantCyc_checkbox.IsChecked == false && SMPDB_checkbox.IsChecked == false && YMDB_checkbox.IsChecked == false &&
                DrugBank_checkbox.IsChecked == false && DSSTOX_checkbox.IsChecked == false && HSDB_checkbox.IsChecked == false && T3DB_checkbox.IsChecked == false &&
                TTD_checkbox.IsChecked == false && ANPDB_checkbox.IsChecked == false && COCONUT_checkbox.IsChecked == false && NPASS_checkbox.IsChecked == false &&
                UNPD_checkbox.IsChecked == false && BLEXP_checkbox.IsChecked == false && NORMAN_checkbox.IsChecked == false && STF_IDENT_checkbox.IsChecked == false &&
                MaConDa_checkbox.IsChecked == false)
            {
                throwInvalidInputErrorBasicSettings = true;
            }

            ////raw data processing
            //localSettings.Values["ExpSpecificGlobalAnnotation_include"] = ExpSpecificGlobalAnnotation_checkbox.IsChecked;
            //localSettings.Values["MetaScore_include"] = MetaScore_checkbox.IsChecked;
            ////timeout
            //localSettings.Values["timeout_single_include"] = timeout_single_checkbox.IsChecked;

            //double timeout = 0.0;
            //if (double.TryParse(timeout_single_box.Text, out timeout))
            //{
            //    if (timeout > 0)
            //    {
            //        localSettings.Values["timeout_single"] = timeout;
            //    }
            //    else
            //    {
            //        throwInvalidInputErrorBasicSettings = true;
            //        //return;
            //    }
            //}
            //else
            //{
            //    throwInvalidInputErrorBasicSettings = true;
            //    //return;
            //}

            if (throwInvalidInputErrorBasicSettings)
            {
                InvalidInputError();
            }
            else
            {
                Window.Current.Close();
            }
        }
        private void BSC_Cancel(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }
        private void ExpSpecificGlobalAnnotation_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (ExpSpecificGlobalAnnotation_checkbox.IsChecked == true)
            {
                BUDDY_checkbox.IsChecked = true;
            }
        }
        private void BUDDY_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (BUDDY_checkbox.IsChecked == false)
            {
                ExpSpecificGlobalAnnotation_checkbox.IsChecked = false;
            }
        }
        private void noResDB_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (noResDB_checkbox.IsChecked == true)
            {
                pubchem_checkbox.IsChecked = false;
                mtb_lpd_checkbox.IsChecked = false;
                drug_txn_checkbox.IsChecked = false;
                natProducts_checkbox.IsChecked = false;
                xbiotics_checkbox.IsChecked = false;
                contaminants_checkbox.IsChecked = false;
                BMDB_checkbox.IsChecked = false;
                ChEBI_checkbox.IsChecked = false;
                ECMDB_checkbox.IsChecked = false;
                FooDB_checkbox.IsChecked = false;
                HMDB_checkbox.IsChecked = false;
                KEGG_checkbox.IsChecked = false;
                LMSD_checkbox.IsChecked = false;
                MarkerDB_checkbox.IsChecked = false;
                MCDB_checkbox.IsChecked = false;
                PlantCyc_checkbox.IsChecked = false;
                SMPDB_checkbox.IsChecked = false;
                YMDB_checkbox.IsChecked = false;
                DrugBank_checkbox.IsChecked = false;
                DSSTOX_checkbox.IsChecked = false;
                HSDB_checkbox.IsChecked = false;
                T3DB_checkbox.IsChecked = false;
                TTD_checkbox.IsChecked = false;
                ANPDB_checkbox.IsChecked = false;
                COCONUT_checkbox.IsChecked = false;
                NPASS_checkbox.IsChecked = false;
                UNPD_checkbox.IsChecked = false;
                BLEXP_checkbox.IsChecked = false;
                NORMAN_checkbox.IsChecked = false;
                STF_IDENT_checkbox.IsChecked = false;
                MaConDa_checkbox.IsChecked = false;
            }
            else
            {
                pubchem_checkbox.IsChecked = true;
                mtb_lpd_checkbox.IsChecked = true;
                BMDB_checkbox.IsChecked = true;
                ChEBI_checkbox.IsChecked = true;
                ECMDB_checkbox.IsChecked = true;
                FooDB_checkbox.IsChecked = true;
                HMDB_checkbox.IsChecked = true;
                KEGG_checkbox.IsChecked = true;
                LMSD_checkbox.IsChecked = true;
                MarkerDB_checkbox.IsChecked = true;
                MCDB_checkbox.IsChecked = true;
                PlantCyc_checkbox.IsChecked = true;
                SMPDB_checkbox.IsChecked = true;
                YMDB_checkbox.IsChecked = true;
            }
        }
        private void pubchem_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (pubchem_checkbox.IsChecked == false &&
                mtb_lpd_checkbox.IsChecked == false &&
                drug_txn_checkbox.IsChecked == false &&
                natProducts_checkbox.IsChecked == false &&
                xbiotics_checkbox.IsChecked == false &&
                contaminants_checkbox.IsChecked == false)
            {
                noResDB_checkbox.IsChecked = true;
            }
            if (pubchem_checkbox.IsChecked == true)
            {
                noResDB_checkbox.IsChecked = false;
            }
        }

        private void mtb_lpd_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (mtb_lpd_checkbox.IsChecked == true)
            {
                BMDB_checkbox.IsChecked = true;
                ChEBI_checkbox.IsChecked = true;
                ECMDB_checkbox.IsChecked = true;
                FooDB_checkbox.IsChecked = true;
                HMDB_checkbox.IsChecked = true;
                KEGG_checkbox.IsChecked = true;
                LMSD_checkbox.IsChecked = true;
                MarkerDB_checkbox.IsChecked = true;
                MCDB_checkbox.IsChecked = true;
                PlantCyc_checkbox.IsChecked = true;
                SMPDB_checkbox.IsChecked = true;
                YMDB_checkbox.IsChecked = true;
                noResDB_checkbox.IsChecked = false;
            }
            else
            {
                BMDB_checkbox.IsChecked = false;
                ChEBI_checkbox.IsChecked = false;
                ECMDB_checkbox.IsChecked = false;
                FooDB_checkbox.IsChecked = false;
                HMDB_checkbox.IsChecked = false;
                KEGG_checkbox.IsChecked = false;
                LMSD_checkbox.IsChecked = false;
                MarkerDB_checkbox.IsChecked = false;
                MCDB_checkbox.IsChecked = false;
                PlantCyc_checkbox.IsChecked = false;
                SMPDB_checkbox.IsChecked = false;
                YMDB_checkbox.IsChecked = false;
            }
            if (pubchem_checkbox.IsChecked == false &&
                mtb_lpd_checkbox.IsChecked == false &&
                drug_txn_checkbox.IsChecked == false &&
                natProducts_checkbox.IsChecked == false &&
                xbiotics_checkbox.IsChecked == false &&
                contaminants_checkbox.IsChecked == false)
            {
                noResDB_checkbox.IsChecked = true;
            }
        }

        private void drug_txn_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (drug_txn_checkbox.IsChecked == true)
            {
                DrugBank_checkbox.IsChecked = true;
                DSSTOX_checkbox.IsChecked = true;
                HSDB_checkbox.IsChecked = true;
                T3DB_checkbox.IsChecked = true;
                TTD_checkbox.IsChecked = true;
                noResDB_checkbox.IsChecked = false;
            }
            else
            {
                DrugBank_checkbox.IsChecked = false;
                DSSTOX_checkbox.IsChecked = false;
                HSDB_checkbox.IsChecked = false;
                T3DB_checkbox.IsChecked = false;
                TTD_checkbox.IsChecked = false;
            }
            if (pubchem_checkbox.IsChecked == false &&
                mtb_lpd_checkbox.IsChecked == false &&
                drug_txn_checkbox.IsChecked == false &&
                natProducts_checkbox.IsChecked == false &&
                xbiotics_checkbox.IsChecked == false &&
                contaminants_checkbox.IsChecked == false)
            {
                noResDB_checkbox.IsChecked = true;
            }
        }

        private void natProducts_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (natProducts_checkbox.IsChecked == true)
            {
                ANPDB_checkbox.IsChecked = true;
                COCONUT_checkbox.IsChecked = true;
                NPASS_checkbox.IsChecked = true;
                UNPD_checkbox.IsChecked = true;
                noResDB_checkbox.IsChecked = false;
            }
            else
            {
                ANPDB_checkbox.IsChecked = false;
                COCONUT_checkbox.IsChecked = false;
                NPASS_checkbox.IsChecked = false;
                UNPD_checkbox.IsChecked = false;
            }
            if (pubchem_checkbox.IsChecked == false &&
                mtb_lpd_checkbox.IsChecked == false &&
                drug_txn_checkbox.IsChecked == false &&
                natProducts_checkbox.IsChecked == false &&
                xbiotics_checkbox.IsChecked == false &&
                contaminants_checkbox.IsChecked == false)
            {
                noResDB_checkbox.IsChecked = true;
            }
        }

        private void xbiotics_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (xbiotics_checkbox.IsChecked == true)
            {
                BLEXP_checkbox.IsChecked = true;
                NORMAN_checkbox.IsChecked = true;
                STF_IDENT_checkbox.IsChecked = true;
                noResDB_checkbox.IsChecked = false;
            }
            else
            {
                BLEXP_checkbox.IsChecked = false;
                NORMAN_checkbox.IsChecked = false;
                STF_IDENT_checkbox.IsChecked = false;
            }
            if (pubchem_checkbox.IsChecked == false &&
                mtb_lpd_checkbox.IsChecked == false &&
                drug_txn_checkbox.IsChecked == false &&
                natProducts_checkbox.IsChecked == false &&
                xbiotics_checkbox.IsChecked == false &&
                contaminants_checkbox.IsChecked == false)
            {
                noResDB_checkbox.IsChecked = true;
            }
        }

        private void contaminants_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (contaminants_checkbox.IsChecked == true)
            {
                MaConDa_checkbox.IsChecked = true;
                noResDB_checkbox.IsChecked = false;
            }
            else
            {
                MaConDa_checkbox.IsChecked = false;
            }
            if (pubchem_checkbox.IsChecked == false &&
                mtb_lpd_checkbox.IsChecked == false &&
                drug_txn_checkbox.IsChecked == false &&
                natProducts_checkbox.IsChecked == false &&
                xbiotics_checkbox.IsChecked == false &&
                contaminants_checkbox.IsChecked == false)
            {
                noResDB_checkbox.IsChecked = true;
            }
        }

        //private void mtb_lpd_checkbox_Click(object sender, RoutedEventArgs e)
        //{
        //    if (mtb_lpd_checkbox.IsChecked == true)
        //    {
        //        BMDB_checkbox.IsChecked = true;
        //        ChEBI_checkbox.IsChecked = true;
        //        ECMDB_checkbox.IsChecked = true;
        //        FooDB_checkbox.IsChecked = true;
        //        HMDB_checkbox.IsChecked = true;
        //        KEGG_checkbox.IsChecked = true;
        //        LMSD_checkbox.IsChecked = true;
        //        MarkerDB_checkbox.IsChecked = true;
        //        MCDB_checkbox.IsChecked = true;
        //        PlantCyc_checkbox.IsChecked = true;
        //        SMPDB_checkbox.IsChecked = true;
        //        YMDB_checkbox.IsChecked = true;
        //    }
        //    else
        //    {
        //        BMDB_checkbox.IsChecked = false;
        //        ChEBI_checkbox.IsChecked = false;
        //        ECMDB_checkbox.IsChecked = false;
        //        FooDB_checkbox.IsChecked = false;
        //        HMDB_checkbox.IsChecked = false;
        //        KEGG_checkbox.IsChecked = false;
        //        LMSD_checkbox.IsChecked = false;
        //        MarkerDB_checkbox.IsChecked = false;
        //        MCDB_checkbox.IsChecked = false;
        //        PlantCyc_checkbox.IsChecked = false;
        //        SMPDB_checkbox.IsChecked = false;
        //        YMDB_checkbox.IsChecked = false;
        //    }
        //}

        //private void drug_txn_checkbox_Click(object sender, RoutedEventArgs e)
        //{
        //    if (drug_txn_checkbox.IsChecked == true)
        //    {
        //        DrugBank_checkbox.IsChecked = true;
        //        DSSTOX_checkbox.IsChecked = true;
        //        HSDB_checkbox.IsChecked = true;
        //        T3DB_checkbox.IsChecked = true;
        //        TTD_checkbox.IsChecked = true;
        //    }
        //    else
        //    {
        //        DrugBank_checkbox.IsChecked = false;
        //        DSSTOX_checkbox.IsChecked = false;
        //        HSDB_checkbox.IsChecked = false;
        //        T3DB_checkbox.IsChecked = false;
        //        TTD_checkbox.IsChecked = false;
        //    }
        //}

        //private void natProducts_checkbox_Click(object sender, RoutedEventArgs e)
        //{
        //    if (natProducts_checkbox.IsChecked == true)
        //    {
        //        ANPDB_checkbox.IsChecked = true;
        //        COCONUT_checkbox.IsChecked = true;
        //        NPASS_checkbox.IsChecked = true;
        //        UNPD_checkbox.IsChecked = true;
        //    }
        //    else
        //    {
        //        ANPDB_checkbox.IsChecked = false;
        //        COCONUT_checkbox.IsChecked = false;
        //        NPASS_checkbox.IsChecked = false;
        //        UNPD_checkbox.IsChecked = false;
        //    }
        //}

        //private void xbiotics_checkbox_Click(object sender, RoutedEventArgs e)
        //{
        //    if (xbiotics_checkbox.IsChecked == true)
        //    {
        //        BLEXP_checkbox.IsChecked = true;
        //        NORMAN_checkbox.IsChecked = true;
        //        STF_IDENT_checkbox.IsChecked = true;
        //    }
        //    else
        //    {
        //        BLEXP_checkbox.IsChecked = false;
        //        NORMAN_checkbox.IsChecked = false;
        //        STF_IDENT_checkbox.IsChecked = false;
        //    }
        //}

        //private void contaminants_checkbox_Click(object sender, RoutedEventArgs e)
        //{
        //    if (contaminants_checkbox.IsChecked == true)
        //    {
        //        MaConDa_checkbox.IsChecked = true;
        //    }
        //    else
        //    {
        //        MaConDa_checkbox.IsChecked = false;
        //    }
        //}

        private void ms_instrument_selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ms_instrument_selection.SelectedIndex;
            switch (index)
            {
                case 0:
                    ms1tol_box.Text = "2";
                    ms2tol_box.Text = "5";
                    ms1tol_switch.IsOn = true;
                    ms2tol_switch.IsOn = true;
                    break;
                case 1:
                    ms1tol_box.Text = "5";
                    ms2tol_box.Text = "10";
                    ms1tol_switch.IsOn = true;
                    ms2tol_switch.IsOn = true;
                    break;
                case 2:
                    ms1tol_box.Text = "10";
                    ms2tol_box.Text = "20";
                    ms1tol_switch.IsOn = true;
                    ms2tol_switch.IsOn = true;
                    break;
                case 3:
                    ms1tol_box.Text = "0.2";
                    ms2tol_box.Text = "0.2";
                    ms1tol_switch.IsOn = false;
                    ms2tol_switch.IsOn = false;
                    break;
                default:
                    break;
            }
        }

        private async void InvalidInputError()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid input.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
    }
}
