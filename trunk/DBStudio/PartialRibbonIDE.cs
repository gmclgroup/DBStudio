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
using DBStudio.AdditionUI;
using System.Collections.ObjectModel;
using DBStudio.BaseUI;
using XLCS.Common;
using CustomControl.NewXLAGControl;
using System.Data.SqlServerCe;
using wf = System.Windows.Forms;
using System.IO;
using DBStudio.SqlCE.UI;
using DBStudio.SqlCE.AdditionUI;
using System.ServiceProcess;
using ETL;
using DBStudio.GlobalDefine;
using Microsoft.Windows.Controls;
using DBStudio.UI;
using System.Threading;
using System.Linq;
using System.Data.Linq;
using DBStudio.CommandFactory;
using System.Windows.Threading;
using DBStudio.ContextMenuFactory;
using DBStudio.SqlServer.DataExchange;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using DBStudio.CommonUI;
using DBStudio.SqlCE.Tools;
using DBStudio.SqlCE.Sync;
using Microsoft.Windows.Controls.Ribbon;
using CoreEA.LoginInfo;
using DBStudio.Utility;
using CoreEA.SchemaInfo;
using DBStudio.Bases;
using System.Windows.Media.Animation;
using DBStudio.CreateTableSchemaFactory;
using MPL.MyControls;

namespace DBStudio
{

    /// <summary>
    /// For Create Sql Command Ribbion Tab 
    /// </summary>
    public partial class RibbionIDE
    {
        #region Table
        public static RibbonCommand CreateTableCmd = new RibbonCommand();
        public static RibbonCommand RenameTableCmd = new RibbonCommand();
        public static RibbonCommand DropTableCmd = new RibbonCommand();
        public static RibbonCommand ModifyTableCmd = new RibbonCommand();

        public static RibbonCommand ShowDataCmd = new RibbonCommand();
        public static RibbonCommand SelctColumnCmd = new RibbonCommand();
        public static RibbonCommand AddColumnCmd = new RibbonCommand();
        public static RibbonCommand CopyTableNameCmd = new RibbonCommand();

        public static RibbonCommand GetAllColumnSchemaInfoCmd = new RibbonCommand();
        public static RibbonCommand GetAllIndexSchemaInfoCmd = new RibbonCommand();
        public static RibbonCommand ShowSupportedDbTypeCmd = new RibbonCommand();
        public static RibbonCommand ResetIdentityColumnCmd = new RibbonCommand();
        #endregion

        #region Columns Root
        public static RibbonCommand RefreshColumnCmd = new RibbonCommand();
        #endregion

        #region Columns
        public static RibbonCommand RenameColumnCmd = new RibbonCommand();
        public static RibbonCommand ModifyColumnCmd = new RibbonCommand();
        public static RibbonCommand DropColumnCmd = new RibbonCommand();
        public static RibbonCommand CreateIndexCmd = new RibbonCommand();
        public static RibbonCommand CopyColumnNameCmd = new RibbonCommand();
        #endregion

        #region Index Root
        public static RibbonCommand RefreshIndexCmd = new RibbonCommand();
        #endregion

        #region Indexes
        public static RibbonCommand DropIndexCmd = new RibbonCommand();
        #endregion

        #region System Views
        public static RibbonCommand RefreshSystemViewCmd = new RibbonCommand();
        public static RibbonCommand ShowSystemViewDataCmd = new RibbonCommand();
        #endregion



