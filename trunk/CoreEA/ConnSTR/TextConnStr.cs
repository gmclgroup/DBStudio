using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.ConnSTR
{
    public static partial class DbConnectionString
    {
        /// <summary>
        /// Text Based File 
        /// Like CSV...
        /// </summary>
        public static class TxtFile
        {
            /// <summary>
            /// Notice : 
            /// the dbFolderName is just only the folder name 
            /// because the txt file name is the table name . 
            /// </summary>
            /// <param name="dbFolderName">the database folder</param>
            /// <returns></returns>
            public static string ODBC(string dbFolderName)
            {
                return string.Format("Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq={0};Extensions=asc,csv,tab,txt;", dbFolderName);
            }

            /// <summary>
            /// The delimiter can be specified in the registry at the following location:
            ///HKEY_LOCAL_MACHINE \ SOFTWARE \ Microsoft \ Jet \ 4.0 \ Engines \ Text
            ///"Format" = "TabDelimited"
            ///or
            ///"Format" = "Delimited(;)"
            /// </summary>
            /// <param name="folderName">the database folder</param>
            /// <param name="HDR">"HDR=Yes;" indicates that the first row contains columnnames,
            /// not data. "HDR=No;" indicates the opposite.</param>
            /// <returns></returns>
            public static string OleDb_DelimitedColumns(string folderName, bool HDR)
            {
                string isHDR = HDR ? "Yes" : "No";
                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"text;HDR={1};FMT=Delimited\";", folderName, isHDR);
            }

            /// <summary>
            ///  "HDR=Yes;" indicates that the first row contains columnnames, not data. "HDR=No;" indicates the opposite.
            ///To specify each columns length use the Schema.ini file. See description below.
            ///Important note!
            ///The quota " in the string needs to be escaped using your language specific escape syntax.
            ///c#, c++   \"
            ///VB6, VBScript   ""
            ///xml (web.config etc)   &quot;
            ///or maybe use a single quota '.
            ///
            /// Schema.ini
            ///_
            ///The schema information file tells the driver about the format of the text files. The file is always located in the same folder as the text files and must be named schema.ini.	
            ///[customers.txt]
            ///Format=TabDelimited
            ///ColNameHeader=True
            ///MaxScanRows=0
            ///CharacterSet=ANSI
            ///[orders.txt]
            ///Format=Delimited(;)
            ///ColNameHeader=True
            ///MaxScanRows=0
            ///CharacterSet=ANSI
            ///[invoices.txt]
            ///Format=FixedLength
            ///ColNameHeader=False
            ///Col1=FieldName1 Integer Width 15
            ///Col2=FieldName2 Date Width 15
            ///Col3=FieldName3 Char Width 40
            ///Col4=FieldName4 Float Width 20
            ///CharacterSet=ANSI
            /// </summary>
            /// <param name="folderName"></param>
            /// <param name="HDR"></param>
            /// <returns></returns>
            public static string OleDb_FixedLengthColumn(string folderName, bool HDR)
            {
                string isHDR = HDR ? "Yes" : "No";
                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"text;HDR={1};FMT=Fixed\";", folderName, isHDR);
            }


        }

    }
}
