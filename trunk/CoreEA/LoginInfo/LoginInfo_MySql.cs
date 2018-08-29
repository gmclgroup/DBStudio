using System;
using System.Collections.Generic;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_MySql : BaseLoginInfo
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
        public string Username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeOut { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPolling { get; set; }
    }
}
