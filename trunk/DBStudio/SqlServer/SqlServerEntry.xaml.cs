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
using DBStudio.SqlServer.SomeFunctions;
using DBStudio.GlobalDefine;
using DBStudio.CommonMethod;

using System.ComponentModel;
using System.Data;
using DBStudio.Utility;
using ETL;
using CoreEA.LoginInfo;
using CoreEA;
using XLCS.Common;
using Microsoft.Win32;
using CustomControl.DynaProgressBar;

namespace DBStudio.SqlServer
{
    /// <summary>
    /// Interaction logic for NewSqlServerMain.xaml
    /// </summary>
    public partial class SqlServerEntry : UserControl
    {
        /// <summary>
        /// This routed command should apply on the connect button
        /// </summary>
        RoutedUICommand DoConnectCmd = new RoutedUICommand();

        public SqlServerEntry()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(DoConnectCmd, DoConnectCmd_Executed, DoConnectCmd_CanExecute));
            butConnect.Command = DoConnectCmd;

            Loaded += new RoutedEventHandler(SqlServerEntry_Loaded);
        }

        private void DoConnectCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WPFCommonControl.SqlServerLoginControl.X_CollectionData controlResult =
               this.sqlServerLoginControl1.X_Result;

            if (controlResult.IsUseFileDirectly)
            {
                
            }
            else
            {
                if (string.IsNullOrEmpty(controlResult.Server) || string.IsNullOrEmpty(controlResult.DbName)
                    || string.IsNullOrEmpty(controlResult.UID))
                {
                    "ImportEachElement".GetFromResourece().Notify();
                    return;
                }
            }
            LoginInSqlServer(controlResult);
        }

        private void DoConnectCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sqlServerLoginControl1.X_Result.IsUseFileDirectly)
            {
                if (string.IsNullOrEmpty(sqlServerLoginControl1.X_Result.DbFileLocation)
                || string.IsNullOrEmpty(sqlServerLoginControl1.X_Result.Server)
        )
                {
                    e.CanExecute = false;
                }
                else
                {
                    e.CanExecute = true;
                }
            }
            else
            {
                if (sqlServerLoginControl1.X_Result.Server.IsNotEmpty() &&
                    sqlServerLoginControl1.X_Result.UID.IsNotEmpty())
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        void SqlServerEntry_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> serverInfo = new List<string>();
            SerializeClass.DatabaseHistoryInfo.SqlServerHistory.ForEach(c => serverInfo.Add(c.ServerAddress));

            sqlServerLoginControl1.SetServerNameList(serverInfo);
        }


        [Description("Here the creation of MainEnginer should obey this rules ,do not refactor it ")]
        private void LoginInSqlServer(WPFCommonControl.SqlServerLoginControl.X_CollectionData controlResult)
        {
            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }

            LoginInfo_SqlServer info = new LoginInfo_SqlServer()
                {
                    X_Server = controlResult.Server,
                    X_Database = controlResult.DbName,
                    X_Pwd = controlResult.PWD,
                    X_UserName = controlResult.UID,
                    X_Port = controlResult.Port.ToString(),
                    X_CurDbConnectionMode = controlResult.CurType.CurConnMode,
                    IsTrustedConn = controlResult.IsTrust,
                    AttchFile=controlResult.DbFileLocation,
                };

            try
            {
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    #region Save to Opened History Info
                    SqlServerObjects sqlserverItem = new SqlServerObjects();
                    sqlserverItem.ServerAddress = info.X_Server;
                    if (info.X_Port.IsNotEmpty())
                    {
                        sqlserverItem.Port = int.Parse(info.X_Port);
                    }
                    sqlserverItem.Username = info.X_UserName;

                    if (!SerializeClass.DatabaseHistoryInfo.SqlServerHistory.IsContainSubValue(sqlserverItem.ServerAddress))
                    {
                        HistoryObject oldObject = SerializeClass.DatabaseHistoryInfo;
                        oldObject.SqlServerHistory.Add(sqlserverItem);

                        SerializeClass.DatabaseHistoryInfo = oldObject;
                    }
                    #endregion

                    App.MainEngineer.CurDatabase = controlResult.DbName;

                    RibbionIDE ide = new RibbionIDE();
                    //this.Visibility = Visibility.Hidden;
                    //ide.Closed += (a, b) => { this.Visibility = Visibility.Visible; };
                    ide.Title = info.X_Server;

                    ide.ShowDialog();

                }
            }
            catch (Exception ee)
            {
                App.ResetMainEngineer();
                ee.HandleMyException();
            }
        }

        private void butEmulateSqlServerInstance_Click(object sender, RoutedEventArgs e)
        {
            EnumSqlServer eServer = new EnumSqlServer();
            eServer.SelectedSqlServer += (MySender, MyArgs) =>
                {
                    this.sqlServerLoginControl1.SetServerName(((MyEventArgs)MyArgs).MyText);
                };
            eServer.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            eServer.ShowDialog();
        }

        private void butConnectWithConStr_Directly_Click(object sender, RoutedEventArgs e)
        {
            string connStr = string.Empty;
            connStr = this.txtConnStr.Text;
            if (string.IsNullOrEmpty(connStr))
            {
                "Please input the connection string".Notify();
                return;
            }

            CoreEA.CoreE c = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer);
            try
            {
                c.X_Handler.Open(connStr);

                if (c.X_Handler.IsOpened)
                {

                    RibbionIDE ide = new RibbionIDE();

                    ide.ShowDialog();
                }
                else
                {
                    MessageBox.Show(c.X_Handler.LastErrorMsg);
                }
            }
            catch (Exception ee)
            {
                XLCS.Common.ProcessException.DisplayErrors(ee);
            }
        }

        private void butCreateDb_Click(object sender, RoutedEventArgs e)
        {
            WPFCommonControl.SqlServerLoginControl.X_CollectionData controlResult =
this.sqlServerLoginControl1.X_Result;

            if (string.IsNullOrEmpty(controlResult.Server) || string.IsNullOrEmpty(controlResult.DbName)
                || string.IsNullOrEmpty(controlResult.UID))
            {
                "ImportEachElement".GetFromResourece().Notify();
                return;
            }

            CreateDatabase createDbWin = new CreateDatabase();
            createDbWin.Args = controlResult;
            createDbWin.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butBackupDb_Click(object sender, RoutedEventArgs e)
        {
            WPFCommonControl.SqlServerLoginControl.X_CollectionData controlResult =
    this.sqlServerLoginControl1.X_Result;

            if (string.IsNullOrEmpty(controlResult.Server) || string.IsNullOrEmpty(controlResult.DbName)
                || string.IsNullOrEmpty(controlResult.UID))
            {
                "ImportEachElement".GetFromResourece().Notify();
                return;
            }

            App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;

            LoginInfo_SqlServer info = new LoginInfo_SqlServer()
          {
              X_Server = controlResult.Server,
              X_Database = controlResult.DbName,
              X_Pwd = controlResult.PWD,
              X_UserName = controlResult.UID,
              X_Port = controlResult.Port.ToString(),
              X_CurDbConnectionMode = controlResult.CurType.CurConnMode,
              IsTrustedConn = controlResult.IsTrust,
          };
            PageSwitchProgressBar dp = null;
            try
            {
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    if (!"InfoBackupDb".GetFromResourece().Confirm())
                    {
                        return;
                    }

                    SaveFileDialog sf = new SaveFileDialog();
                    sf.Filter = MyGlobal.BackupDatabase_FILE_FILTER;


                    if ((bool)sf.ShowDialog())
                    {
                        string cmdStr = string.Format("BACKUP DATABASE {0} TO  DISK = N'{1}' WITH NOFORMAT, NOINIT,NAME = N'{2}', SKIP, NOREWIND, NOUNLOAD,  STATS = 10",
                            App.MainEngineer.CurDatabase, sf.FileName, "BackupDataFile");

                        IDbCommand cmd = App.MainEngineer.GetNewStringCommand(cmdStr);
                        cmd.CommandTimeout = int.MaxValue / 2;
                        dp = PageSwitchProgressBar.X_BeginLoadingDialog();

                        cmd.ExecuteNonQuery();
                        dp.X_EndLoadingDialog();
                        dp = null;
                        "InfoBackupSuccesful".GetFromResourece().Show();
                    }

                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
                if (null != dp)
                {
                    dp.X_EndLoadingDialog();
                }
                App.ResetMainEngineer();
            }
        }
    }
}
