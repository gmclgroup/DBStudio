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
using CoreEA.LoginInfo;
using ETL;
using DBStudio.GlobalDefine;
using wf = System.Windows.Forms;

namespace DBStudio.Effiproz
{
    /// <summary>
    /// Interaction logic for CreateElliprozDb.xaml
    /// </summary>
    public partial class CreateElliprozDb : BaseUI.BaseFadeDialog
    {
        public CreateElliprozDb()
        {
            InitializeComponent();
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            if (txtDBName.Text.IsEmpty())
            {
                "TitleInputDbFile".GetFromResourece().Show();
                return;
            }
            string targetDbName=string.Empty;
            
            ConnectionType currentType;
            //If file mode ,then check the location
            //otherwise ,just get the name value
            if (cmbDbType.SelectedIndex == 0)
            {
                currentType=ConnectionType.File;

                if (!System.IO.Directory.Exists(txtDBLocation.Text))
                {
                    "The database location is not exist,please check it and try again".Show();
                    return;
                }

                targetDbName = txtDBLocation.Text + txtDBName.Text;
                if (System.IO.File.Exists(targetDbName + ".loc") ||
                    System.IO.File.Exists(targetDbName + ".script"))
                {
                    if (!"The database with same name looks like existed,do you confirm create again ?".Confirm())
                    {
                        return;
                    }
                }

            }
            else
            {
                currentType=ConnectionType.Memory;
                targetDbName = txtDBName.Text;
            }

            
            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Effiproz).X_Handler;
            
            try
            {
                if (core.CreateDatabase(new LoginInfo_Effiproz()
                {
                    InitialCatalog = targetDbName,
                    Username = txtUserName.Text,
                    Password = txtPwd.Password,
                    DBConnectionType = currentType,
                }))
                {
                    "Create OK".Notify();
                    this.Close();
                }
                else
                {
                    "Create Failure".Show();
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                core.Dispose();
            }
        }

        private void butSelectPath_Click(object sender, RoutedEventArgs e)
        {
            wf.FolderBrowserDialog f = new wf.FolderBrowserDialog();
            if (f.ShowDialog() == wf.DialogResult.OK)
            {
                txtDBLocation.Text = f.SelectedPath;
            }
        }

        private void cmbDbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectLocationPanel == null)
            {
                return;
            }

            if (((ComboBoxItem)e.AddedItems[0]).Content.ToString() == "File")
            {
                selectLocationPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                selectLocationPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