        private void InitRoutedUICommand()
        {
            #region Table
            CreateTableCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\Table_new.png", UriKind.Relative));
            CreateTableCmd.LabelTitle = "CreatTableDialogTitle".GetFromResourece();

            RenameTableCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_name_pencil_magenta.png", UriKind.Relative));
            RenameTableCmd.LabelTitle = "TitleRenameTable".GetFromResourece();

            DropTableCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_delete.png", UriKind.Relative));
            DropTableCmd.LabelTitle = "TitleDropTable".GetFromResourece();

            ModifyTableCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_info.png", UriKind.Relative));
            ModifyTableCmd.LabelTitle = "TitleModifyTableSchema".GetFromResourece();

            CopyTableNameCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_name_copy.png", UriKind.Relative));
            CopyTableNameCmd.LabelTitle = "TitleCopyTableName".GetFromResourece();

            ShowDataCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\select_all.png", UriKind.Relative));
            ShowDataCmd.LabelTitle = "TitleSelectAll".GetFromResourece();

            SelctColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_pencil_magenta.png", UriKind.Relative));
            SelctColumnCmd.LabelTitle = "TitleSelectWithUI".GetFromResourece();

            AddColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_column_new.png", UriKind.Relative));
            AddColumnCmd.LabelTitle = "TitleAddColumn".GetFromResourece();

            GetAllColumnSchemaInfoCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_column_reload.png", UriKind.Relative));
            GetAllColumnSchemaInfoCmd.LabelTitle = "TitleGetColumnInfo".GetFromResourece();

            GetAllIndexSchemaInfoCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_index_info.png", UriKind.Relative));
            GetAllIndexSchemaInfoCmd.LabelTitle = "TitleGetIndexesInfo".GetFromResourece();

            ShowSupportedDbTypeCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\table_info.png", UriKind.Relative));
            ShowSupportedDbTypeCmd.LabelTitle = "TitleGetSupportedType".GetFromResourece();

            ResetIdentityColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\RibbonIcons\key_primary_reset.png", UriKind.Relative));
            ResetIdentityColumnCmd.LabelTitle = "TitleResetIdentityColumn".GetFromResourece();


            this.CommandBindings.Add(new CommandBinding(ResetIdentityColumnCmd, ResetIdentityColumnCmd_Execute, ResetIdentityColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CreateTableCmd, CreateTableCmd_Execute, CreateTableCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(RenameTableCmd, RenameTableCmd_Execute, RenameTableCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ShowDataCmd, ShowDataCmd_Execute, ShowDataCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(SelctColumnCmd, SelctColumnCmd_Execute, SelctColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(AddColumnCmd, AddColumnCmd_Execute, AddColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DropTableCmd, DropTableCmd_Execute, DropTableCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ModifyTableCmd, ModifyTableCmd_Execute, ModifyTableCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(GetAllColumnSchemaInfoCmd, GetAllColumnSchemaInfoCmd_Execute, GetAllColumnSchemaInfoCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(GetAllIndexSchemaInfoCmd, GetAllIndexSchemaInfoCmd_Execute, GetAllIndexSchemaInfoCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CopyTableNameCmd, CopyTableNameCmd_Execute, CopyTableNameCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ShowSupportedDbTypeCmd, ShowSupportedDbTypeCmd_Execute, ShowSupportedDbTypeCmd_CanExecute));
            #endregion

            #region Colums
            RefreshColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            RefreshColumnCmd.LabelTitle = "TitleRefreshColumn".GetFromResourece();

            RenameColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            RenameColumnCmd.LabelTitle = "TitleRenameColumn".GetFromResourece();
            DropColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            DropColumnCmd.LabelTitle = "TitleDropColumn".GetFromResourece();
            ModifyColumnCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            ModifyColumnCmd.LabelTitle = "TitleModifyColumn".GetFromResourece();
            CopyColumnNameCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            CopyColumnNameCmd.LabelTitle = "TitleCopyColumnName".GetFromResourece();
            CreateIndexCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            CreateIndexCmd.LabelTitle = "TitleCreateIndex".GetFromResourece();

            this.CommandBindings.Add(new CommandBinding(RefreshColumnCmd, RefreshColumnCmd_Execute, RefreshColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(RenameColumnCmd, RenameColumnCmd_Execute, RenameColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DropColumnCmd, DropColumnCmd_Execute, DropColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ModifyColumnCmd, ModifyColumnCmd_Execute, ModifyColumnCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CopyColumnNameCmd, CopyColumnNameCmd_Execute, CopyColumnNameCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CreateIndexCmd, CreateIndexCmd_Execute, CreateIndexCmd_CanExecute));
            #endregion

            #region Indexs
            RefreshIndexCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            RefreshIndexCmd.LabelTitle = "TitleRefreshIndex".GetFromResourece();

            DropIndexCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            DropIndexCmd.LabelTitle = "TitleDropIndex".GetFromResourece();

            this.CommandBindings.Add(new CommandBinding(RefreshIndexCmd, RefreshIndexCmd_Execute, RefreshIndexCmd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(RefreshIndexCmd, DropIndexCmd_Execute, DropIndexCmd_CanExecute));
            #endregion

            #region SystemViews
            RefreshSystemViewCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            RefreshSystemViewCmd.LabelTitle = "TitleRefreshSystemView".GetFromResourece();

            this.CommandBindings.Add(new CommandBinding(RefreshSystemViewCmd, RefreshSystemViewCmd_Execute, RefreshSystemViewCmd_CanExecute));

            ShowSystemViewDataCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            ShowSystemViewDataCmd.LabelTitle = "TitleShowSystemViewData".GetFromResourece();

            this.CommandBindings.Add(new CommandBinding(ShowSystemViewDataCmd, ShowSystemViewDataCmd_Execute, ShowSystemViewDataCmd_CanExecute));
            #endregion

            //For Generate Sql Script
            //this method in another file
            InitSqlScriptCmd();
        }

        #region Tables
        private void ShowSupportedDbTypeCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DataTable dt = App.MainEngineer.GetProviderInfoFromTable(CurrentTreeArgs.TableName);
                SetResultInUI(dt);
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            //CurrentDataGrid.DataContext = App.MainEngineer.GetProviderInfoFromTable(CurrentTreeArgs.TableName);
        }

        private void ShowSupportedDbTypeCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void DropTableCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if ("ConfirmTextDropTable".GetFromResourece().Confirm())
            {
                string cmdStr = string.Empty;
                if ("ConfirmTextDropTableWithCascade".GetFromResourece().Confirm())
                {
                    cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropTableCmdStrWithCascade(CurrentTreeArgs.TableName);
                }
                else
                {
                    cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropTableCmdStr(CurrentTreeArgs.TableName);
                }
                App.MainEngineer.DoExecuteNonQuery(cmdStr);

                LoadAllSchemaInfo();
            }
        }

        private void DropTableCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void ModifyTableCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
        }

        private void ModifyTableCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void GetAllColumnSchemaInfoCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            //CurrentDataGrid.DataContext = App.MainEngineer.GetColumnInfoFromTable(CurrentTreeArgs.TableName);
            try
            {
                DataTable dt = App.MainEngineer.GetColumnInfoFromTable(CurrentTreeArgs.TableName);
                SetResultInUI(dt);
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
           }

        private void GetAllColumnSchemaInfoCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void GetAllIndexSchemaInfoCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            //CurrentDataGrid.DataContext = App.MainEngineer.GetIndexInfoFromTable(CurrentTreeArgs.TableName);
            try{
            DataTable dt = App.MainEngineer.GetIndexInfoFromTable(CurrentTreeArgs.TableName);
            SetResultInUI(dt);
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
        }

        private void GetAllIndexSchemaInfoCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void CopyTableNameCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(CurrentTreeArgs.TableName);
        }

        private void CopyTableNameCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }


        private void RenameTableCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            string oldTableName = CurrentTreeArgs.TableName;
            InputValueWindows iv = new InputValueWindows("Table Name");

            if (iv.ShowDialog() == true)
            {
                string newTableName = iv.X_GetInputedValue.ToString();
                if (newTableName == oldTableName)
                {
                    return;
                }

                string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetRenameTableCmdStr(oldTableName, newTableName);

                App.MainEngineer.DoExecuteNonQuery(cmdStr);

                //here if we load all schema info that will cost a lot of time to do 
                //so here ,just find the node of this table and change the treeNode Text value
                //LoadAllSchemaInfo();
                foreach (TreeViewItem item in mainTreeView.Items)
                {
                    if (item.Header.ToString() == oldTableName)
                    {
                        item.Header = newTableName;
                        CurrentTreeArgs.TableName = newTableName;
                    }
                }
            }

        }

        private void RenameTableCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void SelctColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
        }

