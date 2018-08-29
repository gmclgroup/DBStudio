using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.ConnSTR
{
    public static partial class DbConnectionString
    {


        /// <summary>
        /// 
        /// </summary>
        public static class Access
        {
            /// <summary>
            /// return Acess ODBC string
            /// </summary>
            /// <param name="database"></param>
            /// <param name="senduid"></param>
            /// <param name="sendpwd"></param>
            /// <returns></returns>
            public static string GetOdbcAccess(string database, string senduid, string sendpwd)
            {
                return "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + database + ";Uid=" + senduid + ";Pwd=" + sendpwd + ";";
            }

            /// <summary>
            /// return Acess OLEDB connection string
            /// </summary>
            /// <param name="database"></param>
            /// <param name="senduid"></param>
            /// <param name="sendpwd"></param>
            /// <returns></returns>
            public static string GetOledbAccess(string database, string senduid, string sendpwd)
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + database + ";User Id=" + senduid + ";Password=" + sendpwd + ";";
            }

            /// <summary>
            /// Return Access Connection String with Setting Password
            /// </summary>
            /// <param name="database"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string GetOleDbAccessWithPassword(string database, string pwd)
            {
                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password={1};",
                database, pwd);

            }
        }
    }
}
