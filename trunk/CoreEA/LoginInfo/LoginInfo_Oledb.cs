using System;
using System.Collections.Generic;
using System.Text;
using CoreEA.Args;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_Oledb : BaseLoginInfo
    {
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
        /// Office 2003 or 2007 or later version
        /// </summary>
        public OleDBVersion CurrentOleDBVersion { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum OleDBVersion
    {
        /// <summary>
        /// 
        /// </summary>
        Is2003,
        /// <summary>
        /// 
        /// </summary>
        Is2007,
    };
}
