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
using ETL;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using MPL.MyControls;
using log4net;
using DBStudio.Utility;
using CoreEA.SchemaInfo;

namespace DBStudio.GlobalDefine
{
    public static class GlobalExtension
    {



        /// <summary>
        /// Detect whether the history list contain server info or db file file 
        /// If yes,it will not add it to the history list again 
        /// This method will depend on HistoryObject object, so can't used external
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="subItemValue"></param>
        /// <returns></returns>
        public static bool IsContainSubValue<T>(this List<T> source, string subItemValue)
        {
            bool ret = false;

            foreach (object item in source)
            {
                if (item is MySqlObjects)
                {
                    if (((MySqlObjects)item).ServerAddress == subItemValue)
                    {
                        ret = true;
                        break;
                    }
                }

                if (item is SqlServerObjects)
                {
                    if (((SqlServerObjects)item).ServerAddress == subItemValue)
                    {
                        ret = true;
                        break;
                    }
                }

                if (item is SSCEObjects)
                {
                    if (((SSCEObjects)item).DbFileFullPath == subItemValue)
                    {
                        ret = true;
                        break;
                    }
                }


                if (item is OleDbObjects)
                {
                    if (((OleDbObjects)item).DbFileFullPath == subItemValue)
                    {
                        ret = true;
                        break;
                    }
                }

                if (item is OracleObjects)
                {
                    if (((OracleObjects)item).HostName == subItemValue)
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }

        public static void WriteError(this ILog log, Exception ee)
        {
            log.Error(ee.Message+"\r\n"+ee.StackTrace);
        }

        /// <summary>
        /// Log4net helper method
        /// </summary>
        /// <param name="currentView"></param>
        /// <returns></returns>
        public static ILog Logger(this object currentView)
        {
            return LogManager.GetLogger(currentView.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mycontrol"></param>
        /// <returns></returns>
        internal static bool HasText(this TextBox mycontrol)
        {
            return !string.IsNullOrEmpty(mycontrol.Text);
        }

        /// <summary>
        /// Get String From Resources.
        /// </summary>
        /// <param name="stringNameInResFile"></param>
        /// <returns></returns>
        internal static string GetFromResourece(this string stringNameInResFile)
        {
            Debug.Assert(!string.IsNullOrEmpty(stringNameInResFile));

            if (App.MainResources == null)
            {
                return "";
            }

            return App.MainResources.GetString(stringNameInResFile);
        }

        /// <summary>
        /// Display the result info in the sql query result panel 
        /// like sqlserver management studio
        /// </summary>
        /// <param name="t"></param>
        //public static void NotifyInfoInResultPanel(this string t)
        //{

        //}

        public static void Notify(this Exception ex)
        {
            if (Properties.Settings.Default.UseSpeakInsteadOfMsgBox)
            {
                ex.Message.Say();
            }
            else
            {
                Debug.WriteLine("----------------***************************************-----------------------");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("----------------***************************************-----------------------");

                MyMsgBox msg = null;
                if (ex.InnerException != null)
                {
                    msg = new MyMsgBox("Information", ex.Message, ex.InnerException.Message, MsgMode.Info);
                }
                else
                {
                    msg = new MyMsgBox("Information", ex.Message, "", MsgMode.Info);
                }
                msg.ShowDialog();
            }
        }

        public static void Notify(this string t)
        {
            if (Properties.Settings.Default.UseSpeakInsteadOfMsgBox)
            {
                t.Say();
            }
            else
            {
                Debug.WriteLine("----------------***************************************-----------------------");
                Debug.WriteLine(t);
                Debug.WriteLine("----------------***************************************-----------------------");

                MyMsgBox msg = new MyMsgBox("Information", t, t, MsgMode.Info);
                msg.ShowDialog();
            }
        }

        internal static void Warning(this string t)
        {
            MyMsgBox msg = new MyMsgBox("Warning", t, t, MsgMode.Warning);
            msg.ShowDialog();
        }

        internal static void ShowMyDialog(this BaseUI.BaseFadeDialog myDialog, Window hostedWin)
        {
            myDialog.Owner = hostedWin;
            myDialog.ShowDialog();
        }
    }
}
