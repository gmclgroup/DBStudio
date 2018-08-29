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
using MPL.Diagnostics;
using Microsoft.Windows.Controls;
using DBStudio.MyInterface;
using DBStudio.MyInterfaceImplementations;
using System.ComponentModel;
using DBStudio.Effiproz;
using CoreEA.GlobalDefine;
using PagedDataGridControls;


namespace DBStudio
{
    /// <summary>
    /// Interaction logic for NewSqlServerIDE.xaml
    /// </summary>
    public partial class RibbionIDE : BaseMainPage
    {
        /// <summary>
        /// Commands collection for undo and redo
        /// </summary>
        ObservableCollection<String> CmdHistoryList = new ObservableCollection<String>();
        /// <summary>
        /// 
        /// </summary>
        private int CommandCurrency = 0;
        /// <summary>
        /// Context menu factory 
        /// each provider should give one
        /// </summary>
        CreateCMenuFactory GloablMenuFactory;
        /// <summary>
        /// Timer for calculate the sql execution time
        /// </summary>
        Stopwatch watch = new Stopwatch();
        /// <summary>
        /// Sql execution worker for multi thread
        /// </summary>
        BackgroundWorker worker = null;
        /// <summary>
        /// Timer to open or close the stopwatch
        /// </summary>
        DispatcherTimer timer = null;

        /// <summary>
        /// True mean current closing IDE not whole application
        /// False mean other condition
        /// This value used when this closing event
        /// </summary>
        bool isGoingToCloseCurrentIDE = false;

        /// <summary>
        /// Current TreeNode tag value
        /// </summary>
        internal TreeItemArgs CurrentTreeArgs
        {
            get
            {

                if (mainTreeView.SelectedItem == null)
                    return null;

                return ((TreeItemArgs)((TreeViewItem)mainTreeView.SelectedItem).Tag);
            }
        }

        /// <summary>
        /// Allow Mulit Result Pane
        /// Here use paged datagrid 
        /// </summary>
        internal PagedDataGrid CurrentPagedDataGrid
        {
            get
            {
                Wpf.Controls.TabItem tabItem = tabControl_ForSqlResult.SelectedItem as Wpf.Controls.TabItem;
                if (tabItem != null)
                {
                    PagedDataGrid dgGrid = tabItem.Content as PagedDataGrid;
                    if (dgGrid != null)
                    {
                        return dgGrid;
                    }
                }

                return null;
            }
        }


        /// <summary>
        /// Allow Multi Sql Query Text Editor
        /// </summary>
        internal ISqlQueryTextor CurrentSqlQueryEditor
        {
            get
            {
                if (tabControl_ForSqlQuery.SelectedItem == null)
                {
                    if (tabControl_ForSqlQuery.Items.Count > 0)
                    {
                        tabControl_ForSqlQuery.SelectedIndex = 0;
                    }
                }

                //TabItem tabItem = tabControl_ForSqlQuery.SelectedItem as TabItem;
                Wpf.Controls.TabItem tabItem = tabControl_ForSqlQuery.SelectedItem as Wpf.Controls.TabItem;
                if (tabItem != null)
                {
                    DockPanel container = tabItem.Content as DockPanel;
                    if (container == null)
                    {
                        return null;
                    }

                    ISqlQueryTextor myTextEditor = container.Children[1] as ISqlQueryTextor;

                    if (myTextEditor != null)
                    {
                        myTextEditor.Font = new System.Drawing.Font(
                            Properties.Settings.Default.SqlEditorFontName,
                            Properties.Settings.Default.SqlEditorFontSize);

                        return myTextEditor;
                    }
                }

                return null;
            }

        }

        public RibbionIDE()
        {
            InitializeComponent();
            App.Current.MainWindow = this;
            GloablMenuFactory = new CreateCMenuFactory(App.MainEngineer.HostedType);
            InitRoutedUICommand();

            InitSelf();
            CmdHistoryList.CollectionChanged += (m, n) =>
                {
                    CommandCurrency = CmdHistoryList.Count;
                };

        }


        /// <summary>
        /// If use ISharpTextEditor some properties need commented
        /// </summary>
        private void InitSelf()
        {
            switch (App.MainEngineer.HostedType)
            {
                case CoreEA.CoreE.UsedDatabaseType.OleDb:
                    InitRibbonMenu(ribbionMenu_ForOleDb);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.SqlServer:
                    InitRibbonMenu(ribbionMenu_ForSqlServer);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.MySql:
                    InitRibbonMenu(ribbionMenu_ForMySql);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.SqlCE35:
                    InitRibbonMenu(ribbionMenu_ForSSCE);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Sqlite:
                    InitRibbonMenu(ribbionMenu_ForSqlite);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Firebird:
                    InitRibbonMenu(ribbionMenu_ForFirBird);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Excel:
                    InitRibbonMenu(ribbionMenu_ForExcel);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.CSV:
                    InitRibbonMenu(ribbionMenu_ForCSV);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Oracle:
                    InitRibbonMenu(ribbionMenu_ForOracle);
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Effiproz:
                    InitRibbonMenu(ribbionMenu_ForElliproz);
                    break;
                default:
                    break;
            }

            Loaded += new RoutedEventHandler(RibbonIDE_Loaded);

            //Handle closing event
            Closing += new System.ComponentModel.CancelEventHandler(RibbionIDE_Closing);

            Unloaded += (sender, e) =>
                {
                    CurrentSqlQueryEditor.Dispose();
                    App.ResetMainEngineer();
                };

        }

        void RibbionIDE_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //If only close current ide ui ,then do nothing 
                //otherwise popup notify confirm dialog before really closing
                if (isGoingToCloseCurrentIDE)
                {
                    App.MainEntry.Visibility = Visibility.Visible;
                }
                else
                {
                    if ("ExitAppConfirmMsg".GetFromResourece().Confirm())
                    {
                        App.MainEntry.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
                this.Logger().WriteError(ee);
            }
        }


        /// <summary>
        /// Center Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void myControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((e.KeyCode == wf.Keys.B) && (e.Modifiers == wf.Keys.Control))
            {

                Undo();
            }
            else if ((e.KeyCode == wf.Keys.N) && (e.Modifiers == wf.Keys.Control))
            {
                Redo();
            }

            if (String.IsNullOrEmpty(CurrentSqlQueryEditor.Text))
            {
                return;
            }

            //Don't allow enter key know
            if (
                //(e.KeyCode == wf.Keys.Enter)||
                ((e.KeyCode == wf.Keys.X) && (e.Modifiers == wf.Keys.Alt))
                )
            {
                butQueryClick(null, null);
            }

            else if (e.KeyCode == wf.Keys.F1)
            {
                CallHelp();
            }

            else if ((e.KeyCode == wf.Keys.J) && (e.Modifiers == wf.Keys.Control))
            {
                DynaControls.GetDbTypeControl dj = new DynaControls.GetDbTypeControl();
                dj.Location = new System.Drawing.Point(200, 200);

                dj.AfterSelectedValue += (myArgs) =>
                {

                };

                dj.ShowDialog();

                //e.Handled = true;
            }
        }

        private void Redo()
        {
            if (CommandCurrency + 1 <= CmdHistoryList.Count)
            {
                CommandCurrency++;
                if (CmdHistoryList.Count > CommandCurrency)
                {
                    CurrentSqlQueryEditor.Text = CmdHistoryList[CommandCurrency];
                }
            }
        }

        private void Undo()
        {
            if (CommandCurrency - 1 >= 0)
            {
                CommandCurrency--;
                if (CommandCurrency >= 0)
                {
                    CurrentSqlQueryEditor.Text = CmdHistoryList[CommandCurrency];
                }
            }
        }

