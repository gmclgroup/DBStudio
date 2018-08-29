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
using CoreEA.LoginInfo;
using Microsoft.Win32;

namespace DBStudio.Sqlite
{
    /// <summary>
    /// Interaction logic for CreateSqliteDb.xaml
    /// </summary>
    public partial class CreateSqliteDb : BaseUI.BaseFadeDialog
    {
        public CreateSqliteDb()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(GlobalDefine.RoutedUICommands.ConnectCmd, CreateCmd_Execute, CreateCmd_CanExecute));
            butCreate.Command = GlobalDefine.RoutedUICommands.ConnectCmd;
        }
        public void CreateCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {

            string dbFile = txtSqlFile.Text;

            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;
            try
            {
                if (core.CreateDatabase(new LoginInfo_Sqlite() { DbFile = dbFile }))
                {
                    "Create OK".Notify();
                }
                else
                {
                    "Create Failure".Notify();
                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
                core.Dispose();
            }
        }

        public void CreateCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSqlFile.Text))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void butSelectSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = MyGlobal.SQLite_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {

                txtSqlFile.Text = sf.FileName;
            }
        }
    }
}
