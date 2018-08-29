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

namespace DBStudio.Utility
{
    /// <summary>
    /// This class will save the opened file or connected database server history information
    /// If further database type need to add ,just add another class like olds.
    /// </summary>
    public class HistoryObject
    {
        public List<SqlServerObjects> SqlServerHistory { get; set; }
        public List<SSCEObjects> SSCEHistory { get; set; }
        public List<MySqlObjects> MySqlHistory { get; set; }
        public List<OleDbObjects> OleDbHistory { get; set; }
        public List<OracleObjects> OracleHistory { get; set; }
    }

    public class SSCEObjects
    {
        public string DbFileFullPath { get; set; }
        public DateTime LatestVisitTime { get; set; }

        /// <summary>
        /// This property will be set later in UI 
        /// </summary>
        public bool IsExisted { get; set; }
    }

    public class OleDbObjects
    {
        public string DbFileFullPath { get; set; }
    }

    public class SqlServerObjects
    {
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public int Port { get; set; }
    }

    public class MySqlObjects
    {
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public int Port { get; set; }
    }

    public class OracleObjects
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public int Port { get; set; }
        public string SID { get; set; }

    }
}
