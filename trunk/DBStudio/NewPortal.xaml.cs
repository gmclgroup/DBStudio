//=============================================================================
//    DBStudio
//    Copyright (C) 2006  ms44

//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation; either
//    version 2 of the License, or (at your option) any later version.

//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.

//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//    If you have any questions ,please contact me via 54715112@qq.com
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBStudio.GlobalDefine;
using System.Windows.Navigation;
using ETL;
using System.Diagnostics;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Globalization;
using DBStudio.DataExchangeCenter;
using System.Windows.Media.Animation;
using XLAG.WPFThemes;
using MPL;
using DBStudio.Bases;
using MPL.Diagnostics;
using DBStudio.CommonUI;
using DBStudio.DocumentingDB;
using DBStudio.UI;
namespace DBStudio
{
    /// <summary>
    /// Interaction logic for NewPortal.xaml
    /// </summary>
    public partial class NewPortal : Window
    {
        public NewPortal()
        {
            this.Logger().Info("1 > entry point ctor");
            //record language
            MyGlobal.OriginalCultureName = Thread.CurrentThread.CurrentCulture.Name;

            InitializeComponent();

            this.Logger().Info("2 > init UI finished ,start to do load");

            #region Code Try to load WPFToolkit
            //No any acctural code need here,just call the WPFToolkit dll
            var t = typeof(System.Windows.Controls.DataGrid);
            if (t != null) // This ensures that the above line isn't optimized away
            {
                // Load your XAML
            }
            #endregion

            Loaded += new RoutedEventHandler(NewPortal_Loaded);

            this.Logger().Info("3 > After loaded ");

           // txtVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            InitLanguages();
        }

        void NewPortal_Loaded(object sender, RoutedEventArgs e)
        {
            #region Lisence Checking

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LicenseCheck);
            this.Logger().Info("1) Read Native Key");
            //if (!NativeGate.Util.LicenseKeySimplestValidation(Properties.Settings.Default.SQLCELicense))
            //{
            //    "Your license is not valid,please mail to ms44cn@hotmail.com".Notify();
            //    App.Current.Shutdown();
            //    return;
            //}

            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LicenseCheck);

            #endregion
            this.Logger().Info("2) End of read Native Key ,read resource");

            this.Logger().Info("3) Start to load Db collections");
            #region Init Db Collections
            List<NewDbCollection> dbList = new List<NewDbCollection>()
                    {
                        
                    new NewDbCollection() {
                        Comment = "SqlCE",
                        LogoImage = new BitmapImage(new Uri("Images\\SqlCE.jpg", UriKind.Relative)),
                        //NewURL = "SqlCE\\SqlCeMain.xaml",
                        NewURL = "SqlCE\\CeEntry.xaml",
                        ToolTipText="Entry_SSCE_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.microsoft.com/Sqlserver/2005/en/us/compact.aspx",
                    },

                    new NewDbCollection() {
                        Comment = "SqlServer",
                        LogoImage = new BitmapImage(new Uri("Images\\SqlServer.jpg", UriKind.Relative)),
                        NewURL = "SqlServer\\SqlServerEntry.xaml",
                        ToolTipText="SqlServerTooltip".GetFromResourece(),
                        OfficalLink="http://www.microsoft.com/Sqlserver/2005/en/us/default.aspx",
                    },
                                                                new NewDbCollection() { 
                        Comment = "Effiproz",
                        LogoImage = new BitmapImage(new Uri("Images\\effiproz_logo90.png", UriKind.Relative)), 
                        NewURL = "Effiproz\\EffiprozEntry.xaml",
                        ToolTipText="Entry_Elliproz_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.effiproz.com",
                    },
                    new NewDbCollection() {
                        Comment = "Oracle",
                        LogoImage = new BitmapImage(new Uri("Images\\Oracle.jpg", UriKind.Relative)),
                        NewURL = "Oracle\\OracleEntry.xaml",
                        ToolTipText="OracleTooltip".GetFromResourece(),
                        OfficalLink="http://www.oracle.com/index.html",
                    },
                    new NewDbCollection() {
                        Comment = "MySql",
                        LogoImage = new BitmapImage(new Uri("Images\\MySql.jpg", UriKind.Relative)),
                        NewURL = "MySql\\MySqlEntry.xaml",
                        ToolTipText="MySqlTooltip".GetFromResourece(),
                        OfficalLink="http://www.mysql.com/",
                    },
                    new NewDbCollection() {
                        Comment = "Sqlite",
                        LogoImage = new BitmapImage(new Uri("Images\\Sqlite.png", UriKind.Relative)),
                        NewURL = "Sqlite\\SqliteEntry.xaml",
                        ToolTipText="SqliteTooltip".GetFromResourece(),
                        OfficalLink="http://www.sqlite.org/",
                    },
                    new NewDbCollection() { 
                        Comment = "OleDB",
                        LogoImage = new BitmapImage(new Uri("Images\\OleDB.jpg", UriKind.Relative)), 
                        NewURL = "OleDb\\OleDbMain.xaml",
                        ToolTipText="OleDBTooltip".GetFromResourece(),
                        OfficalLink="http://msdn.microsoft.com/en-us/library/ms722784(VS.85).aspx",
                    },
                                         new NewDbCollection() { 
                        Comment = "Excel",
                        LogoImage = new BitmapImage(new Uri("Images\\Excel-logo.jpg", UriKind.Relative)), 
                        NewURL = "Excel\\ExcelEntry.xaml",
                        ToolTipText="ExcelTooltip".GetFromResourece(),
                        OfficalLink="http://www.microsoft.com/",
                    },
                                         new NewDbCollection() { 
                        Comment = "CSV",
                        LogoImage = new BitmapImage(new Uri("Images\\CSV-logo.jpg", UriKind.Relative)), 
                        NewURL = "CSV\\CSVEntry.xaml",
                        ToolTipText="CSVTooltip".GetFromResourece(),
                        OfficalLink="https://sourceforge.net/project/showfiles.php?group_id=204776&package_id=275988&release_id=670864/",
                    },
                      new NewDbCollection() { 
                        Comment = "FireBird",
                        LogoImage = new BitmapImage(new Uri("Images\\firebird-logo-90.png", UriKind.Relative)), 
                        NewURL = "FireBird\\FireBirdEntry.xaml",
                        ToolTipText="FirebirdTooltip".GetFromResourece(),
                        OfficalLink="http://www.firebirdsql.org/",
                    },

                };

