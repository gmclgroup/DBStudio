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
using SqlCeViewer.GlobalDefine;
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
using SqlCeViewer.DataExchangeCenter;
using System.Windows.Media.Animation;
using SqlCeViewer.CommonUI;
using XLAG.WPFThemes;
namespace SqlCeViewer
{
    /// <summary>
    /// Interaction logic for New3DWindow.xaml
    /// </summary>
    public partial class New3DWindow : Page
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

        public class NewDbCollection
        {
            public ImageSource LogoImage { get; set; }
            public string Comment { get; set; }
            public string NewURL { get; set; }
            public string ToolTipText { get; set; }
            public string OfficalLink { get; set; }
        }


        public New3DWindow()
        {
            InitializeComponent();
            //Init Themes
            InitSkin();
            Loaded += (Sender, E) =>
                {
                    #region Loading Code
                    if (isLoaded)
                    {
                        return;
                    }

                    isLoaded = true;



                    string curTheme = string.Empty;

                    if (!string.IsNullOrEmpty(Properties.Settings.Default.WPFControlThemeName))
                    {
                        curTheme = Properties.Settings.Default.WPFControlThemeName;
                    }
                    else
                    {
                        curTheme = "RainierOrange";
                    }

                    //I need scrool to the selected Item of ThemeListBox, 
                    //But it seems no effect. 

                    
                    foreach (ThemeCollection item in themeListBox.Items)
                    {
                        if (item.ThemeName == curTheme)
                        {
                            themeListBox.ScrollIntoView(item);
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

                    if (!NativeGate.Util.LicenseKeySimplestValidation(Properties.Settings.Default.SQLCELicense))
                    {
                        "Your license is not valid,please mail to ms44cn@hotmail.com".Notify();
                        App.Current.Shutdown();
                        return;
                    }
                    #endregion

                    #region
                    //Init Db Collections
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
                        ToolTipText="Entry_SqlServer_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.microsoft.com/Sqlserver/2005/en/us/default.aspx",
                    },
                    new NewDbCollection() {
                        Comment = "MySql",
                        LogoImage = new BitmapImage(new Uri("Images\\MySql.jpg", UriKind.Relative)),
                        NewURL = "MySql\\MySqlEntry.xaml",
                        ToolTipText="Entry_MySql_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.mysql.com/",
                    },
                    new NewDbCollection() {
                        Comment = "Sqlite",
                        LogoImage = new BitmapImage(new Uri("Images\\Sqlite.png", UriKind.Relative)),
                        NewURL = "Sqlite\\SqliteEntry.xaml",
                        ToolTipText="Entry_Sqlite_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.sqlite.org/",
                    },
                    new NewDbCollection() { 
                        Comment = "OleDB",
                        LogoImage = new BitmapImage(new Uri("Images\\OleDB.jpg", UriKind.Relative)), 
                        NewURL = "OleDb\\OleDbMain.xaml",
                        ToolTipText="Entry_Oledb_Tooltip".GetFromResourece(),
                        OfficalLink="http://msdn.microsoft.com/en-us/library/ms722784(VS.85).aspx",
                    },
                      new NewDbCollection() { 
                        Comment = "FireBird",
                        LogoImage = new BitmapImage(new Uri("Images\\firebird-logo-90.png", UriKind.Relative)), 
                        NewURL = "FireBird\\FireBirdEntry.xaml",
                        ToolTipText="Entry_Firebird_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.firebirdsql.org/",
                    },
                    new NewDbCollection() {
                        Comment = "Oracle",
                        LogoImage = new BitmapImage(new Uri("Images\\Oracle.jpg", UriKind.Relative)),
                        NewURL = "",
                        ToolTipText="Entry_Oracle_Tooltip".GetFromResourece(),
                        OfficalLink="http://www.oracle.com/index.html",
                    },
                };
                    DbCollectionList.DataContext = dbList;
                    #endregion

                    #endregion
                };

        }

        public AppendixPanel appendixPanel
        {
            get
            {
                FrameworkElement element = Resources["Visual2"] as FrameworkElement;

                AppendixPanel t = element.FindName("appendixPanel") as AppendixPanel;
                return t;
            }
        }

        public ListBox DbCollectionList
        {
            get
            {
                FrameworkElement element = Resources["Visual1"] as FrameworkElement;

                ListBox t = element.FindName("DbCollectionList") as ListBox;
                return t;
            }
        }
        public ListBox themeListBox
        {
            get
            {
                FrameworkElement element = Resources["Visual1"] as FrameworkElement;

                ListBox t = element.FindName("themeListBox") as ListBox;
                return t;
            }
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

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string newUrl = ((Border)sender).Tag.ToString();
            if (string.IsNullOrEmpty(newUrl))
            {
                "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
                return;
            }

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


            string resAsm = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + curCultureName + "\\SqlCeViewer.resources.dll";
            try
            {
                if (File.Exists(resAsm))
                {
                    Assembly curAsm = Assembly.LoadFrom(resAsm);

                    App.MainResources = new ResourceManager("SqlCeViewer.Properties.Resource." + curCultureName, curAsm);
                }
                else
                {
                    App.MainResources = new ResourceManager("SqlCeViewer.Properties.Resources", Assembly.GetExecutingAssembly());
                }
            }
            catch (Exception resourceException)
            {
                resourceException.HandleMyException();
            }
            //

            this.NavigatorTo(new Uri(newUrl, UriKind.Relative));
        }

        private void themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string theme = e.AddedItems[0].ToString();
                ThemeManager.ApplyTheme(theme);
                Properties.Settings.Default.WPFControlThemeName = theme;
            }
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

            themeList.Add(new ThemeCollection()
            {
                ItemThemeType = ThemeType.Default,
                ThemeFile = GlobalDefine.MyGlobal.DefaultSkinFileFullPath,
                ThemeIcon = new BitmapImage(new Uri("Images\\Theme\\Default.png", UriKind.Relative)),
                ThemeName = "Default",
            });

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

        /// <summary>
        /// Restore to the default theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string themeName = "Aero";
            string themeFile = "Aero.NormalColor.xaml";
            Properties.Settings.Default.WPFControlThemeName = themeName;
            Properties.Settings.Default.Save();
            MPL.Utility.ApplyDefaultTheme(App.Current, themeName, themeFile);
        }

        private void CallTraditioalWind_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigatorTo("NewMain.xaml");
        }
    }
}
