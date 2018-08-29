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
using SSDM.GlobalDefine;
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
using SSDM.DataExchangeCenter;
using System.Windows.Media.Animation;
using XLAG.WPFThemes;
using MPL;
using SSDM.Bases;
using MPL.Diagnostics;


namespace SSDM
{
    /// <summary>
    /// Interaction logic for NewEntry.xaml
    /// </summary>
    [Obsolete("Old Portal")]
    public partial class NewEntry : MPL.BaseFadeDialog
    {

        bool isLoaded = false;

        /// <summary>
        /// 
        /// </summary>
        public enum ThemeType
        {
            Default,
            SystemEmbedded,
            Toolkit,
        };

        /// <summary>
        /// Theme object when databinding
        /// </summary>
        public class ThemeCollection
        {
            public ImageSource ThemeIcon { get; set; }
            public string ThemeName { get; set; }
            public ThemeType ItemThemeType { get; set; }
            public string ThemeFile { get; set; }
        }




        public NewEntry()
        {
            InitializeComponent();

            X_AnimationTime = new TimeSpan(0, 0, 0, 1);

            X_AnimationType = "BounsMoveTop2BottomAnimation";

            Loaded += new RoutedEventHandler(NewEntry_Loaded);

            //Init Themes
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.InitSkin);
            InitSkin();
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.InitSkin);

        }

        private void InitSkin()
        {
            string[] toolkitThemeList = ThemeManager.GetThemes();
            List<ThemeCollection> themeList = new List<ThemeCollection>();
            //Add theme in toolkit
            foreach (var eachTheme in toolkitThemeList)
            {
                themeList.Add(new ThemeCollection()
                {
                    ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\" + eachTheme + ".png", UriKind.Relative)),
                    ThemeName = eachTheme,
                    ItemThemeType = ThemeType.Toolkit,
                });
            }

            //themeList.Add(new ThemeCollection()
            //{
            //    ItemThemeType = ThemeType.Default,
            //    ThemeFile = GlobalDefine.MyGlobal.DefaultSkinFileFullPath,
            //    ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Default.png", UriKind.Relative)),
            //    ThemeName = "Default",
            //});

            //Add theme in system default library
            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.SystemEmbedded,
                ThemeFile = "Luna.NormalColor.xaml",
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Luna.png", UriKind.Relative)),
                ThemeName = "Luna",
            }
            );
            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.SystemEmbedded,
                ThemeFile = "Luna.Metallic.xaml",
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Luna.png", UriKind.Relative)),
                ThemeName = "Luna",
            }
);
            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.SystemEmbedded,
                ThemeFile = "Luna.HomeStead.xaml",
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Luna.png", UriKind.Relative)),
                ThemeName = "Luna",
            }
);
            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.SystemEmbedded,
                ThemeFile = "Aero.NormalColor.xaml",
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Aero.png", UriKind.Relative)),
                ThemeName = "Aero",
            }
);
            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.SystemEmbedded,
                ThemeFile = "Classic.xaml",
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Classic.png", UriKind.Relative)),
                ThemeName = "Classic",
            }
);
            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.SystemEmbedded,
                ThemeFile = "Royale.NormalColor.xaml",
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Royale.png", UriKind.Relative)),
                ThemeName = "Royale",
            }
);

            themeListBox.DataContext = themeList;
        }


        void NewEntry_Loaded(object sender, RoutedEventArgs e)
        {
            #region Loading Code
            if (isLoaded)
            {
                return;
            }

            isLoaded = true;
  
            string curTheme = string.Empty;

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadTheme);
            if (!string.IsNullOrEmpty(Properties.Settings.Default.WPFControlThemeName))
            {
                curTheme = Properties.Settings.Default.WPFControlThemeName;
            }
            else
            {
                curTheme = ThemeManager.DefaultTheme;
            }
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadTheme);

            //I need scrool to the selected Item of ThemeListBox, 
            //But it seems no effect. 
            foreach (ThemeCollection item in themeListBox.Items)
            {
                if (item.ThemeName == curTheme)
                {
                    themeListBox.ScrollIntoView(item);
                    break;
                }
            }

            if (curTheme.Trim().ToLower() == "default")
            {
                MPL.Utility.ApplyTheme(App.Current, GlobalDefine.MyGlobal.DefaultSkinFileFullPath);
            }
            else if (ThemeManager.GetThemes().Contains(curTheme))
            {
                ThemeManager.ApplyTheme(curTheme);
            }
            else
            {
                string themeFile = curTheme;

                if (curTheme == "Classic")
                {
                    themeFile += ".xaml";
                }
                else
                {
                    themeFile += ".NormalColor.xaml";
                }

                MPL.Utility.ApplyDefaultTheme(App.Current, curTheme, themeFile);
            }

            #region Lisence Checking

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LicenseCheck);
            if (!NativeGate.Util.LicenseKeySimplestValidation(Properties.Settings.Default.SQLCELicense))
            {
                "Your license is not valid,please mail to ms44cn@hotmail.com".Notify();
                App.Current.Shutdown();
                return;
            }
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LicenseCheck);

            #endregion

            #region Init Db Collections
            List<NewDbCollection> dbList = new List<NewDbCollection>()
                    {
                        
                    new NewDbCollection() {
                        Comment = "SqlCE",
                        LogoImage = new BitmapImage(new Uri("Images\\SqlCE.jpg", UriKind.Relative)),
                        NewURL = "SqlCE\\CeEntry.xaml",
                        ToolTipText="Sql Server Compact Edition 3.5 or up ,please click here",
                        OfficalLink="http://www.microsoft.com/Sqlserver/2005/en/us/compact.aspx",
                    },
                    new NewDbCollection() {
                        Comment = "SqlServer",
                        LogoImage = new BitmapImage(new Uri("Images\\SqlServer.jpg", UriKind.Relative)),
                        NewURL = "SqlServer\\SqlServerEntry.xaml",
                        ToolTipText="Sql Server 2000 or up ,please click here",
                        OfficalLink="http://www.microsoft.com/Sqlserver/2005/en/us/default.aspx",
                    },
                    new NewDbCollection() {
                        Comment = "Oracle",
                        LogoImage = new BitmapImage(new Uri("Images\\Oracle.jpg", UriKind.Relative)),
                        NewURL = "Oracle\\OracleEntry.xaml",
                        ToolTipText="Oracle 9 or up ,please click here",
                        OfficalLink="http://www.oracle.com/index.html",
                    },
                    new NewDbCollection() {
                        Comment = "MySql",
                        LogoImage = new BitmapImage(new Uri("Images\\MySql.jpg", UriKind.Relative)),
                        NewURL = "MySql\\MySqlEntry.xaml",
                        ToolTipText="MySql 5.0 or up ,please click here",
                        OfficalLink="http://www.mysql.com/",
                    },
                    new NewDbCollection() {
                        Comment = "Sqlite",
                        LogoImage = new BitmapImage(new Uri("Images\\Sqlite.png", UriKind.Relative)),
                        NewURL = "Sqlite\\SqliteEntry.xaml",
                        ToolTipText="Sqlite",
                        OfficalLink="http://www.sqlite.org/",
                    },
                    new NewDbCollection() { 
                        Comment = "OleDB",
                        LogoImage = new BitmapImage(new Uri("Images\\OleDB.jpg", UriKind.Relative)), 
                        NewURL = "OleDb\\OleDbMain.xaml",
                        ToolTipText="OleDB based type ,such as Access,Excel c,please click here",
                        OfficalLink="http://msdn.microsoft.com/en-us/library/ms722784(VS.85).aspx",
                    },
                                         new NewDbCollection() { 
                        Comment = "Excel",
                        LogoImage = new BitmapImage(new Uri("Images\\Excel-logo.jpg", UriKind.Relative)), 
                        NewURL = "Excel\\ExcelEntry.xaml",
                        ToolTipText="Excel ,please click here",
                        OfficalLink="http://www.microsoft.com/",
                    },
                                         new NewDbCollection() { 
                        Comment = "CSV",
                        LogoImage = new BitmapImage(new Uri("Images\\CSV-logo.jpg", UriKind.Relative)), 
                        NewURL = "CSV\\CSVEntry.xaml",
                        ToolTipText="CSV or Text File,please click here",
                        OfficalLink="https://sourceforge.net/project/showfiles.php?group_id=204776&package_id=275988&release_id=670864/",
                    },
                      new NewDbCollection() { 
                        Comment = "FireBird",
                        LogoImage = new BitmapImage(new Uri("Images\\firebird-logo-90.png", UriKind.Relative)), 
                        NewURL = "FireBird\\FireBirdEntry.xaml",
                        ToolTipText="FireBird / Interbase ,please click here",
                        OfficalLink="http://www.firebirdsql.org/",
                    },

                };
            DbCollectionList.DataContext = dbList;
            #endregion


            ResetLocationAndLanguage();
            #endregion
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

        private void Image_PreviewMouseLeftButtonDown_ForEntry(object sender, MouseButtonEventArgs e)
        {
            string newUrl = ((Image)sender).Tag.ToString();
            if (string.IsNullOrEmpty(newUrl))
            {
                "This feature is not complete , please waiting for the next release".Show();
                return;
            }

            GoToNewPage(newUrl);
        }

        private void Border_MouseEnter_Theme(object sender, MouseEventArgs e)
        {
            Border bdr = sender as Border;
            bdr.RenderTransformOrigin = new Point(.5, .5);
            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = 1.8;
            scale.ScaleY = 1.8;
            bdr.RenderTransform = scale;
        }

        private void Border_MouseLeave_Theme(object sender, MouseEventArgs e)
        {
            Border bdr = sender as Border;
            bdr.RenderTransform = new ScaleTransform();
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ThemeCollection curCollection = ((Image)sender).Tag as ThemeCollection;
            if (curCollection != null)
            {

                switch (curCollection.ItemThemeType)
                {
                    case ThemeType.SystemEmbedded:
                        MPL.Utility.ApplyDefaultTheme(App.Current, curCollection.ThemeName, curCollection.ThemeFile);
                        break;
                    case ThemeType.Toolkit:
                        ThemeManager.ApplyTheme(curCollection.ThemeName);
                        break;
                    case ThemeType.Default:
                        MPL.Utility.ApplyTheme(App.Current, curCollection.ThemeFile);
                        break;
                }
            }
        }

        private void maskLayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Storyboard story = new Storyboard();
            //story = this.Resources["MaskLayerFadeStory_End"] as Storyboard;

            //story.Begin();
            maskLayer.Visibility = Visibility.Hidden;
        }

        private void butCallAppendixPanel_Click(object sender, RoutedEventArgs e)
        {

            Storyboard story = null;
            if (appendixPanel.Height > 0)
            {

                story = this.Resources["appendixPanelStoryEnd"] as Storyboard;
            }
            else
            {
                story = this.Resources["appendixPanelStoryStart"] as Storyboard;
            }
            if (story != null)
            {
                story.Begin();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string newUrl = ((Border)sender).Tag.ToString();
            if (string.IsNullOrEmpty(newUrl))
            {
                Properties.Resources.NotCompletedFeatureNotifyMsg.Show();
                return;
            }
            GoToNewPage(newUrl);
        }

        /// <summary>
        /// Localization
        /// </summary>
        void ResetLocationAndLanguage()
        {
            //Set the global resources
            string curCultureName = string.Empty;

            switch (appendixPanel.X_SelectedLanguage)
            {
                case "Auto Detect":
                    curCultureName = Thread.CurrentThread.CurrentCulture.Name;
                    break;
                case "English":
                    curCultureName = "en-US";
                    break;
                case "Chinese":
                    curCultureName = "zh-CN";
                    break;
                default:
                    curCultureName = Thread.CurrentThread.CurrentCulture.Name;
                    break;
            }

            //Translator.Culture = Thread.CurrentThread.CurrentCulture;

            string resAsm = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + curCultureName + "\\SSDM.resources.dll";
            try
            {
                if (File.Exists(resAsm))
                {
                    Assembly curAsm = Assembly.LoadFrom(resAsm);

                    App.MainResources = new ResourceManager("SSDM.Properties.Resource." + curCultureName, curAsm);
                }
                else
                {
                    App.MainResources = new ResourceManager("SSDM.Properties.Resources", Assembly.GetExecutingAssembly());
                }
            }
            catch (Exception resourceException)
            {
                resourceException.HandleMyException();
            }
        }

        private void GoToNewPage(string newUrl)
        {

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ApplicationWindowConstructor);
            ResetLocationAndLanguage();

            UserControl win = App.LoadComponent(new Uri(newUrl, UriKind.Relative)) as UserControl;
            
            maskLayer.Children.Clear();
            maskLayer.Children.Add(win);

            Storyboard story = new Storyboard();
            story = this.Resources["MaskLayerFadeStory_Start"] as Storyboard;
            maskLayer.Visibility = Visibility.Visible;
            story.Begin();

            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ApplicationWindowConstructor);
        }
    }
}
