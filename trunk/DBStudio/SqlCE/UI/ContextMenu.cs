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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBStudio.CommonMethod;
using System.Diagnostics;
using System.Data.Common;
using System.Data;
using CustomControl.NewXLAGControl;
using DBStudio.AdditionUI;
using System.ComponentModel;
using DBStudio.GlobalDefine;

namespace DBStudio.UI
{
    /// <summary>
    /// For ContextMenu Class
    /// </summary>
    public partial class CEMain
    {

        //private ContextMenu MakeContextmenu_ForSystemSchemaNode()
        //{
        //    ContextMenu ctMenu = new ContextMenu();

        //    MenuItem getItem = new MenuItem();
        //    getItem.Header = "Select Schema Data";
        //    getItem.Click += (s,e)=>
        //    {
        //        txtMainCmd.Text = "select * from " + GetArgsFromSelectedItem().SystemViewName;

        //        butQueryClick(null, null);

        //        txtMainCmd.Text = string.Empty;
        //    };
        //    ctMenu.Items.Add(getItem);

        //    MenuItem refreshItem = new MenuItem();
        //    refreshItem.Header = "Refresh";
        //    refreshItem.Click += (s, e) =>
        //        {
                    
        //        };
        //    ctMenu.Items.Add(refreshItem);

        //    return ctMenu;
        //}

        //private ContextMenu MakeContextmenu_ForAllIndex()
        //{
        //    ContextMenu ctMenu = new ContextMenu();


        //    MenuItem itemDropIndex = new MenuItem();
        //    itemDropIndex.Header = "Drop Index";
        //    itemDropIndex.Click += delegate { itemDropIndex_Click(); };
        //    ctMenu.Items.Add(itemDropIndex);

        //    return ctMenu;
        //}

        //private ContextMenu MakeContextmenu_ForAllColumn()
        //{

        //    ContextMenu ctMenu = new ContextMenu();

        //    MenuItem itemInsertColumn = new MenuItem();
        //    itemInsertColumn.Header = "Insert Column";
        //    itemInsertColumn.Click += delegate { itemInsertColumn_Click(false); };
        //    ctMenu.Items.Add(itemInsertColumn);

        //    return ctMenu;
        //}

        ///// <summary>
        ///// Current use MakeContextmenu_WhenEmptyContent() ContextMenu
        ///// But in the future, if necessary , refactor it
        ///// </summary>
        ///// <returns></returns>
        //private ContextMenu MakeContextmenu_WhenNoSelection()
        //{
        //    return MakeContextmenu_WhenEmptyContent();
        //}



        ///// <summary>
        ///// When db has no tables 
        ///// </summary>
        //private ContextMenu MakeContextmenu_WhenEmptyContent()
        //{
        //    ContextMenu ctMenu = new ContextMenu();

        //    MenuItem itemCreateTable = new MenuItem();
        //    itemCreateTable.Header = "Create Table";
        //    itemCreateTable.Click += delegate { itemCreateTable_Click(); };
        //    ctMenu.Items.Add(itemCreateTable);

        //    return ctMenu;
        //}


        ///// <summary>
        ///// Content for Column
        ///// </summary>
        //private ContextMenu MakeContextmenu_ForColumn()
        //{

        //    ContextMenu ctMenu = new ContextMenu();

        //    MenuItem itemDropColumn = new MenuItem();
        //    itemDropColumn.Header = "Drop Column";
        //    itemDropColumn.Click += delegate { itemDropColumn_Click(); };
        //    ctMenu.Items.Add(itemDropColumn);

        //    MenuItem itemRenameColumn = new MenuItem();
        //    itemRenameColumn.Header = "Rename Column";
        //    itemRenameColumn.Click += delegate { itemRenameColumn_Click(); };
        //    ctMenu.Items.Add(itemRenameColumn);

        //    MenuItem itemModidyTypeColumn = new MenuItem();
        //    itemModidyTypeColumn.Header = "Modify Column";
        //    itemModidyTypeColumn.Click += delegate { itemModifyTypeColumn_Click(); };
        //    ctMenu.Items.Add(itemModidyTypeColumn);

        //    MenuItem itemInsertColumn = new MenuItem();
        //    itemInsertColumn.Header = "Insert Column";
        //    itemInsertColumn.Click += delegate { itemInsertColumn_Click(true); };
        //    ctMenu.Items.Add(itemInsertColumn);

