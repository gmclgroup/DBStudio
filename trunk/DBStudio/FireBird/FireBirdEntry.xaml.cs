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
using CoreEA.LoginInfo;
using ETL;
using Microsoft.Win32;
using DBStudio.GlobalDefine;

namespace DBStudio.FireBird
{
    /// <summary>
    /// Interaction logic for FireBirdEntry.xaml
    /// </summary>
    public partial class FireBirdEntry : UserControl
    {
        public FireBirdEntry()
        {
            InitializeComponent();
        }

        private void butLoginIn_Click(object sender, RoutedEventArgs e)
        {
            if (cmbServerType.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(txtServer.Text))
                {
                    "Please input the server".Show();
                    return;
                }

            }
            if (string.IsNullOrEmpty(txtFile.Text))
            {
                "Please select the file".Show();
                return;
            }

            LoginInfo_Firebird info = new LoginInfo_Firebird()
            {
                Username=txtUsername.Text,
                Password=txtPwd.Password,
                DataFile=txtFile.Text,
            };

            if (cmbServerType.SelectedIndex == 0)
            {
                info.DataSource = "localhost";
            }
            else
            {
                info.DataSource = txtServer.Text;
            }


            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Firebird).X_Handler;

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }


            try
            {
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    "OK".Show();
                }
            }
            catch (Exception ee)
            {
                App.ResetMainEngineer();
                ee.HandleMyException();
            }
        }

        private void cmbServerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServerPanel != null)
            {
                if (cmbServerType.SelectedIndex == 0)
                {
                    ServerPanel.Visibility = Visibility.Collapsed; 
                }
                else if (cmbServerType.SelectedIndex == 1)
                {
                   ServerPanel.Visibility = Visibility.Visible;
                }
            }
            
        }

        private void butSelectFDBFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.FireBird_FILE_FILTER;

            if (sf.ShowDialog() == true)
            {
                txtFile.Text = sf.FileName;
            }
        }
    }
}