            DbCollectionList.DataContext = dbList;

            //Binding to simple control as well for some strange scenario
            //In windows 64 bit ,the normal combox control can not display .
            //So we put this simplest way to allow user enter db center UI.
            simpleDbCollectionControl.DataContext = dbList;
            #endregion

            this.Logger().Info("4) End of load db collections and start to set language");


            //Set the start up language
            ResetLocationAndLanguage(LanguageType.AutoDetect);

        }

        private void InitLanguages()
        {
            #region Init Languages
            cmbLanguages.Items.Clear();

            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_AutoDetect".GetFromResourece(),
                Tag = LanguageType.AutoDetect
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_Chinese".GetFromResourece(),
                Tag = LanguageType.Chinese
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_English".GetFromResourece(),
                Tag = LanguageType.English
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_German".GetFromResourece(),
                Tag = LanguageType.German
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_Dutch".GetFromResourece(),
                Tag = LanguageType.Dutch
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_French".GetFromResourece(),
                Tag = LanguageType.French
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_Italy".GetFromResourece(),
                Tag = LanguageType.Italy
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_Spanish".GetFromResourece(),
                Tag = LanguageType.Spanish
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_Russian".GetFromResourece(),
                Tag = LanguageType.Russian
            });
            cmbLanguages.Items.Add(new ComboBoxItem()
            {
                Content = "Language_Japanese".GetFromResourece(),
                Tag = LanguageType.Japanese
            });
  
            #endregion
        }

        public enum LanguageType
        {
            AutoDetect,
            Chinese,
            English,
            German,
            Dutch,
            French,
            Spanish,
            Russian,
            Japanese,
            Italy,
        };

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = cmbSkinStyle.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                try
                {
                    string resName = item.Tag.ToString();
                    ResourceDictionary dict = new ResourceDictionary();
                    App.Current.Resources.Clear();
                    dict.Source = new Uri(string.Format(@"Resources\Skins\{0}", resName), UriKind.Relative);
                    App.Current.Resources.MergedDictionaries.Add(dict);
                }
                catch (Exception ee)
                {
                    this.Logger().Debug(ee.Message);
                    Debug.WriteLine(ee.Message);
                }
            }
        }

        //When user click the mask layer ,do nothing now .
        //Because we changed the UI 
        private void maskLayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string newUrl = ((Border)sender).Tag.ToString();
            if (string.IsNullOrEmpty(newUrl))
            {
               "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
                return;
            }
            GoToNewPage(newUrl);
        }

        private void GoToNewPage(string newUrl)
        {

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ApplicationWindowConstructor);
            //ResetLocationAndLanguage();

            UserControl win = App.LoadComponent(new Uri(newUrl, UriKind.Relative)) as UserControl;
            #region Prepare the UI layout
            Grid hostGrid = new Grid();

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;


            Button but = new Button();
            //BitmapImage image = new BitmapImage(new Uri("Images\\SqlCE.jpg", UriKind.Relative));
            //image.
            //but.Content = image;
            but.Content = "BackButtonText".GetFromResourece();
            but.Height = 20;
            but.Click += delegate
            {
                maskLayer.Visibility = Visibility.Hidden;
            };
            but.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            win.Margin = new Thickness(50, 0, 2, 0);
            win.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            win.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            win.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

            panel.Children.Add(but);
            panel.Children.Add(win);
            hostGrid.Children.Add(panel);
            #endregion
            maskLayer.Children.Clear();
            maskLayer.Children.Add(hostGrid);

            Storyboard story = new Storyboard();
            story = this.Resources["MaskLayerFadeStory_Start"] as Storyboard;

            maskLayer.Visibility = Visibility.Visible;
            story.Begin();

            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ApplicationWindowConstructor);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border bdr = sender as Border;
            bdr.RenderTransformOrigin = new Point(.5, .5);
            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = 1.3;
            scale.ScaleY = 1.1;
            bdr.RenderTransform = scale;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border bdr = sender as Border;
            bdr.RenderTransform = new ScaleTransform();
        }

        /// <summary>
        /// Localization
        /// </summary>
        void ResetLocationAndLanguage(LanguageType currentLanguageType)
        {
            string curCultureName = string.Empty;
            switch (currentLanguageType)
            {
                case LanguageType.AutoDetect:
                    curCultureName = MyGlobal.OriginalCultureName;
                    break;
                case LanguageType.Chinese:
                    curCultureName = "zh-CN";
                    break;
                case LanguageType.English:
                    curCultureName = "en-US";
                    break;
                case LanguageType.German:
                    curCultureName = "de-DE";
                    break;
                case LanguageType.French:
                    curCultureName = "fr-FR";
                    break;
                case LanguageType.Italy:
                    curCultureName = "it-IT";
                    break;
                case LanguageType.Russian:
                    curCultureName = "ru-RU";
                    break;
                case LanguageType.Spanish:
                    curCultureName = "es-ES";
                    break;
                case LanguageType.Dutch:
                    curCultureName = "nl-NL";
                    break;
                case LanguageType.Japanese:
                    curCultureName = "ja-JP";
                    break;
                default:
                    curCultureName = MyGlobal.OriginalCultureName;
                    break;
            }

            Debug.WriteLine("Switch to " + curCultureName);

            Thread.CurrentThread.CurrentCulture = new CultureInfo(curCultureName);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(curCultureName);

            Translator.Culture = Thread.CurrentThread.CurrentCulture;
           
            string resAsm = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + curCultureName + "\\DBStudio.resources.dll";
            try
            {
                if (File.Exists(resAsm))
                {
                    Assembly curAsm = Assembly.LoadFrom(resAsm);

                    App.MainResources = new ResourceManager("DBStudio.Properties.Resources." + curCultureName, curAsm);
                }
                else
                {
                    App.MainResources = new ResourceManager("DBStudio.Properties.Resources", Assembly.GetExecutingAssembly());
                }
            }
            catch (Exception resourceException)
            {
                resourceException.HandleMyException();
            }
        }

        private void cmbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }
            ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
            if (null != item)
            {
                LanguageType currentLanguageType=(LanguageType)item.Tag;
                ResetLocationAndLanguage(currentLanguageType);

                //I am not sure ,but there is a culture selection problem .
                //Here is a workaround to fix .
                //The UI will auto changed when language changed.
                //
                ResetLocationAndLanguage(currentLanguageType);

                //InitLanguages();
            }
        }

        private void Image_PreviewMouseLeftButtonDown_ForEntry(object sender, MouseButtonEventArgs e)
        {
            string newUrl = ((Image)sender).Tag.ToString();
            if (string.IsNullOrEmpty(newUrl))
            {

                "NotifyWhenThisFeatureNotCompleted".GetFromResourece().Show();
                return;
            }

            GoToNewPage(newUrl);
        }


        private void butExchnageData_Click(object sender, RoutedEventArgs e)
        {
            DataExchangeWizard de = new DataExchangeWizard();
            de.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if ((bool)de.ShowDialog())
            {

            }
            else
            {

            }
        }

        private void butDocumentDb_Click(object sender, RoutedEventArgs e)
        {
            DDBWizard.ShowDialog();
        }

        private void butConnectionStringHelper_Click(object sender, RoutedEventArgs e)
        {
            //ConnectionString_Help ch = new ConnectionString_Help();
            //ch.Show();

            try
            {
                Process.Start("http://www.connectionstrings.com/");
            }
            catch (Exception ee)
            {
                this.Logger().Debug(ee.Message);
            }
        }

        private void butGetStart_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(MyGlobal.GettingStartDoc))
            {
                "File Lost".Notify();
                return;
            }

            Process.Start(MyGlobal.GettingStartDoc);
        }

        private void butGoToOnlineSite_CLick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.SourceForgeHomePgae);
            }
            catch (Exception ee)
            {
                this.Logger().Debug(ee.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleDbCollectionControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (simpleDbCollectionControl.Visibility == System.Windows.Visibility.Visible)
            {
                //NewDbCollection item = e.AddedItems[0] as NewDbCollection;
                //Debug.Assert(item != null);
                //GoToNewPage(item.NewURL);
                ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
                GoToNewPage(item.Tag.ToString());
            }
        }

        private void butEmergenceMode_Click(object sender, RoutedEventArgs e)
        {
            if (simpleDbCollectionControl.Visibility == System.Windows.Visibility.Visible)
            {
                "EmergencyModeAgain".GetFromResourece().Show();
                return;
            }

            if ("EmergencyModeConfirm".GetFromResourece().Confirm())
            {
                DbCollectionList.Visibility = System.Windows.Visibility.Hidden;
                simpleDbCollectionControl.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void butAboutUs_Click(object sender, RoutedEventArgs e)
        {
            About aboutUI = new About();
            aboutUI.ShowDialog();
        }
    }
}
