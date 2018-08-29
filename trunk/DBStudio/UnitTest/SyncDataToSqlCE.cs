using DBStudio.SqlCE.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using SSCEViewer_UnitTest;
using System.Diagnostics;

namespace SqlCeViewer_UnitTest
{
    
    
    /// <summary>
    ///This is a test class for Excel2SDFTest and is intended
    ///to contain all Excel2SDFTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Excel2SDFTest
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
        ///A test for butSycn_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void SyncSqlServer2SSCE_ClickTest()
        {
            testContextInstance.RecordTestMethod();


            NewSqlServer2SSCE_Accessor target = new NewSqlServer2SSCE_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            RoutedEventArgs e = null; // TODO: Initialize to an appropriate value

            target.sqlServerLoginControl1.SetPwd("");

            target.butGetTableList_Click(sender, e);
            target.chkIsNeedCopyData.IsChecked = true;

            target.tableList.SelectedItems.Add(target.tableList.Items[0]);
            target.tableList.SelectedItems.Add(target.tableList.Items[1]);
            target.tableList.SelectedItems.Add(target.tableList.Items[2]);
            string t = new Random().Next(1000).ToString();

            target.txtTargetFile.Text = string.Format("C:\\TestResult_SyncSqlserver2SSCEFile-{0}.sdf", t);

            target.SyncSqlServer2SSCE(sender, e);

            TestGlobalExtension.RecordTestMethod();
            Assert.IsTrue(target.tableList.Items.Count > 0);
        }
        /// <summary>
        ///A test for Convert
        ///</summary>
        [TestMethod()]
        public void SyncOledb2SSCE_Test()
        {
            Debug.WriteLine(testContextInstance.TestName);

            Access2SDF_Accessor target = new Access2SDF_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            RoutedEventArgs e = null; // TODO: Initialize to an appropriate value

            target.txtSrcFile.Text = "C:\\TestAccessDb2003.mdb";

            string t = new Random().Next(1000).ToString();
            target.txtTargetFile.Text = string.Format("C:\\TestResult_SyncAccess2SSCEFile-{0}.sdf", t);



            target.SyncOledb2SSCE(sender, e);

            Assert.Inconclusive("A method that does not return a value cannot be verified.You can check the output for more failure info");
        }
        /// <summary>
        ///A test for butSync_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void butSync_ClickTest()
        {
            testContextInstance.RecordTestMethod();


            MySql2SDF_Accessor target = new MySql2SDF_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            RoutedEventArgs e = null; // TODO: Initialize to an appropriate value
            target.SyncMysql2SSCE(sender, e);

            TestGlobalExtension.RecordTestMethod();
        }
        /// <summary>
        ///A test for Convert
        ///</summary>
        [TestMethod()]
        public void SyncExcel2SSCE_Test()
        {
            testContextInstance.RecordTestMethod();


            Excel2SDF_Accessor target = new Excel2SDF_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            RoutedEventArgs e = null; // TODO: Initialize to an appropriate value

            target.txtSrcFile.Text = "C:\\TestData\\TestExcelData2003.xls";

            string t = new Random().Next(1000).ToString();
            target.txtTargetFile.Text = string.Format("C:\\TestResult_SyncExcel2SSCEFile-{0}.sdf", t);

            target.SyncExcel2SSCE(sender, e);

            TestGlobalExtension.RecordTestMethod();
        }


        /// <summary>
        ///A test for butSync_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void SyncCSV2SSCE_Test()
        {
            testContextInstance.RecordTestMethod();

            Csv2Sdf_Accessor target = new Csv2Sdf_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            RoutedEventArgs e = null; // TODO: Initialize to an appropriate value
            target.txtSrcFile.Text = "C:\\TestData\\TestCSV.csv";

            string t = new Random().Next(1000).ToString();
            target.txtTargetFile.Text = string.Format("C:\\TestResult_SyncCSV2SSCEFile-{0}.sdf", t);

            target.SyncCSV2SSCE(sender, e);

            TestGlobalExtension.RecordTestMethod();
        }
    }
}
