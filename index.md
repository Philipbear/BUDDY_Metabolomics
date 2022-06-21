
# BUDDY User Manual
[![Generic badge](https://img.shields.io/badge/BUDDY-ver_1.0-<COLOR>.svg)](https://github.com/Philipbear/BUDDY_Metabolomics)
![Maintainer](https://img.shields.io/badge/maintainer-Shipei_Xing-blue)

## 1. Introduction
Welcome to BUDDY!

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/AppIcon.png" align="right" width = "150" height = "150">

BUDDY is an open-source cheminformatic software platform developed for MS-based metabolomics research, capitalizing on **bottom-up MS/MS interrogation** and **experiment-specific global peak annotation**.

Bottom-up MS/MS interrogation aims to determine molecular formulae for all metabolic features with significance estimation. Experiment-specific global peak annotation is achieved to select the optimal molecular network considering both individual peak annotations and peak interrelationships. For detailed methodology, please refer to our paper.

Wish you a wonderful journey in BUDDY!

## 2. Installation
BUDDY can be freely downloaded via this [Zenodo link](https://zenodo.org/record/6581871#.YrD2U3bMKbg) under the _Creative Commons Attribution 4.0 International_ license (https://zenodo.org/record/6581871#.YrD2U3bMKbg). 

Unzip and open the downloaded file, right click the “Install.ps1” file and **Run with PowerShell**.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/installation_1.PNG" width = "350" >

Press **Enter** and **Y** to complete the installation. Meanwhile, you will be directed to enable the _Developer Mode_ on your PC.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/installation_2.PNG" width = "600" >

You are all set! BUDDY should appear in your **start** menu! 

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/installation_3.PNG" width = "300" >

## 3. Graphical User Interface
We offer an intuitive graphical user interface for BUDDY. A brief introduction of each panel is provided as below.
<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/GUI.png" width = "950" >

### 3.1 File List
This panel contains all the imported files. You can check any file to show its metabolic features in the **Feature List** panel.
### 3.2 Feature List
This panel contains all the metabolic features within selected files. You can sort each column by clicking on the column name. You can redefine the adduct form for each feature by double clicking the adduct box. To check multiple features, highlight wanted features and click **Select Multiple**. To select or unselect all features shown in the list, click **(Un)Select All**. During the computation, a progress bar will appear in the bottom left corner. After calculation, features labelled with the blue “S” are identified features (according to the set-up metabolite identification threshold).
### 3.3 Functional Viewer
There are two panels in the **Functional Viewer**. **Feature Spot Viewer** and **Spectral Comparison**.
**Feature Spot Viewer** offers a scatter plot in the _m/z_-RT domain. It shows the all the feature spots within the selected file in the **File List**.
**Spectral Comparison** provides a straightforward panel to directly compare two MS/MS spectra. Users can simply drag features with MS/MS from the **Feature List** and drop them here for spectral comparison. In practice, this allows users to quickly check whether two MS/MS are of the same identity or any structural relation.
### 3.4 Candidate List
This panel lists the formula candidates together with their basic information including mass deviations, explained fragment counts & intensities, estimated posterior probabilities and FDRs. Users can highlight each formula candidate to see their corresponding MS/MS explanation quality in the **Result** panel.
The **Databases** list in the right shows in which chemical databases this formula can be found.
### 3.5 MS2 Spectrum
This panel shows the original MS/MS spectrum and some metadata. Users can left click on the peak apex to show _m/z_ & abundance. Hold right click to adjust the spectrum axis range. Double click the scroll wheel to restore the default view.
### 3.6 Result
There are currently three panels within the **Result** module.
**MS2 Library Search** is available for seed metabolites (or identified metabolites), showing mirror plots of MS/MS matching and detailed introductions of the identified molecule. Molecule descriptions are mirrored from _PubChem_ using its API, allowing users to perform manual inspections more conveniently.
**Feature Connection** panel is available when the **experiment-specific global annotation** task is performed. This panel lists all the metabolic features that are connected to the selected feature and their connection types.
**MS2 Explanation Summary** summarizes how the selected formula candidate explains the target MS/MS spectrum in terms of double bond equivalent (DBE) values, hydrogen / C ratios, heteroatom / C ratios and radical fragment ions. These evaluation metrics can be essential to the candidate ranking process.
### 3.7 MS2 Fragment Table
This panel lists all the fragment ions and their subformula annotations. In case that some fragment ions are not annotated within the first-round search, users can apply **MS2 Fragment Reannotation** in **Others** in the advanced settings. The fragment table is sortable.
### 3.8 MS1 Isotope Matching
If MS1 isotope pattern is used for annotation, a mirror image of experimental MS1 isotope and theoretical isotope pattern will be shown here. The detailed MS1 isotope similarity algorithm can be found within the supplementary information of our paper.

## 4. Data import
BUDDY provides both single query import and batch import.
### 4.1 Single import
For single query import, BUDDY offers a pop-up window. Users can click on the **Demo data** to see the input format. At least one of the MS1 isotope and MS/MS spectrum is required for the following annotation.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/import_1.PNG" width = "400" >

### 4.2 Batch import
BUDDY accepts various formats of batch query import, including metabolic feature tables (output by MS-DIAL or custom tables as indicated), mzML files and MGF files.
#### 4.2.1 Import from feature table
If you are using **MS-DIAL** for data preprocessing, you can directly import metabolic feature tables output by MS-DIAL (in **txt** format) into BUDDY. Specifically for feature tables output by MS-DIAL, we offer an addition step of metabolic feature clustering to remove redundant features, which can be set up in the advanced settings.

Otherwise, we also provide a custom feature table format (in **csv**). A template table is downloadable in the pop-up window. Please follow the instructions carefully.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/import_2.PNG" width = "600" >

#### 4.2.2 Import from mzML or MGF file
You can directly import mzML files containing raw data into BUDDY for downstream analysis. At this stage, we only accept data-dependent acquisition (DDA) data and automatically reserve metabolic features with MS/MS collected. Adjacent MS/MS will be grouped by default using the cosine similarity. MS1 isotope patterns will also be extracted. You can change the relevant parameter details in the advanced settings.

MGF files containing multiple MS/MS spectra can also be directly imported into BUDDY. Every MS/MS will be treated separately, and no additional MS/MS grouping is performed.
 
## 5. Settings
### 5.1 Basic settings
**Basic Settings** is a simplified version of **Advanced Settings**, allowing users to change the most important parameters at the first convenience.
<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/basicsettings.PNG" width = "600" >


### 5.2 Advanced settings
#### 5.2.1 General
BUDDY is able to perform three tasks as shown below. **Experiment-specific global annotation** must be performed together with **Bottom-up MS2 interrogation**. Notably, users can choose to apply meta-score inclusion for annotation, which uses metadata of molecular formulae in chemical databases. This could improve the annotation rate for known formulae at the cost of potentially missing discovery of novel molecular formulae.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/advsettings_1.PNG" width = "600" >

#### 5.2.2 MS2 library search
Users can upload their own spectral libraries in **msp** format for metabolite identification. By default, the Fiehn HILIC library will be used. We provide three MS/MS searching algorithms—dot product, reverse dot product and spectral entropy similarity. Users can customize metabolite annotation threshold on MS/MS similarity score, minimum matched fragment count and retention time matching.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/advsettings_2.PNG" width = "600" >

#### 5.2.3 Chemical element
Users can upload their own spectral libraries in **msp** format for metabolite identification. By default, the Fiehn HILIC library will be used. We provide three MS/MS searching algorithms—dot product, reverse dot product and spectral entropy similarity. Users can customize metabolite annotation threshold on MS/MS similarity score, minimum matched fragment count and retention time matching.

<img src = "https://raw.githubusercontent.com/Philipbear/BUDDY_Metabolomics/main/image/advsettings_3.PNG" width = "600" >
