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
        public static class Sqlserver
        {
            /// <summary>
            /// Standard security
            /// </summary>
            /// <returns></returns>
            public static string Standard(string server, string db, string uid, string pwd)
            {
                return string.Format("Data Source = {0}; Initial Catalog = {1}; User Id = {2}; Password = {3}", server, db, uid, pwd);
            }

            /// <summary>
            ///  Get Sql Server 2005 Express CS
            /// </summary>
            /// <param name="server"></param>
            /// <param name="db"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <param name="isTrust"></param>
            /// <returns></returns>
            public static string Standard_WithTrustOrNot(string server, string db, string uid, string pwd, bool isTrust)
            {
                string trustStr = isTrust ? "TRUE" : "FALSE";
                return String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Trusted_Connection={4}",
                    server, db, uid, pwd, trustStr);

            }

            /// <summary>
            /// Trusted Connection
            /// This is used when Intergration with windows authentication
            /// 
            /// This is also same as
            /// <code>Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;</code>
            /// </summary>
            /// <returns></returns>
            public static string AttachFile(string db, string dbFileFullPath)
            {
                return string.Format("Data Source = {0}; Initial Catalog = {1}; Integrated Security = SSPI", db, dbFileFullPath);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="serverName"></param>
            /// <param name="dbFullPath"></param>
            /// <returns></returns>
            public static string AttachFileEx(string serverName,string dbFullPath)
            {
                return string.Format("Server={0};AttachDbFilename={1};Database=DefaultDBName; Trusted_Connection=Yes;",serverName,
                    dbFullPath);
            }

            /// <summary>
            /// Connect via an IP address
            /// </summary>
            /// <param name="ip">like 192.143.33.34</param>
            /// <param name="port">1433 is the default</param>
            /// <param name="db"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string ConnectionWithIP(string ip, string port, string db, string uid, string pwd)
            {
                return string.Format("Data Source={0},{1};Network Library=DBMSSOCN;Initial Catalog={2};User ID={3};Password={4};", ip, port, db, uid, pwd);
            }

            /// <summary>
            /// Enabling MARS (multiple active result sets)
            /// Use ADO.NET 2.0 for MARS functionality. MARS is not supported in ADO.NET 1.0 nor ADO.NET 1.1.
            /// </summary>
            /// <param name="server"></param>
            /// <param name="db"></param>
            /// <returns></returns>
            public static string Connection_Mars(string server, string db)
            {
                return string.Format("Server ={0}; Database = {1}; Trusted_Connection = True; MultipleActiveResultSets = true", server, db);
            }

            /// <summary>
            /// Enabling MARS (multiple active result sets)
            /// Use ADO.NET 2.0 for MARS functionality. MARS is not supported in ADO.NET 1.0 nor ADO.NET 1.1.
            /// </summary>
            /// <param name="server"></param>
            /// <param name="db"></param>
            /// <param name="username"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string Connection_Mars_NoTrust(string server, string db,string username,string pwd)
            {
                return string.Format("Server ={0}; Database = {1};User Id = {2}; Password = {3}; MultipleActiveResultSets = true", server, db,username,pwd);
            }

            /// <summary>
            /// If you connect with ADO.NET or the SQL Native Client to a database that is being mirrored,
            /// your application can take advantage of the drivers ability to automatically redirect connections when a database mirroring failover occurs. 
            /// You must specify the initial principal server and database in the connection string and the failover partner server.
            /// </summary>
            /// <param name="dbServer"></param>
            /// <param name="mirrorServer"></param>
            /// <param name="db"></param>
            /// <returns></returns>
            public static string Connection_DbMirror(string dbServer, string mirrorServer, string db)
            {
                return string.Format("Data Source = {0}; Failover Partner = {1}; Initial Catalog = {2}; Integrated Security = True;", dbServer, mirrorServer, db);
            }

            /// <summary>
            /// Using an User Instance on a local SQL Server Express instance
            /// 
            /// The User Instance functionality creates a new SQL Server instance on the fly during connect. 
            /// This works only on a local SQL Server 2005 instance and only when connecting using windows authentication over local named pipes.
            /// The purpose is to be able to create a full rights SQL Server instance to a user with limited administrative rights on the computer.
            /// </summary>
            /// <param name="server"></param>
            /// <param name="dbFileFullPath"></param>
            /// <returns></returns>
            public static string UseUserInstanceOnLocal(string server, string dbFileFullPath)
            {
                return string.Format("Data Source={0};Integrated Security=true;AttachDbFilename={1};User Instance=true;", server, dbFileFullPath);
            }

            /// <summary>
            /// This oledb connection is test passed to the online databases
            /// </summary>
            /// <param name="server"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <param name="db"></param>
            /// <returns></returns>
            public static string GetOledbConnectionString_NoTrust(string server, string uid, string pwd, string db)
            {
                return "Provider=sqloledb;Data Source=" + server + ";Initial Catalog=" + db + ";UID=" + uid + ";Pwd=" + pwd + ";";
            }

            /// <summary>
            /// Sql server
            /// </summary>
            /// <param name="server"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <param name="db"></param>
            /// <returns></returns>
            public static string SqlServerConnectionString(string server, string uid, string pwd, string db)
            {
                return "Server=" + server + ";database=" + db + ";uid=" + uid + ";pwd=" + pwd;
            }


            /// <summary>
            /// data source may be:IP ADDRESS (such as :192.122.122.122:1433,)noticed 1433 it the por (by default)
            /// In fact sqlConnectionWay in not very well,compare with accessConnectionWay
            /// so,It maybe overrided any time if necessary
            /// </summary>
            public static string GetConnectionString(string server, string uid, string pwd, string db)
            {
                return "Data Source=" + server + ";Initial Catalog=" + db + ";Integrated Security=SSPI;UID=" + uid + ";Pwd=" + pwd + ";";
            }


            /// <summary>
            ///  Get Sql Server 2005 Express CS
            /// </summary>
            /// <param name="server"></param>
            /// <param name="db"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <param name="isTrust"></param>
            /// <returns></returns>
            public static string GetSqlServerExpressCS(string server, string db, string uid, string pwd, bool isTrust)
            {
                string trustStr = isTrust ? "TRUE" : "FALSE";
                return String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Trusted_Connection={4}",
                    server, db, uid, pwd, trustStr);

            }


            /// <summary>
            /// Get OleDb Connection To sql server
            /// </summary>
            /// <param name="server"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <param name="db"></param>
            /// <returns></returns>
            public static string GetOledbConnectionString_Trust(string server, string uid, string pwd, string db)
            {
                return "Provider=sqloledb;Data Source=" + server + ";Initial Catalog=" + db + ";Integrated Security=SSPI;UID=" + uid + ";Pwd=" + pwd + ";";
            }

            /// <summary>
            /// Get odbc to sql server
            /// </summary>
            /// <param name="server"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <param name="db"></param>
            /// <returns></returns>
            public static string GetOdbcConnectionString(string server, string uid, string pwd, string db)
            {
                return "DRIVER={SQL Server};Trusted_Connection=yes;SERVER=" + server + ";DATABASE=" + db + ";UID=" + uid + ";Pwd=" + pwd + ";min pool size=1;max pool size=50";
            }



            internal static string GetSqlServer2008_Trust(string server, string dbName)
            {
                return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;",
                    server, dbName);
            }

            internal static string GetSqlServer2008_StandardSecurity(string server, string dbName, string uid, string pwd)
            {
                return string.Format("Data Source = {0}; Initial Catalog = {1}; User Id = {2}; Password = {3};",
                    server, dbName, uid, pwd);
            }
        }
    }
}
