using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_Firebird : BaseLoginInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string  DataSource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DataFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        //The following property ,will set default value whenever use or not
        /// <summary>
        /// Default:3
        /// </summary>
        public int Dialect { get; set; }
        /// <summary>
        /// Default:3050
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Default:50
        /// </summary>
        public int MaxPollSize { get; set; }
        /// <summary>
        /// Default:0
        /// </summary>
        public int MinPollSize { get; set; }
        /// <summary>
        /// Default:8192
        /// </summary>
        public int PacketSize { get; set; }
        /// <summary>
        /// Default:0
        /// </summary>
        public int ServerType { get; set; }
        /// <summary>
        /// Default:true
        /// </summary>
        public bool IsPolling { get; set; }
        /// <summary>
        /// Default:15
        /// </summary>
        public int ConnectionLifeTime { get; set; }
        /// <summary>
        /// Default:None
        /// </summary>
        public string CharSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoginInfo_Firebird()
        {
            CharSet = "None";
            Port = 3050;
            Dialect = 3;
            ConnectionLifeTime = 15;
            IsPolling = true;
            MinPollSize = 0;
            MaxPollSize = 50;
            PacketSize = 8192;
            ServerType = 0;
        }

    }
}
