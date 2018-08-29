using CoreEA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for ICoreEAHanderTest and is intended
    ///to contain all ICoreEAHanderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ICoreEAHanderTest
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


        internal virtual ICoreEAHander CreateICoreEAHander()
        {
            // TODO: Instantiate an appropriate concrete class.
            ICoreEAHander target = new CoreE(CoreE.UsedDatabaseType.SqlServer).X_Handler;
            return target;
        }

        /// <summary>
        ///A test for GetTableListInDatabase
        ///</summary>
        [TestMethod()]
        public void GetTableListInDatabaseTest()
        {
            ICoreEAHander target = CreateICoreEAHander(); // TODO: Initialize to an appropriate value

            target.Open(new LoginDbInfo("LEOND\\SQLEXPRESS","MyDb","MyTable","LEOND\\Leon",""));

            string databaseName = "master"; // TODO: Initialize to an appropriate value
            List<string> expected = new List<string>(); // TODO: Initialize to an appropriate value
            
            List<string> actual;

            actual = target.GetTableListInDatabase(databaseName);

            Assert.AreEqual(expected, actual);
        }
    }
}
