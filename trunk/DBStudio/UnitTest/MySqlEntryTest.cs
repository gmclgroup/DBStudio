using DBStudio.MySql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFCommonControl;
using SSCEViewer_UnitTest;

namespace SqlCeViewer_UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MySqlEntryTest and is intended
    ///to contain all MySqlEntryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MySqlEntryTest
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
        ///A test for LoginInServer
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void LoginInMySql_Test()
        {
            testContextInstance.RecordTestMethod();

            MySqlEntry_Accessor target = new MySqlEntry_Accessor(); // TODO: Initialize to an appropriate value

            //SqlServerLoginControl.X_CollectionData data = new SqlServerLoginControl.X_CollectionData()
            //{
            //    DbName = "MySql",
            //    CurType = new SqlServerLoginControl.X_LoginDbParas()
            //    {
            //        CurConnMode = CoreEA.Args.CurDbServerConnMode.Standard,
            //        Name = "MySql",
            //    },

            //    PWD = "noway",
            //    Server = "localhost",
            //    UID = "root",
            //};

//            target.txtDbName = "localhost";
            target.passwordBox1.Password = "noway";
            target.txtUsername.Text = "root";
            target.txtServername.Text = "localhost";

            target.ConnectCmd_Executed(null, null);

            TestGlobalExtension.RecordTestMethod();
        }
    }
}