        //    ctMenu.Items.Add(new Separator());

        //    MenuItem itemCreateIndex = new MenuItem();
        //    itemCreateIndex.Header = "Create Index";
        //    itemCreateIndex.Click += delegate { itemCreateIndex_Click(); };
        //    ctMenu.Items.Add(itemCreateIndex);

        //    ctMenu.Items.Add(new Separator());
        //    MenuItem itemCopyColumnName = new MenuItem();
        //    itemCopyColumnName.Header = "Copy Column Name";
        //    itemCopyColumnName.Click += delegate { itemCopyColumnName_Click(); };
        //    ctMenu.Items.Add(itemCopyColumnName);


        //    return ctMenu;

        //}

        //private void itemCopyColumnName_Click()
        //{
        //    Clipboard.SetText(GetArgsFromSelectedItem().ColumnName);
        //}

        ///// <summary>
        ///// Insert a column
        ///// </summary>
        ///// <param name="IsFromTableNode">Indicate this event is raised from Which kind of TreeItemType</param>
        ///// 
        //private void itemInsertColumn_Click(bool IsFromTableNode)
        //{
        //    string tableName = string.Empty;
        //    if (IsFromTableNode)
        //    {
        //        tableName = GetArgsFromSelectedItem().TableName;
        //    }
        //    else
        //    {
        //        tableName = GetParentItemArgs().TableName;
        //    }

        //    txtMainCmd.Text = String.Format("ALTER TABLE [{0}] ADD COLUMN [ColumnName] [ColumnType]", tableName);

        //    txtMainCmd.Select(txtMainCmd.Text.IndexOf('['), 10);
        //}

        ///// <summary>
        ///// Modify Column type
        ///// </summary>
        //private void itemModifyTypeColumn_Click()
        //{
        //    string tableName = GetTopNodeArgs().TableName;

        //    txtMainCmd.Text = String.Format("ALTER TABLE [{0}]  ALTER COLUMN {1} [NewType] ", tableName, GetArgsFromSelectedItem().ColumnName);
        //}



        ///// <summary>
        ///// Delete Column
        ///// </summary>
        //private void itemDropColumn_Click()
        //{

        //    if (MessageBox.Show("Do you confirm this action", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
        //    {
        //        string tableName = GetTopNodeArgs().TableName;

        //        txtMainCmd.Text = String.Format("ALTER TABLE [{0}]  Drop COLUMN {1} ", tableName, GetArgsFromSelectedItem().ColumnName);

        //        butQueryClick(null, null);

        //        txtMainCmd.Text = string.Empty;
        //    }
        //}


        //private void itemRenameColumn_Click()
        //{
        //    string tableName = GetTopNodeArgs().TableName;

        //    string newColumnName = string.Empty;

        //ReDo:
        //    using (PopupForm p = new PopupForm())
        //    {
        //        p.X_NotifyStr = "Column name";
        //        if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            newColumnName = p.X_Result;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }

        //if (newColumnName.Length > 200)
        //{
        //    MessageBox.Show(String.Format("Sql Ce Column name length must large than {0} ", 200));

        //    goto ReDo;
        //}

        //txtMainCmd.Text = String.Format("ALTER TABLE [{0}]  RENAME COLUMN {1} To {2}", tableName, GetArgsFromSelectedItem().ColumnName, newColumnName);

        //butQueryClick(null, null);

        //txtMainCmd.Text = string.Empty;



        //}


        ///// <summary>
        ///// Content for table based
        ///// </summary>
        //private ContextMenu MakeContextmenu_ForTable()
        //{

        //    ContextMenu ctMenu = new ContextMenu();

        //    MenuItem itemSelectAll = new MenuItem();
        //    itemSelectAll.Header = "Select all";
        //    itemSelectAll.Click += delegate { itemSelectAll_Click(); };
        //    ctMenu.Items.Add(itemSelectAll);

        //    ctMenu.Items.Add(new Separator());

        //    MenuItem itemSelect = new MenuItem();
        //    itemSelect.Header = "Select ...";
        //    itemSelect.Click += delegate { itemSelect_Click(); };
        //    ctMenu.Items.Add(itemSelect);

