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
        public static class DB2
        {
            /// <summary>
            /// Ole db supported by .net 
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="db"></param>
            /// <param name="packCollection"></param>
            /// <param name="schema"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string OleDbConnection(string ipAddress, string db, string packCollection, string schema, string uid, string pwd)
            {
                return string.Format("Provider=DB2OLEDB;Network Transport Library=TCPIP;Network Address={0};Initial Catalog={1};Package Collection={2};Default Schema={3};User ID={4};Password={5};", ipAddress, db, packCollection, uid, pwd);
            }

            /// <summary>
            /// APPC
            /// </summary>
            /// <param name="alias"></param>
            /// <param name="remote"></param>
            /// <param name="db"></param>
            /// <param name="packCollection"></param>
            /// <param name="schema"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string APPC(string alias, string remote, string db, string packCollection, string schema, string uid, string pwd)
            {
                return string.Format("Provider=DB2OLEDB;APPC Local LU Alias={0};APPC Remote LU Alias={1};Initial Catalog={2};Package Collection={3};Default Schema={4};User ID={5};Password={6};", alias, remote, db, packCollection, schema, uid, pwd);
            }

            /// <summary>
            /// IBM's OLE DB Provider (shipped with IBM DB2 UDB v7 or above)
            /// </summary>
            /// <param name="db"></param>
            /// <param name="server"></param>
            /// <param name="port"></param>
            /// <param name="uid"></param>
            /// <param name="pwd"></param>
            /// <returns></returns>
            public static string OleDb_IBM(string db, string server, string port, string uid, string pwd)
            {
                return string.Format("Provider=IBMDADB2;Database={0};Hostname={1};Protocol=TCPIP;Port={2}; Uid={3};Pwd={4};", db, server, port, uid, pwd);
            }
        }

    }
}