        /// <summary>
        /// here notice , 
        /// We need connect to a network sql server intance  
        /// So may be get the table and column /index information will cost a lot of time . 
        /// We should use mutli thread to refresh the UI . 
        /// So here we can't assign to current selected item . 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RibbonIDE_Loaded(object sender, RoutedEventArgs e)
        {
            App.MainEntry.Visibility = Visibility.Hidden;

            //Create SqlEditor
            MenuItem_Click_NewQueryItem(null, null);

            #region Set Title
            string title = string.Empty;
            switch (App.MainEngineer.HostedType)
            {
                case CoreEA.CoreE.UsedDatabaseType.OleDb:
                    title = "Oledb based";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.SqlServer:
                    title = "Sql Server";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.MySql:
                    title = "My Sql";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.SqlCE35:
                    title = "Sql Server Compact Edition 3.5 ." + App.MainEngineer.CurDatabase.ToMyString();
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Sqlite:
                    title = "Sqlite 3.0";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Firebird:
                    title = "Firebird/interbase";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Excel:
                    title = "Excel";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.CSV:
                    title = "CSV";
                    break;
                case CoreEA.CoreE.UsedDatabaseType.Effiproz:
                    title = "Effiproz";
                    break;
                default:
                    break;
            }

            Title = string.Format("CurDbMode".GetFromResourece(), title);
            #endregion

            LoadAllSchemaInfo();

            //Binding double click event
            mainResultGrid.PreviewMouseDoubleClick += new MouseButtonEventHandler(DataGrid_MouseDoubleClick);

            mainResultGrid.RaiseExceptionNotify += new RoutedEventHandler(mainResultGrid_RaiseExceptionNotify);
        }

        void mainResultGrid_RaiseExceptionNotify(object sender, RoutedEventArgs e)
        {
            MyEventRoutedArgs args = e as MyEventRoutedArgs;
            if (null != args)
            {
                statusBarNotify.Text = args.CurrentException.Message;
            }
        }

        private void LoadAllSchemaInfo()
        {

            //Clear to Orginal Status
            this.mainTreeView.Items.Clear();

            #region Enum the tables
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Tables);
            List<string> curTableList = App.MainEngineer.GetTableListInDatabase();
            foreach (string tempTable in curTableList)
            {
                //If this is a table item , then automatic add a empty sub item
                //for expanding using
                //each time user want to expand this item and see more detail , we will 
                //clear this temporary item and generate the valid data
                TreeViewItem item = new TreeViewItem();
                item.Header = tempTable;
                item.Tag = new TreeItemArgs() { ItemType = TreeItemType.Table, TableName = tempTable };

                item.Items.Add(new TreeViewItem());
                #region Generate the tooltip in code
                StackPanel panel = new StackPanel();
                ToolTip tableTooltip = new ToolTip();
                tableTooltip.Content = panel;
                TextBlock text = new TextBlock();
                text.FontSize = 12;
                text.Text = "TableItemUsage".GetFromResourece();
                panel.Children.Add(text);

                item.ToolTip = tableTooltip;
                #endregion

                item.Expanded += new RoutedEventHandler(item_Expanded);
                mainTreeView.Items.Add(item);
            }
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Tables);
            #endregion

            EmulatorSystemViews(true);

            EmulatorViews(true);

            EmulatorStoredProcedures(true);

            EmulatorTriggers(true);

            //Utility.TreeViewHelper.ExpandAll(mainTreeView);
            mainTreeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(mainTreeView_SelectedItemChanged);

        }

        private void EmulatorTriggers(bool isFirstRun)
        {
            #region Emulator the Triggers
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Triggers);
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                if (!isFirstRun)
                {
                    foreach (TreeViewItem subItem in mainTreeView.Items)
                    {
                        if (((TreeItemArgs)subItem.Tag).ItemType == TreeItemType.TriggersParent)
                        {
                            mainTreeView.Items.Remove(subItem);
                            break;
                        }
                    }
                }

                List<BaseTriggerInfo> triggerList = null;
                try
                {
                    triggerList = App.MainEngineer.GetTriggersInfo();
                }
                catch (Exception ee)
                {
                    statusBarStatus.Text = "ErrorInfo_GetTriggerListError".GetFromResourece() + ee.Message;
                    this.Logger().WriteError(ee);
                    return;
                }

                TreeViewItem triggerParentNode = new TreeViewItem();
                triggerParentNode.Foreground = new SolidColorBrush(Colors.Red);
                triggerParentNode.Header = "TriggerName".GetFromResourece();
                triggerParentNode.Tag = new TreeItemArgs() { ItemType = TreeItemType.TriggersParent };
                mainTreeView.Items.Add(triggerParentNode);
                if (triggerList == null)
                {
                    return;
                }
                foreach (var subRow in triggerList)
                {
                    string triggerName = subRow.TriggerName;
                    TreeViewItem item = new TreeViewItem();
                    item.Header = triggerName;
                    item.Tag = new TreeItemArgs()
                    {
                        ItemType = TreeItemType.Triggers,
                        TriggerName = triggerName
                    };
                    triggerParentNode.Items.Add(item);
                }
            });
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Triggers);
            #endregion
        }

        private void EmulatorStoredProcedures(bool isFirstRun)
        {
            #region Emulator the StoredProcedures
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_StoredProcedures);

            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                if (!isFirstRun)
                {
                    foreach (TreeViewItem subItem in mainTreeView.Items)
                    {
                        if (((TreeItemArgs)subItem.Tag).ItemType == TreeItemType.StoredProcedureParentNode)
                        {
                            mainTreeView.Items.Remove(subItem);
                            break;
                        }
                    }
                }

                List<BaseStoredProcedureInfo> dsSPList = null; ;

                try
                {
                    dsSPList = App.MainEngineer.GetStoredProceduresList();
                }
                catch (Exception ee)
                {
                    statusBarStatus.Text = "ErrorInfo_GetSPListError".GetFromResourece() + ee.Message;
                    this.Logger().WriteError(ee);
                    return;
                }

                TreeViewItem spParentNode = new TreeViewItem();
                spParentNode.Foreground = new SolidColorBrush(Colors.Red);
                spParentNode.Header = "SpName".GetFromResourece();
                spParentNode.Tag = new TreeItemArgs() { ItemType = TreeItemType.StoredProcedureParentNode };
                mainTreeView.Items.Add(spParentNode);

                if (dsSPList == null)
                {
                    return;
                }
                foreach (BaseStoredProcedureInfo spInfo in dsSPList)
                {
                    string spName = spInfo.ProcedureName;
                    TreeViewItem item = new TreeViewItem();
                    item.Header = spName;
                    item.Tag = new TreeItemArgs() { ItemType = TreeItemType.StoredProcedure, StoredProcedureName = spName };

                    spParentNode.Items.Add(item);

                }


            });

            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_StoredProcedures);
            #endregion
        }

        private void EmulatorViews(bool isFirstRun)
        {
            #region Emulator the Views
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Views);
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                if (!isFirstRun)
                {
                    foreach (TreeViewItem subItem in mainTreeView.Items)
                    {
                        if (((TreeItemArgs)subItem.Tag).ItemType == TreeItemType.ViewParent)
                        {
                            mainTreeView.Items.Remove(subItem);
                            break;
                        }
                    }
                }


                List<BaseViewInfo> viewList = null; ;
                try
                {
                    viewList = App.MainEngineer.GetViews();
                }
                catch (Exception ee)
                {
                    statusBarStatus.Text = "ErrorInfo_GetViewListError".GetFromResourece() + ee.Message;
                    this.Logger().WriteError(ee);
                    return;
                }

                TreeViewItem viewNode = new TreeViewItem();
                viewNode.Foreground = new SolidColorBrush(Colors.Red);
                viewNode.Header = "LabelView".GetFromResourece();
                viewNode.Tag = new TreeItemArgs() { ItemType = TreeItemType.ViewParent };
                mainTreeView.Items.Add(viewNode);

                if (viewList == null)
                {
                    return;
                }
                foreach (var subRow in viewList)
                {
                    string viewName = subRow.ViewName;
                    TreeViewItem item = new TreeViewItem();
                    item.Header = viewName;

                    item.Tag = new TreeItemArgs() { ItemType = TreeItemType.View, ViewName = viewName };
                    viewNode.Items.Add(item);
                }
            });


            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Views);
            #endregion
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                if (item.Items.Count <= 1)
                {
                    item.Items.Clear();

                    TreeItemArgs args = item.Tag as TreeItemArgs;
                    if (args == null)
                    {
                        return;
                    }

                    GetColumnsInfo(args.TableName);

                    GenerateContextMenu(args);
                }
            }



        }

        void GenerateContextMenu(TreeItemArgs args)
        {
            mainTreeView.ContextMenu = null;
            mainTreeView.ContextMenu = GloablMenuFactory.GetMenu(args);
            IMenu iMenu = (IMenu)mainTreeView.ContextMenu.Tag;

            iMenu.ItemClickWithSqlCmdEvent -= new EventHandler(iMenu_ItemClickWithSqlCmdEvent);
            iMenu.ItemClickWithSqlCmdEvent += new EventHandler(iMenu_ItemClickWithSqlCmdEvent);

            iMenu.ItemClickWithResult -= new EventHandler(iMenu_ItemClickWithResult);
            iMenu.ItemClickWithResult += new EventHandler(iMenu_ItemClickWithResult);

            iMenu.RefreshAllNodeEvent -= new EventHandler(RibbionIDE_RefreshAllNodeEvent);
            iMenu.RefreshAllNodeEvent += new EventHandler(RibbionIDE_RefreshAllNodeEvent);

            iMenu.RefreshSystemViewNodeEvent -= new EventHandler(RibbionIDE_RefreshSystemViewNodeEvent);
            iMenu.RefreshSystemViewNodeEvent += new EventHandler(RibbionIDE_RefreshSystemViewNodeEvent);

        }
        /// <summary>
        /// Refresh Column Info also can use this method
        /// Notice : 
        /// Currently Load Column and Index info together .
        /// Later consider refactor it . 
        /// </summary>
        /// <param name="tableName"></param>
        void GetColumnsInfo(object tableName)
        {
#if DEBUG
            ListColumnsAndIndexesInTable(tableName.ToString());
#else

            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                ListColumnsAndIndexesInTable(tableName.ToString());
            });
