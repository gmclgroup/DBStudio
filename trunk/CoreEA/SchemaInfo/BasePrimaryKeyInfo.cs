using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SchemaInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class BasePrimaryKeyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 	primary key constraint name
        /// </summary>
        public string PKName { get; set; }
        /// <summary>
        /// sequence number within primary key
        /// </summary>
        public int KeySequence { get; set; }
    }
}
