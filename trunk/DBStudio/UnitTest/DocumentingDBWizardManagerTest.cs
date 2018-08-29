using DBStudio.DocumentingDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBStudio.DocumentingDB.Exportors;
using CoreEA.LoginInfo;
using System.Collections.Generic;
using System.Linq;
using CoreEA.SchemaInfo;
using SSCEViewer_UnitTest;
namespace SqlCeViewer_UnitTest
{
    
    
    /// <summary>
    ///This is a test class for WizardManagerTest and is intended
    ///to contain all WizardManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WizardManagerTest
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


        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void DocDB_SSCE_XML()
        {
            WizardManager target = new WizardManager(); // TODO: Initialize to an appropriate value
            target.myDoc = new DocDbObject();
            target.myDoc.CurExportor = new XmlExportor();
            target.myDoc.TargetFile = "C:\\testOutputDBFile_FromSqlce.xml";
            target.myDoc.SourceDbType = CoreEA.CoreE.UsedDatabaseType.SqlCE35;
            target.myDoc.LoginInfo = new LoginInfo_SSCE()
            {
                DbName = GlobalInfo.SqlCE_TestFile,
            };
            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            srcHandle.Open(target.myDoc.LoginInfo);
            string usedTable = string.Empty;
            if (srcHandle.IsOpened)
            {
                usedTable = srcHandle.GetTableListInDatabase()[0];
            }
            //here we just test the first table
            BaseTableSchema schema = srcHandle.GetTableSchemaInfoObject(usedTable);

            List<DBObject> testData = new List<DBObject>();
            foreach (var item in schema.Columns)
            {
                testData.Add(new DBObject()
                {
                    TableName = schema.TableName,
                    ColumnName = item.ColumnName,
                    Category = "Column",
                    Description = "",
                    DbType = item.ColumnType,
                    Length = item.CharacterMaxLength,
                    OrdinaryPosition = item.OrdinalPosition,
                });
            }
 

            target.myDoc.DbObjectList = testData;
            
            bool result = target.Export();

            Assert.AreEqual(true,result);
        }


        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void DocDB_SSCE_EXCEL()
        {
            WizardManager target = new WizardManager(); // TODO: Initialize to an appropriate value
            target.myDoc = new DocDbObject();
            target.myDoc.CurExportor = new ExcelExportor();
            target.myDoc.TargetFile = "C:\\testOutputDBFile_FromSqlce.xls";
            target.myDoc.SourceDbType = CoreEA.CoreE.UsedDatabaseType.SqlCE35;
            target.myDoc.LoginInfo = new LoginInfo_SSCE()
            {
                DbName = GlobalInfo.SqlCE_TestFile,
            };
            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            srcHandle.Open(target.myDoc.LoginInfo);
            string usedTable = string.Empty;
            if (srcHandle.IsOpened)
            {
                usedTable = srcHandle.GetTableListInDatabase()[0];
            }
            //here we just test the first table
            BaseTableSchema schema = srcHandle.GetTableSchemaInfoObject(usedTable);

            List<DBObject> testData = new List<DBObject>();
                 foreach (var item in schema.Columns)
            {
                testData.Add(new DBObject()
                {
                    TableName = schema.TableName,
                    ColumnName = item.ColumnName,
                    Category = "Column",
                    Description = "",
                    DbType = item.ColumnType,
                    Length = item.CharacterMaxLength,
                    OrdinaryPosition = item.OrdinalPosition,
                });
            }

            target.myDoc.DbObjectList = testData;

            bool result = target.Export();

            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void DocDB_SqlServer_XML()
        {
            WizardManager target = new WizardManager(); // TODO: Initialize to an appropriate value
            target.myDoc = new DocDbObject();
            target.myDoc.CurExportor = new XmlExportor();
            target.myDoc.TargetFile = "C:\\testOutputDBFile_FromSqlServer.xml";
            target.myDoc.SourceDbType = CoreEA.CoreE.UsedDatabaseType.SqlServer;

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
            string usedTable = tableList[0];

            BaseTableSchema schema= srcHandle.GetTableSchemaInfoObject(usedTable);

            List<DBObject> testData = new List<DBObject>();
            testData.Add(new DBObject()
            {
                TableName = schema.TableName,
                ColumnName = schema.Columns[0].ColumnName,
                Category = "Tets1",
                DbType = schema.Columns[0].ColumnType,
                Length = schema.Columns[0].CharacterMaxLength,
                OrdinaryPosition = schema.Columns[0].OrdinalPosition,
            });

            target.myDoc.DbObjectList = testData;

            bool result = target.Export();

            Assert.IsTrue(result);
        }


        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void DocDB_MYSQL_XML()
        {
            WizardManager target = new WizardManager(); // TODO: Initialize to an appropriate value
            target.myDoc = new DocDbObject();
            target.myDoc.CurExportor = new XmlExportor();
            target.myDoc.TargetFile = "C:\\testOutputDBFile_FromMySql.xml";
            target.myDoc.SourceDbType = CoreEA.CoreE.UsedDatabaseType.MySql;
            target.myDoc.LoginInfo = new LoginInfo_MySql()
            {
                Server = "localhost",
                Username = "root",
                Pwd = "noway",
                Database = "test",
            };

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
            srcHandle.Open(target.myDoc.LoginInfo);
            string usedTable = string.Empty;
            if (srcHandle.IsOpened)
            {
                usedTable = srcHandle.GetTableListInDatabase()[0];
            }
            BaseTableSchema schema = srcHandle.GetTableSchemaInfoObject(usedTable);

            List<DBObject> testData = new List<DBObject>();
            testData.Add(new DBObject()
            {
                TableName = schema.TableName,
                ColumnName = schema.Columns[0].ColumnName,
                Category = "Tets1",
                Description = "Test1",
                DbType = schema.Columns[0].ColumnType,
                Length = schema.Columns[0].CharacterMaxLength,
                OrdinaryPosition = schema.Columns[0].OrdinalPosition,
            });

            testData.Add(new DBObject()
            {
                TableName = schema.TableName,
                ColumnName = schema.Columns[1].ColumnName,
                Category = "Tets2",
                DbType = schema.Columns[1].ColumnType,
                Length = schema.Columns[1].CharacterMaxLength,
                OrdinaryPosition = schema.Columns[1].OrdinalPosition,
            });

            target.myDoc.DbObjectList = testData;

            bool result = target.Export();

            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void DocDB_SQLITE_XML()
        {
            WizardManager target = new WizardManager(); // TODO: Initialize to an appropriate value
            target.myDoc = new DocDbObject();
            target.myDoc.CurExportor = new XmlExportor();
            target.myDoc.TargetFile = "C:\\testOutputDBFile_FromSqlite.xml";
            target.myDoc.SourceDbType = CoreEA.CoreE.UsedDatabaseType.Sqlite;
            target.myDoc.LoginInfo = new LoginInfo_Sqlite()
            {
                DbFile = GlobalInfo.Sqlite_TestFile,
                IsUnicode = true 
            };
            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;
            srcHandle.Open(target.myDoc.LoginInfo);
            string usedTable = string.Empty;
            if (srcHandle.IsOpened)
            {
                usedTable = srcHandle.GetTableListInDatabase()[0];
            }
            BaseTableSchema schema = srcHandle.GetTableSchemaInfoObject(usedTable);

            List<DBObject> testData = new List<DBObject>();
            foreach (var item in schema.Columns)
            {
                testData.Add(new DBObject()
                {
                    TableName = schema.TableName,
                    ColumnName = item.ColumnName,
                    Category = "Column",
                    Description = "",
                    DbType = item.ColumnType,
                    Length = item.CharacterMaxLength,
                    OrdinaryPosition = item.OrdinalPosition,
                });
            }
            
            target.myDoc.DbObjectList = testData;

            bool result = target.Export();
            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void DocDB_ACCESS_XML()
        {
            WizardManager target = new WizardManager(); // TODO: Initialize to an appropriate value
            target.myDoc = new DocDbObject();
            target.myDoc.CurExportor = new XmlExportor();
            target.myDoc.TargetFile = "C:\\testOutputDBFile_FromAccess.xml";
            target.myDoc.SourceDbType = CoreEA.CoreE.UsedDatabaseType.OleDb;
            target.myDoc.LoginInfo = new LoginInfo_Oledb()
            {
                Database =GlobalInfo.Access_TestFile,
            };

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.OleDb).X_Handler;
            srcHandle.Open(target.myDoc.LoginInfo);
            string usedTable = string.Empty;
            if (srcHandle.IsOpened)
            {
                usedTable = srcHandle.GetTableListInDatabase()[0];
            }
            BaseTableSchema schema = srcHandle.GetTableSchemaInfoObject(usedTable);

            List<DBObject> testData = new List<DBObject>();
            foreach (var item in schema.Columns)
            {
                testData.Add(new DBObject()
                {
                    TableName = schema.TableName,
                    ColumnName = item.ColumnName,
                    Category = "Column",
                    Description = "",
                    DbType = item.ColumnType,
                    Length = item.CharacterMaxLength,
                    OrdinaryPosition = item.OrdinalPosition,
                });
            }
            target.myDoc.DbObjectList = testData;

            bool result = target.Export();

            Assert.IsTrue(result);
        }


    }
}
