using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_CSV : BaseLoginInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// 为什么需要用户名呢？谁能回答我下
        /// </summary>
        public string Username { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstRowIsColumnName { get; set; }
    }
}
