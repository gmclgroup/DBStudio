using System;
using System.Collections.Generic;
using System.Text;

namespace CoreEA.GlobalDefine
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SP
    {
        private static string _openDbErrorString = "Can not open database";
        private static string _invalidParameters = "Invalid Parameters";
        /// <summary>
        /// Error info when received the invalid msg
        /// </summary>
        public static string X_InvalidParameters
        {
            get { return _invalidParameters; }
            set { _invalidParameters = value; }
        }
        /// <summary>
        /// Opening Database error string
        /// </summary>
        public static string X_OpenDbErrorString
        {
            get { return _openDbErrorString; }
            set { _openDbErrorString = value; }
        }

        private static string lastErrorMsg;

        public static string LastErrorMsg
        {
            get { return lastErrorMsg; }
            set { lastErrorMsg = value; }
        }

        /// <summary>
        /// Error Number when retrive the max  id 
        /// It will return this value if error occured
        /// </summary>
        public static readonly int ErrorNumber = 9999999;
    }
}
