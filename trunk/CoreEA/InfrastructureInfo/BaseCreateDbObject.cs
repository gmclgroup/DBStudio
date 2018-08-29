using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.InfrastructureInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseCreateDbObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DbLocation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DbLogFileLocation { get; set; }
        /// <summary>
        /// /
        /// </summary>
        public uint InitSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public uint FileGrowth { get; set; }
    }
}
