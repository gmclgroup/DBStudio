using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_Effiproz : BaseLoginInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public LoginInfo_Effiproz()
        {
            IsAutoCommit = false;
            IsAutoShutdown = false;
            IsReadOnly = false;
            DBConnectionType = ConnectionType.Memory;
        }

        /// <summary>
        /// Sets the "connection type" for the connection string. 
        /// The default is "memory".
        /// Valid values are "file" and "memory" 
        /// </summary>
        public ConnectionType DBConnectionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Sets the default isolation level for transactions on the connection. 
        /// The default is ReadCommitted. 
        /// </summary>
        public string IsolateLevel { get; set; }


        /// <summary>
        /// Sets the database to open on the connection. 
        /// </summary>
        public string InitialCatalog { get; set; }
        /// <summary>
        /// Sets the "auto commit" for the database connection. The default is false. 
        /// </summary>
        public bool IsAutoCommit { get; set; }
        /// <summary>
        /// Sets the "auto shutdown" for the database connection. 
        /// The default is false. 
        /// If true database is automatically checkpoint/shutdown when all 
        /// connections are closed (In-Memory databases will loose all data). 
        /// </summary>
        public bool IsAutoShutdown { get; set; }
        /// <summary>
        /// string type
        /// Selects the encryption algorithm. Valid values are {DES, TripleDES, AES, Rijndael}. 
        /// Optional. 
        /// </summary>
        public CipherType CipherTypeString { get; set; }
        /// <summary>
        /// The encryption algorithm initialization vector. Optional. 
        /// </summary>
        public string CipherKey { get; set; }
        /// <summary>
        /// The encryption algorithm initialization vector. Optional. 
        /// </summary>
        public string CipherIV { get; set; }

        /// <summary>
        /// Determines whether or not the connection will automatically participate in the current distributed transaction. Default is false. 
        /// </summary>
        public bool IsEnList { get; set; }

        /// <summary>
        /// Sets the "readonly" for the database connection. The default is false. 
        /// </summary>
        public bool IsReadOnly { get; set; }
    }

    /// <summary>
    ///  Optional Value 
    /// Default is None
    /// </summary>
    public enum CipherType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        DES,
        /// <summary>
        /// 
        /// </summary>
        TripleDES,
        /// <summary>
        /// 
        /// </summary>
        AES,
        /// <summary>
        /// 
        /// </summary>
        Rijndael,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// 
        /// </summary>
        File,
        /// <summary>
        /// 
        /// </summary>
        Memory,
    }

}
