using DbQueryBrowser.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using DbQueryBrowser;

namespace TestProject
{
    /// <summary>
    ///This is a test class for MainTest and is intended
    ///to contain all MainTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MainTest
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
        ///A test for Do_SelectCmd
        ///</summary>
        [TestMethod()]
        [DeploymentItem("DbQueryBrowser.exe")]
        public void Do_SelectCmdTest()
        {
            Main_Accessor target = new Main_Accessor(); // TODO: Initialize to an appropriate value

            DbCommand cmd = target.X_MainEnginer.DbHandler.GetSqlStringCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES");
            
            target.Do_SelectCmd(cmd);

            Assert.AreSame(true, true);

        }
    }
}