        //    MenuItem itemInsert = new MenuItem();
        //    itemInsert.Header = "Insert ...";

        //    itemInsert.Click += delegate {

        //        TreeItemArgs args = GetArgsFromSelectedItem();
        //        Debug.Assert(args.ItemType == TreeItemType.Table);
        //        itemInsert_Click(args.TableName);
        //    };
        //    ctMenu.Items.Add(itemInsert);

        //    MenuItem itemUpdate= new MenuItem();
        //    itemUpdate.Header = "Update ...";
        //    itemUpdate.Click += delegate {
        //        TreeItemArgs args = GetArgsFromSelectedItem();
        //        Debug.Assert(args.ItemType == TreeItemType.Table);
        //        List<string> columnlist = App.MainEngineer.GetColumnNameListFromTable(args.TableName);
        //        itemUpdate_Click(columnlist);
        //    };
        //    ctMenu.Items.Add(itemUpdate);

        //    MenuItem itemDelete = new MenuItem();
        //    itemDelete.Header = "Delete ...";
        //    itemDelete.Click += delegate { itemDelete_Click(); };
        //    ctMenu.Items.Add(itemDelete);


        //    ctMenu.Items.Add(new Separator());

        //    MenuItem itemInsertDataRow = new MenuItem();
        //    itemInsertDataRow.Header = "Insert a row";
        //    itemInsertDataRow.Click += delegate { itemInsertDataRow_Click(); };
        //    ctMenu.Items.Add(itemInsertDataRow);

        //    ctMenu.Items.Add(new Separator());

        //    MenuItem itemCreateTable = new MenuItem();
        //    itemCreateTable.Header = "Create table";
        //    itemCreateTable.Click += delegate { itemCreateTable_Click(); };
        //    ctMenu.Items.Add(itemCreateTable);

        //    MenuItem itemRenameTable = new MenuItem();
        //    itemRenameTable.Header = "Rename table...";
        //    itemRenameTable.Click += delegate { itemRenameTable_Click(); };
        //    ctMenu.Items.Add(itemRenameTable);

        //    MenuItem itemDeleteTable = new MenuItem();
        //    itemDeleteTable.Header = "Drop table";
        //    itemDeleteTable.Click += delegate { itemDeleteTable_Click(); };
        //    ctMenu.Items.Add(itemDeleteTable);

        //    MenuItem itemModifySchema = new MenuItem();
        //    itemModifySchema.Header = "Modify Table Schema";
        //    itemModifySchema.Click += delegate { itemModifySchema_Click(); };
        //    ctMenu.Items.Add(itemModifySchema);

        //    ctMenu.Items.Add(new Separator());

        //    MenuItem itemGetColumnsInfo = new MenuItem();
        //    itemGetColumnsInfo.Header = "Get All columns info";
        //    itemGetColumnsInfo.Click += delegate { itemGetColumnsInfo_Click(); };
        //    ctMenu.Items.Add(itemGetColumnsInfo);

        //    MenuItem itemGetIndexesInfo = new MenuItem();
        //    itemGetIndexesInfo.Header = "Get All Indexes info";
        //    itemGetIndexesInfo.Click += delegate { itemGetIndexesInfo_Click(); };
        //    ctMenu.Items.Add(itemGetIndexesInfo);

        //    ctMenu.Items.Add(new Separator());

        //    MenuItem itemProviderInfo = new MenuItem();
        //    itemProviderInfo.Header = "Get supported type";
        //    itemProviderInfo.Click += delegate { itemProviderInfo_Click(); };
        //    ctMenu.Items.Add(itemProviderInfo);

        //    return ctMenu;
        //}

        //private void itemSelect_Click()
        //{
        //    txtMainCmd.Text = String.Format("SELECT * FROM [{0}]", GetArgsFromSelectedItem().TableName);

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="columnNameList"></param>
        //private void itemInsert_Click(string tableName)
        //{
        //    DataSet ds = App.MainEngineer.GetColumnInfoFromTable(tableName);
        //    Dictionary<string, string> columnInfo = new Dictionary<string, string>();

        //    foreach (DataRow dt in ds.Tables[0].Rows)
        //    {
        //        columnInfo[dt["COLUMN_NAME"].ToString()] = dt["DATA_TYPE"].ToString();
        //    }

