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
using DBStudio.CommandFactory;
using XLCS.Common;
using ETL;
using System.Windows;
using System.Diagnostics;

namespace DBStudio.GlobalDefine
{
    internal partial class MyGlobal
    {
        internal static void ExitApplication()
        {
            if ("ExitAppConfirmMsg".GetFromResourece().Confirm())
            {
                //foreach (Window item in App.Current.Windows)
                //{
                //    item.Close();
                //}

               App.Current.Shutdown();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <returns></returns>
        internal static ICmd ParseSqlCmdString(string cmdStr, CoreEA.ICoreEAHander dbe)
         {
            ICmd curCmd = null;

            if (string.IsNullOrEmpty(cmdStr))
            {
                ProcessException.DisplayErrors(new Exception("Empty Command"));
                return null;
            }
            try
            {
                cmdStr = cmdStr.TrimStart();
                cmdStr = cmdStr.TrimEnd();

                //Can't to lower , otherwise the insert data will never 大写
                //cmdText = cmdText.ToLower();
                string tempCmdText = cmdStr.ToLower();
                if (tempCmdText.StartsWith("select"))
                {
                    curCmd = new SelectCmd(dbe);
                }
                else if (tempCmdText.StartsWith("insert"))
                {
                    curCmd = new InsertCmd(dbe);
                }
                else if (tempCmdText.StartsWith("update"))
                {
                    curCmd = new UpdateCmd(dbe);
                }
                else if (tempCmdText.StartsWith("delete"))
                {
                    curCmd = new DeleteCmd(dbe);
                }
                else if (tempCmdText.StartsWith("alter") || (tempCmdText.StartsWith("drop")))
                {
                    curCmd = new SpecialCmd(dbe);
                }
                else if (tempCmdText.StartsWith("create"))
                {
                    curCmd = new CreateCmd(dbe);

                }
                else if (tempCmdText.StartsWith("sp_rename"))
                {
                    curCmd = new SpecialCmd(dbe);
                }
                else
                {
                    curCmd = null;
                    //throw new Exception("ErrorMsg_CannotParseSqlCmd".GetFromResourece());
                }

            }
            catch (Exception ee)
            {
                ProcessException.DisplayErrors(ee);
            }

            return curCmd;
        }

        internal static string ConfigFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SqlCeViewerConfig.config";
            }
        }

        internal static string UserDocsFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
            }
        }

    }
}
