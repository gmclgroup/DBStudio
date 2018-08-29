using DBStudio.SqlCE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Diagnostics;
using SSCEViewer_UnitTest;
using CoreEA.LoginInfo;
using CoreEA.SchemaInfo;
using System;
using XLCS.Sqlce;
using System.Data.SqlServerCe;
using DBStudio.GlobalDefine;
using System.Collections.ObjectModel;

namespace UnitTest
{


    /// <summary>
    ///This is a test class for CEEntryTest and is intended
    ///to contain all CEEntryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CEEntryTest
    {



        /// <summary>
        ///  The AddColumnToTable method execute the sql command failure ,
        ///  But if execute the sql command directly in SSDM it will successful .
        ///  Skip this error
        /// </summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void TestSqlCE_AddColumn()
        {

            CoreEA.ICoreEAHander srcHandle = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            srcHandle.Open(new LoginInfo_SSCE() { DbName = GlobalInfo.SqlCE_TestFile });
            int columnCount = 0;
            int nowColumnCount = 0;
            if (srcHandle.IsOpened)
            {
                columnCount = srcHandle.GetColumnNameListFromTable(GlobalInfo.SqlCE_TestTable).Count;

                BaseColumnSchema columnSchema = new BaseColumnSchema();

                columnSchema.ColumnName = "TestColumn11";
                columnSchema.ColumnType = "nvarchar";
                columnSchema.CharacterMaxLength = 50;

                //columnSchema.IsNullable = true;
                columnSchema.IsNullable = false;

                bool ret = srcHandle.AddColumnToTable(GlobalInfo.SqlCE_TestTable, columnSchema);

                //Should create column ok
                Assert.AreEqual(true, ret);
                nowColumnCount = srcHandle.GetColumnNameListFromTable(GlobalInfo.SqlCE_TestTable).Count;

            }
            srcHandle.Close();
            
            Assert.AreEqual(columnCount, nowColumnCount - 1);

        }


        /// <summary>
        ///A test for butOpen_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void LoginInSSCE_Test()
        {
            bool isCanLogIn = false;
            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            core.Open(new LoginInfo_SSCE()
            {
                DbName = GlobalInfo.SqlCE_TestFile,

            });
            if (core.IsOpened)
            {
                isCanLogIn = true;
            }
            core.Close();
            Assert.AreEqual(true, isCanLogIn);
        }

        /// <summary>
        ///A test for butDetectFileVersion_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void DetectSSCEFileVersion()
        {
            string result = string.Empty;

            result = String.Format("Version : Sql Ce {0}", UndocumentedFunctions.GetCeVersion(GlobalInfo.SqlCE_TestFile));
            Assert.AreNotEqual(String.Empty, result);
        }

        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void CreateSSCEFile()
        {
            if (System.IO.File.Exists(GlobalInfo.SqlCE_NewTestFile))
            {
                System.IO.File.Delete(GlobalInfo.SqlCE_NewTestFile);
            }

            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;

            bool result=core.CreateDatabase(new LoginInfo_SSCE()
            {
                DbName = GlobalInfo.SqlCE_NewTestFile
            });
            core.Close();
            Assert.AreEqual(true, result);
        }


        /// <summary>
        ///A test for butChange_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SeasonStarDatabaseManagement.exe")]
        public void butChangeSSCEPwd_ClickTest()
        {

            string oldPwd = string.Empty;
            string newPwd = "1111";
            //Change old pwd to new pwd
            using (SqlCeEngine eg = new SqlCeEngine())
            {
                eg.LocalConnectionString = String.Format("Data Source={0};Password={1}", GlobalInfo.SqlCE_TestFile, oldPwd);
                eg.Compact(string.Format("Data Source={0};Password={1}", GlobalInfo.SqlCE_TestFile, newPwd));
            }

            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            core.Open(new LoginInfo_SSCE()
            {
                DbName = GlobalInfo.SqlCE_TestFile,
                Pwd = newPwd,
            });

            bool status1 = core.IsOpened;
            if (core.IsOpened)
            {
                core.Close();
                core.Dispose();
            }
            //change new pwd to old pwd 
            using (SqlCeEngine eg = new SqlCeEngine())
            {
                eg.LocalConnectionString = String.Format("Data Source={0};Password={1}", GlobalInfo.SqlCE_TestFile, newPwd);
                eg.Compact(string.Format("Data Source={0};Password={1}", GlobalInfo.SqlCE_TestFile, oldPwd));
            }

            CoreEA.ICoreEAHander core2 = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            core2.Open(new LoginInfo_SSCE()
            {
                DbName = GlobalInfo.SqlCE_TestFile,
                Pwd = oldPwd,
            });
            bool status2 = core2.IsOpened;
            if (core2.IsOpened)
            {
                core2.Close();
                core2.Dispose();
            }

            Assert.AreEqual(status1, status2);

        }


        [TestMethod()]
        public void CreateTableWithSchemaInfo()
        {
            ObservableCollection<CreateTableBindingArgs_SqlCe> bindingArgsList =
    new ObservableCollection<CreateTableBindingArgs_SqlCe>();
            //Add test object
            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "ID",
                ColumnType = System.Data.SqlDbType.Int,
                IsPK = true,
                IsUnique = true,
            });

            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "CustomerID",
                ColumnType = System.Data.SqlDbType.Int,
                IsPK = true,
                IsUnique = true,
            });

            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "T1",
                ColumnType = System.Data.SqlDbType.Bit,
            });
            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "T2",
                ColumnType = System.Data.SqlDbType.Char,
                ColumnLength = 5,
            });

            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "T3",
                ColumnType = System.Data.SqlDbType.TinyInt,
            });

            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "Name",
                ColumnType = System.Data.SqlDbType.NVarChar,
                ColumnLength = 500,
            });

            bindingArgsList.Add(new CreateTableBindingArgs_SqlCe()
            {
                ColumnName = "Comment",
                ColumnType = System.Data.SqlDbType.DateTime,
                ColumnLength = 8,
            });

            //Generate table base schema info
            BaseTableSchema schemaInfo = new BaseTableSchema();

            string t = new Random().Next(1000).ToString();

            string tableName = string.Format("MyTestDataTable{0}", t);
            schemaInfo.TableName = tableName;

            foreach (var item in bindingArgsList)
            {
                if (item.IsPK)
                {
                    schemaInfo.PrimaryKey.Add(new BasePrimaryKeyInfo()
                    {
                        ColumnName = item.ColumnName,
                    });
                }

                schemaInfo.Columns.Add(new BaseColumnSchema()
                {
                    ColumnName = item.ColumnName,
                    ColumnType = item.ColumnType.ToString(),

                    IsNullable = item.AllowNulls,
                    CharacterMaxLength = item.ColumnLength,
                    IsIdentity = item.IsUnique,
                });
            }
            CoreEA.ICoreEAHander core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            core.Open(new LoginInfo_SSCE() { DbName = GlobalInfo.SqlCE_TestFile });

            bool result = core.CreateTable(schemaInfo);
            
            if (result)
            {
                if (core.GetTableListInDatabase().Contains(schemaInfo.TableName))
                {

                    Assert.IsTrue(true);
                }
            }
            else
            {
                Assert.IsTrue(false);
            }
            core.Close();
        }
    }
}
