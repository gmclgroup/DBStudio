using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSCEViewer_UnitTest
{
    public static class GlobalInfo
    {
        public static string Effiproz_NewTestFile
        {
            get
            {
                return "C:\\effiproz_new";
            }
        }

        public static string Effiproz_TestFile
        {
            get
            {
                return "C:\\ExampleData\\EffiprozData\\NorthwindEF.properties";
            }
        }

        public static string Sqlite_TestFile
        {
            get
            {
                return "C:\\ExampleData\\Sqlite\\test.db3";
            }
        }

        public static string Access_TestFile
        {
            get
            {
                return "c:\\ExampleData\\Access\\TestAccessDb2003.mdb";
            }
        }

        public static string Excel_TestFile
        {
            get
            {
                return "c:\\ExampleData\\Excel\\TestExcelData2003.xls";
            }
        }

        public static string SqlCE_TestFile
        {
            get
            {
                return "C:\\ExampleData\\Northwind.sdf";
            }
        }
        public static string SqlCE_NewTestFile
        {
            get
            {
                return "C:\\ExampleData\\NewTestFile.sdf";
            }
        }
        public static string SqlCE_TestTable
        {
            get
            {
                return "Customers";
            }
        }


    }
}
