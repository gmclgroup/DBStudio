using System;
using System.Collections.Generic;
using System.Text;

namespace CoreEA.Exceptions
{
    /// <summary>
    /// /
    /// </summary>
    public class SubClassMustImplementException : SystemException
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                return "Child or inherited class must implement this base method";
            }
        }
    }

    /// <summary>
    /// /
    /// </summary>
    public class NotImplement : SystemException
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                return "Not implemented";
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ConnectErrorException : SystemException
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                return "Connection is not opened or timeout";
            }
        }
    }
}
