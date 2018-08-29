using DBStudio.Oracle;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SSCEViewer_UnitTest
{
    
    
    /// <summary>
    ///This is a test class for OracleEntryTest and is intended
    ///to contain all OracleEntryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OracleEntryTest
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
        ///A test for OracleEntry Constructor
        ///</summary>
        [TestMethod()]
        public void TestConnectToOracle()
        {
            string hostname = "10.204.2.142";
            string sid = "MyOracle";
            string username = "system";
            string pwd = "111111";

            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Oracle).X_Handler;
            core.Open(new CoreEA.LoginInfo.LoginInfo_Oracle()
            {
                HostName=hostname,
                SID=sid,
                Username=username,
                Password=pwd,
                Port=1521,
            });

            Assert.IsTrue(core.IsOpened);
        }
    }
}
