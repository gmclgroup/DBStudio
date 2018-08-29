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
using System.Windows.Controls;
using DBStudio.GlobalDefine;
using System.Data;
using CustomControl.NewXLAGControl;
using System.Windows;
using DBStudio.AdditionUI;
using DBStudio.CommonUI;
using DBStudio.CreateTableSchemaFactory;
using CoreEA.SchemaInfo;
using MPL.MyControls;
using System.Diagnostics;
using ETL;

namespace DBStudio.ContextMenuFactory
{
    internal abstract class MenuBase : IMenu
    {
        public TreeItemArgs X_CurItemArgs { get; set; }

        public event EventHandler ItemClickWithSqlCmdEvent;
        public event EventHandler ItemClickWithResult;

        public event EventHandler RefreshSystemViewNodeEvent;
        public event EventHandler RefreshAllNodeEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CmdStr"></param>
        /// <param name="isExecuteCmd"></param>
        public void X_CallNotifyEvent(string CmdStr, bool isExecuteCmd)
        {
            if (ItemClickWithSqlCmdEvent != null)
            {
                ItemClickWithSqlCmdEvent(CmdStr, new NotifySqlCmdArgs() { IsExecuteCommand = isExecuteCmd });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        public void X_CallShowResultEvent(DataTable ds)
        {
            if (ItemClickWithResult != null)
            {
                ItemClickWithResult(ds, null);
            }
        }

        #region IMenu Members

        public virtual ContextMenu CreateMenuForDataBase()
        {
            return CreateMenuForNotDefined();
        }

        public virtual ContextMenu CreateMenuForTable()
        {
            ContextMenu ctMenu = new ContextMenu();

            #region select all
            MenuItem itemSelectAll = new MenuItem();
            itemSelectAll.Header = "TitleSelectAll".GetFromResourece();
            itemSelectAll.Click += (s, e) =>
            {
                string cmdStr = string.Format("Select * from {0}", App.MainEngineer.GetMaskedTableName(X_CurItemArgs.TableName));
                X_CallNotifyEvent(cmdStr, true);
            };
            ctMenu.Items.Add(itemSelectAll);
            #endregion
            ctMenu.Items.Add(new Separator());

            #region Select
            MenuItem itemSelect = new MenuItem();
            itemSelect.Header = "TitleSelect".GetFromResourece();
            itemSelect.Click += (s1, e1) =>
            {
                X_CallNotifyEvent(String.Format("SELECT * FROM {0}", App.MainEngineer.GetMaskedTableName(X_CurItemArgs.TableName)), false);
            };
            ctMenu.Items.Add(itemSelect);
            #endregion

            #region Insert
            MenuItem itemInsert = new MenuItem();
            itemInsert.Header = "TitleInsert".GetFromResourece();
            itemInsert.Click += delegate
            {
                itemInsert_Click(X_CurItemArgs.TableName);
            };
            ctMenu.Items.Add(itemInsert);
            #endregion

            #region Update
            MenuItem itemUpdate = new MenuItem();
            itemUpdate.Header = "TilteUpdate".GetFromResourece();
            itemUpdate.Click += delegate
            {
                string cmdStr = string.Empty;
                try
                {
                    List<string> columnlist = App.MainEngineer.GetColumnNameListFromTable(X_CurItemArgs.TableName);

                    string columnP = string.Empty;
                    foreach (string item in columnlist)
                    {
                        columnP += string.Format("{0}={1},", item, "COLUMN_VALUE");
                    }

                    columnP = columnP.Substring(0, columnP.Length - 1);

                    columnP += " where WHERE_CALUSE";
                    cmdStr = String.Format("UPDATE {0} SET {1}", App.MainEngineer.GetMaskedTableName(X_CurItemArgs.TableName), columnP);
                }
                catch (Exception ee)
                {
                    ee.HandleMyException();
                    return;
                }

                X_CallNotifyEvent(cmdStr, false);
            };
            ctMenu.Items.Add(itemUpdate);
            #endregion

            #region Delete
            MenuItem itemDelete = new MenuItem();
            itemDelete.Header = "TitleDelete".GetFromResourece();
            itemDelete.Click += delegate
            {
                string cmdStr = String.Format("Delete from {0} WHERE (COLUMN_NAME) = (COLUMN_VALUE) ",
                    App.MainEngineer.GetMaskedTableName(X_CurItemArgs.TableName));

                X_CallNotifyEvent(cmdStr, false);
            };
            ctMenu.Items.Add(itemDelete);
            #endregion

            ctMenu.Items.Add(new Separator());

            #region Insert Row
            MenuItem itemInsertDataRow = new MenuItem();
            itemInsertDataRow.Header = "TitleInsertARow".GetFromResourece();
            itemInsertDataRow.Click += delegate { itemInsertDataRow_Click(); };
            ctMenu.Items.Add(itemInsertDataRow);
            #endregion

            #region NewTable
            ctMenu.Items.Add(new Separator());

            MenuItem itemCreateTable = new MenuItem();
            itemCreateTable.Header = "CreatTableDialogTitle".GetFromResourece();
            itemCreateTable.Click += new RoutedEventHandler(itemCreateTable_Click);
            ctMenu.Items.Add(itemCreateTable);
            #endregion


            #region Rename Table
            MenuItem itemRenameTable = new MenuItem();
            itemRenameTable.Header = "TitleRenameTable".GetFromResourece();
            itemRenameTable.Click += delegate
            {
                string newTableName = string.Empty;
                InputValueWindows iv = new InputValueWindows("Table Name");
                if (iv.ShowDialog() == true)
                {
                    newTableName = iv.X_GetInputedValue.ToString();
                }
                else
                {
                    return;
                }
                if (string.IsNullOrEmpty(newTableName))
                {
                    "TitleCanNotEmptyNewTableName".GetFromResourece().Warning();
                    return;
                }

                string cmdStr =App.MainEngineer.CurrentCommandTextHandler.GetRenameTableCmdStr(X_CurItemArgs.TableName, newTableName);
                X_CallNotifyEvent(cmdStr, true);

            };
            ctMenu.Items.Add(itemRenameTable);

            MenuItem copyTableItem = new MenuItem();
            copyTableItem.Header = "TitleCopyTable".GetFromResourece();
            copyTableItem.Click += delegate
            {
                try
                {
                    string newTableName = string.Empty;
                    InputValueWindows iv = new InputValueWindows("Table Name");
                    if (iv.ShowDialog() == true)
                    {
                        newTableName = iv.X_GetInputedValue.ToString();
                    }

                    if (string.IsNullOrEmpty(newTableName))
                    {
                        ("TitleCanNotEmptyNewTableName".GetFromResourece()).Warning();
                        return;
                    }

                    List<string> allTableList = App.MainEngineer.GetTableListInDatabase();
                    if (allTableList.Contains(newTableName))
                    {
                        "InfoTableNameExisted".GetFromResourece().Show();
                        return;
                    }

                    // oldTableName = App.MainEngineer.GetMaskedTableName(oldTableName);
                    // newTableName = App.MainEngineer.GetMaskedTableName(newTableName);
                    //If copy successful
                    if (CopyTable(X_CurItemArgs.TableName,newTableName))
                    {
                        "TitleCopyDataSuccessful".GetFromResourece().Show();
                        if (RefreshAllNodeEvent != null)
                        {
                            RefreshAllNodeEvent(null, null);
                        }
                    }
                }
                catch (Exception ee)
                {
                    ee.HandleMyException();
                }
            };
            ctMenu.Items.Add(copyTableItem);

            #endregion

            #region Drop Table
            MenuItem itemDeleteTable = new MenuItem();
            itemDeleteTable.Header = "TitleDropTable".GetFromResourece();
            itemDeleteTable.Click += delegate
            {
                if ("ConfirmTextDropTable".GetFromResourece().Confirm())
                {
                    string cmdStr = string.Empty;
                    if ("ConfirmTextDropTableWithCascade".GetFromResourece().Confirm())
                    {
                        cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropTableCmdStrWithCascade(X_CurItemArgs.TableName);
                    }
                    else
                    {
                        cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropTableCmdStr(X_CurItemArgs.TableName);
                    }
                    X_CallNotifyEvent(cmdStr, true);
                }
            };
            ctMenu.Items.Add(itemDeleteTable);
            #endregion

            #region Modify Schema
            MenuItem itemModifySchema = new MenuItem();
            itemModifySchema.Header = "TitleModifyTableSchema".GetFromResourece();
            itemModifySchema.Click += delegate
            {
                //CreateTableDetailWindow ct = new CreateTableDetailWindow(CreateTableDetailWindow.EnteringType.ModifySchema,
                //X_CurItemArgs.TableName);
                //ct.ShowDialog();
                //X_CurItemArgs.TableName
                BaseTableSchema schemaInfo = App.MainEngineer.GetTableSchemaInfoObject(X_CurItemArgs.TableName);
                CreateTableSchemaBaseWin win = GetCreateTableSchemaWindow();
                win.CurSchemaInfo = schemaInfo;
                win.IsModifyMode = true;
                //If Successful then refresh table list
                if ((bool)win.ShowDialog())
                {
                    if (RefreshAllNodeEvent != null)
                    {
                        RefreshAllNodeEvent(null, null);
                    }
                }

            };
            ctMenu.Items.Add(itemModifySchema);
            #endregion

            ctMenu.Items.Add(new Separator());

            #region GetAllColumnInfo
            MenuItem itemGetColumnsInfo = new MenuItem();
            itemGetColumnsInfo.Header = "TitleGetColumnInfo".GetFromResourece();
            itemGetColumnsInfo.Click += delegate
            {
                X_CallShowResultEvent(App.MainEngineer.GetColumnInfoFromTable(X_CurItemArgs.TableName));
            };
            ctMenu.Items.Add(itemGetColumnsInfo);
            #endregion

            #region GetAllIndexes
            MenuItem itemGetIndexesInfo = new MenuItem();
            itemGetIndexesInfo.Header = "TitleGetIndexesInfo".GetFromResourece();
            itemGetIndexesInfo.Click += delegate
            {
                X_CallShowResultEvent(App.MainEngineer.GetIndexInfoFromTable(X_CurItemArgs.TableName));
            };
            ctMenu.Items.Add(itemGetIndexesInfo);
            #endregion

            ctMenu.Items.Add(new Separator());

            #region GetSupportType
            MenuItem itemProviderInfo = new MenuItem();
            itemProviderInfo.Header = "TitleGetSupportedType".GetFromResourece();
            itemProviderInfo.Click += delegate
            {
                X_CallShowResultEvent(App.MainEngineer.GetProviderInfoFromTable(X_CurItemArgs.TableName));

            };
            ctMenu.Items.Add(itemProviderInfo);
            #endregion

            return ctMenu;
        }


        private void itemInsertDataRow_Click()
        {

            string tableName = X_CurItemArgs.TableName;
            string args = string.Empty;

            DataTable dataRowCollections = App.MainEngineer.GetColumnInfoFromTable(tableName);
            args += "( ";
            foreach (DataRow item in dataRowCollections.Rows)
            {
                args += item["COLUMN_NAME"] + ",";
            }
            args = args.Substring(0, args.Length - 1);

            args += ") VALUES (";

            for (int i = 0; i < dataRowCollections.Rows.Count; i++)
            {
                switch (dataRowCollections.Rows[i]["DATA_TYPE"].ToString())
                {
                    case "int":
                        args += String.Format(" {0},", i.ToString());
                        break;

                    default:
                        args += String.Format(" '{0}',", i.ToString());
                        break;
                }


            }

            args = args.Substring(0, args.Length - 1);

            args += " )";

            string cmdStr = String.Format("INSERT INTO {0} {1}", App.MainEngineer.GetMaskedTableName(tableName), args);

            X_CallNotifyEvent(cmdStr, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnNameList"></param>
        private void itemInsert_Click(string tableName)
        {
            DataTable ds = App.MainEngineer.GetColumnInfoFromTable(tableName);
            Dictionary<string, string> columnInfo = new Dictionary<string, string>();

            foreach (DataRow dt in ds.Rows)
            {
                columnInfo[dt["COLUMN_NAME"].ToString()] = dt["DATA_TYPE"].ToString();
            }

            string columnP = string.Empty;
            string columnValue = string.Empty;

            foreach (KeyValuePair<string, string> item in columnInfo)
            {
                columnP += string.Format("{0},", item.Key);

                columnValue += string.Format("{0},", GlobalDefine.MyGlobal.ConvertToTargetValue(item.Value));
            }


            columnP = columnP.Substring(0, columnP.Length - 1);
            columnValue = columnValue.Substring(0, columnValue.Length - 1);

            string cmdStr = String.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                App.MainEngineer.GetMaskedTableName(X_CurItemArgs.TableName), columnP, columnValue);

            X_CallNotifyEvent(cmdStr, false);
            //txtMainCmd.Focus();
            //txtMainCmd.Select(txtMainCmd.Text.Length - 14, 14);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ContextMenu CreateMenuForStoredProducers()
        {
            ContextMenu ctMenu = new ContextMenu();

            #region Run Sp
            MenuItem itemRunSP = new MenuItem();
            itemRunSP.Header = "TitleExecuteSP".GetFromResourece();
            itemRunSP.Click += delegate
            {
                ExecuteSp exeSpWin = new ExecuteSp();
                exeSpWin.CurrentSelectItemDataContext = X_CurItemArgs;

                exeSpWin.ShowDialog();

            };
            ctMenu.Items.Add(itemRunSP);

            #endregion

            #region View Region
            MenuItem itemViewSp = new MenuItem();
            itemViewSp.Header = "TitleViewSP".GetFromResourece();
            itemViewSp.Click += delegate
            {
                "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
            };
            ctMenu.Items.Add(itemViewSp);
            #endregion

            return ctMenu;
        }

        public virtual ContextMenu CreateMenuForIndexParentNode()
        {
            return CreateMenuForNotDefined();
        }

        public virtual ContextMenu CreateMenuForColumnParentNode()
        {
            return CreateMenuForNotDefined();
        }

        public virtual ContextMenu CreateMenuForIndex()
        {
            ContextMenu ctMenu = new ContextMenu();

            #region Drop Index

            MenuItem itemDropIndex = new MenuItem();
            itemDropIndex.Header = "TitleDropIndex".GetFromResourece();
            itemDropIndex.Click += delegate
            {
                if ("ConfirmTextDropIndex".GetFromResourece().Confirm())
                {
                    string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropIndexCmdStr(X_CurItemArgs.TableName, X_CurItemArgs.IndexName);

                    X_CallNotifyEvent(cmdStr, true);
                }

            };

            ctMenu.Items.Add(itemDropIndex);
            #endregion

            #region Rebuild Index

            #endregion

            return ctMenu;
        }

        public virtual ContextMenu CreateMenuForView()
        {
            ContextMenu ctMenu = new ContextMenu();

            MenuItem itemDropIndex = new MenuItem();
            itemDropIndex.Header = "Execute View";
            itemDropIndex.Click += delegate
            {

                string cmdStr = "select * from " + App.MainEngineer.GetMaskedTableName(X_CurItemArgs.ViewName);

                X_CallNotifyEvent(cmdStr, true);

            };

            ctMenu.Items.Add(itemDropIndex);

            return ctMenu;
        }

        public virtual ContextMenu CreateMenuForColumn()
        {
            ContextMenu ctMenu = new ContextMenu();

            MenuItem itemDropColumn = new MenuItem();
            itemDropColumn.Header = "Drop Column";

            itemDropColumn.Click += delegate
            {
                if ("ConfirmTextDropColumn".GetFromResourece().Confirm())
                {
                    string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropColumnCmdStr(X_CurItemArgs.TableName, X_CurItemArgs.ColumnName);

                    X_CallNotifyEvent(cmdStr, true);
                }
            };

            ctMenu.Items.Add(itemDropColumn);

            MenuItem itemRenameColumn = new MenuItem();
            itemRenameColumn.Header = "Rename Column";
            itemRenameColumn.Click += delegate
            {
                string newColumnName = string.Empty;
                //using (PopupForm p = new PopupForm())
                //{
                //    p.X_NotifyStr = "New Column name";
                //    if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //    {
                //        newColumnName = p.X_Result;
                //    }
                //    else
                //    {
                //        return;
                //    }
                //}

                InputValueWindows iv = new InputValueWindows("New Column name");
                if (iv.ShowDialog() == true)
                {
                    newColumnName = iv.X_GetInputedValue.ToString();
                }
                else
                {
                    return;

                }

                string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetRenameColumnCmdStr(X_CurItemArgs.TableName, X_CurItemArgs.ColumnName, newColumnName);

                X_CallNotifyEvent(cmdStr, true);
            };

            ctMenu.Items.Add(itemRenameColumn);

            MenuItem itemModidyTypeColumn = new MenuItem();
            itemModidyTypeColumn.Header = "Modify Column";
            itemModidyTypeColumn.Click += delegate
            {
                string cmdStr = String.Format("ALTER TABLE [{0}]  ALTER COLUMN {1} [NewType] ",
                    X_CurItemArgs.TableName, X_CurItemArgs.ColumnName);
                X_CallNotifyEvent(cmdStr, false);
            };
            ctMenu.Items.Add(itemModidyTypeColumn);

            MenuItem itemInsertColumn = new MenuItem();
            itemInsertColumn.Header = "Insert Column";
            itemInsertColumn.Click += delegate
            {
                string cmdStr = String.Format("ALTER TABLE [{0}] ADD COLUMN [ColumnName] [ColumnType]", X_CurItemArgs.TableName);

                X_CallNotifyEvent(cmdStr, false);
            };

            ctMenu.Items.Add(itemInsertColumn);

            ctMenu.Items.Add(new Separator());

            MenuItem itemCreateIndex = new MenuItem();
            itemCreateIndex.Header = "Create Index";
            itemCreateIndex.Click += delegate { itemCreateIndex_Click(); };
            ctMenu.Items.Add(itemCreateIndex);

            ctMenu.Items.Add(new Separator());
            MenuItem itemCopyColumnName = new MenuItem();
            itemCopyColumnName.Header = "Copy Column Name";
            itemCopyColumnName.Click += delegate
            {
                Clipboard.SetText(X_CurItemArgs.ColumnName);
            };
            ctMenu.Items.Add(itemCopyColumnName);


            return ctMenu;
        }

        private void itemCreateIndex_Click()
        {
            string newIndexName = string.Empty;

        ReDo:
            //using (PopupForm p = new PopupForm())
            //{
            //    p.X_NotifyStr = "Index name";
            //    if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        newIndexName = p.X_Result;
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            InputValueWindows iv = new InputValueWindows("New Index name");
            if (iv.ShowDialog() == true)
            {
                newIndexName = iv.X_GetInputedValue.ToString();
            }
            else
            {
                return;

            }

            if ((newIndexName.Length > 20) || (string.IsNullOrEmpty(newIndexName)))
            {
                MessageBox.Show(String.Format("Index name length must less than {0} and large than 0 ", 20));
                goto ReDo;
            }

            string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetCreateIndexCmdStr(newIndexName, X_CurItemArgs.TableName, X_CurItemArgs.ColumnName);

            X_CallNotifyEvent(cmdStr, true);
        }

        public virtual ContextMenu CreateMenuForSPParentNode()
        {
            return CreateMenuForNotDefined();
        }

        public virtual ContextMenu CreateMenuForSViewNode()
        {
            return MakeForSystemViewMenu();
        }

        public virtual ContextMenu CreateMenuForSV_SchemaNode()
        {
            return MakeForSystemViewMenu();
        }

        public virtual ContextMenu CreateMenuForSV_Schema_ColumnNode()
        {
            return MakeForSystemViewMenu();
        }

        private ContextMenu MakeForSystemViewMenu()
        {
            ContextMenu ctMenu = new ContextMenu();
            MenuItem refreshItem = new MenuItem();
            refreshItem.Header = "Refresh";
            refreshItem.Click += (s, e) =>
            {
                if (RefreshSystemViewNodeEvent != null)
                {
                    RefreshSystemViewNodeEvent(null, null);
                }
            };
            ctMenu.Items.Add(refreshItem);

            return ctMenu;
        }

        public virtual ContextMenu CreateMenuForNotDefined()
        {
            ContextMenu ctMenu = new ContextMenu();

            MenuItem itemCreateTable = new MenuItem();
            itemCreateTable.Header = "Create Table";
            itemCreateTable.Click += new RoutedEventHandler(itemCreateTable_Click);
            ctMenu.Items.Add(itemCreateTable);

            MenuItem refreshItem = new MenuItem();
            refreshItem.Header = "Refresh";
            refreshItem.Click += (s, e) =>
            {
                if (RefreshAllNodeEvent != null)
                {
                    RefreshAllNodeEvent(null, null);
                }
            };
            ctMenu.Items.Add(refreshItem);

            return ctMenu;
        }



        /// <summary>
        /// Create Table 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void itemCreateTable_Click(object sender, RoutedEventArgs e)
        {
            SwitchToCreateTable st = new SwitchToCreateTable();
            st.ShowDialog();

            switch (st.CurCreateType)
            {
                case SwitchToCreateTable.CreateType.NotSelected:
                    return;
                case SwitchToCreateTable.CreateType.Simple:
                    st.Close();
                    goto ReDo;
                case SwitchToCreateTable.CreateType.Normal:

                    st.Close();
                    CreateTableSchemaBaseWin win = GetCreateTableSchemaWindow();
                    //If Successful then refresh table list
                    if ((bool)win.ShowDialog())
                    {

                        if (RefreshAllNodeEvent != null)
                        {
                            RefreshAllNodeEvent(null, null);
                        }
                    }

                    return;
                default:
                    return;
            }



        ReDo:
            string tableName = string.Empty;
            //using (PopupForm p = new PopupForm())
            //{
            //    p.X_NotifyStr = "Table name";
            //    if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        tableName = p.X_Result;
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            InputValueWindows iv = new InputValueWindows("Table name");
            if (iv.ShowDialog() == true)
            {
                tableName = iv.X_GetInputedValue.ToString();
            }
            else
            {
                return;

            }

            if (tableName.Length > App.MainEngineer.MaxTableNameLength)
            {
                MessageBox.Show(String.Format("Table name length must large than {0} ", App.MainEngineer.MaxTableNameLength));

                goto ReDo;
            }
            if (string.IsNullOrEmpty(tableName))
            {
                "Please input the table name".Notify();
                goto ReDo;
            }

            string cmdStr = String.Format("Create table [{0}] (ID int , Comment nvarchar(255))", tableName);

            X_CallNotifyEvent(cmdStr, true);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enterType"></param>
        /// <returns></returns>
        public abstract CreateTableSchemaBaseWin GetCreateTableSchemaWindow();

        /// <summary>
        /// Copy data from old table to new table
        /// </summary>
        /// <param name="oldTableName"></param>
        /// <param name="newTableName"></param>
        /// <returns></returns>
        public virtual bool CopyTable(string oldTableName,string newTableName)
        {
            bool ret = false;
            try
            {
                BaseTableSchema tableInfo = App.MainEngineer.GetTableSchemaInfoObject(oldTableName);
                tableInfo.TableName = newTableName;
                //Also update the indexes info
                foreach (var item in tableInfo.Indexes)
                {
                    item.TableName = newTableName;
                }

                if (App.MainEngineer.CreateTable(tableInfo))
                {
                    oldTableName = App.MainEngineer.GetMaskedTableName(oldTableName);
                    newTableName = App.MainEngineer.GetMaskedTableName(newTableName);
                    App.MainEngineer.DoExecuteNonQuery("insert into " + newTableName + " select * from " + oldTableName);

                    ret = true;
                }

            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }

            return ret;
        }
    }//Endof Class
}//Endof Namespace
