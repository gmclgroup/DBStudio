using System;
using System.Collections.Generic;
using System.Text;
using CoreEA.Args;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_SSCE:BaseLoginInfo
    {
        /// <summary>
        /// Database name
        /// same to database file full path
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEncrypted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OpenMode CurOpenMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCaseSensitive { get; set; }


        /// <summary>
        /// Default is 256M 
        /// Recommend set it when you know it .
        /// The max value is 4GB(4000M) in CE3.5
        /// 
        /// </summary>
        public uint MaxDbSize { get; set; }

        /// <summary>
        /// Default is 1024M
        /// Recommend set it when you know it.
        /// </summary>
        public uint MaxBufferSize { get; set; }
    }
}
