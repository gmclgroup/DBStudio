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
using CoreEA.LoginInfo;
using CoreEA.Args;
namespace DBStudio.SqlServer.DataExchange
{
    /// <summary>
    /// Interaction logic for SyncDataInSqlServer.xaml
    /// </summary>
    public partial class SyncDataInSqlServer : BaseUI.BaseFadeDialog
    {
        public SyncDataInSqlServer()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string server = txtServername.Text;
            string database = this.txtDbName.Text;
            string username = this.txtUsername.Text;
            string pwd = passwordBox1.Password;
            bool isTrustedConn = (bool)chkIsTrustedConn.IsChecked;
            bool isSql05Express = (bool)chkIsSql05Expresss.IsChecked;

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database)
                || string.IsNullOrEmpty(username))
            {
                "ImportEachElement".GetFromResourece().Notify();
                return;
            }


            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;
            LoginInfo_SqlServer info = new LoginInfo_SqlServer()
                {
                    X_Server = server,
                    X_Database = database,
                    X_UserName = username,
                    X_Pwd = pwd,
                    IsTrustedConn=(bool)chkIsTrustedConn.IsChecked,
                };

            if (isSql05Express)
            {
                info.X_CurDbConnectionMode = CurDbServerConnMode.SqlServer2005Express;
            }

            try
            {
                core.Open(info);
                if (core.IsOpened)
                {
                    SrcTableList tableUI = new SrcTableList(core);

                    tableUI.ShowDialog();
                }
            }
            catch (Exception ee)
            {
                XLCS.Common.ProcessException.DisplayErrors(ee);
            }

        }
    }
}
