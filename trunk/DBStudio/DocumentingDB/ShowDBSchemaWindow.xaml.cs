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
using CoreEA;
using ETL;
using CoreEA.SchemaInfo;
using System.Collections.ObjectModel;
using DBStudio.GlobalDefine;
using CoreEA.GlobalDefine;

namespace DBStudio.DocumentingDB
{
    /// <summary>
    /// Interaction logic for DoDBSchema.xaml
    /// </summary>
    public partial class ShowDBSchemaWindow : UserControl, IStep
    {
        public ShowDBSchemaWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DoDBSchema_Loaded);
        }

        /// <summary>
        /// Generate the detail schemas 
        /// Important method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DoDBSchema_Loaded(object sender, RoutedEventArgs e)
        {
            DocDbObject myData = MyDocDataContext as DocDbObject;
            List<DBObject> dbList = new List<DBObject>();
            
            CoreEA.CoreE core = new CoreEA.CoreE(myData.SourceDbType);
            ICoreEAHander myHandler = null;
            myHandler = core.X_Handler;
            this.DataContext = null;

            try
            {
                myHandler.Open(myData.LoginInfo);
                if (myHandler.IsOpened)
                {
                    //Just process the selected table .
                    foreach (var table in myData.SelectedTableNameCollection)
                    {
                        BaseTableSchema mySchema = myHandler.GetTableSchemaInfoObject(table);
                        
                        foreach (var column in mySchema.Columns)
                        {
                            dbList.Add(new DBObject()
                            {
                                IsIdentity=column.IsIdentity,
                                TableName = table,
                                ColumnName = column.ColumnName,
                                DbType = column.ColumnType,
                                Length = column.CharacterMaxLength,
                                OrdinaryPosition = column.OrdinalPosition,
                                RevisionDate = DateTime.Now,
                                RevisionNote = "",
                                Category = "",
                                Format = 0,
                                IsPrimaryKey = mySchema.PrimaryKey.IsContaintPrimaryColumn(column.ColumnName),
                                Description = "",
                                IsIndex = (mySchema.Indexes.Where(c => c.ColumnName == column.ColumnName).Count() > 0),
                            });
                        }
                    }

                    this.DataContext = dbList;

                    myData.DbObjectList = dbList;
                }
                else
                {
                    throw new Exception("Can't open such data source");
                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
                if (myHandler.IsOpened)
                {
                    myHandler.Close();
                }
            }
    
        }

        #region IStep Members

        public object MyDocDataContext
        {
            get;
            set;
        }

        public bool CanLeave()
        {
            return true;
        }

        public void Leave()
        {

        }

        public object Result
        {
            get
            {
                return null;
            }
        }

        public bool IsSource
        {
            get
            {
                return false;
            }
        }

        #endregion

        private void chkIsEditable_Checked(object sender, RoutedEventArgs e)
        {
            myGrid.IsReadOnly = false;
        }

        private void chkIsEditable_Unchecked(object sender, RoutedEventArgs e)
        {
            myGrid.IsReadOnly = true;
        }
    }
}
