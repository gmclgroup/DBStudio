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
using System.Reflection;
using System.IO;
namespace DBStudio.Excel
{
    /// <summary>
    /// Interaction logic for ExcelEntry.xaml
    /// </summary>
    public partial class ExcelEntry : UserControl
    {
        public ExcelEntry()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(GlobalDefine.RoutedUICommands.ConnectCmd,
                ConnectCmd_Execute, ConnectCmd_CanExecute));
            butGo.Command = GlobalDefine.RoutedUICommands.ConnectCmd;
        }

        public void ConnectCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSrcFile.Text))
            {
                return;
            }
            CoreEA.LoginInfo.OleDBVersion curVersion;
            if ((bool)checkBoxVersion.IsChecked)
            {
                curVersion = OleDBVersion.Is2007;
            }
            else
            {
                curVersion = OleDBVersion.Is2003;
            }
            OpenExcelFile(txtSrcFile.Text,curVersion);
        }

        private void OpenExcelFile(string srcFile, CoreEA.LoginInfo.OleDBVersion curVersion)
        {
            if (!File.Exists(srcFile))
            {
                "FileNotFound".GetFromResourece().Show();
                return;
            }
            LoginInfo_Excel info = new LoginInfo_Excel()
            {
                Database = srcFile,
                IsFirstRowIsColumnName = (bool)chkIsFirstRowIsColumnName.IsChecked,
                CurrentOleDBVersion=curVersion,
            };

            App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Excel).X_Handler;

            try
            {
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    App.MainEngineer.CurDatabase = info.Database;
                    App.MainEngineer.CurPwd = info.Pwd;
                    RibbionIDE ide = new RibbionIDE();
                    ide.ShowDialog();
                }
            }
            catch (Exception ee)
            {
                App.ResetMainEngineer();
                ee.HandleMyException();
            }
        }

        public void ConnectCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtSrcFile.Text.IsEmpty())
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        
        private void butSelectSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.EXCEL_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                this.txtSrcFile.Text = sf.FileName;
            }
        }

        private void butExample2007_Click(object sender, RoutedEventArgs e)
        {
            string scrFile= AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ExampleData\\Excel\\TestExcelData2003.xls";
            
            OpenExcelFile(scrFile,OleDBVersion.Is2003);
        }

        private void butExample2003_Click(object sender, RoutedEventArgs e)
        {

            string scrFile= AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ExampleData\\Excel\\TestExcelData2007.xlsx";
            
            OpenExcelFile(scrFile,OleDBVersion.Is2007);
        }

    }
}
