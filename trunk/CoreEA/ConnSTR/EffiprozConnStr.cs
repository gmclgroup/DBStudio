using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEA.LoginInfo;

namespace CoreEA.ConnSTR
{
    public static partial class DbConnectionString
    {
        /// <summary>
        /// 
        /// </summary>
        public static class EffiprozConnStr
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="connType"></param>
            /// <param name="dbname"></param>
            /// <param name="username"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string GetConnectionString(ConnectionType connType, string dbname,string username,string pwd)
            {
                string result = string.Empty;

                result = string.Format("Connection Type={3} ; Initial Catalog={0}; User={1}; Password={2};"
                    , dbname, username, pwd, connType.ToString());

                
                return result;
            }
        }
    }
}
