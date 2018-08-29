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
        public static class Excel
        {
            /// <summary>
            /// Get Connection String To Excel
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static string GetConnectionString(string file)
            {
                return "Driver={Microsoft Excel Driver (*.xls)};DBQ=" + file;
            }

            /// <summary>
            /// Connect Ok
            /// 
            /// </summary>
            /// <param name="file"></param>
            /// <param name="isFirstRowIsColumnName">
            /// "HDR=Yes;" indicates that the first row contains columnnames, not data. "HDR=No;" indicates the opposite.
            /// </param>
            /// <returns></returns>
            public static string GetOleDbConnectionString(string file, bool isFirstRowIsColumnName)
            {
                string c = "Yes";
                if (!isFirstRowIsColumnName)
                {
                    c = "No";
                }

                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR={1};IMEX=1\"", file, c);
            }

            /// <summary>
            /// ACE OLEDB 12.0 Xlsx files
            /// 
            /// "HDR=Yes;" indicates that the first row contains columnnames, not data. 
            /// "HDR=No;" indicates the opposite.
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static string GetXlsx(string file)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", file);
            }

            /// <summary>
            /// This one is for connecting to Excel 2007 files with the Xlsb file extension. 
            /// That is the Office Open XML format saved in a binary format.
            /// I e the structure is similar but it's not saved in a text readable format 
            /// as the Xlsx files and can improve performance if the file contains a lot of data.
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static string GetXlsb(string file)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\";", file);
            }

            /// <summary>
            /// This one is for connecting to Excel 2007 files with the Xlsm file extension. 
            /// That is the Office Open XML format with macros enabled.
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static string GetXlsm(string file)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Macro;HDR=YES\";", file);
            }
        }
    }
}