        private void SelctColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void ShowDataCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try{
            DataTable dt = App.MainEngineer.GetAllDataFromTable(CurrentTreeArgs.TableName);
            SetResultInUI(dt);
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
        }

        private void SetResultInUI(DataTable dt)
        {
            try
            {
#if DEBUG
            dt.TableName = "xcserfdscv34rd";
            dt.WriteXml("Endresult.xml");
#else
#endif
            CurrentPagedDataGrid.MyDataSource = dt;
            
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
        }

        private void SetResultInUI(object dt)
        {
            CurrentPagedDataGrid.MyDataSource = dt;
        }

        private void ShowDataCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        /// <summary>
        /// SqlCE and other db are different.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetIdentityColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if ("Do you confirm you want to reset the identity column ?".Confirm())
            {
                try
                {
                    BaseTableSchema schema = App.MainEngineer.GetTableSchemaInfoObject(CurrentTreeArgs.TableName);
                    var result = schema.Columns.Where(c => c.IsIdentity == true);

                    if (result.Count() > 0)
                    {
                        BaseColumnSchema columnSchema = result.First();

                        string cmd = App.MainEngineer.CurrentCommandTextHandler.GetResetIdentityColumn(CurrentTreeArgs.TableName, columnSchema.ColumnName);
                        App.MainEngineer.DoExecuteNonQuery(cmd);
                        //
                        RefreshColumnsInfo(CurrentTreeArgs.TableName);

                    }
                    else
                    {
                        "NoDataToProcess".GetFromResourece().Show();
                    }
                }
                catch (Exception ee)
                {
                    ee.HandleMyException();
                }
            }
        }

