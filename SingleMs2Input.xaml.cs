using BUDDY.FormulaData;
using BUDDY.RawData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public sealed partial class SingleMs2Input : Page
    {
        ObservableCollection<Adduct> adductList = new ObservableCollection<Adduct>();
        public StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public MainPage MainPage { get; private set; }

        public SingleMs2Input()
        {
            this.InitializeComponent();
            this.Loaded += SingleInput_Page_Loaded;

            ionMode_neg_checkbox.IsChecked = false;
            ionMode_pos_checkbox.IsChecked = true;
            ObservableCollection<Adduct> adductListPos;
            //ObservableCollection<Adduct> adductListNeg;

            using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                adductListPos = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
            }
            //using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Open))
            //{
            //    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    adductListNeg = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
            //}
            for (int i = 0; i < adductListPos.Count; i++)
            {
                adductList.Add(adductListPos[i]);
            }
            adduct_selection.SelectedIndex = 0;

        }

        private void SingleInput_Page_Loaded(object sender, RoutedEventArgs e)
        {
            var s = ApplicationView.GetForCurrentView();
            s.TryResizeView(new Windows.Foundation.Size { Width = 670.0, Height = 750.0 });
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            Ms2Utility newMS2 = new Ms2Utility();

            //if((bool)ionMode_pos_checkbox.IsChecked == true && ((Adduct)adduct_selection.SelectedItem).Mode == "N")
            //{
            //    ReadError();
            //    return;
            //}
            //if ((bool)ionMode_pos_checkbox.IsChecked == false && ((Adduct)adduct_selection.SelectedItem).Mode == "P")
            //{
            //    ReadError();
            //    return;
            //}

            if (ms1_box.Text == "" && ms2_box.Text == "")
            {
                ReadError();
                return;
            }
            double preMZ;
            bool parsePreMZ = double.TryParse(preMz_box.Text, out preMZ);
            if (parsePreMZ && preMZ >= 0)
            {
                newMS2.Mz = Math.Round(preMZ, 4);
            }
            else
            {
                ReadError();
                return;
            }
            if(ionMode_pos_checkbox.IsChecked == true)
            {
                newMS2.Polarity = "P";
            }
            else
            {
                newMS2.Polarity = "N";
            }

            newMS2.Adduct = new Adduct(((Adduct)adduct_selection.SelectedItem).Formula);
            List<RAW_PeakElement> currms1 = new List<RAW_PeakElement>();
            List<RAW_PeakElement> currms2 = new List<RAW_PeakElement>();

            if (ms1_box.Text == "")
            {
                newMS2.Ms1 = currms1;
            }
            else
            {
                List<string> ms1splitnewline = ms1_box.Text.Split("\r").ToList();
                for(int i = 0; i < ms1splitnewline.Count; i++)
                {
                    if (ms1splitnewline[i] == "")
                    {
                        continue;
                    }
                    List<string> currms1pkTab = ms1splitnewline[i].Split("\t").ToList();
                    List<string> currms1pkSpace = ms1splitnewline[i].Split(" ").ToList();

                    if (currms1pkTab.Count < 2 && currms1pkSpace.Count < 2)
                    {
                        ReadError();
                        return;
                    }
                    else
                    {
                        List<string> currms1pk;
                        if(currms1pkTab.Count >= 2)
                        {
                            currms1pk = currms1pkTab;
                        }
                        else
                        {
                            currms1pk = currms1pkSpace;
                        }
                        double currMS1MZ;
                        bool parseMS1MZ = double.TryParse(currms1pk[0].Trim(), out currMS1MZ);
                        if ((!parseMS1MZ) || currMS1MZ < 0)
                        {
                            ReadError();
                            return;
                        }
                        double currMS1INT;
                        bool parseMS1INT = double.TryParse(currms1pk[1].Trim(), out currMS1INT);
                        if ((!parseMS1INT) || currMS1INT < 0)
                        {
                            ReadError();
                            return;
                        }
                        currms1.Add(new RAW_PeakElement() { Mz = currMS1MZ, Intensity = currMS1INT });
                    }
                }
                newMS2.Ms1 = currms1;
            }
            if (ms2_box.Text == "")
            {
                newMS2.OriSpectrum = currms2;
            }
            else
            {
                List<string> ms2splitnewline = ms2_box.Text.Split("\r").ToList();
                for (int i = 0; i < ms2splitnewline.Count; i++)
                {
                    if (ms2splitnewline[i] == "")
                    {
                        continue;
                    }
                    List<string> currms2pkTab = ms2splitnewline[i].Split("\t").ToList();
                    List<string> currms2pkSpace = ms2splitnewline[i].Split(" ").ToList();

                    if (currms2pkTab.Count < 2 && currms2pkSpace.Count < 2)
                    {
                        ReadError();
                        return;
                    }
                    else
                    {
                        List<string> currms2pk;
                        if (currms2pkTab.Count >= 2)
                        {
                            currms2pk = currms2pkTab;
                        }
                        else
                        {
                            currms2pk = currms2pkSpace;
                        }
                        double currMS2MZ;
                        bool parseMS2MZ = double.TryParse(currms2pk[0].Trim(), out currMS2MZ);
                        if ((!parseMS2MZ) || currMS2MZ < 0)
                        {
                            ReadError();
                            return;
                        }
                        double currMS2INT;
                        bool parseMS2INT = double.TryParse(currms2pk[1].Trim(), out currMS2INT);
                        if ((!parseMS2INT) || currMS2INT < 0)
                        {
                            ReadError();
                            return;
                        }
                        currms2.Add(new RAW_PeakElement() { Mz = currMS2MZ, Intensity = currMS2INT });
                    }
                }
                newMS2.OriSpectrum = currms2;
            }

            newMS2.Selected = false;
            //newMS2.FileIndex = 0;
            newMS2.Rt = 0.00;
            newMS2.ImageLink = "open.png";
            newMS2.InChiKey = "Unknown";
            newMS2.Formula_PC = "Unknown";
            newMS2.Filename = "Custom MS2";
            newMS2.ScanNumber = 1;

            await this.MainPage.UpdateCustomMS2(newMS2);
            Window.Current.Close();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.MainPage = (MainPage)e.Parameter;
        }
        private async void ReadError()
        {
            ContentDialog noEXEDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Invalid entry",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noEXEDialog.ShowAsync();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }

        private void ionMode_pos_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (ionMode_pos_checkbox.IsChecked == true)
            {
                ionMode_neg_checkbox.IsChecked = false;
                ObservableCollection<Adduct> adductListPos;

                using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    adductListPos = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
                }

                adductList.Clear();
                for (int i = 0; i < adductListPos.Count; i++)
                {
                    adductList.Add(adductListPos[i]);
                }
                adduct_selection.SelectedIndex = 0;
            }
            else
            {
                ionMode_neg_checkbox.IsChecked = true;
                ObservableCollection<Adduct> adductListNeg;

                using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    adductListNeg = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
                }

                adductList.Clear();
                for (int i = 0; i < adductListNeg.Count; i++)
                {
                    adductList.Add(adductListNeg[i]);
                }
                adduct_selection.SelectedIndex = 0;
            }
            //bool polarity = (bool)ionMode_pos_checkbox.IsChecked;
        }

        private void ionMode_neg_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (ionMode_neg_checkbox.IsChecked == true)
            {
                ionMode_pos_checkbox.IsChecked = false;
                ObservableCollection<Adduct> adductListNeg;

                using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Neg.bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    adductListNeg = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
                }

                adductList.Clear();
                for (int i = 0; i < adductListNeg.Count; i++)
                {
                    adductList.Add(adductListNeg[i]);
                }
                adduct_selection.SelectedIndex = 0;
            }
            else
            {
                ionMode_pos_checkbox.IsChecked = true;
                ObservableCollection<Adduct> adductListPos;

                using (Stream stream = File.Open(storageFolder.Path + @"\adductList_Pos.bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    adductListPos = new ObservableCollection<Adduct>((IEnumerable<Adduct>)bformatter.Deserialize(stream));
                }

                adductList.Clear();
                for (int i = 0; i < adductListPos.Count; i++)
                {
                    adductList.Add(adductListPos[i]);
                }
                adduct_selection.SelectedIndex = 0;
            }
            //bool polarity = (bool)ionMode_pos_checkbox.IsChecked;
        }

        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            preMz_box.Text = "283.1864";
            ionMode_pos_checkbox.IsChecked = true;
            ionMode_neg_checkbox.IsChecked = false;
            adduct_selection.SelectedIndex = 0;
            ms1_box.Text = "283.1864\t359645\r284.1895\t46830\r285.1914\t7021";
            ms2_box.Text = "68.0492\t491\r70.0648\t20896\r74.0597\t230\r80.0492\t234\r81.0571\t420\r82.065\t115\r86.0598\t4990\r94.0649\t5423\r98.0599\t556\r100.0757\t213\r104.0704\t1022\r108.0806\t199\r114.0912\t784\r116.0705\t1055\r126.0911\t794\r134.081\t1841\r144.1017\t2071\r162.1122\t2106";
        }

    }
}
