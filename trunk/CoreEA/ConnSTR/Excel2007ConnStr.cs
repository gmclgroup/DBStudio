using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.ConnSTR
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class DbConnectionString
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Excel2007
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="database"></param>
            /// <param name="isFirstColumnRowName"></param>
            /// <returns></returns>
            public static string GetOleDBString(string database,bool isFirstColumnRowName)
            {
                string result=string.Empty;
                string tt="NO";
                if(isFirstColumnRowName)
                {
                    tt="YES";
                }
                result=string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR={1}\";",
                    database, tt);
                return result;
            }
        }
    }
}
