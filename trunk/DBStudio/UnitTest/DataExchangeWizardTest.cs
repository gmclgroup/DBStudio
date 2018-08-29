using DBStudio.DataExchangeCenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Collections.Generic;
using System.Data.Linq;
using System;
using System.Linq;
using CoreEA.LoginInfo;
using System.IO;
using SSCEViewer_UnitTest;
using CoreEA.SchemaInfo;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using DBStudio.CommonMethod;
using System.Text;
using System.Data;
using System.Data.Common;
using ETL;

namespace SeasonStarDatabaseManagement_UnitTest
{


    /// <summary>
    ///This is a test class for DataExchangeWizardTest and is intended
    ///to contain all DataExchangeWizardTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DataExchangeWizardTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region 2 SSCE
        /// <summary>
        ///A test for DoFinish
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_SqlServer2SSCE()
        {

            testContextInstance.RecordTestMethod();

            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            List<string> tableList = new List<string>();
            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;
            srcHandle.Open(new CoreEA.LoginInfo.LoginInfo_SqlServer()
            {
                X_Server = "",
                X_UserName = "",
                X_Pwd = "",
                X_Database = "",
                IsTrustedConn = false,
                X_CurDbConnectionMode = CoreEA.Args.CurDbServerConnMode.SqlServer2000
            });

            if (srcHandle.IsOpened)
            {
                tableList = srcHandle.GetTableListInDatabase();
            }

            tableList = tableList.Take(4).ToList();

            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;

            string targetFile = "C:\\a3.sdf";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            targetHandle.CreateDatabase(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile, IsCaseSensitive = true });

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile });

            if (targetHandle.IsOpened)
            {
                target.DoExchangeData(srcHandle, targetHandle, tableList);
            }

            TestGlobalExtension.RecordTestMethod();
        }

        /// <summary>
        ///First try to test the simple db file
        ///then comment visa vesa ,to test the full element db file
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_SSCE2SSCE()
        {

            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            srcHandle.Open(new LoginInfo_SSCE() { DbName = GlobalInfo.SqlCE_TestFile });
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            string targetFile = "C:\\a4.sdf";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            targetHandle.CreateDatabase(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile, IsCaseSensitive = true });

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            bool result = target.DoExchangeData(srcHandle, targetHandle, tableList, false);
            Assert.AreEqual(true, result);
        }

        /// <summary>
        ///A test for DoFinish
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_Access2SSCE()
        {
            testContextInstance.RecordTestMethod();

            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.OleDb).X_Handler;
            srcHandle.Open(new LoginInfo_Oledb()
            {
                Database = GlobalInfo.Access_TestFile,
            });
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            string targetFile = "C:\\a5.sdf";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            targetHandle.CreateDatabase(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile, IsCaseSensitive = true });

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            bool result = target.DoExchangeData(srcHandle, targetHandle, tableList, false);
            Assert.AreEqual(true, result);

        }


        /// <summary>
        ///A test for DoFinish
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_MySQL2SSCE()
        {
            testContextInstance.RecordTestMethod();

            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
            srcHandle.Open(new LoginInfo_MySql() { Server = "localhost", Username = "root", Pwd = "noway", Database = "test" });
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            string targetFile = "C:\\a6.sdf";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            targetHandle.CreateDatabase(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile, IsCaseSensitive = true });

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            target.DoExchangeData(srcHandle, targetHandle, tableList);

            TestGlobalExtension.RecordTestMethod();
        }

        /// <summary>
        ///A test for DoFinish
        ///</summary>
         [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_Excel2SSCE()
        {
            testContextInstance.RecordTestMethod();

            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Excel).X_Handler;
            srcHandle.Open(new LoginInfo_Excel() { Database =GlobalInfo.Excel_TestFile});
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            string targetFile = "C:\\a7.sdf";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            targetHandle.CreateDatabase(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile, IsCaseSensitive = true });

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            bool result=target.DoExchangeData(srcHandle, targetHandle, tableList,false);

            Assert.AreEqual(true, result);
            
        }

        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_Sqlite2SSCE()
        {
            testContextInstance.RecordTestMethod();


            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;
            srcHandle.Open(new LoginInfo_Sqlite() { DbFile = GlobalInfo.Sqlite_TestFile, IsUnicode = true });
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            string targetFile = "C:\\a8.sdf";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            targetHandle.CreateDatabase(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile, IsCaseSensitive = true });

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SSCE() { DbName = targetFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            bool result=target.DoExchangeData(srcHandle, targetHandle, tableList,false);

            Assert.AreEqual(true, result);
        }

        #endregion 


        #region 2 CSV
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_SSCE2CSV()
        {
            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            srcHandle.Open(new LoginInfo_SSCE() { DbName =GlobalInfo.SqlCE_TestFile });
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.CSV).X_Handler;
            string targetFile = "C:\\a11.csv";
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_CSV()
            {
                Database = targetFile,
                IsFirstRowIsColumnName = true,
            });

            List<string> tableList = srcHandle.GetTableListInDatabase();
            bool result=target.DoExchangeData(srcHandle, targetHandle, tableList,false);

            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_Effiproz2CSV()
        {
            //DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            //CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Effiproz).X_Handler;
            //srcHandle.Open(new LoginInfo_Effiproz() { 
            //    DBConnectionType=ConnectionType.File,
            //    InitialCatalog=GlobalInfo.Effiproz_TestFile,
            //    Username="sa",
            //    Password=""});

            //CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.CSV).X_Handler;
            //string targetFile = "C:\\a12.csv";
            //if (File.Exists(targetFile))
            //{
            //    File.Delete(targetFile);
            //}
            //targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_CSV()
            //{
            //    Database = targetFile,
            //    IsFirstRowIsColumnName = true,
            //});

            //List<string> tableList = srcHandle.GetTableListInDatabase();
            //bool result = target.DoExchangeData(srcHandle, targetHandle, tableList, false);

            //Assert.AreEqual(true, result);
        }
        #endregion 

        #region Excel 2 Mysql
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_Excel2MySql()
        {
            DataExchangeWizard_Accessor target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Excel).X_Handler;
            //srcHandle.Open(new LoginInfo_Excel() { Database = GlobalInfo.Excel_TestFile});
            srcHandle.Open(new LoginInfo_Excel() { Database = "C:\\1.xlsx",CurrentOleDBVersion=OleDBVersion.Is2007
            ,IsFirstRowIsColumnName=true});
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_MySql()
            {
                Server="localhost",
                Username="root",
                Database="TestDemo",
                Pwd="noway",
            });

            //List<string> tableList = srcHandle.GetTableListInDatabase();

            List<string> tableList = new List<string>()
            {
               "Sheet1$",
            };
            bool result = target.DoExchangeData(srcHandle, targetHandle, tableList, false);

            Assert.AreEqual(true, result);
        }

    
        #endregion 

        /// <summary>
        ///A test for DoFinish
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_SSCE2SqlServer()
        {
            testContextInstance.RecordTestMethod();
            DataExchangeWizard_Accessor target = null;
            try
            {
                target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value
            }
            catch
            { }

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;

            string srcCEFile =GlobalInfo.SqlCE_TestFile;
            srcHandle.Open(new LoginInfo_SSCE() { DbName = srcCEFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_SqlServer()
            {
                X_Server = @"VincentNotebook\SQL2008EXPRESS",
                X_UserName = "sa",
                X_Pwd = "dfbbybf",
                X_Database = "MyTestDB",
                IsTrustedConn = false,
                X_CurDbConnectionMode = CoreEA.Args.CurDbServerConnMode.SqlServer2000
            });

            List<string> targetDbList = targetHandle.GetTableListInDatabase();


            foreach (var tableName in tableList)
            {
                BaseTableSchema schemaInfo = srcHandle.GetTableSchemaInfoObject(tableName);

                if (!targetDbList.Contains(tableName))
                {
                    targetHandle.CreateTable(schemaInfo);
                    try
                    {
                        CommonUtil_Accessor.ExchangeDataBetweenAnyDbs(srcHandle, targetHandle, tableName);
                    }
                    catch
                    {
                        targetHandle.DeleteTable(tableName);
                    }
                }


            }


            TestGlobalExtension.RecordTestMethod();
        }


        /// <summary>
        ///A test for DoFinish
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_SSCE2Sqlite()
        {
            testContextInstance.RecordTestMethod();
            DataExchangeWizard_Accessor target = null;
            try
            {
                target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value
            }
            catch
            { }

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;

            string srcCEFile = GlobalInfo.SqlCE_TestFile;
            srcHandle.Open(new LoginInfo_SSCE() { DbName = srcCEFile });
            List<string> tableList = srcHandle.GetTableListInDatabase();

            targetHandle.Open(new CoreEA.LoginInfo.LoginInfo_Sqlite()
            {
                DbFile="C:\\1.sqlite",
            });

            List<string> targetDbList = targetHandle.GetTableListInDatabase();

            foreach (var tableName in tableList)
            {
                BaseTableSchema schemaInfo = srcHandle.GetTableSchemaInfoObject(tableName);

                if (!targetDbList.Contains(tableName))
                {
                    targetHandle.CreateTable(schemaInfo);
                    try
                    {
                        CommonUtil_Accessor.ExchangeDataBetweenAnyDbs(srcHandle, targetHandle, tableName);
                    }
                    catch
                    {
                        targetHandle.DeleteTable(tableName);
                    }
                }


            }


            TestGlobalExtension.RecordTestMethod();
        }


        #region MySql 
        
        /// <summary>
        ///A test for DoFinish
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DataTransfer_SSCE2MySQL()
        {
            DataExchangeWizard_Accessor target = null;
            try
            {
                target = new DataExchangeWizard_Accessor(); // TODO: Initialize to an appropriate value
            }
            catch
            { }

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            CoreEA.ICoreEAHander targetHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;

            srcHandle.Open(new LoginInfo_SSCE() { DbName = GlobalInfo.SqlCE_TestFile });

            List<string> tableList = srcHandle.GetTableListInDatabase();

            targetHandle.Open(new LoginInfo_MySql()
            {
                Database = "category",
                Server = "localhost",
                Username = "root",
                Pwd = "noway",
                
            });

            List<string> targetDbList = targetHandle.GetTableListInDatabase();
            bool isHasError = false;
            foreach (var tableName in tableList)
            {
                BaseTableSchema schemaInfo = srcHandle.GetTableSchemaInfoObject(tableName);

                if (!targetDbList.Contains(tableName,new MyStringComparer()))
                {
                    targetHandle.CreateTable(schemaInfo);
                    try
                    {
                        bool result=CommonUtil_Accessor.ExchangeDataBetweenAnyDbs(srcHandle, targetHandle, tableName);
                        if (result == false)
                        {
                            isHasError = true;
                        }
                    }
                    catch
                    {
                        targetHandle.DeleteTable(tableName);
                    }
                }
            }

            Assert.AreEqual(false, isHasError);
        }
        #endregion


        class MyStringComparer : IEqualityComparer<string>
        {

            public bool Equals(string b1, string b2)
            {
                if (b1.ToLower() == b2.ToLower())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(string bx)
            {
                //int hCode = bx.Height ^ bx.Length ^ bx.Width;
                //return hCode.GetHashCode();
                return base.GetHashCode();
            }

        }


    }//Endof Classs
}//Endof Namespace

