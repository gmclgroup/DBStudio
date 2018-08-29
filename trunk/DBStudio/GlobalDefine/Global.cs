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
using System.Windows.Controls;
using System.ComponentModel;
using System.Data;
using CoreEA.SchemaInfo;
using ETL;
using System.Diagnostics;
using CoreEA;
using DBStudio.Utility;

namespace DBStudio.GlobalDefine
{
    internal partial class MyGlobal
    {
        internal static string DefaultSkinFileFullPath
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Themes\\Default\\DefaultSkin.xaml";
            }
        }

        /// <summary>
        /// Orginal culture name 
        /// record it 
        /// </summary>
        public static string OriginalCultureName { get; set; }

        private static string globalDebugFolder = string.Empty;
        /// <summary>
        /// Temporary folder for save debug info files
        /// </summary>
        internal static string GlobalDebugFolder
        {
            get
            {
                if (string.IsNullOrEmpty(globalDebugFolder))
                {
                    globalDebugFolder = System.IO.Path.GetTempPath();

                }

                return globalDebugFolder;
            }
        }

        internal static string GettingStartDoc
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Help\\GettingStart.doc";
            }
        }

        internal static string GetCreateSSCESchemaStringCmd(BaseTableSchema tableSchmea)
        {
            
            string tableName = tableSchmea.TableName;

            StringBuilder createSchemaScript = new StringBuilder();

            createSchemaScript.AppendFormat("CREATE TABLE [{0}] (", tableName);

            //Here should convert to SSCEColumnSchema
            //because here we only support sync data to SSCE currently
            //Maybe will refactor ,but not sure.
            tableSchmea.Columns.ForEach(delegate(BaseColumnSchema pColumn)
            {
                BaseColumnSchema eachColumn = pColumn as BaseColumnSchema;
                if (eachColumn == null)
                {
                    return;
                }

                switch (eachColumn.ColumnType.ToLower())
                {
                    case "char":
                        eachColumn.ColumnType = "nchar";
                        break;
                    case "varchar":
                        eachColumn.ColumnType = "nvarchar";
                        break;
                    case "text":
                        eachColumn.ColumnType = "ntext";
                        break;
                }

                switch (eachColumn.ColumnType.ToLower())
                {
                    case "nvarchar":
                        createSchemaScript.AppendFormat("[{0}] {1}({2}) {3} {4} {5}, ",
                             eachColumn.ColumnName,
                             eachColumn.ColumnType,
                             eachColumn.CharacterMaxLength,
                             eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                             eachColumn.DefaultValue.IsNotEmpty() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                             System.Environment.NewLine);
                        break;
                    case "nchar":
                        createSchemaScript.AppendFormat("[{0}] {1}({2}) {3} {4} {5}, ",
                            eachColumn.ColumnName,
                            "nchar",
                            eachColumn.CharacterMaxLength,
                             eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                             eachColumn.DefaultValue.IsNotEmpty() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                             System.Environment.NewLine);
                        break;
                    case "numeric":
                        createSchemaScript.AppendFormat("[{0}] {1}({2},{3}) {4} {5} {6}, ",
                             eachColumn.ColumnName,
                             eachColumn.ColumnType,
                             eachColumn.NumericPrecision,
                             eachColumn.NumericScale,
                             eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                             eachColumn.DefaultValue.IsNotEmpty() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                             System.Environment.NewLine);
                        break;
                    //Not support such type
                    case "timestamp":

                        break;
                    case "enum":

                        break;
                    default:
                        createSchemaScript.AppendFormat("[{0}] {1} {2} {3} {4}{5} {6}, ",
                             eachColumn.ColumnName,
                             eachColumn.ColumnType,
                             eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                             eachColumn.DefaultValue.IsNotEmpty() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                             eachColumn.RowGuidCol ? "ROWGUIDeachColumn" : string.Empty,
                             (eachColumn.AutoIncrementBy > 0 ? string.Format("IDENTITY ({0},{1})", eachColumn.AutoIncrementSeed, eachColumn.AutoIncrementBy) : string.Empty),
                             System.Environment.NewLine);
                        break;
                }
            });

            createSchemaScript.Remove(createSchemaScript.Length - 2, 2);
            createSchemaScript.Append(");");

            return createSchemaScript.ToString();
        }

        internal static List<DbTypeWrapper> GetDbTypeWrapper
        {
            get
            {
                List<DbTypeWrapper> m = new List<DbTypeWrapper>();
                m.Add(new DbTypeWrapper() { DisplayName = "Sql CE", MyType = CoreE.UsedDatabaseType.SqlCE35});
                m.Add(new DbTypeWrapper() { DisplayName = "Sql Server", MyType = CoreE.UsedDatabaseType.SqlServer });
                m.Add(new DbTypeWrapper() { DisplayName = "My Sql", MyType = CoreE.UsedDatabaseType.MySql });
                m.Add(new DbTypeWrapper() { DisplayName = "Sqlite 3", MyType = CoreE.UsedDatabaseType.Sqlite });
                m.Add(new DbTypeWrapper() { DisplayName = "OleDB", MyType = CoreE.UsedDatabaseType.OleDb });
                m.Add(new DbTypeWrapper() { DisplayName = "Effiproz", MyType = CoreE.UsedDatabaseType.Effiproz });
                return m;
            }
        }

        /// <summary>
        /// Filter the column type and generate a useful /firendly sql command with machine auto generation
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 
        [Description("Need Refactor")]
        internal static string ConvertToTargetValue(string p)
        {
            string ret = string.Empty;

            switch (p.ToLower())
            {
                case "nvarchar":
                    ret = @"''";
                    break;
                case "datetime":
                    ret = "2008-11-11 11:11:11";
                    break;
                case "int":
                    ret = "1";
                    break;
                default:
                    break;
            }

            return ret;
        }

        static List<string> _SqlCeDbTypeList = new List<string>();
        internal static List<string> SSCE_SUPPORTED_DATATYPE
        {
            get
            {
                if (_SqlCeDbTypeList.Count == 0)
                {
                    _SqlCeDbTypeList.Add("tinyint");
                    _SqlCeDbTypeList.Add("bigint");
                    _SqlCeDbTypeList.Add("integer");
                    _SqlCeDbTypeList.Add("int");
                    _SqlCeDbTypeList.Add("smallint");
                    _SqlCeDbTypeList.Add("bit");
                    _SqlCeDbTypeList.Add("decimal");
                    _SqlCeDbTypeList.Add("money");
                    _SqlCeDbTypeList.Add("float");
                    _SqlCeDbTypeList.Add("real");
                    _SqlCeDbTypeList.Add("datetime");
                    _SqlCeDbTypeList.Add("nchar");
                    _SqlCeDbTypeList.Add("nvarchar");
                    _SqlCeDbTypeList.Add("binary");
                    _SqlCeDbTypeList.Add("varbinary");
                    _SqlCeDbTypeList.Add("image");
                    _SqlCeDbTypeList.Add("ntext");
                    _SqlCeDbTypeList.Add("uniqueidentifier");
                }
                return _SqlCeDbTypeList;
            }
        }


        internal static string HelpFile
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Content\\" + Properties.Settings.Default.HelpFile;
            }

        }

        internal static string ColumnName = "COLUMN_NAME";
        internal static string DataType = "DATA_TYPE";
        internal static string DataLength = "CHARACTER_MAXIMUM_LENGTH";
        //internal static string Unique= "Unique";
        //internal static string PrimaryKey = "COLUMN_NAME";
        internal static string AllowNull = "IS_NULLABLE";

        //These const value called external ,too.
        //please pay attention
        internal static string SQLCE_FILE_FILTER = "Sql Ce db file (*.sdf)|*.sdf|All Files(*.*)|*.*";
        internal static string SQLite_FILE_FILTER = "Sqlite db(*.db)|*.db|Sqlite db file (*.db3)|*.db3|All Files(*.*)|*.*";
        internal static string MDB_FILE_FILTER = "MDB file (*.mdb)|*.mdb|All Files(*.*)|*.*";
        internal static string EXCEL_FILE_FILTER = "Excel file (*.xls)|*.xls|All Files(*.*)|*.*";
        internal static string CVS_FILE_FILTER = "Csv file (*.csv)|*.csv|Text file (*.txt)|*.txt|All Files(*.*)|*.*";
        internal static string Effiproz_FILE_FILTER = "properties file (*.properties)|*.properties|script file (*.script)|*.script|Loc file (*.loc)|*.loc|All Files(*.*)|*.*";
        internal static string FireBird_FILE_FILTER = "FDB file (*.fdb)|*.fdb|All Files(*.*)|*.*";

        internal static string BackupDatabase_FILE_FILTER = "bak (*.bak)|*.bak|All Files(*.*)|*.*";

        internal static string CreateSqlDatabase_FILE_FILTER = "mdf (*.mdf)|*.mdf|All Files(*.*)|*.*";
        internal static string CreateSqlLogDatabase_FILE_FILTER = "ldf (*.ldf)|*.ldf|All Files(*.*)|*.*";
    }

    internal static class NoLenghtType
    {
        private static List<string> coll = new List<string>();
        internal static List<string> Collections
        {
            get
            {
                if (coll.Count == 0)
                {
                    coll.Add("int");
                    coll.Add("integer");
                    coll.Add("smallint");
                    coll.Add("byte");
                    coll.Add("double");
                    coll.Add("decimal");
                    coll.Add("datetime");
                    coll.Add("money");
                    coll.Add("bool");
                    coll.Add("bigint");
                    coll.Add("tinyint");
                }
                return coll;
            }
        }

    }

    internal class DbTypeWrapper
    {
        public string DisplayName { get; set; }

        public CoreE.UsedDatabaseType MyType { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }



    internal class DbTypeCollection
    {
        public int SortIndex { get; set; }

        public string DisplayName { get; set; }

        public CoreE.UsedDatabaseType MyType { get; set; }

        public string MainUI_URI { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
