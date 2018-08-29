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
using System.Data;
using DBStudio.GlobalDefine;
using System.Collections.ObjectModel;
using CoreEA.SchemaInfo;
using ETL;

namespace DBStudio.CreateTableSchemaFactory
{
    /// <summary>
    /// Interaction logic for CreateTableSchema_SSCE.xaml
    /// </summary>
    public partial class CreateTableSchema_SSCE : CreateTableSchemaBaseWin
    {
        public CreateTableSchema_SSCE()
            : base(new BaseTableSchema())
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CreateTableSchema_SSCE_Loaded);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CreateTableSchema_SSCE_Loaded(object sender, RoutedEventArgs e)
        {
            InitDataSource();
        }

        ObservableCollection<CreateTableBindingArgs_SqlCe> bindingArgsList =
            new ObservableCollection<CreateTableBindingArgs_SqlCe>();

        private void InitDataSource()
        {
            #region Init DBType List
            List<SqlDbType> typeList = new List<SqlDbType>();
            for (int i = 0; i < 35; i++)
            {
                if ((i <= 28 && i >= 26)||(i==24))
                {
                    continue;
                }

                typeList.Add((SqlDbType)i);
            }

            columnTypeColumn.ItemsSource = typeList;
            #endregion 

            //Parse if base.CurSchemaInfo is has data
            if (base.CurSchemaInfo != null)
            {
                //read column
                foreach (var item in base.CurSchemaInfo.Columns)
                {
                    CreateTableBindingArgs_SqlCe args = new CreateTableBindingArgs_SqlCe()
                    {
                        ColumnName=item.ColumnName,
                        IsUnique=item.IsIdentity,
                        AllowNulls=item.IsNullable,
                        ColumnLength=(int)item.CharacterMaxLength,
                        //ColumnType=item.ColumnType,
                    };

                    foreach (var pkInfo in base.CurSchemaInfo.PrimaryKey)
                    {
                        if (pkInfo.ColumnName == item.ColumnName)
                        {
                            args.IsPK = true;
                            break;
                        }
                    }

                    bindingArgsList.Add(args);
                }
            }

            myGrid.DataContext = bindingArgsList;
        }

        private void cmdBar_PressCancelButton(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtTableName.Text.IsEmpty())
            {
                e.CanExecute = false;
                return;
            }

            if (bindingArgsList.Count > 0)
            {
                foreach (var item in bindingArgsList)
                {
                    if (item.ColumnName.IsEmpty())
                    {
                        e.CanExecute = false;
                        return;
                    }
                    
                }
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BaseTableSchema schemaInfo = new BaseTableSchema();
            schemaInfo.TableName = txtTableName.Text;
            
            foreach (var item in bindingArgsList)
            {
                if (item.IsPK)
                {
                    schemaInfo.PrimaryKey.Add(
                        new BasePrimaryKeyInfo()
                        {
                            ColumnName=item.ColumnName,
                        });
                }

                schemaInfo.Columns.Add(new BaseColumnSchema()
                {
                    ColumnName = item.ColumnName,
                    ColumnType = item.ColumnType.ToString(),

                    IsNullable = item.AllowNulls,
                    CharacterMaxLength = item.ColumnLength,
                    IsIdentity = item.IsUnique,
                });
            }

            //Valid Schema if UI is not good.

            if (base.IsModifyMode)
            {
            }
            else
            {
                if (App.MainEngineer.CreateTable(schemaInfo))
                {
                //if (base.NotifyRefreshTableAfterCreated != null)
                //{
                //    NotifyRefreshTableAfterCreated();
                //}
                DialogResult = true;
                }
                else
                {
                    "Create Error".Notify();

                }
            }
        }

    }//End of Class
}//End of namespace
