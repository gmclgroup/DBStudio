using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_Oracle : BaseLoginInfo
    {
        /// <summary>
        /// Ipaddress or machine name
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Server Instance Name
        /// </summary>
        public string SID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// Default port  is 1521
        /// </summary>
        public int Port { get; set; }

    }
}
