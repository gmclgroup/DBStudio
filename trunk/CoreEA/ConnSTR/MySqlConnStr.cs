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
        public static class MySql
        {
            /// <summary>
            /// 
            /// </summary>
            public static int DefaultPort
            {
                get
                {
                    return 3306;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public static string DefaultDatabaseName
            {
                get
                {
                    return "mysql";
                }
            }

            /// <summary>
            /// Currnet this var was not used ,
            /// Based on .net framework 4, we used optional parameters instead.
            /// </summary>
            private static int DefaultConnectionTimeout
            {
                get
                {
                    return 25;
                }

            }

            /// <summary>
            /// MySql server 
            /// </summary>
            /// <param name="i_server"></param>
            /// <param name="i_username"></param>
            /// <param name="i_pwd"></param>
            /// <param name="connTimeout"></param>
            /// <param name="defaultCommandTimeout"></param>
            /// <returns></returns>
            public static string GetMySqlConnectionString(string i_server, string i_username, string i_pwd, int connTimeout = 25, int defaultCommandTimeout = 50)
            {
                return GetMySqlConnectionString(i_server, i_username, i_pwd, DefaultPort, DefaultDatabaseName);
            }

            /// <summary>
            /// MySql server with port
            /// </summary>
            /// <param name="i_server"></param>
            /// <param name="i_username"></param>
            /// <param name="i_pwd"></param>
            /// <param name="i_port"></param>
            /// <param name="connTimeout"></param>
            /// <param name="defaultCommandTimeout"></param>
            /// <returns></returns>
            public static string GetMySqlConnectionString(string i_server, string i_username, string i_pwd, int i_port, int connTimeout = 25, int defaultCommandTimeout = 50)
            {
                return GetMySqlConnectionString(i_server, i_username, i_pwd, i_port, DefaultDatabaseName);
            }

            /// <summary>
            /// MySql server with dbname and port
            /// </summary>
            /// <param name="i_server"></param>
            /// <param name="i_username"></param>
            /// <param name="i_pwd"></param>
            /// <param name="i_port"></param>
            /// <param name="dbname"></param>
            /// <param name="connTimeout"></param>
            /// <param name="defaultCommandTimeout"></param>
            /// <param name="isPolliing"></param>
            /// <returns></returns>
            public static string GetMySqlConnectionString(string i_server, string i_username, string i_pwd, int i_port, string dbname, int connTimeout = 25, int defaultCommandTimeout = 50,bool isPolliing=false)
            {
                #region auto adjust to Default value
                if (string.IsNullOrEmpty(dbname))
                {
                    dbname = DefaultDatabaseName;
                }

                if (i_port <= 0)
                {
                    i_port = DefaultPort;
                }
                #endregion
                if (connTimeout == 0)
                {
                    connTimeout = 25;
                }

                return String.Format("server={0};user id={1}; password={2}; database={4}; pooling={7};port={3};Connection Timeout={5};default command timeout={6}",
                    i_server, i_username, i_pwd, i_port, dbname, connTimeout, defaultCommandTimeout,isPolliing);
            }
        }
    }
}
