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
using Microsoft.Win32;
using DBStudio.GlobalDefine;
using ETL;
using CoreEA.LoginInfo;


namespace DBStudio.SqlCE.UI
{
    /// <summary>
    /// Interaction logic for CreateEmptyCEDB.xaml
    /// </summary>
    public partial class CreateEmptyCEDB : BaseUI.BaseFadeDialog
    {
        public delegate void Del_CreateDbAfterSuccesful(string fullPath, string pwd);
        public Del_CreateDbAfterSuccesful CreateDbAfterSuccesful;

        public CreateEmptyCEDB()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void butSelectCeDBFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = MyGlobal.SQLCE_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                txtDbName.Text = sf.FileName;
            }
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDbName.Text))
            {
                "Please input db name".Notify();
                return;
            }
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtDbName.Text)))
            {
                "The specified directory did not existed, please check it and try again".Notify();
                return;
            }
            if (txtPwd.Password != txtConfirmPwd.Password)
            {
                "Two passwords are not euqal , please try again".Notify();
                return;
            }

            string dbName = txtDbName.Text;
            string pwd = txtPwd.Password;
            bool isEncrypted = (bool)this.chkIsEncrypted.IsChecked;
            bool isCaseSensitive = (bool)this.chkIsCaseSensitive.IsChecked;

            if (string.IsNullOrEmpty(pwd))
            {
                if (!"Do you confirm create this database with no password protect ? ".Confirm())
                {
                    return;
                }
            }

            App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;

            try
            {
                if (App.MainEngineer.CreateDatabase(
                    new LoginInfo_SSCE() { DbName = dbName,Pwd=pwd,IsCaseSensitive=isCaseSensitive,IsEncrypted=isEncrypted }
                    ))
                {
                    MessageBox.Show(string.Format("Create {0} successful", dbName));
                    if (CreateDbAfterSuccesful != null)
                    {
                        CreateDbAfterSuccesful(dbName, pwd);
                    }

                    Close();
                }
            }
            catch(Exception ee)
            {
                ee.Message.Notify();
            }
            finally
            {
                App.ResetMainEngineer();
            }
        }
    }
}
