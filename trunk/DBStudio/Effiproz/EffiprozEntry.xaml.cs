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
using Microsoft.Win32;
using ETL;
using CoreEA.LoginInfo;
using System.Diagnostics;
using DBStudio.GlobalDefine;
using System.IO;

namespace DBStudio.Effiproz
{
    /// <summary>
    /// Interaction logic for EffiprozEntry.xaml
    /// </summary>
    public partial class EffiprozEntry : UserControl
    {
        /// <summary>
        /// Current Connection Type of combobox value
        /// change it carefully
        /// </summary>
        ConnectionType currentDBConnectionType;

        public EffiprozEntry()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(EffiprozEntry_Loaded);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EffiprozEntry_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            new CreateElliprozDb().Show();
        }

        private void butOpen_Click(object sender, RoutedEventArgs e)
        {
            switch (currentDBConnectionType)
            {
                case ConnectionType.File:
                    if (txtDbLocation.Text.IsEmpty())
                    {
                        "TitleInputDbFile".GetFromResourece().Show();
                        return;
                    }

                    break;
                case ConnectionType.Memory:
                    if (txtDBName.Text.IsEmpty())
                    {
                        "TitleInputDbFile".GetFromResourece().Show();
                        return;
                    }

                    break;
            }
          
            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Effiproz).X_Handler;


                string dbName = string.Empty;
                switch (currentDBConnectionType)
                {
                    case ConnectionType.File:

                        string rootPath = System.IO.Path.GetDirectoryName(txtDbLocation.Text);
                        dbName=rootPath + "\\"+System.IO.Path.GetFileNameWithoutExtension(txtDbLocation.Text);
                        break;
                    case ConnectionType.Memory:
                        dbName = txtDBName.Text;
                        break;
                    default:
                        throw new Exception("invalid mode");
                }
              

                App.MainEngineer.Open
                    (new CoreEA.LoginInfo.LoginInfo_Effiproz()
                    {
                        InitialCatalog = dbName,
                        Username = txtUserName.Text,
                        Password = txtPwd.Password,
                        DBConnectionType = currentDBConnectionType,
                        IsReadOnly = (bool)chkIsReadonly.IsChecked,
                        IsAutoShutdown = (bool)chkIsAutoShutdown.IsChecked,
                        IsAutoCommit = (bool)chkIsAutoCommit.IsChecked,
                    }
                    );
                if (App.MainEngineer.IsOpened)
                {

                    RibbionIDE ide = new RibbionIDE();
                    try
                    {
                        ide.WindowState = WindowState.Maximized;
                        ide.BringIntoView();
                        ide.ShowDialog();
                    }
                    catch (InvalidOperationException ee)
                    {
                        Debug.WriteLine(ee.Message);

                        ee.Message.Show();
                    }
                    catch (Exception ee)
                    {
                        ee.Message.Show();
                    }
                }
                else
                {
                    App.ResetMainEngineer();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                App.ResetMainEngineer();
                return;
            }

        }

        private void butSelectPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.Effiproz_FILE_FILTER;
            if ((bool)sf.ShowDialog())
            {

                txtDbLocation.Text = sf.FileName;
            }
        }

        private void cmbDbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)e.AddedItems[0]).Content.ToString() == "File")
            {
                currentDBConnectionType = ConnectionType.File;
                if (panel1 != null)
                {
                    panel1.Visibility = System.Windows.Visibility.Visible;
                    panel2.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                currentDBConnectionType = ConnectionType.Memory;
                panel2.Visibility = System.Windows.Visibility.Visible;
                panel1.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Example data should be file type based 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butOpenExampleData_Click(object sender, RoutedEventArgs e)
        {
            string exampleFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ExampleData\\EffiprozeData\\NorthwindEF";
            if (!File.Exists(exampleFile+".properties"))
            {
                "ErrorExampleDataLost".GetFromResourece().Show();
                return;
            }
            txtDbLocation.Text = exampleFile;

            currentDBConnectionType = ConnectionType.File;
            txtPwd.Password = string.Empty;
            txtUserName.Text = "sa";
            butOpen_Click(null, null);
        }

    }
}