        //    string columnP = string.Empty;
        //    string columnValue = string.Empty;

        //    foreach (KeyValuePair<string,string> item in columnInfo)
        //    {
        //        columnP += string.Format("{0},", item.Key);

        //        columnValue += string.Format("{0},", GlobalDefine.MyGlobal.ConvertToTargetValue(item.Value));
        //    }
                
            
        //    columnP = columnP.Substring(0, columnP.Length - 1);
        //    columnValue = columnValue.Substring(0, columnValue.Length - 1);

        //    txtMainCmd.Text = String.Format("INSERT INTO [{0}] ({1}) VALUES ({2})", GetArgsFromSelectedItem().TableName, columnP,columnValue);

        //    txtMainCmd.Focus();
        //    txtMainCmd.Select(txtMainCmd.Text.Length - 14, 14);
            
        //}



        //private void itemUpdate_Click(List<string> columnNameList)
        //{
        //    string columnP = string.Empty;
        //    foreach (string item in columnNameList)
        //    {
        //        columnP += string.Format("{0}={1},",item, "COLUMN_VALUE");
        //    }

        //    columnP = columnP.Substring(0, columnP.Length - 1);

        //    columnP += " where WHERE_CALUSE";
        //    txtMainCmd.Text = String.Format("UPDATE [{0}] SET {1}", GetArgsFromSelectedItem().TableName,columnP);

        //}

        //private void itemDelete_Click()
        //{
        //    txtMainCmd.Text = String.Format("Delete from [{0}] WHERE (COLUMN_NAME) = (COLUMN_VALUE) ", GetArgsFromSelectedItem().TableName);
        //}

        //private void itemProviderInfo_Click()
        //{
        //    X_ShowSqlExecutionResult(App.MainEngineer.GetProviderInfoFromTable(GetArgsFromSelectedItem().TableName));
        //}

        //private void itemGetIndexesInfo_Click()
        //{
        //     X_ShowSqlExecutionResult(App.MainEngineer.GetIndexInfoFromTable(GetArgsFromSelectedItem().TableName));
        //}

        //private void itemGetColumnsInfo_Click()
        //{
        //    X_ShowSqlExecutionResult(App.MainEngineer.GetColumnInfoFromTable(GetArgsFromSelectedItem().TableName));
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //private void itemModifySchema_Click()
        //{
        //    CreateTableDetailWindow ct = new CreateTableDetailWindow(CreateTableDetailWindow.EnteringType.ModifySchema,GetArgsFromSelectedItem().TableName);

        //    ct.Owner = this;    
            
        //    ct.ShowDialog();
        //}

        //private void itemDropIndex_Click()
        //{

        //    if (MessageBox.Show("Do you confirm this action", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
        //    {
        //        txtMainCmd.Text = String.Format("DROP INDEX [{0}].[{1}]",
        //            GetTopNodeArgs().TableName, GetTopNodeArgs().IndexName);

        //        butQueryClick(null, null);

        //        txtMainCmd.Text = string.Empty;
        //    }

        //}

        //private void itemCreateIndex_Click()
        //{
        //    string newIndexName = string.Empty;
        //ReDo:
        //    using (PopupForm p = new PopupForm())
        //    {
        //        p.X_NotifyStr = "Index name";
        //        if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            newIndexName = p.X_Result;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }

        //if (newIndexName.Length > 20)
        //{
        //    MessageBox.Show(String.Format("Index name length must large than {0} ", 20));

        //    goto ReDo;
        //}

        //txtMainCmd.Text = String.Format("CREATE UNIQUE INDEX {0} ON [{1}] ({2}) ", newIndexName, GetParentItemArgs().TableName, GetArgsFromSelectedItem().ColumnName);
        //butQueryClick(null, null);

        //txtMainCmd.Text = string.Empty;


        //}

        ///// <summary>
        ///// Insert a row data in current table
        ///// </summary>
        //private void itemInsertDataRow_Click()
        //{

        //    string tableName = GetArgsFromSelectedItem().TableName;
        //    string args = string.Empty;

