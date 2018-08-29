using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEA.SchemaInfo;

namespace CoreEA.GlobalDefine
{
    /// <summary>
    /// 
    /// </summary>
    public static class GlobalExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsDBNull(this object text)
        {
            return DBNull.Value.Equals(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="targetColumnName"></param>
        /// <returns></returns>
        public static bool IsContaintPrimaryColumn(this List<BasePrimaryKeyInfo> pInfo, string targetColumnName)
        {
            bool ret = false;
            foreach (var item in pInfo)
            {
                if (item.ColumnName == targetColumnName)
                {
                    ret = true;
                    break;
                }

            }
            return ret;
        }
    }
}
