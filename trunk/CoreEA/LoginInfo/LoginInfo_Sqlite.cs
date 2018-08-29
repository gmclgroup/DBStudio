using System;
using System.Collections.Generic;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_Sqlite : BaseLoginInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string DbFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUnicode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly { get; set; }

    }
}