        //    DataSet dataRowCollections = App.MainEngineer.GetColumnInfoFromTable(tableName);
        //    args += "( ";
        //    foreach (DataRow item in dataRowCollections.Tables[0].Rows)
        //    {
        //        args += item["COLUMN_NAME"] + ",";
        //    }
        //    args = args.Substring(0, args.Length - 1);

        //    args += ") VALUES (";

        //    for (int i = 0; i < dataRowCollections.Tables[0].Rows.Count; i++)
        //    {
        //        switch (dataRowCollections.Tables[0].Rows[i]["DATA_TYPE"].ToString())
        //        {
        //            case "int":
        //                args += String.Format(" {0},", i.ToString());
        //                break;

        //            default:
        //                args += String.Format(" '{0}',", i.ToString());
        //                break;
        //        }


        //    }

        //    args = args.Substring(0, args.Length - 1);

        //    args += " )";

        //    txtMainCmd.Text = String.Format("INSERT INTO [{0}] {1}", tableName, args);
        //}

        ///// <summary>
        ///// Delete Table
        ///// </summary>
        //private void itemDeleteTable_Click()
        //{

        //    if (MessageBox.Show("Do you confirm this action", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
        //    {
        //        txtMainCmd.Text = String.Format("DROP TABLE [{0}] ", GetArgsFromSelectedItem().TableName);

        //        butQueryClick(null, null);

        //        txtMainCmd.Text = string.Empty;
        //    }
        //}

        ///// <summary>
        ///// Rename table
        ///// </summary>
        //void itemRenameTable_Click()
        //{
        //    string newTableName = string.Empty;

        //ReDo:
        //    using (PopupForm p = new PopupForm())
        //    {
        //        p.X_NotifyStr = "Table name";
        //        if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            newTableName = p.X_Result;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }

        //if (newTableName.Length > App.MainEngineer.GetTableMaxLength)
        //{
        //    MessageBox.Show(String.Format("Sql Ce table name length must large than {0} ", App.MainEngineer.GetTableMaxLength));

        //    goto ReDo;
        //}

        ////txtMainCmd.Text = string.Format("ALTER TABLE [{0}] RENAME TO [{1}] ", GetArgsFromSelectedItem().TableName, newTableName);

        // //In sqlce3.5 this method is not saied in anywhere ,but this docs
        //txtMainCmd.Text = string.Format("sp_rename '{0}' '{1}'", GetArgsFromSelectedItem().TableName, newTableName);
        //butQueryClick(null, null);

        //txtMainCmd.Text = string.Empty;

        //}

        ///// <summary>
        ///// Select Cmd
        ///// </summary>
        //void itemSelectAll_Click()
        //{
        //    txtMainCmd.Text = string.Format("Select * from [{0}]",GetArgsFromSelectedItem().TableName);

        //    butQueryClick(null, null);

        //    txtMainCmd.Text = string.Empty;

        //}

        ///// <summary>
        ///// Create table sql command
        ///// </summary>
        //private void itemCreateTable_Click()
        //{
        //    SwitchToCreateTable st = new SwitchToCreateTable();
        //    st.Owner = this;
        //    st.ShowDialog();
        //    switch (st.CurCreateType)
        //    {
        //        case SwitchToCreateTable.CreateType.NotSelected:
        //            return;
        //        case SwitchToCreateTable.CreateType.Simple:
        //            goto ReDo;
        //        case SwitchToCreateTable.CreateType.Normal:
        //            CreateTableDetailWindow ct = new CreateTableDetailWindow(CreateTableDetailWindow.EnteringType.CreateTable);
        //            ct.Owner = this;
        //            ct.RaiseAddNewTable += delegate { RefreshCurrentDbStatus(); };
        //            ct.ShowDialog();
        //            return;
        //        default:
        //            return;
        //    }

            

        //ReDo:
        //    string tableName = string.Empty;
        //    using (PopupForm p = new PopupForm())
        //    {
        //        p.X_NotifyStr = "Table name";
        //        if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            tableName = p.X_Result;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }

        //if (tableName.Length > App.MainEngineer.GetTableMaxLength)
        //{
        //    MessageBox.Show(String.Format("Table name length must large than {0} ", App.MainEngineer.GetTableMaxLength));

        //    goto ReDo;
        //}

        //txtMainCmd.Text = String.Format("Create table [{0}] (ID int , Comment nvarchar(255))", tableName);



        //}

    }
}
