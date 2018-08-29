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
        public static class Oracle
        {
            /// <summary>
            /// No need to install Oracle client
            /// use tns
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port">1521</param>
            /// <param name="dbInstanceName"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string ConnectionStringWithoutClient(string ip, string port, string dbInstanceName, string uid, string pwd)
            {
                return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})) (CONNECT_DATA=(SERVICE_NAME={2})));User ID={3};Password={4};Unicode=True",
                    ip, port, dbInstanceName, uid, pwd);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string OracleXE(string ip, string port, string uid, string pwd)
            {
                return string.Format("Driver=(Oracle in XEClient);dbq={0}:{1}/XE;Uid={2};Pwd={3};", ip, port, uid, pwd);
            }

            /// <summary>
            /// Use Traditional OleDB Type to connect oracle
            /// </summary>
            /// <param name="server"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string StandardOledb_Security_FromMS(string server, string uid, string pwd)
            {
                return string.Format("Provider=msdaora;Data Source={0};User Id={1};Password={2};", server, uid, pwd);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="server"></param>
            /// <returns></returns>
            public static string TrustedConnection_MS(string server)
            {
                return string.Format("Provider=msdaora;Data Source={0};Persist Security Info=False;Integrated Security=Yes;", server);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="server"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string StandardOledb_Security_Oracele(string server, string uid, string pwd)
            {
                return string.Format("Provider=OraOLEDB.Oracle;Data Source={0};User Id={1};Password={2};", server, uid, pwd);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="server"></param>
            /// <returns></returns>
            public static string TrustedConnection_Oracle(string server)
            {
                return string.Format("Provider=OraOLEDB.Oracle;Data Source={0};OSAuthent=1;", server);
            }

        }
    
    }
}
