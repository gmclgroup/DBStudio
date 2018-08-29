using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SchemaInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseTableSchema
    {

        /// <summary>
        /// The Catalog of Table 
        /// For example : A Table named [TestTable] is existed under [MyDb] Database ,
        /// the [MyDb] is the catalog
        /// File based database has no using of this property
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<BaseColumnSchema> Columns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<BasePrimaryKeyInfo> PrimaryKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<BaseIndexSchema> Indexes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BaseTableSchema()
        {
            Columns = new List<BaseColumnSchema>();
            Indexes = new List<BaseIndexSchema>();
            PrimaryKey = new List<BasePrimaryKeyInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        ~BaseTableSchema()
        {
            Columns.Clear();
            Indexes.Clear();
            PrimaryKey.Clear();
        }

    }
}
