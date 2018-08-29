using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CoreEA.Utility
{
    /// <summary>
    /// 数据库字段类型转换
    /// </summary>
    public static class TypeConvertor
    {

        /// <summary>
        /// 由于各种类型所存储长度的节点不同，
        /// 因此需要在Sqlserver中根据当前类型名来确定存储字段长度的列名
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ParseSqlServerLengthNodeNameFromTypeName(string p)
        {
            string rt=string.Empty;
            switch (p.ToLower())
            {
                case "nchar":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "bigint":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "binary":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "ntext":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "numeric":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "nvarchar":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "real":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "smalldatetime":
                    rt = "";
                    break;
                case "smallint":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "smallmoney":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "sql_variant":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "text":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "time":
                    rt = "";
                    break;
                case "timestamp":
                    rt = "";
                    break;
                case "tinyint":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "uniqueidentifier":
                    rt = "";
                    break;
                case "varbinary":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "varchar":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "xml":
                    rt = "";
                    break;
                case "money":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "int":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "image":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "hierarchyid":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "geometry":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "geography":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "float":
                    rt = "NUMERIC_PRECISION";
                    break;
                case "bit":
                    rt = "";
                    break;
                case "char":
                    rt = "CHARACTER_MAXIMUM_LENGTH";
                    break;
                case "datetime":
                    rt = "";
                    break;
                default:
                    //
                    break;
            }
            return rt;
        }

        /// <summary>
        /// 将sqlserver 类型和长度转换成相对应的SSCE语句
        /// 注意，某些类型即使指定长度，也不作用。因为系统自身限制
        /// </summary>
        /// <param name="sqlServerType"></param>
        /// <param name="length">
        /// 注意这里的Length是可空类型，当为空值时，各类型自己决定长度
        /// </param>
        /// <returns></returns>
        public static string ParseSqlServerDbTypeToSqlCeDbType(string sqlServerType, int? length)
        {
            string ceType = string.Empty;
            string srcType=sqlServerType.ToLower();
            switch (srcType)
            {
                case "bitint":
                    ceType = "bigint";
                    break;
                case "binary":
                    if (length == null)
                    {
                        length = 10;
                    }
                    ceType = String.Format("binary({0})", length);
                    break;
                case "char":
                    //SSCE has no this type
                    if (length == null)
                    {
                        length = 10;
                    }
                    ceType = String.Format("char({0})", length);
                    break;

                case "varchar":
                    //SSCE has no this type
                    if (length == null)
                    {
                        length = 255;
                    }
                    ceType = String.Format("nvarchar({0})", length);
                    break;
                case "varchar(MAX)":
                    if (length == null)
                    {
                        length = 4000;
                    }
                    ceType = String.Format("nTEXT", length);
                    break;
                case "computed columns":
                    //SSCE has no this type
                    ceType = String.Format("nVarChar({0})", length);
                    break;
                case "datetime":
                    ceType = "datetime";
                    break;
                case "decimal":
                    ceType = "decimal ";
                    break;
                case "double precision":
                    ceType = "double precision";
                    break;

                case "float":
                    ceType = "float";
                    break;
                case "image":
                    ceType = "image";
                    break;
                case "int":
                    ceType = "integer";
                    break;

                case "money":
                    ceType = "money";
                    break;
                case "nchar":
                    if (length == null)
                    {
                        length = 16;
                    }
                    ceType = String.Format("nchar({0})", length);
                    break;
                case "nvarchar":
                    if (length == null)
                    {
                        length = 4000;
                    }
                    ceType = String.Format("nvarchar({0})", length);
                    break;
                case "nvarchar(MAX)":
                    if (length == null)
                    {
                        length = 4000;
                    }
                    ceType = String.Format("nTEXT", length);
                    break;
                case "ntext":
                 //   ceType = String.Format("nTEXT", length);
                    ceType = "nText";
                    break;
                case "dec":
                    ceType = "numeric";
                    break;

                case "real":
                    ceType = "real";
                    break;

                case "smalldatetime":
                    ceType = "datetime";
                    break;
                case "smallint ":
                    ceType = "smallint";
                    break;
                case "smallmoney":
                    ceType = "money";
                    break;
                case "variant ":
                    ceType = "nText";
                    break;
                case "text":
                    ceType = "nText";
                    break;
                case "tinyint":
                    ceType = "tinyint";
                    break;
                case "uniqueidentifier":
                    ceType = "uniqueidentifier";
                    break;
                case "varbinary":
                    ceType = string.Format("varbinary({)})", length);
                    break;
                case "varbinary(MAX)":
                    ceType = "image";
                    break;
                case "xml":
                    ceType = "nText";
                    break;
                case "bit":
                    ceType = "bit";
                    break;
                default:
                    if (length == null)
                    {
                        length = 50;
                    }
                    ceType = String.Format("nvarchar({0})", length);
                    Debug.WriteLine("Use default type ========>" + sqlServerType);
                    break;
            }
            return ceType;
        }

        /// <summary>
        /// 转换Ado类型到SSCE
        /// </summary>
        /// <param name="adoType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ParseADODbTypeToSqlCeDbType(string adoType, int length)
        {
            string ceType = string.Empty;
            switch (adoType)
            {
                //adTinyInt
                case "adUnsignedTinyInt":
                    ceType = "tinyint";
                    break;
                case "adBigInt":
                    ceType = "bigint";
                    break;
                case "adInteger":
                    ceType = "integer";
                    break;
                case "adSmallInt":
                    ceType = "smallint";
                    break;
                case "adBoolean":
                    ceType = "bit";
                    break;
                case "adNumeric":
                    ceType = "decimal ";
                    break;
                case "adCurrency":
                    ceType = "money";
                    break;
                case "adSingle":
                    ceType = "float";
                    break;
                case "adDouble":
                    ceType = "real";
                    break;
                case "adDate":
                    ceType = "datetime";
                    break;
                case "adDBDate":
                    ceType = "datetime";
                    break;
                case "adDBTime":
                    ceType = "datetime";
                    break;
                case "adWChar":
                    ceType = String.Format("nchar({0})", length);
                    break;
                case "adVarWChar":
                    ceType = String.Format("nvarchar({0})", length);
                    break;
                case "adChar":
                    ceType = "nchar(length)";
                    break;
                case "adVarChar":
                    ceType = String.Format("nvarchar({0})", length);
                    break;
                case "adBinary":
                    ceType = String.Format("binary({0})", length);
                    break;
                case "adVarBinary":
                    ceType = String.Format("varbinary({0})", length);
                    break;
                case "adLongVarBinary":
                    ceType = "image";
                    break;
                case "adLongVarWChar":
                    ceType = "ntext";
                    break;
                case "adLongVarChar":
                    ceType = "ntext";
                    break;
                case "adGuid":
                    ceType = "uniqueidentifier";
                    break;
                default:
                    ceType = String.Format("nVarChar({0})", length);
                    Debug.WriteLine("Use default type ========>" + adoType);
                    break;
            }
            return ceType;
        }

        /// <summary>
        /// 转换MySql 类型到SSCE
        /// </summary>
        /// <param name="mysqlDbType"></param>
        /// <returns></returns>
        public static string ParseMySqlDbTypeToSqlCeDbType(string mysqlDbType)
        {
            string ceType = string.Empty;
            switch (mysqlDbType)
            {
                default:
                    ceType = "nVarChar(255)";
                    break;
            }
            return ceType;
        }

    }
}
