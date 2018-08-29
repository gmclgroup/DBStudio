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
using DBStudio.GlobalDefine;
using System.Diagnostics;
using CoreEA.Args;
using CoreEA;

namespace DBStudio.DataExchangeCenter
{
    internal static class SourceControlFactory
    {
        /// <summary>
        /// Currently Only support Export to SSCE
        /// 2009-03-23 support Export to CSV
        /// </summary>
        /// <param name="srcDbType"></param>
        /// <param name="IsAsSourceControl">
        /// If true means this control is a souce selection UI
        /// If false means this control is a target selection UI</param>
        /// <returns></returns>
        public static ISrcControl GetProcessingControl(CoreE.UsedDatabaseType srcDbType,bool IsAsSourceControl)
        {
            switch (srcDbType)
            {
                case CoreE.UsedDatabaseType.SqlCE35:
                    if (IsAsSourceControl)
                    {
                        return new SelectSSCEFile();
                    }
                    else
                    {
                        return new SelectTargetDb_SqlCe();
                    }
                case CoreE.UsedDatabaseType.SqlServer:
                    if (IsAsSourceControl)
                    {
                        return new SelectSqlServerSource();
                    }
                    else
                    {
                        return new SelectSqlServerTarget();
                    }
                case CoreE.UsedDatabaseType.MySql:
                    if (IsAsSourceControl)
                    {
                        return new SelectMySqlSource();
                    }
                    else
                    {
                        return new SelectTargetMySql();
                    }
                case CoreE.UsedDatabaseType.OleDb:
                    if (IsAsSourceControl)
                    {
                        return new SelectSourceDbFile_OleDB();
                    }
                    else
                    {
                        //Can't enter here so need code process it .
                        return new SelectSourceDbFile_OleDB(UsingOleDbType.CSV);
                    }
                case CoreE.UsedDatabaseType.Sqlite:
                    if (IsAsSourceControl)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        return new SelectTargetSqliteDB();
                    }
                    return new SelectSqlite3DbFile();
                case CoreE.UsedDatabaseType.Effiproz:
                    return new SelectEffiproz();
                case CoreE.UsedDatabaseType.CSV:
                    if (IsAsSourceControl)
                    {
                        //Can't enter here so need code process it .
                        return null;
                    }
                    else
                    {
                        return new SelectTargetCSVFile();
                    }
                default:
                    Debug.Assert(false, "ErrorMsg_InvalidType".GetFromResourece());
                    return null;

            }
        }
    }
}