#endif

        }

        /// <summary>
        /// Refresh Current Selected Table Schema
        /// </summary>
        /// <param name="tableName"></param>
        void RefreshColumnsInfo(string tableName)
        {
            GetColumnsInfo(tableName);
        }

        /// <summary>
        /// Refresh System View Schema Info
        /// </summary>
        void RefreshSystemViewInfo()
        {
            EmulatorSystemViews(false);
        }

        void RefreshTrggerInfo()
        {
            EmulatorTriggers(false);
        }

        void RefreshStoredProceduresInfo()
        {
            EmulatorStoredProcedures(false);
        }

        void RefreshViewsInfo()
        {
            EmulatorViews(false);
        }

        /// <summary>
        /// Support Recall (refresh) this function 
        /// </summary>
        private void EmulatorSystemViews(bool isFirstRun)
        {
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_SystemViews);
            #region Emulator the System Viewer

            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                if (!isFirstRun)
                {
                    foreach (TreeViewItem subItem in mainTreeView.Items)
                    {
                        if (((TreeItemArgs)subItem.Tag).ItemType == TreeItemType.SystemViewNode)
                        {
                            mainTreeView.Items.Remove(subItem);
                            break;
                        }
                    }
                }

                TreeViewItem systemViewerItem = new TreeViewItem();
                systemViewerItem.Foreground = new SolidColorBrush(Colors.Red);
                systemViewerItem.Header = "SysSchemaViews".GetFromResourece();
                systemViewerItem.Tag = new TreeItemArgs() { ItemType = TreeItemType.SystemViewNode };
                mainTreeView.Items.Add(systemViewerItem);
                //if (App.MainEngineer != null)
                //{
                List<string> sysViewsList = App.MainEngineer.GetSystemViewList();
                foreach (var eachView in sysViewsList)
                {
                    TreeViewItem sysViewNodeItem = new TreeViewItem();
                    sysViewNodeItem.Tag = new TreeItemArgs() { ItemType = TreeItemType.SystemView_SchemaNode, SystemViewName = eachView };
                    sysViewNodeItem.Header = eachView;
                    systemViewerItem.Items.Add(sysViewNodeItem);

                    TreeViewItem eachViewItem = new TreeViewItem();
                    eachViewItem.Header = "ColumnsName".GetFromResourece();
                    sysViewNodeItem.Items.Add(eachViewItem);

                    List<string> columnList = App.MainEngineer.GetSystemViewColumnsNameByViewName(eachView);
                    foreach (var eachColumn in columnList)
                    {
                        TreeViewItem eachColumnItem = new TreeViewItem();
                        eachColumnItem.Tag = new TreeItemArgs() { ItemType = TreeItemType.SystemView_Schema_ColumnNode, ColumnName = eachColumn };
                        eachColumnItem.Header = eachColumn;
                        eachViewItem.Items.Add(eachColumnItem);
                    }
                }
                // }
            });

            #endregion
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_SystemViews);
        }

        /// <summary>
        /// Get Current Selected TreeItem Value
        /// </summary>
        /// <returns></returns>
        private TreeItemArgs GetArgsFromSelectedItem()
        {
            TreeItemArgs result = null;

            TreeViewItem item = mainTreeView.SelectedItem as TreeViewItem;
            if (item != null)
            {
                result = item.Tag as TreeItemArgs;
            }
            return result;
        }

        void ExecuteSqlCmd(string e, bool IsNeedExecute)
        {
            if (string.IsNullOrEmpty(e))
            {
                throw new ArgumentException();
            }

            CurrentSqlQueryEditor.Text = e;
            if (IsNeedExecute)
            {
                CmdHistoryList.Add(CurrentSqlQueryEditor.Text);

                butQueryClick(null, null);
                CurrentSqlQueryEditor.Text = string.Empty;
            }
            else
            {
                CurrentSqlQueryEditor.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            #region Process Node
            TreeItemArgs args = GetArgsFromSelectedItem();
            if ((args != null) && (args.ItemType == TreeItemType.Table))
            {

                TreeViewItem mysubItem = mainTreeView.SelectedItem as TreeViewItem;
                //If empty or only has one node ,then regenerate the data
                if (mysubItem.Items.Count <= 1)
                {
                    mysubItem.Items.Clear();

                    GetColumnsInfo(args.TableName);

                }
            }
            #endregion

            GenerateContextMenu(args);

        }

        void iMenu_ItemClickWithResult(object sender, EventArgs e)
        {
            object resultInfo = sender;

            if (resultInfo == null)
            {
                "QueryWithNoDataResultNotifyMsg".GetFromResourece().Notify();
                return;
            }

            //CurrentDataGrid.DataContext = resultInfo;
            SetResultInUI(resultInfo);
        }

        void iMenu_ItemClickWithSqlCmdEvent(object sender, EventArgs e)
        {
            string cmdStr = sender.ToString();

            if (string.IsNullOrEmpty(cmdStr.ToString()))
            {
                "InvalidSqlCmdNotifyMsg".Notify();
                return;
            }
            ExecuteSqlCmd(cmdStr.ToString(), ((NotifySqlCmdArgs)e).IsExecuteCommand);
        }

        void RibbionIDE_RefreshSystemViewNodeEvent(object sender, EventArgs e)
        {
            EmulatorSystemViews(false);
        }

        void RibbionIDE_RefreshAllNodeEvent(object sender, EventArgs e)
        {
            LoadAllSchemaInfo();
        }


        /// <summary>
        /// Get the Current Item which saved such table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        TreeViewItem GetItem(string tableName)
        {
            foreach (TreeViewItem item in mainTreeView.Items)
            {
                TreeItemArgs args = item.Tag as TreeItemArgs;
                if (null == args)
                {
                    return null;
                }
                else
                {
                    if (args.TableName == tableName)
                    {
                        return item;
                    }

                }

            }

            //If run here mean didn't find anything.
            return null;

            //var a = from b in mainTreeView.Items.Cast<TreeViewItem>()
            //        where ((TreeItemArgs)b.Tag).TableName == tableName
            //        select b;

            //if (a.Count() > 0)
            //{
            //    return a.First();
            //}
            //else
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// Display the column info of this table in the curretn TreeViewItem
        /// 
        /// This function is same to App.MainEngineer.GetTableSchemaInfoObject method
        /// We recommend use GetTableSchemaInfoObject()
        /// but currently this changed will be seperated in some pieces.
        /// Because some of codes rely on old methods.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <param name="p"></param>
        private void ListColumnsAndIndexesInTable(string tableName)
        {

            //PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Tables);
            BaseTableSchema schemaInfo = App.MainEngineer.GetTableSchemaInfoObject(tableName);
            //PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Tables);

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Columns);
            TreeViewItem curItem = GetItem(tableName);
            curItem.Items.Clear();

            Debug.WriteLine("Processed Table ---->" + tableName);

            TreeViewItem columnItem = new TreeViewItem();
            columnItem.Header = "ColumnsName".GetFromResourece();
            columnItem.Tag = new TreeItemArgs() { ItemType = TreeItemType.ColumnParentNode, TableName = tableName };

            curItem.Items.Add(columnItem);

            TreeViewItem indexItem = new TreeViewItem();

            indexItem.Header = "IndexesName".GetFromResourece();
            indexItem.Tag = new TreeItemArgs() { ItemType = TreeItemType.IndexParentNode, TableName = tableName };
            curItem.Items.Add(indexItem);

            try
            {
                #region Enum Columns
                DataTable dataRowCollections = App.MainEngineer.GetColumnInfoFromTable(tableName);

#if DEBUG
                dataRowCollections.WriteXml(GlobalDefine.MyGlobal.GlobalDebugFolder + "columns.xml");
#else

#endif

                foreach (BaseColumnSchema columnSchema in schemaInfo.Columns)
                {
                    TreeViewItem item = new TreeViewItem();
                    string rowName = columnSchema.ColumnName;
                    string rowType = columnSchema.ColumnType;


                    if (columnSchema.ColumnLength != 0.0d)
                    {
                        rowType = string.Format("{0}({1})", rowType, columnSchema.ColumnLength.ToString());

                    }
                    //If is primary key then add the icon
                    if (schemaInfo.PrimaryKey.IsContaintPrimaryColumn(columnSchema.ColumnName))
                    {
                        //This is traditional mode to show the column 
                        string header = String.Format("{0}  ({1}  {2})", rowName, rowType, "(Primary Key)");
                        //item.Header =header;

                        //This is the wpf mode to show the column whick contain a icon indicate this is a PK 
                        StackPanel panel = new StackPanel();
                        panel.Orientation = Orientation.Horizontal;
                        Image image = new Image();
                        image.Width = 16;
                        image.Height = 16;
                        image.Source = new BitmapImage(new Uri("Images/Image2/Pk.png", UriKind.Relative));
                        panel.Children.Add(image);
                        TextBlock text = new TextBlock();
                        text.Text = header;
                        panel.Children.Add(text);

                        item.Header = panel;
                    }
                    else
                    {
                        //Like sqlserver management studio ,place the column type in the middle of ()
                        item.Header = String.Format("{0}  ({1})", rowName, rowType);
                    }

                    item.Tag = new TreeItemArgs()
                    {
                        ItemType = TreeItemType.Column,
                        TableName = tableName,
                        ColumnName = rowName,
                    };

                    columnItem.Items.Add(item);
                    //Generate tooltips

                    StackPanel tooltipPanel = new StackPanel();
                    ToolTip tableTooltip = new ToolTip();
                    tableTooltip.Content = tooltipPanel;
                    TextBlock tooltipText = new TextBlock();
                    tooltipText.FontSize = 12;
                    tooltipText.Text = "ColumnItemUsage".GetFromResourece();
                    tooltipPanel.Children.Add(tooltipText);

                    item.ToolTip = tableTooltip;
                }


                //foreach (DataRow row in dataRowCollections.Rows)
                //{
                //    TreeViewItem item = new TreeViewItem();
                //    string rowName= row["COLUMN_NAME"].ToString();
                //    string rowType=row["DATA_TYPE"].ToString();

                //    if(rowType.Contains("char"))
                //    {
                //        if (row["CHARACTER_MAXIMUM_LENGTH"] != null)
                //        {
                //            int maxCharLegnth = int.Parse(row["CHARACTER_MAXIMUM_LENGTH"].ToString());

                //            rowType = string.Format("{0}({1})", rowType, maxCharLegnth.ToString());
                //        }
                //    }
                //    //If is primary key then add the icon
                //    if (pKeyList.Contains(rowName))
                //    {
                //        //This is traditional mode to show the column 
                //        string header= String.Format("{0}  ({1}  {2})", rowName,rowType ,"(Primary Key)");
                //        //item.Header =header;

                //        //This is the wpf mode to show the column whick contain a icon indicate this is a PK 
                //        StackPanel panel = new StackPanel();
                //        panel.Orientation = Orientation.Horizontal;
                //        Image image = new Image();
                //        image.Width = 16;
                //        image.Height = 16;
                //        image.Source =new BitmapImage(new Uri("Images/Image2/Pk.png", UriKind.Relative));
                //        panel.Children.Add(image);
                //        TextBlock text = new TextBlock();
                //        text.Text = header;
                //        panel.Children.Add(text);

                //        item.Header = panel;
                //    }
                //    else
                //    {
                //        //Like sqlserver management studio ,place the column type in the middle of ()
                //        item.Header = String.Format("{0}  ({1})", rowName, rowType);
                //    }

                //    item.Tag = new TreeItemArgs()
                //    {
                //        ItemType = TreeItemType.Column,
                //        TableName = tableName,
                //        ColumnName = rowName,
                //    };

                //    columnItem.Items.Add(item);
                //    //Generate tooltips

                //    StackPanel tooltipPanel = new StackPanel();
                //    ToolTip tableTooltip = new ToolTip();
                //    tableTooltip.Content = tooltipPanel;
                //    TextBlock tooltipText = new TextBlock();
                //    tooltipText.FontSize = 12;
                //    tooltipText.Text = "ColumnItemUsage".GetFromResourece();
                //    tooltipPanel.Children.Add(tooltipText);

                //    item.ToolTip = tableTooltip;
                //}

                #endregion

                #region Enum indexes
                DataTable indexCollections = App.MainEngineer.GetIndexInfoFromTable(tableName);

#if DEBUG
                indexCollections.WriteXml(GlobalDefine.MyGlobal.GlobalDebugFolder + "indexes.xml");
#else

#endif

                foreach (DataRow row in indexCollections.Rows)
                {
                    TreeViewItem item = new TreeViewItem();

                    item.Header = String.Format("{0}  {1}  {2}", row["INDEX_NAME"], "", "");

                    item.Tag = new TreeItemArgs()
                    {
                        ItemType = TreeItemType.Index,
                        TableName = tableName,
                        IndexName = row["INDEX_NAME"].ToString()
                    };
                    indexItem.Items.Add(item);
                    //Generate tooltips

                    StackPanel panel = new StackPanel();
                    ToolTip tableTooltip = new ToolTip();
                    tableTooltip.Content = panel;
                    TextBlock text = new TextBlock();
                    text.FontSize = 12;
                    text.Text = "IndexItemUsage".GetFromResourece();
                    panel.Children.Add(text);

                    item.ToolTip = tableTooltip;
                }
                #endregion

                PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadSchemaInfo_Columns);
            }
            catch (Exception EE)
            {

                EE.HandleMyException();
                this.Logger().WriteError(EE);
                return;
            }

        }



        /// <summary>
        /// Do command collection 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butQueryClick(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(CurrentSqlQueryEditor.Text))
            {
                //"InputSqlCommandNotifyMsg".GetFromResourece().Notify();
                return;
            }

            //If user select some texts then only execute selected sql command
            //otherwise do all the text command in the Editor
            //This action is like Sqlserver Managemnet Studio
            string sqlCmdString = string.Empty;
            if (CurrentSqlQueryEditor.SelectedText.IsEmpty())
            {
                sqlCmdString = CurrentSqlQueryEditor.Text;
            }
            else
            {
                sqlCmdString = CurrentSqlQueryEditor.SelectedText;
            }

            ICmd myCmd = GlobalDefine.MyGlobal.ParseSqlCmdString(sqlCmdString, App.MainEngineer);
            if (myCmd == null)
            {
                NotifyInfoInResultPanel("CannotParseSqlCmdNotifyMsg".GetFromResourece(), false);
                return;
            }
            //Calc the executation time
            watch.Reset();
            watch.Start();

            //When command execute ,here will process the UI 
            //But it will affect the UI Thread ,so here need dispatcher to call it.
            myCmd.CmdExecutedOk += (mySender, myArgs) =>
                {
                    this.Dispatcher.BeginInvoke((ThreadStart)delegate
                    {
                        CmdBase.MyCmdArgs args = myArgs as CmdBase.MyCmdArgs;
                        Debug.Assert(args != null);

                        if (args.ExecuteOK)
                        {
                            //We are now no need to pop up the result 
                            //"ExeCmdOKNotifyMsg".GetFromResourece().Notify();
                            NotifyInfoInResultPanel("ExeCmdOKNotifyMsg".GetFromResourece(), true);
                        }
                        else
                        {
                            NotifyInfoInResultPanel(args.ErrorMsg, false);
                        }


                        if (args.IsNeedRefreshUI)
                        {
                            LoadAllSchemaInfo();
                        }
                    });
                };

            //Show progress bar for UI
            progressBar.Visibility = System.Windows.Visibility.Visible;


            #region Show the process time
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            #endregion

            #region Execute in background thread
            ExecuteArgs cmdArgs = new ExecuteArgs();
            cmdArgs.CommandString = sqlCmdString;
            cmdArgs.CurrentCommand = myCmd;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync(cmdArgs);
            #endregion
        }

        /// <summary>
        /// Refresh the query time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                statusBarCostTime.Text = watch.Elapsed.ToString();
            });
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExecuteArgs args = e.Argument as ExecuteArgs;
            Debug.Assert(args != null);
            string sqlCmdString = args.CommandString;
            ICmd myCmd = args.CurrentCommand;

            object result = myCmd.ExecuteWithResult(sqlCmdString);

            e.Result = result;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Visibility = System.Windows.Visibility.Visible;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
            timer = null;

            if (e.Cancelled)
            {
                return;
            }
            if (e.Error != null)
            {
                NotifyInfoInResultPanel("ExecuteCmdWithNoResult".GetFromResourece() + e.Error, false);
                return;
            }

            object result = e.Result;
            if (result != null)
            {
                DataSet ds = result as DataSet;
                if (ds != null)
                {
                    //CurrentDataGrid.DataContext = ds.Tables[0];
                    SetResultInUI(ds.Tables[0]);
                    NotifyInfoInResultPanel("ExecuteCmdWithResultOK".GetFromResourece(), true);
                }
                else
                {
                    NotifyInfoInResultPanel("Unkown result", true);
                }
            }
            else
            {
                NotifyInfoInResultPanel("ExecuteCmdWithNoResult".GetFromResourece(), false);
            }

            worker.Dispose();
            worker = null;
        }

        private void NotifyInfoInResultPanel(string msg, bool isSuccessful)
        {
            watch.Stop();
            try
            {
                this.Dispatcher.BeginInvoke((ThreadStart)delegate
                   {
                       if (isSuccessful)
                       {
                           //Read Color value from global resources
                           txtSqlQueryResultPanel.SetResourceReference(ForegroundProperty, "TextBrush");
                           //txtSqlQueryResultPanel.Foreground = Brushes.Blue;
                       }
                       else
                       {

                           txtSqlQueryResultPanel.Foreground = Brushes.Red;

                       }

                       txtSqlQueryResultPanel.AcceptsReturn = true;
                       string costTime = watch.Elapsed.Hours.ToString() + ":" + watch.Elapsed.Minutes.ToString() + ":" + watch.Elapsed.Seconds.ToString()
                           + ":" + watch.Elapsed.Milliseconds.ToString();
                       txtSqlQueryResultPanel.Text = string.Format("{1} ({3}) {2} \r\n{0}", txtSqlQueryResultPanel.Text,
                           DateTime.Now.ToString(), msg, costTime);

                       progressBar.Visibility = System.Windows.Visibility.Collapsed;

                       statusBarCostTime.Text = watch.Elapsed.ToString();
                   }
               );
            }
            catch (Exception se)
            {
                this.Logger().WriteError(se);
                Debug.WriteLine(se.Message);
            }
        }

        private void CallHelp()
        {
            //"NotCompletedFeatureNotifyMsg".GetFromResourece().Notify();
            if (!File.Exists(MyGlobal.GettingStartDoc))
            {
                "File Lost".Notify();
                return;
            }

            Process.Start(MyGlobal.GettingStartDoc);

        }

        private void Image_PreviewMouseLeftButtonDown_Pinned(object sender, MouseButtonEventArgs e)
        {
            Utility.TreeViewHelper.DisExpandAll(mainTreeView);
        }

        private void Image_PreviewMouseLeftButtonDown_UnPinned(object sender, MouseButtonEventArgs e)
        {
            Utility.TreeViewHelper.ExpandAll(mainTreeView);
        }


        /// <summary>
        /// New Sql Query Result View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_NewTabItem(object sender, RoutedEventArgs e)
        {
            //TabItem tabItem = new TabItem();
            Wpf.Controls.TabItem tabItem = new Wpf.Controls.TabItem();
            tabItem.Header = string.Format("{0} {1} ",
                "NewTabItemHeader".GetFromResourece(), (tabControl_ForSqlResult.Items.Count + 1).ToString());

            PagedDataGrid tempG = new PagedDataGrid();

            tempG.MouseDoubleClick += new MouseButtonEventHandler(DataGrid_MouseDoubleClick);
            tabItem.Content = tempG;
            tabControl_ForSqlResult.Items.Add(tabItem);
            tabControl_ForSqlResult.SelectedItem = tabItem;
        }

        public void ProcessDataGridExceptionEvent(Exception ee)
        {

        }

        /// <summary>
        /// Process doublic click event on datagrid control 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            //if (CurrentPagedDataGrid.dataGrid.SelectedItem != null)
            //{
            //    DataRowView row = CurrentPagedDataGrid.dataGrid.SelectedItem as DataRowView;
            //    if (row != null)
            //    {
            //        DetailDataView detailView = new DetailDataView();
            //        detailView.TargetObject = row.Row.ItemArray;
            //        detailView.ShowDialog();
            //    }
            //}

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete("not used")]
        private void MenuItem_Click_CloseTabItem(object sender, RoutedEventArgs e)
        {
            if (tabControl_ForSqlResult.Items.Count <= 1)
            {
                "CannotCloseTabItemNotifyMsg".GetFromResourece().Notify();
                return;
            }

            TabItem tabItem = tabControl_ForSqlResult.SelectedItem as TabItem;
            if (tabItem != null)
            {
                tabControl_ForSqlResult.Items.Remove(tabItem);
            }
        }


        /// <summary>
        /// 注意这里的代码要和XAML创建一致 。因为有对象解析UI的
        /// Create Sql Query Panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_NewQueryItem(object sender, RoutedEventArgs e)
        {

            //TabItem tabItem = new TabItem();
            Wpf.Controls.TabItem tabItem = new Wpf.Controls.TabItem();

            tabItem.Header = string.Format("{0} {1} ", "NewSqlCommandTabHeader".GetFromResourece()
                , (tabControl_ForSqlQuery.Items.Count + 1).ToString());

            DockPanel d = new DockPanel();
            Button b = new Button();
            b.Width = 1;
            b.Height = 1;
            d.Children.Add(b);

            ISqlQueryTextor MyNewEditor = CreateNewSqlQueryTextor();
            //Set the font name and size 
            //But currently the CoreEditor looks like didn't works fine with this property
            //Need fix it if necessary later.
            MyNewEditor.Font = new System.Drawing.Font("arial", Properties.Settings.Default.SqlEditorFontSize);

            d.Children.Add((UIElement)MyNewEditor);
            tabItem.Content = d;
            tabControl_ForSqlQuery.Items.Add(tabItem);
            tabControl_ForSqlQuery.SelectedItem = tabItem;
        }

        [Obsolete("not used")]
        private void MenuItem_Click_CloseQueryItem(object sender, RoutedEventArgs e)
        {
            if (tabControl_ForSqlQuery.Items.Count <= 1)
            {
                "CannotCloseTabItemNotifyMsg".GetFromResourece().Notify();
                return;
            }

            TabItem tabItem = tabControl_ForSqlQuery.SelectedItem as TabItem;
            if (tabItem != null)
            {
                tabControl_ForSqlQuery.Items.Remove(tabItem);
            }
        }


        #region Ribbion RoutedCommand SqlServer


        private void RibbonCommand_Executed_OpenOnlineSqlServerForumn(object sender, ExecutedRoutedEventArgs e)
        {
            if ("OnlineRequestMsg_ForSqlServer".GetFromResourece().Confirm())
            {
                Process.Start("http://forums.microsoft.com/MSDN/default.aspx?siteid=1&ForumGroupID=19");
            }
        }

        private void RibbonCommand_Executed_AboutUs(object sender, ExecutedRoutedEventArgs e)
        {
            new About().ShowMyDialog(this);
        }


        private void RibbonCommand_CanExecute_OpenScirpt(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RibbonCommand_Executed_OpenScirpt(object sender, ExecutedRoutedEventArgs e)
        {
            using (wf.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog())
            {
                of.Filter = "Sql Script(*.sql)|*.sql|Text File(*.txt)|*.txt|All Files(*.*)|*.*";
                if (of.ShowDialog() == wf.DialogResult.OK)
                {
                    try
                    {
                        string sqlConntent = File.ReadAllText(of.FileName);

                        #region Adjust the format type
                        //If larger than this value ,then switch the text editor to simplest mode
                        if (sqlConntent.Length > Properties.Settings.Default.MaxSqlScriptContextLengthForSwitchTextEditor)
                        {
                            SetCurrentTextEditor(this.CreateNewSqlQueryTextor(true));
                            //This is not enough
                            //CurrentSqlQueryEditor.IsAllowFormatSqlContent = false;
                        }
                        else
                        {
                            SetCurrentTextEditor(this.CreateNewSqlQueryTextor());
                            //This is not enough
                            //CurrentSqlQueryEditor.IsAllowFormatSqlContent = true;
                        }
                        #endregion

                        CurrentSqlQueryEditor.Text = sqlConntent;
                    }
                    catch (Exception ee)
                    {
                        this.Logger().WriteError(ee);
                        ee.HandleMyException();
                    }
                }
            }
        }

        private void SetCurrentTextEditor(ISqlQueryTextor iSqlQueryTextor)
        {

            if (tabControl_ForSqlQuery.SelectedItem == null)
            {
                if (tabControl_ForSqlQuery.Items.Count > 0)
                {
                    tabControl_ForSqlQuery.SelectedIndex = 0;
                }
            }

            //TabItem tabItem = tabControl_ForSqlQuery.SelectedItem as TabItem;
            Wpf.Controls.TabItem tabItem = tabControl_ForSqlQuery.SelectedItem as Wpf.Controls.TabItem;
            if (tabItem != null)
            {
                DockPanel container = tabItem.Content as DockPanel;
                if (container != null)
                {
                    container.Children.RemoveAt(1);
                    container.Children.Add((UIElement)iSqlQueryTextor);
                }
            }
        }

        private void RibbonCommand_CanExecute_SaveScript(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentSqlQueryEditor.Text))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void RibbonCommand_Executed_SaveScript(object sender, ExecutedRoutedEventArgs e)
        {
            using (wf.SaveFileDialog of = new System.Windows.Forms.SaveFileDialog())
            {
                of.Filter = "Sql Script(*.sql)|*.sql|Text File(*.txt)|*.txt|All Files(*.*)|*.*";
                if (of.ShowDialog() == wf.DialogResult.OK)
                {
                    try
                    {
                        string sqlConntent = CurrentSqlQueryEditor.Text;
                        FileStream fs = File.Create(of.FileName);
                        fs.Flush();
                        fs.Close();
                        fs.Dispose();

                        using (StreamWriter w = File.AppendText(of.FileName))
                        {
                            w.Write(CurrentSqlQueryEditor.Text);
                            w.Flush();
                            w.Close();
                        }

                    }
                    catch (Exception ee)
                    {

                        ee.HandleMyException();
                        this.Logger().WriteError(ee);
                    }
                }
            }
        }

        private void RibbonCommand_CanExecute_SaveResult(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentPagedDataGrid == null)
            {
                e.CanExecute = false;
                return;
            }

            if (CurrentPagedDataGrid.MyDataSource != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void RibbonCommand_Executed_SaveResult(object sender, ExecutedRoutedEventArgs e)
        {
            //DataTable dt = CurrentDataGrid.DataContext as DataTable;
            DataTable dt = CurrentPagedDataGrid.MyDataSource as DataTable;
            if (dt != null)
            {
                Microsoft.Win32.SaveFileDialog sf = new Microsoft.Win32.SaveFileDialog();
                sf.Filter = "Xml file (*.xml)|*.xml|All Files(*.*)|*.*";
                if ((bool)sf.ShowDialog() == true)
                {
                    dt.WriteXml(sf.FileName);
                    "Save ok".Notify();
                }

            }
        }

        private void RibbonCommand_Executed_Exit(object sender, ExecutedRoutedEventArgs e)
        {
            GlobalDefine.MyGlobal.ExitApplication();
        }

        private void RibbonCommand_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            CloseCurrentIDE();
        }

        private void RibbonCommand_Executed_GetServerInfo(object sender, ExecutedRoutedEventArgs e)
        {
            "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
        }

        private void RibbonCommand_Executed_SyncFromSSCEToSS(object sender, ExecutedRoutedEventArgs e)
        {
            new SyncDataInSqlServer().ShowMyDialog(this);
        }
        private void RibbonCommand_CanExecute_ExeSqlCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentSqlQueryEditor == null) || (CurrentSqlQueryEditor.Text.IsEmpty()))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void RibbonCommand_Executed_ExeSqlCommand(object sender, ExecutedRoutedEventArgs e)
        {
            butQueryClick(null, null);
        }

        #endregion

        #region Ribbion RoutedCommand SSCE

        private void RibbonCommand_Executed_OpenOnlineCeForumn(object sender, ExecutedRoutedEventArgs e)
        {
            if ("OnlineRequestMsg_SSCE".GetFromResourece().Confirm())
            {
                Process.Start("http://social.msdn.microsoft.com/forums/en-US/sqlce/threads/");
            }
        }

        private void RibbonCommand_Executed_redo(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        private void RibbonCommand_Executed_undo(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        private void RibbonCommand_GetDataBaseInfomation(object sender, ExecutedRoutedEventArgs e)
        {
            DbConnection dbConn = App.MainEngineer.GetConnection();
            if (dbConn != null)
            {
                SqlCeConnection curConn = dbConn as SqlCeConnection;
                if (curConn != null)
                {
                    List<KeyValuePair<string, string>> myDbInfo = curConn.GetDatabaseInfo();
                    new ShowSqlCeDbInfo(myDbInfo).ShowMyDialog(this);
                }
            }
        }

        private void RibbonCommand_Executed_GenerateColumnSchema(object sender, ExecutedRoutedEventArgs e)
        {
            GenerateDbSchema gb = new GenerateDbSchema();
            gb.Owner = this;
            gb.ShowSchemaDataInfo += (My, My1) =>
            {
                //TabItem tabItem = new TabItem();
                Wpf.Controls.TabItem tabItem = new Wpf.Controls.TabItem();

                tabItem.Header = "SchemaResultInfoHeader".GetFromResourece();

                PagedDataGrid tempG = new PagedDataGrid();
                //Binding b = new Binding();
                //tempG.SetBinding(System.Windows.Controls.DataGrid.ItemsSourceProperty, b);
                tempG.PreviewMouseDoubleClick += new MouseButtonEventHandler(DataGrid_MouseDoubleClick);

                tabItem.Content = tempG;
                tabControl_ForSqlResult.Items.Add(tabItem);
                tabControl_ForSqlResult.SelectedItem = tabItem;

                SetResultInUI((DataTable)My);
                gb.Close();

            };

            gb.ShowDialog();
        }


        private void RibbonCommand_Executed_ImportFromCSV(object sender, ExecutedRoutedEventArgs e)
        {
            new Csv2Sdf().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_GenerateSelfBussinessEntity(object sender, ExecutedRoutedEventArgs e)
        {
            new GenerateBusinessEntity().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_DownloadDbFile(object sender, ExecutedRoutedEventArgs e)
        {
            new DownloadSdf().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_HelpDoc(object sender, ExecutedRoutedEventArgs e)
        {
            CallHelp();
            //new Help().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_ResetDb(object sender, ExecutedRoutedEventArgs e)
        {
            new ResetData().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_Option(object sender, ExecutedRoutedEventArgs e)
        {
            new Options().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_GenerateLinqMap(object sender, ExecutedRoutedEventArgs e)
        {
            new GenerateLinqContext().ShowMyDialog(this);
        }


        private void RibbonCommand_Executed_ImportFromMySql(object sender, ExecutedRoutedEventArgs e)
        {
            MySql2SDF dialog = new MySql2SDF();
            dialog.Owner = this;
            if ((bool)dialog.ShowDialog())
            {
                //Refresh table list
                LoadAllSchemaInfo();
            }
        }

        private void RibbonCommand_Executed_ImportFromExcel(object sender, ExecutedRoutedEventArgs e)
        {
            Excel2SDF dialog = new Excel2SDF();
            dialog.Owner = this;
            if ((bool)dialog.ShowDialog())
            {
                //Refresh table list
                LoadAllSchemaInfo();
            }
        }

        private void RibbonCommand_Executed_ImportFromAccess(object sender, ExecutedRoutedEventArgs e)
        {
            new Access2SDF().ShowMyDialog(this);
        }

        private void RibbonCommand_Executed_ImportFromSqlServer(object sender, ExecutedRoutedEventArgs e)
        {
            new NewSqlServer2SSCE().ShowMyDialog(this);
        }

        private void RibbonCommand_CanExecute_ForUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CommandCurrency <= 0)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void RibbonCommand_CanExecuteForRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CommandCurrency >= CmdHistoryList.Count)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }


        private void RibbonCommand_Executed_OpenOnlineMySqlForumn(object sender, ExecutedRoutedEventArgs e)
        {
            if ("OnlineRequestMsg_MySql".GetFromResourece().Confirm())
            {
                Process.Start("http://www.mysql.com");
            }
        }

        private void RibbonCommand_Executed_OpenOnlineSqliteForumn(object sender, ExecutedRoutedEventArgs e)
        {
            if ("OnlineRequestMsg_Sqlite".GetFromResourece().Confirm())
            {
                Process.Start("http://www.sqlite.org");
            }
        }

        #endregion

        void OnCloseApplication(object sender, ExecutedRoutedEventArgs e)
        {
            CloseCurrentIDE();
        }

        private void CloseCurrentIDE()
        {
            if (null != App.MainEngineer)
            {
                switch (App.MainEngineer.HostedType)
                {
                    case CoreEA.CoreE.UsedDatabaseType.Effiproz:
                        if (!"NotifyMsgBeforeExit".GetFromResourece().Confirm())
                        {
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
            //Here set to true mean not close via whole application exit button
            isGoingToCloseCurrentIDE = true;
            App.Current.MainWindow = App.MainEntry;
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonCommand_Executed_ChangeSSCEPwd(object sender, ExecutedRoutedEventArgs e)
        {

            ChangeSdfPwd ch = new ChangeSdfPwd(App.MainEngineer.CurDatabase);

            ch.ShowDialog();
        }

        private void RibbonCommand_Executed_ConverExcel2Access(object sender, ExecutedRoutedEventArgs e)
        {
            BatchConvertXlsToAccess ss = new BatchConvertXlsToAccess(BatchConvertXlsToAccess.ConvertMode.SingleFile);
            ss.ShowDialog();
        }

        /// <summary>
        /// can
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonCommand_CanExecut_ExportTableSchema(object sender, CanExecuteRoutedEventArgs e)
        {
            if (mainTreeView.SelectedItem != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Export simple schema 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonCommand_Executed_ExportTableSchema(object sender, ExecutedRoutedEventArgs e)
        {

            if (CurrentTreeArgs != null)
            {
                if (!string.IsNullOrEmpty(CurrentTreeArgs.TableName))
                {
                    BaseTableSchema st = App.MainEngineer.GetTableSchemaInfoObject(CurrentTreeArgs.TableName);

                    string cmdStr = App.MainEngineer.GetCreateTableString(st);

                    CurrentSqlQueryEditor.Text = cmdStr;
                }
            }
        }


        private void RibbonCommand_CanExecut_DocumentingDb(object sender, CanExecuteRoutedEventArgs e)
        {
            if (mainTreeView.SelectedItem != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Documenting the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonCommand_Executed_DocumentingDb(object sender, ExecutedRoutedEventArgs e)
        {

            if (CurrentTreeArgs != null)
            {
                if (!string.IsNullOrEmpty(CurrentTreeArgs.TableName))
                {

                }
            }
        }

        private void tabControl_ForSqlQuery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }

            //TabItem item = e.AddedItems[0] as TabItem;
            Wpf.Controls.TabItem item = e.AddedItems[0] as Wpf.Controls.TabItem;
            if (null == item)
            {
                return;
            }

            FrameworkElement element = item.Content as FrameworkElement;
            if (null == element)
            {
                return;
            }

            element.BeginAnimation(FrameworkElement.OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.Parse("00:00:00.8"))));

        }

        private void tabControl_ForSqlResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }

            //TabItem item = e.AddedItems[0] as TabItem;
            Wpf.Controls.TabItem item = e.AddedItems[0] as Wpf.Controls.TabItem;
            if (null == item)
            {
                return;
            }

            FrameworkElement element = item.Content as FrameworkElement;
            if (null == element)
            {
                return;
            }

            element.BeginAnimation(FrameworkElement.OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.Parse("00:00:00.8"))));

        }

        private void tabControl_ForSqlQuery_TabItemAdded(object sender, Wpf.Controls.TabItemEventArgs e)
        {
            Wpf.Controls.TabControl tabControl = sender as Wpf.Controls.TabControl;
            Wpf.Controls.TabItem tabItem = tabControl.SelectedItem as Wpf.Controls.TabItem;

            tabItem.Header = string.Format("{0} {1} ", "NewSqlCommandTabHeader".GetFromResourece()
                , (tabControl_ForSqlQuery.Items.Count).ToString());

            DockPanel d = new DockPanel();
            Button b = new Button();
            b.Width = 1;
            b.Height = 1;
            d.Children.Add(b);

            ISqlQueryTextor MyNewEditor = CreateNewSqlQueryTextor();

            d.Children.Add((UIElement)MyNewEditor);
            tabItem.Content = d;

        }

        /// <summary>
        /// Mulit Type Inherit or Implement the ISqlQueryTextor interface.
        /// </summary>
        /// <param name="useSimplestControl">Default parameter :
        /// True mean use simeplestTextEditor Control when some performance issues
        /// False mean use in normal way 
        /// Default is fault
        /// </param>
        /// <returns></returns>
        private ISqlQueryTextor CreateNewSqlQueryTextor(bool useSimplestControl = false)
        {
            ISqlQueryTextor ret = null;
            if (useSimplestControl)
            {
                ret = new MySimplestTextEditor();
                ret.MyAdvKeyDown += new System.Windows.Input.KeyEventHandler(myControl_MyAdvKeyDown);
            }
            else
            {
                //Current is true
                if (Properties.Settings.Default.IsUseAdvTextEditor)
                {
                    ret = new MyAdvTextEditor();
                    ret.MyAdvKeyDown += new System.Windows.Input.KeyEventHandler(myControl_MyAdvKeyDown);
                }
                else
                {
                    ret = new MyTextEditor();
                    ret.MyKeyDown += new System.Windows.Forms.KeyEventHandler(myControl_KeyDown);
                }
            }
            return ret;
        }

        void myControl_MyAdvKeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e.Key);
            Debug.WriteLine(Keyboard.Modifiers);

            if ((e.Key == Key.B) && ((Keyboard.Modifiers & ModifierKeys.Control) != 0))
            {

                Undo();
            }
            else if ((e.Key == Key.N) && ((Keyboard.Modifiers & ModifierKeys.Control) != 0))
            {
                Redo();
            }

            if (String.IsNullOrEmpty(CurrentSqlQueryEditor.Text))
            {
                return;
            }

            //Don't allow enter key know
            //I don't why but this code can't work fine.
            //if ((e.Key == Key.X) && ((Keyboard.Modifiers & ModifierKeys.Alt) != 0))
            //{
            //    butQueryClick(null, null);
            //}

            if ((e.KeyStates == Keyboard.GetKeyStates(Key.X)) && ((Keyboard.Modifiers & ModifierKeys.Alt) != 0))
            {
                Debug.WriteLine("Execute sql query");
                butQueryClick(null, null);
            }

            else if (e.Key == Key.F1)
            {
                CallHelp();
            }

            else if ((e.Key == Key.J) && ((Keyboard.Modifiers & ModifierKeys.Control) != 0))
            {
                DynaControls.GetDbTypeControl dj = new DynaControls.GetDbTypeControl();
                dj.Location = new System.Drawing.Point(200, 200);

                dj.AfterSelectedValue += (myArgs) =>
                {

                };

                dj.ShowDialog();

                //e.Handled = true;
            }
        }

        private void tabControl_ForSqlResult_TabItemAdded(object sender, Wpf.Controls.TabItemEventArgs e)
        {
            Wpf.Controls.TabControl tabControl = sender as Wpf.Controls.TabControl;
            Wpf.Controls.TabItem tabItem = tabControl.SelectedItem as Wpf.Controls.TabItem;

            tabItem.Header = string.Format("{0} {1} ",
            "NewTabItemHeader".GetFromResourece(), (tabControl_ForSqlResult.Items.Count).ToString());

            PagedDataGrid tempG = new PagedDataGrid();
            tempG.MouseDoubleClick += new MouseButtonEventHandler(DataGrid_MouseDoubleClick);
            tabItem.Content = tempG;
        }

        private void tabControl_ForSqlQuery_TabItemClosing(object sender, Wpf.Controls.TabItemCancelEventArgs e)
        {
            if (tabControl_ForSqlQuery.Items.Count <= 1)
            {
                e.Cancel = true;
            }
        }

        private void tabControl_ForSqlResult_TabItemClosing(object sender, Wpf.Controls.TabItemCancelEventArgs e)
        {
            if (tabControl_ForSqlResult.Items.Count <= 1)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Submit changes when use Effiproz db type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonCommand_Executed_SubmitChangesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                App.MainEngineer.SubmitChanges();
                "SubmitChangesOK".GetFromResourece().Show();
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
                this.Logger().WriteError(ee);
            }

        }

        private void RibbonCommand_Executed_OpenOnlineEffiprozHomePageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if ("Do you confirm go to Effiproz database homepage".Confirm())
            {
                Process.Start("http://www.effiproz.com");
            }
        }

        private void RibbonCommand_Executed_ChangeEffiprozPassword(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeEffiprozDbPassword sf = new ChangeEffiprozDbPassword();

            if ((bool)sf.ShowDialog())
            {
                if (App.MainEngineer.ChangePassword(new UserTokenInfo()
              {
                  NewPassword = sf.Result
              })
                    )
                {
                    "TitleChangePasswordSuccessful".GetFromResourece().Show();
                }
                else
                {
                    "TitleChangePasswordFailed".GetFromResourece().Show();
                }
            }

        }


    }
}
