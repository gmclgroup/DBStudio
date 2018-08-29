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
using DBStudio.GlobalDefine;
using System.Diagnostics;
using Microsoft.Win32;
using CoreEA.LoginInfo;
using System.Data.SqlServerCe;
using DBStudio.CommonMethod;
using ETL;
using CoreEA.SchemaInfo;
using WPFCommonControl;
using System.Data;
using DBStudio.CommonUI;
using CustomControl.DynaProgressBar;
using System.IO;
using System.Globalization;
using CoreEA;
using System.Data.SqlClient;
using System.Data.Common;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for DataExchangeWizard.xaml
    /// </summary>
    public partial class DataExchangeWizard : BaseUI.BaseFadeDialog
    {
        internal CoreE.UsedDatabaseType CurSrcDbType { get; set; }
        internal CoreE.UsedDatabaseType CurTargetDbType { get; set; }

        internal CoreEA.ICoreEAHander srcHandler { get; set; }
        internal CoreEA.ICoreEAHander targetHandler { get; set; }

        private string sufix = "GO" + System.Environment.NewLine;

        public DataExchangeWizard()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(DataExchangeWizard_Loaded);

            Unloaded += (sender, e) =>
                {
                    if (srcHandler != null)
                    {
                        srcHandler.Close();
                        srcHandler.Dispose();
                    }
                    if (targetHandler != null)
                    {
                        targetHandler.Close();
                        targetHandler.Dispose();
                    }
                };
        }

        /// <summary>
        /// I wanna make this wizard a standalone component , but time reason .
        /// 
        /// This will be later . 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataExchangeWizard_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSrcDbType.Items.Clear();

            foreach (var item in MyGlobal.GetDbTypeWrapper)
            {
                cmbSrcDbType.Items.Add(item);
            }

            cmbSrcDbType.SelectedIndex = 0;

            cmbTargetDbType.Items.Clear();
            //Supported Type
            cmbTargetDbType.Items.Add(new DbTypeWrapper() { DisplayName = "SSCE", MyType = CoreE.UsedDatabaseType.SqlCE35 });
            cmbTargetDbType.Items.Add(new DbTypeWrapper() { DisplayName = "CSV", MyType = CoreE.UsedDatabaseType.CSV });
            cmbTargetDbType.Items.Add(new DbTypeWrapper() { DisplayName = "Sql Server", MyType = CoreE.UsedDatabaseType.SqlServer });
            cmbTargetDbType.Items.Add(new DbTypeWrapper() { DisplayName = "MySql", MyType = CoreE.UsedDatabaseType.MySql });
            cmbTargetDbType.Items.Add(new DbTypeWrapper() { DisplayName = "Sqlite", MyType = CoreE.UsedDatabaseType.Sqlite });
            cmbTargetDbType.SelectedIndex = 0;

            butBack.IsEnabled = false;
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            tab1.SelectedIndex -= 1;

            if (tab1.SelectedIndex == 0)
            {
                butBack.IsEnabled = false;
            }

            butNext.Content = "Next";
            butNext.IsEnabled = true;
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            //Do data transfer if finished
            if (tab1.SelectedIndex == tab1.Items.Count - 1)
            {
                if (!"DataTransferConfrimMsg".GetFromResourece().Confirm())
                {
                    return;
                }

                //Filter system table 
                List<string> tableList = new List<string>();

                listTablesList.SelectedItems.Cast<string>().ToList().ForEach(subItem =>
                {
                    tableList.Add(subItem);
                });

                if (tableList.Count < 1)
                {
                    "DataTransferNoTableMsg".GetFromResourece().Notify();
                    return;
                }

                DoExchangeData(srcHandler, targetHandler, tableList);
                return;
            }

            if (tab1.SelectedIndex == 2)
            {
                if (listTablesList.SelectedItems.Count == 0)
                {
                    "DataTransferSelectTableMsg".GetFromResourece().Notify();
                    return;
                }
            }

            #region Generate the Handler
            Debug.WriteLine(((StackPanel)((TabItem)tab1.SelectedItem).Content).Children[0].GetType());
            UIElement curElement = ((StackPanel)((TabItem)tab1.SelectedItem).Content).Children[0];
            if (curElement.GetType().BaseType == typeof(UserControl))
            {
                ISrcControl tempSrcControl = (ISrcControl)curElement;

                if (!tempSrcControl.X_CanForwardToNext)
                {
                    tempSrcControl.X_ShowErrorTips();
                    return;
                }
                else
                {
                    if (tempSrcControl.X_IsSourceHandler)
                    {
                        if ((srcHandler != null) && (srcHandler.IsOpened))
                        {
                            srcHandler.Close();
                        }
                        srcHandler = CreateHandler(curElement);
                        if (!srcHandler.IsOpened)
                        {
                            "ErrorMsg_CannotConnectToSource".GetFromResourece().Warning();
                            return;
                        }
                    }
                    else
                    {
                        if ((targetHandler != null) && (targetHandler.IsOpened))
                        {
                            targetHandler.Close();
                        }
                        targetHandler = CreateHandler(curElement);
                        if (!targetHandler.IsOpened)
                        {
                            "ErrorMsg_CannotConnectToTarget".GetFromResourece().Warning();
                            return;
                        }
                    }
                }
            }

            #endregion

            tab1.SelectedIndex = 1 + tab1.SelectedIndex;

            //Select Source 
            if (tab1.SelectedIndex == 1)
            {
                CurSrcDbType = ((DbTypeWrapper)cmbSrcDbType.SelectedItem).MyType;

                srcDbFileContainer.Children.Clear();

                ISrcControl srcControl = SourceControlFactory.GetProcessingControl(CurSrcDbType, true);
                if (srcControl == null)
                {
                    "DataTransfer_NotSupportDBType".GetFromResourece().Notify();
                    return;
                }
                srcDbFileContainer.Children.Add((UserControl)srcControl);
            }

            if (tab1.SelectedIndex == 2)
            {
                listTablesList.DataContext = null;

                List<string> t = srcHandler.GetTableListInDatabase();


                t.Sort();
                listTablesList.DataContext = t;

            }

            //Select Target Type
            if (tab1.SelectedIndex == 4)
            {
                CurTargetDbType = ((DbTypeWrapper)cmbTargetDbType.SelectedItem).MyType;

                targetDbFileContainer.Children.Clear();

                ISrcControl targetControl = SourceControlFactory.GetProcessingControl(CurTargetDbType, false);
                if (targetControl == null)
                {
                    "DataTransfer_NotSupportDBType".GetFromResourece().Notify();
                    return;
                }
                targetDbFileContainer.Children.Add((UserControl)targetControl);
            }

            //No next page
            if (tab1.SelectedIndex == tab1.Items.Count - 1)
            {
                resultSrcFile.Text = srcHandler.CurDatabase;
                resultTargetFile.Text = targetHandler.CurDatabase;
                butNext.Content = "Start";
            }

            butBack.IsEnabled = true;
        }

        private CoreEA.ICoreEAHander CreateHandler(UIElement curElement)
        {
            CoreEA.ICoreEAHander tempHandler = null;

            ISrcControl srcControl = (ISrcControl)curElement;

            try
            {
                #region Source Control Handler
                if (curElement.GetType() == typeof(SelectSourceDbFile_OleDB))
                {
                    SelectSourceDbFile_OleDB curUI = curElement as SelectSourceDbFile_OleDB;
                    switch (curUI.CurrentDbType)
                    {
                        case UsingOleDbType.Excel:
                            tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Excel).X_Handler;
                            tempHandler.Open((LoginInfo_Excel)srcControl.X_Result);
                            break;
                        case UsingOleDbType.Access:
                            tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.OleDb).X_Handler;
                            tempHandler.Open((LoginInfo_Oledb)srcControl.X_Result);
                            break;
                        case UsingOleDbType.CSV:
                            tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.CSV).X_Handler;
                            tempHandler.Open((LoginInfo_CSV)srcControl.X_Result);
                            break;
                    }


                }
                else if (curElement.GetType() == typeof(SelectSqlServerSource))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;
                    tempHandler.Open((LoginInfo_SqlServer)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectMySqlSource))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                    tempHandler.Open((LoginInfo_MySql)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectSqlite3DbFile))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;
                    tempHandler.Open((LoginInfo_Sqlite)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectSSCEFile))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
                    tempHandler.Open((LoginInfo_SSCE)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectEffiproz))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Effiproz).X_Handler;
                    tempHandler.Open((LoginInfo_Effiproz)srcControl.X_Result);
                }
                #endregion

                #region TargetControlHandler
                //Here the type is the class name of TargetUI
                else if (curElement.GetType() == typeof(SelectTargetDb_SqlCe))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
                    LoginInfo_SSCE ceInfo = srcControl.X_Result as LoginInfo_SSCE;
                    ceInfo.MaxDbSize = 3000;

                    tempHandler.Open(ceInfo);
                }
                else if (curElement.GetType() == typeof(SelectSqlServerTarget))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;
                    tempHandler.Open((LoginInfo_SqlServer)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectTargetSqliteDB))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;
                    tempHandler.Open((LoginInfo_Sqlite)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectTargetMySql))
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                    tempHandler.Open((LoginInfo_MySql)srcControl.X_Result);
                }
                else if (curElement.GetType() == typeof(SelectTargetCSVFile))
                {

                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.CSV).X_Handler;
                    LoginInfo_CSV tempLogInfo = srcControl.X_Result as LoginInfo_CSV;
                    tempLogInfo.Database += "1.csv";

                    tempHandler.Open(tempLogInfo);
                }
                #endregion
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }

            Debug.Assert(tempHandler != null, "Handler can't be null");
            return tempHandler;
        }

        /// <summary>
        /// Finish Step
        /// Notice : Each table schema from Core can't be changed .
        /// We will filter the table schema for each target handler in this method.
        /// </summary>
        /// <param name="srcHandler"></param>
        /// <param name="targetHandler"></param>
        /// <param name="tableList"></param>
        /// <param name="isShowUI">This value will be false when UT</param>
        private bool DoExchangeData(CoreEA.ICoreEAHander srcHandler, CoreEA.ICoreEAHander targetHandler, List<string> tableList
            , bool isShowUI = true)
        {
            bool ret = false;
            //Due to the old code use .ForEach way, it is hard to detect the error position and status 
            //So use this boolean object to indicate whether there has error in the for each cycle . 
            //Is true,didn't show the UI error messgae when UI , just set this value to true .
            bool isHasError = false;

            if (tableList.Count < 1)
            {
                "DataTransferNoTableMsg".GetFromResourece().Show();
                return false;
            }

            List<SyncResultArgs> result = new List<SyncResultArgs>();
            List<string> targetDbTableList = targetHandler.GetTableListInDatabase();
            PageSwitchProgressBar dp = null;
            if (isShowUI)
            {
                dp = PageSwitchProgressBar.X_BeginLoadingDialog();
            }
            try
            {
                foreach (string tableName in tableList)
                {
                    SyncResultArgs curStatus = new SyncResultArgs();

                    //Get target table schema info
                    BaseTableSchema tableSchmea = srcHandler.GetTableSchemaInfoObject(tableName);

                    try
                    {
                        //If table not existed ,then create it.
                        if (!targetDbTableList.Contains(tableName))
                        {


                            if (true)
                            {
                                switch (targetHandler.HostedType)
                                {
                                    case CoreE.UsedDatabaseType.OleDb:
                                        throw new NotImplementedException();
                                        break;
                                    case CoreE.UsedDatabaseType.SqlServer:
                                        targetHandler.CreateTable(tableSchmea);
                                        try
                                        {
                                            CommonUtil.ExchangeDataBetweenAnyDbs(srcHandler, targetHandler, tableName);
                                        }
                                        catch (Exception exee)
                                        {
                                            Debug.WriteLine(exee.Message);
                                            targetHandler.DeleteTable(tableSchmea.TableName);
                                        }
                                        break;
                                    case CoreE.UsedDatabaseType.MySql:
                                        targetHandler.CreateTable(tableSchmea);
                                        try
                                        {
                                            CommonUtil.ExchangeDataBetweenAnyDbs(srcHandler, targetHandler, tableName);
                                        }
                                        catch (Exception exee)
                                        {
                                            Debug.WriteLine(exee.Message);
                                            targetHandler.DeleteTable(tableSchmea.TableName);
                                        }
                                        finally
                                        {

                                        }
                                        break;
                                    case CoreE.UsedDatabaseType.SqlCE35:
                                        FilterTableSchemInfoWithSSCE(tableSchmea);
                                        targetHandler.CreateTable(tableSchmea);
                                        CommonUtil.CopyTable(srcHandler.GetConnection(), (SqlCeConnection)targetHandler.GetConnection(),
        string.Format("Select * from {0}", srcHandler.GetMaskedTableName(tableName)), tableName);
                                        break;
                                    case CoreE.UsedDatabaseType.Sqlite:
                                        targetHandler.CreateTable(tableSchmea);
                                        CommonUtil.ExchangeDataBetweenAnyDbs(srcHandler, targetHandler, tableName);
                                        break;
                                    case CoreE.UsedDatabaseType.Firebird:
                                        throw new NotImplementedException();
                                        break;
                                    case CoreE.UsedDatabaseType.CSV:
                                        targetHandler.CreateTable(tableSchmea);
                                        try
                                        {
                                            PreProcessCSV(srcHandler, targetHandler, tableName);
                                        }
                                        catch (Exception exee)
                                        {
                                            Debug.WriteLine(exee.Message);
                                            targetHandler.DeleteTable(tableSchmea.TableName);
                                        }
                                        break;
                                    case CoreE.UsedDatabaseType.Excel:
                                        throw new NotImplementedException();
                                    case CoreE.UsedDatabaseType.Oracle:
                                        throw new NotImplementedException();
                                    default:
                                        break;
                                }

                            }

                            curStatus.ProcessStatus = true;
                            curStatus.TableName = tableName;

                        }
                    }
                    catch (Exception sss)
                    {
                        curStatus.LastErrorMsg = sss.Message;
                        curStatus.ProcessStatus = false;
                        curStatus.TableName = tableName;
                        if (isShowUI)
                        {
                            sss.HandleMyException();
                        }
                        else
                        {
                            isHasError = true;
                        }
                        return false;
                    }
                    finally
                    {
                        result.Add(curStatus);
                        butNext.Content = "End";
                        butNext.IsEnabled = false;

                    }
                };
                if (isHasError)
                {
                    ret = false;
                }
                else
                {
                    ret = true;
                }
            }
            finally
            {
                if (isShowUI)
                {
                    if (!dp.IsDisposed)
                    {
                        dp.X_EndLoadingDialog();
                    }
                }
            }
            if (isShowUI)
            {
                if ("DataTransfer_ConfirmViewReport".GetFromResourece().Confirm())
                {
                    ShowSyncResult rShow = new ShowSyncResult(result);
                    rShow.ShowDialog();

                }

                Close();
            }

            return ret;
        }

        /// <summary>
        /// Filter table columns schems for SSCE
        /// 
        /// For example :the Single type in Access will convert to float .
        /// </summary>
        /// <param name="tableSchmea"></param>
        private void FilterTableSchemInfoWithSSCE(BaseTableSchema tableSchmea)
        {
            foreach (BaseColumnSchema item in tableSchmea.Columns)
            {
                if (item.ColumnType.ToLower() == "double")
                {
                    item.ColumnType = "float";
                }
                if (item.ColumnType.ToLower() == "nchar")
                {
                    item.ColumnType = "nvarchar";
                }
                if (item.ColumnType.ToLower() == "single")
                {
                    item.ColumnType = "float";
                }
            }
        }

        /// <summary>
        /// Process CSV Type
        /// All args are checked external
        /// So here is no need to do again
        /// </summary>
        /// <param name="srcHandler"></param>
        /// <param name="targetHandler"></param>
        /// <param name="tableList"></param>
        private void PreProcessCSV(CoreEA.ICoreEAHander srcHandler, CoreEA.ICoreEAHander targetHandler, List<string> tableList)
        {
            List<SyncResultArgs> result = new List<SyncResultArgs>();
            tableList.ForEach((tableName) =>
            {
                SyncResultArgs curStatus = new SyncResultArgs();
                try
                {
                    BaseTableSchema tableSchmea = srcHandler.GetTableSchemaInfoObject(tableName);

                    if (tableName.Contains(" "))
                    {
                        tableName = tableName.Replace(" ", "");
                    }
                    using (StreamWriter sw = File.CreateText(targetHandler.CurDatabase.Append(tableName + ".csv")))
                    {

                        //Create Column Header
                        StringBuilder sb = new StringBuilder();
                        tableSchmea.Columns.ForEach((column) =>
                            {
                                sb.Append(column.ColumnName);
                                sb.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                            });
                        sw.WriteLine(sb.ToString());

                        //Create Data
                        DataTable dt = srcHandler.GetAllDataFromTable(tableName);

                        foreach (DataRow row in dt.Rows)
                        {
                            StringBuilder builder = new StringBuilder();
                            foreach (var column in tableSchmea.Columns)
                            {
                                builder.Append(row[column.ColumnName]);
                                builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                            }
                            sw.WriteLine(builder.ToString());
                        }
                    }

                    curStatus.ProcessStatus = true;
                    curStatus.TableName = tableName;
                }
                catch (Exception accessEx)
                {
                    curStatus.LastErrorMsg = accessEx.Message;
                    curStatus.ProcessStatus = false;
                    curStatus.TableName = tableName;

                    accessEx.HandleMyException();
                    return;
                }
                finally
                {
                    result.Add(curStatus);
                    butNext.Content = "End";
                    butNext.IsEnabled = false;

                }
            });

            if ("DataTransfer_ConfirmViewReport".GetFromResourece().Confirm())
            {
                ShowSyncResult rShow = new ShowSyncResult(result);
                rShow.ShowDialog();

            }
            else
            {

            }

            Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcHandler"></param>
        /// <param name="targetHandler"></param>
        /// <param name="tableName">TableName is the file name</param>
        private void PreProcessCSV(CoreEA.ICoreEAHander srcHandler, CoreEA.ICoreEAHander targetHandler, string tableName)
        {
            try
            {
                BaseTableSchema tableSchmea = srcHandler.GetTableSchemaInfoObject(tableName);

                if (tableName.Contains(" "))
                {
                    tableName = tableName.Replace(" ", "");
                }
                using (StreamWriter sw = File.CreateText(targetHandler.CurDatabase.Append(tableName + ".csv")))
                {

                    //Create Column Header
                    StringBuilder sb = new StringBuilder();
                    tableSchmea.Columns.ForEach((column) =>
                    {
                        sb.Append(column.ColumnName);
                        sb.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                    });
                    sw.WriteLine(sb.ToString());

                    //Create Data
                    DataTable dt = srcHandler.GetAllDataFromTable(tableName);

                    foreach (DataRow row in dt.Rows)
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (var column in tableSchmea.Columns)
                        {
                            builder.Append(row[column.ColumnName]);
                            builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                        }
                        sw.WriteLine(builder.ToString());
                    }
                }

            }
            catch (Exception accessEx)
            {
                accessEx.HandleMyException();
                return;
            }
            finally
            {
                butNext.Content = "End";
                butNext.IsEnabled = false;

            }
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            listTablesList.SelectAll();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            listTablesList.SelectedItems.Clear();
        }

    }
}
