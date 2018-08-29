using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SchemaInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseColumnSchema
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseColumnSchema()
        {
            IsNullable = true;
            //Default set
            ColumnLength = 0.0d;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnType{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNullable{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultValue{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity{ get; set; }

        /// <summary>
        /// Some database has not this property.
        /// So we should replaced it by common property :
        /// ColumnLength
        /// </summary>
        public Int64 CharacterMaxLength { get; set; }

        /// <summary>
        /// Default value is 0.0d;
        /// </summary>
        public Double ColumnLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumericPrecision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumericScale { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OrdinalPosition { get; set; }

        #region Special For SSCE
        /// <summary>
        /// 
        /// </summary>
        public int AutoIncrementBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AutoIncrementSeed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RowGuidCol { get; set; }

        #endregion 

        #region ForSqlite
        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoIncrement { get; set; }
        #endregion
    }
}
