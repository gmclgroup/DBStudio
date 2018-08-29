using CoreEA.Effiproz;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.EffiProz;
using System.Data;
using System.Diagnostics;

namespace SSCEViewer_UnitTest
{
    
    
    /// <summary>
    ///This is a test class for EffiprozRobotTest and is intended
    ///to contain all EffiprozRobotTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EffiprozRobotTest
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
        ///A test for GetTableListInDatabase
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void Effiproz_TestBasicFunctions()
        {
            string databaseName = string.Empty; // TODO: Initialize to an appropriate value

            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Effiproz).X_Handler;
            if (System.IO.File.Exists(GlobalInfo.Effiproz_NewTestFile + ".properties"))
            {
                System.IO.File.Delete(GlobalInfo.Effiproz_NewTestFile + ".properties");
            }
            if (System.IO.File.Exists(GlobalInfo.Effiproz_NewTestFile + ".log"))
            {
                System.IO.File.Delete(GlobalInfo.Effiproz_NewTestFile + ".log");
            }
            if (System.IO.File.Exists(GlobalInfo.Effiproz_NewTestFile + ".lck"))
            {
                System.IO.File.Delete(GlobalInfo.Effiproz_NewTestFile + ".lck");
            }
            if (System.IO.File.Exists(GlobalInfo.Effiproz_NewTestFile + ".script"))
            {
                System.IO.File.Delete(GlobalInfo.Effiproz_NewTestFile + ".script");
            }
            try
            {
                core.Open(new CoreEA.LoginInfo.LoginInfo_Effiproz()
                {
                    InitialCatalog = GlobalInfo.Effiproz_NewTestFile,
                    Username = "sa",
                    Password = "",
                    
                    DBConnectionType = CoreEA.LoginInfo.ConnectionType.File,
                });

                if (core.IsOpened)
                {
                    List<string> tableList = core.GetTableListInDatabase();
                    string sql = "CREATE TABLE TESTAAFF(ID INT,ID2 INT, NAME VARCHAR(100),ID3 INT,ID4 INT DEFAULT 5, PRIMARY KEY(ID), UNIQUE (ID2), UNIQUE (ID3,ID4));";
                    DbCommand cmd = core.GetNewStringCommand(sql);
                    int count=cmd.ExecuteNonQuery();
                    Assert.AreEqual(count, 0);

                    tableList = core.GetTableListInDatabase();

                    Assert.IsTrue(tableList.Count > 0);
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
                throw;
            }
            finally
            {
                if (core.IsOpened)
                {
                    core.Close();
                   
                }
                core.Dispose();
            }
        }


