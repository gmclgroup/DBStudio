using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_Excel:BaseLoginInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstRowIsColumnName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Database { get; set; }
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
        public OleDBVersion CurrentOleDBVersion { get; set; }
    }
}
