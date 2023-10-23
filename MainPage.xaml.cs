using BUDDY.BufpHelper;
using BUDDY.FormulaData;
using BUDDY.Helper;
using BUDDY.MgfHandler;
using BUDDY.MsdialDataHandler;
using BUDDY.MzmlDataHandler.Parser;
using BUDDY.RawData;
using BUDDY.RawDataHandlerCommon;
using BUDDY.GlobalOptimizationMILP;
using MoreLinq;
using NCDK;
using NCDK.Formula;
using NCDK.Tools.Manipulator;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO.Compression;
using Windows.Storage.Pickers;
using System.Threading;
using System.Globalization;
using Windows.Graphics.Display;
using System.Data;
using MathNet.Numerics.Statistics;
using Google.OrTools.LinearSolver;
using Google.OrTools;
using System.Net;
using BUDDY.MS2LibrarySearch;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using System.Xml;
using System.Collections.Concurrent;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BUDDY
{
    //using CustomExtension;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        Ms2Utility MS2compareTop;
        Ms2Utility MS2compareBottom;
        //ObservableCollection<CFDF> f_dbeHist = new ObservableCollection<CFDF>();
        //ObservableCollection<CFDF> nl_dbeHist = new ObservableCollection<CFDF>();
        //ObservableCollection<CFDF> f_h2cHist = new ObservableCollection<CFDF>();
        //ObservableCollection<CFDF> nl_h2cHist = new ObservableCollection<CFDF>();
        //ObservableCollection<CFDF> f_hetero2cHist = new ObservableCollection<CFDF>();
        //ObservableCollection<CFDF> nl_hetero2cHist = new ObservableCollection<CFDF>();
        public int GROUP_MZ_RANGE = 1;
        public List<List<AlignedFormula>> groupedDB = new List<List<AlignedFormula>>();
        public List<Distribution> distPos_df = new List<Distribution>();
        public List<Distribution> distNeg_df = new List<Distribution>();
        public List<Distribution> pre_dist_df = new List<Distribution>();
        public List<Connection> connection_df = new List<Connection>();
        public List<MS2DBEntry> MS2DB { get; set; }

        ObservableCollection<Database> candidateDB_list = new ObservableCollection<Database>();
        StorageFile openProjectFile = null;
        StorageFile modelFile1 = null;
        StorageFile modelFile2 = null;
        StorageFile modelFile3 = null;
        StorageFile modelFile4 = null;
        StorageFile modelFile5 = null;
        StorageFile modelFile6 = null;
        StorageFile modelFile7 = null;
        StorageFile modelFile8 = null;

        Uri USERMANUAL_URI = new Uri(@"https://philipbear.github.io/BUDDY_Metabolomics/");

        int MAX_CANDIDATE_SAVED = 50;
        bool allDatabaseLoaded = false;
        Task loadingDatabase;
        string loadedcustomMS2DB = "";


        public double E_MASS = 0.0005485; // electron
        public double P_MASS = 1.007276; // proton
        public double H_MASS = 1.007825; // H
        public double Na_MASS = 22.989769;
        public double K_MASS = 38.963707;
        bool calculationInProgress = false;
        bool exportInProgress = false;

        public MainPage()
        {
            // default settings
            #region
            //default general
            localSettings.Values["MS2LibrarySearch_include_default"] = false;
            if (!localSettings.Values.ContainsKey("MS2LibrarySearch_include"))
                localSettings.Values["MS2LibrarySearch_include"] = localSettings.Values["MS2LibrarySearch_include_default"];
            localSettings.Values["BUDDY_include_default"] = true;
            if (!localSettings.Values.ContainsKey("BUDDY_include"))
                localSettings.Values["BUDDY_include"] = localSettings.Values["BUDDY_include_default"];
            localSettings.Values["ExpSpecificGlobalAnnotation_include_default"] = false;
            if (!localSettings.Values.ContainsKey("ExpSpecificGlobalAnnotation_include"))
                localSettings.Values["ExpSpecificGlobalAnnotation_include"] = localSettings.Values["ExpSpecificGlobalAnnotation_include_default"];

            localSettings.Values["selectedMSinstrument_default"] = 1;
            if (!localSettings.Values.ContainsKey("selectedMSinstrument"))
                localSettings.Values["selectedMSinstrument"] = localSettings.Values["selectedMSinstrument_default"];
            localSettings.Values["ms1tol_default"] = (double)5;
            localSettings.Values["ms2tol_default"] = (double)10;
            localSettings.Values["ms1tol_ppmON_default"] = true;
            localSettings.Values["ms2tol_ppmON_default"] = true;
            if (!localSettings.Values.ContainsKey("ms1tol"))
                localSettings.Values["ms1tol"] = localSettings.Values["ms1tol_default"];
            if (!localSettings.Values.ContainsKey("ms2tol"))
                localSettings.Values["ms2tol"] = localSettings.Values["ms2tol_default"];
            if (!localSettings.Values.ContainsKey("ms1tol_ppmON"))
                localSettings.Values["ms1tol_ppmON"] = localSettings.Values["ms1tol_ppmON_default"];
            if (!localSettings.Values.ContainsKey("ms2tol_ppmON"))
                localSettings.Values["ms2tol_ppmON"] = localSettings.Values["ms2tol_ppmON_default"];
            localSettings.Values["topFrag_default"] = 50;
            localSettings.Values["use_topFrag_default"] = true;
            localSettings.Values["use_allFrag_default"] = false;
            localSettings.Values["ms2Deisotope_default"] = true;
            localSettings.Values["ms2Denoise_default"] = true;
            localSettings.Values["maxNoiseInt_default"] = (double)5e3;
            localSettings.Values["maxNoiseFragRatio_default"] = (double)80;
            localSettings.Values["maxNoiseRSD_default"] = (double)0.25;
            if (!localSettings.Values.ContainsKey("topFrag"))
                localSettings.Values["topFrag"] = localSettings.Values["topFrag_default"];
            if (!localSettings.Values.ContainsKey("use_topFrag"))
                localSettings.Values["use_topFrag"] = localSettings.Values["use_topFrag_default"];
            if (!localSettings.Values.ContainsKey("use_allFrag"))
                localSettings.Values["use_allFrag"] = localSettings.Values["use_allFrag_default"];
            if (!localSettings.Values.ContainsKey("ms2Deisotope"))
                localSettings.Values["ms2Deisotope"] = localSettings.Values["ms2Deisotope_default"];
            if (!localSettings.Values.ContainsKey("ms2Denoise"))
                localSettings.Values["ms2Denoise"] = localSettings.Values["ms2Denoise_default"];
            if (!localSettings.Values.ContainsKey("maxNoiseInt"))
                localSettings.Values["maxNoiseInt"] = localSettings.Values["maxNoiseInt_default"];
            if (!localSettings.Values.ContainsKey("maxNoiseFragRatio"))
                localSettings.Values["maxNoiseFragRatio"] = localSettings.Values["maxNoiseFragRatio_default"];
            if (!localSettings.Values.ContainsKey("maxNoiseRSD"))
                localSettings.Values["maxNoiseRSD"] = localSettings.Values["maxNoiseRSD_default"];


            localSettings.Values["MetaScore_include_default"] = false;
            if (!localSettings.Values.ContainsKey("MetaScore_include"))
                localSettings.Values["MetaScore_include"] = localSettings.Values["MetaScore_include_default"];

            // default ms2 library search

            localSettings.Values["useCustomMS2DB_include_default"] = false;
            if (!localSettings.Values.ContainsKey("useCustomMS2DB_include"))
                localSettings.Values["useCustomMS2DB_include"] = localSettings.Values["useCustomMS2DB_include_default"];

            //localSettings.Values["customMS2DBName"] = ""; // DB name of custom MS2DB, ready to be read

            localSettings.Values["ms2MatchingAlgorithm_default"] = 0;
            if (!localSettings.Values.ContainsKey("ms2MatchingAlgorithm"))
                localSettings.Values["ms2MatchingAlgorithm"] = localSettings.Values["ms2MatchingAlgorithm_default"];

            localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold_default"] = (double)0.7;
            if (!localSettings.Values.ContainsKey("metaboliteIdentificationMS2SimilarityScoreThreshold"))
                localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold"] = localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold_default"];

            localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include_default"] = true;
            if (!localSettings.Values.ContainsKey("metaboliteIdentificationMS2MinMatchedFrag_include"))
                localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include"] = localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include_default"];

            localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo_default"] = 6;
            if (!localSettings.Values.ContainsKey("metaboliteIdentificationMS2MinMatchedFragNo"))
                localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo"] = localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo_default"];

            localSettings.Values["metaboliteIdentificationRT_include_default"] = false;
            if (!localSettings.Values.ContainsKey("metaboliteIdentificationRT_include"))
                localSettings.Values["metaboliteIdentificationRT_include"] = localSettings.Values["metaboliteIdentificationRT_include_default"];

            localSettings.Values["metaboliteIdentificationRTtol_default"] = (double)0.2;
            if (!localSettings.Values.ContainsKey("metaboliteIdentificationRTtol"))
                localSettings.Values["metaboliteIdentificationRTtol"] = localSettings.Values["metaboliteIdentificationRTtol_default"];

            //default elements (range)
            localSettings.Values["c_min_default"] = 0;
            localSettings.Values["c_max_default"] = 80;
            localSettings.Values["h_min_default"] = 0;
            localSettings.Values["h_max_default"] = 150;
            localSettings.Values["n_min_default"] = 0;
            localSettings.Values["n_max_default"] = 20;
            localSettings.Values["o_min_default"] = 0;
            localSettings.Values["o_max_default"] = 30;
            localSettings.Values["p_min_default"] = 0;
            localSettings.Values["p_max_default"] = 10;
            localSettings.Values["s_min_default"] = 0;
            localSettings.Values["s_max_default"] = 15;
            localSettings.Values["f_min_default"] = 0;
            localSettings.Values["f_max_default"] = 20;
            localSettings.Values["cl_min_default"] = 0;
            localSettings.Values["cl_max_default"] = 15;
            localSettings.Values["br_min_default"] = 0;
            localSettings.Values["br_max_default"] = 10;
            localSettings.Values["i_min_default"] = 0;
            localSettings.Values["i_max_default"] = 10;
            localSettings.Values["si_min_default"] = 0;
            localSettings.Values["si_max_default"] = 15;
            localSettings.Values["b_min_default"] = 0;
            localSettings.Values["b_max_default"] = 10;
            localSettings.Values["se_min_default"] = 0;
            localSettings.Values["se_max_default"] = 10;
            if (!localSettings.Values.ContainsKey("c_min"))
                localSettings.Values["c_min"] = localSettings.Values["c_min_default"];
            if (!localSettings.Values.ContainsKey("c_max"))
                localSettings.Values["c_max"] = localSettings.Values["c_max_default"];
            if (!localSettings.Values.ContainsKey("h_min"))
                localSettings.Values["h_min"] = localSettings.Values["h_min_default"];
            if (!localSettings.Values.ContainsKey("h_max"))
                localSettings.Values["h_max"] = localSettings.Values["h_max_default"];
            if (!localSettings.Values.ContainsKey("n_min"))
                localSettings.Values["n_min"] = localSettings.Values["n_min_default"];
            if (!localSettings.Values.ContainsKey("n_max"))
                localSettings.Values["n_max"] = localSettings.Values["n_max_default"];
            if (!localSettings.Values.ContainsKey("o_min"))
                localSettings.Values["o_min"] = localSettings.Values["o_min_default"];
            if (!localSettings.Values.ContainsKey("o_max"))
                localSettings.Values["o_max"] = localSettings.Values["o_max_default"];
            if (!localSettings.Values.ContainsKey("p_min"))
                localSettings.Values["p_min"] = localSettings.Values["p_min_default"];
            if (!localSettings.Values.ContainsKey("p_max"))
                localSettings.Values["p_max"] = localSettings.Values["p_max_default"];
            if (!localSettings.Values.ContainsKey("s_min"))
                localSettings.Values["s_min"] = localSettings.Values["s_min_default"];
            if (!localSettings.Values.ContainsKey("s_max"))
                localSettings.Values["s_max"] = localSettings.Values["s_max_default"];
            if (!localSettings.Values.ContainsKey("f_min"))
                localSettings.Values["f_min"] = localSettings.Values["f_min_default"];
            if (!localSettings.Values.ContainsKey("f_max"))
                localSettings.Values["f_max"] = localSettings.Values["f_max_default"];
            if (!localSettings.Values.ContainsKey("cl_min"))
                localSettings.Values["cl_min"] = localSettings.Values["cl_min_default"];
            if (!localSettings.Values.ContainsKey("cl_max"))
                localSettings.Values["cl_max"] = localSettings.Values["cl_max_default"];
            if (!localSettings.Values.ContainsKey("br_min"))
                localSettings.Values["br_min"] = localSettings.Values["br_min_default"];
            if (!localSettings.Values.ContainsKey("br_max"))
                localSettings.Values["br_max"] = localSettings.Values["br_max_default"];
            if (!localSettings.Values.ContainsKey("i_min"))
                localSettings.Values["i_min"] = localSettings.Values["i_min_default"];
            if (!localSettings.Values.ContainsKey("i_max"))
                localSettings.Values["i_max"] = localSettings.Values["i_max_default"];
            if (!localSettings.Values.ContainsKey("si_min"))
                localSettings.Values["si_min"] = localSettings.Values["si_min_default"];
            if (!localSettings.Values.ContainsKey("si_max"))
                localSettings.Values["si_max"] = localSettings.Values["si_max_default"];
            if (!localSettings.Values.ContainsKey("b_min"))
                localSettings.Values["b_min"] = localSettings.Values["b_min_default"];
            if (!localSettings.Values.ContainsKey("b_max"))
                localSettings.Values["b_max"] = localSettings.Values["b_max_default"];
            if (!localSettings.Values.ContainsKey("se_min"))
                localSettings.Values["se_min"] = localSettings.Values["se_min_default"];
            if (!localSettings.Values.ContainsKey("se_max"))
                localSettings.Values["se_max"] = localSettings.Values["se_max_default"];
            //default elements (include?)
            localSettings.Values["c_include_default"] = true;
            localSettings.Values["h_include_default"] = true;
            localSettings.Values["n_include_default"] = true;
            localSettings.Values["o_include_default"] = true;
            localSettings.Values["p_include_default"] = true;
            localSettings.Values["s_include_default"] = true;
            localSettings.Values["f_include_default"] = false;
            localSettings.Values["cl_include_default"] = false;
            localSettings.Values["br_include_default"] = false;
            localSettings.Values["i_include_default"] = false;
            localSettings.Values["si_include_default"] = false;
            localSettings.Values["b_include_default"] = false;
            localSettings.Values["se_include_default"] = false;
            if (!localSettings.Values.ContainsKey("c_include"))
                localSettings.Values["c_include"] = localSettings.Values["c_include_default"];
            if (!localSettings.Values.ContainsKey("h_include"))
                localSettings.Values["h_include"] = localSettings.Values["h_include_default"];
            if (!localSettings.Values.ContainsKey("n_include"))
                localSettings.Values["n_include"] = localSettings.Values["n_include_default"];
            if (!localSettings.Values.ContainsKey("o_include"))
                localSettings.Values["o_include"] = localSettings.Values["o_include_default"];
            if (!localSettings.Values.ContainsKey("p_include"))
                localSettings.Values["p_include"] = localSettings.Values["p_include_default"];
            if (!localSettings.Values.ContainsKey("s_include"))
                localSettings.Values["s_include"] = localSettings.Values["s_include_default"];
            if (!localSettings.Values.ContainsKey("f_include"))
                localSettings.Values["f_include"] = localSettings.Values["f_include_default"];
            if (!localSettings.Values.ContainsKey("cl_include"))
                localSettings.Values["cl_include"] = localSettings.Values["cl_include_default"];
            if (!localSettings.Values.ContainsKey("br_include"))
                localSettings.Values["br_include"] = localSettings.Values["br_include_default"];
            if (!localSettings.Values.ContainsKey("i_include"))
                localSettings.Values["i_include"] = localSettings.Values["i_include_default"];
            if (!localSettings.Values.ContainsKey("si_include"))
                localSettings.Values["si_include"] = localSettings.Values["si_include_default"];
            if (!localSettings.Values.ContainsKey("b_include"))
                localSettings.Values["b_include"] = localSettings.Values["b_include_default"];
            if (!localSettings.Values.ContainsKey("se_include"))
                localSettings.Values["se_include"] = localSettings.Values["se_include_default"];

            //default formula database selection
            localSettings.Values["NoResDB_include_default"] = false;
            if (!localSettings.Values.ContainsKey("NoResDB_include"))
                localSettings.Values["NoResDB_include"] = localSettings.Values["NoResDB_include_default"];
            localSettings.Values["PubChem_include_default"] = true;
            localSettings.Values["mtb_lpd_include_default"] = true;
            localSettings.Values["BMDB_include_default"] = true;
            localSettings.Values["ChEBI_include_default"] = true;
            localSettings.Values["ECMDB_include_default"] = true;
            localSettings.Values["FooDB_include_default"] = true;
            localSettings.Values["HMDB_include_default"] = true;
            localSettings.Values["KEGG_include_default"] = true;
            localSettings.Values["LMSD_include_default"] = true;
            localSettings.Values["MarkerDB_include_default"] = true;
            localSettings.Values["MCDB_include_default"] = true;
            localSettings.Values["PlantCyc_include_default"] = true;
            localSettings.Values["SMPDB_include_default"] = true;
            localSettings.Values["YMDB_include_default"] = true;
            localSettings.Values["drug_txn_include_default"] = false;
            localSettings.Values["DrugBank_include_default"] = false;
            localSettings.Values["DSSTOX_include_default"] = false;
            localSettings.Values["HSDB_include_default"] = false;
            localSettings.Values["T3DB_include_default"] = false;
            localSettings.Values["TTD_include_default"] = false;
            localSettings.Values["natProducts_include_default"] = false;
            localSettings.Values["ANPDB_include_default"] = false;
            localSettings.Values["COCONUT_include_default"] = false;
            localSettings.Values["NPASS_include_default"] = false;
            localSettings.Values["UNPD_include_default"] = false;
            localSettings.Values["xbiotics_include_default"] = false;
            localSettings.Values["BLEXP_include_default"] = false;
            localSettings.Values["NORMAN_include_default"] = false;
            localSettings.Values["STF_IDENT_include_default"] = false;
            localSettings.Values["contaminants_include_default"] = false;
            localSettings.Values["MaConDa_include_default"] = false;
            if (!localSettings.Values.ContainsKey("PubChem_include"))
                localSettings.Values["PubChem_include"] = localSettings.Values["PubChem_include_default"];
            if (!localSettings.Values.ContainsKey("mtb_lpd_include"))
                localSettings.Values["mtb_lpd_include"] = localSettings.Values["mtb_lpd_include_default"];
            if (!localSettings.Values.ContainsKey("BMDB_include"))
                localSettings.Values["BMDB_include"] = localSettings.Values["BMDB_include_default"];
            if (!localSettings.Values.ContainsKey("ChEBI_include"))
                localSettings.Values["ChEBI_include"] = localSettings.Values["ChEBI_include_default"];
            if (!localSettings.Values.ContainsKey("ECMDB_include"))
                localSettings.Values["ECMDB_include"] = localSettings.Values["ECMDB_include_default"];
            if (!localSettings.Values.ContainsKey("FooDB_include"))
                localSettings.Values["FooDB_include"] = localSettings.Values["FooDB_include_default"];
            if (!localSettings.Values.ContainsKey("HMDB_include"))
                localSettings.Values["HMDB_include"] = localSettings.Values["HMDB_include_default"];
            if (!localSettings.Values.ContainsKey("KEGG_include"))
                localSettings.Values["KEGG_include"] = localSettings.Values["KEGG_include_default"];
            if (!localSettings.Values.ContainsKey("LMSD_include"))
                localSettings.Values["LMSD_include"] = localSettings.Values["LMSD_include_default"];
            if (!localSettings.Values.ContainsKey("MarkerDB_include"))
                localSettings.Values["MarkerDB_include"] = localSettings.Values["MarkerDB_include_default"];
            if (!localSettings.Values.ContainsKey("MCDB_include"))
                localSettings.Values["MCDB_include"] = localSettings.Values["MCDB_include_default"];
            if (!localSettings.Values.ContainsKey("PlantCyc_include"))
                localSettings.Values["PlantCyc_include"] = localSettings.Values["PlantCyc_include_default"];
            if (!localSettings.Values.ContainsKey("SMPDB_include"))
                localSettings.Values["SMPDB_include"] = localSettings.Values["SMPDB_include_default"];
            if (!localSettings.Values.ContainsKey("YMDB_include"))
                localSettings.Values["YMDB_include"] = localSettings.Values["YMDB_include_default"];
            if (!localSettings.Values.ContainsKey("drug_txn_include"))
                localSettings.Values["drug_txn_include"] = localSettings.Values["drug_txn_include_default"];
            if (!localSettings.Values.ContainsKey("DrugBank_include"))
                localSettings.Values["DrugBank_include"] = localSettings.Values["DrugBank_include_default"];
            if (!localSettings.Values.ContainsKey("DSSTOX_include"))
                localSettings.Values["DSSTOX_include"] = localSettings.Values["DSSTOX_include_default"];
            if (!localSettings.Values.ContainsKey("HSDB_include"))
                localSettings.Values["HSDB_include"] = localSettings.Values["HSDB_include_default"];
            if (!localSettings.Values.ContainsKey("T3DB_include"))
                localSettings.Values["T3DB_include"] = localSettings.Values["T3DB_include_default"];
            if (!localSettings.Values.ContainsKey("TTD_include"))
                localSettings.Values["TTD_include"] = localSettings.Values["TTD_include_default"];
            if (!localSettings.Values.ContainsKey("natProducts_include"))
                localSettings.Values["natProducts_include"] = localSettings.Values["natProducts_include_default"];
            if (!localSettings.Values.ContainsKey("ANPDB_include"))
                localSettings.Values["ANPDB_include"] = localSettings.Values["ANPDB_include_default"];
            if (!localSettings.Values.ContainsKey("COCONUT_include"))
                localSettings.Values["COCONUT_include"] = localSettings.Values["COCONUT_include_default"];
            if (!localSettings.Values.ContainsKey("NPASS_include"))
                localSettings.Values["NPASS_include"] = localSettings.Values["NPASS_include_default"];
            if (!localSettings.Values.ContainsKey("UNPD_include"))
                localSettings.Values["UNPD_include"] = localSettings.Values["UNPD_include_default"];
            if (!localSettings.Values.ContainsKey("xbiotics_include"))
                localSettings.Values["xbiotics_include"] = localSettings.Values["xbiotics_include_default"];
            if (!localSettings.Values.ContainsKey("BLEXP_include"))
                localSettings.Values["BLEXP_include"] = localSettings.Values["BLEXP_include_default"];
            if (!localSettings.Values.ContainsKey("NORMAN_include"))
                localSettings.Values["NORMAN_include"] = localSettings.Values["NORMAN_include_default"];
            if (!localSettings.Values.ContainsKey("STF_IDENT_include"))
                localSettings.Values["STF_IDENT_include"] = localSettings.Values["STF_IDENT_include_default"];
            if (!localSettings.Values.ContainsKey("contaminants_include"))
                localSettings.Values["contaminants_include"] = localSettings.Values["contaminants_include_default"];
            if (!localSettings.Values.ContainsKey("MaConDa_include"))
                localSettings.Values["MaConDa_include"] = localSettings.Values["MaConDa_include_default"];

            //Adduct List
            List<Adduct> adductList_Pos_Default = new List<Adduct>();
            adductList_Pos_Default.Add(new Adduct("[M+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M+NH4]+"));
            adductList_Pos_Default.Add(new Adduct("[M+Na]+"));
            adductList_Pos_Default.Add(new Adduct("[M+CH3OH+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M+K]+"));
            //adductList_Pos_Default.Add(new Adduct("[M+Li]+"));
            adductList_Pos_Default.Add(new Adduct("[M+C2H3N+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M+H-H2O]+"));
            adductList_Pos_Default.Add(new Adduct("[M+H-2H2O]+"));
            adductList_Pos_Default.Add(new Adduct("[M+C3H8O+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M+C2H3N+Na]+"));
            adductList_Pos_Default.Add(new Adduct("[M+C2H6OS+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M+C4H6N2+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M+C3H8O+Na+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M-C6H10O4+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M-C6H10O5+H]+"));
            adductList_Pos_Default.Add(new Adduct("[M-C6H8O6+H]+"));
            //adductList_Pos_Default.Add(new Adduct("[2M+H]+"));
            //adductList_Pos_Default.Add(new Adduct("[2M+NH4]+"));
            //adductList_Pos_Default.Add(new Adduct("[2M+Na]+"));
            //adductList_Pos_Default.Add(new Adduct("[2M+K]+"));
            List<Adduct> adductList_Neg_Default = new List<Adduct>();
            adductList_Neg_Default.Add(new Adduct("[M-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M-H2O-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+Na-2H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+Cl]-"));
            adductList_Neg_Default.Add(new Adduct("[M+K-2H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+HCOOH-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+CH3COOH-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+C2H3N+Na-2H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+Br]-"));
            adductList_Neg_Default.Add(new Adduct("[M+CF3COOH-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M-C6H10O4-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M-C6H10O5-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M-C6H8O6-H]-"));
            adductList_Neg_Default.Add(new Adduct("[M+CH3COONa-H]-"));
            //adductList_Neg_Default.Add(new Adduct("[2M-H]-"));
            //adductList_Neg_Default.Add(new Adduct("[2M+Cl]-"));
            //serialize
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos_Default.bin", FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, adductList_Pos_Default);
            }
            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg_Default.bin", FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, adductList_Neg_Default);
            }
            AsyncAdductListStorage("adductList_Pos.bin", @"\adductList_Pos_Default.bin");
            AsyncAdductListStorage("adductList_Neg.bin", @"\adductList_Neg_Default.bin");
            //for (int i = 0; i < adductList_Pos_Default.Count; i++)
            //{
            //    //Debug.WriteLine(adductList_Pos_Default[i].Formula);
            //}

            //for (int i = 0; i < adductList_Neg_Default.Count; i++)
            //{
            //    //Debug.WriteLine(adductList_Neg_Default[i].Formula);
            //}

            //raw data processing
            //localSettings.Values["isotopic_pattern_imp_default"] = true;
            localSettings.Values["featureClusterMsdial_include_default"] = true;
            localSettings.Values["max_isotopic_peaks_default"] = 3;
            localSettings.Values["isotopic_abundance_cutoff_default"] = (double)1;
            localSettings.Values["isotopologue_grp_masstol_default"] = (double)0.02;
            localSettings.Values["ms2_group_imp_default"] = true;
            localSettings.Values["cos_similarity_cutoff_default"] = (double)0.8;
            localSettings.Values["max_peak_width_default"] = (double)120;
            localSettings.Values["ms2_merging_default"] = true;
            //if (!localSettings.Values.ContainsKey("isotopic_pattern_imp"))
            //    localSettings.Values["isotopic_pattern_imp"] = localSettings.Values["isotopic_pattern_imp_default"];
            if (!localSettings.Values.ContainsKey("featureClusterMsdial_include"))
                localSettings.Values["featureClusterMsdial_include"] = localSettings.Values["featureClusterMsdial_include_default"];
            if (!localSettings.Values.ContainsKey("max_isotopic_peaks"))
                localSettings.Values["max_isotopic_peaks"] = localSettings.Values["max_isotopic_peaks_default"];
            if (!localSettings.Values.ContainsKey("isotopic_abundance_cutoff"))
                localSettings.Values["isotopic_abundance_cutoff"] = localSettings.Values["isotopic_abundance_cutoff_default"];
            if (!localSettings.Values.ContainsKey("isotopologue_grp_masstol"))
                localSettings.Values["isotopologue_grp_masstol"] = localSettings.Values["isotopologue_grp_masstol_default"];
            if (!localSettings.Values.ContainsKey("ms2_group_imp"))
                localSettings.Values["ms2_group_imp"] = localSettings.Values["ms2_group_imp_default"];
            if (!localSettings.Values.ContainsKey("cos_similarity_cutoff"))
                localSettings.Values["cos_similarity_cutoff"] = localSettings.Values["cos_similarity_cutoff_default"];
            if (!localSettings.Values.ContainsKey("max_peak_width"))
                localSettings.Values["max_peak_width"] = localSettings.Values["max_peak_width_default"];
            if (!localSettings.Values.ContainsKey("ms2_merging"))
                localSettings.Values["ms2_merging"] = localSettings.Values["ms2_merging_default"];

            //candidate filtration
            localSettings.Values["topCandidates_default"] = 150;
            localSettings.Values["use_topCandidates_default"] = true;
            localSettings.Values["use_allCandidates_default"] = false;
            if (!localSettings.Values.ContainsKey("topCandidates"))
                localSettings.Values["topCandidates"] = localSettings.Values["topCandidates_default"];
            if (!localSettings.Values.ContainsKey("use_topCandidates"))
                localSettings.Values["use_topCandidates"] = localSettings.Values["use_topCandidates_default"];
            if (!localSettings.Values.ContainsKey("use_allCandidates"))
                localSettings.Values["use_allCandidates"] = localSettings.Values["use_allCandidates_default"];
            //localSettings.Values["expMS2fragNum_include_default"] = true;
            //localSettings.Values["expMS2fragNum_default"] = (double)0;
            //localSettings.Values["expMS2fragInt_include_default"] = true;
            //localSettings.Values["expMS2fragInt_default"] = (double)0;
            localSettings.Values["ispPatternSim_include_default"] = true;
            localSettings.Values["ispPatternSim_default"] = (double)0.3;

            localSettings.Values["h_c_ratio_include_default"] = true;
            localSettings.Values["h_c_ratio_box1_default"] = (double)0.2;
            localSettings.Values["h_c_ratio_box2_default"] = (double)3.1;
            localSettings.Values["o_c_ratio_include_default"] = true;
            localSettings.Values["o_c_ratio_box1_default"] = (double)0;
            localSettings.Values["o_c_ratio_box2_default"] = (double)1.2;
            localSettings.Values["n_c_ratio_include_default"] = true;
            localSettings.Values["n_c_ratio_box1_default"] = (double)0;
            localSettings.Values["n_c_ratio_box2_default"] = (double)1.3;
            localSettings.Values["p_c_ratio_include_default"] = true;
            localSettings.Values["p_c_ratio_box1_default"] = (double)0;
            localSettings.Values["p_c_ratio_box2_default"] = (double)0.3;
            localSettings.Values["s_c_ratio_include_default"] = true;
            localSettings.Values["s_c_ratio_box1_default"] = (double)0;
            localSettings.Values["s_c_ratio_box2_default"] = (double)0.8;
            localSettings.Values["f_c_ratio_include_default"] = true;
            localSettings.Values["f_c_ratio_box1_default"] = (double)0;
            localSettings.Values["f_c_ratio_box2_default"] = (double)1.5;
            localSettings.Values["cl_c_ratio_include_default"] = true;
            localSettings.Values["cl_c_ratio_box1_default"] = (double)0;
            localSettings.Values["cl_c_ratio_box2_default"] = (double)0.8;
            localSettings.Values["br_c_ratio_include_default"] = true;
            localSettings.Values["br_c_ratio_box1_default"] = (double)0;
            localSettings.Values["br_c_ratio_box2_default"] = (double)0.8;
            localSettings.Values["si_c_ratio_include_default"] = true;
            localSettings.Values["si_c_ratio_box1_default"] = (double)0;
            localSettings.Values["si_c_ratio_box2_default"] = (double)0.5;
            localSettings.Values["o_p_ratio_include_default"] = true;

            //localSettings.Values["h_c_ratio_include_default"] = true;
            //localSettings.Values["h_c_ratio_box1_default"] = (double)0.1;
            //localSettings.Values["h_c_ratio_box2_default"] = (double)6;
            //localSettings.Values["o_c_ratio_include_default"] = true;
            //localSettings.Values["o_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["o_c_ratio_box2_default"] = (double)3;
            //localSettings.Values["n_c_ratio_include_default"] = true;
            //localSettings.Values["n_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["n_c_ratio_box2_default"] = (double)4;
            //localSettings.Values["p_c_ratio_include_default"] = true;
            //localSettings.Values["p_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["p_c_ratio_box2_default"] = (double)2;
            //localSettings.Values["s_c_ratio_include_default"] = true;
            //localSettings.Values["s_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["s_c_ratio_box2_default"] = (double)3;
            //localSettings.Values["f_c_ratio_include_default"] = true;
            //localSettings.Values["f_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["f_c_ratio_box2_default"] = (double)6;
            //localSettings.Values["cl_c_ratio_include_default"] = true;
            //localSettings.Values["cl_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["cl_c_ratio_box2_default"] = (double)2;
            //localSettings.Values["br_c_ratio_include_default"] = true;
            //localSettings.Values["br_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["br_c_ratio_box2_default"] = (double)2;
            //localSettings.Values["si_c_ratio_include_default"] = true;
            //localSettings.Values["si_c_ratio_box1_default"] = (double)0;
            //localSettings.Values["si_c_ratio_box2_default"] = (double)1;
            //localSettings.Values["o_p_ratio_include_default"] = true;

            //if (!localSettings.Values.ContainsKey("expMS2fragNum_include"))
            //    localSettings.Values["expMS2fragNum_include"] = localSettings.Values["expMS2fragNum_include_default"];
            //if (!localSettings.Values.ContainsKey("expMS2fragNum"))
            //    localSettings.Values["expMS2fragNum"] = localSettings.Values["expMS2fragNum_default"];
            //if (!localSettings.Values.ContainsKey("expMS2fragInt_include"))
            //    localSettings.Values["expMS2fragInt_include"] = localSettings.Values["expMS2fragInt_include_default"];
            //if (!localSettings.Values.ContainsKey("expMS2fragInt"))
            //    localSettings.Values["expMS2fragInt"] = localSettings.Values["expMS2fragInt_default"];
            if (!localSettings.Values.ContainsKey("ispPatternSim_include"))
                localSettings.Values["ispPatternSim_include"] = localSettings.Values["ispPatternSim_include_default"];
            if (!localSettings.Values.ContainsKey("ispPatternSim"))
                localSettings.Values["ispPatternSim"] = localSettings.Values["ispPatternSim_default"];
            if (!localSettings.Values.ContainsKey("h_c_ratio_include"))
                localSettings.Values["h_c_ratio_include"] = localSettings.Values["h_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("h_c_ratio_box1"))
                localSettings.Values["h_c_ratio_box1"] = localSettings.Values["h_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("h_c_ratio_box2"))
                localSettings.Values["h_c_ratio_box2"] = localSettings.Values["h_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("o_c_ratio_include"))
                localSettings.Values["o_c_ratio_include"] = localSettings.Values["o_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("o_c_ratio_box1"))
                localSettings.Values["o_c_ratio_box1"] = localSettings.Values["o_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("o_c_ratio_box2"))
                localSettings.Values["o_c_ratio_box2"] = localSettings.Values["o_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("n_c_ratio_include"))
                localSettings.Values["n_c_ratio_include"] = localSettings.Values["n_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("n_c_ratio_box1"))
                localSettings.Values["n_c_ratio_box1"] = localSettings.Values["n_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("n_c_ratio_box2"))
                localSettings.Values["n_c_ratio_box2"] = localSettings.Values["n_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("p_c_ratio_include"))
                localSettings.Values["p_c_ratio_include"] = localSettings.Values["p_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("p_c_ratio_box1"))
                localSettings.Values["p_c_ratio_box1"] = localSettings.Values["p_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("p_c_ratio_box2"))
                localSettings.Values["p_c_ratio_box2"] = localSettings.Values["p_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("s_c_ratio_include"))
                localSettings.Values["s_c_ratio_include"] = localSettings.Values["s_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("s_c_ratio_box1"))
                localSettings.Values["s_c_ratio_box1"] = localSettings.Values["s_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("s_c_ratio_box2"))
                localSettings.Values["s_c_ratio_box2"] = localSettings.Values["s_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("f_c_ratio_include"))
                localSettings.Values["f_c_ratio_include"] = localSettings.Values["f_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("f_c_ratio_box1"))
                localSettings.Values["f_c_ratio_box1"] = localSettings.Values["f_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("f_c_ratio_box2"))
                localSettings.Values["f_c_ratio_box2"] = localSettings.Values["f_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("cl_c_ratio_include"))
                localSettings.Values["cl_c_ratio_include"] = localSettings.Values["cl_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("cl_c_ratio_box1"))
                localSettings.Values["cl_c_ratio_box1"] = localSettings.Values["cl_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("cl_c_ratio_box2"))
                localSettings.Values["cl_c_ratio_box2"] = localSettings.Values["cl_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("br_c_ratio_include"))
                localSettings.Values["br_c_ratio_include"] = localSettings.Values["br_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("br_c_ratio_box1"))
                localSettings.Values["br_c_ratio_box1"] = localSettings.Values["br_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("br_c_ratio_box2"))
                localSettings.Values["br_c_ratio_box2"] = localSettings.Values["br_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("si_c_ratio_include"))
                localSettings.Values["si_c_ratio_include"] = localSettings.Values["si_c_ratio_include_default"];
            if (!localSettings.Values.ContainsKey("si_c_ratio_box1"))
                localSettings.Values["si_c_ratio_box1"] = localSettings.Values["si_c_ratio_box1_default"];
            if (!localSettings.Values.ContainsKey("si_c_ratio_box2"))
                localSettings.Values["si_c_ratio_box2"] = localSettings.Values["si_c_ratio_box2_default"];
            if (!localSettings.Values.ContainsKey("o_p_ratio_include"))
                localSettings.Values["o_p_ratio_include"] = localSettings.Values["o_p_ratio_include_default"];

            //timeout
            localSettings.Values["timeout_single_include_default"] = false;
            localSettings.Values["timeout_single_default"] = (double)30;
            //localSettings.Values["timeout_batch_include_default"] = true;
            //localSettings.Values["timeout_batch_default"] = (double)24;
            if (!localSettings.Values.ContainsKey("timeout_single_include"))
                localSettings.Values["timeout_single_include"] = localSettings.Values["timeout_single_include_default"];
            if (!localSettings.Values.ContainsKey("timeout_single"))
                localSettings.Values["timeout_single"] = localSettings.Values["timeout_single_default"];
            //if (!localSettings.Values.ContainsKey("timeout_batch_include"))
            //    localSettings.Values["timeout_batch_include"] = localSettings.Values["timeout_batch_include_default"];
            //if (!localSettings.Values.ContainsKey("timeout_batch"))
            //    localSettings.Values["timeout_batch"] = localSettings.Values["timeout_batch_default"];

            localSettings.Values["MS2FragReannotate_include_default"] = false;
            if (!localSettings.Values.ContainsKey("MS2FragReannotate_include"))
                localSettings.Values["MS2FragReannotate_include"] = localSettings.Values["MS2FragReannotate_include_default"];


            #endregion

            #region
            ////load the chemical formula database from CSV
            //DirectoryInfo directory = new DirectoryInfo(@"X:\Users\Shipei_Xing\Bottom-up_2020Dec\C#_trial_20210222\file_20210610\allDB_csv_mz");
            //FileInfo[] Files = directory.GetFiles("*.csv");
            //int saveIndex = 0;
            //string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            //List<AlignedFormula> DB = new List<AlignedFormula>();

            //foreach (FileInfo file in Files)
            //{
            //    List<AlignedFormula> tmp = AlignedFormulaManager.GetFormula(file.FullName);
            //    Debug.WriteLine(tmp.Count);
            //    DB.AddRange(tmp);
            //}
            //Debug.WriteLine(DB.Count);

            ////split formula database
            //for (int i = 0; i < (int)DB[DB.Count - 1].mass / GROUP_MZ_RANGE; i++)
            //{
            //    List<AlignedFormula> tmp = new List<AlignedFormula>();
            //    tmp = DB.Where(o => o.mass >= i * GROUP_MZ_RANGE && o.mass < (i + 1) * GROUP_MZ_RANGE).ToList();
            //    //groupedDB.Add();

            //    //save .bin database
            //    //serialize
            //    using (Stream stream = File.Open(@"X:\Users\Shipei_Xing\Bottom-up_2020Dec\C#_trial_20210222\file_20210610\BUDDY_formulaDB\" + i + ".bin", FileMode.Create))
            //    {
            //        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //        bformatter.Serialize(stream, tmp);
            //    }
            //}
            //Debug.WriteLine("DONE");
            //load the chemical formula database from CSV



            //List<Distribution> tmp = DistributionManager.GetDistribution(@"X:\Users\Shipei_Xing\Bottom-up_2020Dec\C#_trial_20210222\file_20210610\dist_df_pos.csv");
            //Debug.WriteLine(tmp.Count);

            ////save .bin database
            ////serialize
            //using (Stream stream = File.Open(@"X:\Users\Shipei_Xing\Bottom-up_2020Dec\C#_trial_20210222\file_20210610\dist_df_pos.bin", FileMode.Create))
            //{
            //    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    bformatter.Serialize(stream, tmp);
            //}



            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //List<MS2DBEntryInput> tmp = MS2DBManager.GetMS2DBEntry(@"X:\Users\Shipei_Xing\MS2_DB\For_BUDDY\FiehnHILIC_MS2DB_20220315.txt");
            //Debug.WriteLine(tmp.Count);

            //fiehnHILICMS2DB.MS2DBName = "Fiehn HILIC";
            //fiehnHILICMS2DB.DBEntries = new List<MS2DBEntry>();
            //for (int i = 0; i < tmp.Count; i++)
            //{
            //    List<RAW_PeakElement> ms2Spec = new List<RAW_PeakElement>();
            //    string[] mzStr = tmp[i].MS2mz.Split(";");
            //    string[] intStr = tmp[i].MS2int.Split(";");
            //    for (int j = 0; j < mzStr.Length; j++)
            //    {
            //        ms2Spec.Add(new RAW_PeakElement { Mz = Double.Parse(mzStr[j]), Intensity = Double.Parse(intStr[j]) });
            //    }

            //    fiehnHILICMS2DB.DBEntries.Add(new MS2DBEntry
            //    {
            //        MetaboliteName = tmp[i].MetaboliteName,
            //        DBNumberString = tmp[i].DBNumberString,
            //        InChIKey = tmp[i].InChIKey,
            //        Adduct = tmp[i].Adduct,
            //        PrecursorMz = Double.Parse(tmp[i].PrecursorMz),
            //        InstrumentType = tmp[i].InstrumentType,
            //        Instrument = tmp[i].Instrument,
            //        CollisionEnergy = tmp[i].CollisionEnergy,
            //        Formula = tmp[i].Formula,
            //        ExactMass = Double.Parse(tmp[i].ExactMass),
            //        Comments = tmp[i].Comments,
            //        MS2Spec = ms2Spec,
            //        InChIKeyFirstHalf = tmp[i].InChIKeyFirstHalf
            //    });
            //}
            //sw.Stop();
            //Debug.WriteLine("_____________ Time _______________________");
            //Debug.WriteLine("Elapsed={0}", sw.Elapsed);

            ////save .bin database
            ////serialize
            //using (Stream stream = File.Open(@"X:\Users\Shipei_Xing\MS2_DB\For_BUDDY\fiehnHILICMS2DB_20220315.bin", FileMode.Create))
            //{
            //    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    bformatter.Serialize(stream, fiehnHILICMS2DB);
            //}
            #endregion

            //Debug.WriteLine("processor: " + Environment.ProcessorCount);

            if (allDatabaseLoaded == false && groupedDB.Count == 0)
            {
                loadingDatabase = LoadDatabase();
            }

            MaximizeWindowOnLoad();
            this.InitializeComponent();
            //calculateMS2Progressbar.Visibility = Visibility.Collapsed;

            void MaximizeWindowOnLoad()
            {
                var view = DisplayInformation.GetForCurrentView();

                // Get the screen resolution (APIs available from 14393 onward).
                var resolution = new Windows.Foundation.Size(view.ScreenWidthInRawPixels, view.ScreenHeightInRawPixels);

                // Calculate the screen size in effective pixels. 
                // Note the height of the Windows Taskbar is ignored here since the app will only be given the maxium available size.
                var scale = view.ResolutionScale == ResolutionScale.Invalid ? 1 : view.RawPixelsPerViewPixel;
                var bounds = new Windows.Foundation.Size(resolution.Width / scale, resolution.Height / scale);

                ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(bounds.Width, bounds.Height);
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            }

            // drag & drop
            this.ms2grid.RowDragDropController = new GridRowDragDropControllerExt();
            this.ms2ComparePanel_Top.Drop += MS2CompareTop_Drop;
            this.ms2ComparePanel_Top.DragEnter += MS2CompareTop_DragEnter;
            this.ms2ComparePanel_Bottom.Drop += MS2CompareBottom_Drop;
            this.ms2ComparePanel_Bottom.DragEnter += MS2CompareBottom_DragEnter;

        }
        // Load formula database if not loaded
        private async Task LoadDatabase()
        {
            //load distribution
            LoadDistributionFilesAndClearCache();

            //load connection file for MILP
            LoadConnectionFile();

            // 1498 -> 300 for debug purpose
            for (int i = 0; i < 1498; i++)
            {
                StorageFile currFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FormulaDatabase/" + i + ".bin"));
                await currFile.CopyAsync(storageFolder, i + ".bin", NameCollisionOption.ReplaceExisting);
                List<AlignedFormula> formDBtmp = new List<AlignedFormula>();
                //load .bin database
                //deserialize
                using (Stream stream = File.Open(storageFolder.Path + @"\" + i + ".bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formDBtmp = (List<AlignedFormula>)bformatter.Deserialize(stream);
                }
                groupedDB.Add(formDBtmp);
            }

            modelFile1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_pos_withMS1.zip"));
            modelFile2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_pos_noMS1.zip"));
            modelFile3 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_neg_withMS1.zip"));
            modelFile4 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_neg_noMS1.zip"));
            modelFile5 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_pos_withMS1.zip"));
            modelFile6 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_pos_noMS1.zip"));
            modelFile7 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_neg_withMS1.zip"));
            modelFile8 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_neg_noMS1.zip"));

            allDatabaseLoaded = true;
        }

        //load distribution files and clear cache
        private async void LoadDistributionFilesAndClearCache()
        {
            //StorageFile dist_pos_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DistributionFile/dist_df_pos.csv"));
            //distPos_df = DistributionManager.GetDistribution(dist_pos_file.Path);
            //StorageFile dist_neg_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DistributionFile/dist_df_neg.csv"));
            //distNeg_df = DistributionManager.GetDistribution(dist_neg_file.Path);
            //StorageFile dist_pre_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DistributionFile/dist_df_pre.csv"));
            //pre_dist_df = DistributionManager.GetDistribution(dist_pre_file.Path);

            StorageFile dist_pre_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DistributionFile/dist_df_pre.bin"));
            await dist_pre_file.CopyAsync(storageFolder, "dist_df_pre.bin", NameCollisionOption.ReplaceExisting);
            //deserialize
            using (Stream stream = File.Open(storageFolder.Path + @"\dist_df_pre.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                pre_dist_df = (List<Distribution>)bformatter.Deserialize(stream);
            }
            Debug.WriteLine("preDistDF" + pre_dist_df.Count);

            StorageFile dist_pos_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DistributionFile/dist_df_pos.bin"));
            await dist_pos_file.CopyAsync(storageFolder, "dist_df_pos.bin", NameCollisionOption.ReplaceExisting);
            //deserialize
            using (Stream stream = File.Open(storageFolder.Path + @"\dist_df_pos.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                distPos_df = (List<Distribution>)bformatter.Deserialize(stream);
            }

            StorageFile dist_neg_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DistributionFile/dist_df_neg.bin"));
            await dist_neg_file.CopyAsync(storageFolder, "dist_df_neg.bin", NameCollisionOption.ReplaceExisting);
            //deserialize
            using (Stream stream = File.Open(storageFolder.Path + @"\dist_df_neg.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                distNeg_df = (List<Distribution>)bformatter.Deserialize(stream);
            }

            IReadOnlyList<StorageFile> storageFiles = await storageFolder.GetFilesAsync();
            foreach (StorageFile file in storageFiles)
            {
                if (file.Path.ToString().Contains(".zip") || file.Path.ToString().Contains(".mgf") || file.Path.ToString().Contains(".MGF") || file.Path.ToString().Contains(".mzML") || file.Path.ToString().Contains(".mzml") ||
                    file.Path.ToString().Contains(".MZML") || file.Path.ToString().Contains(".txt") || file.Path.ToString().Contains(".TXT") || file.Path.ToString().Contains(".csv") || file.Path.ToString().Contains(".CSV"))
                {
                    await file.DeleteAsync();
                }
            }
        }
        //load bioconnection files
        private async void LoadConnectionFile()
        {

            StorageFile connection_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///GlobalOptimizationMILP/FormulaConnectionDB.csv"));
            connection_df = ConnectionManager.GetConnection(connection_file.Path);
        }


        private async void AsyncAdductListStorage(string name, string defaultName)
        {
            try
            {
                StorageFile file = await storageFolder.GetFileAsync(name);
            }
            catch
            {
                List<Adduct> adductList = new List<Adduct>();
                //deserialize
                using (Stream stream = File.Open(storageFolder.Path + defaultName, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    adductList = (List<Adduct>)bformatter.Deserialize(stream);
                }
                //serialize
                using (Stream stream = File.Open(storageFolder.Path + @"\" + name, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bformatter.Serialize(stream, adductList);
                }

            }
        }

        //raw file import functions
        private async void ImportMGF_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".mgf");
            picker.FileTypeFilter.Add(".MGF");
            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                var filemodel = (FileModel)dataGrid.DataContext;
                int index;
                if(filemodel.Files.Count == 0)
                {
                    index = 1;
                }
                else
                {
                    index = filemodel.Files.Max(o => o.Index) + 1;
                }
                //filemodel.Files.Clear();
                foreach (StorageFile file in files)
                {
                    if (filemodel.Files.Where(o => o.FileName == file.Name).ToList().Count == 0)
                    {
                        StorageFile BUDDYfile = await file.CopyAsync(storageFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        filemodel.Files.Add(new FileUtility(false, BUDDYfile.Name, BUDDYfile.Path, index, "", false));
                        index++;
                    }
                    else
                    {
                        DuplicateFileSelected();
                        return;
                    }
                }
            }
            else
            {
                NoFileSelected();
            }
        }
        private async void ImportMZML_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".mzml");
            picker.FileTypeFilter.Add(".mzML");
            picker.FileTypeFilter.Add(".MZML");
            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                var filemodel = (FileModel)dataGrid.DataContext;
                int index;
                if (filemodel.Files.Count == 0)
                {
                    index = 1;
                }
                else
                {
                    index = filemodel.Files.Max(o => o.Index) + 1;
                }

                foreach (StorageFile file in files)
                {
                    if(filemodel.Files.Where(o => o.FileName == file.Name).ToList().Count == 0)
                    {
                        StorageFile BUDDYfile = await file.CopyAsync(storageFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        filemodel.Files.Add(new FileUtility(false, BUDDYfile.Name, BUDDYfile.Path, index, "", false));
                        index++;
                    }
                    else
                    {
                        DuplicateFileSelected();
                        return;
                    }

                    //Debug.WriteLine(file.Path);
                }
            }
            else
            {
                NoFileSelected();
            }
        }
        private async void ImportMSDIAL_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".TXT");

            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                var filemodel = (FileModel)dataGrid.DataContext;
                int index;
                if (filemodel.Files.Count == 0)
                {
                    index = 1;
                }
                else
                {
                    index = filemodel.Files.Max(o => o.Index) + 1;
                }

                try
                {
                    foreach (StorageFile file in files)
                    {
                        if (filemodel.Files.Where(o => o.FileName == file.Name).ToList().Count == 0)
                        {
                            StorageFile BUDDYfile = await file.CopyAsync(storageFolder, file.Name, NameCollisionOption.ReplaceExisting);
                            filemodel.Files.Add(new FileUtility(false, BUDDYfile.Name, BUDDYfile.Path, index, "", false));
                            index++;
                        }
                        else
                        {
                            DuplicateFileSelected();
                            return;
                        }

                        //Debug.WriteLine(file.Path);
                    }
                }
                catch
                {
                    InvalidFileFormat();
                    return;
                }
            }
            else
            {
                NoFileSelected();
            }
        }
        //update files table when user upload custom csv in popup window
        public async Task UpdateCustomFilesAsync(List<StorageFile> files)
        {
            FileModel thisFileModel = new FileModel();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => thisFileModel = (FileModel)dataGrid.DataContext);
            //var thisFileModel = (FileModel)dataGrid.DataContext;

            int index;
            if(thisFileModel.Files.Count == 0)
            {
                index = 1;
            }
            else
            {
                index = thisFileModel.Files.Max(o => o.Index) + 1;
            }
            for (int i = 0; i < files.Count; i++)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ((FileModel)dataGrid.DataContext).Files.Add(new FileUtility(false, files[i].Name, files[i].Path, index, "", false)));
                index++;
            }
            //filemodel.Files.Add(new FileUtility(false, file.Name, file.Path, index, ""));
        }
        
        //update ms2 table when user upload custom ms2 in popup window
        public async Task UpdateCustomMS2(Ms2Utility ms2)
        {
            FileModel thisFileModel = new FileModel();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => thisFileModel = (FileModel)dataGrid.DataContext);
            //var thisFileModel = (FileModel)dataGrid.DataContext;

            int index;
            if (thisFileModel.Files.Count == 0)
            {
                index = 1;
            }
            else
            {
                index = thisFileModel.Files.Max(o => o.Index) + 1;
            }
            
            List<Ms2Utility> ms2List = new List<Ms2Utility>();
            ms2.FileIndex = index;
            ms2List.Add(ms2);
            string fileName = "Single_input_mz" + ms2.Mz.ToString();
            FileUtility fileToAdd = new FileUtility(false, fileName, null, index, "", true);
            fileToAdd.MS2List = ms2List;
            fileToAdd.Ms2number = "1";

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ((FileModel)dataGrid.DataContext).Files.Add(fileToAdd));
        }

        //clear selected files in the files table
        private void fileClearSelected_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var filemodel = (FileModel)dataGrid.DataContext;
            List<int> removeIndex = new List<int>();
            for (int i = 0; i < filemodel.Files.Count; i++)
            {
                if (filemodel.Files[i].Selected)
                {
                    removeIndex.Add(filemodel.Files[i].Index);
                    //removeIndex.Add(i + 1);
                }
            }

            int fileIndexToStart = 0;
        filestart:
            for (int i = fileIndexToStart; i < filemodel.Files.Count; i++)
            {
                if (filemodel.Files[i].Selected)
                {
                    fileIndexToStart = i;
                    filemodel.Files.RemoveAt(i);
                    goto filestart;
                }
            }
            
            var ms2model = (Ms2Model)ms2grid.DataContext;
            int ms2IndexToStart = 0;
        ms2start:
            for (int i = ms2IndexToStart; i < ms2model.MS2s.Count; i++)
            {
                if (removeIndex.Contains(ms2model.MS2s[i].FileIndex))
                {
                    ms2IndexToStart = i;
                    ms2model.MS2s.RemoveAt(i);
                    goto ms2start;
                }
            }

            if(ms2model.MS2s == null || ms2model.MS2s.Count == 0)
            {
                emptyMS2ListMessage.Visibility = Visibility.Visible;
            }            

        }
        private void fileClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var filemodel = (FileModel)dataGrid.DataContext;
            var ms2model = (Ms2Model)ms2grid.DataContext;
            filemodel.Files.Clear();

            //ObservableCollection<Ms2Utility> newMS2list = new ObservableCollection<Ms2Utility>(ms2model.MS2s.Where(o => o.FileIndex == 0));
            //ms2model.MS2s = newMS2list;
            ms2model.MS2s.Clear();

            emptyMS2ListMessage.Visibility = Visibility.Visible;
        }

        private List<Ms2Utility> loadRawFileFromFileUtility(FileUtility item)
        {
            List<Ms2Utility> ms2inFile;
            if (item.Loaded)
            {
                return item.MS2List;
            }
            else
            {
                try
                {
                    //curritem.MS2List.Clear(); // cannot clear null
                    ms2inFile = ParseRawFiles(item);
                    if (ms2inFile != null && ms2inFile.Count > 0)
                    {
                        item.MS2List = ms2inFile;
                        item.ImageLink = "complete.png";
                        item.Loaded = true;
                        item.Ms2number = item.MS2List.Count.ToString();
                        return ms2inFile;
                    }
                    else
                    {
                        item.ImageLink = "cancelled.png";
                        loadFileProgressbar.Visibility = Visibility.Collapsed;
                        loadFileReminderText.Visibility = Visibility.Collapsed;
                        EmptyFile();
                        return null;
                    }
                }
                catch
                {
                    item.ImageLink = "cancelled.png";
                    loadFileProgressbar.Visibility = Visibility.Collapsed;
                    loadFileReminderText.Visibility = Visibility.Collapsed;
                    InvalidFileFormat();
                    return null;
                }
            }
            
        }
        //update files table when user upload custom csv in popup window

        private void plotFileMzRTDiagram(List<Ms2Utility> MS2inFile, string fileName)
        {
            var model = new PlotModel { Title = "File Name: " + fileName };
            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

            for (int i = 0; i < MS2inFile.Count; i++)
            {
                var x = MS2inFile[i].Rt;
                var y = MS2inFile[i].Mz;
                var size = 2;
                var colorValue = 1;
                scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));
            }

            var customAxis = new RangeColorAxis { Key = "customColors" };
            customAxis.AddRange(0, 2000, OxyColors.Navy);
            model.Axes.Add(customAxis);
            model.Series.Add(scatterSeries);

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Retention Time (min)", AbsoluteMinimum = 0, TitleFontSize = 15 });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "m/z", AbsoluteMinimum = 0, TitleFontSize = 15 });
            model.TitleFontSize = 16;
            MS2Scatter.Model = model;
            MS2Scatter.Model.TitleFontWeight = 2;
        }
        //When user clicks on a row in the files table...
        private void dataGrid_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            FileUtility curritem = (FileUtility)dataGrid.CurrentItem;
            if (curritem.Loaded)
            {
                List<Ms2Utility> ms2inFile = curritem.MS2List;
                if (ms2inFile != null && ms2inFile.Count > 0)
                {
                    plotFileMzRTDiagram(ms2inFile, curritem.FileName);
                }
            }

        }
        //When user clicks on checkmark in files table...
        private async void UpdateMS2Table(object sender, CurrentCellValueChangedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }

            // Runable version
            loadFileProgressbar.Visibility = Visibility.Visible;
            loadFileReminderText.Visibility = Visibility.Visible;
            await Task.Delay(1);

            FileUtility curritem = (FileUtility)dataGrid.CurrentItem;
            if (curritem.Selected)
            {
                List<Ms2Utility> ms2inFile = loadRawFileFromFileUtility(curritem);

                if (ms2inFile != null && ms2inFile.Count > 0)
                {
                    plotFileMzRTDiagram(ms2inFile, curritem.FileName);
                }
            }

            if (curritem.MS2List == null || curritem.MS2List.Count == 0)
            {
                loadFileProgressbar.Visibility = Visibility.Collapsed;
                loadFileReminderText.Visibility = Visibility.Collapsed;
                return;
            }

            var ms2model = (Ms2Model)ms2grid.DataContext;
            List<Ms2Utility> tmp = ms2model.MS2s.ToList();
            if (curritem.Selected)
            {
                tmp.AddRange(curritem.MS2List);
            }
            else
            {
                tmp.RemoveAll(o => o.FileIndex == curritem.Index);
            }
            ms2model.MS2s = new ObservableCollection<Ms2Utility>(tmp);
            

            if (ms2model.MS2s.Count == 0)
            {
                emptyMS2ListMessage.Visibility = Visibility.Visible;
            }
            else
            {
                emptyMS2ListMessage.Visibility = Visibility.Collapsed;
            }
            loadFileProgressbar.Visibility = Visibility.Collapsed;
            loadFileReminderText.Visibility = Visibility.Collapsed;
        }

        //Read and parse raw files async
        private List<Ms2Utility> ParseRawFiles(FileUtility item)
        {

            List<Ms2Utility> ms2model = new List<Ms2Utility>();
            if (item.FileName.Contains(".mgf") || item.FileName.Contains(".MGF"))
            {
                List<Ms2Utility> parsed = new List<Ms2Utility>(MgfParser.ReadMgf(item));
                foreach (Ms2Utility util in parsed)
                {
                    if (util.OriSpectrum.Count > 0)
                    {
                        ms2model.Add(util);
                    }
                }
                //item.Ms2number = parsed.Count.ToString();
                //ms2model.MS2s.AddRange(MgfParser.ReadMgf(file));
            }
            else if (item.FileName.Contains(".mzML") || item.FileName.Contains(".mzml") || item.FileName.Contains(".MZML"))
            {
                RAW_Measurement mzmlFile = new MzmlReader().ReadMzmlVer2(item.FilePath, 0);
                string mzmlPolarity;
                if (mzmlFile.SpectrumList[0].ScanPolarity == ScanPolarity.Positive)
                {
                    mzmlPolarity = "P";
                }
                else if (mzmlFile.SpectrumList[0].ScanPolarity == ScanPolarity.Negative)
                {
                    mzmlPolarity = "N";
                }
                else
                {
                    return ms2model;
                }

                double ms1tol = (double)localSettings.Values["ms1tol"];
                double ms2tol = (double)localSettings.Values["ms2tol"];
                bool ms1tol_ppmON = (bool)localSettings.Values["ms1tol_ppmON"];
                bool ms2_merging = (bool)localSettings.Values["ms2_merging"];
                double cos_similarity_cutoff = (double)localSettings.Values["cos_similarity_cutoff"];
                double max_peak_width = (double)localSettings.Values["max_peak_width"];
                int max_isotopic_peaks = (int)localSettings.Values["max_isotopic_peaks"];
                double isotopic_abundance_cutoff = (double)localSettings.Values["isotopic_abundance_cutoff"];
                double isp_grp_mass_tol = (double)localSettings.Values["isotopologue_grp_masstol"];

                if ((bool)localSettings.Values["ms2_group_imp"])
                {
                    // MS2Clustering
                    List<MS2Group> MS2GroupList = new List<MS2Group>(MS2Clustering.MS2Cluster(mzmlFile, ms1tol, ms2tol, ms1tol_ppmON, ms2_merging,
                        cos_similarity_cutoff, max_peak_width, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol));
                    foreach (MS2Group group in MS2GroupList)
                    {
                        Ms2Utility groupedMS2 = new Ms2Utility();
                        groupedMS2.Selected = false;
                        groupedMS2.FileIndex = item.Index;
                        groupedMS2.Mz = Math.Round(group.PrecursorMz, 4);
                        groupedMS2.Rt = Math.Round((group.RTmax + group.RTmin) / 2 / 60, 2);
                        groupedMS2.ImageLink = "open.png";
                        groupedMS2.polarity = mzmlPolarity;
                        groupedMS2.Filename = item.FileName;
                        if (mzmlPolarity == "P")
                        {
                            groupedMS2.Adduct = new Adduct("[M+H]+");
                        }
                        else
                        {
                            groupedMS2.Adduct = new Adduct("[M-H]-");
                        }
                        if (ms2_merging == true)
                        {
                            groupedMS2.OriSpectrum = group.MergedMS2;
                        }
                        else
                        {
                            groupedMS2.OriSpectrum = group.MostAbundantMS2;
                        }
                        groupedMS2.ScanNumber = group.HighestPreIntMS2ScanIndex;
                        groupedMS2.Formula_PC = "Unknown";
                        groupedMS2.InChiKey = "Unknown";
                        groupedMS2.Ms1 = group.MS1Isotope;
                        groupedMS2.MergedIndex = new List<int>(group.MS2ScanIndex);
                        ms2model.Add(groupedMS2);
                    }
                    //item.Ms2number = MS2GroupList.Count.ToString();
                    MS2GroupingDone();
                }
                else
                {
                    RAW_Spectrum currMS1 = new RAW_Spectrum();
                    foreach (RAW_Spectrum rawSpec in mzmlFile.SpectrumList)
                    {
                        if (rawSpec.MsLevel == 1)
                        {
                            currMS1 = rawSpec;

                        }
                        else if (rawSpec.MsLevel == 2)
                        {
                            Ms2Utility groupedMS2 = new Ms2Utility();
                            groupedMS2.Selected = false;
                            groupedMS2.FileIndex = item.Index;
                            groupedMS2.Mz = Math.Round(rawSpec.Precursor.SelectedIonMz, 4); //Premz
                            double tmpScanTime = rawSpec.ScanStartTime; //RT
                            if (rawSpec.ScanStartTimeUnit == Units.Second)
                            {
                                tmpScanTime = rawSpec.ScanStartTime / 60.0;
                            }
                            groupedMS2.Rt = Math.Round(tmpScanTime, 2);
                            groupedMS2.ImageLink = "open.png";
                            groupedMS2.polarity = mzmlPolarity;
                            groupedMS2.Filename = item.FileName;
                            if (mzmlPolarity == "P")
                            {
                                groupedMS2.Adduct = new Adduct("[M+H]+");
                            }
                            else
                            {
                                groupedMS2.Adduct = new Adduct("[M-H]-");
                            }
                            groupedMS2.OriSpectrum = rawSpec.Spectrum.ToList();
                            groupedMS2.ScanNumber = rawSpec.ScanNumber;
                            groupedMS2.Formula_PC = "Unknown";
                            groupedMS2.InChiKey = "Unknown";
                            groupedMS2.Ms1 = MS2Clustering.FindMS1Isotope(currMS1.Spectrum.ToList(), groupedMS2.Mz, isp_grp_mass_tol, max_isotopic_peaks, isotopic_abundance_cutoff, false);
                            if (groupedMS2.Spectrum.Count >= 1)
                            {
                                ms2model.Add(groupedMS2);
                            }
                        }
                    }
                    //item.Ms2number = mzmlFile.SpectrumList.Where(o => o.MsLevel == 2).ToList().Count.ToString();
                }
            }
            else if (item.FileName.Contains(".txt") || item.FileName.Contains(".TXT"))
            {
                bool featureClustering = (bool)localSettings.Values["featureClusterMsdial_include"];
                List<Ms2Utility> parsed = new List<Ms2Utility>(MsdialParser.ReadMsdialTxt(item, featureClustering));
                foreach (Ms2Utility util in parsed)
                {
                    //if (util.Spectrum.Count > 0)
                    //{
                    //    ms2model.Add(util);
                    //}
                    ms2model.Add(util);
                }
                //item.Ms2number = parsed.Count.ToString();
            }
            else if (item.FileName.Contains(".csv") || item.FileName.Contains(".CSV"))
            {
                List<Ms2Utility> parsed = new List<Ms2Utility>(MS2Manager.GetMS2(item));
                foreach (Ms2Utility util in parsed)
                {
                    //if (util.Spectrum.Count > 0)
                    //{
                    //    ms2model.Add(util);
                    //}
                    ms2model.Add(util);
                }
                //item.Ms2number = parsed.Count.ToString();
            }
            return ms2model;
        }
        //popup messages

        private async void MS2GroupingDone()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Note",
                Content = "MS2 grouping is performed.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void InvalidFileFormat()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid file format.",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void EmptyFile()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "The input file is empty or invalid.",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void CalculationInProgress()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Computation in progress. Please wait until the job is completed.",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void ExportInProgress()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Result exporting. Please wait until the job is completed.",
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }

        //MS2 table functions
        private void SelectMultipleMs2_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var selectedItems = ms2grid.SelectedItems;

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (Ms2Utility selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                }
            }
        }
        private void DeOrSelectAllMs2_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var ms2model = (Ms2Model)ms2grid.DataContext;
            bool doSelectAll = false;
            for (int i = 0; i < ms2model.MS2s.Count; i++)
            {
                if(ms2model.MS2s[i].Selected == false)
                {
                    doSelectAll = true;
                    break;
                }
            }
            if (!doSelectAll)
            {
                for (int i = 0; i < ms2model.MS2s.Count; i++)
                {
                    ms2model.MS2s[i].Selected = false;
                }
            }
            else
            {
                for (int i = 0; i < ms2model.MS2s.Count; i++)
                {
                    ms2model.MS2s[i].Selected = true;
                }
            }
        }
        //When user calculates selected MS2s, contains machine learning model
        private async void CalculateMS2_Click(object sender, RoutedEventArgs e)
        {

            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            calculationInProgress = true;

            //get setting parameters
            bool ppm = (bool)localSettings.Values["ms1tol_ppmON"];
            double ms1tol = (double)localSettings.Values["ms1tol"];
            double ms2tol = (double)localSettings.Values["ms2tol"];
            bool deisotope = (bool)localSettings.Values["ms2Deisotope"];
            bool denoise = (bool)localSettings.Values["ms2Denoise"];
            double maxNoiseFragRatio = (double)localSettings.Values["maxNoiseFragRatio"] * 0.01;
            double maxNoiseInt = (double)localSettings.Values["maxNoiseInt"];
            double maxNoiseRSD = (double)localSettings.Values["maxNoiseRSD"];

            bool expSpecificPred = (bool)localSettings.Values["ExpSpecificGlobalAnnotation_include"];
            bool ms2LibrarySearch = (bool)localSettings.Values["MS2LibrarySearch_include"];
            bool buddySearch = (bool)localSettings.Values["BUDDY_include"];

            // ms2 library search
            int ms2MatchingAlgorithm = (int)localSettings.Values["ms2MatchingAlgorithm"];
            double metaIdenMS2SimilarityScoreThreshold = (double)localSettings.Values["metaboliteIdentificationMS2SimilarityScoreThreshold"];
            bool metaIdenMS2MinMatchedFragBool = (bool)localSettings.Values["metaboliteIdentificationMS2MinMatchedFrag_include"];
            int metaIdenMS2MinMatchedFragNo = (int)localSettings.Values["metaboliteIdentificationMS2MinMatchedFragNo"];
            bool metaIdenRTBool = (bool)localSettings.Values["metaboliteIdentificationRT_include"];
            double metaIdenRTtol = (double)localSettings.Values["metaboliteIdentificationRTtol"];

            int topfragNo = (int)localSettings.Values["topFrag"];
            bool use_allFrag = (bool)localSettings.Values["use_allFrag"];

            bool metaScoreBool = (bool)localSettings.Values["MetaScore_include"];

            double max_peak_width = (double)localSettings.Values["max_peak_width"];
            //double expFragNoRatioCutoff = 0.0;
            //if ((bool)localSettings.Values["expMS2fragNum_include"])
            //{
            //    expFragNoRatioCutoff = (double)localSettings.Values["expMS2fragNum"] * 0.01;
            //}
            //double expFragIntRatioCutoff = 0.0;
            //if ((bool)localSettings.Values["expMS2fragInt_include"])
            //{
            //    expFragIntRatioCutoff = (double)localSettings.Values["expMS2fragInt"] * 0.01;
            //}
            int topCandidateNum = (int)localSettings.Values["topCandidates"];
            bool topcandcut = (bool)localSettings.Values["use_allCandidates"];
            int max_isotopic_peaks = (int)localSettings.Values["max_isotopic_peaks"];
            double isotopic_abundance_cutoff = (double)localSettings.Values["isotopic_abundance_cutoff"];
            double isp_grp_mass_tol = (double)localSettings.Values["isotopologue_grp_masstol"];

            bool OPratiocheck = (bool)localSettings.Values["o_p_ratio_include"];

            double timeout = (double)localSettings.Values["timeout_single"];
            if (!(bool)localSettings.Values["timeout_single_include"])
            {
                timeout = 1e6;
            }
            //element count restriction
            #region
            FormulaRestriction restriction = new FormulaRestriction() { };
            if ((bool)localSettings.Values["c_include"])
            {
                restriction.C_min = (int)localSettings.Values["c_min"];
                restriction.C_max = (int)localSettings.Values["c_max"];
            }
            else
            {
                restriction.C_min = 0;
                restriction.C_max = 0;
            }
            if ((bool)localSettings.Values["h_include"])
            {
                restriction.H_min = (int)localSettings.Values["h_min"];
                restriction.H_max = (int)localSettings.Values["h_max"];
            }
            else
            {
                restriction.H_min = 0;
                restriction.H_max = 0;
            }
            if ((bool)localSettings.Values["n_include"])
            {
                restriction.N_min = (int)localSettings.Values["n_min"];
                restriction.N_max = (int)localSettings.Values["n_max"];
            }
            else
            {
                restriction.N_min = 0;
                restriction.N_max = 0;
            }
            if ((bool)localSettings.Values["o_include"])
            {
                restriction.O_min = (int)localSettings.Values["o_min"];
                restriction.O_max = (int)localSettings.Values["o_max"];
            }
            else
            {
                restriction.O_min = 0;
                restriction.O_max = 0;
            }
            if ((bool)localSettings.Values["p_include"])
            {
                restriction.P_min = (int)localSettings.Values["p_min"];
                restriction.P_max = (int)localSettings.Values["p_max"];
            }
            else
            {
                restriction.P_min = 0;
                restriction.P_max = 0;
            }
            if ((bool)localSettings.Values["s_include"])
            {
                restriction.S_min = (int)localSettings.Values["s_min"];
                restriction.S_max = (int)localSettings.Values["s_max"];
            }
            else
            {
                restriction.S_min = 0;
                restriction.S_max = 0;
            }
            if ((bool)localSettings.Values["f_include"])
            {
                restriction.F_min = (int)localSettings.Values["f_min"];
                restriction.F_max = (int)localSettings.Values["f_max"];
            }
            else
            {
                restriction.F_min = 0;
                restriction.F_max = 0;
            }
            if ((bool)localSettings.Values["cl_include"])
            {
                restriction.Cl_min = (int)localSettings.Values["cl_min"];
                restriction.Cl_max = (int)localSettings.Values["cl_max"];
            }
            else
            {
                restriction.Cl_min = 0;
                restriction.Cl_max = 0;
            }
            if ((bool)localSettings.Values["br_include"])
            {
                restriction.Br_min = (int)localSettings.Values["br_min"];
                restriction.Br_max = (int)localSettings.Values["br_max"];
            }
            else
            {
                restriction.Br_min = 0;
                restriction.Br_max = 0;
            }
            if ((bool)localSettings.Values["i_include"])
            {
                restriction.I_min = (int)localSettings.Values["i_min"];
                restriction.I_max = (int)localSettings.Values["i_max"];
            }
            else
            {
                restriction.I_min = 0;
                restriction.I_max = 0;
            }
            if ((bool)localSettings.Values["si_include"])
            {
                restriction.Si_min = (int)localSettings.Values["si_min"];
                restriction.Si_max = (int)localSettings.Values["si_max"];
            }
            else
            {
                restriction.Si_min = 0;
                restriction.Si_max = 0;
            }
            if ((bool)localSettings.Values["b_include"])
            {
                restriction.B_min = (int)localSettings.Values["b_min"];
                restriction.B_max = (int)localSettings.Values["b_max"];
            }
            else
            {
                restriction.B_min = 0;
                restriction.B_max = 0;
            }
            if ((bool)localSettings.Values["se_include"])
            {
                restriction.Se_min = (int)localSettings.Values["se_min"];
                restriction.Se_max = (int)localSettings.Values["se_max"];
            }
            else
            {
                restriction.Se_min = 0;
                restriction.Se_max = 0;
            }

            if ((bool)localSettings.Values["h_c_ratio_include"])
            {
                restriction.H_C_min = (double)localSettings.Values["h_c_ratio_box1"];
                restriction.H_C_max = (double)localSettings.Values["h_c_ratio_box2"];
            }
            else
            {
                restriction.H_C_min = 0.0;
                restriction.H_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["o_c_ratio_include"])
            {
                restriction.O_C_min = (double)localSettings.Values["o_c_ratio_box1"];
                restriction.O_C_max = (double)localSettings.Values["o_c_ratio_box2"];
            }
            else
            {
                restriction.O_C_min = 0.0;
                restriction.O_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["n_c_ratio_include"])
            {
                restriction.N_C_min = (double)localSettings.Values["n_c_ratio_box1"];
                restriction.N_C_max = (double)localSettings.Values["n_c_ratio_box2"];
            }
            else
            {
                restriction.N_C_min = 0.0;
                restriction.N_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["p_c_ratio_include"])
            {
                restriction.P_C_min = (double)localSettings.Values["p_c_ratio_box1"];
                restriction.P_C_max = (double)localSettings.Values["p_c_ratio_box2"];
            }
            else
            {
                restriction.P_C_min = 0.0;
                restriction.P_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["s_c_ratio_include"])
            {
                restriction.S_C_min = (double)localSettings.Values["s_c_ratio_box1"];
                restriction.S_C_max = (double)localSettings.Values["s_c_ratio_box2"];
            }
            else
            {
                restriction.S_C_min = 0.0;
                restriction.S_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["f_c_ratio_include"])
            {
                restriction.F_C_min = (double)localSettings.Values["f_c_ratio_box1"];
                restriction.F_C_max = (double)localSettings.Values["f_c_ratio_box2"];
            }
            else
            {
                restriction.F_C_min = 0.0;
                restriction.F_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["cl_c_ratio_include"])
            {
                restriction.Cl_C_min = (double)localSettings.Values["cl_c_ratio_box1"];
                restriction.Cl_C_max = (double)localSettings.Values["cl_c_ratio_box2"];
            }
            else
            {
                restriction.Cl_C_min = 0.0;
                restriction.Cl_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["br_c_ratio_include"])
            {
                restriction.Br_C_min = (double)localSettings.Values["br_c_ratio_box1"];
                restriction.Br_C_max = (double)localSettings.Values["br_c_ratio_box2"];
            }
            else
            {
                restriction.Br_C_min = 0.0;
                restriction.Br_C_max = double.MaxValue;
            }
            if ((bool)localSettings.Values["si_c_ratio_include"])
            {
                restriction.Si_C_min = (double)localSettings.Values["si_c_ratio_box1"];
                restriction.Si_C_max = (double)localSettings.Values["si_c_ratio_box2"];
            }
            else
            {
                restriction.Si_C_min = 0.0;
                restriction.Si_C_max = double.MaxValue;
            }

            #endregion

            //get data from ms2 table 
            var ms2model = (Ms2Model)ms2grid.DataContext;

            // selected & non-seed metabolite
            int selectedMS2Count = ms2model.MS2s.Where(o => o.Selected == true).Count();
            
            // if no MS2
            if (ms2model.MS2s.Count == 0)
            {
                NoMS2Selected();
                calculationInProgress = false;
                return;
            }
            // no MS2 selected
            if (selectedMS2Count == 0)
            {
                NoMS2Selected();
                calculationInProgress = false;
                return;
            }

            //if database not already loaded, show loading screen and load database
            if (buddySearch && allDatabaseLoaded == false)
            {
                DatabaseLoad hud = new DatabaseLoad("Loading");
                hud.Show();
                await loadingDatabase;
                //await loadingMS2Database;
                hud.Close();
            }

            for (int i = 0; i < ms2model.MS2s.Count; i++) //iterate over all ms2s in table
            {
                if (ms2model.MS2s[i].SeedMetabolite && ms2model.MS2s[i].Ms2Matching == null) // seed metabolites provided by users
                {
                    ms2model.MS2s[i].ImageLink = "seedMetabolite.png";
                    continue;
                }
                if (ms2model.MS2s[i].Selected) //only calculate non-seed ms2 with checkmark
                {
                    // clear previous searching results
                    ms2model.MS2s[i].SeedMetabolite = false;
                    ms2model.MS2s[i].inchikey = null;
                    ms2model.MS2s[i].formula_pc = null;
                    ms2model.MS2s[i].inchikeyFirstHalf = null;
                    ms2model.MS2s[i].metaboliteName = null;
                    ms2model.MS2s[i].pubchemAccessedBefore = false;
                    ms2model.MS2s[i].pubchemCID = null;
                    ms2model.MS2s[i].pubchemDescription = null;
                    ms2model.MS2s[i].pubchemImage = null;

                    ms2model.MS2s[i].candidates = null;
                    ms2model.MS2s[i].featureConnections = null;
                    ms2model.MS2s[i].ms2Matching = null;
                    if (buddySearch)
                    {
                        ms2model.MS2s[i].ImageLink = "datagridloading.gif";
                    }
                    else
                    {
                        ms2model.MS2s[i].ImageLink = "open.png";
                    }
                }
            }

            calculatedMS2No.Text = "";
            totalMS2No.Text = "";
            //totalMS2No.Visibility = Visibility.Visible;
            bool useCustomMS2DB = (bool)localSettings.Values["useCustomMS2DB_include"];
            string ms2DBfileName = ""; // full file path for custom MS2DB
            List<MS2DBEntry> posMS2DB = new List<MS2DBEntry>();
            List<MS2DBEntry> negMS2DB = new List<MS2DBEntry>();
            if (ms2LibrarySearch) // load ms2 DB
            {
                taskInProgress.Text = "Loading MS2 library...";
                await Task.Delay(1);
                ms2DBfileName = (string)localSettings.Values["customMS2DBName"]; // full path
               
                if (useCustomMS2DB && loadedcustomMS2DB != ms2DBfileName) // load a new ms2 db
                {
                    MS2DB = MS2DBManager.ReadMspMS2DB(ms2DBfileName);
                    loadedcustomMS2DB = ms2DBfileName;
                }
                else if (!useCustomMS2DB)
                {
                    StorageFile fiehnHilicFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///MS2LibrarySearch/MS2DBs/MoNA-export-Fiehn_HILIC.msp"));
                    MS2DB = MS2DBManager.ReadMspMS2DB(fiehnHilicFile.Path);
                }
                posMS2DB = MS2DB.Where(o => o.IonMode == "P").ToList();
                negMS2DB = MS2DB.Where(o => o.IonMode == "N").ToList();

                //Debug.WriteLine("posDB: " + posMS2DB.Count);
                //Debug.WriteLine("negDB: " + negMS2DB.Count);
            }

            var filemodel = (FileModel)dataGrid.DataContext;

            //Stopwatch w0 = new Stopwatch();
            //w0.Start();
            if (ms2LibrarySearch)
            {
                taskInProgress.Text = "MS2 library searching...";
                totalMS2No.Text = " / " + selectedMS2Count.ToString();
                calculatedMS2No.Text = "0";
                calculateMS2Progressbar.Minimum = 0;
                calculateMS2Progressbar.Maximum = selectedMS2Count;
                calculateMS2Progressbar.Value = 0;

                string loadedMS2DBName = "";
                if (useCustomMS2DB)
                {
                    loadedMS2DBName = ms2DBfileName.Substring(ms2DBfileName.LastIndexOf("\\") + 1);
                }
                else
                {
                    loadedMS2DBName = "Fiehn HILIC";
                }

                for (int i = 0; i < ms2model.MS2s.Count; i++)
                {
                    //debug
                    //Debug.WriteLine("MS2 searching, ms2 index: " + i);

                    if (ms2model.MS2s[i].Selected)
                    {
                         //  seed metabolite offered by users
                        if (ms2model.MS2s[i].SeedMetabolite)
                        {
                            calculateMS2Progressbar.Value++;
                            calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                            continue;
                        }

                        //if (ms2model.MS2s[i].NoiseFragments != null)
                        //{
                        //    ms2model.MS2s[i].Spectrum.AddRange(ms2model.MS2s[i].NoiseFragments);
                        //    ms2model.MS2s[i].NoiseFragments = null;
                        //}
                        //if (ms2model.MS2s[i].IsotopeFragments != null)
                        //{
                        //    ms2model.MS2s[i].Spectrum.AddRange(ms2model.MS2s[i].IsotopeFragments);
                        //    ms2model.MS2s[i].IsotopeFragments = null;
                        //}
                        //if (ms2model.MS2s[i].PrecursorFragments != null)
                        //{
                        //    ms2model.MS2s[i].Spectrum.AddRange(ms2model.MS2s[i].PrecursorFragments);
                        //    ms2model.MS2s[i].PrecursorFragments = null;
                        //}
                        if (ms2model.MS2s[i].OriSpectrum == null || ms2model.MS2s[i].OriSpectrum.Count == 0)
                        {
                            calculateMS2Progressbar.Value++;
                            calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                            continue;
                        }

                        //orber by mz
                        ms2model.MS2s[i].OriSpectrum = ms2model.MS2s[i].OriSpectrum.OrderBy(o => o.Mz).ToList();

                        Ms2Utility ms2 = new Ms2Utility(ms2model.MS2s[i].Mz, ms2model.MS2s[i].polarity, ms2model.MS2s[i].OriSpectrum, ms2model.MS2s[i].Adduct, ms2tol, ms1tol, ms2model.MS2s[i].Ms1);

                        List<MS2MatchResult> thisLibrarySearchOutput = new List<MS2MatchResult>();
                        if (ms2model.MS2s[i].polarity == "P")
                        {
                            thisLibrarySearchOutput = await Task.Run(() => MS2Compare.MS2Matching(ms2, ppm, posMS2DB, ms2MatchingAlgorithm,
                                        metaIdenMS2SimilarityScoreThreshold, metaIdenMS2MinMatchedFragBool, metaIdenMS2MinMatchedFragNo, metaIdenRTBool, metaIdenRTtol));
                        }
                        else
                        {
                            thisLibrarySearchOutput = await Task.Run(() => MS2Compare.MS2Matching(ms2, ppm, negMS2DB, ms2MatchingAlgorithm,
                                        metaIdenMS2SimilarityScoreThreshold, metaIdenMS2MinMatchedFragBool, metaIdenMS2MinMatchedFragNo, metaIdenRTBool, metaIdenRTtol));
                        }

                        // for debug
                        //thisLibrarySearchOutput = MS2Compare.MS2Matching(ms2, ppm, posMS2DB, ms2MatchingAlgorithm,
                        //    metaIdenMS2SimilarityScoreThreshold, metaIdenMS2MinMatchedFragBool, metaIdenMS2MinMatchedFragNo, metaIdenRTBool, metaIdenRTtol);


                        ms2model.MS2s[i].Ms2Matching = new Ms2MatchingResult();
                        ms2model.MS2s[i].Ms2Matching.MS2DB = loadedMS2DBName;
                        ms2model.MS2s[i].Ms2Matching.MS2MatchingAlgorithm = ms2MatchingAlgorithm;
                        ms2model.MS2s[i].Ms2Matching.ms2MatchingReturns = thisLibrarySearchOutput;

                        calculateMS2Progressbar.Value++;
                        calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();

                        if (thisLibrarySearchOutput.Count > 0) // check matching results, add to seed metabolites
                        {
                            bool validMatch = true;
                            // adduct check: interprettable & match
                            try
                            {
                                Adduct adductToAdd = new Adduct(thisLibrarySearchOutput[0].matchedDBEntry.Adduct);
                                if (adductToAdd.Formula != ms2model.MS2s[i].Adduct.Formula || adductToAdd.M != ms2model.MS2s[i].Adduct.M)
                                {
                                    validMatch = false;
                                }
                            }
                            catch
                            {
                                validMatch = false;
                            }
                            if (validMatch)
                            {
                                // formula valid
                                Tuple<bool, string> formulaCheck = MS2DBManager.FormulaValidilityCheck(thisLibrarySearchOutput[0].matchedDBEntry.Formula);
                                if (formulaCheck.Item1)
                                {
                                    ms2model.MS2s[i].SeedMetabolite = true;
                                    ms2model.MS2s[i].Formula_PC = formulaCheck.Item2; //proton-adjusted formula
                                    ms2model.MS2s[i].InChiKey = thisLibrarySearchOutput[0].matchedDBEntry.InChIKey;
                                    ms2model.MS2s[i].InChiKeyFirstHalf = thisLibrarySearchOutput[0].matchedDBEntry.InChIKeyFirstHalf;
                                    ms2model.MS2s[i].MetaboliteName = thisLibrarySearchOutput[0].matchedDBEntry.MetaboliteName;
                                    ms2model.MS2s[i].ImageLink = "seedMetabolite.png";
                                }
                            }
                        }
                        if (ms2model.MS2s[i].Equals((Ms2Utility)ms2grid.CurrentItem))
                        {
                            updateMS2TableAfterCalculation();
                        }
                    }
                }
            }
            //w0.Stop();
            //Debug.WriteLine("----------------------------------");
            //Debug.WriteLine("----------------------------------");
            //Debug.WriteLine("----------------------------------");
            //Debug.WriteLine("----------------------------------");
            //Debug.WriteLine("search: " + w0.ElapsedMilliseconds);

            //Stopwatch w = new Stopwatch();
            //w.Start();

            if (buddySearch)
            {
                //calculationProgressSlash.Visibility = Visibility.Visible;
                taskInProgress.Text = "Bottom-up MS/MS interrogation...";
                totalMS2No.Text = " / " + selectedMS2Count.ToString();
                calculatedMS2No.Text = "0";
                calculateMS2Progressbar.Minimum = 0;
                calculateMS2Progressbar.Maximum = selectedMS2Count;
                calculateMS2Progressbar.Value = 0;

                // high or low res-MS data
                bool highResMSData = true;
                if ((ppm && ms1tol > 100) || (ppm == false && ms1tol >= 0.1))
                {
                    highResMSData = false;
                }
                // if high-res & experiment-specific feature scaling, calculate by file, otherwise by MS2
                if (highResMSData && expSpecificPred && selectedMS2Count > 50)
                {
                    // for each selected file
                    for (int i = 0; i < filemodel.Files.Count; i++)
                    {
                        if (filemodel.Files[i].Selected)
                        {
                            List<Ms2Utility> MS2InThisFile = ms2model.MS2s.Where(o => o.FileIndex == filemodel.Files[i].Index && o.Selected).ToList();
                            // record pos and neg MS2 in this file
                            int posMS2InThisFile = MS2InThisFile.Where(o => o.Polarity == "P").Count();
                            int negMS2InThisFile = MS2InThisFile.Where(o => o.Polarity == "N").Count();

                            bool posIonMode = true;
                            string ionModeStr = "P";
                            for (int j = 0; j < 2; j++) // for: ion mode, 0 P, 1 N
                            {
                                if (j == 0 && posMS2InThisFile == 0)
                                {
                                    continue;
                                }
                                if (j == 1 && negMS2InThisFile == 0)
                                {
                                    continue;
                                }

                                int MS2CountInThisFile = posMS2InThisFile;
                                if (j == 1)
                                {
                                    posIonMode = false;
                                    ionModeStr = "N";
                                    MS2CountInThisFile = negMS2InThisFile;
                                }

                                // ms2 index
                                List<int> MS2Index = Enumerable.Range(0, ms2model.MS2s.Count).Where(o => ms2model.MS2s[o].Selected &&
                                                     ms2model.MS2s[o].FileIndex == filemodel.Files[i].Index && ms2model.MS2s[o].Polarity == ionModeStr).ToList();

                                // create empty lists
                                List<List<Feature>> batchBoosterOutput = new List<List<Feature>>();
                                List<double> preMzErrorRatioList = new List<double>();
                                List<double> waFragMzErrorRatioList = new List<double>();

                                // booster first part
                                for (int m = 0; m < MS2Index.Count; m++)
                                {
                                    int k = MS2Index[m]; // index in MS2model
                                    if (ms2model.MS2s[k].SeedMetabolite) // seed metabolite
                                    {
                                        calculateMS2Progressbar.Value++;
                                        calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                        //ms2model.MS2s[k].ImageLink = "complete.png";
                                        batchBoosterOutput.Add(new List<Feature>());
                                        continue;
                                    }
                                    if (ms2model.MS2s[k].Mz > 1498) //if precursor mz out of range, no results
                                    {
                                        ms2model.MS2s[k].candidates = null;
                                        ms2model.MS2s[k].Formula_PC = "Unknown";
                                        ms2model.MS2s[k].ImageLink = "cancelled.png";
                                        calculateMS2Progressbar.Value++;
                                        calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                        batchBoosterOutput.Add(new List<Feature>());
                                        continue;
                                    }

                                    ms2model.MS2s[k].Spectrum = new List<RAW_PeakElement>(ms2model.MS2s[k].OriSpectrum);
                                    if (ms2model.MS2s[k].OriSpectrum.Count != 0) //if fragments, do deisotope, denoise and precursor exclusions
                                    {
                                        if (ms2model.MS2s[k].NoiseFragments != null)
                                        {
                                            //ms2model.MS2s[k].Spectrum.AddRange(ms2model.MS2s[k].NoiseFragments);
                                            ms2model.MS2s[k].NoiseFragments = null;
                                        }
                                        if (ms2model.MS2s[k].IsotopeFragments != null)
                                        {
                                            //ms2model.MS2s[k].Spectrum.AddRange(ms2model.MS2s[k].IsotopeFragments);
                                            ms2model.MS2s[k].IsotopeFragments = null;
                                        }
                                        if (ms2model.MS2s[k].PrecursorFragments != null)
                                        {
                                            //ms2model.MS2s[k].Spectrum.AddRange(ms2model.MS2s[k].PrecursorFragments);
                                            ms2model.MS2s[k].PrecursorFragments = null;
                                        }
                                        
                                        if (denoise) //ms2 deisotope (including precursor fragments)
                                        {
                                            List<RAW_PeakElement> rawMS2 = new List<RAW_PeakElement>(ms2model.MS2s[k].Spectrum);
                                            ms2model.MS2s[k].Spectrum = Booster.ms2denoise(ms2model.MS2s[k].Spectrum, maxNoiseFragRatio, maxNoiseRSD, maxNoiseInt);
                                            ms2model.MS2s[k].NoiseFragments = rawMS2.Except(ms2model.MS2s[k].Spectrum).ToList();
                                        }


                                        if (deisotope)
                                        {
                                            List<RAW_PeakElement> rawMS2_2 = new List<RAW_PeakElement>(ms2model.MS2s[k].Spectrum);
                                            ms2model.MS2s[k].Spectrum = Booster.ms2deisotope(ms2model.MS2s[k].Spectrum, ms2tol, ppm);
                                            ms2model.MS2s[k].IsotopeFragments = rawMS2_2.Except(ms2model.MS2s[k].Spectrum).ToList();
                                        }


                                        // MS2 precursor exclusion
                                        if (ppm)
                                        {
                                            ms2model.MS2s[k].PrecursorFragments = ms2model.MS2s[k].Spectrum.Where(o => o.Mz >= (ms2model.MS2s[k].Mz - ms2tol * ms2model.MS2s[k].Mz * 1e-6)).ToList();
                                            ms2model.MS2s[k].Spectrum = ms2model.MS2s[k].Spectrum.Where(o => o.Mz < (ms2model.MS2s[k].Mz - ms2tol * ms2model.MS2s[k].Mz * 1e-6)).ToList();
                                        }
                                        else
                                        {
                                            ms2model.MS2s[k].PrecursorFragments = ms2model.MS2s[k].Spectrum.Where(o => o.Mz >= (ms2model.MS2s[k].Mz - ms2tol)).ToList();
                                            ms2model.MS2s[k].Spectrum = ms2model.MS2s[k].Spectrum.Where(o => o.Mz < (ms2model.MS2s[k].Mz - ms2tol)).ToList();
                                        }

                                        // top frag No
                                        if (!use_allFrag)
                                        {
                                            if (ms2model.MS2s[k].Spectrum.Count > topfragNo)
                                            {
                                                List<RAW_PeakElement> rawMS2_3 = new List<RAW_PeakElement>(ms2model.MS2s[k].Spectrum);
                                                ms2model.MS2s[k].Spectrum = ms2model.MS2s[k].Spectrum.OrderByDescending(o => o.Intensity).ToList().Take(topfragNo).ToList();
                                                if (ms2model.MS2s[k].NoiseFragments != null)
                                                {
                                                    ms2model.MS2s[k].NoiseFragments.AddRange(rawMS2_3.Except(ms2model.MS2s[k].Spectrum).ToList());
                                                }
                                                else
                                                {
                                                    ms2model.MS2s[k].NoiseFragments = rawMS2_3.Except(ms2model.MS2s[k].Spectrum).ToList();
                                                }
                                            }
                                        }
                                        //orber by mz
                                        ms2model.MS2s[k].Spectrum = ms2model.MS2s[k].Spectrum.OrderBy(o => o.Mz).ToList();
                                    }

                                    // Recalculate sumformula in case user change the adduct setting.
                                    try
                                    {
                                        ms2model.MS2s[k].Adduct = new Adduct(ms2model.MS2s[k].Adduct.Formula);
                                    }
                                    catch
                                    {
                                        ms2model.MS2s[k].candidates = null;
                                        ms2model.MS2s[k].Formula_PC = "Unknown";
                                        ms2model.MS2s[k].ImageLink = "cancelled.png";
                                        calculateMS2Progressbar.Value++;
                                        calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                        batchBoosterOutput.Add(new List<Feature>());
                                        continue;
                                    }
                                    Ms2Utility ms2 = new Ms2Utility(ms2model.MS2s[k].Mz, ms2model.MS2s[k].polarity, ms2model.MS2s[k].OriSpectrum, ms2model.MS2s[k].Adduct, ms2tol, ms1tol, ms2model.MS2s[k].Ms1, ms2model.MS2s[k].Spectrum);

                                    // Booster main program
                                    //List<Feature> thisBoosterOutput = null;
                                    List<Feature> thisBoosterOutput = new List<Feature>(await Task.Run(() => BoosterCalculation(ppm, ms1tol, ms2tol, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction, ms2, i, timeout, OPratiocheck)));
                                    batchBoosterOutput.Add(thisBoosterOutput);

                                    if (thisBoosterOutput != null && thisBoosterOutput.Count > 0)
                                    {
                                        preMzErrorRatioList.Add(thisBoosterOutput[0].p_mzErrorRatio);
                                        waFragMzErrorRatioList.Add(thisBoosterOutput[0].waf_mzErrorRatio);
                                        //expFragNoList.Add(thisBoosterOutput[0].expfNoRatio);
                                        //expFragIntList.Add(thisBoosterOutput[0].expfIntRatio);
                                    }
                                    else
                                    {
                                        ms2model.MS2s[k].candidates = null;
                                        ms2model.MS2s[k].Formula_PC = "Unknown";
                                        ms2model.MS2s[k].ImageLink = "cancelled.png";
                                    }
                                    calculateMS2Progressbar.Value++;
                                    calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                }

                                if (expSpecificPred && MS2CountInThisFile > 50)
                                {
                                    for (int m = 0; m < batchBoosterOutput.Count; m++)
                                    {
                                        if (batchBoosterOutput[m].Count > 0)
                                        {
                                            batchBoosterOutput[m] = FeatureScaling(posIonMode, batchBoosterOutput[m], preMzErrorRatioList.Median(), waFragMzErrorRatioList.Median());
                                        }
                                    }
                                }
                                else // still perform MS2-specific explained frag correction
                                {
                                    for (int m = 0; m < batchBoosterOutput.Count; m++)
                                    {
                                        if (batchBoosterOutput[m].Count > 0)
                                        {
                                            batchBoosterOutput[m] = ExpFragCorrection(batchBoosterOutput[m], highResMSData, posIonMode);
                                        }
                                    }
                                }

                                //calculationProgressSlash.Visibility = Visibility.Collapsed;
                                //totalMS2No.Visibility = Visibility.Collapsed;


                                // MLRanking part
                                // if qualified for MILP-expSpecificPred, go to MILP global optimization
                                if (expSpecificPred && MS2CountInThisFile > 50)
                                {
                                    taskInProgress.Text = "Machine-learned ranking...";
                                    calculatedMS2No.Text = "";
                                    totalMS2No.Text = "";
                                    await Task.Delay(1);

                                    List<MetaboliteFeature> metaboliteFeatures = new List<MetaboliteFeature>();

                                    // ml ranking
                                    // Source must be array or IList.
                                    var source = Enumerable.Range(0, MS2Index.Count).ToArray();
                                    // Partition the entire source array.
                                    var rangePartitioner = Partitioner.Create(0, source.Length);
                                    // Loop over the partitions in parallel.
                                    Parallel.ForEach(rangePartitioner,
                                        new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.8) * 1.0)) },
                                        (range, loopState) =>
                                    {
                                        // Loop over each range element without a delegate invocation.
                                        for (int m = range.Item1; m < range.Item2; m++)
                                        {
                                            int k = MS2Index[m]; //actual MS2 index in the MS2 table, not scan number
                                            bool MS1Available = true;
                                            if (ms2model.MS2s[k].Ms1 == null || ms2model.MS2s[k].Ms1.Count == 0)
                                            {
                                                MS1Available = false;
                                            }

                                            //BUDDY search returns
                                            if (ms2model.MS2s[k].SeedMetabolite)
                                            {
                                                lock (metaboliteFeatures)
                                                {
                                                    metaboliteFeatures.Add(new MetaboliteFeature
                                                    {
                                                        ms2Index = k,
                                                        features = new List<Feature>(),
                                                        ms2 = ms2model.MS2s[k].OriSpectrum,
                                                        mz = ms2model.MS2s[k].Mz,
                                                        rt = ms2model.MS2s[k].Rt,
                                                        adduct = ms2model.MS2s[k].Adduct,
                                                        seedMetabolite = ms2model.MS2s[k].SeedMetabolite,
                                                        formulaGroundTruth = ms2model.MS2s[k].Formula_PC
                                                    });
                                                }
                                            }
                                            else // non-seed
                                            {
                                                if (batchBoosterOutput[m] == null || batchBoosterOutput[m].Count == 0) { continue; }

                                                //List<Feature> thisBUDDYOutput = null;
                                                List<Feature> thisBUDDYOutput = MLRanking(batchBoosterOutput[m], highResMSData, posIonMode, MS1Available, metaScoreBool);

                                                if (thisBUDDYOutput.Count > 0 && thisBUDDYOutput.Count <= MAX_CANDIDATE_SAVED)
                                                {
                                                    lock (metaboliteFeatures)
                                                    {
                                                        metaboliteFeatures.Add(new MetaboliteFeature
                                                        {
                                                            ms2Index = k,
                                                            features = thisBUDDYOutput,
                                                            ms2 = ms2model.MS2s[k].OriSpectrum,
                                                            mz = ms2model.MS2s[k].Mz,
                                                            rt = ms2model.MS2s[k].Rt,
                                                            adduct = ms2model.MS2s[k].Adduct,
                                                            seedMetabolite = false
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    lock (metaboliteFeatures)
                                                    {
                                                        metaboliteFeatures.Add(new MetaboliteFeature
                                                        {
                                                            ms2Index = k,
                                                            features = thisBUDDYOutput.Take(MAX_CANDIDATE_SAVED).ToList(),
                                                            ms2 = ms2model.MS2s[k].OriSpectrum,
                                                            mz = ms2model.MS2s[k].Mz,
                                                            rt = ms2model.MS2s[k].Rt,
                                                            adduct = ms2model.MS2s[k].Adduct,
                                                            seedMetabolite = false
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    });

                                    //w.Stop();
                                    //Debug.WriteLine("----------------------------------");
                                    //Debug.WriteLine("----------------------------------");
                                    //Debug.WriteLine("----------------------------------");
                                    //Debug.WriteLine("----------------------------------");
                                    //Debug.WriteLine("line 2431: " + w.ElapsedMilliseconds);

                                    // global optim
                                    if (metaboliteFeatures.Count > 1)
                                    {
                                        taskInProgress.Text = "Exp.-specific global annotation...";
                                        await Task.Delay(1);
                                        //Stopwatch w1 = new Stopwatch();
                                        //w1.Start();
                                        // fill in the formula element list in "Feature" class                                        
                                        Parallel.ForEach(metaboliteFeatures,
                                            new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.8) * 1.0)) },
                                            metaboliteFeature =>
                                        {
                                            if (metaboliteFeature.seedMetabolite)
                                            {
                                                int targetInd = (int)((metaboliteFeature.mz - metaboliteFeature.adduct.SumFormula.mass) / metaboliteFeature.adduct.M / GROUP_MZ_RANGE);
                                                if (targetInd > 0 && targetInd < (groupedDB.Count - 1))
                                                {
                                                    List<AlignedFormula> precursor_db_uf = new List<AlignedFormula>();
                                                    precursor_db_uf.AddRange(groupedDB[targetInd - 1]);
                                                    precursor_db_uf.AddRange(groupedDB[targetInd]);
                                                    precursor_db_uf.AddRange(groupedDB[targetInd + 1]);
                                                    List<AlignedFormula> matchFormula = precursor_db_uf.Where(o => o.formstr_neutral == metaboliteFeature.formulaGroundTruth).ToList();
                                                    if (matchFormula.Count > 0)
                                                    {
                                                        metaboliteFeature.seedFormulaElement = new FormulaElement(matchFormula[0]);
                                                    }
                                                    else
                                                    {
                                                        metaboliteFeature.seedFormulaElement = MILPVariable.FormulaParse(metaboliteFeature.formulaGroundTruth);
                                                    }
                                                }
                                                else
                                                {
                                                    metaboliteFeature.seedFormulaElement = MILPVariable.FormulaParse(metaboliteFeature.formulaGroundTruth);
                                                }
                                            }
                                            else
                                            {
                                                for (int n = 0; n < metaboliteFeature.features.Count; n++)
                                                {
                                                    if (metaboliteFeature.features[n].alignedFormula != null)
                                                    {
                                                        metaboliteFeature.features[n].formulaElement = new FormulaElement(metaboliteFeature.features[n].alignedFormula);
                                                    }
                                                    else
                                                    {
                                                        metaboliteFeature.features[n].formulaElement = MILPVariable.FormulaParse(metaboliteFeature.features[n].p_formula);
                                                    }
                                                }
                                            }
                                        });

                                        List<MetaboliteFeature> optimizedOutput = GlobalOptimization(connection_df, metaboliteFeatures, 20, ms1tol, ms2tol, ppm, 0.05, 0.6, 0.10, 0.05, 0.002); // RTtol, ms2Cutoff, A, abioB, bioB
                                        //w1.Stop();
                                        //Debug.WriteLine("----------------------------------");
                                        //Debug.WriteLine("----------------------------------");
                                        //Debug.WriteLine("----------------------------------");
                                        //Debug.WriteLine("----------------------------------");
                                        //Debug.WriteLine("line 2482: " + w1.ElapsedMilliseconds);


                                        // update metaboliteFeature, then update ms2model
                                        for (int m = 0; m < optimizedOutput.Count; m++)
                                        {
                                            int thisMS2Index = metaboliteFeatures[m].ms2Index;
                                            if (!ms2model.MS2s[thisMS2Index].SeedMetabolite)
                                            {
                                                ms2model.MS2s[thisMS2Index].candidates = metaboliteFeatures[m].features.OrderByDescending(o => o.MILPScore).ThenBy(o => o.relevance).ToList();
                                                ms2model.MS2s[thisMS2Index].Formula_PC = ms2model.MS2s[thisMS2Index].candidates[0].p_formula;
                                                ms2model.MS2s[thisMS2Index].ImageLink = "complete.png";
                                            }
                                            // fill in Feature Connections
                                            if (metaboliteFeatures[m].featureConnections != null)
                                            {
                                                ms2model.MS2s[thisMS2Index].FeatureConnections = metaboliteFeatures[m].featureConnections;
                                                // fill in scan No info
                                                foreach (FeatureConnection connection in ms2model.MS2s[thisMS2Index].FeatureConnections)
                                                {
                                                    connection.pairedMs2ScanNumber = ms2model.MS2s[connection.pairedMs2Index].ScanNumber;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int m = 0; m < metaboliteFeatures.Count; m++)
                                        {
                                            int thisMS2Index = metaboliteFeatures[m].ms2Index;
                                            if (!ms2model.MS2s[thisMS2Index].SeedMetabolite)
                                            {
                                                ms2model.MS2s[thisMS2Index].candidates = metaboliteFeatures[m].features;
                                                ms2model.MS2s[thisMS2Index].Formula_PC = ms2model.MS2s[thisMS2Index].candidates[0].p_formula;
                                                ms2model.MS2s[thisMS2Index].ImageLink = "complete.png";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // if not qualified for MILP-expSpecificPred, go through MLR and output results directly
                                    for (int m = 0; m < MS2Index.Count; m++)
                                    {
                                        int k = MS2Index[m]; //actual MS2 index in the MS2 table
                                        bool MS1Available = true;
                                        if (ms2model.MS2s[k].Ms1 == null || ms2model.MS2s[k].Ms1.Count == 0)
                                        {
                                            MS1Available = false;
                                        }

                                        if (ms2model.MS2s[k].SeedMetabolite) { continue; }

                                        //List<Feature> thisBUDDYOutput = null;
                                        List<Feature> thisBUDDYOutput = MLRanking(batchBoosterOutput[m], highResMSData, posIonMode, MS1Available, metaScoreBool);

                                        //BUDDY search returns
                                        if (thisBUDDYOutput == null || thisBUDDYOutput.Count == 0)
                                        {
                                            ms2model.MS2s[k].candidates = null;
                                            ms2model.MS2s[k].Formula_PC = "Unknown";
                                            ms2model.MS2s[k].ImageLink = "cancelled.png";
                                        }
                                        else if (thisBUDDYOutput.Count > 0 && thisBUDDYOutput.Count <= MAX_CANDIDATE_SAVED)
                                        {
                                            ms2model.MS2s[k].candidates = thisBUDDYOutput;
                                            ms2model.MS2s[k].Formula_PC = ms2model.MS2s[k].candidates[0].p_formula;
                                            ms2model.MS2s[k].ImageLink = "complete.png";
                                        }
                                        else
                                        {
                                            ms2model.MS2s[k].candidates = thisBUDDYOutput.Take(MAX_CANDIDATE_SAVED).ToList();
                                            ms2model.MS2s[k].Formula_PC = ms2model.MS2s[k].candidates[0].p_formula;
                                            ms2model.MS2s[k].ImageLink = "complete.png";
                                        }

                                        if (ms2model.MS2s[k].Equals((Ms2Utility)ms2grid.CurrentItem))
                                        {
                                            updateMS2TableAfterCalculation();
                                        }
                                    }

                                }

                            }

                        }
                    }
                }
                else // calculate by MS2
                {
                    for (int i = 0; i < ms2model.MS2s.Count; i++)
                    {
                        if (ms2model.MS2s[i].Selected)
                        {
                            if (ms2model.MS2s[i].SeedMetabolite)
                            {
                                calculateMS2Progressbar.Value++;
                                calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                //ms2model.MS2s[i].ImageLink = "complete.png";
                                continue;
                            }
                            if (ms2model.MS2s[i].Mz > 1498) //if precursor mz out of range, no results
                            {
                                ms2model.MS2s[i].candidates = null;
                                ms2model.MS2s[i].Formula_PC = "Unknown";
                                ms2model.MS2s[i].ImageLink = "cancelled.png";
                                calculateMS2Progressbar.Value++;
                                calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                continue;
                            }

                            ms2model.MS2s[i].Spectrum = new List<RAW_PeakElement>(ms2model.MS2s[i].OriSpectrum);
                            if (ms2model.MS2s[i].OriSpectrum.Count != 0) //if fragments, do deisotope, denoise and precursor exclusions
                            {
                                if (ms2model.MS2s[i].NoiseFragments != null)
                                {
                                    //ms2model.MS2s[i].Spectrum.AddRange(ms2model.MS2s[i].NoiseFragments);
                                    ms2model.MS2s[i].NoiseFragments = null;
                                }
                                if (ms2model.MS2s[i].IsotopeFragments != null)
                                {
                                    //ms2model.MS2s[i].Spectrum.AddRange(ms2model.MS2s[i].IsotopeFragments);
                                    ms2model.MS2s[i].IsotopeFragments = null;
                                }
                                if (ms2model.MS2s[i].PrecursorFragments != null)
                                {
                                    //ms2model.MS2s[i].Spectrum.AddRange(ms2model.MS2s[i].PrecursorFragments);
                                    ms2model.MS2s[i].PrecursorFragments = null;
                                }

                                if (denoise) //ms2 deisotope (including precursor fragments)
                                {
                                    List<RAW_PeakElement> rawMS2 = new List<RAW_PeakElement>(ms2model.MS2s[i].Spectrum);
                                    ms2model.MS2s[i].Spectrum = Booster.ms2denoise(ms2model.MS2s[i].Spectrum, maxNoiseFragRatio, maxNoiseRSD, maxNoiseInt);
                                    ms2model.MS2s[i].NoiseFragments = rawMS2.Except(ms2model.MS2s[i].Spectrum).ToList();
                                }

                                if (deisotope)
                                {
                                    List<RAW_PeakElement> rawMS2_2 = new List<RAW_PeakElement>(ms2model.MS2s[i].Spectrum);
                                    ms2model.MS2s[i].Spectrum = Booster.ms2deisotope(ms2model.MS2s[i].Spectrum, ms2tol, ppm);
                                    ms2model.MS2s[i].IsotopeFragments = rawMS2_2.Except(ms2model.MS2s[i].Spectrum).ToList();
                                }

                                // MS2 precursor exclusion
                                if (ppm)
                                {
                                    ms2model.MS2s[i].PrecursorFragments = ms2model.MS2s[i].Spectrum.Where(o => o.Mz >= (ms2model.MS2s[i].Mz - ms2tol * ms2model.MS2s[i].Mz * 1e-6)).ToList();
                                    ms2model.MS2s[i].Spectrum = ms2model.MS2s[i].Spectrum.Where(o => o.Mz < (ms2model.MS2s[i].Mz - ms2tol * ms2model.MS2s[i].Mz * 1e-6)).ToList();
                                }
                                else
                                {
                                    ms2model.MS2s[i].PrecursorFragments = ms2model.MS2s[i].Spectrum.Where(o => o.Mz >= (ms2model.MS2s[i].Mz - ms2tol)).ToList();
                                    ms2model.MS2s[i].Spectrum = ms2model.MS2s[i].Spectrum.Where(o => o.Mz < (ms2model.MS2s[i].Mz - ms2tol)).ToList();
                                }

                                // top frag No
                                if (!use_allFrag)
                                {
                                    if (ms2model.MS2s[i].Spectrum.Count > topfragNo)
                                    {
                                        List<RAW_PeakElement> rawMS2_3 = new List<RAW_PeakElement>(ms2model.MS2s[i].Spectrum);
                                        ms2model.MS2s[i].Spectrum = ms2model.MS2s[i].Spectrum.OrderByDescending(o => o.Intensity).ToList().Take(topfragNo).ToList();
                                        if (ms2model.MS2s[i].NoiseFragments != null)
                                        {
                                            ms2model.MS2s[i].NoiseFragments.AddRange(rawMS2_3.Except(ms2model.MS2s[i].Spectrum).ToList());
                                        }
                                        else
                                        {
                                            ms2model.MS2s[i].NoiseFragments = rawMS2_3.Except(ms2model.MS2s[i].Spectrum).ToList();
                                        }
                                    }
                                }
                                //orber by mz
                                ms2model.MS2s[i].Spectrum = ms2model.MS2s[i].Spectrum.OrderBy(o => o.Mz).ToList();
                            }

                            // Recalculate sumformula in case user change the adduct setting.
                            try
                            {
                                ms2model.MS2s[i].Adduct = new Adduct(ms2model.MS2s[i].Adduct.Formula);
                            }
                            catch
                            {
                                ms2model.MS2s[i].candidates = null;
                                ms2model.MS2s[i].Formula_PC = "Unknown";
                                ms2model.MS2s[i].ImageLink = "cancelled.png";
                                calculateMS2Progressbar.Value++;
                                calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                                continue;
                            }

                            Ms2Utility ms2 = new Ms2Utility(ms2model.MS2s[i].Mz, ms2model.MS2s[i].polarity, ms2model.MS2s[i].OriSpectrum, ms2model.MS2s[i].Adduct, ms2tol, ms1tol, ms2model.MS2s[i].Ms1, ms2model.MS2s[i].Spectrum);

                            // Booster main program
                            //List<Feature> thisBoosterOutput = null;

                            // DEBUG
                            List<Feature> thisBoosterOutput = new List<Feature>(await Task.Run(() => BoosterCalculation(ppm, ms1tol, ms2tol, topCandidateNum,
                                                    topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction, ms2, i, timeout, OPratiocheck)));
                            //List<Feature> thisBoosterOutput = new List<Feature>(BoosterCalculation(ppm, ms1tol, ms2tol, topCandidateNum,
                            //                                    topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction, ms2, i, timeout, OPratiocheck));


                            bool MS1Available = true;
                            if (ms2model.MS2s[i].Ms1 == null || ms2model.MS2s[i].Ms1.Count == 0)
                            {
                                MS1Available = false;
                            }
                            bool posIonMode = true;
                            if (ms2model.MS2s[i].Polarity == "N")
                            {
                                posIonMode = false;
                            }

                            // MS2-based expFragNo/Int correction
                            List<Feature> thisBoosterOutputCorrected = ExpFragCorrection(thisBoosterOutput, highResMSData, posIonMode);

                            //List<Feature> thisBUDDYOutput = null;
                            List<Feature> thisBUDDYOutput = MLRanking(thisBoosterOutputCorrected, highResMSData, posIonMode, MS1Available, metaScoreBool);

                            //BUDDY search returns
                            if (thisBUDDYOutput == null || thisBUDDYOutput.Count == 0)
                            {
                                ms2model.MS2s[i].candidates = thisBUDDYOutput;
                                ms2model.MS2s[i].Formula_PC = "Unknown";
                                ms2model.MS2s[i].ImageLink = "cancelled.png";
                            }
                            else if (thisBUDDYOutput.Count > 0 && thisBUDDYOutput.Count <= MAX_CANDIDATE_SAVED)
                            {
                                ms2model.MS2s[i].candidates = thisBUDDYOutput;
                                ms2model.MS2s[i].Formula_PC = ms2model.MS2s[i].candidates[0].p_formula;
                                ms2model.MS2s[i].ImageLink = "complete.png";
                            }
                            else
                            {
                                ms2model.MS2s[i].candidates = thisBUDDYOutput.Take(MAX_CANDIDATE_SAVED).ToList();
                                ms2model.MS2s[i].Formula_PC = ms2model.MS2s[i].candidates[0].p_formula;
                                ms2model.MS2s[i].ImageLink = "complete.png";
                            }
                            calculateMS2Progressbar.Value++;
                            calculatedMS2No.Text = calculateMS2Progressbar.Value.ToString();
                            if (ms2model.MS2s[i].Equals((Ms2Utility)ms2grid.CurrentItem))
                            {
                                updateMS2TableAfterCalculation();
                            }
                        }
                    }
                }
            }

            // update MS2Utility in files
            for (int i = 0; i < filemodel.Files.Count; i++)
            {
                if (filemodel.Files[i].Selected)
                {
                    List<Ms2Utility> updatedMS2List = new List<Ms2Utility>();
                    updatedMS2List.AddRange(ms2model.MS2s.Where(o => o.FileIndex == filemodel.Files[i].Index).ToList());

                    // update MS2List in File
                    filemodel.Files[i].MS2List = updatedMS2List;
                }
            }

            //calculateMS2Progressbar.Visibility = Visibility.Collapsed;
            taskInProgress.Text = "Computation completed. ";
            //calculationProgressSlash.Visibility = Visibility.Collapsed;
            //totalMS2No.Visibility = Visibility.Collapsed;
            ComputationCompleted();
            calculationInProgress = false;
        }


        //Booster async
        private List<Feature> BoosterCalculation(bool ppm, double ms1tol, double ms2tol, int topCandidateNum, bool topcandcut, int max_isotopic_peaks,
            double isotopic_abundance_cutoff, double isp_grp_mass_tol, FormulaRestriction restriction, Ms2Utility ms2, int i, double timeout, bool OPratiocheck)
        {

            List<Feature> boosterOutput = new List<Feature>();
            bool Booster0Redo = true;

            if (ms2.Spectrum.Count == 0 || ms2.adduct.AbsCharge > 1) //if no ms2 specturm, run precursor search ONLY function
            {
                if (ms2.Polarity == "P")
                {
                    boosterOutput = Booster.Booster0(ms2, ppm,
                        groupedDB, distPos_df, pre_dist_df, (UInt32)i, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction);
                }
                else
                {
                    boosterOutput = Booster.Booster0(ms2, ppm,
                        groupedDB, distNeg_df, pre_dist_df, (UInt32)i, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction);
                }
                Booster0Redo = false;
            }
            else
            {
                if (ms2.Polarity == "P")
                {
                    boosterOutput = Booster.Booster1(ms2, ppm,
                        groupedDB, distPos_df, pre_dist_df, (UInt32)i, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, timeout, restriction);
                }
                else
                {
                    boosterOutput = Booster.Booster1(ms2, ppm,
                        groupedDB, distNeg_df, pre_dist_df, (UInt32)i, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, timeout, restriction);
                }
                // for later redo Booster0
                if (boosterOutput.Count > 0)
                {
                    if (boosterOutput[0].expfIntRatio == 0.0)
                    {
                        Booster0Redo = false;
                    }
                }
            }

            //BUDDY search returns no output
            if (boosterOutput.Count == 0)
            {
                return new List<Feature>();
            }

            //filter BUDDY candidates by element & database

            //element count restriction
            boosterOutput = boosterOutput.Where(o => o.cfdf[0].c >= restriction.C_min &&
                        o.cfdf[0].h >= restriction.H_min &&
                        o.cfdf[0].n >= restriction.N_min &&
                        o.cfdf[0].o >= restriction.O_min &&
                        o.cfdf[0].p >= restriction.P_min &&
                        o.cfdf[0].s >= restriction.S_min &&
                        o.cfdf[0].f >= restriction.F_min &&
                        o.cfdf[0].cl >= restriction.Cl_min &&
                        o.cfdf[0].br >= restriction.Br_min &&
                        o.cfdf[0].i >= restriction.I_min &&
                        o.cfdf[0].si >= restriction.Si_min &&
                        o.cfdf[0].b >= restriction.B_min &&
                        o.cfdf[0].se >= restriction.Se_min &&
                        o.cfdf[0].c <= restriction.C_max &&
                        o.cfdf[0].h <= restriction.H_max &&
                        o.cfdf[0].n <= restriction.N_max &&
                        o.cfdf[0].o <= restriction.O_max &&
                        o.cfdf[0].p <= restriction.P_max &&
                        o.cfdf[0].s <= restriction.S_max &&
                        o.cfdf[0].f <= restriction.F_max &&
                        o.cfdf[0].cl <= restriction.Cl_max &&
                        o.cfdf[0].br <= restriction.Br_max &&
                        o.cfdf[0].i <= restriction.I_max &&
                        o.cfdf[0].si <= restriction.Si_max &&
                        o.cfdf[0].b <= restriction.B_max &&
                        o.cfdf[0].se <= restriction.Se_max).ToList();


            //element ratio restriction
            List<Feature> nonCfeature = new List<Feature>(boosterOutput.Where(o => o.cfdf[0].c == 0).ToList());
            boosterOutput = boosterOutput.Where(o => o.cfdf[0].c > 0).ToList();
            boosterOutput = boosterOutput.Where(r => (double)r.cfdf[0].h / r.cfdf[0].c >= restriction.H_C_min &&
                    (double)r.cfdf[0].o / r.cfdf[0].c >= restriction.O_C_min &&
                    (double)r.cfdf[0].n / r.cfdf[0].c >= restriction.N_C_min &&
                    (double)r.cfdf[0].p / r.cfdf[0].c >= restriction.P_C_min &&
                    (double)r.cfdf[0].s / r.cfdf[0].c >= restriction.S_C_min &&
                    (double)r.cfdf[0].f / r.cfdf[0].c >= restriction.F_C_min &&
                    (double)r.cfdf[0].cl / r.cfdf[0].c >= restriction.Cl_C_min &&
                    (double)r.cfdf[0].br / r.cfdf[0].c >= restriction.Br_C_min &&
                    (double)r.cfdf[0].si / r.cfdf[0].c >= restriction.Si_C_min &&
                    (double)r.cfdf[0].h / r.cfdf[0].c <= restriction.H_C_max &&
                    (double)r.cfdf[0].o / r.cfdf[0].c <= restriction.O_C_max &&
                    (double)r.cfdf[0].n / r.cfdf[0].c <= restriction.N_C_max &&
                    (double)r.cfdf[0].p / r.cfdf[0].c <= restriction.P_C_max &&
                    (double)r.cfdf[0].s / r.cfdf[0].c <= restriction.S_C_max &&
                    (double)r.cfdf[0].f / r.cfdf[0].c <= restriction.F_C_max &&
                    (double)r.cfdf[0].cl / r.cfdf[0].c <= restriction.Cl_C_max &&
                    (double)r.cfdf[0].br / r.cfdf[0].c <= restriction.Br_C_max &&
                    (double)r.cfdf[0].si / r.cfdf[0].c <= restriction.Si_C_max).ToList();
            boosterOutput.AddRange(nonCfeature);

            // O >= 3*P
            if (OPratiocheck)
            {
                boosterOutput = boosterOutput.Where(o => o.cfdf[0].o >= 3 * o.cfdf[0].p).ToList();
            }


            //databse restriction
            if (boosterOutput.Count > 0)
            {
                if (!(bool)localSettings.Values["NoResDB_include"])
                {
                    //formula db restriction
                    int targetInd = (int)((ms2.Mz * ms2.Adduct.AbsCharge - ms2.Adduct.SumFormula.mass) / ms2.Adduct.M / GROUP_MZ_RANGE);
                    if (targetInd <= 0 || targetInd >= groupedDB.Count - 1)
                    {
                        return new List<Feature>();
                    }
                    List<AlignedFormula> precursor_db_uf = new List<AlignedFormula>();
                    precursor_db_uf.AddRange(groupedDB[targetInd - 1]);
                    precursor_db_uf.AddRange(groupedDB[targetInd]);
                    precursor_db_uf.AddRange(groupedDB[targetInd + 1]);
                    //database restriction
                    #region
                    List<AlignedFormula> PubChem_include = new List<AlignedFormula>();
                    List<AlignedFormula> ANPDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> BLEXP_include = new List<AlignedFormula>();
                    List<AlignedFormula> BMDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> ChEBI_include = new List<AlignedFormula>();
                    List<AlignedFormula> COCONUT_include = new List<AlignedFormula>();
                    List<AlignedFormula> DrugBank_include = new List<AlignedFormula>();
                    List<AlignedFormula> DSSTOX_include = new List<AlignedFormula>();
                    List<AlignedFormula> ECMDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> FooDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> HMDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> HSDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> KEGG_include = new List<AlignedFormula>();
                    List<AlignedFormula> LMSD_include = new List<AlignedFormula>();
                    List<AlignedFormula> MaConDa_include = new List<AlignedFormula>();
                    List<AlignedFormula> MarkerDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> MCDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> NORMAN_include = new List<AlignedFormula>();
                    List<AlignedFormula> NPASS_include = new List<AlignedFormula>();
                    List<AlignedFormula> Plantcyc_include = new List<AlignedFormula>();
                    List<AlignedFormula> SMPDB_include = new List<AlignedFormula>();
                    List<AlignedFormula> STF_IDENT_include = new List<AlignedFormula>();
                    List<AlignedFormula> T3DB_include = new List<AlignedFormula>();
                    List<AlignedFormula> TTD_include = new List<AlignedFormula>();
                    List<AlignedFormula> UNPD_include = new List<AlignedFormula>();
                    List<AlignedFormula> YMDB_include = new List<AlignedFormula>();

                    if ((bool)localSettings.Values["PubChem_include"])
                    {
                        PubChem_include = precursor_db_uf.Where(o => o.PubChem > 0).ToList();
                    }
                    if ((bool)localSettings.Values["ANPDB_include"])
                    {
                        ANPDB_include = precursor_db_uf.Where(o => o.ANPDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["BLEXP_include"])
                    {
                        BLEXP_include = precursor_db_uf.Where(o => o.BLEXP > 0).ToList();
                    }
                    if ((bool)localSettings.Values["BMDB_include"])
                    {
                        BMDB_include = precursor_db_uf.Where(o => o.BMDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["ChEBI_include"])
                    {
                        ChEBI_include = precursor_db_uf.Where(o => o.ChEBI > 0).ToList();
                    }
                    if ((bool)localSettings.Values["COCONUT_include"])
                    {
                        COCONUT_include = precursor_db_uf.Where(o => o.COCONUT > 0).ToList();
                    }
                    if ((bool)localSettings.Values["DrugBank_include"])
                    {
                        DrugBank_include = precursor_db_uf.Where(o => o.DrugBank > 0).ToList();
                    }
                    if ((bool)localSettings.Values["DSSTOX_include"])
                    {
                        DSSTOX_include = precursor_db_uf.Where(o => o.DSSTOX > 0).ToList();
                    }
                    if ((bool)localSettings.Values["ECMDB_include"])
                    {
                        ECMDB_include = precursor_db_uf.Where(o => o.ECMDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["FooDB_include"])
                    {
                        FooDB_include = precursor_db_uf.Where(o => o.FooDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["HMDB_include"])
                    {
                        HMDB_include = precursor_db_uf.Where(o => o.HMDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["HSDB_include"])
                    {
                        HSDB_include = precursor_db_uf.Where(o => o.HSDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["KEGG_include"])
                    {
                        KEGG_include = precursor_db_uf.Where(o => o.KEGG > 0).ToList();
                    }
                    if ((bool)localSettings.Values["LMSD_include"])
                    {
                        LMSD_include = precursor_db_uf.Where(o => o.LMSD > 0).ToList();
                    }
                    if ((bool)localSettings.Values["MaConDa_include"])
                    {
                        MaConDa_include = precursor_db_uf.Where(o => o.MaConDa > 0).ToList();
                    }
                    if ((bool)localSettings.Values["MarkerDB_include"])
                    {
                        MarkerDB_include = precursor_db_uf.Where(o => o.MarkerDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["MCDB_include"])
                    {
                        MCDB_include = precursor_db_uf.Where(o => o.MCDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["NORMAN_include"])
                    {
                        NORMAN_include = precursor_db_uf.Where(o => o.NORMAN > 0).ToList();
                    }
                    if ((bool)localSettings.Values["NPASS_include"])
                    {
                        NPASS_include = precursor_db_uf.Where(o => o.NPASS > 0).ToList();
                    }
                    if ((bool)localSettings.Values["Plantcyc_include"])
                    {
                        Plantcyc_include = precursor_db_uf.Where(o => o.Plantcyc > 0).ToList();
                    }
                    if ((bool)localSettings.Values["SMPDB_include"])
                    {
                        SMPDB_include = precursor_db_uf.Where(o => o.SMPDB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["STF_IDENT_include"])
                    {
                        STF_IDENT_include = precursor_db_uf.Where(o => o.STOFF_IDENT > 0).ToList();
                    }
                    if ((bool)localSettings.Values["T3DB_include"])
                    {
                        T3DB_include = precursor_db_uf.Where(o => o.T3DB > 0).ToList();
                    }
                    if ((bool)localSettings.Values["TTD_include"])
                    {
                        TTD_include = precursor_db_uf.Where(o => o.TTD > 0).ToList();
                    }
                    if ((bool)localSettings.Values["UNPD_include"])
                    {
                        UNPD_include = precursor_db_uf.Where(o => o.UNPD > 0).ToList();
                    }
                    if ((bool)localSettings.Values["YMDB_include"])
                    {
                        YMDB_include = precursor_db_uf.Where(o => o.YMDB > 0).ToList();
                    }

                    precursor_db_uf.Clear();
                    precursor_db_uf.AddRange(PubChem_include);
                    precursor_db_uf.AddRange(ANPDB_include);
                    precursor_db_uf.AddRange(BLEXP_include);
                    precursor_db_uf.AddRange(BMDB_include);
                    precursor_db_uf.AddRange(ChEBI_include);
                    precursor_db_uf.AddRange(COCONUT_include);
                    precursor_db_uf.AddRange(DrugBank_include);
                    precursor_db_uf.AddRange(DSSTOX_include);
                    precursor_db_uf.AddRange(ECMDB_include);
                    precursor_db_uf.AddRange(FooDB_include);
                    precursor_db_uf.AddRange(HMDB_include);
                    precursor_db_uf.AddRange(HSDB_include);
                    precursor_db_uf.AddRange(KEGG_include);
                    precursor_db_uf.AddRange(LMSD_include);
                    precursor_db_uf.AddRange(MaConDa_include);
                    precursor_db_uf.AddRange(MarkerDB_include);
                    precursor_db_uf.AddRange(MCDB_include);
                    precursor_db_uf.AddRange(NORMAN_include);
                    precursor_db_uf.AddRange(NPASS_include);
                    precursor_db_uf.AddRange(Plantcyc_include);
                    precursor_db_uf.AddRange(SMPDB_include);
                    precursor_db_uf.AddRange(STF_IDENT_include);
                    precursor_db_uf.AddRange(T3DB_include);
                    precursor_db_uf.AddRange(TTD_include);
                    precursor_db_uf.AddRange(UNPD_include);
                    precursor_db_uf.AddRange(YMDB_include);
                    precursor_db_uf = precursor_db_uf.DistinctBy(o => o.formstr_neutral).ToList();
                    #endregion

                    List<Feature> filteredBooOutput = new List<Feature>();
                    for (int j = 0; j < boosterOutput.Count; j++)
                    {
                        List<AlignedFormula> matchFormula = precursor_db_uf.Where(o => o.formstr_neutral == boosterOutput[j].p_formula).ToList();
                        if (matchFormula.Count > 0)
                        {
                            boosterOutput[j].alignedFormula = matchFormula[0];
                            filteredBooOutput.Add(boosterOutput[j]);
                        }
                    }
                    boosterOutput = filteredBooOutput;
                }
                else
                {
                    //No formula db restriction
                    int targetInd = (int)((ms2.Mz * ms2.Adduct.AbsCharge - ms2.Adduct.SumFormula.mass) / ms2.Adduct.M / GROUP_MZ_RANGE);
                    if (targetInd <= 0 || targetInd >= groupedDB.Count - 1)
                    {
                        return new List<Feature>();
                    }
                    List<AlignedFormula> precursor_db_uf = new List<AlignedFormula>();
                    precursor_db_uf.AddRange(groupedDB[targetInd - 1]);
                    precursor_db_uf.AddRange(groupedDB[targetInd]);
                    precursor_db_uf.AddRange(groupedDB[targetInd + 1]);
                    for (int j = 0; j < boosterOutput.Count; j++)
                    {
                        List<AlignedFormula> matchFormula = precursor_db_uf.Where(o => o.formstr_neutral == boosterOutput[j].p_formula).ToList();
                        if (matchFormula.Count > 0)
                        {
                            boosterOutput[j].alignedFormula = matchFormula[0];
                        }
                    }
                }
            }

            // after finishing all restrictions for Booster1, if no candidates, go back to Booster0 -> restriction
            if (boosterOutput.Count == 0 && Booster0Redo)
            {
                List<Feature> boosterOutput1 = new List<Feature>();
                if (ms2.Polarity == "P")
                {
                    boosterOutput1 = Booster.Booster0(ms2, ppm,
                        groupedDB, distPos_df, pre_dist_df, (UInt32)i, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction);
                }
                else
                {
                    boosterOutput1 = Booster.Booster0(ms2, ppm,
                        groupedDB, distNeg_df, pre_dist_df, (UInt32)i, topCandidateNum, topcandcut, max_isotopic_peaks, isotopic_abundance_cutoff, isp_grp_mass_tol, restriction);
                }

                //BUDDY search returns no output
                if (boosterOutput1.Count == 0)
                {
                    return new List<Feature>();
                }
                else //filter BUDDY candidates by element & database
                {
                    //element count restriction
                    boosterOutput1 = boosterOutput1.Where(o => o.cfdf[0].c >= restriction.C_min &&
                                o.cfdf[0].h >= restriction.H_min &&
                                o.cfdf[0].n >= restriction.N_min &&
                                o.cfdf[0].o >= restriction.O_min &&
                                o.cfdf[0].p >= restriction.P_min &&
                                o.cfdf[0].s >= restriction.S_min &&
                                o.cfdf[0].f >= restriction.F_min &&
                                o.cfdf[0].cl >= restriction.Cl_min &&
                                o.cfdf[0].br >= restriction.Br_min &&
                                o.cfdf[0].i >= restriction.I_min &&
                                o.cfdf[0].si >= restriction.Si_min &&
                                o.cfdf[0].b >= restriction.B_min &&
                                o.cfdf[0].se >= restriction.Se_min &&
                                o.cfdf[0].c <= restriction.C_max &&
                                o.cfdf[0].h <= restriction.H_max &&
                                o.cfdf[0].n <= restriction.N_max &&
                                o.cfdf[0].o <= restriction.O_max &&
                                o.cfdf[0].p <= restriction.P_max &&
                                o.cfdf[0].s <= restriction.S_max &&
                                o.cfdf[0].f <= restriction.F_max &&
                                o.cfdf[0].cl <= restriction.Cl_max &&
                                o.cfdf[0].br <= restriction.Br_max &&
                                o.cfdf[0].i <= restriction.I_max &&
                                o.cfdf[0].si <= restriction.Si_max &&
                                o.cfdf[0].b <= restriction.B_max &&
                                o.cfdf[0].se <= restriction.Se_max).ToList();

                    //all candidates are filtered
                    if (boosterOutput1.Count == 0)
                    {
                        return new List<Feature>();
                    }

                    //element ratio restriction
                    List<Feature> nonCfeature1 = new List<Feature>(boosterOutput1.Where(o => o.cfdf[0].c == 0).ToList());
                    boosterOutput1 = boosterOutput1.Where(o => o.cfdf[0].c > 0).Where(r => (double)r.cfdf[0].h / r.cfdf[0].c >= restriction.H_C_min &&
                            (double)r.cfdf[0].o / r.cfdf[0].c >= restriction.O_C_min &&
                            (double)r.cfdf[0].n / r.cfdf[0].c >= restriction.N_C_min &&
                            (double)r.cfdf[0].p / r.cfdf[0].c >= restriction.P_C_min &&
                            (double)r.cfdf[0].s / r.cfdf[0].c >= restriction.S_C_min &&
                            (double)r.cfdf[0].f / r.cfdf[0].c >= restriction.F_C_min &&
                            (double)r.cfdf[0].cl / r.cfdf[0].c >= restriction.Cl_C_min &&
                            (double)r.cfdf[0].br / r.cfdf[0].c >= restriction.Br_C_min &&
                            (double)r.cfdf[0].si / r.cfdf[0].c >= restriction.Si_C_min &&
                            (double)r.cfdf[0].h / r.cfdf[0].c <= restriction.H_C_max &&
                            (double)r.cfdf[0].o / r.cfdf[0].c <= restriction.O_C_max &&
                            (double)r.cfdf[0].n / r.cfdf[0].c <= restriction.N_C_max &&
                            (double)r.cfdf[0].p / r.cfdf[0].c <= restriction.P_C_max &&
                            (double)r.cfdf[0].s / r.cfdf[0].c <= restriction.S_C_max &&
                            (double)r.cfdf[0].f / r.cfdf[0].c <= restriction.F_C_max &&
                            (double)r.cfdf[0].cl / r.cfdf[0].c <= restriction.Cl_C_max &&
                            (double)r.cfdf[0].br / r.cfdf[0].c <= restriction.Br_C_max &&
                            (double)r.cfdf[0].si / r.cfdf[0].c <= restriction.Si_C_max).ToList();
                    boosterOutput1.AddRange(nonCfeature1);

                    // O >= 3*P
                    if (OPratiocheck)
                    {
                        boosterOutput1 = boosterOutput1.Where(o => o.cfdf[0].o >= 3 * o.cfdf[0].p).ToList();
                    }

                    //all candidates filtered
                    if (boosterOutput1.Count == 0)
                    {
                        return new List<Feature>();
                    }

                    //databse restriction
                    if (!(bool)localSettings.Values["NoResDB_include"])
                    {
                        //formula db restriction
                        int targetInd = (int)((ms2.Mz * ms2.Adduct.AbsCharge - ms2.Adduct.SumFormula.mass) / GROUP_MZ_RANGE);
                        if (targetInd <= 0 || targetInd >= groupedDB.Count - 1)
                        {
                            return new List<Feature>();
                        }
                        List<AlignedFormula> precursor_db_uf = new List<AlignedFormula>();
                        precursor_db_uf.AddRange(groupedDB[targetInd - 1]);
                        precursor_db_uf.AddRange(groupedDB[targetInd]);
                        precursor_db_uf.AddRange(groupedDB[targetInd + 1]);
                        //database restriction
                        #region
                        List<AlignedFormula> PubChem_include = new List<AlignedFormula>();
                        List<AlignedFormula> ANPDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> BLEXP_include = new List<AlignedFormula>();
                        List<AlignedFormula> BMDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> ChEBI_include = new List<AlignedFormula>();
                        List<AlignedFormula> COCONUT_include = new List<AlignedFormula>();
                        List<AlignedFormula> DrugBank_include = new List<AlignedFormula>();
                        List<AlignedFormula> DSSTOX_include = new List<AlignedFormula>();
                        List<AlignedFormula> ECMDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> FooDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> HMDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> HSDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> KEGG_include = new List<AlignedFormula>();
                        List<AlignedFormula> LMSD_include = new List<AlignedFormula>();
                        List<AlignedFormula> MaConDa_include = new List<AlignedFormula>();
                        List<AlignedFormula> MarkerDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> MCDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> NORMAN_include = new List<AlignedFormula>();
                        List<AlignedFormula> NPASS_include = new List<AlignedFormula>();
                        List<AlignedFormula> Plantcyc_include = new List<AlignedFormula>();
                        List<AlignedFormula> SMPDB_include = new List<AlignedFormula>();
                        List<AlignedFormula> STF_IDENT_include = new List<AlignedFormula>();
                        List<AlignedFormula> T3DB_include = new List<AlignedFormula>();
                        List<AlignedFormula> TTD_include = new List<AlignedFormula>();
                        List<AlignedFormula> UNPD_include = new List<AlignedFormula>();
                        List<AlignedFormula> YMDB_include = new List<AlignedFormula>();

                        if ((bool)localSettings.Values["PubChem_include"])
                        {
                            PubChem_include = precursor_db_uf.Where(o => o.PubChem > 0).ToList();
                        }
                        if ((bool)localSettings.Values["ANPDB_include"])
                        {
                            ANPDB_include = precursor_db_uf.Where(o => o.ANPDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["BLEXP_include"])
                        {
                            BLEXP_include = precursor_db_uf.Where(o => o.BLEXP > 0).ToList();
                        }
                        if ((bool)localSettings.Values["BMDB_include"])
                        {
                            BMDB_include = precursor_db_uf.Where(o => o.BMDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["ChEBI_include"])
                        {
                            ChEBI_include = precursor_db_uf.Where(o => o.ChEBI > 0).ToList();
                        }
                        if ((bool)localSettings.Values["COCONUT_include"])
                        {
                            COCONUT_include = precursor_db_uf.Where(o => o.COCONUT > 0).ToList();
                        }
                        if ((bool)localSettings.Values["DrugBank_include"])
                        {
                            DrugBank_include = precursor_db_uf.Where(o => o.DrugBank > 0).ToList();
                        }
                        if ((bool)localSettings.Values["DSSTOX_include"])
                        {
                            DSSTOX_include = precursor_db_uf.Where(o => o.DSSTOX > 0).ToList();
                        }
                        if ((bool)localSettings.Values["ECMDB_include"])
                        {
                            ECMDB_include = precursor_db_uf.Where(o => o.ECMDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["FooDB_include"])
                        {
                            FooDB_include = precursor_db_uf.Where(o => o.FooDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["HMDB_include"])
                        {
                            HMDB_include = precursor_db_uf.Where(o => o.HMDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["HSDB_include"])
                        {
                            HSDB_include = precursor_db_uf.Where(o => o.HSDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["KEGG_include"])
                        {
                            KEGG_include = precursor_db_uf.Where(o => o.KEGG > 0).ToList();
                        }
                        if ((bool)localSettings.Values["LMSD_include"])
                        {
                            LMSD_include = precursor_db_uf.Where(o => o.LMSD > 0).ToList();
                        }
                        if ((bool)localSettings.Values["MaConDa_include"])
                        {
                            MaConDa_include = precursor_db_uf.Where(o => o.MaConDa > 0).ToList();
                        }
                        if ((bool)localSettings.Values["MarkerDB_include"])
                        {
                            MarkerDB_include = precursor_db_uf.Where(o => o.MarkerDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["MCDB_include"])
                        {
                            MCDB_include = precursor_db_uf.Where(o => o.MCDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["NORMAN_include"])
                        {
                            NORMAN_include = precursor_db_uf.Where(o => o.NORMAN > 0).ToList();
                        }
                        if ((bool)localSettings.Values["NPASS_include"])
                        {
                            NPASS_include = precursor_db_uf.Where(o => o.NPASS > 0).ToList();
                        }
                        if ((bool)localSettings.Values["Plantcyc_include"])
                        {
                            Plantcyc_include = precursor_db_uf.Where(o => o.Plantcyc > 0).ToList();
                        }
                        if ((bool)localSettings.Values["SMPDB_include"])
                        {
                            SMPDB_include = precursor_db_uf.Where(o => o.SMPDB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["STF_IDENT_include"])
                        {
                            STF_IDENT_include = precursor_db_uf.Where(o => o.STOFF_IDENT > 0).ToList();
                        }
                        if ((bool)localSettings.Values["T3DB_include"])
                        {
                            T3DB_include = precursor_db_uf.Where(o => o.T3DB > 0).ToList();
                        }
                        if ((bool)localSettings.Values["TTD_include"])
                        {
                            TTD_include = precursor_db_uf.Where(o => o.TTD > 0).ToList();
                        }
                        if ((bool)localSettings.Values["UNPD_include"])
                        {
                            UNPD_include = precursor_db_uf.Where(o => o.UNPD > 0).ToList();
                        }
                        if ((bool)localSettings.Values["YMDB_include"])
                        {
                            YMDB_include = precursor_db_uf.Where(o => o.YMDB > 0).ToList();
                        }

                        precursor_db_uf.Clear();
                        precursor_db_uf.AddRange(PubChem_include);
                        precursor_db_uf.AddRange(ANPDB_include);
                        precursor_db_uf.AddRange(BLEXP_include);
                        precursor_db_uf.AddRange(BMDB_include);
                        precursor_db_uf.AddRange(ChEBI_include);
                        precursor_db_uf.AddRange(COCONUT_include);
                        precursor_db_uf.AddRange(DrugBank_include);
                        precursor_db_uf.AddRange(DSSTOX_include);
                        precursor_db_uf.AddRange(ECMDB_include);
                        precursor_db_uf.AddRange(FooDB_include);
                        precursor_db_uf.AddRange(HMDB_include);
                        precursor_db_uf.AddRange(HSDB_include);
                        precursor_db_uf.AddRange(KEGG_include);
                        precursor_db_uf.AddRange(LMSD_include);
                        precursor_db_uf.AddRange(MaConDa_include);
                        precursor_db_uf.AddRange(MarkerDB_include);
                        precursor_db_uf.AddRange(MCDB_include);
                        precursor_db_uf.AddRange(NORMAN_include);
                        precursor_db_uf.AddRange(NPASS_include);
                        precursor_db_uf.AddRange(Plantcyc_include);
                        precursor_db_uf.AddRange(SMPDB_include);
                        precursor_db_uf.AddRange(STF_IDENT_include);
                        precursor_db_uf.AddRange(T3DB_include);
                        precursor_db_uf.AddRange(TTD_include);
                        precursor_db_uf.AddRange(UNPD_include);
                        precursor_db_uf.AddRange(YMDB_include);
                        precursor_db_uf = precursor_db_uf.DistinctBy(o => o.formstr_neutral).ToList();
                        #endregion

                        List<Feature> filteredBooOutput = new List<Feature>();
                        for (int j = 0; j < boosterOutput1.Count; j++)
                        {
                            List<AlignedFormula> matchFormula = precursor_db_uf.Where(o => o.formstr_neutral == boosterOutput1[j].p_formula).ToList();
                            if (matchFormula.Count > 0)
                            {
                                boosterOutput1[j].alignedFormula = matchFormula[0];
                                filteredBooOutput.Add(boosterOutput1[j]);
                            }
                        }
                        boosterOutput1 = filteredBooOutput;
                    }
                    else
                    {
                        //No formula db restriction
                        int targetInd = (int)((ms2.Mz * ms2.Adduct.AbsCharge - ms2.Adduct.SumFormula.mass) / GROUP_MZ_RANGE);
                        if (targetInd <= 0 || targetInd >= groupedDB.Count - 1)
                        {
                            return new List<Feature>();
                        }
                        List<AlignedFormula> precursor_db_uf = new List<AlignedFormula>();
                        precursor_db_uf.AddRange(groupedDB[targetInd - 1]);
                        precursor_db_uf.AddRange(groupedDB[targetInd]);
                        precursor_db_uf.AddRange(groupedDB[targetInd + 1]);
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            List<AlignedFormula> matchFormula = precursor_db_uf.Where(o => o.formstr_neutral == boosterOutput1[j].p_formula).ToList();
                            if (matchFormula.Count > 0)
                            {
                                boosterOutput1[j].alignedFormula = matchFormula[0];
                            }
                        }
                    }

                    if (boosterOutput1.Count == 0)
                    {
                        return new List<Feature>();
                    }
                }
                boosterOutput = boosterOutput1;
                //Debug.WriteLine("Line 2236 " + boosterOutput1.Count);
            }

            if (boosterOutput.Count == 0 && Booster0Redo == false)
            {
                return new List<Feature>();
            }

            return boosterOutput;

        }

        #region
        private List<Feature> MLRanking(List<Feature> boosterOutput, bool highRes, bool posIonMode, bool MS1Available, bool metaScoreBool)
        {
            if (boosterOutput == null || boosterOutput.Count == 0)
            {
                return new List<Feature>();
            }

            //ml model ranking
            List<MlModel> testModel = new List<MlModel>();
            foreach (Feature ft in boosterOutput)
            {
                testModel.Add(new MlModel(ft.ind,
                ft.p_formula,
                ft.p_dbe,
                ft.p_h2c,
                ft.p_hetero2c,
                ft.p_mzErrorRatio,
                ft.expfNoRatio,
                ft.expfIntRatio,
                ft.wf_DBE,
                ft.wl_DBE,
                ft.wfl_DBE,
                ft.wf_H2C,
                ft.wl_H2C,
                ft.wfl_H2C,
                ft.wf_Hetero2C,
                ft.wl_Hetero2C,
                ft.wfl_Hetero2C,
                ft.waf_mzErrorRatio,
                ft.ionmode,
                ft.fragno,
                ft.f_nonintDBENoRatio,
                ft.mztol,
                ft.fDBEsd,
                ft.lDBEsd,
                ft.flDBEsd,
                ft.fH2Csd,
                ft.lH2Csd,
                ft.flH2Csd,
                ft.fHetero2Csd,
                ft.lHetero2Csd,
                ft.flHetero2Csd,
                ft.ispsim,
                ft.relevance,
                ft.waf_logFreq,
                ft.wal_logFreq,
                ft.halogenAtomRatio,
                ft.cAtomRatio,
                ft.choAtomRatio,
                ft.chnoAtomRatio,
                ft.chnopsAtomRatio,
                ft.choOnly,
                ft.chnoOnly,
                ft.chnopsOnly,
                ft.atomNo2Mass));
            }


            if (highRes) // high-res
            {
                if (posIonMode) //positive
                {
                    if (MS1Available) // with MS1
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_pos_withMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile1.Path, 2);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 1;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 1, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                    else // no MS1
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_pos_noMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile2.Path, 1);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 2;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 2, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                }
                else //negative
                {
                    if (MS1Available)
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_neg_withMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile3.Path, 2);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 3;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 3, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                    else
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_highres_neg_noMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile4.Path, 1);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 4;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 4, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                }
            }
            else // low-res
            {
                if (posIonMode) //positive
                {
                    if (MS1Available)
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_pos_withMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile5.Path, 2);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 5;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 5, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                    else
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_pos_noMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile6.Path, 1);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 6;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 6, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;  // ranking
                        }
                    }
                }
                else //negative
                {
                    if (MS1Available)
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_neg_withMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile7.Path, 2);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 7;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 7, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                    else
                    {
                        //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/model_final_lowres_neg_noMS1.zip"));
                        List<Prediction> prediction = Test(testModel, modelFile8.Path, 1);
                        for (int k = 0; k < prediction.Count; k++)
                        {
                            boosterOutput[k].score = prediction[k].Score;
                            boosterOutput[k].modelType = 8;
                            double formLogFreq = 0;
                            if (boosterOutput[k].alignedFormula != null)
                            {
                                formLogFreq = boosterOutput[k].alignedFormula.LogFreq;
                            }
                            boosterOutput[k].plattProb = PlattScaling(prediction[k].Score, formLogFreq, 8, metaScoreBool);
                        }
                        boosterOutput = boosterOutput.OrderByDescending(o => o.plattProb).ToList();
                        for (int j = 0; j < boosterOutput.Count; j++)
                        {
                            boosterOutput[j].relevance = j + 1;
                        }
                    }
                }
            }
            return boosterOutput;

        }


        //model type 1: does not contain ms1 isotope similarity info
        //model type 2: contains ms1 isotope similarity info
        public List<Prediction> Test(List<MlModel> result, string modelpath, int modelType)
        {
            //Create MLContext
            MLContext mlContext = new MLContext(seed: 1);

            // Load testing Data
            IDataView testing = mlContext.Data.LoadFromEnumerable<MlModel>(result.ToArray());

            // Pipline for Testing
            // Map ind to numeric
            // Convert int to double
            // Define Data Prep Estimator
            // 1. Concatenate all columns into a single feature vector output to a new column called Features
            // 2. Normalize Features vector
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(new[] {
                    new  InputOutputColumnPair("GroupID", "ind")},
                keyOrdinality: Microsoft.ML.Transforms.ValueToKeyMappingEstimator
                    .KeyOrdinality.ByValue, addKeyValueAnnotationsAsText: true)
                .Append(mlContext.Transforms.Conversion.ConvertType(new[]
                {
                        new InputOutputColumnPair("dbl_p_dbe", "p_dbe"),
                        new InputOutputColumnPair("dbl_p_h2c", "p_h2c"),
                        new InputOutputColumnPair("dbl_p_hetero2c", "p_hetero2c"),
                        new InputOutputColumnPair("dbl_p_mzErrorRatio", "p_mzErrorRatio"),
                        new InputOutputColumnPair("dbl_expfNoRatio", "expfNoRatio"),
                        new InputOutputColumnPair("dbl_expfIntRatio", "expfIntRatio"),
                        new InputOutputColumnPair("dbl_wf_DBE", "wf_DBE"),
                        new InputOutputColumnPair("dbl_wl_DBE", "wl_DBE"),
                        new InputOutputColumnPair("dbl_wfl_DBE", "wfl_DBE"),
                        new InputOutputColumnPair("dbl_wf_H2C", "wf_H2C"),
                        new InputOutputColumnPair("dbl_wl_H2C", "wl_H2C"),
                        new InputOutputColumnPair("dbl_wfl_H2C", "wfl_H2C"),
                        new InputOutputColumnPair("dbl_wf_Hetero2C", "wf_Hetero2C"),
                        new InputOutputColumnPair("dbl_wl_Hetero2C", "wl_Hetero2C"),
                        new InputOutputColumnPair("dbl_wfl_Hetero2C", "wfl_Hetero2C"),
                        new InputOutputColumnPair("dbl_waf_mzErrorRatio", "waf_mzErrorRatio"),
                        //new InputOutputColumnPair("dbl_ionmode", "ionmode"),
                        //new InputOutputColumnPair("dbl_fragno", "fragno"),
                        new InputOutputColumnPair("dbl_f_nonintDBENoRatio", "f_nonintDBENoRatio"),
                        //new InputOutputColumnPair("dbl_mztol", "mztol"),
                        new InputOutputColumnPair("dbl_fDBEsd", "fDBEsd"),
                        new InputOutputColumnPair("dbl_lDBEsd", "lDBEsd"),
                        new InputOutputColumnPair("dbl_flDBEsd", "flDBEsd"),
                        new InputOutputColumnPair("dbl_fH2Csd", "fH2Csd"),
                        new InputOutputColumnPair("dbl_lH2Csd", "lH2Csd"),
                        new InputOutputColumnPair("dbl_flH2Csd", "flH2Csd"),
                        new InputOutputColumnPair("dbl_fHetero2Csd", "fHetero2Csd"),
                        new InputOutputColumnPair("dbl_lHetero2Csd", "lHetero2Csd"),
                        new InputOutputColumnPair("dbl_flHetero2Csd", "flHetero2Csd"),
                        new InputOutputColumnPair("Label", "Label_int"),
                        new InputOutputColumnPair("dbl_waf_logFreq", "waf_logFreq"),
                        new InputOutputColumnPair("dbl_wal_logFreq", "wal_logFreq"),
                        new InputOutputColumnPair("dbl_halogenAtomRatio", "halogenAtomRatio"),
                        new InputOutputColumnPair("dbl_cAtomRatio", "cAtomRatio"),
                        new InputOutputColumnPair("dbl_choAtomRatio", "choAtomRatio"),
                        new InputOutputColumnPair("dbl_chnoAtomRatio", "chnoAtomRatio"),
                        new InputOutputColumnPair("dbl_chnopsAtomRatio", "chnopsAtomRatio"),
                        new InputOutputColumnPair("dbl_choOnly", "choOnly"),
                        new InputOutputColumnPair("dbl_chnoOnly", "chnoOnly"),
                        new InputOutputColumnPair("dbl_chnopsOnly", "chnopsOnly"),
                        new InputOutputColumnPair("dbl_atomNo2Mass", "atomNo2Mass"),
                }, DataKind.Single))
                .Append(mlContext.Transforms.Concatenate("Features", "dbl_p_dbe", "dbl_p_h2c", "dbl_p_hetero2c", "dbl_p_mzErrorRatio", "dbl_expfNoRatio",
                "dbl_expfIntRatio", "dbl_wf_DBE", "dbl_wl_DBE", "dbl_wfl_DBE", "dbl_wf_H2C", "dbl_wl_H2C", "dbl_wfl_H2C", "dbl_wf_Hetero2C", "dbl_wl_Hetero2C",
                "dbl_wfl_Hetero2C", "dbl_waf_mzErrorRatio", "dbl_f_nonintDBENoRatio", "dbl_fDBEsd", "dbl_lDBEsd", "dbl_flDBEsd",
                "dbl_fH2Csd", "dbl_lH2Csd", "dbl_flH2Csd", "dbl_fHetero2Csd", "dbl_lHetero2Csd", "dbl_flHetero2Csd", "dbl_waf_logFreq", "dbl_wal_logFreq",
                "dbl_halogenAtomRatio", "dbl_cAtomRatio", "dbl_choAtomRatio", "dbl_chnoAtomRatio", "dbl_chnopsAtomRatio", "dbl_choOnly", "dbl_chnoOnly",
                "dbl_chnopsOnly", "dbl_atomNo2Mass"));

            if (modelType == 2)
            {
                pipeline = mlContext.Transforms.Conversion.MapValueToKey(new[] {
                    new  InputOutputColumnPair("GroupID", "ind")},
                keyOrdinality: Microsoft.ML.Transforms.ValueToKeyMappingEstimator
                    .KeyOrdinality.ByValue, addKeyValueAnnotationsAsText: true)
                .Append(mlContext.Transforms.Conversion.ConvertType(new[]
                {
                        new InputOutputColumnPair("dbl_p_dbe", "p_dbe"),
                        new InputOutputColumnPair("dbl_p_h2c", "p_h2c"),
                        new InputOutputColumnPair("dbl_p_hetero2c", "p_hetero2c"),
                        new InputOutputColumnPair("dbl_p_mzErrorRatio", "p_mzErrorRatio"),
                        new InputOutputColumnPair("dbl_expfNoRatio", "expfNoRatio"),
                        new InputOutputColumnPair("dbl_expfIntRatio", "expfIntRatio"),
                        new InputOutputColumnPair("dbl_wf_DBE", "wf_DBE"),
                        new InputOutputColumnPair("dbl_wl_DBE", "wl_DBE"),
                        new InputOutputColumnPair("dbl_wfl_DBE", "wfl_DBE"),
                        new InputOutputColumnPair("dbl_wf_H2C", "wf_H2C"),
                        new InputOutputColumnPair("dbl_wl_H2C", "wl_H2C"),
                        new InputOutputColumnPair("dbl_wfl_H2C", "wfl_H2C"),
                        new InputOutputColumnPair("dbl_wf_Hetero2C", "wf_Hetero2C"),
                        new InputOutputColumnPair("dbl_wl_Hetero2C", "wl_Hetero2C"),
                        new InputOutputColumnPair("dbl_wfl_Hetero2C", "wfl_Hetero2C"),
                        new InputOutputColumnPair("dbl_waf_mzErrorRatio", "waf_mzErrorRatio"),
                        //new InputOutputColumnPair("dbl_ionmode", "ionmode"),
                        //new InputOutputColumnPair("dbl_fragno", "fragno"),
                        new InputOutputColumnPair("dbl_f_nonintDBENoRatio", "f_nonintDBENoRatio"),
                        //new InputOutputColumnPair("dbl_mztol", "mztol"),
                        new InputOutputColumnPair("dbl_fDBEsd", "fDBEsd"),
                        new InputOutputColumnPair("dbl_lDBEsd", "lDBEsd"),
                        new InputOutputColumnPair("dbl_flDBEsd", "flDBEsd"),
                        new InputOutputColumnPair("dbl_fH2Csd", "fH2Csd"),
                        new InputOutputColumnPair("dbl_lH2Csd", "lH2Csd"),
                        new InputOutputColumnPair("dbl_flH2Csd", "flH2Csd"),
                        new InputOutputColumnPair("dbl_fHetero2Csd", "fHetero2Csd"),
                        new InputOutputColumnPair("dbl_lHetero2Csd", "lHetero2Csd"),
                        new InputOutputColumnPair("dbl_flHetero2Csd", "flHetero2Csd"),
                        new InputOutputColumnPair("dbl_ispsim", "ispsim"),
                        new InputOutputColumnPair("Label", "Label_int"),
                        new InputOutputColumnPair("dbl_waf_logFreq", "waf_logFreq"),
                        new InputOutputColumnPair("dbl_wal_logFreq", "wal_logFreq"),
                        new InputOutputColumnPair("dbl_halogenAtomRatio", "halogenAtomRatio"),
                        new InputOutputColumnPair("dbl_cAtomRatio", "cAtomRatio"),
                        new InputOutputColumnPair("dbl_choAtomRatio", "choAtomRatio"),
                        new InputOutputColumnPair("dbl_chnoAtomRatio", "chnoAtomRatio"),
                        new InputOutputColumnPair("dbl_chnopsAtomRatio", "chnopsAtomRatio"),
                        new InputOutputColumnPair("dbl_choOnly", "choOnly"),
                        new InputOutputColumnPair("dbl_chnoOnly", "chnoOnly"),
                        new InputOutputColumnPair("dbl_chnopsOnly", "chnopsOnly"),
                        new InputOutputColumnPair("dbl_atomNo2Mass", "atomNo2Mass"),
                }, DataKind.Single))
                .Append(mlContext.Transforms.Concatenate("Features", "dbl_p_dbe", "dbl_p_h2c", "dbl_p_hetero2c", "dbl_p_mzErrorRatio", "dbl_expfNoRatio",
                "dbl_expfIntRatio", "dbl_wf_DBE", "dbl_wl_DBE", "dbl_wfl_DBE", "dbl_wf_H2C", "dbl_wl_H2C", "dbl_wfl_H2C", "dbl_wf_Hetero2C", "dbl_wl_Hetero2C",
                "dbl_wfl_Hetero2C", "dbl_waf_mzErrorRatio", "dbl_f_nonintDBENoRatio", "dbl_fDBEsd", "dbl_lDBEsd", "dbl_flDBEsd",
                "dbl_fH2Csd", "dbl_lH2Csd", "dbl_flH2Csd", "dbl_fHetero2Csd", "dbl_lHetero2Csd", "dbl_flHetero2Csd", "dbl_ispsim", "dbl_waf_logFreq", "dbl_wal_logFreq",
                "dbl_halogenAtomRatio", "dbl_cAtomRatio", "dbl_choAtomRatio", "dbl_chnoAtomRatio", "dbl_chnopsAtomRatio", "dbl_choOnly", "dbl_chnoOnly",
                "dbl_chnopsOnly", "dbl_atomNo2Mass"));
            }

            // Fits the pipeline to the data.
            IDataView combTestingData = pipeline.Fit(testing).Transform(testing);

            //Define DataViewSchema for data preparation pipeline and trained model
            DataViewSchema modelSchema;

            // Load trained model
            ITransformer trainedModel = mlContext.Model.Load(modelpath, out modelSchema);

            // Run the model on test data set.
            IDataView transformedTestData = trainedModel.Transform(combTestingData);

            var newePipeline = mlContext.Transforms.Conversion.MapKeyToValue(new[] { new InputOutputColumnPair("GroupIDorig", "GroupID") });

            IDataView outTestData = newePipeline.Fit(transformedTestData).Transform(transformedTestData);

            // Take the top 5 rows.
            //IDataView topTransformedTestData = mlContext.Data.TakeRows(
            //    transformedTestData, 5);

            // Convert IDataView object to a list.
            List<Prediction> predictions = mlContext.Data.CreateEnumerable<Prediction>(
                outTestData, reuseRowObject: false).ToList();

            return predictions;
        }
        #endregion

        //Feature scaling, only applied to high-res MS data
        private List<Feature> FeatureScaling(bool posIonMode, List<Feature> thisMS2BoosterOutput, double preMzErrorSourceMedian, double waFragMzErrorSourceMedian)
        {
            double preMzErrorTargetMedian;
            double waFragMzErrorTargetMedian;
            double expFragNoTargetMedian;
            double expFragIntTargetMedian;

            if (posIonMode)
            {
                preMzErrorTargetMedian = 0.1395;
                waFragMzErrorTargetMedian = 0.2969;
                expFragNoTargetMedian = 0.98;
                expFragIntTargetMedian = 0.9841;
            }
            else
            {
                preMzErrorTargetMedian = 0.1158;
                waFragMzErrorTargetMedian = 0.2631;
                expFragNoTargetMedian = 0.99;
                expFragIntTargetMedian = 0.99;
            }

            double expFragNoFactor;
            double expFragIntFactor;

            // for candidates for this MS2, find max expFragNo / Int
            double maxExpFragNo = thisMS2BoosterOutput.Max(o => o.expfNoRatio);
            double maxExpFragInt = thisMS2BoosterOutput.Max(o => o.expfIntRatio);
            if (maxExpFragNo < expFragNoTargetMedian && maxExpFragNo > 0)
            {
                expFragNoFactor = Math.Pow(expFragNoTargetMedian / maxExpFragNo, 0.8);
            }
            else
            {
                expFragNoFactor = 1;
            }
            if (maxExpFragInt < expFragIntTargetMedian && maxExpFragInt > 0)
            {
                expFragIntFactor = Math.Pow(expFragIntTargetMedian / maxExpFragInt, 0.8);
            }
            else
            {
                expFragIntFactor = 1;
            }

            double preMzErrorFactor = 1;
            double waFragMzErrorFactor = 1;
            // factor = (target/source)^x
            if (preMzErrorSourceMedian > 0 && waFragMzErrorSourceMedian > 0)
            {
                preMzErrorFactor = Math.Pow(preMzErrorTargetMedian / preMzErrorSourceMedian, 0.5);
                waFragMzErrorFactor = Math.Pow(waFragMzErrorTargetMedian / waFragMzErrorSourceMedian, 0.5);
            }


            //expFragNoFactor = Math.Pow(expFragNoTargetMedian / expFragNoSourceMedian, 0.25);
            //expFragIntFactor = Math.Pow(expFragIntTargetMedian / expFragIntSourceMedian, 0.25);

            for (int i = 0; i < thisMS2BoosterOutput.Count; i++)
            {
                double newPreMzErrorRatio = thisMS2BoosterOutput[i].p_mzErrorRatio * preMzErrorFactor;
                double newWaFragMzErrorRatio = thisMS2BoosterOutput[i].waf_mzErrorRatio * waFragMzErrorFactor;
                double newExpFragNo = thisMS2BoosterOutput[i].expfNoRatio * expFragNoFactor;
                double newExpFragInt = thisMS2BoosterOutput[i].expfIntRatio * expFragIntFactor;

                if (newPreMzErrorRatio >= 1)
                {
                    newPreMzErrorRatio = 0.99;
                }
                if (newWaFragMzErrorRatio >= 1)
                {
                    newWaFragMzErrorRatio = 0.99;
                }
                if (newExpFragNo > 1)
                {
                    newExpFragNo = 1;
                }
                if (newExpFragInt > 1)
                {
                    newExpFragInt = 1;
                }

                thisMS2BoosterOutput[i].p_mzErrorRatio = newPreMzErrorRatio;
                thisMS2BoosterOutput[i].waf_mzErrorRatio = newWaFragMzErrorRatio;
                thisMS2BoosterOutput[i].expfNoRatio = newExpFragNo;
                thisMS2BoosterOutput[i].expfIntRatio = newExpFragInt;
            }

            return thisMS2BoosterOutput;
        }
        // MS2-based expFragNo/Int correction
        private List<Feature> ExpFragCorrection(List<Feature> thisMS2BoosterOutput, bool highResData ,bool posIonMode)
        {
            if(thisMS2BoosterOutput ==null || thisMS2BoosterOutput.Count == 0)
            {
                return thisMS2BoosterOutput;
            }

            double expFragNoTargetMedian;
            double expFragIntTargetMedian;
            if (highResData)
            {
                if (posIonMode)
                {
                    expFragNoTargetMedian = 0.98;
                    expFragIntTargetMedian = 0.9841;
                }
                else
                {
                    expFragNoTargetMedian = 0.99;
                    expFragIntTargetMedian = 0.99;
                }
            }
            else
            {
                expFragNoTargetMedian = 0.99;
                expFragIntTargetMedian = 0.99;
            }

            double expFragNoFactor;
            double expFragIntFactor;

            // for candidates for this MS2, find max expFragNo / Int
            double maxExpFragNo = thisMS2BoosterOutput.Max(o => o.expfNoRatio);
            double maxExpFragInt = thisMS2BoosterOutput.Max(o => o.expfIntRatio);
            if (maxExpFragNo < expFragNoTargetMedian && maxExpFragNo > 0)
            {
                expFragNoFactor = Math.Pow(expFragNoTargetMedian / maxExpFragNo, 0.8);
            }
            else
            {
                expFragNoFactor = 1;
            }
            if (maxExpFragInt < expFragIntTargetMedian && maxExpFragInt > 0)
            {
                expFragIntFactor = Math.Pow(expFragIntTargetMedian / maxExpFragInt, 0.8);
            }
            else
            {
                expFragIntFactor = 1;
            }
            for (int i = 0; i < thisMS2BoosterOutput.Count; i++)
            {
                double newExpFragNo = thisMS2BoosterOutput[i].expfNoRatio * expFragNoFactor;
                double newExpFragInt = thisMS2BoosterOutput[i].expfIntRatio * expFragIntFactor;

                if (newExpFragNo > 1)
                {
                    newExpFragNo = 1;
                }
                if (newExpFragInt > 1)
                {
                    newExpFragInt = 1;
                }

                thisMS2BoosterOutput[i].expfNoRatio = newExpFragNo;
                thisMS2BoosterOutput[i].expfIntRatio = newExpFragInt;
            }
            return thisMS2BoosterOutput;
        }


        //When user clicks on a row in the ms2 table...
        //If there are candidate formulas, display information for the highest ranked candidate
        private void ms2grid_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            updateMS2TableAfterCalculation();
        }
        // function for updating MS2 fragment table after calculation
        private void updateMS2TableAfterCalculation()
        {
            //get data from ms2 table 
            var ms2model = (Ms2Model)ms2grid.DataContext;

            bool MS2FragReannotate = (bool)localSettings.Values["MS2FragReannotate_include"];
            bool ppm = (bool)localSettings.Values["ms1tol_ppmON"];
            double ms2tol = (double)localSettings.Values["ms2tol"];

            Ms2Utility item = (Ms2Utility)ms2grid.CurrentItem;

            if (item.SeedMetabolite)
            {
                ms2LibSearch_tabView.IsSelected = true;
                ms2ExplainSummary_tabView.IsSelected = false;
                featureConnection_tabView.IsSelected = false;
            }
            else if (item.featureConnections != null && item.featureConnections.Count > 0)
            {
                ms2LibSearch_tabView.IsSelected = false;
                ms2ExplainSummary_tabView.IsSelected = false;
                featureConnection_tabView.IsSelected = true;
            }
            else
            {
                ms2LibSearch_tabView.IsSelected = false;
                ms2ExplainSummary_tabView.IsSelected = true;
                featureConnection_tabView.IsSelected = false;
            }

            //bool ms2deisotope_mainpage = (bool)localSettings.Values["ms2Deisotope"];
            //bool ms2denoise_mainpage = (bool)localSettings.Values["ms2Denoise"];
            if (item != null)
            {
                if (item.MergedIndex != null)
                {
                    if (item.MergedIndex.Count > 1) // for mzML input
                    {
                        mergedMS2DisplayPanel.Visibility = Visibility.Visible;
                        mergedMS2IndexDisplay.Text = string.Join(", ", item.MergedIndex.Select(o => o.ToString()).ToArray());
                    }
                    else
                    {
                        mergedMS2DisplayPanel.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    mergedMS2DisplayPanel.Visibility = Visibility.Collapsed;
                }

                metaIdenName.Text = "";
                metaIdenFormula.Text = "";
                metaIdenInchikey.Text = "";
                metaIdenImage.Visibility = Visibility.Collapsed;
                metaIdenPubChemCID.Text = "";
                metaIdenDescription.Text = "";
                metaIdenDBNo.Text = "";
                metaIdenCollisionEnergy.Text = "";
                metaIdenComments.Text = "";
                metaIdenRT.Text = "";
                ms2LibSearch_ms2Score.Text = "";
                ms2LibSearch_ms2matchedFragNo.Text = "";
                //ms2LibSearch_matchingAlgorithm.Text = "";
                MS2LibSearch_ComparePlot.Visibility = Visibility.Collapsed;

                // update pubchem image, description if seed metabolite
                if (item.SeedMetabolite) // update structural info
                {
                    seedMetaboliteDisplay.Visibility = Visibility.Visible;

                    metaIdenName.Text = item.MetaboliteName;
                    metaIdenFormula.Text = item.Formula_PC;
                    metaIdenInchikey.Text = item.InChiKey;
                    metaIdenImage.Visibility = Visibility.Visible;
                    if (!item.pubchemAccessedBefore)
                    {
                        item.pubchemAccessedBefore = true;
                        try
                        {
                            // CID
                            XmlTextReader CIDreader = new XmlTextReader("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/" + item.MetaboliteName + "/cids/XML");
                            string CIDstring = "";
                            while (CIDreader.Read())
                            {
                                if (CIDreader.NodeType == XmlNodeType.Element && CIDreader.Name == "CID")
                                {
                                    CIDreader.Read();
                                    CIDstring = CIDreader.Value;
                                    break;
                                }
                            }
                            metaIdenPubChemCID.Text = CIDstring;
                            item.pubchemCID = CIDstring;
                        }
                        catch
                        {
                            item.pubchemCID = "";
                        }
                        if (item.pubchemCID == "")
                        {
                            try
                            {
                                // CID
                                XmlTextReader CIDreader = new XmlTextReader("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/inchikey/" + item.inchikey + "/cids/XML");
                                string CIDstring = "";
                                while (CIDreader.Read())
                                {
                                    if (CIDreader.NodeType == XmlNodeType.Element && CIDreader.Name == "CID")
                                    {
                                        CIDreader.Read();
                                        CIDstring = CIDreader.Value;
                                        break;
                                    }
                                }
                                metaIdenPubChemCID.Text = CIDstring;
                                item.pubchemCID = CIDstring;
                            }
                            catch
                            {
                                item.pubchemCID = "";
                            }
                        }
                        if (item.pubchemCID == "") // use inchikey first half
                        {
                            try
                            {
                                XmlTextReader CIDreader2 = new XmlTextReader("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/inchikey/" + item.inchikeyFirstHalf + "/cids/XML");
                                string CIDstring = "";
                                while (CIDreader2.Read())
                                {
                                    if (CIDreader2.NodeType == XmlNodeType.Element && CIDreader2.Name == "CID")
                                    {
                                        CIDreader2.Read();
                                        CIDstring = CIDreader2.Value;
                                        break;
                                    }
                                }
                                metaIdenPubChemCID.Text = CIDstring;
                                item.pubchemCID = CIDstring;
                            }
                            catch
                            {
                                item.pubchemCID = "";
                            }
                        }
                        if (item.pubchemCID != "")
                        {
                            try
                            {
                                string imageUri = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/" + item.pubchemCID + "/PNG";

                                BitmapImage cmpdImage = new BitmapImage(new Uri(imageUri));
                                metaIdenImage.Source = cmpdImage;
                                item.pubchemImage = cmpdImage;
                            }
                            catch
                            {
                                metaIdenImage.Visibility = Visibility.Collapsed;
                                item.pubchemImage = new BitmapImage();
                            }
                            try
                            {
                                string descriptionUri = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/" + item.pubchemCID + "/description/XML";
                                XmlTextReader DescriptionReader = new XmlTextReader(descriptionUri);
                                string Descriptionstring = "";
                                while (DescriptionReader.Read())
                                {
                                    if (DescriptionReader.NodeType == XmlNodeType.Element && DescriptionReader.Name == "Description")
                                    {
                                        DescriptionReader.Read();
                                        Descriptionstring += DescriptionReader.Value;
                                        continue;
                                    }
                                    if (DescriptionReader.NodeType == XmlNodeType.Element && DescriptionReader.Name == "DescriptionSourceName")
                                    {
                                        DescriptionReader.Read();
                                        Descriptionstring += " (source from ";
                                        Descriptionstring += DescriptionReader.Value;
                                        Descriptionstring += ": ";
                                        continue;
                                    }
                                    if (DescriptionReader.NodeType == XmlNodeType.Element && DescriptionReader.Name == "DescriptionURL")
                                    {
                                        DescriptionReader.Read();
                                        Descriptionstring += DescriptionReader.Value;
                                        Descriptionstring += ")\n\n";
                                        continue;
                                    }
                                }
                                metaIdenDescription.Text = Descriptionstring;
                                item.pubchemDescription = Descriptionstring;
                            }
                            catch
                            {
                                item.pubchemDescription = "";
                            }
                        }
                        else
                        {
                            item.pubchemImage = new BitmapImage();
                            item.pubchemDescription = "";
                        }
                    }
                    else
                    {
                        metaIdenImage.Source = item.pubchemImage;
                        metaIdenPubChemCID.Text = item.pubchemCID;
                        metaIdenDescription.Text = item.pubchemDescription;
                    }

                    if (item.Ms2Matching != null && item.Ms2Matching.ms2MatchingReturns.Count > 0) // seed metabolites generated from ms2 searching
                    {
                        MS2DBEntry mS2DBEntry = item.Ms2Matching.ms2MatchingReturns[0].matchedDBEntry;
                        if (mS2DBEntry.DBNumberString != null)
                        {
                            metaIdenDBNo.Text = mS2DBEntry.DBNumberString;
                        }
                        if (mS2DBEntry.CollisionEnergy != null)
                        {
                            metaIdenCollisionEnergy.Text = mS2DBEntry.CollisionEnergy;
                        }
                        if (mS2DBEntry.Comments != null)
                        {
                            metaIdenComments.Text = mS2DBEntry.Comments;
                        }
                        if (item.Ms2Matching.ms2MatchingReturns[0].matchedDBEntry.ValidRT)
                        {
                            metaIdenRT.Text = item.Ms2Matching.ms2MatchingReturns[0].matchedDBEntry.RTminute.ToString();
                        }
                        ms2LibSearch_ms2Score.Text = Math.Round(item.Ms2Matching.ms2MatchingReturns[0].ms2CompareResult.Score, 2).ToString();
                        ms2LibSearch_ms2matchedFragNo.Text = item.Ms2Matching.ms2MatchingReturns[0].ms2CompareResult.MatchNumber.ToString();
                        //switch ((int)localSettings.Values["ms2MatchingAlgorithm"])
                        //{
                        //    case 0:
                        //        ms2LibSearch_matchingAlgorithm.Text = "Dot product";
                        //        break;
                        //    case 1:
                        //        ms2LibSearch_matchingAlgorithm.Text = "Reverse dot product";
                        //        break;
                        //    case 2:
                        //        ms2LibSearch_matchingAlgorithm.Text = "Spectral entropy similarity";
                        //        break;
                        //    default:
                        //        break;
                        //}
                        PlotMS2LibSearchComparison(item);
                    }
                }
                else
                {
                    seedMetaboliteDisplay.Visibility = Visibility.Collapsed;
                }

                //feature connection
                var connections = (FeatureConnectionModel)featureConnectionGrid.DataContext; //candidates datagrid
                connections.Connections.Clear();

                if (item.Formula_PC != null)
                {
                    featureConnectionThisFormula.Text = item.Formula_PC;
                }
                featureConnectionThisRT.Text = item.Rt.ToString();
                //feature connection
                if (item.FeatureConnections != null && item.FeatureConnections.Count > 0)
                {
                    foreach (var connection in item.FeatureConnections)
                    {
                        string pairedMetabolite = "";
                        string pairedFormula = "";
                        var pairedMS2 = ms2model.MS2s.First(o => o.fileindex == item.fileindex && o.ScanNumber == connection.pairedMs2ScanNumber);
                        pairedFormula = pairedMS2.Formula_PC;
                        if (pairedMS2 != null && pairedMS2.seedMetabolite)
                        {
                            pairedMetabolite = pairedMS2.metaboliteName;
                        }
                        connections.Connections.Add(new FeatureConnectionUtility() {
                            pairedMs2ScanNumber = connection.pairedMs2ScanNumber,
                            pairedMs2MetaboliteName = pairedMetabolite,
                            pairedMs2Formula = pairedFormula,
                            pairedMs2RT = Math.Round(pairedMS2.Rt, 2),
                            ms2Similarity = connection.ms2Similarity == 0? double.NaN : Math.Round(connection.ms2Similarity, 3),
                            connectionType = connection.connection.ConnectionType,
                            formulaChange = connection.connection.FormulaChange,
                            massDiff = Math.Round(connection.connection.MassDiff, 4),
                            description = connection.connection.Description});
                    }
                    //featureConnectionGrid.SelectedIndex = 0; // select the first
                }


                //candidates
                var candidates = (CandidateModel)candidateFormulaGrid.DataContext; //candidates datagrid
                candidates.Candidates.Clear();


                // calculate the explained fragment Number & explained fragment Int (considering isotope, noise, precursor)
                int expFragNoToAdd = 0;
                int sumMS2No = 0;
                double expFragIntToAdd = 0.0;
                double sumMS2Int = 0.0;
                if (item.OriSpectrum != null)
                {
                    sumMS2No = item.OriSpectrum.Count;
                    sumMS2Int = item.OriSpectrum.Sum(o => o.Intensity);
                }
                if (item.NoiseFragments != null)
                {
                    expFragNoToAdd += item.NoiseFragments.Count;
                    //sumMS2No += item.NoiseFragments.Count;
                    double noiseSumInt = item.NoiseFragments.Sum(o => o.Intensity);
                    expFragIntToAdd += noiseSumInt;
                    //sumMS2Int += noiseSumInt;
                }
                if (item.IsotopeFragments != null)
                {
                    expFragNoToAdd += item.IsotopeFragments.Count;
                    //sumMS2No += item.IsotopeFragments.Count;
                    double isotopeSumInt = item.IsotopeFragments.Sum(o => o.Intensity);
                    expFragIntToAdd += isotopeSumInt;
                    //sumMS2Int += isotopeSumInt;
                }
                if (item.PrecursorFragments != null)
                {
                    expFragNoToAdd += item.PrecursorFragments.Count;
                    //sumMS2No += item.PrecursorFragments.Count;
                    double precursorSumInt = item.PrecursorFragments.Sum(o => o.Intensity);
                    expFragIntToAdd += precursorSumInt;
                    //sumMS2Int += precursorSumInt;
                }
                //if (item.Spectrum != null)
                //{
                //    sumMS2No += item.Spectrum.Count;
                //    sumMS2Int += item.Spectrum.Sum(o => o.Intensity);
                //}

                if (item.Candidates != null && item.Candidates.Count > 0) //1+ candidates
                {
                    // add candidates to datagrid
                    // generate candidateUtility

                    double sumErrorProbability = 0.0;
                    //double sumScore = item.Candidates.Sum(o => Math.Exp(o.score));
                    //double sumScore = item.Candidates.Sum(o => PlattScaling(o.score, item.Candidates[0].modelType));
                    int candidateCount = 1;
                    foreach (Feature ft in item.Candidates)
                    {
                        double expMS2Int = 0.0;
                        int expMS2No = 0;
                        List<CFDF> candidateCFDF = ft.cfdf;

                        double PlattProbability = ft.plattProb; // calculate Platt calibrated probability
                        //double PlattProbability = PlattScaling(Math.Exp(ft.score) / sumScore, ft.modelType); // calculate Platt calibrated probability

                        sumErrorProbability += 1 - PlattProbability;

                        if (candidateCFDF != null && candidateCFDF.Count > 0) // loop over all the explained fragments in a candidate
                        {
                            for (int z = 0; z < candidateCFDF.Count; z++)
                            {
                                if (candidateCFDF[z].expfragindex != -1)
                                {
                                    expMS2Int += item.Spectrum[candidateCFDF[z].expfragindex].Intensity;
                                    expMS2No++;
                                }

                            }
                        }
                        expMS2Int += expFragIntToAdd;
                        expMS2No += expFragNoToAdd;
                        candidates.Candidates.Add(new CandidateUtility(ft.relevance, ft.p_formula, Math.Round(1e6 * ft.p_mzErrorRatio * ft.mztol / item.Mz, 2), Math.Round((double)expMS2No / sumMS2No, 2),
                            Math.Round(expMS2Int / sumMS2Int, 2), Math.Round(ft.waf_mzErrorRatio, 2), Math.Round(PlattProbability, 3), Math.Round(sumErrorProbability / candidateCount, 3),
                            ft.ispsim, ft.theoMS1, ft.cfdf, ft.alignedFormula));
                        candidateCount++;
                    }

                    candidateFormulaGrid.SelectedIndex = 0; // select the first candidate

                    //histograms
                    List<CFDF> histogramCFDF = item.Candidates[0].cfdf;

                    List<CFDF> histogramH2C_CFDF = item.Candidates[0].cfdf.Where(o =>
                        o.h2c_f < 888888 && o.h2c_f > -888888 &&
                        o.h2c_l < 888888 && o.h2c_l > -888888).ToList();

                    List<CFDF> histogramHetero2C_CFDF = item.Candidates[0].cfdf.Where(o =>
                        o.hetero2c_f < 888888 && o.hetero2c_f > -888888 &&
                        o.hetero2c_l < 888888 && o.hetero2c_l > -888888).ToList();


                    if ((histogramH2C_CFDF.Count + histogramHetero2C_CFDF.Count) > 0 && histogramCFDF[0].expfragindex != -1)
                    {
                        fragDBEhistogram.ItemsSource = histogramCFDF;
                        double fragDBEhistogramInterval = (histogramCFDF.Max(o => o.dbe_f) - histogramCFDF.Min(o => o.dbe_f)) / 6;
                        if (fragDBEhistogramInterval != 0.0)
                        {
                            fragDBEhistogram.HistogramInterval = fragDBEhistogramInterval;
                        }
                        else
                        {
                            fragDBEhistogram.HistogramInterval = 1;
                        }

                        nlDBEhistogram.ItemsSource = histogramCFDF;
                        double nlDBEhistogramInterval = (histogramCFDF.Max(o => o.dbe_l) - histogramCFDF.Min(o => o.dbe_l)) / 6;
                        if (nlDBEhistogramInterval != 0.0)
                        {
                            nlDBEhistogram.HistogramInterval = nlDBEhistogramInterval;
                        }
                        else
                        {
                            nlDBEhistogram.HistogramInterval = 1;
                        }

                        if (histogramH2C_CFDF.Count > 0)
                        {
                            fragH2Chistogram.ItemsSource = histogramH2C_CFDF;
                            double fragH2ChistogramInterval = (histogramH2C_CFDF.Max(o => o.h2c_f) - histogramH2C_CFDF.Min(o => o.h2c_f)) / 6;
                            if (fragH2ChistogramInterval != 0.0)
                            {
                                fragH2Chistogram.HistogramInterval = fragH2ChistogramInterval;
                            }
                            else
                            {
                                fragH2Chistogram.HistogramInterval = 0.2;
                            }

                            nlH2Chistogram.ItemsSource = histogramH2C_CFDF;
                            double nlH2ChistogramInterval = (histogramH2C_CFDF.Max(o => o.h2c_l) - histogramH2C_CFDF.Min(o => o.h2c_l)) / 6;
                            if (nlH2ChistogramInterval != 0.0)
                            {
                                nlH2Chistogram.HistogramInterval = nlH2ChistogramInterval;
                            }
                            else
                            {
                                nlH2Chistogram.HistogramInterval = 0.2;
                            }
                        }

                        if (histogramHetero2C_CFDF.Count > 0)
                        {
                            fragHetero2Chistogram.ItemsSource = histogramHetero2C_CFDF;
                            double fragHetero2ChistogramInterval = (histogramHetero2C_CFDF.Max(o => o.hetero2c_f) - histogramHetero2C_CFDF.Min(o => o.hetero2c_f)) / 6;
                            if (fragHetero2ChistogramInterval != 0.0)
                            {
                                fragHetero2Chistogram.HistogramInterval = fragHetero2ChistogramInterval;
                            }
                            else
                            {
                                fragHetero2Chistogram.HistogramInterval = 0.2;
                            }


                            nlHetero2Chistogram.ItemsSource = histogramHetero2C_CFDF;
                            double nlHetero2ChistogramInterval = (histogramHetero2C_CFDF.Max(o => o.hetero2c_l) - histogramHetero2C_CFDF.Min(o => o.hetero2c_l)) / 6;
                            if (nlHetero2ChistogramInterval != 0.0)
                            {
                                nlHetero2Chistogram.HistogramInterval = nlHetero2ChistogramInterval;
                            }
                            else
                            {
                                nlHetero2Chistogram.HistogramInterval = 0.2;
                            }
                        }

                        //radical ion ratio
                        ObservableCollection<RadicalIonRatioUtility> radicals = new ObservableCollection<RadicalIonRatioUtility>();
                        radicals.Add(new RadicalIonRatioUtility("Cnt.", histogramCFDF.Where(o => (o.dbe_f % 1) == 0).Count(), histogramCFDF.Where(o => (o.dbe_f % 1) != 0).Count()));

                        double radInt = 0.0;
                        double nonRadInt = 0.0;
                        for (int w = 0; w < histogramCFDF.Count; w++)
                        {
                            if (histogramCFDF[w].dbe_f % 1 == 0)
                            {
                                radInt += item.Spectrum[histogramCFDF[w].expfragindex].Intensity;
                            }
                            else
                            {
                                nonRadInt += item.Spectrum[histogramCFDF[w].expfragindex].Intensity;
                            }
                        }
                        radicals.Add(new RadicalIonRatioUtility("Int.", radInt, nonRadInt));

                        radicalIonPlot.ItemsSource = radicals;
                        nonRadicalIonPlot.ItemsSource = radicals;
                        radicalIonCt_box.Text = Math.Round(radicals[0].radical / (radicals[0].radical + radicals[0].nonradical) * 100, 0).ToString() + "%";
                        nonradicalIonCt_box.Text = Math.Round(radicals[0].nonradical / (radicals[0].radical + radicals[0].nonradical) * 100, 0).ToString() + "%";
                        radicalIonInt_box.Text = Math.Round(radicals[1].radical / (radicals[1].radical + radicals[1].nonradical) * 100, 0).ToString() + "%";
                        nonradicalIonInt_box.Text = Math.Round(radicals[1].nonradical / (radicals[1].radical + radicals[1].nonradical) * 100, 0).ToString() + "%";
                    }
                    else
                    {
                        fragDBEhistogram.ItemsSource = new List<CFDF>();
                        nlDBEhistogram.ItemsSource = new List<CFDF>(); ;
                        fragH2Chistogram.ItemsSource = new List<CFDF>(); ;
                        nlH2Chistogram.ItemsSource = new List<CFDF>(); ;
                        fragHetero2Chistogram.ItemsSource = new List<CFDF>(); ;
                        nlHetero2Chistogram.ItemsSource = new List<CFDF>(); ;
                        radicalIonPlot.ItemsSource = new ObservableCollection<RadicalIonRatioUtility>();
                        nonRadicalIonPlot.ItemsSource = new ObservableCollection<RadicalIonRatioUtility>();
                        radicalIonCt_box.Text = "";
                        nonradicalIonCt_box.Text = "";
                        radicalIonInt_box.Text = "";
                        nonradicalIonInt_box.Text = "";
                    }
                }
                else
                {
                    if (item.SeedMetabolite)
                    {
                        candidates.Candidates.Add(new CandidateUtility(1, item.Formula_PC, double.NaN, double.NaN, double.NaN, double.NaN, 1, 0, double.NaN, null, null, null));
                    }
                    fragDBEhistogram.ItemsSource = new List<CFDF>();
                    nlDBEhistogram.ItemsSource = new List<CFDF>(); ;
                    fragH2Chistogram.ItemsSource = new List<CFDF>(); ;
                    nlH2Chistogram.ItemsSource = new List<CFDF>(); ;
                    fragHetero2Chistogram.ItemsSource = new List<CFDF>(); ;
                    nlHetero2Chistogram.ItemsSource = new List<CFDF>(); ;
                    radicalIonPlot.ItemsSource = new ObservableCollection<RadicalIonRatioUtility>();
                    nonRadicalIonPlot.ItemsSource = new ObservableCollection<RadicalIonRatioUtility>();
                    radicalIonCt_box.Text = "";
                    nonradicalIonCt_box.Text = "";
                    radicalIonInt_box.Text = "";
                    nonradicalIonInt_box.Text = "";
                }

                //database list
                candidateDB_list.Clear();
                if (item.Candidates != null && item.Candidates.Count > 0)
                {
                    if (item.Candidates[0].alignedFormula != null)
                    {
                        #region
                        if (item.Candidates[0].alignedFormula.PubChem > 0)
                        {
                            candidateDB_list.Add(new Database("PubChem_include"));
                        }
                        if (item.Candidates[0].alignedFormula.ANPDB > 0)
                        {
                            candidateDB_list.Add(new Database("ANPDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.BLEXP > 0)
                        {
                            candidateDB_list.Add(new Database("BLEXP_include"));
                        }
                        if (item.Candidates[0].alignedFormula.BMDB > 0)
                        {
                            candidateDB_list.Add(new Database("BMDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.ChEBI > 0)
                        {
                            candidateDB_list.Add(new Database("ChEBI_include"));
                        }
                        if (item.Candidates[0].alignedFormula.COCONUT > 0)
                        {
                            candidateDB_list.Add(new Database("COCONUT_include"));
                        }
                        if (item.Candidates[0].alignedFormula.DrugBank > 0)
                        {
                            candidateDB_list.Add(new Database("DrugBank_include"));
                        }
                        if (item.Candidates[0].alignedFormula.DSSTOX > 0)
                        {
                            candidateDB_list.Add(new Database("DSSTOX_include"));
                        }
                        if (item.Candidates[0].alignedFormula.ECMDB > 0)
                        {
                            candidateDB_list.Add(new Database("ECMDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.FooDB > 0)
                        {
                            candidateDB_list.Add(new Database("FooDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.HMDB > 0)
                        {
                            candidateDB_list.Add(new Database("HMDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.HSDB > 0)
                        {
                            candidateDB_list.Add(new Database("HSDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.KEGG > 0)
                        {
                            candidateDB_list.Add(new Database("KEGG_include"));
                        }
                        if (item.Candidates[0].alignedFormula.LMSD > 0)
                        {
                            candidateDB_list.Add(new Database("LMSD_include"));
                        }
                        if (item.Candidates[0].alignedFormula.MaConDa > 0)
                        {
                            candidateDB_list.Add(new Database("MaConDa_include"));
                        }
                        if (item.Candidates[0].alignedFormula.MarkerDB > 0)
                        {
                            candidateDB_list.Add(new Database("MarkerDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.MCDB > 0)
                        {
                            candidateDB_list.Add(new Database("MCDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.NORMAN > 0)
                        {
                            candidateDB_list.Add(new Database("NORMAN_include"));
                        }
                        if (item.Candidates[0].alignedFormula.NPASS > 0)
                        {
                            candidateDB_list.Add(new Database("NPASS_include"));
                        }
                        if (item.Candidates[0].alignedFormula.Plantcyc > 0)
                        {
                            candidateDB_list.Add(new Database("Plantcyc_include"));
                        }
                        if (item.Candidates[0].alignedFormula.SMPDB > 0)
                        {
                            candidateDB_list.Add(new Database("SMPDB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.STOFF_IDENT > 0)
                        {
                            candidateDB_list.Add(new Database("STF_IDENT_include"));
                        }
                        if (item.Candidates[0].alignedFormula.T3DB > 0)
                        {
                            candidateDB_list.Add(new Database("T3DB_include"));
                        }
                        if (item.Candidates[0].alignedFormula.TTD > 0)
                        {
                            candidateDB_list.Add(new Database("TTD_include"));
                        }
                        if (item.Candidates[0].alignedFormula.UNPD > 0)
                        {
                            candidateDB_list.Add(new Database("UNPD_include"));
                        }
                        if (item.Candidates[0].alignedFormula.YMDB > 0)
                        {
                            candidateDB_list.Add(new Database("YMDB_include"));
                        }
                        #endregion
                    }
                }

                //MS1 Comparison Plot
                if ((item.Ms1 != null && item.Ms1.Count > 0) || (item.Candidates != null && item.Candidates.Count > 0)) //either has exp. ms1 or 1+ theoretical candidates
                {
                    //initiate ms1 spectrum comparison model
                    var ms1model = new PlotModel { Title = "" };
                    var ms1scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

                    //experimental ms1 available
                    if (item.Ms1 != null && item.Ms1.Count > 0)
                    {
                        double expMaxInt = item.Ms1.Max(o => o.Intensity); //max intensity for normalization
                        //add data points
                        for (int i = 0; i < item.Ms1.Count; i++)
                        {
                            var x = item.Ms1[i].Mz;
                            var y = item.Ms1[i].Intensity / expMaxInt * 100;
                            var size = 2;
                            var colorValue = 1;
                            ms1scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));
                            var annotation = new LineAnnotation();
                            annotation.Color = OxyColors.Orange;
                            annotation.MinimumY = 0;
                            annotation.MaximumY = item.Ms1[i].Intensity / expMaxInt * 100;
                            annotation.X = item.Ms1[i].Mz;
                            annotation.LineStyle = LineStyle.Solid;
                            annotation.Type = LineAnnotationType.Vertical;
                            annotation.StrokeThickness = 1.5;
                            ms1model.Annotations.Add(annotation);
                        }
                    }

                    //1+ candidates, use first candidate theoretical ms1
                    if (item.Candidates != null && item.Candidates.Count > 0)
                    {
                        IsotopePattern iPattern = new IsotopePattern();
                        //theoMS1 already calculated in booster because ms2 has experimental ms1
                        if (item.Candidates[0].theoMS1 != null)
                        {
                            iPattern = item.Candidates[0].theoMS1;
                        }
                        //theoMS1 not precalculated
                        else
                        {
                            string new_formula = "";
                            IDictionary<string, int> v = new Dictionary<string, int>();
                            v.Add("C", item.Candidates[0].cfdf[0].c + item.adduct.SumFormula.c); //adding a key/value using the Add() method
                            v.Add("H", item.Candidates[0].cfdf[0].h + item.adduct.SumFormula.h);
                            v.Add("B", item.Candidates[0].cfdf[0].b + item.adduct.SumFormula.b);
                            v.Add("Br", item.Candidates[0].cfdf[0].br + item.adduct.SumFormula.br);
                            v.Add("Cl", item.Candidates[0].cfdf[0].cl + item.adduct.SumFormula.cl);
                            v.Add("F", item.Candidates[0].cfdf[0].f + item.adduct.SumFormula.f);
                            v.Add("I", item.Candidates[0].cfdf[0].i + item.adduct.SumFormula.i);
                            v.Add("K", item.Candidates[0].cfdf[0].k + item.adduct.SumFormula.k);
                            v.Add("N", item.Candidates[0].cfdf[0].n + item.adduct.SumFormula.n);
                            v.Add("Na", item.Candidates[0].cfdf[0].na + item.adduct.SumFormula.na);
                            v.Add("O", item.Candidates[0].cfdf[0].o + item.adduct.SumFormula.o);
                            v.Add("P", item.Candidates[0].cfdf[0].p + item.adduct.SumFormula.p);
                            v.Add("S", item.Candidates[0].cfdf[0].s + item.adduct.SumFormula.s);
                            v.Add("Se", item.Candidates[0].cfdf[0].se + item.adduct.SumFormula.se);
                            v.Add("Si", item.Candidates[0].cfdf[0].si + item.adduct.SumFormula.si);
                            foreach (KeyValuePair<string, int> element in v)
                            {
                                if (element.Value > 1)
                                {
                                    new_formula = new_formula + element.Key + element.Value.ToString();
                                }
                                if (element.Value == 1)
                                {
                                    new_formula = new_formula + element.Key;
                                }
                            }

                            IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(new_formula);
                            IsotopePatternGenerator IPG = new IsotopePatternGenerator((double)localSettings.Values["isotopic_abundance_cutoff"] / 100);
                            iPattern = IPG.GetIsotopes(mf);
                        }

                        double simMaxInt = iPattern.Isotopes.Max(o => o.Intensity); //find max intensity for normalization
                        int ionModeFactor = 1;
                        if(item.Polarity == "N")
                        {
                            ionModeFactor = -1;
                        }
                        //add datapoints
                        for (int i = 0; i < iPattern.Isotopes.Count; i++)
                        {
                            var x = iPattern.Isotopes[i].Mass - 0.0005485 * ionModeFactor;
                            var y = iPattern.Isotopes[i].Intensity / simMaxInt * -100;
                            var size = 2;
                            var colorValue = 1;
                            ms1scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));
                            var annotation = new LineAnnotation();
                            annotation.Color = OxyColors.DarkBlue;
                            annotation.MaximumY = 0;
                            annotation.MinimumY = iPattern.Isotopes[i].Intensity / simMaxInt * -100;
                            annotation.X = iPattern.Isotopes[i].Mass - 0.0005485 * ionModeFactor;
                            annotation.LineStyle = LineStyle.Solid;
                            annotation.Type = LineAnnotationType.Vertical;
                            annotation.StrokeThickness = 1.5;
                            ms1model.Annotations.Add(annotation);
                        }
                    }

                    double maxMZ = ms1scatterSeries.Points.Max(o => o.X);
                    double minMZ = ms1scatterSeries.Points.Min(o => o.X);
                    if (item.Ms1 != null && item.Ms1.Count > 0 && item.Candidates != null && item.Candidates.Count > 0)
                    {
                        maxMZ = item.Ms1.Max(o => o.Mz);
                        minMZ = item.Ms1.Min(o => o.Mz);
                    }

                    var zeroLine = new LineAnnotation();
                    zeroLine.Color = OxyColors.Black;
                    zeroLine.Y = 0;
                    zeroLine.MinimumX = 0;
                    zeroLine.MaximumX = double.MaxValue;
                    zeroLine.LineStyle = LineStyle.Solid;
                    zeroLine.Type = LineAnnotationType.Horizontal;
                    zeroLine.StrokeThickness = 1.5;
                    ms1model.Annotations.Add(zeroLine);

                    var ms1customAxis = new RangeColorAxis { Key = "customColors" };
                    ms1customAxis.AddRange(0, 2000, OxyColors.DarkGray);
                    ms1model.Axes.Add(ms1customAxis);
                    ms1model.Series.Add(ms1scatterSeries);

                    //Debug.WriteLine("min " + minMZ);
                    //Debug.WriteLine("max " + maxMZ);

                    //x-axis
                    ms1model.Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Title = "m/z",
                        AbsoluteMinimum = minMZ - 3.0,
                        Minimum = minMZ - 3.0,
                        AbsoluteMaximum = maxMZ + 3.0,
                        Maximum = maxMZ + 3.0,
                        TitleFontSize = 16
                    });
                    //y-axis
                    ms1model.Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Relative Abundance",
                        Minimum = -110,
                        Maximum = 110,
                        TitleFontSize = 15
                    });
                    MS1ComparePlot.Model = ms1model;
                    MS1ComparePlot.Visibility = Visibility.Visible;
                    ms1ComparePlotExpLabel.Visibility = Visibility.Visible;
                    ms1ComparePlotTheoLabel.Visibility = Visibility.Visible;
                    emptyMS1CompareImage.Visibility = Visibility.Collapsed;

                    if (item.Ms1 != null && item.Ms1.Count > 0 && item.Candidates != null && item.Candidates.Count > 0)
                    {
                        ms1PlotIspSimilaritySP.Visibility = Visibility.Visible;
                        //Debug.WriteLine("ispsim===" + item.Candidates[0].ispsim);
                        ms1PlotIspSimilarity.Text = "Isotope Similarity: " + Math.Round(item.Candidates[0].ispsim, 2);
                    }
                    else
                    {
                        ms1PlotIspSimilaritySP.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    MS1ComparePlot.Visibility = Visibility.Collapsed;
                    ms1ComparePlotExpLabel.Visibility = Visibility.Collapsed;
                    ms1ComparePlotTheoLabel.Visibility = Visibility.Collapsed;
                    emptyMS1CompareImage.Visibility = Visibility.Visible;

                    ms1PlotIspSimilaritySP.Visibility = Visibility.Collapsed;

                }



                // RAW MS2
                List<RAW_PeakElement> allMS2 = new List<RAW_PeakElement>(item.OriSpectrum); //fragment spectrum for plotting

                //fragment table
                var MS2selected = (FragmentModel)fragmentGrid.DataContext; //ms2 fragments dataframe
                MS2selected.Fragments.Clear(); //clear ms2 fragments dataframe
                if (allMS2.Count > 0)
                {
                    double maxInt = allMS2.Max(x => x.Intensity);
                    //process precursor fragments
                    if (item.PrecursorFragments != null && item.PrecursorFragments.Count > 0)
                    {
                        // find the fragment closet to the premass
                        int indexInPrecursorFrags = 0;
                        double currMzDiff;
                        bool precursorFragFound = false;
                        if (ppm)
                        {
                            currMzDiff = ms2tol * item.Mz * 1e-6;
                        }
                        else
                        {
                            currMzDiff = ms2tol;
                        }
                        for (int z = 0; z < item.PrecursorFragments.Count; z++)
                        {
                            if (Math.Abs(item.PrecursorFragments[z].Mz - item.Mz) < currMzDiff)
                            {
                                indexInPrecursorFrags = z;
                                currMzDiff = Math.Abs(item.PrecursorFragments[z].Mz - item.Mz);
                                precursorFragFound = true;
                            }
                        }

                        int d = 0;
                        foreach (RAW_PeakElement pk in item.PrecursorFragments)
                        {
                            if (d == indexInPrecursorFrags && precursorFragFound) // precursor fragment
                            {
                                MS2selected.Fragments.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Precursor"));
                            }
                            else
                            {
                                MS2selected.Fragments.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Noise"));
                            }
                            d++;
                        }
                    }

                    //process noise fragments
                    if (item.NoiseFragments != null && item.NoiseFragments.Count > 0)
                    {
                        foreach (RAW_PeakElement pk in item.NoiseFragments)
                        {
                            MS2selected.Fragments.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Noise"));
                        }
                    }

                    //process isotope fragments
                    if (item.IsotopeFragments != null && item.IsotopeFragments.Count > 0)
                    {
                        foreach (RAW_PeakElement pk in item.IsotopeFragments)
                        {
                            MS2selected.Fragments.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Isotope"));
                        }
                    }

                    // process spectrum                    
                    if (item.Candidates == null || item.Candidates.Count == 0) //no candidates
                    {
                        if (item.Spectrum != null)
                        {
                            foreach (RAW_PeakElement pk in item.Spectrum)
                            {
                                MS2selected.Fragments.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1), Math.Round(pk.Intensity / maxInt * 100, 1), "Unexplained"));
                            }
                        }
                        else //  no valid "spectrum"
                        {
                            foreach (RAW_PeakElement pk in item.OriSpectrum)
                            {
                                MS2selected.Fragments.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1), Math.Round(pk.Intensity / maxInt * 100, 1), "Unexplained"));
                            }
                        }

                    }
                    else
                    {
                        // for MS2 fragment reannotation
                        int ionModeFactor = 1;
                        List<int> candidateNeutralFormList = new List<int>();
                        if (MS2FragReannotate)
                        {
                            if (item.Polarity == "P")
                            {
                                ionModeFactor = 1;
                            }
                            else
                            {
                                ionModeFactor = -1;
                            }

                            if (item.Candidates[0].cfdf != null && item.Candidates[0].cfdf.Count > 0)
                            {
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].c);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].h);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].n);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].o);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].p);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].f);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].cl);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].br);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].i);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].s);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].si);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].b);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].se);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].na);
                                candidateNeutralFormList.Add(item.Candidates[0].cfdf[0].k);
                            }
                            else
                            {
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.c);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.h);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.n);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.o);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.p);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.f);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.cl);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.br);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.i);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.s);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.si);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.b);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.se);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.na);
                                candidateNeutralFormList.Add(item.Candidates[0].alignedFormula.k);
                            }
                        }


                        // have candidates & first time access
                        if (item.Candidates[0].accessedBefore == false)
                        {
                            List<string> fragFormStrList = new List<string>();
                            List<bool> fragReannotatedBool = new List<bool>();

                            int ind = 0;
                            foreach (RAW_PeakElement pk in item.Spectrum)
                            {
                                List<CFDF> expList = item.Candidates[0].cfdf.Where(o => o.expfragindex == ind).ToList(); //get corresponding fragment explanation for first candidate
                                string expFrag;
                                bool fragReannotation = false;
                                if (expList.Count > 0) //fragment explained
                                {
                                    //recreate new formula
                                    string new_formula = "";
                                    IDictionary<string, int> v = new Dictionary<string, int>();
                                    v.Add("C", expList[0].c_f); //adding a key/value using the Add() method
                                    v.Add("H", expList[0].h_f);
                                    v.Add("B", expList[0].b_f);
                                    v.Add("Br", expList[0].br_f);
                                    v.Add("Cl", expList[0].cl_f);
                                    v.Add("F", expList[0].f_f);
                                    v.Add("I", expList[0].i_f);
                                    v.Add("K", expList[0].k_f);
                                    v.Add("N", expList[0].n_f);
                                    v.Add("Na", expList[0].na_f);
                                    v.Add("O", expList[0].o_f);
                                    v.Add("P", expList[0].p_f);
                                    v.Add("S", expList[0].s_f);
                                    v.Add("Se", expList[0].se_f);
                                    v.Add("Si", expList[0].si_f);
                                    foreach (KeyValuePair<string, int> element in v)
                                    {
                                        if (element.Value > 1)
                                        {
                                            new_formula = new_formula + element.Key + element.Value.ToString();
                                        }
                                        if (element.Value == 1)
                                        {
                                            new_formula = new_formula + element.Key;
                                        }
                                    }
                                    expFrag = new_formula;
                                }
                                else //fragment unexplained
                                {
                                    expFrag = "Unexplained";
                                    if (MS2FragReannotate)
                                    {
                                        expFrag = MS2FragmentReannotation(ppm, ms2tol, pk.Mz, ionModeFactor, candidateNeutralFormList, groupedDB);
                                        fragReannotation = true;
                                    }
                                }
                                fragFormStrList.Add(expFrag);
                                fragReannotatedBool.Add(fragReannotation);
                                MS2selected.Fragments.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), expFrag));
                                ind++;
                            }
                            item.Candidates[0].fragmentFormulaList = fragFormStrList;
                            item.Candidates[0].fragmentReannotated = fragReannotatedBool;
                            item.Candidates[0].accessedBefore = true;
                            item.Candidates[0].fragmentReannotatedCompleted = true;
                        }
                        // have candidates & not the first time access
                        else
                        {
                            if (MS2FragReannotate)
                            {
                                if (item.Candidates[0].fragmentReannotatedCompleted)
                                {
                                    int ind = 0;
                                    foreach (RAW_PeakElement pk in item.Spectrum)
                                    {
                                        MS2selected.Fragments.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                            Math.Round(pk.Intensity / maxInt * 100, 1), item.Candidates[0].fragmentFormulaList[ind]));
                                        ind++;
                                    }
                                }
                                else
                                {
                                    int ind = 0;
                                    foreach (RAW_PeakElement pk in item.Spectrum)
                                    {
                                        if (item.Candidates[0].fragmentFormulaList[ind] == "Unexplained")
                                        {
                                            item.Candidates[0].fragmentFormulaList[ind] = MS2FragmentReannotation(ppm, ms2tol, pk.Mz, ionModeFactor, candidateNeutralFormList, groupedDB);
                                            item.Candidates[0].fragmentReannotated[ind] = true;
                                        }
                                        MS2selected.Fragments.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                            Math.Round(pk.Intensity / maxInt * 100, 1), item.Candidates[0].fragmentFormulaList[ind]));
                                        
                                        ind++;
                                    }
                                    item.Candidates[0].fragmentReannotatedCompleted = true;
                                }
                            }
                            else
                            {
                                int ind = 0;
                                foreach (RAW_PeakElement pk in item.Spectrum)
                                {
                                    string expFrag = item.Candidates[0].fragmentFormulaList[ind];
                                    if (item.Candidates[0].fragmentReannotated[ind])
                                    {
                                        expFrag = "Unexplained";
                                    }
                                    MS2selected.Fragments.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                        Math.Round(pk.Intensity / maxInt * 100, 1), expFrag));
                                    ind++;
                                }
                            }

                        }
                    }


                    //modify fragment dataframe index number from 0+ to 1+
                    MS2selected.Fragments = new ObservableCollection<FragmentUtility>(MS2selected.Fragments.OrderBy(o => o.Mz).ToList());
                    for (int i = 0; i < MS2selected.Fragments.Count; i++)
                    {
                        MS2selected.Fragments[i].Index = i + 1;
                    }

                    ms2SpecBox_fragNum.Text = allMS2.Count.ToString();

                    #region
                    //labels
                    ms2SpecBox_mz.Text = Math.Round(item.Mz, 4).ToString();
                    ms2SpecBox_rt.Text = Math.Round(item.Rt, 2).ToString();

                    RAW_PeakElement basePeak = allMS2.MaxBy(o => o.Intensity).OrderBy(o => o.Mz).First();
                    ms2SpecBox_bpmz.Text = Math.Round(basePeak.Mz, 4).ToString();
                    ms2SpecBox_bpint.Text = Math.Round(basePeak.Intensity, 1).ToString();

                    ms2FragBox_filename.Text = item.Filename;
                    ms2FragBox_scanNum.Text = item.ScanNumber.ToString();
                    ms2FragBox_adduct.Text = item.Adduct.Formula;


                    //plot ms2 fragment spectrum
                    var model = new PlotModel { Title = "" };
                    var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
                    for (int i = 0; i < allMS2.Count; i++)
                    {
                        var x = allMS2[i].Mz;
                        var y = allMS2[i].Intensity;
                        var size = 2;
                        var colorValue = 1;
                        scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

                        var annotation = new LineAnnotation();
                        annotation.Color = OxyColors.MediumPurple;
                        annotation.MinimumY = 0;
                        annotation.MaximumY = allMS2[i].Intensity;
                        annotation.X = allMS2[i].Mz;
                        annotation.LineStyle = LineStyle.Solid;
                        annotation.Type = LineAnnotationType.Vertical;
                        annotation.StrokeThickness = 1.7;
                        model.Annotations.Add(annotation);
                    }

                    var customAxis = new RangeColorAxis { Key = "customColors" };
                    customAxis.AddRange(0, 2000, OxyColors.Purple);
                    model.Axes.Add(customAxis);
                    model.Series.Add(scatterSeries);

                    model.Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Title = "m/z",
                        AbsoluteMinimum = 0,
                        Minimum = 0,
                        Maximum = allMS2.Max(o => o.Mz) + 30,
                        TitleFontSize = 15
                    });
                    model.Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Absolute Abundance",
                        AbsoluteMinimum = 0,
                        Minimum = 0,
                        Maximum = basePeak.Intensity * 1.2,
                        TitleFontSize = 15,
                        AxisTitleDistance = 15
                    });
                    MS2Spectrum.Model = model;
                    #endregion
                }
                else
                {
                    ms2SpecBox_fragNum.Text = "0";

                    #region
                    //labels
                    ms2SpecBox_mz.Text = Math.Round(item.Mz, 4).ToString();
                    ms2SpecBox_rt.Text = Math.Round(item.Rt, 2).ToString();

                    ms2SpecBox_bpmz.Text = "";
                    ms2SpecBox_bpint.Text = "";

                    ms2FragBox_filename.Text = item.Filename;
                    ms2FragBox_scanNum.Text = item.ScanNumber.ToString();
                    ms2FragBox_adduct.Text = item.Adduct.Formula;


                    //plot ms2 fragment spectrum
                    var model = new PlotModel { Title = "" };
                    var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

                    var x = item.Mz;
                    var y = 0;
                    var size = 2;
                    var colorValue = 1;
                    scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

                    var annotation = new LineAnnotation();
                    annotation.Color = OxyColors.MediumPurple;
                    annotation.MinimumY = 0;
                    annotation.MaximumY = 0;
                    annotation.X = item.Mz;
                    annotation.LineStyle = LineStyle.Solid;
                    annotation.Type = LineAnnotationType.Vertical;
                    annotation.StrokeThickness = 1.7;
                    model.Annotations.Add(annotation);

                    var customAxis = new RangeColorAxis { Key = "customColors" };
                    customAxis.AddRange(0, 2000, OxyColors.Purple);
                    model.Axes.Add(customAxis);
                    model.Series.Add(scatterSeries);

                    model.Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Title = "m/z",
                        AbsoluteMinimum = 0,
                        Minimum = 0,
                        Maximum = item.Mz + 30,
                        TitleFontSize = 15
                    });
                    model.Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Absolute Abundance",
                        AbsoluteMinimum = 0,
                        Minimum = 0,
                        Maximum = 100,
                        TitleFontSize = 15,
                        AxisTitleDistance = 15
                    });
                    MS2Spectrum.Model = model;
                    #endregion
                }

                //MS2Spectrum.Model.TitleFontWeight = 2;
            }
        }


        //When user clicks on a row in the candidate formula table...
        //Display information for the clicked candidate formula
        private void candidateFormulaGrid_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            bool MS2FragReannotate = (bool)localSettings.Values["MS2FragReannotate_include"];
            bool ppm = (bool)localSettings.Values["ms1tol_ppmON"];
            double ms2tol = (double)localSettings.Values["ms2tol"];

            Ms2Utility currMS2 = (Ms2Utility)ms2grid.CurrentItem; //current selected ms2
            CandidateUtility item = (CandidateUtility)candidateFormulaGrid.CurrentItem; //current selected candidate formula

            int candidateIndex = currMS2.Candidates.FindIndex(o => o.p_formula == item.Formula);

            if (item != null)
            {
                //database list
                candidateDB_list.Clear();
                if (item.AlignedFormula != null)
                {// add database icons
                    #region
                    if (item.AlignedFormula.PubChem > 0)
                    {
                        candidateDB_list.Add(new Database("PubChem_include"));
                    }
                    if (item.AlignedFormula.ANPDB > 0)
                    {
                        candidateDB_list.Add(new Database("ANPDB_include"));
                    }
                    if (item.AlignedFormula.BLEXP > 0)
                    {
                        candidateDB_list.Add(new Database("BLEXP_include"));
                    }
                    if (item.AlignedFormula.BMDB > 0)
                    {
                        candidateDB_list.Add(new Database("BMDB_include"));
                    }
                    if (item.AlignedFormula.ChEBI > 0)
                    {
                        candidateDB_list.Add(new Database("ChEBI_include"));
                    }
                    if (item.AlignedFormula.COCONUT > 0)
                    {
                        candidateDB_list.Add(new Database("COCONUT_include"));
                    }
                    if (item.AlignedFormula.DrugBank > 0)
                    {
                        candidateDB_list.Add(new Database("DrugBank_include"));
                    }
                    if (item.AlignedFormula.DSSTOX > 0)
                    {
                        candidateDB_list.Add(new Database("DSSTOX_include"));
                    }
                    if (item.AlignedFormula.ECMDB > 0)
                    {
                        candidateDB_list.Add(new Database("ECMDB_include"));
                    }
                    if (item.AlignedFormula.FooDB > 0)
                    {
                        candidateDB_list.Add(new Database("FooDB_include"));
                    }
                    if (item.AlignedFormula.HMDB > 0)
                    {
                        candidateDB_list.Add(new Database("HMDB_include"));
                    }
                    if (item.AlignedFormula.HSDB > 0)
                    {
                        candidateDB_list.Add(new Database("HSDB_include"));
                    }
                    if (item.AlignedFormula.KEGG > 0)
                    {
                        candidateDB_list.Add(new Database("KEGG_include"));
                    }
                    if (item.AlignedFormula.LMSD > 0)
                    {
                        candidateDB_list.Add(new Database("LMSD_include"));
                    }
                    if (item.AlignedFormula.MaConDa > 0)
                    {
                        candidateDB_list.Add(new Database("MaConDa_include"));
                    }
                    if (item.AlignedFormula.MarkerDB > 0)
                    {
                        candidateDB_list.Add(new Database("MarkerDB_include"));
                    }
                    if (item.AlignedFormula.MCDB > 0)
                    {
                        candidateDB_list.Add(new Database("MCDB_include"));
                    }
                    if (item.AlignedFormula.NORMAN > 0)
                    {
                        candidateDB_list.Add(new Database("NORMAN_include"));
                    }
                    if (item.AlignedFormula.NPASS > 0)
                    {
                        candidateDB_list.Add(new Database("NPASS_include"));
                    }
                    if (item.AlignedFormula.Plantcyc > 0)
                    {
                        candidateDB_list.Add(new Database("Plantcyc_include"));
                    }
                    if (item.AlignedFormula.SMPDB > 0)
                    {
                        candidateDB_list.Add(new Database("SMPDB_include"));
                    }
                    if (item.AlignedFormula.STOFF_IDENT > 0)
                    {
                        candidateDB_list.Add(new Database("STF_IDENT_include"));
                    }
                    if (item.AlignedFormula.T3DB > 0)
                    {
                        candidateDB_list.Add(new Database("T3DB_include"));
                    }
                    if (item.AlignedFormula.TTD > 0)
                    {
                        candidateDB_list.Add(new Database("TTD_include"));
                    }
                    if (item.AlignedFormula.UNPD > 0)
                    {
                        candidateDB_list.Add(new Database("UNPD_include"));
                    }
                    if (item.AlignedFormula.YMDB > 0)
                    {
                        candidateDB_list.Add(new Database("YMDB_include"));
                    }
                    #endregion

                }

                //MS1 Comparison Plot
                if (currMS2 != null)
                {
                    if ((currMS2.Ms1 != null && currMS2.Ms1.Count > 0) || (currMS2.Candidates != null && currMS2.Candidates.Count > 0))
                    {
                        var ms1model = new PlotModel { Title = "" };
                        var ms1scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

                        if (currMS2.Ms1 != null && currMS2.Ms1.Count > 0)
                        {
                            double expMaxInt = currMS2.Ms1.Max(o => o.Intensity);
                            for (int i = 0; i < currMS2.Ms1.Count; i++)
                            {
                                var x = currMS2.Ms1[i].Mz;
                                var y = currMS2.Ms1[i].Intensity / expMaxInt * 100;
                                var size = 2;
                                var colorValue = 1;
                                ms1scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));
                                var annotation = new LineAnnotation();
                                annotation.Color = OxyColors.Orange;
                                annotation.MinimumY = 0;
                                annotation.MaximumY = currMS2.Ms1[i].Intensity / expMaxInt * 100;
                                annotation.X = currMS2.Ms1[i].Mz;
                                annotation.LineStyle = LineStyle.Solid;
                                annotation.Type = LineAnnotationType.Vertical;
                                annotation.StrokeThickness = 1.5;
                                ms1model.Annotations.Add(annotation);
                            }
                        }

                        //1+ candidates, use first candidate theoretical ms1
                        if (currMS2.Candidates != null && currMS2.Candidates.Count > 0)
                        {
                            IsotopePattern iPattern;
                            //theoMS1 already calculated in booster because ms2 has experimental ms1
                            if (item.theoMS1 != null)
                            {
                                iPattern = item.theoMS1;
                            }
                            //theoMS1 not precalculated
                            else
                            {
                                string new_formula = "";
                                IDictionary<string, int> v = new Dictionary<string, int>();
                                v.Add("C", item.cfdf[0].c + currMS2.adduct.SumFormula.c); //adding a key/value using the Add() method
                                v.Add("H", item.cfdf[0].h + currMS2.adduct.SumFormula.h);
                                v.Add("B", item.cfdf[0].b + currMS2.adduct.SumFormula.b);
                                v.Add("Br", item.cfdf[0].br + currMS2.adduct.SumFormula.br);
                                v.Add("Cl", item.cfdf[0].cl + currMS2.adduct.SumFormula.cl);
                                v.Add("F", item.cfdf[0].f + currMS2.adduct.SumFormula.f);
                                v.Add("I", item.cfdf[0].i + currMS2.adduct.SumFormula.i);
                                v.Add("K", item.cfdf[0].k + currMS2.adduct.SumFormula.k);
                                v.Add("N", item.cfdf[0].n + currMS2.adduct.SumFormula.n);
                                v.Add("Na", item.cfdf[0].na + currMS2.adduct.SumFormula.na);
                                v.Add("O", item.cfdf[0].o + currMS2.adduct.SumFormula.o);
                                v.Add("P", item.cfdf[0].p + currMS2.adduct.SumFormula.p);
                                v.Add("S", item.cfdf[0].s + currMS2.adduct.SumFormula.s);
                                v.Add("Se", item.cfdf[0].se + currMS2.adduct.SumFormula.se);
                                v.Add("Si", item.cfdf[0].si + currMS2.adduct.SumFormula.si);
                                foreach (KeyValuePair<string, int> element in v)
                                {
                                    if (element.Value > 1)
                                    {
                                        new_formula = new_formula + element.Key + element.Value.ToString();
                                    }
                                    if (element.Value == 1)
                                    {
                                        new_formula = new_formula + element.Key;
                                    }
                                }

                                IMolecularFormula mf = MolecularFormulaManipulator.GetMolecularFormula(new_formula);
                                IsotopePatternGenerator IPG = new IsotopePatternGenerator((double)localSettings.Values["isotopic_abundance_cutoff"] / 100);
                                iPattern = IPG.GetIsotopes(mf);
                            }

                            double simMaxInt = iPattern.Isotopes.Max(o => o.Intensity); //find max intensity for normalization
                            //add datapoints
                            for (int i = 0; i < iPattern.Isotopes.Count; i++)
                            {
                                var x = iPattern.Isotopes[i].Mass;
                                var y = iPattern.Isotopes[i].Intensity / simMaxInt * -100;
                                var size = 2;
                                var colorValue = 1;
                                ms1scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));
                                var annotation = new LineAnnotation();
                                annotation.Color = OxyColors.DarkBlue;
                                annotation.MaximumY = 0;
                                annotation.MinimumY = iPattern.Isotopes[i].Intensity / simMaxInt * -100;
                                annotation.X = iPattern.Isotopes[i].Mass;
                                annotation.LineStyle = LineStyle.Solid;
                                annotation.Type = LineAnnotationType.Vertical;
                                annotation.StrokeThickness = 1.5;
                                ms1model.Annotations.Add(annotation);
                            }
                        }

                        double maxMZ = ms1scatterSeries.Points.Max(o => o.X);
                        double minMZ = ms1scatterSeries.Points.Min(o => o.X);
                        if (currMS2.Ms1 != null && currMS2.Ms1.Count > 0 && currMS2.Candidates != null && currMS2.Candidates.Count > 0)
                        {
                            maxMZ = currMS2.Ms1.Max(o => o.Mz);
                            minMZ = currMS2.Ms1.Min(o => o.Mz);
                        }
                        var zeroLine = new LineAnnotation();
                        zeroLine.Color = OxyColors.Black;
                        zeroLine.Y = 0;
                        zeroLine.MinimumX = 0;
                        zeroLine.MaximumX = double.MaxValue;
                        zeroLine.LineStyle = LineStyle.Solid;
                        zeroLine.Type = LineAnnotationType.Horizontal;
                        zeroLine.StrokeThickness = 1.5;
                        ms1model.Annotations.Add(zeroLine);

                        var ms1customAxis = new RangeColorAxis { Key = "customColors" };
                        ms1customAxis.AddRange(0, 2000, OxyColors.DarkGray);
                        ms1model.Axes.Add(ms1customAxis);
                        ms1model.Series.Add(ms1scatterSeries);

                        ms1model.Axes.Add(new LinearAxis
                        {
                            Position = AxisPosition.Bottom,
                            Title = "m/z",
                            AbsoluteMinimum = Math.Max(minMZ - 2, 0),
                            Minimum = Math.Max(minMZ - 2, 0),
                            AbsoluteMaximum = maxMZ + 2,
                            Maximum = maxMZ + 2,
                            TitleFontSize = 15
                        });
                        ms1model.Axes.Add(new LinearAxis
                        {
                            Position = AxisPosition.Left,
                            Title = "Relative Abundance",
                            Minimum = -110,
                            Maximum = 110,
                            TitleFontSize = 15
                        });
                        MS1ComparePlot.Model = ms1model;
                        MS1ComparePlot.Visibility = Visibility.Visible;
                        ms1ComparePlotExpLabel.Visibility = Visibility.Visible;
                        ms1ComparePlotTheoLabel.Visibility = Visibility.Visible;
                        emptyMS1CompareImage.Visibility = Visibility.Collapsed;

                        if (currMS2.Ms1 != null && currMS2.Ms1.Count > 0 && currMS2.Candidates != null && currMS2.Candidates.Count > 0)
                        {
                            ms1PlotIspSimilaritySP.Visibility = Visibility.Visible;
                            //Debug.WriteLine("ispsim===" + item.ispsim);
                            ms1PlotIspSimilarity.Text = "Isotope Similarity: " + Math.Round(item.ispsim, 2);
                        }
                        else
                        {
                            ms1PlotIspSimilaritySP.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        MS1ComparePlot.Visibility = Visibility.Collapsed;
                        ms1ComparePlotExpLabel.Visibility = Visibility.Collapsed;
                        ms1ComparePlotTheoLabel.Visibility = Visibility.Collapsed;
                        emptyMS1CompareImage.Visibility = Visibility.Visible;

                        ms1PlotIspSimilaritySP.Visibility = Visibility.Collapsed;

                    }
                }

                //fragment table
                var MS2selected = (FragmentModel)fragmentGrid.DataContext;
                if (MS2selected == null || MS2selected.Fragments == null || MS2selected.Fragments.Count == 0)
                {
                    return;
                }

                // for MS2 fragment reannotation
                int ionModeFactor = 1;
                List<int> candidateNeutralFormList = new List<int>();
                if (MS2FragReannotate)
                {

                    if (currMS2.Polarity == "P")
                    {
                        ionModeFactor = 1;
                    }
                    else
                    {
                        ionModeFactor = -1;
                    }

                    if (currMS2.Candidates[candidateIndex].cfdf != null && currMS2.Candidates[candidateIndex].cfdf.Count > 0)
                    {
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].c);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].h);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].n);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].o);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].p);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].f);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].cl);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].br);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].i);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].s);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].si);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].b);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].se);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].na);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].cfdf[0].k);
                    }
                    else
                    {
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.c);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.h);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.n);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.o);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.p);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.f);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.cl);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.br);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.i);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.s);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.si);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.b);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.se);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.na);
                        candidateNeutralFormList.Add(currMS2.Candidates[candidateIndex].alignedFormula.k);
                    }
                }


                // first time access this candidate
                if (currMS2.Candidates[candidateIndex].accessedBefore == false)
                {
                    List<string> fragFormStrList = new List<string>();
                    List<bool> fragReannotatedBool = new List<bool>();

                    // order by mass
                    MS2selected.Fragments = new ObservableCollection<FragmentUtility>(MS2selected.Fragments.OrderBy(o => o.Mz).ToList());
                    int ind = 0;
                    for (int i = 0; i < MS2selected.Fragments.Count; i++)
                    {
                        if (MS2selected.Fragments[i].Formula == "Isotope" || MS2selected.Fragments[i].Formula == "Precursor" || MS2selected.Fragments[i].Formula == "Noise")
                        {
                            continue;
                        }
                        List<CFDF> expList = item.cfdf.Where(o => o.expfragindex == ind).ToList();
                        string expFrag;
                        bool fragReannotation = false;
                        if (expList.Count > 0)
                        {
                            string new_formula = "";
                            IDictionary<string, int> v = new Dictionary<string, int>();
                            v.Add("C", expList[0].c_f); //adding a key/value using the Add() method
                            v.Add("H", expList[0].h_f);
                            v.Add("B", expList[0].b_f);
                            v.Add("Br", expList[0].br_f);
                            v.Add("Cl", expList[0].cl_f);
                            v.Add("F", expList[0].f_f);
                            v.Add("I", expList[0].i_f);
                            v.Add("K", expList[0].k_f);
                            v.Add("N", expList[0].n_f);
                            v.Add("Na", expList[0].na_f);
                            v.Add("O", expList[0].o_f);
                            v.Add("P", expList[0].p_f);
                            v.Add("S", expList[0].s_f);
                            v.Add("Se", expList[0].se_f);
                            v.Add("Si", expList[0].si_f);
                            foreach (KeyValuePair<string, int> element in v)
                            {
                                if (element.Value > 1)
                                {
                                    new_formula = new_formula + element.Key + element.Value.ToString();
                                }
                                if (element.Value == 1)
                                {
                                    new_formula = new_formula + element.Key;
                                }
                            }
                            expFrag = new_formula;
                        }
                        else
                        {
                            expFrag = "Unexplained";
                            if (MS2FragReannotate)
                            {
                                expFrag = MS2FragmentReannotation(ppm, ms2tol, MS2selected.Fragments[i].Mz, ionModeFactor, candidateNeutralFormList, groupedDB);
                                fragReannotation = true;
                            }
                        }
                        fragFormStrList.Add(expFrag);
                        fragReannotatedBool.Add(fragReannotation);
                        MS2selected.Fragments[i].Formula = expFrag;
                        ind++;
                    }
                    currMS2.Candidates[candidateIndex].fragmentFormulaList = fragFormStrList;
                    currMS2.Candidates[candidateIndex].fragmentReannotated = fragReannotatedBool;
                    currMS2.Candidates[candidateIndex].accessedBefore = true;
                    currMS2.Candidates[candidateIndex].fragmentReannotatedCompleted = true;
                }
                else
                {
                    if (MS2FragReannotate)
                    {
                        if (currMS2.Candidates[candidateIndex].fragmentReannotatedCompleted)
                        {
                            int spectrumIndex = 0;
                            for (int i = 0; i < MS2selected.Fragments.Count; i++)
                            {
                                if (MS2selected.Fragments[i].Formula == "Isotope" || MS2selected.Fragments[i].Formula == "Precursor" || MS2selected.Fragments[i].Formula == "Noise")
                                {
                                    continue;
                                }
                                else
                                {
                                    MS2selected.Fragments[i].Formula = currMS2.Candidates[candidateIndex].fragmentFormulaList[spectrumIndex];
                                    spectrumIndex++;
                                }
                            }
                        }
                        else
                        {
                            int spectrumIndex = 0;
                            for (int i = 0; i < MS2selected.Fragments.Count; i++)
                            {
                                if (MS2selected.Fragments[i].Formula == "Isotope" || MS2selected.Fragments[i].Formula == "Precursor" || MS2selected.Fragments[i].Formula == "Noise")
                                {
                                    continue;
                                }
                                else
                                {
                                    if(currMS2.Candidates[candidateIndex].fragmentFormulaList[spectrumIndex] == "Unexplained")
                                    {
                                        currMS2.Candidates[candidateIndex].fragmentFormulaList[spectrumIndex] = MS2FragmentReannotation(ppm, ms2tol, MS2selected.Fragments[i].Mz,
                                                                        ionModeFactor, candidateNeutralFormList, groupedDB);
                                        currMS2.Candidates[candidateIndex].fragmentReannotated[spectrumIndex] = true;
                                    }
                                    MS2selected.Fragments[i].Formula = currMS2.Candidates[candidateIndex].fragmentFormulaList[spectrumIndex];
                                    spectrumIndex++;
                                }
                            }
                            currMS2.Candidates[candidateIndex].fragmentReannotatedCompleted = true;
                        }
                    }
                    else
                    {
                        int spectrumIndex = 0; // count the index of fragments other than isotope, noise, precursor
                        for (int i = 0; i < MS2selected.Fragments.Count; i++)
                        {
                            if (MS2selected.Fragments[i].Formula == "Isotope" || MS2selected.Fragments[i].Formula == "Precursor" || MS2selected.Fragments[i].Formula == "Noise")
                            {
                                continue;
                            }
                            else
                            {
                                if (currMS2.Candidates[candidateIndex].fragmentReannotated[spectrumIndex])
                                {
                                    MS2selected.Fragments[i].Formula = "Unexplained";
                                }
                                else
                                {
                                    MS2selected.Fragments[i].Formula = currMS2.Candidates[candidateIndex].fragmentFormulaList[spectrumIndex];
                                }
                                spectrumIndex++;
                            }
                        }
                    }
                }

                //histograms
                List<CFDF> histogramCFDF = item.cfdf;

                List<CFDF> histogramH2C_CFDF = item.cfdf.Where(o =>
                    o.h2c_f < 888888 && o.h2c_f > -888888 &&
                    o.h2c_l < 888888 && o.h2c_l > -888888).ToList();

                List<CFDF> histogramHetero2C_CFDF = item.cfdf.Where(o =>
                    o.hetero2c_f < 888888 && o.hetero2c_f > -888888 &&
                    o.hetero2c_l < 888888 && o.hetero2c_l > -888888).ToList();

                if ((histogramH2C_CFDF.Count + histogramHetero2C_CFDF.Count) > 0 && histogramCFDF[0].expfragindex != -1)
                {
                    fragDBEhistogram.ItemsSource = histogramCFDF;
                    double fragDBEhistogramInterval = (histogramCFDF.Max(o => o.dbe_f) - histogramCFDF.Min(o => o.dbe_f)) / 6;
                    if (fragDBEhistogramInterval != 0.0)
                    {
                        fragDBEhistogram.HistogramInterval = fragDBEhistogramInterval;
                    }
                    else
                    {
                        fragDBEhistogram.HistogramInterval = 1;
                    }

                    nlDBEhistogram.ItemsSource = histogramCFDF;
                    double nlDBEhistogramInterval = (histogramCFDF.Max(o => o.dbe_l) - histogramCFDF.Min(o => o.dbe_l)) / 6;
                    if (nlDBEhistogramInterval != 0.0)
                    {
                        nlDBEhistogram.HistogramInterval = nlDBEhistogramInterval;
                    }
                    else
                    {
                        nlDBEhistogram.HistogramInterval = 1;
                    }

                    if(histogramH2C_CFDF.Count > 0)
                    {
                        fragH2Chistogram.ItemsSource = histogramH2C_CFDF;
                        double fragH2ChistogramInterval = (histogramH2C_CFDF.Max(o => o.h2c_f) - histogramH2C_CFDF.Min(o => o.h2c_f)) / 6;
                        if (fragH2ChistogramInterval != 0.0)
                        {
                            fragH2Chistogram.HistogramInterval = fragH2ChistogramInterval;
                        }
                        else
                        {
                            fragH2Chistogram.HistogramInterval = 0.2;
                        }

                        nlH2Chistogram.ItemsSource = histogramH2C_CFDF;
                        double nlH2ChistogramInterval = (histogramH2C_CFDF.Max(o => o.h2c_l) - histogramH2C_CFDF.Min(o => o.h2c_l)) / 6;
                        if (nlH2ChistogramInterval != 0.0)
                        {
                            nlH2Chistogram.HistogramInterval = nlH2ChistogramInterval;
                        }
                        else
                        {
                            nlH2Chistogram.HistogramInterval = 0.2;
                        }
                    }

                    if (histogramHetero2C_CFDF.Count > 0)
                    {
                        fragHetero2Chistogram.ItemsSource = histogramHetero2C_CFDF;
                        double fragHetero2ChistogramInterval = (histogramHetero2C_CFDF.Max(o => o.hetero2c_f) - histogramHetero2C_CFDF.Min(o => o.hetero2c_f)) / 6;
                        if (fragHetero2ChistogramInterval != 0.0)
                        {
                            fragHetero2Chistogram.HistogramInterval = fragHetero2ChistogramInterval;
                        }
                        else
                        {
                            fragHetero2Chistogram.HistogramInterval = 0.2;
                        }

                        nlHetero2Chistogram.ItemsSource = histogramHetero2C_CFDF;
                        double nlHetero2ChistogramInterval = (histogramHetero2C_CFDF.Max(o => o.hetero2c_l) - histogramHetero2C_CFDF.Min(o => o.hetero2c_l)) / 6;
                        if (nlHetero2ChistogramInterval != 0.0)
                        {
                            nlHetero2Chistogram.HistogramInterval = nlHetero2ChistogramInterval;
                        }
                        else
                        {
                            nlHetero2Chistogram.HistogramInterval = 0.2;
                        }
                    }                        

                    //radical ion ratio
                    ObservableCollection<RadicalIonRatioUtility> radicals = new ObservableCollection<RadicalIonRatioUtility>();
                    radicals.Add(new RadicalIonRatioUtility("Cnt.", item.cfdf.Where(o => (o.dbe_f % 1) == 0).Count(), item.cfdf.Where(o => (o.dbe_f % 1) != 0).Count()));

                    double radInt = 0.0;
                    double nonRadInt = 0.0;
                    for (int w = 0; w < item.cfdf.Count; w++)
                    {
                        if (item.cfdf[w].dbe_f % 1 == 0)
                        {
                            radInt += currMS2.Spectrum[item.cfdf[w].expfragindex].Intensity;
                        }
                        else
                        {
                            nonRadInt += currMS2.Spectrum[item.cfdf[w].expfragindex].Intensity;
                        }
                    }
                    radicals.Add(new RadicalIonRatioUtility("Int.", radInt, nonRadInt));

                    radicalIonPlot.ItemsSource = radicals;
                    nonRadicalIonPlot.ItemsSource = radicals;
                    radicalIonCt_box.Text = Math.Round(radicals[0].radical / (radicals[0].radical + radicals[0].nonradical) * 100, 0).ToString() + "%";
                    nonradicalIonCt_box.Text = Math.Round(radicals[0].nonradical / (radicals[0].radical + radicals[0].nonradical) * 100, 0).ToString() + "%";
                    radicalIonInt_box.Text = Math.Round(radicals[1].radical / (radicals[1].radical + radicals[1].nonradical) * 100, 0).ToString() + "%";
                    nonradicalIonInt_box.Text = Math.Round(radicals[1].nonradical / (radicals[1].radical + radicals[1].nonradical) * 100, 0).ToString() + "%";
                }
                else
                {
                    fragDBEhistogram.ItemsSource = new List<CFDF>();
                    nlDBEhistogram.ItemsSource = new List<CFDF>(); ;
                    fragH2Chistogram.ItemsSource = new List<CFDF>(); ;
                    nlH2Chistogram.ItemsSource = new List<CFDF>(); ;
                    fragHetero2Chistogram.ItemsSource = new List<CFDF>(); ;
                    nlHetero2Chistogram.ItemsSource = new List<CFDF>(); ;
                    radicalIonPlot.ItemsSource = new ObservableCollection<RadicalIonRatioUtility>();
                    nonRadicalIonPlot.ItemsSource = new ObservableCollection<RadicalIonRatioUtility>();
                    radicalIonCt_box.Text = "";
                    nonradicalIonCt_box.Text = "";
                    radicalIonInt_box.Text = "";
                    nonradicalIonInt_box.Text = "";
                }



            }
        }


        //export ms2 fragment table to csv
        private async void ExportMs2Fragments_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            exportInProgress = true;
            var MS2selected = (FragmentModel)fragmentGrid.DataContext;
            Ms2Utility item = (Ms2Utility)ms2grid.CurrentItem;
            var highlightcandidate = (CandidateUtility)candidateFormulaGrid.CurrentItem;  ///////////////////////////////////////////////

            if (MS2selected.Fragments.Count == 0)
            {
                NoFragToExport();
                exportInProgress = false;
                return;
            }

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("FragmentTable", new List<string>() { ".csv", ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "FileIndex_" + item.FileIndex + "_MS2Index_" + item.ScanNumber + "_" + highlightcandidate.Formula + ".csv";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                List<string> outCSV = new List<string>();
                outCSV.Add("Index," + "Mz," + "AbsoluteIntensity," + "RelativeIntensity," + "Formula");
                foreach (FragmentUtility frag in MS2selected.Fragments)
                {
                    outCSV.Add(frag.Index + "," + frag.Mz + "," + frag.Abs_int + "," + frag.Rel_int + "," + frag.Formula);
                }
                await FileIO.WriteLinesAsync(file, outCSV);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    FragmentCsvSaved(file.Name);
                }
                else
                {
                    FragmentCsvNotSaved(file.Name);
                }
            }
            else
            {
                FragmentCsvCancelled();
            }
            exportInProgress = false;
        }
        private async void FragmentCsvCancelled()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Operation cancelled.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void FragmentCsvSaved(string filename)
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Success",
                Content = "File " + filename + " was saved.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void FragmentCsvNotSaved(string filename)
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "File " + filename + " couldn't be saved.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void NoFragToExport()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "No MS2 fragments to export.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void ComputationCompleted()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Success",
                Content = "Computation jobs completed.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void NoMS2Selected()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "No MS2 selected.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }

        //open project
        private async void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var filemodel = (FileModel)dataGrid.DataContext;
            var ms2model = (Ms2Model)ms2grid.DataContext;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".BUDDY");
            openProjectFile = await picker.PickSingleFileAsync();
            if(openProjectFile != null)
            {
                try
                {
                    string json = await FileIO.ReadTextAsync(openProjectFile);
                    //string base64EncodedData = await FileIO.ReadTextAsync(openProjectFile);
                    //var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                    //string json = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                    SaveProjectUtility project = new SaveProjectUtility();
                    project = JsonSerializer.Deserialize<SaveProjectUtility>(json);
                    filemodel.Files.Clear();
                    foreach (FileUtility file in project.FileList)
                    {
                        file.Selected = false;
                        filemodel.Files.Add(file);
                    }
                    ms2model.MS2s.Clear();
                }
                catch
                {
                    InvalidProjectSelected();
                }
            }
            else
            {
                NoFileSelected();
            }

            if (ms2model.MS2s.Count == 0)
            {
                emptyMS2ListMessage.Visibility = Visibility.Visible;
            }
            else
            {
                emptyMS2ListMessage.Visibility = Visibility.Collapsed;
            }
        }
        //save file and ms2 table to JSON
        private async void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var filemodel = (FileModel)dataGrid.DataContext;
            ObservableCollection<FileUtility> fileList = filemodel.Files;

            if (fileList.Count == 0)
            {
                NoFileToExport();
                return;
            }
            if (openProjectFile != null)
            {
                SaveProjectLoad hud = new SaveProjectLoad("Loading");
                hud.Show();
                await Task.Delay(1);
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(openProjectFile);
                // write to file
                SaveProjectUtility saveProject = new SaveProjectUtility(fileList);
                string json = JsonSerializer.Serialize(saveProject);

                //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                //string Base64EncodedString = Convert.ToBase64String(plainTextBytes);
                //await FileIO.WriteTextAsync(openProjectFile, Base64EncodedString);
                await FileIO.WriteTextAsync(openProjectFile, json);

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(openProjectFile);

                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    hud.Close();
                    ExportProjectSaved(openProjectFile.Name);
                }
                else
                {
                    hud.Close();
                    ExportProjectNotSaved(openProjectFile.Name);
                }
            }
            else
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("BuddyProject", new List<string>() { ".BUDDY" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "Project_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".BUDDY";
                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    SaveProjectLoad hud = new SaveProjectLoad("Loading");
                    hud.Show();
                    await Task.Delay(1);

                    // Prevent updates to the remote version of the file until
                    // we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    SaveProjectUtility saveProject = new SaveProjectUtility(fileList);

                    string json = JsonSerializer.Serialize(saveProject);
                    //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    //string Base64EncodedString = Convert.ToBase64String(plainTextBytes);
                    //await FileIO.WriteTextAsync(file, Base64EncodedString);

                    await FileIO.WriteTextAsync(file, json);
                    // Let Windows know that we're finished changing the file so
                    // the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        openProjectFile = file;
                        hud.Close();
                        ExportProjectSaved(file.Name);
                    }
                    else
                    {
                        hud.Close();
                        ExportProjectNotSaved(file.Name);
                    }
                }
                else
                {
                    ExportProjectCancelled();
                }
            }
        }
        private async void SaveAsProject_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var filemodel = (FileModel)dataGrid.DataContext;
            ObservableCollection<FileUtility> fileList = filemodel.Files;

            //var ms2model = (Ms2Model)ms2grid.DataContext;
            //ObservableCollection<Ms2Utility> ms2List = ms2model.MS2s;

            if (fileList.Count == 0)
            {
                NoFileToExport();
                return;
            }

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("BuddyProject", new List<string>() { ".BUDDY" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "Project_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".BUDDY";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                SaveProjectLoad hud = new SaveProjectLoad("Loading");
                hud.Show();
                await Task.Delay(1);
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                SaveProjectUtility saveProject = new SaveProjectUtility(fileList);
                string json = JsonSerializer.Serialize(saveProject);

                //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                //string Base64EncodedString = Convert.ToBase64String(plainTextBytes);
                //await FileIO.WriteTextAsync(file, Base64EncodedString);

                await FileIO.WriteTextAsync(file, json);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);

                

                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    openProjectFile = file;
                    hud.Close();
                    ExportProjectSaved(file.Name);
                }
                else
                {
                    hud.Close();
                    ExportProjectNotSaved(file.Name);
                }
            }
            else
            {
                ExportProjectCancelled();
            }
        }
        private async void ExportProjectCancelled()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Operation cancelled.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void ExportProjectSaved(string filename)
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Success",
                Content = "File " + filename + " was saved.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void ExportProjectNotSaved(string filename)
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "File " + filename + " couldn't be saved.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }


        //drag & drop characteristics for drag & drop ms2 comparison feature
        private void MS2CompareTop_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }
        private void MS2CompareTop_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("Records"))
                dragoverMS2 = e.DataView.Properties["Records"] as ObservableCollection<object>;
            foreach (var item in dragoverMS2)
            {
                //this.ms2grid.View.Remove(item as Ms2Utility);
                MS2compareTop = item as Ms2Utility;
                ms2ComparePanel_Top_empty.Visibility = Visibility.Collapsed;
                ms2CompareTop_Scan.Text = MS2compareTop.ScanNumber.ToString();
                ms2CompareTop_Scan.Visibility = Visibility.Visible;
                ms2CompareTop_Mz.Text = MS2compareTop.Mz.ToString();
                ms2CompareTop_Mz.Visibility = Visibility.Visible;
                ms2CompareTop_Rt.Text = MS2compareTop.Rt.ToString();
                ms2CompareTop_Rt.Visibility = Visibility.Visible;
                if (MS2compareTop.Polarity == "P")
                {
                    ms2CompareTop_Polarity.Text = "(+)";
                }
                else
                {
                    ms2CompareTop_Polarity.Text = "(-)";
                }
                //ms2CompareTop_Polarity.Text = MS2compareTop.Polarity;
                ms2CompareTop_Polarity.Visibility = Visibility.Visible;

                if (MS2compareTop != null && MS2compareBottom != null)
                {
                    //emptyMS2CompareImage.Visibility = Visibility.Collapsed;
                    PlotMS2Comparison();
                }
            }
        }
        private void MS2CompareBottom_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }
        private void MS2CompareBottom_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("Records"))
                dragoverMS2 = e.DataView.Properties["Records"] as ObservableCollection<object>;
            foreach (var item in dragoverMS2)
            {
                //this.ms2grid.View.Remove(item as Ms2Utility);
                MS2compareBottom = item as Ms2Utility;
                ms2ComparePanel_Bottom_empty.Visibility = Visibility.Collapsed;
                ms2CompareBottom_Scan.Text = MS2compareBottom.ScanNumber.ToString();
                ms2CompareBottom_Scan.Visibility = Visibility.Visible;
                ms2CompareBottom_Mz.Text = MS2compareBottom.Mz.ToString();
                ms2CompareBottom_Mz.Visibility = Visibility.Visible;
                ms2CompareBottom_Rt.Text = MS2compareBottom.Rt.ToString();
                ms2CompareBottom_Rt.Visibility = Visibility.Visible;
                if (MS2compareBottom.Polarity == "P")
                {
                    ms2CompareBottom_Polarity.Text = "(+)";
                }
                else
                {
                    ms2CompareBottom_Polarity.Text = "(-)";
                }
                //ms2CompareBottom_Polarity.Text = MS2compareBottom.Polarity;
                ms2CompareBottom_Polarity.Visibility = Visibility.Visible;

                if (MS2compareTop != null && MS2compareBottom != null)
                {
                    PlotMS2Comparison();
                }
            }
        }
        //plot ms2 spectrum comparison , left panel (Spectral comparison)
        private void PlotMS2Comparison()
        {
            if (MS2compareTop.OriSpectrum == null || MS2compareTop.OriSpectrum.Count == 0 || MS2compareBottom.OriSpectrum == null || MS2compareBottom.OriSpectrum.Count == 0) { return; }

            emptyMS2CompareImage.Visibility = Visibility.Collapsed;

            MS2CompareResult spec = MS2Clustering.DotProduct_MS2Compare(MS2compareTop.OriSpectrum, MS2compareBottom.OriSpectrum,
                (double)localSettings.Values["ms2tol"], (bool)localSettings.Values["ms1tol_ppmON"]);

            var model = new PlotModel { Title = "" };

            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

            for (int i = 0; i < spec.NormMS2x.Count; i++)
            {
                var x = spec.NormMS2x[i].Mz;
                var y = spec.NormMS2x[i].Intensity;
                var size = 2;
                var colorValue = 1;
                scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

                var annotation = new LineAnnotation();
                if (spec.xMatchFragIndex.Contains(i))
                {
                    annotation.Color = OxyColors.Orange;
                }
                else
                {
                    annotation.Color = OxyColors.DarkBlue;
                }
                annotation.MinimumY = 0;
                annotation.MaximumY = spec.NormMS2x[i].Intensity;
                annotation.X = spec.NormMS2x[i].Mz;
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Vertical;
                annotation.StrokeThickness = 1;
                model.Annotations.Add(annotation);
            }
            for (int i = 0; i < spec.NormMS2y.Count; i++)
            {
                var x = spec.NormMS2y[i].Mz;
                var y = spec.NormMS2y[i].Intensity * -1.0;
                var size = 2;
                var colorValue = 1;
                scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

                var annotation = new LineAnnotation();
                if (spec.yMatchFragIndex.Contains(i))
                {
                    annotation.Color = OxyColors.Orange;
                }
                else
                {
                    annotation.Color = OxyColors.DarkBlue;
                }
                annotation.MaximumY = 0;
                annotation.MinimumY = spec.NormMS2y[i].Intensity * -1.0;
                annotation.X = spec.NormMS2y[i].Mz;
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Vertical;
                annotation.StrokeThickness = 1;
                model.Annotations.Add(annotation);
            }

            double maxMZ = Math.Max(spec.NormMS2x.Max(o => o.Mz), spec.NormMS2y.Max(o => o.Mz));
            double minMZ = Math.Min(spec.NormMS2x.Min(o => o.Mz), spec.NormMS2y.Min(o => o.Mz));

            var zeroLine = new LineAnnotation();
            zeroLine.Color = OxyColors.Black;
            zeroLine.Y = 0;
            zeroLine.MinimumX = 0;
            zeroLine.MaximumX = double.MaxValue;
            zeroLine.LineStyle = LineStyle.Solid;
            zeroLine.Type = LineAnnotationType.Horizontal;
            zeroLine.StrokeThickness = 1.5;
            model.Annotations.Add(zeroLine);

            var customAxis = new RangeColorAxis { Key = "customColors" };
            customAxis.AddRange(0, 2000, OxyColors.DarkGray);
            model.Axes.Add(customAxis);
            model.Series.Add(scatterSeries);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "m/z",
                AbsoluteMinimum = Math.Max(minMZ - 30, 0),
                Minimum = Math.Max(minMZ - 30, 0),
                AbsoluteMaximum = maxMZ + 30,
                Maximum = maxMZ + 30,
                TitleFontSize = 15
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Relative Intensity",
                Minimum = -110,
                Maximum = 110,
                TitleFontSize = 15
            });
            MS2ComparePlot.Visibility = Visibility.Visible;
            MS2ComparePlot.Model = model;

            DPscore_ms2compare.Text = Math.Round(spec.Score, 2).ToString();
            matchNum_ms2compare.Text = spec.MatchNumber.ToString();
        }
        // for spectral library search
        private void PlotMS2LibSearchComparison(Ms2Utility item)
        {
            var model = new PlotModel { Title = "" };

            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

            MS2MatchResult matchedMS2DBResult = item.Ms2Matching.ms2MatchingReturns[0];

            double maxIntSpecX = item.OriSpectrum.Max(o => o.Intensity);
            for (int i = 0; i < item.OriSpectrum.Count; i++)
            {
                var x = item.OriSpectrum[i].Mz;
                var y = 100 * item.OriSpectrum[i].Intensity / maxIntSpecX;
                var size = 2;
                var colorValue = 1;
                scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

                var annotation = new LineAnnotation();
                if (matchedMS2DBResult.ms2CompareResult.xMatchFragIndex.Contains(i))
                {
                    annotation.Color = OxyColors.Orange;
                }
                else
                {
                    annotation.Color = OxyColors.DarkBlue;
                }
                annotation.MinimumY = 0;
                annotation.MaximumY = 100 * item.OriSpectrum[i].Intensity / maxIntSpecX;
                annotation.X = item.OriSpectrum[i].Mz;
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Vertical;
                annotation.StrokeThickness = 1;
                model.Annotations.Add(annotation);
            }

            List<RAW_PeakElement> DBSpec = matchedMS2DBResult.matchedDBEntry.MS2Spec;
            double maxIntSpecY = DBSpec.Max(o => o.Intensity);
            for (int i = 0; i < DBSpec.Count; i++)
            {
                var x = DBSpec[i].Mz;
                var y = DBSpec[i].Intensity * -100.0 / maxIntSpecY;
                var size = 2;
                var colorValue = 1;
                scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

                var annotation = new LineAnnotation();
                if (matchedMS2DBResult.ms2CompareResult.yMatchFragIndex.Contains(i))
                {
                    annotation.Color = OxyColors.Orange;
                }
                else
                {
                    annotation.Color = OxyColors.DarkBlue;
                }
                annotation.MaximumY = 0;
                annotation.MinimumY = DBSpec[i].Intensity * -100.0 / maxIntSpecY;
                annotation.X = DBSpec[i].Mz;
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Vertical;
                annotation.StrokeThickness = 1;
                model.Annotations.Add(annotation);
            }

            double maxMZ = Math.Max(item.OriSpectrum.Max(o => o.Mz), DBSpec.Max(o => o.Mz));
            double minMZ = Math.Min(item.OriSpectrum.Min(o => o.Mz), DBSpec.Min(o => o.Mz));

            var zeroLine = new LineAnnotation();
            zeroLine.Color = OxyColors.Black;
            zeroLine.Y = 0;
            zeroLine.MinimumX = 0;
            zeroLine.MaximumX = double.MaxValue;
            zeroLine.LineStyle = LineStyle.Solid;
            zeroLine.Type = LineAnnotationType.Horizontal;
            zeroLine.StrokeThickness = 1.5;
            model.Annotations.Add(zeroLine);

            var customAxis = new RangeColorAxis { Key = "customColors" };
            customAxis.AddRange(0, 2000, OxyColors.DarkGray);
            model.Axes.Add(customAxis);
            model.Series.Add(scatterSeries);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "m/z",
                AbsoluteMinimum = Math.Max(minMZ - 30, 0),
                Minimum = Math.Max(minMZ - 30, 0),
                AbsoluteMaximum = maxMZ + 30,
                Maximum = maxMZ + 30,
                TitleFontSize = 13,
                AxisTitleDistance = 1
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Rel. Int.",
                Minimum = -110,
                Maximum = 110,
                TitleFontSize = 13,
                AxisTitleDistance = 1
            });
            MS2LibSearch_ComparePlot.Visibility = Visibility.Visible;
            MS2LibSearch_ComparePlot.Model = model;

        }
        ObservableCollection<object> dragoverMS2 = new ObservableCollection<object>();


        //Main menu options

        private async void OpenAdvancedSetting(object sender, RoutedEventArgs e)
        {
            var mView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            int newAppViewID = 0;
            await mView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var window = Window.Current;
                var frame = new Frame();

                frame.Navigate(typeof(AdvancedSetting), this);
                window.Content = frame;
                window.Activate();

                newAppViewID = ApplicationView.GetForCurrentView().Id;

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppViewID, ViewSizePreference.Default, appView.Id, ViewSizePreference.Default);
            });
            
        }

        private async void OpenBasicSetting(object sender, RoutedEventArgs e)
        {
            var mView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            int newAppViewID = 0;
            await mView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var window = Window.Current;
                var frame = new Frame();

                frame.Navigate(typeof(BasicSetting), this);
                window.Content = frame;
                window.Activate();
                newAppViewID = ApplicationView.GetForCurrentView().Id;
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppViewID, ViewSizePreference.Default, appView.Id, ViewSizePreference.Default);
            });
        }

        private async void ImportCustomCSV_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var mView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            int newAppViewID = 0;
            await mView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var window = Window.Current;
                var frame = new Frame();

                frame.Navigate(typeof(CustomInput), this);
                window.Content = frame;
                window.Activate();
                newAppViewID = ApplicationView.GetForCurrentView().Id;
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppViewID, ViewSizePreference.Default, appView.Id, ViewSizePreference.Default);
            });
        }

        private async void ImportSingleMS2_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            var mView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            int newAppViewID = 0;
            await mView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var window = Window.Current;                
                var frame = new Frame();
                
                frame.Navigate(typeof(SingleMs2Input), this);
                window.Content = frame;
                window.Activate();
                newAppViewID = ApplicationView.GetForCurrentView().Id;
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppViewID, ViewSizePreference.Default, appView.Id, ViewSizePreference.Default);
            });
        }
        private async void UserManual_Click(object sender, RoutedEventArgs e)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(USERMANUAL_URI);
        }
        private async void About_Click(object sender, RoutedEventArgs e)
        {
            var mView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            int newAppViewID = 0;
            await mView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var window = Window.Current;
                var frame = new Frame();

                frame.Navigate(typeof(About), this);
                window.Content = frame;
                window.Activate();
                newAppViewID = ApplicationView.GetForCurrentView().Id;
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppViewID, ViewSizePreference.Default, appView.Id, ViewSizePreference.Default);
            });
        }
        private async void ExportBatchSummary_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            exportInProgress = true;
            var fileModel = (FileModel)dataGrid.DataContext;
            var ms2Model = (Ms2Model)ms2grid.DataContext;

            int selectedFiles = 0;
            int totalMS2No = 0;
            for (int i = 0; i < fileModel.Files.Count; i++)
            {
                if (fileModel.Files[i].Selected)
                {
                    selectedFiles++;
                }
            }
            for (int i = 0; i < ms2Model.MS2s.Count; i++)
            {
                if (ms2Model.MS2s[i].Candidates != null && ms2Model.MS2s[i].Candidates.Count > 0)
                {
                    totalMS2No++;
                }
            }
            if (selectedFiles == 0)
            {
                NoFileToExport();
                exportInProgress = false;
                return;
            }


            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("BUDDY_Batch_Summary", new List<string>() { ".csv", ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "BUDDY_Batch_Summary_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".csv";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                List<string> outCSV = new List<string>();
                //outCSV.Add("FileIndex," + "ScanNumber," + "Mz," + "RT," + "Polarity," + "Adduct," + "MS1," + "MS2," + "Formula_Rank_1," + "EstimatedFDR," + "MLRScore," + "p_mzErrorRatio,"+
                //    "expfIntRatio,"+ "expfNoRatio,"+ "waf_logFreq,"+ "wal_logFreq,"+ "ispsim,"+ "fDBEsd,"+ "fH2Csd,"+ "fHetero2Csd,"+ "waf_mzErrorRatio,"+ "wf_DBE,"+ "wf_H2C,"+ "wf_Hetero2C");
                //outCSV.Add("FileIndex," + "ScanNumber," + "Mz," + "RT," + "Polarity," + "Adduct," + "MS1," + "MS2," + "Formula_Rank_1," + "EstimatedFDR," + "MLRScore," +
                //            "Connection_to_scan," + "Connections," + "MS2SimilarityScores");
                outCSV.Add("fileIndex," + "index," + "mz," + "RT," + "ionMode," + "adduct," + "MS1mz," + "MS1int," + "MS2mz," + "MS2int," + "seedMetabolite,"  + "metaboliteName," + "InChIKey," + 
                            "identification_MS2SimilarityScore," + "identification_MS2MatchedFragmentCount," + "Formula_Rank_1," + "estimatedFDR," + "MLRScore," +
                            "connection_to_scan," + "connections," + "MS2SimilarityScores");

                batchExportProgress.Visibility = Visibility.Visible;
                exportProgressbar.Minimum = 0;
                exportProgressbar.Maximum = totalMS2No;
                exportProgressbar.Value = 0;
                for (int i = 0; i < fileModel.Files.Count; i++)
                {
                    if (fileModel.Files[i].Selected)
                    {
                        List<Ms2Utility> calculatedMS2InFile = ms2Model.MS2s.Where(o => o.FileIndex == fileModel.Files[i].Index).ToList();
                        //List<Ms2Utility> calculatedMS2InFile = ms2Model.MS2s.Where(o => o.FileIndex == fileModel.Files[i].Index && o.Candidates != null && o.Candidates.Count > 0).ToList();
                        for (int p = 0; p < calculatedMS2InFile.Count; p++)
                        {
                            exportProgressbar.Value++;
                            Ms2Utility currMS2 = calculatedMS2InFile[p];

                            //Debug.WriteLine(currMS2.ScanNumber.ToString());

                            string Rank1Form;
                            string MLRscore;
                            string EstFDR;
                            string metaboliteName = "";
                            string inchikey = "";
                            if (currMS2.SeedMetabolite)
                            {
                                Rank1Form = currMS2.Formula_PC;
                                MLRscore = "";
                                EstFDR = "NA";
                                metaboliteName = currMS2.MetaboliteName.Replace("," , " ");
                                inchikey = currMS2.InChiKey;
                            }
                            else if (currMS2.Candidates == null || currMS2.Candidates.Count == 0)
                            {
                                Rank1Form = "";
                                EstFDR = "";
                                MLRscore = "";
                            }
                            else
                            {
                                Rank1Form = calculatedMS2InFile[p].Candidates[0].p_formula;
                                EstFDR = (1 - calculatedMS2InFile[p].Candidates[0].plattProb).ToString();
                                MLRscore = calculatedMS2InFile[p].Candidates[0].score.ToString();
                            }

                            string Identification_MS2SimilarityScore = "";
                            string Identification_MS2MatchedFragmentCount = "";
                            if (currMS2.SeedMetabolite && currMS2.ms2Matching != null && currMS2.ms2Matching.ms2MatchingReturns.Count > 0)
                            {
                                Identification_MS2SimilarityScore = currMS2.ms2Matching.ms2MatchingReturns[0].ms2CompareResult.Score.ToString();
                                Identification_MS2MatchedFragmentCount = currMS2.ms2Matching.ms2MatchingReturns[0].ms2CompareResult.MatchNumber.ToString();
                            }

                            string ms1mz = "";
                            string ms1int = "";
                            string ms2mz = "";
                            string ms2int = "";
                            if (currMS2.Ms1 != null && currMS2.Ms1.Count > 0)
                            {
                                foreach (RAW_PeakElement ms1ele in currMS2.Ms1)
                                {
                                    ms1mz = ms1mz + Math.Round(ms1ele.Mz, 4) + ";";
                                    ms1int = ms1int + Math.Round(ms1ele.Intensity, 2) + ";";
                                }
                                ms1mz = ms1mz.Remove(ms1mz.Length - 1);
                                ms1int = ms1int.Remove(ms1int.Length - 1);
                            }
                            if (currMS2.oriSpectrum != null && currMS2.oriSpectrum.Count > 0)
                            {
                                foreach (RAW_PeakElement ms2ele in currMS2.oriSpectrum)
                                {
                                    ms2mz = ms2mz + Math.Round(ms2ele.Mz, 4) + ";";
                                    ms2int = ms2int +  Math.Round(ms2ele.Intensity, 2) + ";";
                                }
                                ms2mz = ms2mz.Remove(ms2mz.Length - 1);
                                ms2int = ms2int.Remove(ms2int.Length - 1);
                            }


                            string Connection_to_scan = "";
                            string Connections = "";
                            string MS2SimilarityScores = "";
                            if (currMS2.FeatureConnections != null && currMS2.FeatureConnections.Count > 0)
                            {
                                for (int m = 0; m < currMS2.FeatureConnections.Count; m++)
                                {
                                    if (m == currMS2.FeatureConnections.Count - 1) // the last one, no ";"
                                    {
                                        Connection_to_scan = Connection_to_scan + currMS2.FeatureConnections[m].pairedMs2ScanNumber.ToString();
                                        Connections = Connections + currMS2.FeatureConnections[m].connection.Description + ":" + currMS2.FeatureConnections[m].connection.FormulaChange;
                                        MS2SimilarityScores = MS2SimilarityScores + Math.Round(currMS2.FeatureConnections[m].ms2Similarity, 3).ToString();
                                    }
                                    else
                                    {
                                        Connection_to_scan = Connection_to_scan + currMS2.FeatureConnections[m].pairedMs2ScanNumber.ToString() + ";";
                                        Connections = Connections + currMS2.FeatureConnections[m].connection.Description + ":" + currMS2.FeatureConnections[m].connection.FormulaChange + ";";
                                        MS2SimilarityScores = MS2SimilarityScores + Math.Round(currMS2.FeatureConnections[m].ms2Similarity, 3).ToString() + ";";
                                    }
                                }
                            }


                            //outCSV.Add(fileModel.Files[i].Index + ","+ currMS2.ScanNumber + "," + Math.Round(currMS2.Mz, 4) + "," + Math.Round(currMS2.Rt, 2) + "," +
                            //    currMS2.Polarity + "," + currMS2.Adduct.Formula + "," + ms1 + "," + ms2 + "," + Rank1Form + "," + EstFDR + "," + calculatedMS2InFile[p].Candidates[0].score + "," +
                            //    calculatedMS2InFile[p].Candidates[0].p_mzErrorRatio + "," + calculatedMS2InFile[p].Candidates[0].expfIntRatio + "," + calculatedMS2InFile[p].Candidates[0].expfNoRatio + "," +
                            //    calculatedMS2InFile[p].Candidates[0].waf_logFreq + "," + calculatedMS2InFile[p].Candidates[0].wal_logFreq + "," + calculatedMS2InFile[p].Candidates[0].ispsim + "," +
                            //    calculatedMS2InFile[p].Candidates[0].fDBEsd + "," + calculatedMS2InFile[p].Candidates[0].fH2Csd + "," + calculatedMS2InFile[p].Candidates[0].fHetero2Csd + "," +
                            //    calculatedMS2InFile[p].Candidates[0].waf_mzErrorRatio + "," + calculatedMS2InFile[p].Candidates[0].wf_DBE + "," + calculatedMS2InFile[p].Candidates[0].wf_H2C + "," +
                            //    calculatedMS2InFile[p].Candidates[0].wf_Hetero2C);
                            outCSV.Add(fileModel.Files[i].Index + "," + currMS2.ScanNumber + "," + Math.Round(currMS2.Mz, 4) + "," + Math.Round(currMS2.Rt, 2) + "," +
                                currMS2.Polarity + "," + currMS2.Adduct.Formula + "," +  ms1mz + "," + ms1int + "," + ms2mz + "," + ms2int + ","
                                + currMS2.SeedMetabolite.ToString() + "," + metaboliteName + "," + inchikey + "," + Identification_MS2SimilarityScore + 
                                "," + Identification_MS2MatchedFragmentCount + "," + Rank1Form + "," + EstFDR + "," + MLRscore + "," + Connection_to_scan + "," +
                                Connections + "," + MS2SimilarityScores);
                        }
                    }
                }
                batchExportProgress.Visibility = Visibility.Collapsed;

                await FileIO.WriteLinesAsync(file, outCSV);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    ExportSuccess();
                }
                else
                {
                    ExportProjectCancelled();
                }
            }
            else
            {
                ExportProjectCancelled();
            }

            exportInProgress = false;
        }
        private async void ExportBatchDetailedResults_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            exportInProgress = true;
            var fileModel = (FileModel)dataGrid.DataContext;
            var ms2Model = (Ms2Model)ms2grid.DataContext;
            //bool ms2deisotope_mainpage = (bool)localSettings.Values["ms2Deisotope"];
            //bool ms2denoise_mainpage = (bool)localSettings.Values["ms2Denoise"];

            int selectedFiles = 0;
            for(int i = 0; i < fileModel.Files.Count; i++)
            {
                if(fileModel.Files[i].Selected == true)
                {
                    selectedFiles++;
                }
            }
            if(selectedFiles == 0 )
            {
                NoFileToExport();
                exportInProgress = false;
                return;
            }

            //ExportLoad hud = new ExportLoad("Loading");
            //hud.Show();
            batchExportProgress.Visibility = Visibility.Visible;
            exportProgressbar.Minimum = 0;
            exportProgressbar.Maximum = ms2Model.MS2s.Where(o => o.Selected == true).Count();
            exportProgressbar.Value = 0;
            StorageFolder mainFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("BUDDY_Export", CreationCollisionOption.ReplaceExisting);
            for (int i = 0; i < fileModel.Files.Count; i++)
            {
                if(fileModel.Files[i].Selected)
                {
                    StorageFolder fileFolder = await mainFolder.CreateFolderAsync(fileModel.Files[i].FileName, CreationCollisionOption.ReplaceExisting);
                    //List<Ms2Utility> calculatedMS2InFile = ms2Model.MS2s.Where(o => o.FileIndex == fileModel.Files[i].Index && o.Candidates != null && o.Candidates.Count > 0).ToList();
                    List<Ms2Utility> calculatedMS2InFile = ms2Model.MS2s.Where(o => o.FileIndex == fileModel.Files[i].Index).ToList();
                    StorageFile fileMS2csv = await fileFolder.CreateFileAsync(fileModel.Files[i].FileName.Split(".")[0] + ".csv", CreationCollisionOption.ReplaceExisting);
                    if (fileMS2csv != null)
                    {
                        // Prevent updates to the remote version of the file until
                        // we finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(fileMS2csv);
                        // write to file
                        List<string> outMS2CSV = new List<string>();
                        outMS2CSV.Add("fileIndex," + "index," + "mz," + "RT," + "ionMode," + "adduct," + "MS1mz," + "MS1int," + "MS2mz," + "MS2int," + "seedMetabolite," + "metaboliteName," + "InChIKey," +
                            "identification_MS2SimilarityScore," + "identification_MS2MatchedFragmentCount," + "Formula_Rank_1," + "estimatedFDR," + "MLRScore," +
                            "connection_to_scan," + "connections," + "MS2SimilarityScores");

                        for (int p = 0; p < calculatedMS2InFile.Count; p++)
                        {

                            Ms2Utility currMS2 = calculatedMS2InFile[p];

                            string Rank1Form;
                            string MLRscore;
                            string EstFDR;
                            string metaboliteName = "";
                            string inchikey = "";
                            if (currMS2.SeedMetabolite)
                            {
                                Rank1Form = currMS2.Formula_PC;
                                MLRscore = "";
                                EstFDR = "NA";
                                metaboliteName = currMS2.MetaboliteName.Replace(",", " ");
                                inchikey = currMS2.InChiKey;
                            }
                            else if (currMS2.Candidates == null || currMS2.Candidates.Count == 0)
                            {
                                Rank1Form = "";
                                EstFDR = "";
                                MLRscore = "";
                            }
                            else
                            {
                                Rank1Form = calculatedMS2InFile[p].Candidates[0].p_formula;
                                EstFDR = (1 - calculatedMS2InFile[p].Candidates[0].plattProb).ToString();
                                MLRscore = calculatedMS2InFile[p].Candidates[0].score.ToString();
                            }
                            string Identification_MS2SimilarityScore = "";
                            string Identification_MS2MatchedFragmentCount = "";
                            if (currMS2.SeedMetabolite && currMS2.ms2Matching != null && currMS2.ms2Matching.ms2MatchingReturns.Count > 0)
                            {
                                Identification_MS2SimilarityScore = currMS2.ms2Matching.ms2MatchingReturns[0].ms2CompareResult.Score.ToString();
                                Identification_MS2MatchedFragmentCount = currMS2.ms2Matching.ms2MatchingReturns[0].ms2CompareResult.MatchNumber.ToString();
                            }

                            string ms1mz = "";
                            string ms1int = "";
                            string ms2mz = "";
                            string ms2int = "";
                            if (currMS2.Ms1 != null && currMS2.Ms1.Count > 0)
                            {
                                foreach (RAW_PeakElement ms1ele in currMS2.Ms1)
                                {
                                    ms1mz = ms1mz + Math.Round(ms1ele.Mz, 4) + ";";
                                    ms1int = ms1int + Math.Round(ms1ele.Intensity, 2) + ";";
                                }
                                ms1mz = ms1mz.Remove(ms1mz.Length - 1);
                                ms1int = ms1int.Remove(ms1int.Length - 1);
                            }
                            if (currMS2.oriSpectrum != null && currMS2.oriSpectrum.Count > 0)
                            {
                                foreach (RAW_PeakElement ms2ele in currMS2.oriSpectrum)
                                {
                                    ms2mz = ms2mz + Math.Round(ms2ele.Mz, 4) + ";";
                                    ms2int = ms2int + Math.Round(ms2ele.Intensity, 2) + ";";
                                }
                                ms2mz = ms2mz.Remove(ms2mz.Length - 1);
                                ms2int = ms2int.Remove(ms2int.Length - 1);
                            }
                            
                            

                            string Connection_to_scan = "";
                            string Connections = "";
                            string MS2SimilarityScores = "";
                            if (currMS2.FeatureConnections != null && currMS2.FeatureConnections.Count > 0)
                            {
                                for (int m = 0; m < currMS2.FeatureConnections.Count; m++)
                                {
                                    if (m == currMS2.FeatureConnections.Count - 1) // the last one, no ";"
                                    {
                                        Connection_to_scan = Connection_to_scan + currMS2.FeatureConnections[m].pairedMs2ScanNumber.ToString();
                                        Connections = Connections + currMS2.FeatureConnections[m].connection.Description + ":" + currMS2.FeatureConnections[m].connection.FormulaChange;
                                        MS2SimilarityScores = MS2SimilarityScores + Math.Round(currMS2.FeatureConnections[m].ms2Similarity,3).ToString();
                                    }
                                    else
                                    {
                                        Connection_to_scan = Connection_to_scan + currMS2.FeatureConnections[m].pairedMs2ScanNumber.ToString() + ";";
                                        Connections = Connections + currMS2.FeatureConnections[m].connection.Description + ":" + currMS2.FeatureConnections[m].connection.FormulaChange + ";";
                                        MS2SimilarityScores = MS2SimilarityScores + Math.Round(currMS2.FeatureConnections[m].ms2Similarity, 3).ToString() + ";";
                                    }
                                }
                            }
                            
                            outMS2CSV.Add(fileModel.Files[i].Index + "," + currMS2.ScanNumber + "," + Math.Round(currMS2.Mz, 4) + "," + Math.Round(currMS2.Rt, 2) + "," +
                                currMS2.Polarity + "," + currMS2.Adduct.Formula + "," + ms1mz + "," + ms1int + "," + ms2mz + "," + ms2int + ","
                                + currMS2.SeedMetabolite.ToString() + "," + metaboliteName + "," + inchikey + "," + Identification_MS2SimilarityScore +
                                "," + Identification_MS2MatchedFragmentCount + "," + Rank1Form + "," + EstFDR + "," + MLRscore + "," + Connection_to_scan + "," +
                                Connections + "," + MS2SimilarityScores);
                        }
                        await FileIO.WriteLinesAsync(fileMS2csv, outMS2CSV);
                        // Let Windows know that we're finished changing the file so
                        // the other app can update the remote version of the file.
                        // Completing updates may require Windows to ask for user input.
                        Windows.Storage.Provider.FileUpdateStatus status =
                            await CachedFileManager.CompleteUpdatesAsync(fileMS2csv);
                    }

                    for (int j = 0; j < calculatedMS2InFile.Count; j++)
                    {
                        exportProgressbar.Value++;
                        if (calculatedMS2InFile[j].SeedMetabolite)
                        {
                            continue;
                        }
                        if (calculatedMS2InFile[j].Candidates == null || calculatedMS2InFile[j].Candidates.Count == 0)
                        {
                            continue;
                        }
                        StorageFolder ms2Folder = await fileFolder.CreateFolderAsync("Scan" + calculatedMS2InFile[j].ScanNumber, CreationCollisionOption.ReplaceExisting);
                        List<CandidateUtility> candidatesInMS2 = new List<CandidateUtility>();

                        double sumErrorProbability = 0.0;
                        //double sumScore = calculatedMS2InFile[j].Candidates.Sum(o =>  Math.Exp(o.score));
                        int candidateCount = 1;
                        foreach (Feature ft in calculatedMS2InFile[j].Candidates)
                        {
                            double PlattProbability = ft.plattProb; // calculate Platt calibrated probability
                            sumErrorProbability += 1 - PlattProbability;
                            candidatesInMS2.Add(new CandidateUtility(ft.relevance, ft.p_formula, Math.Round(1e6 * ft.p_mzErrorRatio * ft.mztol / calculatedMS2InFile[j].Mz, 2), 
                                Math.Round(ft.expfNoRatio, 2), Math.Round(ft.expfIntRatio, 2), Math.Round(ft.waf_mzErrorRatio, 2), Math.Round(PlattProbability, 3), 
                                Math.Round(sumErrorProbability / candidateCount, 3), ft.ispsim, ft.theoMS1, ft.cfdf, ft.alignedFormula, Math.Round(ft.score, 2)));
                            candidateCount++;
                        }

                        StorageFile MS2CandidateCSV = await ms2Folder.CreateFileAsync("Scan" + calculatedMS2InFile[j].ScanNumber + "_Candidates" + ".csv", CreationCollisionOption.ReplaceExisting);
                        if (MS2CandidateCSV != null)
                        {
                            // Prevent updates to the remote version of the file until
                            // we finish making changes and call CompleteUpdatesAsync.
                            CachedFileManager.DeferUpdates(MS2CandidateCSV);
                            // write to file
                            List<string> outCandidateCSV = new List<string>();
                            outCandidateCSV.Add("Rank," + "Formula," + "MzError," + "ExpFragNum," + "ExpFragSumInt," + "MLRScore," + "PosteriorProbability," + "EstimatedFDR");
                            foreach (CandidateUtility currCandidate in candidatesInMS2)
                            {
                                outCandidateCSV.Add(currCandidate.Rank + "," + currCandidate.Formula + "," + currCandidate.MzError + "," +
                                    currCandidate.ExpFragNum + "," + currCandidate.ExpFragSumInt + "," + currCandidate.mlrScore + "," +
                                    currCandidate.EstimatedProbability + "," + currCandidate.EstimatedFDR);
                            }
                            await FileIO.WriteLinesAsync(MS2CandidateCSV, outCandidateCSV);
                            // Let Windows know that we're finished changing the file so
                            // the other app can update the remote version of the file.
                            // Completing updates may require Windows to ask for user input.
                            Windows.Storage.Provider.FileUpdateStatus status =
                                await CachedFileManager.CompleteUpdatesAsync(MS2CandidateCSV);
                        }

                        if (calculatedMS2InFile[j].Spectrum == null || calculatedMS2InFile[j].Spectrum.Count == 0)
                        {
                            continue;
                        }
                        for (int k = 0; k < candidatesInMS2.Count; k++)
                        {
                            StorageFolder candidateFolder = await ms2Folder.CreateFolderAsync("Rank" + candidatesInMS2[k].Rank + "_" + candidatesInMS2[k].Formula, CreationCollisionOption.ReplaceExisting);
                            double maxInt = calculatedMS2InFile[j].Spectrum.Max(x => x.Intensity);
                            int ind = 0;
                            List<FragmentUtility> fragmentsInCandidate = new List<FragmentUtility>(); //fragment spectrum for plotting
                            foreach (RAW_PeakElement pk in calculatedMS2InFile[j].Spectrum)
                            {
                                if (calculatedMS2InFile[j].Candidates == null || calculatedMS2InFile[j].Candidates.Count == 0) //no cfdf
                                {
                                    fragmentsInCandidate.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                        Math.Round(pk.Intensity / maxInt * 100, 1), "Unexplained"));
                                }
                                else //cfdf fragment explanation
                                {
                                    List<CFDF> expList = calculatedMS2InFile[j].Candidates[k].cfdf.Where(o => o.expfragindex == ind).ToList(); //get corresponding fragment explanation for first candidate
                                    string expFrag;
                                    if (expList.Count > 0) //fragment explained
                                    {
                                        //recreate new formula
                                        string new_formula = "";
                                        IDictionary<string, int> v = new Dictionary<string, int>();
                                        v.Add("C", expList[0].c_f); //adding a key/value using the Add() method
                                        v.Add("H", expList[0].h_f);
                                        v.Add("B", expList[0].b_f);
                                        v.Add("Br", expList[0].br_f);
                                        v.Add("Cl", expList[0].cl_f);
                                        v.Add("F", expList[0].f_f);
                                        v.Add("I", expList[0].i_f);
                                        v.Add("K", expList[0].k_f);
                                        v.Add("N", expList[0].n_f);
                                        v.Add("Na", expList[0].na_f);
                                        v.Add("O", expList[0].o_f);
                                        v.Add("P", expList[0].p_f);
                                        v.Add("S", expList[0].s_f);
                                        v.Add("Se", expList[0].se_f);
                                        v.Add("Si", expList[0].si_f);
                                        foreach (KeyValuePair<string, int> element in v)
                                        {
                                            if (element.Value > 1)
                                            {
                                                new_formula = new_formula + element.Key + element.Value.ToString();
                                            }
                                            if (element.Value == 1)
                                            {
                                                new_formula = new_formula + element.Key;
                                            }
                                        }
                                        expFrag = new_formula;
                                    }
                                    else //fragment unexplained
                                    {
                                        expFrag = "Unexplained";
                                    }
                                    fragmentsInCandidate.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                        Math.Round(pk.Intensity / maxInt * 100, 1), expFrag));
                                }
                                ind++;
                            }
                            //process precursor fragments
                            if (calculatedMS2InFile[j].PrecursorFragments != null && calculatedMS2InFile[j].PrecursorFragments.Count > 0)
                            {
                                //item.PrecursorFragments = item.PrecursorFragments.OrderBy(o => o.Mz).ToList(); //all fragments with mz > precursor mz

                                // find the fragment closet to the premass
                                int indexInPrecursorFrags = 0;
                                double currMzDiff;
                                bool precursorFragFound = false;
                                if ((bool)localSettings.Values["ms2tol_ppmON"])
                                {
                                    currMzDiff = (double)localSettings.Values["ms2tol"] * calculatedMS2InFile[j].Mz * 1e-6;
                                }
                                else
                                {
                                    currMzDiff = (double)localSettings.Values["ms2tol"];
                                }
                                for (int z = 0; z < calculatedMS2InFile[j].PrecursorFragments.Count; z++)
                                {
                                    if (Math.Abs(calculatedMS2InFile[j].PrecursorFragments[z].Mz - calculatedMS2InFile[j].Mz) < currMzDiff)
                                    {
                                        indexInPrecursorFrags = z;
                                        currMzDiff = Math.Abs(calculatedMS2InFile[j].PrecursorFragments[z].Mz - calculatedMS2InFile[j].Mz);
                                        precursorFragFound = true;
                                    }
                                }
                                int d = 0;
                                foreach (RAW_PeakElement pk in calculatedMS2InFile[j].PrecursorFragments)
                                {
                                    if (d == indexInPrecursorFrags && precursorFragFound) // precursor fragment
                                    {
                                        fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                            Math.Round(pk.Intensity / maxInt * 100, 1), "Precursor"));
                                    }
                                    else
                                    {
                                        fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                            Math.Round(pk.Intensity / maxInt * 100, 1), "Noise"));
                                    }
                                    d++;
                                }
                            }

                            //process noise fragments
                            if (calculatedMS2InFile[j].NoiseFragments != null && calculatedMS2InFile[j].NoiseFragments.Count > 0)
                            {
                                foreach (RAW_PeakElement pk in calculatedMS2InFile[j].NoiseFragments)
                                {
                                    fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                            Math.Round(pk.Intensity / maxInt * 100, 1), "Noise"));
                                }
                            }

                            //process isotope fragments
                            if (calculatedMS2InFile[j].IsotopeFragments != null && calculatedMS2InFile[j].IsotopeFragments.Count > 0)
                            {
                                foreach (RAW_PeakElement pk in calculatedMS2InFile[j].IsotopeFragments)
                                {
                                    fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                            Math.Round(pk.Intensity / maxInt * 100, 1), "Isotope"));
                                }
                            }

                            //modify fragment dataframe index number from 0+ to 1+
                            fragmentsInCandidate = fragmentsInCandidate.OrderBy(o => o.Mz).ToList();
                            for (int u = 0; u < fragmentsInCandidate.Count; u++)
                            {
                                fragmentsInCandidate[u].Index = u + 1;
                            }

                            StorageFile fragmentCSV = await candidateFolder.CreateFileAsync("Rank" + candidatesInMS2[k].Rank + "_" + candidatesInMS2[k].Formula + 
                                "_FragmentExplanation" + ".csv", CreationCollisionOption.ReplaceExisting);
                            if (fragmentCSV != null)
                            {
                                // Prevent updates to the remote version of the file until
                                // we finish making changes and call CompleteUpdatesAsync.
                                CachedFileManager.DeferUpdates(fragmentCSV);
                                // write to file
                                List<string> outFragmentCSV = new List<string>();
                                outFragmentCSV.Add("Index," + "Mz," + "AbsoluteIntensity," + "RelativeIntensity," + "Formula");
                                foreach (FragmentUtility frag in fragmentsInCandidate)
                                {
                                    outFragmentCSV.Add(frag.Index + "," + frag.Mz + "," + frag.Abs_int + "," + frag.Rel_int + "," + frag.Formula);
                                }
                                await FileIO.WriteLinesAsync(fragmentCSV, outFragmentCSV);
                                // Let Windows know that we're finished changing the file so
                                // the other app can update the remote version of the file.
                                // Completing updates may require Windows to ask for user input.
                                Windows.Storage.Provider.FileUpdateStatus status =
                                    await CachedFileManager.CompleteUpdatesAsync(fragmentCSV);
                            }

                        }

                    }
                }
            }

            IReadOnlyList<StorageFile> storageFiles = await storageFolder.GetFilesAsync();
            foreach (StorageFile file in storageFiles)
            {
                if (file.Path.ToString().Contains(".zip"))
                {
                    await file.DeleteAsync();
                }
            }
            //hud.Close();
            batchExportProgress.Visibility = Visibility.Collapsed;
            ZipFile.CreateFromDirectory(mainFolder.Path, storageFolder.Path + @"\BUDDY_BatchExport.zip", CompressionLevel.Optimal, false);
            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(storageFolder.Path + @"\BUDDY_BatchExport.zip");
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder userDestination = await folderPicker.PickSingleFolderAsync();

            if(userDestination != null)
            {
                await zipFile.CopyAsync(userDestination, "BUDDY_BatchExport_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".zip", NameCollisionOption.ReplaceExisting);
                
                ExportSuccess();
            }
            else
            {
                ExportProjectCancelled();
            }
            exportInProgress = false;
            return;

        }

        private async void ExportSingleMS2_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInProgress)
            {
                CalculationInProgress();
                return;
            }
            if (exportInProgress)
            {
                ExportInProgress();
                return;
            }
            exportInProgress = true;
            Ms2Utility item = (Ms2Utility)ms2grid.CurrentItem;
            if(item == null)
            {
                NoFileToExport();
                exportInProgress = false;
                return;
            }
            if (item.SeedMetabolite)
            {
                ExportSeedMetabolite();
                exportInProgress = false;
                return;
            }
            if (item.Candidates == null || item.Candidates.Count == 0)
            {
                NoCandidatesToExport();
                exportInProgress = false;
                return;
            }

            ExportLoad hud = new ExportLoad("Loading");
            hud.Show();
            StorageFolder mainFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("BUDDY_SingleExport", CreationCollisionOption.ReplaceExisting);
            StorageFile fileMS2csv = await mainFolder.CreateFileAsync("Scan_" + item.ScanNumber.ToString() + ".csv", CreationCollisionOption.ReplaceExisting);
            if (fileMS2csv != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(fileMS2csv);
                // write to file
                List<string> outMS2CSV = new List<string>();
                outMS2CSV.Add("ScanNumber," + "Mz," + "Rt," + "Polarity," + "Adduct," + "MS1," + "MS2," +
                            "Connection_to_scan," + "Connections," + "MS2SimilarityScores");
                string ms1 = "";
                string ms2 = "";
                if (item.Ms1 != null && item.Ms1.Count > 0)
                {
                    foreach (RAW_PeakElement ms1ele in item.Ms1)
                    {
                        ms1 = ms1 + Math.Round(ms1ele.Mz, 4) + ":" + Math.Round(ms1ele.Intensity, 2) + ";";
                    }
                }
                List<RAW_PeakElement> allMS2 = new List<RAW_PeakElement>(item.OriSpectrum); //fragment spectrum for plotting

                if (allMS2 != null && allMS2.Count > 0)
                {
                    foreach (RAW_PeakElement ms2ele in allMS2)
                    {
                        ms2 = ms2 + Math.Round(ms2ele.Mz, 4) + ":" + Math.Round(ms2ele.Intensity, 2) + ";";
                    }
                }

                string Connection_to_scan = "";
                string Connections = "";
                string MS2SimilarityScores = "";
                if (item.FeatureConnections != null && item.FeatureConnections.Count > 0)
                {
                    for (int m = 0; m < item.FeatureConnections.Count; m++)
                    {
                        if (m == item.FeatureConnections.Count - 1) // the last one, no ";"
                        {
                            Connection_to_scan = Connection_to_scan + item.FeatureConnections[m].pairedMs2ScanNumber.ToString();
                            Connections = Connections + item.FeatureConnections[m].connection.Description + ":" + item.FeatureConnections[m].connection.FormulaChange;
                            MS2SimilarityScores = MS2SimilarityScores + Math.Round(item.FeatureConnections[m].ms2Similarity, 3).ToString();
                        }
                        else
                        {
                            Connection_to_scan = Connection_to_scan + item.FeatureConnections[m].pairedMs2ScanNumber.ToString() + ";";
                            Connections = Connections + item.FeatureConnections[m].connection.Description + ":" + item.FeatureConnections[m].connection.FormulaChange + ";";
                            MS2SimilarityScores = MS2SimilarityScores + Math.Round(item.FeatureConnections[m].ms2Similarity, 3).ToString() + ";";
                        }
                    }
                }
                outMS2CSV.Add(item.ScanNumber + "," + Math.Round(item.Mz, 4) + "," + Math.Round(item.Rt, 2) + "," +
                    item.Polarity + "," + item.Adduct.Formula + "," + ms1 + "," + ms2 + "," + Connection_to_scan + "," + Connections + "," + MS2SimilarityScores);
                await FileIO.WriteLinesAsync(fileMS2csv, outMS2CSV);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(fileMS2csv);
            }

            List<CandidateUtility> candidatesInMS2 = new List<CandidateUtility>();
            double sumErrorProbability = 0.0;
            //double sumScore = item.Candidates.Sum(o => Math.Exp(o.score));
            int candidateCount = 1;
            foreach (Feature ft in item.Candidates)
            {
                double PlattProbability = ft.plattProb;
                sumErrorProbability += 1 - PlattProbability;
                candidatesInMS2.Add(new CandidateUtility(ft.relevance, ft.p_formula, Math.Round(1e6 * ft.p_mzErrorRatio * ft.mztol / item.Mz, 2), Math.Round(ft.expfNoRatio, 2),
                    Math.Round(ft.expfIntRatio, 2), Math.Round(ft.waf_mzErrorRatio, 2), Math.Round(PlattProbability, 3), Math.Round(sumErrorProbability / candidateCount, 3), ft.ispsim, ft.theoMS1, ft.cfdf, ft.alignedFormula));
                candidateCount++;
            }

            StorageFile MS2CandidateCSV = await mainFolder.CreateFileAsync("Scan" + item.ScanNumber + "_Candidates" + ".csv", CreationCollisionOption.ReplaceExisting);
            if (MS2CandidateCSV != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(MS2CandidateCSV);
                // write to file
                List<string> outCandidateCSV = new List<string>();
                outCandidateCSV.Add("Rank," + "Formula," + "MzError," + "ExpFragNum," + "ExpFragSumInt," + "EstimatedFDR");
                foreach (CandidateUtility currCandidate in candidatesInMS2)
                {
                    outCandidateCSV.Add(currCandidate.Rank + "," + currCandidate.Formula + "," + currCandidate.MzError + "," +
                        currCandidate.ExpFragNum + "," + currCandidate.ExpFragSumInt + "," + currCandidate.EstimatedFDR);
                }
                await FileIO.WriteLinesAsync(MS2CandidateCSV, outCandidateCSV);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(MS2CandidateCSV);
            }

            if (item.Spectrum != null && item.Spectrum.Count > 0)
            {
                for (int k = 0; k < candidatesInMS2.Count; k++)
                {
                    StorageFolder candidateFolder = await mainFolder.CreateFolderAsync("Rank" + candidatesInMS2[k].Rank + "_" + candidatesInMS2[k].Formula, CreationCollisionOption.ReplaceExisting);
                    double maxInt = item.Spectrum.Max(x => x.Intensity);
                    int ind = 0;
                    List<FragmentUtility> fragmentsInCandidate = new List<FragmentUtility>(); //fragment spectrum for plotting
                    foreach (RAW_PeakElement pk in item.Spectrum)
                    {
                        if (item.Candidates == null || item.Candidates.Count == 0) //no cfdf
                        {
                            fragmentsInCandidate.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                Math.Round(pk.Intensity / maxInt * 100, 1), "Unexplained"));
                        }
                        else //cfdf fragment explanation
                        {
                            List<CFDF> expList = item.Candidates[k].cfdf.Where(o => o.expfragindex == ind).ToList(); //get corresponding fragment explanation for first candidate
                            string expFrag;
                            if (expList.Count > 0) //fragment explained
                            {
                                //recreate new formula
                                string new_formula = "";
                                IDictionary<string, int> v = new Dictionary<string, int>();
                                v.Add("C", expList[0].c_f); //adding a key/value using the Add() method
                                v.Add("H", expList[0].h_f);
                                v.Add("B", expList[0].b_f);
                                v.Add("Br", expList[0].br_f);
                                v.Add("Cl", expList[0].cl_f);
                                v.Add("F", expList[0].f_f);
                                v.Add("I", expList[0].i_f);
                                v.Add("K", expList[0].k_f);
                                v.Add("N", expList[0].n_f);
                                v.Add("Na", expList[0].na_f);
                                v.Add("O", expList[0].o_f);
                                v.Add("P", expList[0].p_f);
                                v.Add("S", expList[0].s_f);
                                v.Add("Se", expList[0].se_f);
                                v.Add("Si", expList[0].si_f);
                                foreach (KeyValuePair<string, int> element in v)
                                {
                                    if (element.Value > 1)
                                    {
                                        new_formula = new_formula + element.Key + element.Value.ToString();
                                    }
                                    if (element.Value == 1)
                                    {
                                        new_formula = new_formula + element.Key;
                                    }
                                }
                                expFrag = new_formula;
                            }
                            else //fragment unexplained
                            {
                                expFrag = "Unexplained";
                            }
                            fragmentsInCandidate.Add(new FragmentUtility(ind, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                Math.Round(pk.Intensity / maxInt * 100, 1), expFrag));
                        }
                        ind++;
                    }
                    //process precursor fragments
                    if (item.PrecursorFragments != null && item.PrecursorFragments.Count > 0)
                    {
                        //item.PrecursorFragments = item.PrecursorFragments.OrderBy(o => o.Mz).ToList(); //all fragments with mz > precursor mz

                        // find the fragment closet to the premass
                        int indexInPrecursorFrags = 0;
                        double currMzDiff;
                        bool precursorFragFound = false;
                        if ((bool)localSettings.Values["ms2tol_ppmON"])
                        {
                            currMzDiff = (double)localSettings.Values["ms2tol"] * item.Mz * 1e-6;
                        }
                        else
                        {
                            currMzDiff = (double)localSettings.Values["ms2tol"];
                        }
                        for (int z = 0; z < item.PrecursorFragments.Count; z++)
                        {
                            if (Math.Abs(item.PrecursorFragments[z].Mz - item.Mz) < currMzDiff)
                            {
                                indexInPrecursorFrags = z;
                                currMzDiff = Math.Abs(item.PrecursorFragments[z].Mz - item.Mz);
                                precursorFragFound = true;
                            }
                        }
                        int d = 0;
                        foreach (RAW_PeakElement pk in item.PrecursorFragments)
                        {
                            if (d == indexInPrecursorFrags && precursorFragFound) // precursor fragment
                            {
                                fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Precursor"));
                            }
                            else
                            {
                                fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Noise"));
                            }
                            d++;
                        }
                    }
                    //process noise fragments
                    if (item.NoiseFragments != null && item.NoiseFragments.Count > 0)
                    {
                        foreach (RAW_PeakElement pk in item.NoiseFragments)
                        {
                            fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Noise"));
                        }
                    }
                    //process isotope fragments
                    if (item.IsotopeFragments != null && item.IsotopeFragments.Count > 0)
                    {
                        foreach (RAW_PeakElement pk in item.IsotopeFragments)
                        {
                            fragmentsInCandidate.Add(new FragmentUtility(0, Math.Round(pk.Mz, 4), Math.Round(pk.Intensity, 1),
                                    Math.Round(pk.Intensity / maxInt * 100, 1), "Isotope"));
                        }
                    }

                    //modify fragment dataframe index number from 0+ to 1+
                    fragmentsInCandidate = fragmentsInCandidate.OrderBy(o => o.Mz).ToList();
                    for (int u = 0; u < fragmentsInCandidate.Count; u++)
                    {
                        fragmentsInCandidate[u].Index = u + 1;
                    }

                    StorageFile fragmentCSV = await candidateFolder.CreateFileAsync("Rank" + candidatesInMS2[k].Rank + "_" + candidatesInMS2[k].Formula + "_CandidateExplanation" + ".csv", CreationCollisionOption.ReplaceExisting);
                    if (fragmentCSV != null)
                    {
                        // Prevent updates to the remote version of the file until
                        // we finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(fragmentCSV);
                        // write to file
                        List<string> outFragmentCSV = new List<string>();
                        outFragmentCSV.Add("Index," + "Mz," + "AbsoluteIntensity," + "RelativeIntensity," + "Formula");
                        foreach (FragmentUtility frag in fragmentsInCandidate)
                        {
                            outFragmentCSV.Add(frag.Index + "," + frag.Mz + "," + frag.Abs_int + "," + frag.Rel_int + "," + frag.Formula);
                        }
                        await FileIO.WriteLinesAsync(fragmentCSV, outFragmentCSV);
                        // Let Windows know that we're finished changing the file so
                        // the other app can update the remote version of the file.
                        // Completing updates may require Windows to ask for user input.
                        Windows.Storage.Provider.FileUpdateStatus status =
                            await CachedFileManager.CompleteUpdatesAsync(fragmentCSV);
                    }
                }
            }

            IReadOnlyList<StorageFile> storageFiles = await storageFolder.GetFilesAsync();
            foreach (StorageFile file in storageFiles)
            {
                if (file.Path.ToString().Contains(".zip"))
                {
                    await file.DeleteAsync();
                }
            }
            hud.Close();

            ZipFile.CreateFromDirectory(mainFolder.Path, storageFolder.Path + @"\BUDDY_SingleExport.zip", CompressionLevel.Optimal, false);
            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(storageFolder.Path + @"\BUDDY_SingleExport.zip");
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder userDestination = await folderPicker.PickSingleFolderAsync();

            if(userDestination != null)
            {
                //DateTime localDate = DateTime.Now;
                //string dateString = localDate.ToShortDateString().Replace("/", "");
                //string timeString = localDate.ToShortTimeString().Replace(":", "").Replace(" ", "");
                //await zipFile.CopyAsync(userDestination, "BUDDY_SingleExport_" + timeString + "_" + dateString + ".zip", NameCollisionOption.ReplaceExisting);

                await zipFile.CopyAsync(userDestination, "BUDDY_SingleExport" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".zip", NameCollisionOption.ReplaceExisting);

                ExportSuccess();
            }
            else
            {
                ExportProjectCancelled();
            }
            exportInProgress = false;
            return;
        }

        //popup warning window
        private async void NoFileToExport()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "No file(s) selected to export.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void NoCandidatesToExport()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "No candidate(s) to export.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void ExportSeedMetabolite()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "This is a seed metabolite.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void ExportSuccess()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Success",
                Content = "BUDDY results have been exported.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void NoFileSelected()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Operation Cancelled",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void InvalidLibraryImported()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Invalid MS/MS library. Please make sure that the imported library has intact information for the following: formula, precursorMz, InChIKey and ion mode. " +
                "\n Fiehn HILIC library will be loaded.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void InvalidProjectSelected()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid project file selected.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private async void DuplicateFileSelected()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Warning",
                Content = "Duplicate File Selected",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }

        // Platt scaling function, no metaScore included
        private double PlattScaling(double BuddyScore, double FormLogFreq, int modelType, bool metaScoreBool)
        {
            double calibratedScore;

            if (!metaScoreBool)
            {
                double slope = -1.3;
                double offset = 1.4;
                switch (modelType)
                {
                    case 1:
                        slope = -1.78681128859578;
                        offset = 3.94926958315605;
                        break;
                    case 2:
                        slope = -1.71704010601797;
                        offset = 3.63527877642779;
                        break;
                    case 3:
                        slope = -1.35093981869282;
                        offset = 1.44076244102185;
                        break;
                    case 4:
                        slope = -1.33699459915359;
                        offset = 1.40535352626451;
                        break;
                    case 5:
                        slope = -1.14764046032908;
                        offset = 2.71687515097843;
                        break;
                    case 6:
                        slope = -1.09726734897032;
                        offset = 2.91912543842274;
                        break;
                    case 7:
                        slope = -1.2478847474037451;
                        offset = 3.4905088631332131;
                        break;
                    case 8:
                        slope = -1.196381776480657;
                        offset = 3.6905032633267023;
                        break;
                    default:
                        break;
                }
                calibratedScore = 1 / (1 + Math.Exp(BuddyScore * slope + offset));
            }
            else // meta-score included
            {
                double beta0 = -5.670705;
                double beta1 = 1.526878;
                double beta2 = 1.526878;
                switch (modelType)
                {
                    case 1:
                        beta0 = -5.670705;
                        beta1 = 1.526878;
                        beta2 = 1.526878;
                        break;
                    case 2:
                        beta0 = -5.865154;
                        beta1 = 1.391433;
                        beta2 = 1.37364;
                        break;
                    case 3:
                        beta0 = -3.295004;
                        beta1 = 1.161641;
                        beta2 = 1.097245;
                        break;
                    case 4:
                        beta0 = -3.598122;
                        beta1 = 1.097723;
                        beta2 = 1.207596;
                        break;
                    case 5:
                        beta0 = -3.259866;
                        beta1 = 1.089766;
                        beta2 = 0.1883218;
                        break;
                    case 6:
                        beta0 = -4.4281512;
                        beta1 = 0.9309135;
                        beta2 = 0.5140863;
                        break;
                    case 7:
                        beta0 = -3.96046;
                        beta1 = 1.2011302;
                        beta2 = 0.1769023;
                        break;
                    case 8:
                        beta0 = -4.6450116;
                        beta1 = 1.102872;
                        beta2 = 0.3561116;
                        break;
                    default:
                        break;
                }
                calibratedScore = 1 / (1 + Math.Exp(-BuddyScore * beta1 - FormLogFreq * beta2 - beta0));
            }
            
            return calibratedScore;
        }
        private string MS2FragmentReannotation(bool ppm, double ms2tol, double peakMz, int ionModeFactor, List<int> candidateNeutralForm, List<List<AlignedFormula>> neutralDB)
        {
            string formToReturn = "Unexplained";
            int frag_FDB_Index = 1;
            int frag_FDB_H_Index = 1;

            frag_FDB_Index = (int)((peakMz + E_MASS * ionModeFactor) / GROUP_MZ_RANGE);
            frag_FDB_H_Index = (int)((peakMz - P_MASS * ionModeFactor) / GROUP_MZ_RANGE);

            List<AlignedFormulaEasyVersion> FDB = new List<AlignedFormulaEasyVersion>();
            List<AlignedFormulaEasyVersion> FDB_H = new List<AlignedFormulaEasyVersion>();
            for (int x = frag_FDB_H_Index - 1; x <= frag_FDB_H_Index + 1; x++)
            {
                if (x >= 0 && x < neutralDB.Count) // formstr: H unadjusted
                {
                    List<AlignedFormula> DBToAdd = neutralDB[x].Where(o =>
                        o.c <= candidateNeutralForm[0] &&
                        o.h <= candidateNeutralForm[1] &&
                        o.n <= candidateNeutralForm[2] &&
                        o.o <= candidateNeutralForm[3] &&
                        o.p <= candidateNeutralForm[4] &&
                        o.f <= candidateNeutralForm[5] &&
                        o.cl <= candidateNeutralForm[6] &&
                        o.br <= candidateNeutralForm[7] &&
                        o.i <= candidateNeutralForm[8] &&
                        o.s <= candidateNeutralForm[9] &&
                        o.si <= candidateNeutralForm[10] &&
                        o.b <= candidateNeutralForm[11] &&
                        o.se <= candidateNeutralForm[12] &&
                        o.na <= candidateNeutralForm[13] &&
                        o.k <= candidateNeutralForm[14]).ToList();
                    FDB_H.AddRange(DBToAdd.ConvertAll(o => new AlignedFormulaEasyVersion
                    {
                        formstr_neutral = o.formstr_neutral,
                        mass = o.mass + P_MASS * ionModeFactor,
                        dbe = o.dbe - 0.5 * ionModeFactor,
                        charge = o.charge,
                        c = o.c,
                        h = o.h + 1 * ionModeFactor,
                        n = o.n,
                        o = o.o,
                        p = o.p,
                        f = o.f,
                        cl = o.cl,
                        br = o.br,
                        i = o.i,
                        s = o.s,
                        si = o.si,
                        b = o.b,
                        se = o.se,
                        na = o.na,
                        k = o.k,
                        LogFreq = o.LogFreq
                    }));
                }
            }
            for (int x = frag_FDB_Index - 1; x <= frag_FDB_Index + 1; x++)
            {
                if (x >= 0 && x < neutralDB.Count)
                {
                    List<AlignedFormula> DBToAdd = neutralDB[x].Where(o =>
                        o.c <= candidateNeutralForm[0] &&
                        o.h <= candidateNeutralForm[1] &&
                        o.n <= candidateNeutralForm[2] &&
                        o.o <= candidateNeutralForm[3] &&
                        o.p <= candidateNeutralForm[4] &&
                        o.f <= candidateNeutralForm[5] &&
                        o.cl <= candidateNeutralForm[6] &&
                        o.br <= candidateNeutralForm[7] &&
                        o.i <= candidateNeutralForm[8] &&
                        o.s <= candidateNeutralForm[9] &&
                        o.si <= candidateNeutralForm[10] &&
                        o.b <= candidateNeutralForm[11] &&
                        o.se <= candidateNeutralForm[12] &&
                        o.na <= candidateNeutralForm[13] &&
                        o.k <= candidateNeutralForm[14]).ToList();
                    FDB.AddRange(DBToAdd.ConvertAll(o => new AlignedFormulaEasyVersion
                    {
                        formstr_neutral = o.formstr_neutral,
                        mass = o.mass - E_MASS * ionModeFactor,
                        dbe = o.dbe,
                        charge = o.charge,
                        c = o.c,
                        h = o.h,
                        n = o.n,
                        o = o.o,
                        p = o.p,
                        f = o.f,
                        cl = o.cl,
                        br = o.br,
                        i = o.i,
                        s = o.s,
                        si = o.si,
                        b = o.b,
                        se = o.se,
                        na = o.na,
                        k = o.k,
                        LogFreq = o.LogFreq
                    }));
                }
            }
            FDB.AddRange(FDB_H);

            if (ppm)
            {
                FDB = FDB.Where(o => Math.Abs(o.mass - peakMz) <= peakMz * ms2tol * 1e-6).ToList();
            }
            else
            {
                FDB = FDB.Where(o => Math.Abs(o.mass - peakMz) <= ms2tol).ToList();
            }

            if (FDB.Count > 0)
            {
                double massDiff = 1.0;
                int finalIndex = 0;
                for (int i = 0; i < FDB.Count; i++)
                {
                    if (Math.Abs(FDB[i].mass - peakMz) <= massDiff)
                    {
                        massDiff = Math.Abs(FDB[i].mass - peakMz);
                        finalIndex = i;
                    }
                }

                //recreate new formula
                string new_formula = "";
                IDictionary<string, int> v = new Dictionary<string, int>();
                v.Add("C", FDB[finalIndex].c); //adding a key/value using the Add() method
                v.Add("H", FDB[finalIndex].h);
                v.Add("B", FDB[finalIndex].b);
                v.Add("Br", FDB[finalIndex].br);
                v.Add("Cl", FDB[finalIndex].cl);
                v.Add("F", FDB[finalIndex].f);
                v.Add("I", FDB[finalIndex].i);
                v.Add("K", FDB[finalIndex].k);
                v.Add("N", FDB[finalIndex].n);
                v.Add("Na", FDB[finalIndex].na);
                v.Add("O", FDB[finalIndex].o);
                v.Add("P", FDB[finalIndex].p);
                v.Add("S", FDB[finalIndex].s);
                v.Add("Se", FDB[finalIndex].se);
                v.Add("Si", FDB[finalIndex].si);
                foreach (KeyValuePair<string, int> element in v)
                {
                    if (element.Value > 1)
                    {
                        new_formula = new_formula + element.Key + element.Value.ToString();
                    }
                    if (element.Value == 1)
                    {
                        new_formula = new_formula + element.Key;
                    }
                }
                formToReturn = new_formula;
            }

            return formToReturn;
        }


        private static List<MetaboliteFeature> GlobalOptimization(List<Connection> connections, List<MetaboliteFeature> metaboliteFeatures, int maxCandidateCount, double ms1tol,
            double ms2tol, bool ppm, double RTtol, double MS2SimilarityCutoff, double edgeCoeffA, double abioEdgeCoeffB, double bioEdgeCoeffB) // edgeCoff: A * MS2 similarity + B
        {

            // metabolite feature count
            int FeatureCount = metaboliteFeatures.Count;
            // all the mz list allowed
            List<Connection> bioConnections = connections.Where(o => o.ConnectionType == "biochemical").ToList();
            double[] BiochemConnectionMzList = bioConnections.Select(o => o.MassDiff).ToArray();
            Array.Sort(BiochemConnectionMzList);
            List<Connection> abioConnections = connections.Where(o => o.ConnectionType != "biochemical").ToList();
            double[] AbioticConnectionMzList = abioConnections.Select(o => o.MassDiff).ToArray();
            Array.Sort(AbioticConnectionMzList);
            //double minAllowedMassDiff = connections.Min(o => o.MassDiff);

            //calculatedMS2No.Text = "nodes";
            //await Task.Delay(1);
            List<MILPVariable> mILPVariables = new List<MILPVariable>();
            int variableIndex = 0;
            // first, node variable (count: FeatureCount)
            for (int i = 0; i < FeatureCount; i++)
            {
                if (metaboliteFeatures[i].seedMetabolite) // seed metabolite
                {
                    mILPVariables.Add(new MILPVariable
                    {
                        Node = true,
                        //VariableIndex = variableIndex,
                        MetaboliteFeatureIndex = metaboliteFeatures[i].ms2Index,
                        CandidateIndex = 0,
                        PlattProb = 1,
                        NodeCandidateFormula = metaboliteFeatures[i].formulaGroundTruth
                    });
                    metaboliteFeatures[i].variableIndexMILP = variableIndex;
                    variableIndex++;
                }
                else // non-seed
                {
                    int featureReserved = 0;
                    if (metaboliteFeatures[i].features.Count == 1)
                    {
                        featureReserved = 1;
                    }
                    else
                    {
                        featureReserved = metaboliteFeatures[i].features.Count(o => o.plattProb >= (metaboliteFeatures[i].features[0].plattProb - 0.5));
                    }

                    featureReserved = Math.Min(featureReserved, maxCandidateCount);
                    metaboliteFeatures[i].featureReservedForMILP = featureReserved;

                    //double sumPlattProb = thisMetaboliteFeatureCandidates.Sum(o => o.plattProb);
                    for (int j = 0; j < featureReserved; j++) // candidates within a metabolite feature
                    {
                        //mILPVariables.Add(new MILPVariable { Node = true, VariableIndex = variableIndex, MetaboliteFeatureIndex = i,
                        //                                     CandidateIndex = j, NormPlattProb = thisMetaboliteFeatureCandidates[j].plattProb / sumPlattProb,
                        //                                     NodeCandidateFormula = thisMetaboliteFeatureCandidates[j].p_formula});
                        mILPVariables.Add(new MILPVariable
                        {
                            Node = true,
                            //VariableIndex = variableIndex,
                            MetaboliteFeatureIndex = metaboliteFeatures[i].ms2Index,
                            CandidateIndex = j,
                            PlattProb = metaboliteFeatures[i].features[j].plattProb,
                            NodeCandidateFormula = metaboliteFeatures[i].features[j].p_formula
                        });
                        metaboliteFeatures[i].features[j].VariableIndexInMILP = variableIndex;
                        variableIndex++;
                    }
                }
            }
            int nodeCount = mILPVariables.Count;

            //precalculate GNPS score
            List<MILPFeaturePair> featurePairs = new List<MILPFeaturePair>();
            for (int i = 0; i < (FeatureCount - 1); i++)
            {
                for (int j = i + 1; j < FeatureCount; j++)
                {
                    bool featureAMzLargerBool = true;
                    if (metaboliteFeatures[i].mz < metaboliteFeatures[j].mz)
                    {
                        featureAMzLargerBool = false;
                    }
                    bool rtMatchedBool = false;
                    if (Math.Abs(metaboliteFeatures[i].rt - metaboliteFeatures[j].rt) <= RTtol) // should be in minutes
                    {
                        rtMatchedBool = true;
                    }
                    featurePairs.Add(new MILPFeaturePair() { AFeatureIndex = i, BFeatureIndex = j, FeatureAMzLarger = featureAMzLargerBool, RtMatched = rtMatchedBool, ValidPair = false});
                }
            }

            // parallel for each
            // Source must be array or IList.
            var source = Enumerable.Range(0, featurePairs.Count).ToArray();
            // Partition the entire source array.
            var rangePartitioner = Partitioner.Create(0, source.Length);
            // Loop over the partitions in parallel.
            Parallel.ForEach(rangePartitioner,
                new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.8) * 1.0)) },
                (range, loopState) =>
            {
                // Loop over each range element without a delegate invocation.
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    MILPFeaturePair featurePair = featurePairs[i];
                    if (metaboliteFeatures[featurePair.AFeatureIndex].ms2.Count > 0 && metaboliteFeatures[featurePair.BFeatureIndex].ms2.Count > 0)
                    {
                        featurePair.bothValidMS2 = true;
                        featurePair.GnpsScore = MS2Compare.GNPS_MS2Compare_forMILP(metaboliteFeatures[featurePair.AFeatureIndex].ms2, metaboliteFeatures[featurePair.AFeatureIndex].mz,
                            metaboliteFeatures[featurePair.BFeatureIndex].ms2, metaboliteFeatures[featurePair.BFeatureIndex].mz, ms2tol, ppm);

                        //if (featurePair.RtMatched) // calculate Reverse dot product
                        //{
                        //    featurePair.RdpScore = MS2Compare.ReverseDotProduct_MS2Compare_forMILP(metaboliteFeatures[featurePair.AFeatureIndex].ms2, metaboliteFeatures[featurePair.BFeatureIndex].ms2,
                        //                                ms2tol, ppm, featurePair.FeatureAMzLarger);
                        //    if (featurePair.RdpScore >= 0.9)
                        //    {
                        //        featurePair.ValidPair = true;
                        //        continue;
                        //    }
                        //}
                        if (featurePair.GnpsScore >= MS2SimilarityCutoff) 
                        {
                            featurePair.ValidPair = true;
                        }
                    }
                    else
                    {
                        featurePair.ValidPair = true;
                    }
                }
            });

            // ms2 GNPS score cutoff
            featurePairs = featurePairs.Where(o => o.ValidPair).ToList();


            // edge variable
            Parallel.ForEach(featurePairs,
                new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.8) * 1.0)) },
                featurePair =>
            {
                int i = featurePair.AFeatureIndex;
                int j = featurePair.BFeatureIndex;

                bool featureAMzLarger = featurePair.FeatureAMzLarger;

                bool rtMatched = featurePair.RtMatched;

                int MetaboliteFeatureACandidatesCount;
                int MetaboliteFeatureBCandidatesCount;
                // i (A)
                if (metaboliteFeatures[i].seedMetabolite)
                {
                    MetaboliteFeatureACandidatesCount = 1;
                }
                else
                {
                    MetaboliteFeatureACandidatesCount = metaboliteFeatures[i].featureReservedForMILP;
                }
                // j (B)
                if (metaboliteFeatures[j].seedMetabolite)
                {
                    MetaboliteFeatureBCandidatesCount = 1;
                }
                else
                {
                    MetaboliteFeatureBCandidatesCount = metaboliteFeatures[j].featureReservedForMILP;
                }
                // loop over candidates
                for (int m = 0; m < MetaboliteFeatureACandidatesCount; m++)
                {
                    string MetaboliteFeatureACandidateFormula;
                    int MetaboliteFeatureACandidateVariableIndex;
                    FormulaElement MetaboliteFeatureACandFormEle;
                    if (metaboliteFeatures[i].seedMetabolite)
                    {
                        MetaboliteFeatureACandidateFormula = metaboliteFeatures[i].formulaGroundTruth;
                        MetaboliteFeatureACandidateVariableIndex = metaboliteFeatures[i].variableIndexMILP;
                        MetaboliteFeatureACandFormEle = metaboliteFeatures[i].seedFormulaElement;
                    }
                    else
                    {
                        MetaboliteFeatureACandidateFormula = metaboliteFeatures[i].features[m].p_formula;
                        MetaboliteFeatureACandidateVariableIndex = metaboliteFeatures[i].features[m].VariableIndexInMILP;
                        MetaboliteFeatureACandFormEle = metaboliteFeatures[i].features[m].formulaElement;
                    }
                    for (int n = 0; n < MetaboliteFeatureBCandidatesCount; n++)
                    {
                        string MetaboliteFeatureBCandidateFormula;
                        int MetaboliteFeatureBCandidateVariableIndex;
                        FormulaElement MetaboliteFeatureBCandFormEle;
                        if (metaboliteFeatures[j].seedMetabolite)
                        {
                            MetaboliteFeatureBCandidateFormula = metaboliteFeatures[j].formulaGroundTruth;
                            MetaboliteFeatureBCandidateVariableIndex = metaboliteFeatures[j].variableIndexMILP;
                            MetaboliteFeatureBCandFormEle = metaboliteFeatures[j].seedFormulaElement;
                        }
                        else
                        {
                            MetaboliteFeatureBCandidateFormula = metaboliteFeatures[j].features[n].p_formula;
                            MetaboliteFeatureBCandidateVariableIndex = metaboliteFeatures[j].features[n].VariableIndexInMILP;
                            MetaboliteFeatureBCandFormEle = metaboliteFeatures[j].features[n].formulaElement;
                        }

                        FormulaDiffOutput formulaDiffOutput = MILPVariable.FormulaCompare(MetaboliteFeatureACandidateFormula, MetaboliteFeatureBCandidateFormula, MetaboliteFeatureACandFormEle,
                                                                                            MetaboliteFeatureBCandFormEle, featureAMzLarger);
                        if (formulaDiffOutput == null) { continue; }


                        List<Connection> thisConnection = new List<Connection>();
                        Connection edgeConnectionToAdd = new Connection();

                        bool ISF = false;
                        if (formulaDiffOutput.formulaSame && rtMatched)// same formula (M, neutral), only consider abiotic connections
                        {
                            // match the formula change string
                            thisConnection = abioConnections.Where(o => o.FormulaChange == formulaDiffOutput.formulaDiff).ToList();
                        }
                        else
                        {
                            if (rtMatched && formulaDiffOutput.subForm) // RT, subformula, (ms2Rdp 0.8,) ms2 frag contain
                            {
                                // further consider possible ISF
                                bool MS2FragContain = MILPVariable.MS2FragContain(metaboliteFeatures[i].mz, metaboliteFeatures[j].mz, metaboliteFeatures[i].ms2,
                                                                                    metaboliteFeatures[j].ms2, featureAMzLarger, ppm, ms1tol, ms2tol, 0.05);
                                if (MS2FragContain)
                                {
                                    ISF = true;
                                    edgeConnectionToAdd = new Connection()
                                    {
                                        ConnectionType = "abiotic",
                                        Description = "in-source fragment",
                                        FormulaChange = formulaDiffOutput.formulaDiff,
                                        MassDiff = Math.Abs(metaboliteFeatures[i].mz - metaboliteFeatures[j].mz)
                                    };
                                }
                                else // bio
                                {
                                    thisConnection = bioConnections.Where(o => o.FormulaChange == formulaDiffOutput.formulaDiff).ToList();
                                }
                            }
                            else
                            {
                                thisConnection = bioConnections.Where(o => o.FormulaChange == formulaDiffOutput.formulaDiff).ToList();
                            }
                        }

                        if (!ISF)
                        {
                            if (thisConnection.Count == 0) { continue; }
                            else
                            {
                                edgeConnectionToAdd = thisConnection.First();
                                if (edgeConnectionToAdd.Description == "thiol to sulfonic acid" && MetaboliteFeatureACandFormEle.s == 0) { continue; }
                                if (edgeConnectionToAdd.Description == "nitrification" && MetaboliteFeatureACandFormEle.n == 0) { continue; }
                            }
                        }

                        bool abioticEdge = edgeConnectionToAdd.ConnectionType == "abiotic";

                        lock (mILPVariables) mILPVariables.Add(new MILPVariable
                        {
                            Node = false,
                            //VariableIndex = variableIndex,
                            EdgeMetaboliteFeatureIndexA = metaboliteFeatures[i].ms2Index,
                            EdgeCandidateIndexA = m,
                            EdgeCandidateFormulaA = MetaboliteFeatureACandidateFormula,
                            EdgeCandidateTotalIndexA = MetaboliteFeatureACandidateVariableIndex,
                            EdgeMetaboliteFeatureIndexB = metaboliteFeatures[j].ms2Index,
                            EdgeCandidateIndexB = n,
                            EdgeCandidateFormulaB = MetaboliteFeatureBCandidateFormula,
                            EdgeCandidateTotalIndexB = MetaboliteFeatureBCandidateVariableIndex,
                            AbioticConnection = abioticEdge,
                            RtMatched = rtMatched,
                            EdgeConnection = edgeConnectionToAdd,
                            MS2SimilarityScore = featurePair.GnpsScore
                        });
                    }
                }
            });

            // free up memory
            featurePairs = null;

            List<MILPVariable> edgeVariable = mILPVariables.Where(o => o.Node == false).ToList();
            int edgeCount = edgeVariable.Count;
            int varCount = mILPVariables.Count;

            // create MILP data model
            //List<int[]> constraintCoeffsArray = new List<int[]>();
            //for (int i = 0; i < (FeatureCount + edgeCount * 2); i++)
            //{
            //    constraintCoeffsArray.Add(new int[mILPVariables.Count]);
            //}

            //int tmpColIndex = 0;
            //for (int i = 0; i < FeatureCount; i++)
            //{
            //    if (metaboliteFeatures[i].seedMetabolite)
            //    {
            //        constraintCoeffsArray[i][tmpColIndex] = 1;
            //        tmpColIndex++;
            //    }
            //    else
            //    {
            //        for (int j = 0; j < metaboliteFeatures[i].featureReservedForMILP; j++)
            //        {
            //            //constraintCoeffsArray[i, j + tmpColIndex] = 1;
            //            constraintCoeffsArray[i][j + tmpColIndex] = 1;
            //        }
            //        tmpColIndex += metaboliteFeatures[i].featureReservedForMILP;
            //    }
            //}

            //tmpColIndex = mILPVariables.Count - edgeCount;
            //for (int i = 0; i < edgeCount; i++)
            //{
            //    constraintCoeffsArray[i + FeatureCount][tmpColIndex] = 1;
            //    constraintCoeffsArray[i + FeatureCount + edgeCount][tmpColIndex] = 1;

            //    int thisNodeAIndex = edgeVariable[i].EdgeCandidateTotalIndexA;
            //    int thisNodeBIndex = edgeVariable[i].EdgeCandidateTotalIndexB;

            //    constraintCoeffsArray[i + FeatureCount][thisNodeAIndex] = -1;
            //    constraintCoeffsArray[i + FeatureCount + edgeCount][thisNodeBIndex] = -1;
            //    tmpColIndex++;
            //}


            int[] lowBoundArray = new int[(FeatureCount + edgeCount * 2)];
            int[] upBoundArray = new int[(FeatureCount + edgeCount * 2)];
            for (int i = 0; i < FeatureCount; i++)
            {
                lowBoundArray[i] = 1;
                upBoundArray[i] = 1;
            }
            for (int i = FeatureCount; i < (FeatureCount + edgeCount * 2); i++)
            {
                lowBoundArray[i] = -1;
                upBoundArray[i] = 0;
            }


            double[] objCoeffsArray = new double[varCount];
            for (int i = 0; i < (varCount - edgeCount); i++)
            {
                objCoeffsArray[i] = mILPVariables[i].PlattProb;
            }
            for (int i = varCount - edgeCount; i < varCount; i++)
            {
                if (mILPVariables[i].AbioticConnection) // abiotic
                {
                    objCoeffsArray[i] = edgeCoeffA * mILPVariables[i].MS2SimilarityScore + abioEdgeCoeffB;
                }
                else
                {
                    objCoeffsArray[i] = edgeCoeffA * mILPVariables[i].MS2SimilarityScore + bioEdgeCoeffB;
                }
            }

            // free up memory
            mILPVariables = null;

            //DataModel dataModelMILP = new DataModel
            //{
            //    //ConstraintCoeffs = constraintCoeffsArray,
            //    LowerBounds = lowBoundArray,
            //    UpperBounds = upBoundArray,
            //    ObjCoeffs = objCoeffsArray,
            //    NumConstraints = FeatureCount + edgeCount * 2,
            //    NumVars = mILPVariables.Count
            //};


            // Create the linear solver with the SCIP backend.
            Solver solver = Solver.CreateSolver("SCIP");

            Variable[] x = new Variable[varCount];
            for (int j = 0; j < varCount; j++)
            {
                x[j] = solver.MakeIntVar(0.0, 1.0, $"x_{j}");
            }


            //for (int i = 0; i < dataModelMILP.NumConstraints; ++i)
            //{
            //    Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(dataModelMILP.LowerBounds[i], dataModelMILP.UpperBounds[i], "");
            //    for (int j = 0; j < dataModelMILP.NumVars; ++j)
            //    {
            //        constraint.SetCoefficient(x[j], dataModelMILP.ConstraintCoeffs[i][j]);
            //    }
            //}

            int tmpColIndex = 0;
            for (int i = 0; i < FeatureCount; ++i)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(lowBoundArray[i], upBoundArray[i], "");
                //for (int j = 0; j < varCount; ++j)
                //{
                //    constraint.SetCoefficient(x[j], 0); // preset to 0 for all
                //}
                if (metaboliteFeatures[i].seedMetabolite)
                {
                    constraint.SetCoefficient(x[tmpColIndex], 1);
                    tmpColIndex++;
                }
                else
                {
                    for (int j = 0; j < metaboliteFeatures[i].featureReservedForMILP; j++)
                    {
                        constraint.SetCoefficient(x[j + tmpColIndex], 1);
                    }
                    tmpColIndex += metaboliteFeatures[i].featureReservedForMILP;
                }
            }

            tmpColIndex = varCount - edgeCount;
            for (int i = 0; i < edgeCount; i++)
            {
                Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(lowBoundArray[i + FeatureCount], upBoundArray[i + FeatureCount], "");
                //for (int j = 0; j < varCount; ++j)
                //{
                //    constraint.SetCoefficient(x[j], 0); // preset to 0 for all
                //}
                constraint.SetCoefficient(x[tmpColIndex], 1);
                int thisNodeAIndex = edgeVariable[i].EdgeCandidateTotalIndexA;
                constraint.SetCoefficient(x[thisNodeAIndex], -1);

                Google.OrTools.LinearSolver.Constraint constraint2 = solver.MakeConstraint(lowBoundArray[i + FeatureCount + edgeCount], upBoundArray[i + FeatureCount + edgeCount], "");
                //for (int j = 0; j < varCount; ++j)
                //{
                //    constraint2.SetCoefficient(x[j], 0); // preset to 0 for all
                //}
                constraint2.SetCoefficient(x[tmpColIndex], 1);
                int thisNodeBIndex = edgeVariable[i].EdgeCandidateTotalIndexB;
                constraint2.SetCoefficient(x[thisNodeBIndex], -1);

                tmpColIndex++;
            }


            Objective objective = solver.Objective();
            for (int j = 0; j < varCount; ++j)
            {
                objective.SetCoefficient(x[j], objCoeffsArray[j]);
            }
            objective.SetMaximization();
            Solver.ResultStatus resultStatus = solver.Solve();

            int tmpIndex = 0;
            for (int i = 0; i < FeatureCount; i++)
            {
                if (metaboliteFeatures[i].seedMetabolite)
                {
                    tmpIndex++;
                }
                else
                {
                    for (int j = 0; j < metaboliteFeatures[i].featureReservedForMILP; j++) // candidates within a metabolite feature
                    {
                        metaboliteFeatures[i].features[j].MILPScore = x[tmpIndex].SolutionValue();
                        tmpIndex++;
                    }
                }
            }

            // valid edges
            List<MILPVariable> validEdge = new List<MILPVariable>();
            for (int i = 0; i < edgeCount; i++)
            {
                if (x[tmpIndex + i].SolutionValue() > 0.5)
                {
                    validEdge.Add(edgeVariable[i]);
                }
            }
            // fill in every metabolic feature
            for (int i = 0; i < metaboliteFeatures.Count; i++)
            {
                List<FeatureConnection> featureConnections = new List<FeatureConnection>();
                List<MILPVariable> thisValidEdgePartA = validEdge.Where(o => o.EdgeMetaboliteFeatureIndexA == metaboliteFeatures[i].ms2Index).ToList();
                List<MILPVariable> thisValidEdgePartB = validEdge.Where(o => o.EdgeMetaboliteFeatureIndexB == metaboliteFeatures[i].ms2Index).ToList();
                for (int j = 0; j < thisValidEdgePartA.Count; j++)
                {
                    featureConnections.Add(new FeatureConnection
                    {
                        connection = thisValidEdgePartA[j].EdgeConnection,
                        pairedMs2Index = thisValidEdgePartA[j].EdgeMetaboliteFeatureIndexB, //index in ms2model, not scan number; ms2model.MS2s[ms2Index]
                        ms2Similarity = thisValidEdgePartA[j].MS2SimilarityScore
                    });
                }
                for (int j = 0; j < thisValidEdgePartB.Count; j++)
                {
                    featureConnections.Add(new FeatureConnection
                    {
                        connection = thisValidEdgePartB[j].EdgeConnection,
                        pairedMs2Index = thisValidEdgePartB[j].EdgeMetaboliteFeatureIndexA, //index in ms2model, not scan number; ms2model.MS2s[ms2Index]
                        ms2Similarity = thisValidEdgePartB[j].MS2SimilarityScore
                    });
                }
                metaboliteFeatures[i].featureConnections = featureConnections;
            }

            return metaboliteFeatures;
        }

    }

    //Overwrite drag & drop characteristics for ms2 spectrum comparison feature
    public class GridRowDragDropControllerExt : GridRowDragDropController
    {
        ObservableCollection<object> draggingRecords = new ObservableCollection<object>();

        /// <summary>
        /// Occurs when the input system reports an underlying dragover event with this element as the potential drop target.
        /// </summary>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DragEventArgs">DragEventArgs</see> that contains the event data.</param>
        /// <param name="rowColumnIndex">Specifies the row column index based on the mouse point.</param>
        protected override void ProcessOnDragOver(DragEventArgs args, RowColumnIndex rowColumnIndex)
        {
            if (args.DataView.Properties.ContainsKey("DraggedItem"))
                draggingRecords = args.DataView.Properties["DraggedItem"] as ObservableCollection<object>;

            else

                draggingRecords = args.DataView.Properties["Records"] as ObservableCollection<object>;

            if (draggingRecords == null)
                return;


            //To get the drop position of the record            
            var dropPosition = GetDropPosition(args, rowColumnIndex, draggingRecords);

            // based on drop positon, the popup will be shown
            if (dropPosition == DropPosition.None)
            {
                CloseDragIndicators();
                args.AcceptedOperation = DataPackageOperation.None;
                args.DragUIOverride.Caption = "Can't drop here";
                return;
            }

            else if (dropPosition == DropPosition.DropAbove)
            {
                if (draggingRecords != null && draggingRecords.Count > 1)
                    args.DragUIOverride.Caption = "Drop these " + draggingRecords.Count + "  rows above";
                else
                {
                    args.AcceptedOperation = DataPackageOperation.Copy;

                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = true;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = "Drop above";
                }
            }

            else
            {
                if (draggingRecords != null && draggingRecords.Count > 1)
                    args.DragUIOverride.Caption = "Drop these " + draggingRecords.Count + "  rows below";
                else
                    args.DragUIOverride.Caption = "Drop below";
            }
            // to accept and move the records. 
            args.AcceptedOperation = DataPackageOperation.Move;

            //To Show the up and down indicators while dragging the row
            ShowDragIndicators(dropPosition, rowColumnIndex, args);
            args.Handled = true;
        }

        ListView listview;

        /// <summary>
        /// Occurs when the input system reports an underlying drop event with this element as the drop target.
        /// </summary>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.DragEventArgs">DragEventArgs</see> that contains the event data.</param>
        /// <param name="rowColumnIndex">Specifies the row column index based on the mouse point.</param>
        protected override void ProcessOnDrop(DragEventArgs args, RowColumnIndex rowColumnIndex)
        {
            listview = null;

            if (args.DataView.Properties.ContainsKey("ListView"))
                listview = args.DataView.Properties["ListView"] as ListView;

            if (!DataGrid.SelectionController.CurrentCellManager.CheckValidationAndEndEdit())
                return;

            //To get the drop position of the record
            var dropPosition = GetDropPosition(args, rowColumnIndex, draggingRecords);
            // based on drop positon, the popup will be shown
            if (dropPosition == DropPosition.None)
                return;

            var droppingRecordIndex = this.DataGrid.ResolveToRecordIndex(rowColumnIndex.RowIndex);

            if (droppingRecordIndex < 0)
                return;

            // to insert the dragged records based on dropping records index 
            foreach (var record in draggingRecords)
            {
                if (listview != null)
                {
                    (listview.ItemsSource as ObservableCollection<Ms2Utility>).Remove(record as Ms2Utility);
                    var sourceCollection = this.DataGrid.View.SourceCollection as IList;

                    if (dropPosition == DropPosition.DropBelow)
                        sourceCollection.Insert(droppingRecordIndex + 1, record);
                    else
                        sourceCollection.Insert(droppingRecordIndex, record);
                }
                else
                {
                    var draggingIndex = this.DataGrid.ResolveToRowIndex(draggingRecords[0]);

                    if (draggingIndex < 0)
                    {
                        return;
                    }

                    var recordindex = this.DataGrid.ResolveToRecordIndex(draggingIndex);
                    var recordEntry = this.DataGrid.View.Records[recordindex];
                    this.DataGrid.View.Records.Remove(recordEntry);
                    // to insert the dragged records based on dropping records index 
                    if (draggingIndex < rowColumnIndex.RowIndex && dropPosition == DropPosition.DropAbove)
                        this.DataGrid.View.Records.Insert(droppingRecordIndex - 1, this.DataGrid.View.Records.CreateRecord(record));
                    else if (draggingIndex > rowColumnIndex.RowIndex && dropPosition == DropPosition.DropBelow)
                        this.DataGrid.View.Records.Insert(droppingRecordIndex + 1, this.DataGrid.View.Records.CreateRecord(record));
                    else
                        this.DataGrid.View.Records.Insert(droppingRecordIndex, this.DataGrid.View.Records.CreateRecord(record));
                }
            }
            //Closes the Drag arrow indication all the rows
            CloseDragIndicators();
        }
    }



}

