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
using DBStudio.GlobalDefine;
using Microsoft.Win32;

using DBStudio.UI;
using System.IO;

namespace DBStudio.Sqlite
{
    /// <summary>
    /// Interaction logic for SqliteEntry.xaml
    /// </summary>
    public partial class SqliteEntry : UserControl
    {

        RoutedUICommand OpenCmd = new RoutedUICommand();

        public SqliteEntry()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(OpenCmd, OpenCmd_Executed, OpenCmd_CanExecuted));
            butOpen.Command = OpenCmd;

            FilePicker.X_OpenFileDialogFilter = MyGlobal.SQLite_FILE_FILTER;
        }

        void OpenCmd_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            butOpen_Click(this, null);
        }

        void OpenCmd_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePicker.X_FileName))
            { e.CanExecute = false; }
            else
            {
                e.CanExecute = true;
            }
        }
        private void butOpen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePicker.X_FileName))
            {
                "Please select file first".Notify();
                return;
            }

            OpenSqliteFile(FilePicker.X_FileName);
        }

        private void OpenSqliteFile(string srcFile)
        {
            if (!File.Exists(srcFile))
            {
                "FileNotFound".GetFromResourece().Show();
                return;
            }

            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;


                string dbName = srcFile;

                App.MainEngineer.Open
                    (new CoreEA.LoginInfo.LoginInfo_Sqlite()
                    {
                        DbFile = dbName,
                        Pwd = txtPwd.Password,
                        IsReadOnly = (bool)chkIsReadOnly.IsChecked,
                        IsUnicode = (bool)chkIsUnicode.IsChecked,
                    }
                    );
                if (App.MainEngineer.IsOpened)
                {

                    RibbionIDE ide = new RibbionIDE();

                    ide.WindowState = WindowState.Maximized;
                    ide.BringIntoView();
                    ide.ShowDialog();
                }
            }

            catch (Exception ee)
            {
                App.ResetMainEngineer();
                ee.HandleMyException();
            }

        }


        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateSqliteDb cs = new CreateSqliteDb();
            cs.ShowDialog();
        }

        private void butOpenExample_Click(object sender, RoutedEventArgs e)
        {
            string srcFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ExampleData\\Sqlite\\test.db3";
            OpenSqliteFile(srcFile);
        }
    }
}
