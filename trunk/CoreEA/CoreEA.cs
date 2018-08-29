using System;
using System.Data;

using System.Diagnostics;
using CoreEA.Sqlite;
using CoreEA.Firebird;
using CoreEA.CSV;
using CoreEA.Excel;
using CoreEA.Oracle;

namespace CoreEA
{
    /// <summary>
    /// Core Enterprise Action Class Library
    /// </summary>
    public partial class CoreE
    {
        private ICoreEAHander _handler = null;
        /// <summary>
        /// 
        /// </summary>
        public ICoreEAHander X_Handler
        {
            get
            {
                return _handler;
            }
            set { _handler = value; }
        }

        /// <summary>
        /// Database Type which used
        /// This enum is used when create CoreEA ,
        /// It Specfied the return Interface Handler 
        /// It's vital
        /// </summary>
        public enum UsedDatabaseType
        {
            /// <summary>
            /// 
            /// </summary>
            OleDb,
            /// <summary>
            /// 
            /// </summary>
            SqlServer,
            /// <summary>
            /// mysql server 5.0
            /// </summary>
            MySql,
            /// <summary>
            /// Sql server compact edition 3.5
            /// </summary>
            SqlCE35,
            /// <summary>
            /// 
            /// </summary>
            Sqlite,
            /// <summary>
            /// 
            /// </summary>
            Firebird,
            /// <summary>
            /// 
            /// </summary>
            CSV,
            /// <summary>
            /// 
            /// </summary>
            Excel,
            /// <summary>
            /// 
            /// </summary>
            Oracle,
            /// <summary>
            /// 
            /// </summary>
            Effiproz,
        };

        /// <summary>
        /// Constructor 1:
        /// According to the DBTYpe , return a Core
        /// Use the default db connection method 'oledb'
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public CoreE(UsedDatabaseType dbType)
        {
            switch (dbType)
            {
                case UsedDatabaseType.OleDb:
                    _handler = new OledbRobot();
                    break;
                case UsedDatabaseType.SqlServer:
                    _handler = new SqlServerRobot();
                    break;
                case UsedDatabaseType.MySql:
                    _handler = new MySqlRobot();
                    break;
                case UsedDatabaseType.SqlCE35:
                    _handler = new SqlCERobot();
                    break;
                case UsedDatabaseType.Sqlite:
                    _handler = new SqliteRobot();
                    break;
                case UsedDatabaseType.Firebird:
                    _handler = new FriebirdRobot();
                    break;
                case UsedDatabaseType.CSV:
                    _handler = new CSVRobot();
                    break;
                case UsedDatabaseType.Excel:
                    _handler = new ExcelRobot();
                    break;
                case UsedDatabaseType.Oracle:
                    _handler = new OracleRobot();
                    break; 
                case UsedDatabaseType.Effiproz:
                    _handler = new Effiproz.EffiprozRobot();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }


    }
}