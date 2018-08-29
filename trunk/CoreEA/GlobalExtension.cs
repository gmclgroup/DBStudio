using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA
{
    /// <summary>
    /// Some methods in this extension class maybe existed in other common library
    /// But due to wanna keep undependent on other libraries
    /// So leave these method standalone . 
    /// </summary>
    static class GlobalExtension
    {
        /// <summary>
        /// Global Extension Method
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool 不是空的(this string text)
        {
            return !string.IsNullOrEmpty(text);
        }
    }
}
