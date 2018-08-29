using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.ConnSTR
{
    public static partial class DbConnectionString
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Access2007
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="database"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static string GetOleDBString(string database, string password)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1};",
                    database, password);
            }
        }
    }
}
