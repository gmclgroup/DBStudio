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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using DBStudio.DataExchangeCenter;
using DBStudio.Bases;
using System.Threading;
using DBStudio.SqlServer;
using DBStudio.DocumentingDB;
using System.Diagnostics;
using System.IO;
using DBStudio.GlobalDefine;
using DBStudio.AdditionUI;

namespace DBStudio.CommonUI
{
    /// <summary>
    /// Interaction logic for AppendixPanel.xaml
    /// </summary>
    public partial class AppendixPanel : UserControl
    {

        /// <summary>
        /// Selected language name
        /// </summary>
        public static readonly DependencyProperty X_SelectedLanguageProperty =
            DependencyProperty.Register("X_SelectedLanguageProperty", typeof(string), typeof(AppendixPanel));
        /// <summary>
        /// 
        /// </summary>
        public string X_SelectedLanguage
        {
            get
            {
                return (string)GetValue(X_SelectedLanguageProperty);
            }
            set
            {
                SetValue(X_SelectedLanguageProperty, value);
            }
        }

        public AppendixPanel()
        {
            InitializeComponent();
            
            Loaded += new RoutedEventHandler(AppendixPanel_Loaded);
        }

        void AppendixPanel_Loaded(object sender, RoutedEventArgs e)
        {
            #region Localization string and objects
            cmbLanguages.Items.Clear();
            //Fill the UI with current support languages
            cmbLanguages.Items.Add("Auto Detect");
            cmbLanguages.Items.Add("English");
            cmbLanguages.Items.Add("Chinese");


            cmbLanguages.SelectionChanged += (a, b) =>
                {
                    if (cmbLanguages.SelectedItem != null)
                    {
                        X_SelectedLanguage = cmbLanguages.SelectedItem.ToString();

                        string curCultureName = string.Empty;

                        switch (X_SelectedLanguage)
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

                        Translator.Culture = new System.Globalization.CultureInfo(curCultureName);
                    }
                };

            cmbLanguages.SelectedIndex = 0;

            //Test code
            //App.MainResources.GetString("DFTestString").Notify();
            //"DFTestString".GetFromResourece().Notify();
            #endregion

            txtVersion.Text = String.Format("{1} {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                "AppName".GetFromResourece());
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
            ConnectionString_Help ch = new ConnectionString_Help();
            ch.Show();
        }

        [Obsolete("This method is exist,but no called")]
        private void butTest_Click(object sender, RoutedEventArgs e)
        {
            TestPerformance test = new TestPerformance();
            test.ShowDialog();
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
            Process.Start(Properties.Settings.Default.SourceForgeHomePgae);
        }
    }
}
