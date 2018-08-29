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

namespace DBStudio
{
    /// <summary>
    /// Interaction logic for NewMain.xaml
    /// </summary>
    /// 
        [Obsolete("This class or main entry will never use again,please do not call it ",false)]
    public partial class NewMain : Page
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


        public NewMain()
        {
            InitializeComponent();
            App.Current.Exit += new ExitEventHandler(Current_Exit);
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

                    #region Load Config Info
                    string configFilePath = MyGlobal.ConfigFilePath;
                    if (!File.Exists(configFilePath))
                    {
                        ConfigInfo cInfo = new ConfigInfo();
                        //XLCS.Serialize.SerializedFile.SaveFile(configFilePath, cInfo);
                        XLCS.Serialize.SerializedFile.SaveXml(configFilePath, cInfo);
                    }
                    else
                    {
                        //App.MyConfigInfo = XLCS.Serialize.SerializedFile.OpenFile(configFilePath) as ConfigInfo;
                        App.MyConfigInfo = XLCS.Serialize.SerializedFile.OpenXml(configFilePath,typeof(ConfigInfo)) as ConfigInfo;
                    }

                    #endregion 


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

                    //if (!NativeGate.Util.LicenseKeySimplestValidation(Properties.Settings.Default.SQLCELicense))
                    //{
                    //    "Your license is not valid,please mail to ms44cn@hotmail.com".Notify();
                    //    App.Current.Shutdown();
                    //    return;
                    //}
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

                    #endregion
                };

        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            //XLCS.Serialize.SerializedFile.SaveFile(MyGlobal.ConfigFilePath,App.MyConfigInfo);
            XLCS.Serialize.SerializedFile.SaveXml(MyGlobal.ConfigFilePath, App.MyConfigInfo);
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


            string resAsm = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + curCultureName + "\\DBStudio.resources.dll";
            try
            {
                if (File.Exists(resAsm))
                {
                    Assembly curAsm = Assembly.LoadFrom(resAsm);

                    App.MainResources = new ResourceManager("DBStudio.Properties.Resource." + curCultureName, curAsm);
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

        private void Image_PreviewMouseLeftButtonDown_ForEntry(object sender, MouseButtonEventArgs e)
        {
            string newUrl = ((Image)sender).Tag.ToString();
            if (string.IsNullOrEmpty(newUrl))
            {
                "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
                return;
            }

            this.NavigatorTo(new Uri(newUrl, UriKind.Relative));
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

        private void CallTraditioalWind_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigatorTo("New3DWindow.xaml");
        }
    }
}