        private void ResetIdentityColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
                    || (CurrentTreeArgs.TableName.IsEmpty())
                )
            {


                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }


        private void CreateTableCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CurrentMenuBase.GetCreateTableSchemaWindow().ShowDialog();
            }
            catch (Exception ee)
            {

                ee.HandleMyException();
            }
        }

        private void CreateTableCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            AddColumn addColumn = new AddColumn(CurrentTreeArgs);

            if ((bool)addColumn.ShowDialog())
            {
                RefreshColumnsInfo(CurrentTreeArgs.TableName);
            }
        }

        private void AddColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
                || (CurrentTreeArgs.TableName.IsEmpty())
            )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        #endregion

        #region Columns
        private void RenameColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            string newColumnName = string.Empty;
            InputValueWindows iv = new InputValueWindows("New column name");
            if (iv.ShowDialog() == true)
            {

                string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetRenameColumnCmdStr(CurrentTreeArgs.TableName,
                    CurrentTreeArgs.ColumnName, newColumnName);
                App.MainEngineer.DoExecuteNonQuery(cmdStr);

                RefreshColumnsInfo(CurrentTreeArgs.TableName);
            }
        }

        private void RenameColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty()) || (CurrentTreeArgs.ColumnName.IsEmpty())
            )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void DropColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if ("ConfirmTextDropColumn".GetFromResourece().Confirm())
            {
                string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropColumnCmdStr(CurrentTreeArgs.TableName, CurrentTreeArgs.ColumnName);

                App.MainEngineer.DoExecuteNonQuery(cmdStr);

                RefreshColumnsInfo(CurrentTreeArgs.TableName);
            }
        }

        private void DropColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty()) || (CurrentTreeArgs.ColumnName.IsEmpty())
            )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void ModifyColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ModifyColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty()) || (CurrentTreeArgs.ColumnName.IsEmpty())
            )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void CreateIndexCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            InputValueWindows iv = new InputValueWindows("New index name");
            iv.X_MaxLength = 20;
            if (iv.ShowDialog() == true)
            {
                string newIndexName = iv.X_GetInputedValue.ToString();

                if ((newIndexName.Length > 20) || (string.IsNullOrEmpty(newIndexName)))
                {
                    MessageBox.Show(String.Format("Index name length must less than {0} and large than 0 ", 20));
                    return;
                }

                string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetCreateIndexCmdStr(newIndexName, CurrentTreeArgs.TableName,
                    CurrentTreeArgs.ColumnName);
                App.MainEngineer.DoExecuteNonQuery(cmdStr);
                RefreshColumnsInfo(CurrentTreeArgs.TableName);

            }
        }

        private void CreateIndexCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty()) || (CurrentTreeArgs.ColumnName.IsEmpty())
            )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void CopyColumnNameCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(CurrentTreeArgs.ColumnName);
        }

        private void CopyColumnNameCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty()) || (CurrentTreeArgs.ColumnName.IsEmpty())
            )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void RefreshColumnCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshColumnsInfo(CurrentTreeArgs.TableName);
        }

        private void RefreshColumnCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        #endregion

        #region Indexes

        private void DropIndexCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if ("ConfirmTextDropIndex".GetFromResourece().Confirm())
            {
                string cmdStr = App.MainEngineer.CurrentCommandTextHandler.GetDropIndexCmdStr(CurrentTreeArgs.TableName, CurrentTreeArgs.IndexName);

                App.MainEngineer.DoExecuteNonQuery(cmdStr);
                RefreshColumnsInfo(CurrentTreeArgs.TableName);
            }
        }

        private void DropIndexCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty()) || (CurrentTreeArgs.IndexName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void RefreshIndexCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshColumnsInfo(CurrentTreeArgs.TableName);

        }

        private void RefreshIndexCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
     || (CurrentTreeArgs.TableName.IsEmpty())
 )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        #endregion

        #region SystemViews
        private void ShowSystemViewDataCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            string cmdStr = "select * from " + CurrentTreeArgs.SystemViewName;
            try
            {
                //CurrentDataGrid.DataContext = App.MainEngineer.ExecuteDataList(cmdStr).Tables[0];
                DataTable dt = App.MainEngineer.ExecuteDataList(cmdStr).Tables[0];
                SetResultInUI(dt);
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
        }

        private void ShowSystemViewDataCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentTreeArgs != null)
            {
                if (CurrentTreeArgs.ItemType == TreeItemType.SystemView_SchemaNode)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void RefreshSystemViewCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshSystemViewInfo();
        }

        private void RefreshSystemViewCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentTreeArgs != null)
            {
                if ((CurrentTreeArgs.ItemType == TreeItemType.SystemViewNode)
                    || (CurrentTreeArgs.ItemType == TreeItemType.SystemView_Schema_ColumnNode)
                    || (CurrentTreeArgs.ItemType == TreeItemType.SystemView_SchemaNode)
                    )
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            else
            {
                e.CanExecute = false;
            }
        }
        #endregion

        #region Menu Area

        private void InitRibbonMenu(Ribbon ribbionMenu)
        {
            ribbionMenu.Visibility = Visibility.Visible;

            //Add some sql script collection function tabs
            PopulateSqlCollectionTabs(ribbionMenu);
            
            //Add the sql script generation tab
            RibbonTab tabForGenerateScript = GetGenerateSqlScriptCommandTab();
            ribbionMenu.Tabs.Add(tabForGenerateScript);

            //set current selected tab
            //in most case we set to tab
            //but in effiproz we should select first tab,because there is a submit change button
            switch (App.MainEngineer.HostedType)
            {
                case CoreEA.CoreE.UsedDatabaseType.Effiproz:
                    ribbionMenu.TabIndex = 0;
                    break;
                //default:
                //    ribbionMenu.SelectedTab = tab;
                //    break;
                default:
                    ribbionMenu.TabIndex = 0;
                    break;
            }
        }



        #endregion


        /// <summary>
        /// Populate the sql script collection tabls
        /// Table 
        /// Column 
        /// Index
        /// Stored Procedure
        /// </summary>
        /// <returns></returns>
        private void PopulateSqlCollectionTabs(Ribbon ribbionMenu)
        {
            RibbonTab tableTab = new RibbonTab();
            RibbonTab columnTab = new RibbonTab();
            RibbonTab indexTab = new RibbonTab();
            RibbonTab sysViewTab = new RibbonTab();
            ribbionMenu.Tabs.Add(tableTab);
            ribbionMenu.Tabs.Add(columnTab);
            ribbionMenu.Tabs.Add(indexTab);
            ribbionMenu.Tabs.Add(sysViewTab);

            tableTab.Label = "TitleTableTab".GetFromResourece();
            columnTab.Label = "TitleColumnTab".GetFromResourece();
            indexTab.Label = "TitleIndexTab".GetFromResourece();
            sysViewTab.Label = "TitleSpTab".GetFromResourece();
            try
            {

                RibbonGroup groupCommon = new RibbonGroup();
                tableTab.Groups.Add(groupCommon);

                RibbonGroup groupTableLevel = new RibbonGroup();
                tableTab.Groups.Add(groupTableLevel);

                RibbonGroup groupColumnLevel = new RibbonGroup();
                columnTab.Groups.Add(groupColumnLevel);

                RibbonGroup groupIndexLevel = new RibbonGroup();
                indexTab.Groups.Add(groupIndexLevel);

                RibbonGroup groupSystemViewsLevel = new RibbonGroup();
                sysViewTab.Groups.Add(groupSystemViewsLevel);

                #region Table_Level
                #region ExecuteSqlCmd

                RibbonButton butExecuteSqlCmd = new RibbonButton();
                butExecuteSqlCmd.Command = ExeMySqlCommand;
                groupCommon.Controls.Add(butExecuteSqlCmd);
                #endregion

                #region ShowDataCmd
                RibbonButton showDataBut = new RibbonButton();
                showDataBut.Command = ShowDataCmd;
                groupTableLevel.Controls.Add(showDataBut);
                #endregion

                #region Rename Table
                RibbonButton butRenameTable = new RibbonButton();
                butRenameTable.Command = RenameTableCmd;
                groupTableLevel.Controls.Add(butRenameTable);
                #endregion

                #region Select Column With UI
                RibbonButton butSelectWithUI = new RibbonButton();
                butSelectWithUI.Command = SelctColumnCmd;
                groupTableLevel.Controls.Add(butSelectWithUI);
                #endregion

                #region CreateTable
                RibbonButton butCreateTable = new RibbonButton();
                butCreateTable.Command = CreateTableCmd;
                groupTableLevel.Controls.Add(butCreateTable);
                #endregion

                #region Add Column
                RibbonButton butAddColumn = new RibbonButton();
                butAddColumn.Command = AddColumnCmd;
                groupTableLevel.Controls.Add(butAddColumn);

                #endregion

                RibbonButton butDropTable = new RibbonButton();
                butDropTable.Command = DropTableCmd;
                groupTableLevel.Controls.Add(butDropTable);

                RibbonButton butModifyTable = new RibbonButton();
                butModifyTable.Command = ModifyTableCmd;
                groupTableLevel.Controls.Add(butModifyTable);

                RibbonButton butCopyTableName = new RibbonButton();
                butCopyTableName.Command = CopyTableNameCmd;
                groupTableLevel.Controls.Add(butCopyTableName);

                RibbonButton butGetColumnInfo = new RibbonButton();
                butGetColumnInfo.Command = GetAllColumnSchemaInfoCmd;
                groupTableLevel.Controls.Add(butGetColumnInfo);

                RibbonButton butGetIndexInfo = new RibbonButton();
                butGetIndexInfo.Command = GetAllIndexSchemaInfoCmd;
                groupTableLevel.Controls.Add(butGetIndexInfo);

                //Here , we confirm the Effiproz database type didn't support getproviderinfo method . 
                //
                switch (App.MainEngineer.HostedType)
                {
                    case CoreEA.CoreE.UsedDatabaseType.Effiproz:
                        break;
                    default:
                        RibbonButton butGetSupportedDbType = new RibbonButton();
                        butGetSupportedDbType.Command = ShowSupportedDbTypeCmd;
                        groupTableLevel.Controls.Add(butGetSupportedDbType);
                        break;
                }


                RibbonButton butResetIdentityColumn = new RibbonButton();
                butResetIdentityColumn.Command = ResetIdentityColumnCmd;
                groupTableLevel.Controls.Add(butResetIdentityColumn);
                #endregion

                #region Column_Level
                RibbonButton butRefreshColumns = new RibbonButton();
                butRefreshColumns.Command = RefreshColumnCmd;
                groupColumnLevel.Controls.Add(butRefreshColumns);
                #endregion

                #region Index_Level
                RibbonButton butRefreshIndexes = new RibbonButton();
                butRefreshIndexes.Command = RefreshIndexCmd;
                groupIndexLevel.Controls.Add(butRefreshIndexes);
                #endregion

                #region SystemViews
                RibbonButton butRefreshSystemView = new RibbonButton();
                butRefreshSystemView.Command = RefreshSystemViewCmd;
                groupSystemViewsLevel.Controls.Add(butRefreshSystemView);

                RibbonButton butShowSystemViewData = new RibbonButton();
                butShowSystemViewData.Command = ShowSystemViewDataCmd;
                groupSystemViewsLevel.Controls.Add(butShowSystemViewData);
                #endregion
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }

        }

        private MenuBase CurrentMenuBase
        {
            get
            {
                return ((MenuBase)GloablMenuFactory.GetMenu(CurrentTreeArgs).Tag);
            }
        }
    }
}
