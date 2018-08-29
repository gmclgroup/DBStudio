using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SchemaInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseIndexSchema
    {
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IndexName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAscending { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsUnique { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPrimaryKey { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IsClustered { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrdinalPosition { get; set; }
    }
}
