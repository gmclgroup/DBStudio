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
using DBStudio.BaseUI;
using ETL;
using DBStudio.GlobalDefine;
using System.Data;
using CoreEA.SchemaInfo;

namespace DBStudio
{
    /// <summary>
    /// Interaction logic for AddColumn
    /// </summary>
    public partial class AddColumn : BaseFadeDialog
    {
        static RoutedUICommand okCmd = new RoutedUICommand();
        internal TreeItemArgs Args { get; set; }

        public AddColumn(TreeItemArgs args)
        {
            InitializeComponent();

            initUI();
            Args = args;
            this.CommandBindings.Add(new CommandBinding(okCmd,okCmd_Execute,okCmd_CanExecute));
            cmdBar.OkButtonCommand = okCmd;
        }

        /// <summary>
        /// Here this method is not fullly complete.
        /// Currently the DBtype is complex for each database type
        /// We only complete the sqlserver based sqldbtype
        /// 
        /// </summary>
        private void initUI()
        {
            #region Init DBType List

            switch (App.MainEngineer.HostedType)
            {
                case CoreEA.CoreE.UsedDatabaseType.OleDb:
                    break;
                case CoreEA.CoreE.UsedDatabaseType.SqlServer:
                    App.MainEngineer.GetDbType().ForEach(item =>
                        {
                            columnType.Items.Add((SqlDbType)item);
                        });
                    break;
                case CoreEA.CoreE.UsedDatabaseType.MySql:
                    
                    break;
                case CoreEA.CoreE.UsedDatabaseType.SqlCE35:
                    App.MainEngineer.GetDbType().ForEach(item =>
                    {
                        columnType.Items.Add((SqlDbType)item);
                    });
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Sqlite:
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Firebird:
                    break;
                case CoreEA.CoreE.UsedDatabaseType.CSV:
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Excel:
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Oracle:
                    break;
                default:
                    break;
            }

            #endregion 
        }


        private void okCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((txtColumnName.Text.IsEmpty()) || (columnType.SelectedItem == null)
                )
            {
                e.CanExecute = false;
            }
            else
            {
                if (columnType.Text.ToLower().Contains("char"))
                {
                    if (txtLength.Text.IsEmpty())
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
                    e.CanExecute = true;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            string columnName=txtColumnName.Text;
            BaseColumnSchema columnSchema = new BaseColumnSchema();

            if (txtLength.Text.IsEmpty())
            {
                columnSchema.CharacterMaxLength = 0;
            }
            else
            {
                columnSchema.CharacterMaxLength = long.Parse(txtLength.Text);
            }
            columnSchema.ColumnName=columnName;
            columnSchema.ColumnType=columnType.Text.ToString();


            try
            {
                if (App.MainEngineer.AddColumnToTable(Args.TableName, columnSchema))
                {
                    DialogResult = true;
                }
            }
            catch (Exception ee)
            {
                ee.Notify();
            }

        }


        private void cmdBar_PressCancelButton(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
