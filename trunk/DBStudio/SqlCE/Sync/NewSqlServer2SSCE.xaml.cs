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
using CustomControl.DynaProgressBar;
using Microsoft.Win32;
using ETL;
using DBStudio.GlobalDefine;
using DBStudio.CommonMethod;
using DBStudio.CommonUI;
using CoreEA.LoginInfo;
using System.ComponentModel;

namespace DBStudio.SqlCE.Sync
{
    /// <summary>
    /// Interaction logic for NewSqlServer2SSCE.xaml
    /// </summary>
    public partial class NewSqlServer2SSCE : BaseUI.BaseFadeDialog
    {
        CoreEA.ICoreEAHander core = null;

        public NewSqlServer2SSCE()
        {
            InitializeComponent();

            Title = "Import data from Sql Server to SSCE";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        [Description("This core will be used later , do not dispose it ")]
        private void butGetTableList_Click(object sender, RoutedEventArgs e)
        {
            WPFCommonControl.SqlServerLoginControl.X_CollectionData controlResult =sqlServerLoginControl1.X_Result;
            
            string server = controlResult.Server;
            string database = controlResult.DbName;
            string username = controlResult.UID;
            string pwd = controlResult.PWD;

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database)
                || string.IsNullOrEmpty(username))
            {
                MessageBox.Show("ImportEachElement".GetFromResourece());
                return;
            }

            //This core will be used later , do not dispose it 
            core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;

            LoginInfo_SqlServer info = new LoginInfo_SqlServer()
            {
                X_Server = controlResult.Server,
                X_UserName = controlResult.UID,
                X_Pwd = controlResult.PWD,
                X_Port = sqlServerLoginControl1.X_Result.Port.ToString(),
                IsTrustedConn = sqlServerLoginControl1.X_Result.IsTrust,
                X_CurDbConnectionMode = sqlServerLoginControl1.X_Result.CurType.CurConnMode,
            };

            try
            {
                core.Open(info);
                if (core.IsOpened)
                {
                    this.tableList.Items.Clear();
                    foreach (string item in core.GetTableListInDatabase(database))
                    {
                        tableList.Items.Add(item);
                    }
                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
            }
        }

        private void SyncSqlServer2SSCE(object sender, RoutedEventArgs e)
        {
            string server = sqlServerLoginControl1.X_Result.Server;
            string database = sqlServerLoginControl1.X_Result.DbName;
            string username = sqlServerLoginControl1.X_Result.UID;
            string pwd = sqlServerLoginControl1.X_Result.PWD;

            if (core == null)
            {
                "ImportNeedSelectTables".GetFromResourece().Notify();
                return;
            }

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database)
                || string.IsNullOrEmpty(username))
            {
                "ImportEachElement".GetFromResourece().Notify();
                return;
            }
            string sdfFile = txtTargetFile.Text;

            PageSwitchProgressBar dp = PageSwitchProgressBar.X_BeginLoadingDialog();
            List<SyncResultArgs> result = null;

            try
            {
                List<string> proecssTableList = new List<string>();

                foreach (string item in tableList.SelectedItems)
                {
                    proecssTableList.Add(item);
                }

                result = CommonUtil.SyncDataFromSqlServerToSSCE(core, sdfFile, proecssTableList, (bool)chkIsNeedCopyData.IsChecked);

            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
                if (!dp.IsDisposed)
                {
                    dp.X_EndLoadingDialog();
                }
            }

            if (result != null)
            {
                ShowSyncResult rShow = new ShowSyncResult(result);
#if DEBUG
#else

                rShow.Owner=this;
#endif

                rShow.ShowDialog();
            }
        }

        private void butSelectTargetFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.SQLCE_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                this.txtTargetFile.Text = sf.FileName;
            }
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)chkSelectAll.IsChecked)
            {
                tableList.SelectAll();
            }
            else
            {
                tableList.SelectedItems.Clear();

                tableList.SelectedIndex = 0;
            }
        }
    }
}