        [TestMethod]
        public void ADOCommandBuilderTest1()
        {
            string connString = @"Connection Type=File ; Initial Catalog=D:\T2; User=sa; Password=;";

            string sql = "CREATE TABLE TEST(ID INT,ID2 INT, NAME VARCHAR(100),ID3 INT,ID4 INT DEFAULT 5, PRIMARY KEY(ID), UNIQUE (ID2), UNIQUE (ID3,ID4));";
            using (EfzConnection conn = new EfzConnection(connString))
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = sql;

                conn.Open();
                int count = command.ExecuteNonQuery();
                //command.CommandText = "INSERT INTO TEST(ID , ID2, NAME ) VALUES(1, 100,'irantha'); INSERT INTO TEST(ID ,ID2, NAME ) VALUES(2, 500,'subash');";
                //count = command.ExecuteNonQuery();
                //Assert.AreEqual(count, 1);

                DataTable tb = conn.GetSchema("Tables", new string[] { null, null, null, "TABLE" });
                Assert.IsNotNull(tb);
                string tableName=tb.Rows[0]["Table_Name"].ToString();

                Assert.AreEqual(tableName, "TEST");
                Assert.AreEqual(1, tb.Rows.Count);

                tb = conn.GetSchema("Columns", new string[] { null, "PUBLIC", "TEST", "ID" });
                Assert.AreEqual(1, tb.Rows.Count);
                tb = conn.GetSchema("Columns", new string[] { null, "PUBLIC", "TEST", "NAME" });
                Assert.AreEqual(1, tb.Rows.Count);

                tb = conn.GetSchema("PRIMARYKEYS", new string[] { null, "PUBLIC", "TEST" });
                Assert.AreEqual(1, tb.Rows.Count);

                tb = conn.GetSchema("INDEXES", new string[] { null, "PUBLIC", "TEST" });
                Assert.AreEqual(3, tb.Rows.Count);

                tb = conn.GetSchema("INDEXES", new string[] { null, "PUBLIC", "TEST", null, "true" });
                Assert.AreEqual(3, tb.Rows.Count);

                tb = conn.GetSchema("SCHEMAS");
                Assert.AreEqual(3, tb.Rows.Count);

                tb = conn.GetSchema("TYPES");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("DataTypes");
                Assert.IsTrue(tb.Rows.Count > 0);
                //PrintDT(tb);

                command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM TEST";
                DbDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable columnSchemaTbl = reader.GetSchemaTable();
                Assert.IsTrue((bool)columnSchemaTbl.Rows[0]["IsKey"]);
                Assert.IsTrue((bool)columnSchemaTbl.Rows[0]["IsUnique"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[1]["IsKey"]);
                Assert.IsTrue((bool)columnSchemaTbl.Rows[1]["IsUnique"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[2]["IsUnique"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[3]["IsUnique"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[2]["IsKey"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[3]["IsKey"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[4]["IsUnique"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[4]["IsKey"]);
                Assert.AreEqual("5", columnSchemaTbl.Rows[4][SchemaTableOptionalColumn.DefaultValue]);


                command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM TEST";
                reader = command.ExecuteReader();
                columnSchemaTbl = reader.GetSchemaTable();
                Assert.IsFalse((bool)columnSchemaTbl.Rows[0]["IsKey"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[1]["IsKey"]);
                Assert.IsFalse((bool)columnSchemaTbl.Rows[1]["IsUnique"]);

                tb = new DataTable("Test");
                tb.Load(reader);

                tb = conn.GetSchema("METADATACOLLECTIONS");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("CharacterSets");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("CheckConstraints");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("Collations");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("Domains");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("ColumnPrivilages");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("PROCEDURES");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("PROCEDUREPARAMETERS");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("TABLEPRIVILEGES");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("TableConstraints");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("TRIGGERS", new string[] { });
                Assert.IsTrue(tb.Rows.Count == 0);

                tb = conn.GetSchema("VIEWS");
                int oldViewCount = tb.Rows.Count;

                command = conn.CreateCommand();
                command.CommandText = "CREATE VIEW v1 AS SELECT ID,NAME FROM TEST";
                command.ExecuteNonQuery();

                tb = conn.GetSchema("VIEWS");
                Assert.IsTrue(tb.Rows.Count == oldViewCount + 1);

                tb = conn.GetSchema("ViewColumns");
                Assert.IsTrue(tb.Rows.Count > 0);

                command = conn.CreateCommand();
                command.CommandText = "CREATE TABLE TEST2(IDD INT,IDD2 INT, NAME VARCHAR(100), FOREIGN KEY(IDD2)" +
                                                                             " REFERENCES TEST(ID));";
                command.ExecuteNonQuery();

                tb = conn.GetSchema("EXPORTEDKEYS");
                Assert.IsTrue(tb.Rows.Count == 1);

                tb = conn.GetSchema("IMPORTEDKEYS");
                Assert.IsTrue(tb.Rows.Count == 1);

                tb = conn.GetSchema("FOREIGNKEYS");
                Assert.IsTrue(tb.Rows.Count == 1);

                tb = conn.GetSchema("ForeignKeyColumns");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("RESERVEDWORDS");
                Assert.IsTrue(tb.Rows.Count > 0);

                tb = conn.GetSchema("RESTRICTIONS");
                Assert.IsTrue(tb.Rows.Count > 0);


                tb = conn.GetSchema("FUNCTIONS");

                tb = conn.GetSchema("FunctionParameters");


                // Assert.IsTrue(tb.Rows.Count> 0);

                // PrintDT(tb);

            }
        }
    }
}
