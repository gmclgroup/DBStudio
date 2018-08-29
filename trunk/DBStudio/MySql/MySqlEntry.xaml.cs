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
using ETL;
using DBStudio.GlobalDefine;
using DBStudio.Utility;
using DBStudio.SqlServer;
using WPFCommonControl;
using CoreEA.LoginInfo;
using CoreEA;

namespace DBStudio.MySql
{
    /// <summary>
    /// Interaction logic for MySqlEntry.xaml
    /// </summary>
    public partial class MySqlEntry : UserControl
    {
        #region Here are standalone commands
        static RoutedUICommand ConnectCmd = new RoutedUICommand();
        static RoutedUICommand CreateCmd = new RoutedUICommand();
        static RoutedUICommand DeleteCmd = new RoutedUICommand();
        #endregion

        List<string> currentDbList = new List<string>();
        List<string> currentCharsetList = new List<string>();

        public MySqlEntry()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MySqlEntry_Loaded);
            passwordBox1.LostFocus += new RoutedEventHandler(passwordBox1_LostFocus);
            this.CommandBindings.Add(new CommandBinding(ConnectCmd, ConnectCmd_Executed, ConnectCmd_CanExecuted));
            this.CommandBindings.Add(new CommandBinding(DeleteCmd, DeleteCmd_Executed, DeleteCmd_CanExecuted));
            this.CommandBindings.Add(new CommandBinding(CreateCmd, CreateCmd_Executed, CreateCmd_CanExecuted));

            butConnect.Command = ConnectCmd;
            butDeleteDB.Command = DeleteCmd;
            butCreateDB.Command = CreateCmd;
        }

        void passwordBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            //Do  not enumerate the database because it is annoying.
            //  InitDBs();
        }

        void MySqlEntry_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            txtServername.Text = "localhost";
            txtUsername.Text = "root";
#endif

            #region Load opend db history info
            SerializeClass.DatabaseHistoryInfo.MySqlHistory.ForEach(c =>
              txtServername.Items.Add(c.ServerAddress));
            #endregion

            #region Load charset info
            EncodingInfo[] eInfos = Encoding.GetEncodings();
            
            currentCharsetList.Clear();
            foreach (EncodingInfo item in eInfos)
            {
                txtCharset.Items.Add(item.Name);
                currentCharsetList.Add(item.Name);
            }
            txtCharset.Text = Encoding.Default.BodyName;
            #endregion
        }

        private void InitDBs()
        {
            if (string.IsNullOrEmpty(txtServername.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                return;
            }


            LoginInfo_MySql info = new LoginInfo_MySql()
            {
                Server = txtServername.Text,
                Username = txtUsername.Text,
                Pwd = passwordBox1.Password,
                Port = int.Parse(txtPort.Text),
            };

            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    List<string> dblist = App.MainEngineer.GetDatabaseList();
                    txtDbName.ItemsSource = null;
                    
                    txtDbName.ItemsSource = dblist;
                    currentDbList.Clear();
                    currentDbList = dblist;
                }

            }
            catch (Exception ce)
            {
                ce.Message.Show();
            }
            finally
            {
                App.ResetMainEngineer();
            }

        }

        void ConnectCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServername.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                return;
            }

            LoginInfo_MySql info = new LoginInfo_MySql()
            {
                Server = txtServername.Text,
                Database = txtDbName.Text,
                Username = txtUsername.Text,
                Pwd = passwordBox1.Password,
                Port = int.Parse(txtPort.Text),
                ConnectionTimeOut = 2000,
            };

            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    #region Save to Opened History Info
                    MySqlObjects mysqlItem = new MySqlObjects();
                    mysqlItem.ServerAddress = info.Server;
                    mysqlItem.Port = info.Port;
                    mysqlItem.Username = info.Username;

                    if (!SerializeClass.DatabaseHistoryInfo.MySqlHistory.IsContainSubValue(mysqlItem.ServerAddress))
                    {
                        HistoryObject oldObject = SerializeClass.DatabaseHistoryInfo;
                        oldObject.MySqlHistory.Add(mysqlItem);

                        SerializeClass.DatabaseHistoryInfo = oldObject;
                    }
                    #endregion
                    App.MainEngineer.CurDatabase = txtDbName.Text;
                    App.MainEngineer.CurPwd = passwordBox1.Password;
                    RibbionIDE ide = new RibbionIDE();

                    ide.Title = info.Server;

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

        void ConnectCmd_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServername.Text))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

        }

        void DeleteCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ("DeleteDatabaseNofityMsg".GetFromResourece().Confirm())
            {
                //Should notice there we didn't specify database name
                LoginInfo_MySql info = new LoginInfo_MySql()
                {
                    Server = txtServername.Text,
                    Username = txtUsername.Text,
                    Pwd = passwordBox1.Password,
                    Port = int.Parse(txtPort.Text),
                };


                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                App.MainEngineer.Open(info);
                try
                {
                    if (App.MainEngineer.IsOpened)
                    {
                        App.MainEngineer.DoExecuteNonQuery(
                            string.Format("Drop database {0}",
                            txtDbName.Text));

                        "DeleteDatabaseOkMsg".GetFromResourece().Show();
             
                    }
                }
                catch (Exception ee)
                {
                    ee.Message.Show();
                }
                finally
                {
                    App.ResetMainEngineer();
                }

                InitDBs();
            }
        }

        void DeleteCmd_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((!string.IsNullOrEmpty(txtDbName.Text)) && (currentDbList.Contains(txtDbName.Text)))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        void CreateCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            if ("CreateDatabaseNotifyMsg".GetFromResourece().Confirm())
            {
                //Should notice there we didn't specify database name
                LoginInfo_MySql info = new LoginInfo_MySql()
                 {
                     Server = txtServername.Text,
                     Username = txtUsername.Text,
                     Pwd = passwordBox1.Password,
                     Port = int.Parse(txtPort.Text),
                 };


                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                App.MainEngineer.Open(info);
                try
                {
                    if (App.MainEngineer.IsOpened)
                    {
                        App.MainEngineer.DoExecuteNonQuery(
                            string.Format("Create database {0} CHARACTER SET {1}",
                            txtDbName.Text,
                            txtCharset.Text));

                        "CreateDatabaseOkMsg".GetFromResourece().Show();
                    }
                }
                catch (Exception ee)
                {
                    ee.Message.Show();
                }
                finally
                {
                    App.ResetMainEngineer();
                }

                InitDBs();
            }

        }


        /// <summary>
        /// Only when create database is enabled ,then show the charset layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CreateCmd_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            //Here is complex,
            //My thinking is :
            //the charset can be empty ,if empty then do as default
            //if not empty ,then should check it is valid or not .
            if ((!string.IsNullOrEmpty(txtDbName.Text)) && 
                (!currentDbList.Contains(txtDbName.Text))&&
                (
                (string.IsNullOrEmpty(txtCharset.Text))||(currentCharsetList.Contains(txtCharset.Text))
                )
                )
            {
                if (charsetLayer.Visibility == System.Windows.Visibility.Collapsed)
                {
                    charsetLayer.Visibility = System.Windows.Visibility.Visible;
                }

                e.CanExecute = true;
            }
            else
            {
                if (charsetLayer.Visibility == System.Windows.Visibility.Visible)
                {
                    charsetLayer.Visibility = System.Windows.Visibility.Collapsed;
                }
                e.CanExecute = false;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDbName_DropDownOpened(object sender, EventArgs e)
        {
            if (txtDbName.Items.Count < 1)
            {
                InitDBs();
            }
        }

        private void butSaveDBHistory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServername.Text))
            {
                return;
            }


        }

    }
}
