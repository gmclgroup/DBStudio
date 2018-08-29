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
using ETL;
using CoreEA.LoginInfo;
using DBStudio.GlobalDefine;
using CoreEA;
using DBStudio.Utility;

namespace DBStudio.Oracle
{
    /// <summary>
    /// Interaction logic for OracleEntry.xaml
    /// </summary>
    public partial class OracleEntry : UserControl
    {

        public static RoutedUICommand ConnectCmd = new RoutedUICommand("ConnectCmd", "ConnectCmd", typeof(OracleEntry));

        public OracleEntry()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(ConnectCmd, ConnectCmd_Execute, ConnectCmd_CanExecute));

            Loaded += new RoutedEventHandler(OracleEntry_Loaded);
        }

        void OracleEntry_Loaded(object sender, RoutedEventArgs e)
        {
            butConnect.Command = ConnectCmd;

            SerializeClass.DatabaseHistoryInfo.OracleHistory.ForEach(c =>
  txtServerName.Items.Add(c.SID));

            SerializeClass.DatabaseHistoryInfo.OracleHistory.ForEach(c =>
 txtHostName.Items.Add(c.HostName));
        }

        public void ConnectCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            LoginInfo_Oracle info = new LoginInfo_Oracle()
            {
                SID = txtServerName.Text,
                Username=txtUsername.Text,
                Password=passwordBox1.Password,
                HostName=txtHostName.Text,
                Port=int.Parse(txtPort.Text),
            };

            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Oracle).X_Handler;
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    #region Save to Opened History Info
                    OracleObjects oracleItem = new OracleObjects();
                    oracleItem.HostName = info.HostName;
                    oracleItem.Username = info.Username;
                    oracleItem.SID = info.SID;

                    if (!SerializeClass.DatabaseHistoryInfo.OracleHistory.IsContainSubValue(oracleItem.HostName))
                    {
                        HistoryObject oldObject = SerializeClass.DatabaseHistoryInfo;
                        oldObject.OracleHistory.Add(oracleItem);

                        SerializeClass.DatabaseHistoryInfo = oldObject;
                    }
                    #endregion

                    App.MainEngineer.CurPwd = passwordBox1.Password;
                    RibbionIDE ide = new RibbionIDE();

                    ide.Title = info.SID;
                    ide.ShowDialog();
                }
            }
            catch (Exception ee)
            {
                //ee._获取我的异常详细信息().Show();
                App.ResetMainEngineer();
                ee.HandleMyException();
            }
        }


        public void ConnectCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtServerName.Text.IsEmpty())
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }


    }
}
