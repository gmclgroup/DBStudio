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
using DBStudio.GlobalDefine;
using CoreEA.LoginInfo;
using DBStudio.Utility;
using ETL;
using CoreEA;

namespace DBStudio.OleDb
{
    /// <summary>
    /// Interaction logic for OleDbMain.xaml
    /// </summary>
    public partial class OleDbMain : UserControl
    {


        public OleDbMain()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(GlobalDefine.RoutedUICommands.ConnectCmd, ConnectCmd_Execute, ConnectCmd_CanExecute));
            butGo.Command = GlobalDefine.RoutedUICommands.ConnectCmd;

            Loaded += new RoutedEventHandler(OleDbMain_Loaded);
        }

        public void ConnectCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileInput.X_FileName))
            {
                return;
            }
            CoreEA.LoginInfo.OleDBVersion curVersion;
            if((bool)checkBoxVersion.IsChecked)
            {
                curVersion=OleDBVersion.Is2003;
            }
            else
            {
                curVersion=OleDBVersion.Is2007;
            }
            OpenOleDbFile(txtFileInput.X_FileName, curVersion);
        }

        private void OpenOleDbFile(string srcFile,CoreEA.LoginInfo.OleDBVersion curVersion)
        {
            if (!System.IO.File.Exists(srcFile))
            {
                "FileNotFound".GetFromResourece().Show();
                return;
            }

            LoginInfo_Oledb info = new LoginInfo_Oledb()
            {
                Database = srcFile,
                Pwd = pwd.Password,
                CurrentOleDBVersion=curVersion,
            };
            App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.OleDb).X_Handler;

            try
            {
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    OleDbObjects oleItem = new OleDbObjects();
                    oleItem.DbFileFullPath = info.Database;
                    if (!SerializeClass.DatabaseHistoryInfo.OleDbHistory.IsContainSubValue(oleItem.DbFileFullPath))
                    {
                        HistoryObject oldObject = SerializeClass.DatabaseHistoryInfo;
                        oldObject.OleDbHistory.Add(oleItem);

                        SerializeClass.DatabaseHistoryInfo = oldObject;
                    }

                    App.MainEngineer.CurDatabase = info.Database;
                    App.MainEngineer.CurPwd = info.Pwd;

                    pwd.Password = "";
                    txtFileInput.X_FileName = "";
                    RibbionIDE ide = new RibbionIDE();
                    ide.ShowDialog();
                }
            }
            catch (Exception ee)
            {
                App.ResetMainEngineer();
                ee.HandleMyException();
            }
            finally
            {
                
            }
        }

        public void ConnectCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtFileInput.X_HasText)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        void OleDbMain_Loaded(object sender, RoutedEventArgs e)
        {
            txtFileInput.X_OpenFileDialogFilter = MyGlobal.MDB_FILE_FILTER;
        }

        private void butExample_Click(object sender, RoutedEventArgs e)
        {
            string exampleFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ExampleData\\Access\\TestAccessDb2003.mdb";
            OpenOleDbFile(exampleFile,OleDBVersion.Is2003);
        }

        private void butExample2_Click(object sender, RoutedEventArgs e)
        {
            string exampleFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ExampleData\\Access\\TestAccessDb2007.accdb";
            OpenOleDbFile(exampleFile,OleDBVersion.Is2007);
        }
    }
}
