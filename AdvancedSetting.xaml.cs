using BUDDY.FormulaData;
using BUDDY.MS2LibrarySearch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
    public sealed partial class AdvancedSetting : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        string currTab = "general";
        ObservableCollection<Adduct> adductList_Pos;
        ObservableCollection<Adduct> adductList_Neg;
        bool throwInvalidInputError = false;
        StorageFile customMS2DB = null;
        List<MS2DBEntry> newDB = new List<MS2DBEntry>();
        public AdvancedSetting()
        {
            //adduct list
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                adductList_Pos = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
            }
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                adductList_Neg = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
            }

            this.InitializeComponent();
            this.Loaded += AdvancedSetting_Page_Loaded;

            #region
            general_panel.Visibility = Visibility.Visible;

            //general
            MS2LibrarySearch_checkbox.IsChecked = (bool)localSettings.Values["MS2LibrarySearch_include"];
            BUDDY_checkbox.IsChecked = (bool)localSettings.Values["BUDDY_include"];
            ExpSpecificGlobalAnnotation_checkbox.IsChecked = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include"];

            ms_instrument_selection.SelectedIndex = (int)localSettings.Values["selectedMSinstrument"];

            ms1tol_box.Text = localSettings.Values["ms1tol"].ToString();
            ms2tol_box.Text = localSettings.Values["ms2tol"].ToString();
            ms1tol_switch.IsOn = (bool)localSettings.Values["ms1tol_ppmON"];
            ms2tol_switch.IsOn = (bool)localSettings.Values["ms2tol_ppmON"];
            topFrag_box.Text = localSettings.Values["topFrag"].ToString();
            use_topFrag_checkbox.IsChecked = (bool)localSettings.Values["use_topFrag"];
            use_allFrag_checkbox.IsChecked = (bool)localSettings.Values["use_allFrag"];
            ms2Deisotope_checkbox.IsChecked = (bool)localSettings.Values["ms2Deisotope"];
            ms2Denoise_checkbox.IsChecked = (bool)localSettings.Values["ms2Denoise"];
            maxNoiseInt_box.Text = localSettings.Values["maxNoiseInt"].ToString();
            maxNoiseFragRatio_box.Text = localSettings.Values["maxNoiseFragRatio"].ToString();
            maxNoiseRSD_box.Text = localSettings.Values["maxNoiseRSD"].ToString();
            
            MetaScore_checkbox.IsChecked = (bool)localSettings.Values["MetaScore_include"];

            //ms2_library_search
            useCustomMS2DB_checkbox.IsChecked = (bool)localSettings.Values["useCustomMS2DB_include"];

            if ((bool)localSettings.Values["useCustomMS2DB_include"])
            {
                string fullPath = (string)localSettings.Values["customMS2DBName"];
                loadedMS2DB_text.Text = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);
            }
            else
            {
                loadedMS2DB_text.Text = "";
            }

            ms2MatchingAlgorithm_selection.SelectedIndex = (int)localSettings.Values["ms2MatchingAlgorithm"];
            metaboliteIdentificationMS2SimilarityScoreThreshold_box.Text = localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold"].ToString();

            string recommendedMetaIdenMS2ScoreStr = "0.7";
            string recommendedMetaIdenMS2MinMatchedFragNoStr = "";
            switch (ms2MatchingAlgorithm_selection.SelectedIndex)
            {
                case 0:
                    recommendedMetaIdenMS2ScoreStr = "0.7";
                    recommendedMetaIdenMS2MinMatchedFragNoStr = "6";
                    break;
                case 1:
                    recommendedMetaIdenMS2ScoreStr = "0.7";
                    recommendedMetaIdenMS2MinMatchedFragNoStr = "6";
                    break;
                case 2:
                    recommendedMetaIdenMS2ScoreStr = "0.75";
                    recommendedMetaIdenMS2MinMatchedFragNoStr = "0";
                    break;
                default:
                    break;
            }
            recommendedMetaIdenMS2Score_text.Text = recommendedMetaIdenMS2ScoreStr;
            recommendedMetaIdenMS2MinMatchedFragNo_text.Text = recommendedMetaIdenMS2MinMatchedFragNoStr;

            metaboliteIdentificationMS2MinMatchedFrag_checkbox.IsChecked = (bool)localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include"];
            metaboliteIdentificationMS2MinMatchedFragNo_box.Text = localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo"].ToString();
            metaboliteIdentificationRT_checkbox.IsChecked = (bool)localSettings.Values["metaboliteIdentificationRT_include"];
            metaboliteIdentificationRTtol_box.Text = localSettings.Values["metaboliteIdentificationRTtol"].ToString();

            //elements
            C_RangeSelector.RangeStart = (int)localSettings.Values["c_min"];
            C_RangeSelector.RangeEnd = (int)localSettings.Values["c_max"];
            ele_C_checkbox.IsChecked = (bool)localSettings.Values["c_include"];
            H_RangeSelector.RangeStart = (int)localSettings.Values["h_min"];
            H_RangeSelector.RangeEnd = (int)localSettings.Values["h_max"];
            ele_H_checkbox.IsChecked = (bool)localSettings.Values["h_include"];
            N_RangeSelector.RangeStart = (int)localSettings.Values["n_min"];
            N_RangeSelector.RangeEnd = (int)localSettings.Values["n_max"];
            ele_N_checkbox.IsChecked = (bool)localSettings.Values["n_include"];
            O_RangeSelector.RangeStart = (int)localSettings.Values["o_min"];
            O_RangeSelector.RangeEnd = (int)localSettings.Values["o_max"];
            ele_O_checkbox.IsChecked = (bool)localSettings.Values["o_include"];
            P_RangeSelector.RangeStart = (int)localSettings.Values["p_min"];
            P_RangeSelector.RangeEnd = (int)localSettings.Values["p_max"];
            ele_P_checkbox.IsChecked = (bool)localSettings.Values["p_include"];
            S_RangeSelector.RangeStart = (int)localSettings.Values["s_min"];
            S_RangeSelector.RangeEnd = (int)localSettings.Values["s_max"];
            ele_S_checkbox.IsChecked = (bool)localSettings.Values["s_include"];
            F_RangeSelector.RangeStart = (int)localSettings.Values["f_min"];
            F_RangeSelector.RangeEnd = (int)localSettings.Values["f_max"];
            ele_F_checkbox.IsChecked = (bool)localSettings.Values["f_include"];
            Cl_RangeSelector.RangeStart = (int)localSettings.Values["cl_min"];
            Cl_RangeSelector.RangeEnd = (int)localSettings.Values["cl_max"];
            ele_Cl_checkbox.IsChecked = (bool)localSettings.Values["cl_include"];
            Br_RangeSelector.RangeStart = (int)localSettings.Values["br_min"];
            Br_RangeSelector.RangeEnd = (int)localSettings.Values["br_max"];
            ele_Br_checkbox.IsChecked = (bool)localSettings.Values["br_include"];
            I_RangeSelector.RangeStart = (int)localSettings.Values["i_min"];
            I_RangeSelector.RangeEnd = (int)localSettings.Values["i_max"];
            ele_I_checkbox.IsChecked = (bool)localSettings.Values["i_include"];
            Si_RangeSelector.RangeStart = (int)localSettings.Values["si_min"];
            Si_RangeSelector.RangeEnd = (int)localSettings.Values["si_max"];
            ele_Si_checkbox.IsChecked = (bool)localSettings.Values["si_include"];
            B_RangeSelector.RangeStart = (int)localSettings.Values["b_min"];
            B_RangeSelector.RangeEnd = (int)localSettings.Values["b_max"];
            ele_B_checkbox.IsChecked = (bool)localSettings.Values["b_include"];
            Se_RangeSelector.RangeStart = (int)localSettings.Values["se_min"];
            Se_RangeSelector.RangeEnd = (int)localSettings.Values["se_max"];
            ele_Se_checkbox.IsChecked = (bool)localSettings.Values["se_include"];

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

            //raw data processing
            //isotopicPT_checkbox.IsChecked = (bool)localSettings.Values["isotopic_pattern_imp"];
            featureClusterMsdial_checkbox.IsChecked = (bool)localSettings.Values["featureClusterMsdial_include"];
            max_ispPeaks_box.Text = localSettings.Values["max_isotopic_peaks"].ToString();
            ispAbundance_cut_box.Text = localSettings.Values["isotopic_abundance_cutoff"].ToString();
            isp_group_masstol_box.Text = localSettings.Values["isotopologue_grp_masstol"].ToString();
            ms2Group_checkbox.IsChecked = (bool)localSettings.Values["ms2_group_imp"];
            cosSim_cut_box.Text = localSettings.Values["cos_similarity_cutoff"].ToString();
            max_peakWidth_box.Text = localSettings.Values["max_peak_width"].ToString();
            ms2Merge_checkbox.IsChecked = (bool)localSettings.Values["ms2_merging"];

            //candidate refinement
            topCandidates_box.Text = localSettings.Values["topCandidates"].ToString();
            use_topCandidates_checkbox.IsChecked = (bool)localSettings.Values["use_topCandidates"];
            use_allCandidates_checkbox.IsChecked = (bool)localSettings.Values["use_allCandidates"];
            //expMS2fragNum_checkbox.IsChecked = (bool)localSettings.Values["expMS2fragNum_include"];
            //expMS2fragNum_box.Text = localSettings.Values["expMS2fragNum"].ToString();
            //expMS2fragInt_checkbox.IsChecked = (bool)localSettings.Values["expMS2fragInt_include"];
            //expMS2fragInt_box.Text = localSettings.Values["expMS2fragInt"].ToString();
            isp_sim_checkbox.IsChecked = (bool)localSettings.Values["ispPatternSim_include"];
            isp_sim_box.Text = localSettings.Values["ispPatternSim"].ToString();
            h_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["h_c_ratio_include"];
            h_c_ratio_box1.Text = localSettings.Values["h_c_ratio_box1"].ToString();
            h_c_ratio_box2.Text = localSettings.Values["h_c_ratio_box2"].ToString();
            o_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["o_c_ratio_include"];
            o_c_ratio_box1.Text = localSettings.Values["o_c_ratio_box1"].ToString();
            o_c_ratio_box2.Text = localSettings.Values["o_c_ratio_box2"].ToString();
            n_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["n_c_ratio_include"];
            n_c_ratio_box1.Text = localSettings.Values["n_c_ratio_box1"].ToString();
            n_c_ratio_box2.Text = localSettings.Values["n_c_ratio_box2"].ToString();
            p_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["p_c_ratio_include"];
            p_c_ratio_box1.Text = localSettings.Values["p_c_ratio_box1"].ToString();
            p_c_ratio_box2.Text = localSettings.Values["p_c_ratio_box2"].ToString();
            s_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["s_c_ratio_include"];
            s_c_ratio_box1.Text = localSettings.Values["s_c_ratio_box1"].ToString();
            s_c_ratio_box2.Text = localSettings.Values["s_c_ratio_box2"].ToString();
            f_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["f_c_ratio_include"];
            f_c_ratio_box1.Text = localSettings.Values["f_c_ratio_box1"].ToString();
            f_c_ratio_box2.Text = localSettings.Values["f_c_ratio_box2"].ToString();
            cl_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["cl_c_ratio_include"];
            cl_c_ratio_box1.Text = localSettings.Values["cl_c_ratio_box1"].ToString();
            cl_c_ratio_box2.Text = localSettings.Values["cl_c_ratio_box2"].ToString();
            br_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["br_c_ratio_include"];
            br_c_ratio_box1.Text = localSettings.Values["br_c_ratio_box1"].ToString();
            br_c_ratio_box2.Text = localSettings.Values["br_c_ratio_box2"].ToString();
            si_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["si_c_ratio_include"];
            si_c_ratio_box1.Text = localSettings.Values["si_c_ratio_box1"].ToString();
            si_c_ratio_box2.Text = localSettings.Values["si_c_ratio_box2"].ToString();
            o_p_ratio_checkbox.IsChecked = (bool)localSettings.Values["o_p_ratio_include"];

            //timeout
            timeout_single_checkbox.IsChecked = (bool)localSettings.Values["timeout_single_include"];
            timeout_single_box.Text = localSettings.Values["timeout_single"].ToString();
            //timeout_batch_checkbox.IsChecked = (bool)localSettings.Values["timeout_batch_include"];
            //timeout_batch_box.Text = localSettings.Values["timeout_batch"].ToString();

            MS2FragReannotate_checkbox.IsChecked = (bool)localSettings.Values["MS2FragReannotate_include"];
            #endregion
        }


        private void AdvancedSetting_Page_Loaded(object sender, RoutedEventArgs e)
        {
            var s = ApplicationView.GetForCurrentView();
            s.TryResizeView(new Windows.Foundation.Size { Width = 800.0, Height = 800.0 });
        }

        private void general_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = true;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = false;

            //general_button.Background = (SolidColorBrush)Resources["MenuLightGrey"];
            //general_button.Foreground = (SolidColorBrush)Resources["MenuDarkGrey"];
            //element_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //element_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //formulaDB_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //formulaDB_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //adduct_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //adduct_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //raw_data_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //raw_data_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //candidate_refinement_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //candidate_refinement_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //others_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //others_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];

            currTab = "general";
            general_panel.Visibility = Visibility.Visible;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Collapsed;
        }

        private void ms2_library_search_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = true;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = false;


            currTab = "ms2_library_search";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Visible;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Collapsed;
        }


        private void element_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = true;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = false;

            //general_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //general_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //element_button.Background = (SolidColorBrush)Resources["MenuLightGrey"];
            //element_button.Foreground = (SolidColorBrush)Resources["MenuDarkGrey"];
            //formulaDB_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //formulaDB_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //adduct_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //adduct_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //raw_data_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //raw_data_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //candidate_refinement_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //candidate_refinement_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //others_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //others_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];

            currTab = "element";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Visible;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Collapsed;
        }

        private void formulaDB_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = true;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = false;

            //general_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //general_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //element_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //element_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //formulaDB_button.Background = (SolidColorBrush)Resources["MenuLightGrey"];
            //formulaDB_button.Foreground = (SolidColorBrush)Resources["MenuDarkGrey"];
            //adduct_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //adduct_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //raw_data_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //raw_data_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //candidate_refinement_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //candidate_refinement_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];
            //others_button.Background = (SolidColorBrush)Resources["MenuDarkGrey"];
            //others_button.Foreground = (SolidColorBrush)Resources["MenuLightGrey"];

            currTab = "formulaDB";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Visible;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Collapsed;
        }

        private void adduct_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = true;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = false;

            currTab = "adduct";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Visible;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Collapsed;
        }

        private void raw_data_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = true;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = false;

            currTab = "raw_data";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Visible;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Collapsed;
        }

        private void candidate_refinement_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = true;
            others_button.IsChecked = false;

            currTab = "candidate_refinement";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Visible;
            others_panel.Visibility = Visibility.Collapsed;
        }

        private void others_button_Click(object sender, RoutedEventArgs e)
        {
            general_button.IsChecked = false;
            ms2_library_search_button.IsChecked = false;
            element_button.IsChecked = false;
            formulaDB_button.IsChecked = false;
            adduct_button.IsChecked = false;
            raw_data_button.IsChecked = false;
            candidate_refinement_button.IsChecked = false;
            others_button.IsChecked = true;

            currTab = "others";
            general_panel.Visibility = Visibility.Collapsed;
            ms2_library_search_panel.Visibility = Visibility.Collapsed;
            element_panel.Visibility = Visibility.Collapsed;
            fdb_res_panel.Visibility = Visibility.Collapsed;
            adduct_panel.Visibility = Visibility.Collapsed;
            raw_data_panel.Visibility = Visibility.Collapsed;
            candidate_refinement_panel.Visibility = Visibility.Collapsed;
            others_panel.Visibility = Visibility.Visible;
        }
        private void ADV_Restore(object sender, RoutedEventArgs e)
        {
            switch (currTab)
            {
                case "general":
                    ms_instrument_selection.SelectedIndex = (int)localSettings.Values["selectedMSinstrument_default"];
                    ms1tol_box.Text = localSettings.Values["ms1tol_default"].ToString();
                    ms2tol_box.Text = localSettings.Values["ms2tol_default"].ToString();
                    ms1tol_switch.IsOn = (bool)localSettings.Values["ms1tol_ppmON_default"];
                    ms2tol_switch.IsOn = (bool)localSettings.Values["ms2tol_ppmON_default"];
                    topFrag_box.Text = localSettings.Values["topFrag_default"].ToString();
                    use_topFrag_checkbox.IsChecked = (bool)localSettings.Values["use_topFrag_default"];
                    use_allFrag_checkbox.IsChecked = (bool)localSettings.Values["use_allFrag_default"];
                    ms2Deisotope_checkbox.IsChecked = (bool)localSettings.Values["ms2Deisotope_default"];
                    ms2Denoise_checkbox.IsChecked = (bool)localSettings.Values["ms2Denoise_default"];
                    maxNoiseInt_box.Text = localSettings.Values["maxNoiseInt_default"].ToString();
                    maxNoiseFragRatio_box.Text = localSettings.Values["maxNoiseFragRatio_default"].ToString();
                    maxNoiseRSD_box.Text = localSettings.Values["maxNoiseRSD_default"].ToString();
                    ExpSpecificGlobalAnnotation_checkbox.IsChecked = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include_default"];
                    MS2LibrarySearch_checkbox.IsChecked = (bool)localSettings.Values["MS2LibrarySearch_include_default"];
                    BUDDY_checkbox.IsChecked = (bool)localSettings.Values["BUDDY_include_default"];

                    MetaScore_checkbox.IsChecked = (bool)localSettings.Values["MetaScore_include_default"];
                    break;
                case "ms2_library_search":
                    useCustomMS2DB_checkbox.IsChecked = (bool)localSettings.Values["useCustomMS2DB_include_default"];
                    ms2MatchingAlgorithm_selection.SelectedIndex = (int)localSettings.Values["ms2MatchingAlgorithm_default"];
                    metaboliteIdentificationMS2SimilarityScoreThreshold_box.Text = localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold_default"].ToString();
                    metaboliteIdentificationMS2MinMatchedFrag_checkbox.IsChecked = (bool)localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include_default"];
                    metaboliteIdentificationMS2MinMatchedFragNo_box.Text = localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo_default"].ToString();
                    metaboliteIdentificationRT_checkbox.IsChecked = (bool)localSettings.Values["metaboliteIdentificationRT_include_default"];
                    metaboliteIdentificationRTtol_box.Text = localSettings.Values["metaboliteIdentificationRTtol_default"].ToString();
                    loadedMS2DB_text.Text = "";
                    break;
                case "element":
                    C_RangeSelector.RangeStart = (int)localSettings.Values["c_min_default"];
                    C_RangeSelector.RangeEnd = (int)localSettings.Values["c_max_default"];
                    ele_C_checkbox.IsChecked = (bool)localSettings.Values["c_include_default"];
                    H_RangeSelector.RangeStart = (int)localSettings.Values["h_min_default"];
                    H_RangeSelector.RangeEnd = (int)localSettings.Values["h_max_default"];
                    ele_H_checkbox.IsChecked = (bool)localSettings.Values["h_include_default"];
                    N_RangeSelector.RangeStart = (int)localSettings.Values["n_min_default"];
                    N_RangeSelector.RangeEnd = (int)localSettings.Values["n_max_default"];
                    ele_N_checkbox.IsChecked = (bool)localSettings.Values["n_include_default"];
                    O_RangeSelector.RangeStart = (int)localSettings.Values["o_min_default"];
                    O_RangeSelector.RangeEnd = (int)localSettings.Values["o_max_default"];
                    ele_O_checkbox.IsChecked = (bool)localSettings.Values["o_include_default"];
                    P_RangeSelector.RangeStart = (int)localSettings.Values["p_min_default"];
                    P_RangeSelector.RangeEnd = (int)localSettings.Values["p_max_default"];
                    ele_P_checkbox.IsChecked = (bool)localSettings.Values["p_include_default"];
                    S_RangeSelector.RangeStart = (int)localSettings.Values["s_min_default"];
                    S_RangeSelector.RangeEnd = (int)localSettings.Values["s_max_default"];
                    ele_S_checkbox.IsChecked = (bool)localSettings.Values["s_include_default"];
                    F_RangeSelector.RangeStart = (int)localSettings.Values["f_min_default"];
                    F_RangeSelector.RangeEnd = (int)localSettings.Values["f_max_default"];
                    ele_F_checkbox.IsChecked = (bool)localSettings.Values["f_include_default"];
                    Cl_RangeSelector.RangeStart = (int)localSettings.Values["cl_min_default"];
                    Cl_RangeSelector.RangeEnd = (int)localSettings.Values["cl_max_default"];
                    ele_Cl_checkbox.IsChecked = (bool)localSettings.Values["cl_include_default"];
                    Br_RangeSelector.RangeStart = (int)localSettings.Values["br_min_default"];
                    Br_RangeSelector.RangeEnd = (int)localSettings.Values["br_max_default"];
                    ele_Br_checkbox.IsChecked = (bool)localSettings.Values["br_include_default"];
                    I_RangeSelector.RangeStart = (int)localSettings.Values["i_min_default"];
                    I_RangeSelector.RangeEnd = (int)localSettings.Values["i_max_default"];
                    ele_I_checkbox.IsChecked = (bool)localSettings.Values["i_include_default"];
                    Si_RangeSelector.RangeStart = (int)localSettings.Values["si_min_default"];
                    Si_RangeSelector.RangeEnd = (int)localSettings.Values["si_max_default"];
                    ele_Si_checkbox.IsChecked = (bool)localSettings.Values["si_include_default"];
                    B_RangeSelector.RangeStart = (int)localSettings.Values["b_min_default"];
                    B_RangeSelector.RangeEnd = (int)localSettings.Values["b_max_default"];
                    ele_B_checkbox.IsChecked = (bool)localSettings.Values["b_include_default"];
                    Se_RangeSelector.RangeStart = (int)localSettings.Values["se_min_default"];
                    Se_RangeSelector.RangeEnd = (int)localSettings.Values["se_max_default"];
                    ele_Se_checkbox.IsChecked = (bool)localSettings.Values["se_include_default"];

                    break;
                case "formulaDB":
                    //formula database
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
                    break;
                case "adduct":
                    List<Adduct> defaultPosList = new List<Adduct>();
                    using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos_Default.bin", FileMode.Open))
                    {
                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        defaultPosList = (List<Adduct>)bformatter.Deserialize(stream);
                    }
                    List<Adduct> defaultNegList = new List<Adduct>();
                    using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg_Default.bin", FileMode.Open))
                    {
                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        defaultNegList = (List<Adduct>)bformatter.Deserialize(stream);
                    }
                    adductList_Pos.Clear();
                    foreach (Adduct ad in defaultPosList)
                    {
                        adductList_Pos.Add(ad);
                    }
                    adductList_Neg.Clear();
                    foreach (Adduct ad in defaultNegList)
                    {
                        adductList_Neg.Add(ad);
                    }
                    break;
                case "raw_data":
                    //isotopicPT_checkbox.IsChecked = (bool)localSettings.Values["isotopic_pattern_imp_default"];
                    featureClusterMsdial_checkbox.IsChecked = (bool)localSettings.Values["featureClusterMsdial_include_default"];
                    max_ispPeaks_box.Text = localSettings.Values["max_isotopic_peaks_default"].ToString();
                    ispAbundance_cut_box.Text = localSettings.Values["isotopic_abundance_cutoff_default"].ToString();
                    isp_group_masstol_box.Text = localSettings.Values["isotopologue_grp_masstol_default"].ToString();
                    ms2Group_checkbox.IsChecked = (bool)localSettings.Values["ms2_group_imp_default"];
                    cosSim_cut_box.Text = localSettings.Values["cos_similarity_cutoff_default"].ToString();
                    max_peakWidth_box.Text = localSettings.Values["max_peak_width_default"].ToString();
                    ms2Merge_checkbox.IsChecked = (bool)localSettings.Values["ms2_merging_default"];
                    break;
                case "candidate_refinement":
                    topCandidates_box.Text = localSettings.Values["topCandidates_default"].ToString();
                    use_topCandidates_checkbox.IsChecked = (bool)localSettings.Values["use_topCandidates_default"];
                    use_allCandidates_checkbox.IsChecked = (bool)localSettings.Values["use_allCandidates_default"];
                    //expMS2fragNum_checkbox.IsChecked = (bool)localSettings.Values["expMS2fragNum_include_default"];
                    //expMS2fragNum_box.Text = localSettings.Values["expMS2fragNum_default"].ToString();
                    //expMS2fragInt_checkbox.IsChecked = (bool)localSettings.Values["expMS2fragInt_include_default"];
                    //expMS2fragInt_box.Text = localSettings.Values["expMS2fragInt_default"].ToString();
                    isp_sim_checkbox.IsChecked = (bool)localSettings.Values["ispPatternSim_include_default"];
                    isp_sim_box.Text = localSettings.Values["ispPatternSim_default"].ToString();
                    h_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["h_c_ratio_include_default"];
                    h_c_ratio_box1.Text = localSettings.Values["h_c_ratio_box1_default"].ToString();
                    h_c_ratio_box2.Text = localSettings.Values["h_c_ratio_box2_default"].ToString();
                    o_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["o_c_ratio_include_default"];
                    o_c_ratio_box1.Text = localSettings.Values["o_c_ratio_box1_default"].ToString();
                    o_c_ratio_box2.Text = localSettings.Values["o_c_ratio_box2_default"].ToString();
                    n_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["n_c_ratio_include_default"];
                    n_c_ratio_box1.Text = localSettings.Values["n_c_ratio_box1_default"].ToString();
                    n_c_ratio_box2.Text = localSettings.Values["n_c_ratio_box2_default"].ToString();
                    p_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["p_c_ratio_include_default"];
                    p_c_ratio_box1.Text = localSettings.Values["p_c_ratio_box1_default"].ToString();
                    p_c_ratio_box2.Text = localSettings.Values["p_c_ratio_box2_default"].ToString();
                    s_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["s_c_ratio_include_default"];
                    s_c_ratio_box1.Text = localSettings.Values["s_c_ratio_box1_default"].ToString();
                    s_c_ratio_box2.Text = localSettings.Values["s_c_ratio_box2_default"].ToString();
                    f_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["f_c_ratio_include_default"];
                    f_c_ratio_box1.Text = localSettings.Values["f_c_ratio_box1_default"].ToString();
                    f_c_ratio_box2.Text = localSettings.Values["f_c_ratio_box2_default"].ToString();
                    cl_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["cl_c_ratio_include_default"];
                    cl_c_ratio_box1.Text = localSettings.Values["cl_c_ratio_box1_default"].ToString();
                    cl_c_ratio_box2.Text = localSettings.Values["cl_c_ratio_box2_default"].ToString();
                    br_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["br_c_ratio_include_default"];
                    br_c_ratio_box1.Text = localSettings.Values["br_c_ratio_box1_default"].ToString();
                    br_c_ratio_box2.Text = localSettings.Values["br_c_ratio_box2_default"].ToString();
                    si_c_ratio_checkbox.IsChecked = (bool)localSettings.Values["si_c_ratio_include_default"];
                    si_c_ratio_box1.Text = localSettings.Values["si_c_ratio_box1_default"].ToString();
                    si_c_ratio_box2.Text = localSettings.Values["si_c_ratio_box2_default"].ToString();
                    o_p_ratio_checkbox.IsChecked = (bool)localSettings.Values["o_p_ratio_include_default"];
                    break;
                case "others":
                    timeout_single_checkbox.IsChecked = (bool)localSettings.Values["timeout_single_include_default"];
                    timeout_single_box.Text = localSettings.Values["timeout_single_default"].ToString();
                    MS2FragReannotate_checkbox.IsChecked = (bool)localSettings.Values["MS2FragReannotate_include_default"];
                    
                    //timeout_batch_checkbox.IsChecked = (bool)localSettings.Values["timeout_batch_include_default"];
                    //timeout_batch_box.Text = localSettings.Values["timeout_batch_default"].ToString();
                    break;
                default:
                    break;
            }
        }
        private void ADV_Apply(object sender, RoutedEventArgs e)
        {
            throwInvalidInputError = false;
            //general panel
            #region
            localSettings.Values["selectedMSinstrument"] = ms_instrument_selection.SelectedIndex;

            double ms1tol_adv;
            if (double.TryParse(ms1tol_box.Text, out ms1tol_adv))
            {
                if (ms1tol_adv > 0)
                {
                    localSettings.Values["ms1tol"] = ms1tol_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;
            }

            double ms2tol_adv;
            if (double.TryParse(ms2tol_box.Text, out ms2tol_adv))
            {
                if (ms2tol_adv > 0)
                {
                    localSettings.Values["ms2tol"] = ms2tol_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;                
            }

            //localSettings.Values["ms1tol"] = Double.Parse(ms1tol_box.Text);
            //localSettings.Values["ms2tol"] = Double.Parse(ms2tol_box.Text);
            localSettings.Values["ms1tol_ppmON"] = ms1tol_switch.IsOn;
            localSettings.Values["ms2tol_ppmON"] = ms2tol_switch.IsOn;

            int topfrag_adv;
            if (int.TryParse(topFrag_box.Text, out topfrag_adv))
            {
                if (topfrag_adv > 0)
                {
                    localSettings.Values["topFrag"] = topfrag_adv;
                }
                else
                {
                    throwInvalidInputError = true;                    
                }
            }
            else
            {
                throwInvalidInputError = true;                
            }
            //localSettings.Values["topFrag"] = int.Parse(topFrag_box.Text);
            localSettings.Values["use_topFrag"] = use_topFrag_checkbox.IsChecked;
            localSettings.Values["use_allFrag"] = use_allFrag_checkbox.IsChecked;
            localSettings.Values["ms2Deisotope"] = ms2Deisotope_checkbox.IsChecked;
            localSettings.Values["ms2Denoise"] = ms2Denoise_checkbox.IsChecked;

            double maxNoiseInt_adv;
            if (double.TryParse(maxNoiseInt_box.Text, out maxNoiseInt_adv))
            {
                if (maxNoiseInt_adv > 0)
                {
                    localSettings.Values["maxNoiseInt"] = maxNoiseInt_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;                
            }

            double maxNoiseFragRatio_adv;
            if (double.TryParse(maxNoiseFragRatio_box.Text, out maxNoiseFragRatio_adv))
            {
                if (maxNoiseFragRatio_adv > 0 && maxNoiseFragRatio_adv < 100)
                {
                    localSettings.Values["maxNoiseFragRatio"] = maxNoiseFragRatio_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;
            }
            double maxNoiseRSD_adv;
            if (double.TryParse(maxNoiseRSD_box.Text, out maxNoiseRSD_adv))
            {
                if (maxNoiseRSD_adv > 0)
                {
                    localSettings.Values["maxNoiseRSD"] = maxNoiseRSD_adv;
                }
                else
                {
                    throwInvalidInputError = true;                    
                }
            }
            else
            {
                throwInvalidInputError = true;                
            }
            localSettings.Values["ExpSpecificGlobalAnnotation_include"] = ExpSpecificGlobalAnnotation_checkbox.IsChecked;
            localSettings.Values["BUDDY_include"] = BUDDY_checkbox.IsChecked;
            localSettings.Values["MS2LibrarySearch_include"] = MS2LibrarySearch_checkbox.IsChecked;
            localSettings.Values["MetaScore_include"] = MetaScore_checkbox.IsChecked;


            //localSettings.Values["maxNoiseInt"] = Double.Parse(maxNoiseInt_box.Text);
            //localSettings.Values["maxNoiseFragRatio"] = Double.Parse(maxNoiseFragRatio_box.Text);

            // ms2 library search
            localSettings.Values["useCustomMS2DB_include"] = useCustomMS2DB_checkbox.IsChecked;
            localSettings.Values["ms2MatchingAlgorithm"] = ms2MatchingAlgorithm_selection.SelectedIndex;

            double metaboliteIdentificationMS2SimilarityScoreThreshold_adv;
            if (double.TryParse(metaboliteIdentificationMS2SimilarityScoreThreshold_box.Text, out metaboliteIdentificationMS2SimilarityScoreThreshold_adv))
            {
                if (metaboliteIdentificationMS2SimilarityScoreThreshold_adv >= 0 && metaboliteIdentificationMS2SimilarityScoreThreshold_adv <= 1)
                {
                    localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold"] = metaboliteIdentificationMS2SimilarityScoreThreshold_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;
            }

            localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include"] =  metaboliteIdentificationMS2MinMatchedFrag_checkbox.IsChecked;

            int metaboliteIdentificationMS2MinMatchedFragNo_adv;
            if (int.TryParse(metaboliteIdentificationMS2MinMatchedFragNo_box.Text, out metaboliteIdentificationMS2MinMatchedFragNo_adv))
            {
                if (metaboliteIdentificationMS2MinMatchedFragNo_adv >= 0)
                {
                    localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo"] = metaboliteIdentificationMS2MinMatchedFragNo_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;
            }

            localSettings.Values["metaboliteIdentificationRT_include"] = metaboliteIdentificationRT_checkbox.IsChecked;

            double metaboliteIdentificationRTtol_adv;
            if (double.TryParse(metaboliteIdentificationRTtol_box.Text, out metaboliteIdentificationRTtol_adv))
            {
                if (metaboliteIdentificationRTtol_adv >= 0)
                {
                    localSettings.Values["metaboliteIdentificationRTtol"] = metaboliteIdentificationRTtol_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;
            }
            #endregion

            if (customMS2DB != null && useCustomMS2DB_checkbox.IsChecked == true)
            {
                localSettings.Values["useCustomMS2DB_include"] = true;
                localSettings.Values["customMS2DBName"] = customMS2DB.Path;
                //await this.MainPage.LoadMS2Database(customMS2DB.Name);
            }

            //elements
            if (currTab == "element")
            {
                C_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minC_box.Text);
                C_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxC_box.Text);
                H_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minH_box.Text);
                H_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxH_box.Text);
                N_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minN_box.Text);
                N_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxN_box.Text);
                O_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minO_box.Text);
                O_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxO_box.Text);
                P_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minP_box.Text);
                P_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxP_box.Text);
                S_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minS_box.Text);
                S_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxS_box.Text);
                F_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minF_box.Text);
                F_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxF_box.Text);
                Cl_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minCl_box.Text);
                Cl_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxCl_box.Text);
                Br_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minBr_box.Text);
                Br_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxBr_box.Text);
                I_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minI_box.Text);
                I_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxI_box.Text);
                Si_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minSi_box.Text);
                Si_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxSi_box.Text);
                B_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minB_box.Text);
                B_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxB_box.Text);
                Se_RangeSelector.RangeStart = tryParsingIntHelper_non_neg(minSe_box.Text);
                Se_RangeSelector.RangeEnd = tryParsingIntHelper_non_neg(maxSe_box.Text);
            }

            localSettings.Values["c_min"] = Convert.ToInt32(C_RangeSelector.RangeStart);
            localSettings.Values["c_max"] = Convert.ToInt32(C_RangeSelector.RangeEnd);
            localSettings.Values["c_include"] = ele_C_checkbox.IsChecked;
            localSettings.Values["h_min"] = Convert.ToInt32(H_RangeSelector.RangeStart);
            localSettings.Values["h_max"] = Convert.ToInt32(H_RangeSelector.RangeEnd);
            localSettings.Values["h_include"] = ele_H_checkbox.IsChecked;
            localSettings.Values["n_min"] = Convert.ToInt32(N_RangeSelector.RangeStart);
            localSettings.Values["n_max"] = Convert.ToInt32(N_RangeSelector.RangeEnd);
            localSettings.Values["n_include"] = ele_N_checkbox.IsChecked;
            localSettings.Values["o_min"] = Convert.ToInt32(O_RangeSelector.RangeStart);
            localSettings.Values["o_max"] = Convert.ToInt32(O_RangeSelector.RangeEnd);
            localSettings.Values["o_include"] = ele_O_checkbox.IsChecked;
            localSettings.Values["p_min"] = Convert.ToInt32(P_RangeSelector.RangeStart);
            localSettings.Values["p_max"] = Convert.ToInt32(P_RangeSelector.RangeEnd);
            localSettings.Values["p_include"] = ele_P_checkbox.IsChecked;
            localSettings.Values["s_min"] = Convert.ToInt32(S_RangeSelector.RangeStart);
            localSettings.Values["s_max"] = Convert.ToInt32(S_RangeSelector.RangeEnd);
            localSettings.Values["s_include"] = ele_S_checkbox.IsChecked;
            localSettings.Values["f_min"] = Convert.ToInt32(F_RangeSelector.RangeStart);
            localSettings.Values["f_max"] = Convert.ToInt32(F_RangeSelector.RangeEnd);
            localSettings.Values["f_include"] = ele_F_checkbox.IsChecked;
            localSettings.Values["cl_min"] = Convert.ToInt32(Cl_RangeSelector.RangeStart);
            localSettings.Values["cl_max"] = Convert.ToInt32(Cl_RangeSelector.RangeEnd);
            localSettings.Values["cl_include"] = ele_Cl_checkbox.IsChecked;
            localSettings.Values["br_min"] = Convert.ToInt32(Br_RangeSelector.RangeStart);
            localSettings.Values["br_max"] = Convert.ToInt32(Br_RangeSelector.RangeEnd);
            localSettings.Values["br_include"] = ele_Br_checkbox.IsChecked;
            localSettings.Values["i_min"] = Convert.ToInt32(I_RangeSelector.RangeStart);
            localSettings.Values["i_max"] = Convert.ToInt32(I_RangeSelector.RangeEnd);
            localSettings.Values["i_include"] = ele_I_checkbox.IsChecked;
            localSettings.Values["si_min"] = Convert.ToInt32(Si_RangeSelector.RangeStart);
            localSettings.Values["si_max"] = Convert.ToInt32(Si_RangeSelector.RangeEnd);
            localSettings.Values["si_include"] = ele_Si_checkbox.IsChecked;
            localSettings.Values["b_min"] = Convert.ToInt32(B_RangeSelector.RangeStart);
            localSettings.Values["b_max"] = Convert.ToInt32(B_RangeSelector.RangeEnd);
            localSettings.Values["b_include"] = ele_B_checkbox.IsChecked;
            localSettings.Values["se_min"] = Convert.ToInt32(Se_RangeSelector.RangeStart);
            localSettings.Values["se_max"] = Convert.ToInt32(Se_RangeSelector.RangeEnd);
            localSettings.Values["se_include"] = ele_Se_checkbox.IsChecked;

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

            if(noResDB_checkbox.IsChecked == false && pubchem_checkbox.IsChecked == false && BMDB_checkbox.IsChecked == false && ChEBI_checkbox.IsChecked == false &&
                ECMDB_checkbox.IsChecked == false && FooDB_checkbox.IsChecked == false && HMDB_checkbox.IsChecked == false && KEGG_checkbox.IsChecked == false && LMSD_checkbox.IsChecked == false &&
                MarkerDB_checkbox.IsChecked == false && MCDB_checkbox.IsChecked == false && PlantCyc_checkbox.IsChecked == false && SMPDB_checkbox.IsChecked == false && YMDB_checkbox.IsChecked == false &&
                DrugBank_checkbox.IsChecked == false && DSSTOX_checkbox.IsChecked == false && HSDB_checkbox.IsChecked == false && T3DB_checkbox.IsChecked == false &&
                TTD_checkbox.IsChecked == false && ANPDB_checkbox.IsChecked == false && COCONUT_checkbox.IsChecked == false && NPASS_checkbox.IsChecked == false &&
                UNPD_checkbox.IsChecked == false && BLEXP_checkbox.IsChecked == false && NORMAN_checkbox.IsChecked == false && STF_IDENT_checkbox.IsChecked == false &&
                MaConDa_checkbox.IsChecked == false)
            {
                throwInvalidInputError = true;
            }

            //adduct list
            //serialize
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, adductList_Pos);
            }
            //serialize
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, adductList_Neg);
            }

            //raw data processing
            //localSettings.Values["isotopic_pattern_imp"] = isotopicPT_checkbox.IsChecked;
            localSettings.Values["featureClusterMsdial_include"] = featureClusterMsdial_checkbox.IsChecked;

            int max_isotopic_peaks_adv;
            if (Int32.TryParse(max_ispPeaks_box.Text, out max_isotopic_peaks_adv))
            {
                if (max_isotopic_peaks_adv > 0)
                {
                    localSettings.Values["max_isotopic_peaks"] = max_isotopic_peaks_adv;
                }
                else
                {
                    throwInvalidInputError = true;
                }
            }
            else
            {
                throwInvalidInputError = true;                
            }

            //localSettings.Values["max_isotopic_peaks"] = int.Parse(max_ispPeaks_box.Text);

            double isotopic_abundance_cutoff_adv;
            if (Double.TryParse(ispAbundance_cut_box.Text, out isotopic_abundance_cutoff_adv))
            {
                if (isotopic_abundance_cutoff_adv > 0 && isotopic_abundance_cutoff_adv < 100)
                {
                    localSettings.Values["isotopic_abundance_cutoff"] = isotopic_abundance_cutoff_adv;
                }
                else
                {
                    throwInvalidInputError = true;                    
                }
            }
            else
            {
                throwInvalidInputError = true;
                
            }
            //localSettings.Values["isotopic_abundance_cutoff"] = Double.Parse(ispAbundance_cut_box.Text);

            localSettings.Values["isotopologue_grp_masstol"] = tryParsingDoubleHelper_non_neg(isp_group_masstol_box.Text);
            localSettings.Values["ms2_group_imp"] = ms2Group_checkbox.IsChecked;
            localSettings.Values["cos_similarity_cutoff"] = tryParsingDoubleHelper_non_neg(cosSim_cut_box.Text);
            localSettings.Values["max_peak_width"] = tryParsingDoubleHelper_non_neg(max_peakWidth_box.Text);
            localSettings.Values["ms2_merging"] = ms2Merge_checkbox.IsChecked;

            //candidate refinement
            localSettings.Values["topCandidates"] = tryParsingIntHelper_pos(topCandidates_box.Text);
            localSettings.Values["use_topCandidates"] = use_topCandidates_checkbox.IsChecked;
            localSettings.Values["use_allCandidates"] = use_allCandidates_checkbox.IsChecked;
            //localSettings.Values["expMS2fragNum_include"] = expMS2fragNum_checkbox.IsChecked;
            //localSettings.Values["expMS2fragNum"] = tryParsingDoubleHelper_for_percentage(expMS2fragNum_box.Text);
            //localSettings.Values["expMS2fragInt_include"] = expMS2fragInt_checkbox.IsChecked;
            //localSettings.Values["expMS2fragInt"] = tryParsingDoubleHelper_for_percentage(expMS2fragInt_box.Text);
            localSettings.Values["ispPatternSim_include"] = isp_sim_checkbox.IsChecked;
            localSettings.Values["ispPatternSim"] = tryParsingDoubleHelper_for_cutoff(isp_sim_box.Text);
            localSettings.Values["h_c_ratio_include"] = h_c_ratio_checkbox.IsChecked;

            double h_c_min = tryParsingDoubleHelper_non_neg(h_c_ratio_box1.Text);
            double h_c_max = tryParsingDoubleHelper_non_neg(h_c_ratio_box2.Text);
            if (h_c_min <= h_c_max)
            {
                localSettings.Values["h_c_ratio_box1"] = h_c_min;
                localSettings.Values["h_c_ratio_box2"] = h_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["o_c_ratio_include"] = o_c_ratio_checkbox.IsChecked;
            double o_c_min = tryParsingDoubleHelper_non_neg(o_c_ratio_box1.Text);
            double o_c_max = tryParsingDoubleHelper_non_neg(o_c_ratio_box2.Text);
            if (o_c_min <= o_c_max)
            {
                localSettings.Values["o_c_ratio_box1"] = o_c_min;
                localSettings.Values["o_c_ratio_box2"] = o_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["n_c_ratio_include"] = n_c_ratio_checkbox.IsChecked;
            double n_c_min = tryParsingDoubleHelper_non_neg(n_c_ratio_box1.Text);
            double n_c_max = tryParsingDoubleHelper_non_neg(n_c_ratio_box2.Text);
            if (n_c_min <= n_c_max)
            {
                localSettings.Values["n_c_ratio_box1"] = n_c_min;
                localSettings.Values["n_c_ratio_box2"] = n_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["p_c_ratio_include"] = p_c_ratio_checkbox.IsChecked;
            double p_c_min = tryParsingDoubleHelper_non_neg(p_c_ratio_box1.Text);
            double p_c_max = tryParsingDoubleHelper_non_neg(p_c_ratio_box2.Text);
            if (p_c_min <= p_c_max)
            {
                localSettings.Values["p_c_ratio_box1"] = p_c_min;
                localSettings.Values["p_c_ratio_box2"] = p_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["s_c_ratio_include"] = s_c_ratio_checkbox.IsChecked;
            double s_c_min = tryParsingDoubleHelper_non_neg(s_c_ratio_box1.Text);
            double s_c_max = tryParsingDoubleHelper_non_neg(s_c_ratio_box2.Text);
            if (s_c_min <= s_c_max)
            {
                localSettings.Values["s_c_ratio_box1"] = s_c_min;
                localSettings.Values["s_c_ratio_box2"] = s_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["f_c_ratio_include"] = f_c_ratio_checkbox.IsChecked;
            double f_c_min = tryParsingDoubleHelper_non_neg(f_c_ratio_box1.Text);
            double f_c_max = tryParsingDoubleHelper_non_neg(f_c_ratio_box2.Text);
            if (f_c_min <= f_c_max)
            {
                localSettings.Values["f_c_ratio_box1"] = f_c_min;
                localSettings.Values["f_c_ratio_box2"] = f_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["cl_c_ratio_include"] = cl_c_ratio_checkbox.IsChecked;
            double cl_c_min = tryParsingDoubleHelper_non_neg(cl_c_ratio_box1.Text);
            double cl_c_max = tryParsingDoubleHelper_non_neg(cl_c_ratio_box2.Text);
            if (cl_c_min <= cl_c_max)
            {
                localSettings.Values["cl_c_ratio_box1"] = cl_c_min;
                localSettings.Values["cl_c_ratio_box2"] = cl_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["br_c_ratio_include"] = br_c_ratio_checkbox.IsChecked;
            double br_c_min = tryParsingDoubleHelper_non_neg(br_c_ratio_box1.Text);
            double br_c_max = tryParsingDoubleHelper_non_neg(br_c_ratio_box2.Text);
            if (br_c_min <= br_c_max)
            {
                localSettings.Values["br_c_ratio_box1"] = br_c_min;
                localSettings.Values["br_c_ratio_box2"] = br_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["si_c_ratio_include"] = si_c_ratio_checkbox.IsChecked;
            double si_c_min = tryParsingDoubleHelper_non_neg(si_c_ratio_box1.Text);
            double si_c_max = tryParsingDoubleHelper_non_neg(si_c_ratio_box2.Text);
            if (si_c_min <= si_c_max)
            {
                localSettings.Values["si_c_ratio_box1"] = si_c_min;
                localSettings.Values["si_c_ratio_box2"] = si_c_max;
            }
            else
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["o_p_ratio_include"] = o_p_ratio_checkbox.IsChecked;

            //timeout
            localSettings.Values["timeout_single_include"] = timeout_single_checkbox.IsChecked;
            double timeoutSingle = tryParsingDoubleHelper_non_neg(timeout_single_box.Text);
            if (timeoutSingle > 30 || timeoutSingle < 0.5)
            {
                throwInvalidInputError = true;
            }
            localSettings.Values["timeout_single"] = timeoutSingle;
            //localSettings.Values["timeout_batch_include"] = timeout_batch_checkbox.IsChecked;
            //localSettings.Values["timeout_batch"] = Double.Parse(timeout_batch_box.Text);

            localSettings.Values["MS2FragReannotate_include"] = MS2FragReannotate_checkbox.IsChecked;

            if (throwInvalidInputError)
            {
                InvalidInputError();
            }
            else
            {
                Window.Current.Close();
            }
        }

        private async void InvalidMS2DBInput()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid MS2 database format.",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void MS2DBInputSuccess()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Success",
                Content = "MS2 database loaded.",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private void ADV_Cancel(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }

        private async void UploadMS2DB_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".msp");
            picker.FileTypeFilter.Add(".MSP");


            var uploadedMS2DB = await picker.PickSingleFileAsync();

            if (uploadedMS2DB != null )
            {
                customMS2DB = await uploadedMS2DB.CopyAsync(storageFolder, uploadedMS2DB.Name, NameCollisionOption.ReplaceExisting);

                loadedMS2DB_text.Text = uploadedMS2DB.Name;

                //try
                //{
                //    newDB = MS2DBManager.ReadMspMS2DB(customMS2DB.Path);
                //}
                //catch
                //{
                //    InvalidMS2DBInput();
                //}
                ////Debug
                //Debug.WriteLine("new DB:   " + newDB.Count);
                //if (newDB.Count > 0)
                //{
                //    MS2DBInputSuccess();
                //}
            }
            else
            {
                NoDBSelected();
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
        // check metabolome databases
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

        private void DeleteSelectedAdducts_Click(object sender, RoutedEventArgs e)
        {
            int[] posIdx = new int[pos_adduct_listview.SelectedItems.Count];
            for (int i = 0; i < pos_adduct_listview.SelectedItems.Count; i++)
            {
                int selectedIndex = pos_adduct_listview.Items.IndexOf(pos_adduct_listview.SelectedItems[i]);
                posIdx[i] = selectedIndex;
            }
            int[] negIdx = new int[neg_adduct_listview.SelectedItems.Count];
            for (int i = 0; i < neg_adduct_listview.SelectedItems.Count; i++)
            {
                int selectedIndex = neg_adduct_listview.Items.IndexOf(neg_adduct_listview.SelectedItems[i]);
                negIdx[i] = selectedIndex;
            }
            bool deleteFailed = false;
            foreach (int indice in posIdx.OrderByDescending(v => v))
            {
                if (adductList_Pos[indice].Formula != "[M+H]+")
                {
                    adductList_Pos.RemoveAt(indice);
                }
                else
                {
                    deleteFailed = true;                    
                }
            }
            foreach (int indice in negIdx.OrderByDescending(v => v))
            {
                if (adductList_Neg[indice].Formula != "[M-H]-")
                {
                    adductList_Neg.RemoveAt(indice);
                }
                else
                {
                    deleteFailed = true;
                }
            }
            if (deleteFailed)
            {
                DeleteAdductFailed();
            }
        }
        private async void DeleteAdductFailed()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Adducts [M+H]+ and [M-H]- cannot be removed.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private void AddAdductToList_Click(object sender, RoutedEventArgs e)
        {
            string customAdduct = custom_adduct_box.Text;
            if (customAdduct.Length < 3)
            {
                InvalidCustomAdduct();
                return;
            }
            customAdduct = customAdduct.Trim();
            customAdduct = Regex.Replace(customAdduct, @"\s+", "");
            if (customAdduct.Substring(0, 1) != "[")
            {
                InvalidCustomAdduct();
                return;
            }

            //if (customAdduct.Substring(customAdduct.Length - 2) != "]+" && customAdduct.Substring(customAdduct.Length - 2) != "]-")
            //{
            //    InvalidCustomAdduct();
            //    return;
            //}
            try
            {
                Adduct add = new Adduct(customAdduct);
                if (add.Mode == "P")
                {
                    if (adductList_Pos.Where(o => o.Formula == add.Formula).ToList().Count > 0)
                    {
                        AdductAlreadyExists();
                        return;
                    }
                    adductList_Pos.Add(add);
                }
                else
                {
                    if (adductList_Neg.Where(o => o.Formula == add.Formula).ToList().Count > 0)
                    {
                        AdductAlreadyExists();
                        return;
                    }
                    adductList_Neg.Add(add);
                }
                CustomAdductSuccess();
            }
            catch
            {
                InvalidCustomAdduct();
                return;
            }
        }

        //private void checkAdductValid(String customAdduct)
        //{
        //    if ()
        //    InvalidCustomAdduct();
        //}

        //private void minC_box_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if(Double.Parse(minC_box.Text) <= C_RangeSelector.RangeEnd)
        //    {
        //        C_RangeSelector.RangeStart = Double.Parse(minC_box.Text);
        //    }
        //    else
        //    {
        //        C_RangeSelector.RangeStart = Double.Parse(localSettings.Values["c_min_default"] as string);
        //    }
        //}

        //private void maxC_box_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (Double.Parse(maxC_box.Text) >= C_RangeSelector.RangeStart)
        //    {
        //        C_RangeSelector.RangeEnd = Double.Parse(maxC_box.Text);
        //    }
        //    else
        //    {
        //        C_RangeSelector.RangeEnd = Double.Parse(localSettings.Values["c_max_default"] as string);
        //    }
        //}
        private async void CustomAdductSuccess()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Success",
                Content = "Custom adduct added.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void InvalidCustomAdduct()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid Adduct",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void AdductAlreadyExists()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "This adduct already exists in the adduct list.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }

        private async void NoDBSelected()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Operation Cancelled.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
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
        private void use_topFrag_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (use_topFrag_checkbox.IsChecked == true)
            {
                use_allFrag_checkbox.IsChecked = false;
            }
            else
            {
                use_allFrag_checkbox.IsChecked = true;
            }
        }

        private void use_allFrag_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (use_allFrag_checkbox.IsChecked == true)
            {
                use_topFrag_checkbox.IsChecked = false;
            }
            else
            {
                use_topFrag_checkbox.IsChecked = true;
            }
        }

        private void ms1tol_switch_Toggled(object sender, RoutedEventArgs e)
        {
            // MS1 and MS2 toggles: on / off simultaneously
            // high resolution: 150 topcandidates; low-res: 250
            if (ms1tol_switch.IsOn == true)
            {
                ms2tol_switch.IsOn = true;
                if ((double)localSettings.Values["ms1tol"] > 100 || (double)localSettings.Values["ms2tol"] > 100)
                {
                    topCandidates_box.Text = "250";
                }
                else
                {
                    topCandidates_box.Text = "150";
                }
            }
            else
            {
                ms2tol_switch.IsOn = false;
                if ((double)localSettings.Values["ms1tol"] >= 0.1 || (double)localSettings.Values["ms2tol"] >= 0.1)
                {
                    topCandidates_box.Text = "250";
                }
                else
                {
                    topCandidates_box.Text = "150";
                }
            }
            localSettings.Values["topCandidates"] = tryParsingIntHelper_pos(topCandidates_box.Text);
        }

        private void ms2tol_switch_Toggled(object sender, RoutedEventArgs e)
        {
            if (ms2tol_switch.IsOn == true)
            {
                ms1tol_switch.IsOn = true;
                if ((double)localSettings.Values["ms1tol"] > 100 || (double)localSettings.Values["ms2tol"] > 100)
                {
                    topCandidates_box.Text = "250";
                }
                else
                {
                    topCandidates_box.Text = "150";
                }
            }
            else
            {
                ms1tol_switch.IsOn = false;
                if ((double)localSettings.Values["ms1tol"] >= 0.1 || (double)localSettings.Values["ms2tol"] >= 0.1)
                {
                    topCandidates_box.Text = "250";
                }
                else
                {
                    topCandidates_box.Text = "150";
                }
            }
            localSettings.Values["topCandidates"] = tryParsingIntHelper_pos(topCandidates_box.Text);
        }

        private void ms2Group_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ms2Merge_checkbox.IsChecked = false;
        }

        private void ms2Merge_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            ms2Group_checkbox.IsChecked = true;
        }

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
        private void ms2DB_selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ms2MatchingAlgorithm_selection.SelectedIndex;
            switch (index)
            {
                case 0:
                    recommendedMetaIdenMS2Score_text.Text = "0.7";
                    recommendedMetaIdenMS2MinMatchedFragNo_text.Text = "6";
                    break;
                case 1:
                    recommendedMetaIdenMS2Score_text.Text = "0.7";
                    recommendedMetaIdenMS2MinMatchedFragNo_text.Text = "6";
                    break;
                case 2:
                    recommendedMetaIdenMS2Score_text.Text = "0.75";
                    recommendedMetaIdenMS2MinMatchedFragNo_text.Text = "0";
                    break;
                default:
                    break;
            }
        }

        private void use_topCandidates_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (use_topCandidates_checkbox.IsChecked == true)
            {
                use_allCandidates_checkbox.IsChecked = false;
            }
            else
            {
                use_allCandidates_checkbox.IsChecked = true;
            }
        }

        private void use_allCandidates_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (use_allCandidates_checkbox.IsChecked == true)
            {
                use_topCandidates_checkbox.IsChecked = false;
                UseAllCandidateWarning();
            }
            else
            {
                use_topCandidates_checkbox.IsChecked = true;
            }
        }
        private async void UseAllCandidateWarning()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Including all candidates may lead to longer computational time.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void InvalidInputError()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid Input.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }

        // Try parsing from input string to int
        // Throw InvalidInputError() if the input string is not a non-negative double
        private double tryParsingDoubleHelper_non_neg(string s)
        {
            if (Double.TryParse(s, out double input))
            {
                if (input >= 0)
                {
                    return input;
                }
                else
                {
                    throwInvalidInputError = true;
                    return -1.0;
                }
            }
            else
            {
                throwInvalidInputError = true;
                return -1.0;
            }
        }

        // Try parsing from input string to int
        // Throw InvalidInputError() if the input string is not a non negative int
        private int tryParsingIntHelper_non_neg(string s)
        {
            int input;
            if (Int32.TryParse(s, out input))
            {
                if (input >= 0)
                {
                    return input;
                }
                else
                {
                    throwInvalidInputError = true;
                    return -1;
                }
            }
            else
            {
                throwInvalidInputError = true;
                return -1;
            }
        }

        // Try parsing from input string to int
        // Throw InvalidInputError() if the input string is not a positive int
        private int tryParsingIntHelper_pos(string s)
        {
            int input;
            if (Int32.TryParse(s, out input))
            {
                if (input > 0)
                {
                    return input;
                }
                else
                {
                    throwInvalidInputError = true;
                    return -1;
                }
            }
            else
            {
                throwInvalidInputError = true;
                return -1;
            }
        }

        // Try parsing from input string to double
        // Throw InvalidInputError() if the input string is outside [0,100]
        private double tryParsingDoubleHelper_for_percentage(string s)
        {
            double input;
            if (Double.TryParse(s, out input))
            {
                if (input >= 0 && input <= 100)
                {
                    return input;
                }
                else
                {
                    throwInvalidInputError = true;
                    return -1.0;
                }
            }
            else
            {
                throwInvalidInputError = true;
                return -1.0;
            }
        }

        // Try parsing from input string to double
        // Throw InvalidInputError() if the input string is outside [0,1]
        private double tryParsingDoubleHelper_for_cutoff(string s)
        {
            double input;
            if (Double.TryParse(s, out input))
            {
                if (input >= 0 && input <= 1)
                {
                    return input;
                }
                else
                {
                    throwInvalidInputError = true;
                    return -1.0;
                }
            }
            else
            {
                throwInvalidInputError = true;
                return -1.0;
            }

        }

    }
}
