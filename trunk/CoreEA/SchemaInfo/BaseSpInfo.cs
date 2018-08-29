using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SchemaInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseStoredProcedureInfo
    {
        /// <summary>
        /// simple routine identifier
        /// </summary>
        public string ProcedureName { get; set; }
        /// <summary>
        /// 	catalog in which routine is defined
        /// </summary>
        public string ProcedureCategory { get; set; }
        /// <summary>
        /// schema in which routine is defined
        /// </summary>
        public string ProcedureSchema { get; set; }
        /// <summary>
        /// 	"Unknown" or "No Result" or "Returns Result"
        /// </summary>
        public BaseSpType ProcedureType { get; set; }

        /// <summary>
        /// 	explanatory comment on the routine
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// name which uniquely identifies the procedure within its schema
        /// </summary>
        public string SpecificName { get; set; }

        /// <summary>
        /// The sql code of sp
        /// </summary>
        public string ProcedureCode { get; set; }

        /// <summary>
        /// Columns info in this sp
        /// </summary>
        public BaseColumnInfoInSP ColumnsInfoInProcedure { get; set; }
    }

    /// <summary>
    /// Column info in sp
    /// </summary>
    public class BaseColumnInfoInSP
    {
        /// <summary>
        /// 
        /// </summary>
        public string COLUMN_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TYPE_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DATA_TYPE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int COLUMN_SIZE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PRECISION { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SCALE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool NULLABLE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ORDINAL_POSITION { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BaseSpType
    {
        /// <summary>
        /// 
        /// </summary>
        Unknown,
        /// <summary>
        /// no return value :1
        /// </summary>
        NoResult,
        /// <summary>
        /// return value
        /// </summary>
        ReturnResult,
    };

}
