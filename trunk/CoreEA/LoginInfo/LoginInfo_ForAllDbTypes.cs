using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEA.Args;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_ForAllDbTypes:BaseLoginInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsTrustedConn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsUnicode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEncrypt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurDbServerConnMode CurConnMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AttachedFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// This only useful when oledb type
        /// Especially for office 2003 or 2007
        /// </summary>
        public OleDBVersion CurrentOleDBVersion { get; set; }
    }
}
