using System;
using System.Collections.Generic;
using System.Text;
using CoreEA.Args;

namespace CoreEA.ConnSTR
{
    /// <summary>
    /// Database connection string library
    /// </summary>
    public static partial class DbConnectionString
    {

        /// <summary>
        /// 
        /// </summary>
        public static class VistaDB
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dbFullPath">such as D:\folder\myVistaDatabaseFile.vdb3</param>
            /// <returns></returns>
            public static string VistaDb_Connection(string dbFullPath)
            {
                return string.Format("Data Source={0};Open Mode=ExclusiveReadWrite;", dbFullPath);
            }
        }


    }
}
