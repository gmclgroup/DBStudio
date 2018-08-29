using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.ConnSTR
{
    public static partial class DbConnectionString
    {



        /// <summary>
        /// For the sqlite.
        /// Notice ,only support sqlite3.0 or up
        /// </summary>
        public static class Sqlite
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileFullPath"></param>
            /// <returns></returns>
            public static string Standard(string fileFullPath)
            {
                return string.Format("Data Source = {0}; Version = 3;", fileFullPath);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileFullPath"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string StandardWithPwd(string fileFullPath, string pwd)
            {
                return string.Format("Data Source = {0}; Version = 3;Password={1};", fileFullPath, pwd);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileFullPath"></param>
            /// <returns></returns>
            public static string StandardWithUnicode(string fileFullPath)
            {
                return string.Format("Data Source = {0}; Version = 3;UseUTF16Encoding=True;", fileFullPath);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileFullPath"></param>
            /// <returns></returns>
            public static string StandardWithReadOnly(string fileFullPath)
            {
                return string.Format("Data Source = {0}; Version = 3;Read Only = True;", fileFullPath);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileFullPath"></param>
            /// <param name="IsReadOnly"></param>
            /// <param name="isUnicode"></param>
            /// <param name="Pwd"></param>
            /// <returns></returns>
            public static string Standard(string fileFullPath, bool IsReadOnly, bool isUnicode, string Pwd)
            {
                string connStr = string.Format("Data Source = {0}; Version = 3;UseUTF16Encoding={1};Read Only = {2};",
                    fileFullPath, isUnicode, IsReadOnly);

                if (!string.IsNullOrEmpty(Pwd))
                {
                    connStr += string.Format("Password={0};", Pwd);
                }
                return connStr;
            }
        }
    }
}
