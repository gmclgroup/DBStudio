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

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using DBStudio.CommonMethod;
//using System.Diagnostics;
//using System.Data.Common;
//using System.Data;
//using XLCS.Common;
//using System.ComponentModel;
//using ETL;
//using Microsoft.Windows.Controls;

//namespace DBStudio.UI
//{
//    public partial class CEMain 
//    {
//        public enum CurCommandType
//        {
//            SpecialCommand,
//            Query,
//            Insert,
//            Update,
//            Delete,
//            SystemAction,
//            CreateTable,
//            Unknown,
//        }

//        public CurCommandType _curCmdType { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public DataGrid CurrentDataGrid
//        {
//            get
//            {

//                TabItem tabItem = tabControl.SelectedItem as TabItem;
//                if (tabItem != null)
//                {
//                    DataGrid dgGrid = tabItem.Content as DataGrid;
//                    if (dgGrid != null)
//                    {
//                        return dgGrid;
//                    }
//                }
//                return null;
//            }
//        }
     

//        private void Do_CreateTableCmd(DbCommand cmd)
//        {
//            try
//            {
//                int resultCount = App.MainEngineer.DoExecuteNonQuery(cmd);
//                if (resultCount == -1)
//                {
//                    MessageBox.Show(String.Format("Table {0} has created ", 1));
//                    RefreshCurrentDbStatus();
//                }
//                else
//                {
//                    MessageBox.Show(String.Format("Table has failure"));
//                }
//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }
//        }

//        private void Do_InsertCmd(DbCommand cmd)
//        {
//            try
//            {
//                int resultCount = App.MainEngineer.DoExecuteNonQuery(cmd);

//            MessageBox.Show(String.Format("{0} has changed ", resultCount));
//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="cmd"></param>
//        private void Do_SpecialCmd(DbCommand cmd)
//        {
//            try
//            {
//                object result = App.MainEngineer.DoExecuteNonQuery(cmd);
//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }
//            RefreshCurrentDbStatus();
//        }


//        private void Do_SysCmd(DbCommand cmd)
//        {
//            try
//            {
//                object result = App.MainEngineer.DoExecuteScalar(cmd);

//                MessageBox.Show(String.Format("{0} has changed ", result));

//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }
//            RefreshCurrentDbStatus();
//        }

//        private void RefreshCurrentDbStatus()
//        {

//            if (App.MainEngineer.GetTableListInDatabase()!=null)
//            {
//                Main_Loaded(null, null);
//            }
//            else
//            {
//                throw new Exception("System Query Error");
//            }
//        }

//        private void Do_DeleteCmd(DbCommand cmd)
//        {
//            try
//            {
//                object result = App.MainEngineer.DoExecuteScalar(cmd);
//                MessageBox.Show(String.Format("{0} has changed ", result));

//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }

//        }

//        private void Do_UpdateCmd(DbCommand cmd)
//        {
//            try
//            {
//                object result = App.MainEngineer.DoExecuteScalar(cmd);
//                MessageBox.Show(String.Format("{0} has changed ", result));

//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }

//        }

//        private void Do_SelectCmd(DbCommand cmd)
//        {
//            try
//            {
//                StartProcessingBar();
//                DataSet ds = App.MainEngineer.ExecuteDataList(cmd);

//                X_ShowSqlExecutionResult(ds);
                
//            }
//            catch (Exception ee)
//            {
//                ProcessException.DisplayErrors(ee);
//            }
//            finally
//            {

//                StopProcessingBar();
//            }
//        }


//        private void X_ShowSqlExecutionResult(DataSet ds)
//        {
//            CurrentDataGrid.DataContext = ds.Tables[0];
//        }
//        /// <summary>
//        /// parse sql command ,and type
//        /// </summary>
//        /// <param name="cmdText"></param>
//        /// <returns></returns>
//        private string ParseCmd(string cmdText)
//        {
            
//            string result = string.Empty;
//            if (string.IsNullOrEmpty(cmdText))
//            {
//                ProcessException.DisplayErrors(new Exception("Empty Command"));
//            }
//            try
//            {
//                cmdText = cmdText.TrimStart();

//                cmdText = cmdText.TrimEnd();

//                //Can't to lower , otherwise the insert data will never 大写
//                //cmdText = cmdText.ToLower();
//                string tempCmdText = cmdText.ToLower();
//                if (tempCmdText.StartsWith("select"))
//                {
//                    _curCmdType = CurCommandType.Query;
//                }
//                else if (tempCmdText.StartsWith("insert"))
//                {
//                    _curCmdType = CurCommandType.Insert;
//                }
//                else if (tempCmdText.StartsWith("update"))
//                {
//                    _curCmdType = CurCommandType.Update;
//                }
//                else if (tempCmdText.StartsWith("delete"))
//                {
//                    _curCmdType = CurCommandType.Delete;
//                }
//                else if (tempCmdText.StartsWith("alter") || (tempCmdText.StartsWith("drop")))
//                {
//                    _curCmdType = CurCommandType.SystemAction;
//                }
//                else if (tempCmdText.StartsWith("create"))
//                {
//                    _curCmdType = CurCommandType.CreateTable;
                
//                }
//                else if (tempCmdText.StartsWith("sp_rename"))
//                {
//                    _curCmdType = CurCommandType.SpecialCommand;
//                }
//                else
//                {
//                    "Invalid Type, Please check your sql command text".Show();
//                    _curCmdType = CurCommandType.Unknown;
//                }

//                result = cmdText;
//            }
//            catch (Exception ee)
//            {
//                ee._处理我的异常();
//            }

//            return result;
//        }


//    }
//}
