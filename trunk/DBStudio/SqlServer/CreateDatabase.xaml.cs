using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBStudio.BaseUI;
using ETL;
using Microsoft.Win32;
using DBStudio.GlobalDefine;
using CoreEA.LoginInfo;

namespace DBStudio.SqlServer
{
    /// <summary>
    /// Interaction logic for CreateDatabase.xaml
    /// </summary>
    public partial class CreateDatabase : BaseFadeDialog
    {
        static RoutedUICommand CreateCmd = new RoutedUICommand();

        public CreateDatabase()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CreateDatabase_Loaded);
        }

        void CreateDatabase_Loaded(object sender, RoutedEventArgs e)
        {
            this.CommandBindings.Add(new CommandBinding(CreateCmd, CreateCmd_Executed, CreateCmd_CanExecuted));
            butCreate.Command = CreateCmd;

        }
        void CreateCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {


                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;

                LoginInfo_SqlServer info = new LoginInfo_SqlServer()
                {
                    X_Server = Args.Server,
                    X_Database = Args.DbName,
                    X_Pwd = Args.PWD,
                    X_UserName = Args.UID,
                    X_Port = Args.Port.ToString(),
                    X_CurDbConnectionMode = Args.CurType.CurConnMode,
                    IsTrustedConn = Args.IsTrust,
                };
                App.MainEngineer.Open(info);
                if (App.MainEngineer.IsOpened)
                {
                    //populate this object to create database ,not open database
                    LoginInfo_SqlServer info2 = new LoginInfo_SqlServer();
                    info2.CreateDatabaseObject = new CoreEA.InfrastructureInfo.BaseCreateDbObject()
                    {
                        DbLocation = txtDbLocation.Text,
                        DbLogFileLocation = txtDbLogLocation.Text,
                        DbName = txtDbName.Text,
                        InitSize = (uint)txtSize.Value,
                        FileGrowth = (uint)txtGrowth.Value,
                    };

                    if (App.MainEngineer.CreateDatabase(info2))
                    {
                        "CreateDatabaseOkMsg".GetFromResourece().Show();
                    }
                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
                App.ResetMainEngineer();
            }
        }

        void CreateCmd_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtDbLocation.Text.IsEmpty()||txtDbLogLocation.Text.IsEmpty()
                ||txtDbName.Text.IsEmpty()
                )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void butSelectLocation_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = MyGlobal.CreateSqlDatabase_FILE_FILTER;

            if ((bool)sf.ShowDialog())
            {
                txtDbLocation.Text = sf.FileName;

                txtDbLogLocation.Text = txtDbLocation.Text.Replace(".mdf", ".ldf");
            }
        }

        private void butDbLogLocation_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = MyGlobal.CreateSqlLogDatabase_FILE_FILTER;

            if ((bool)sf.ShowDialog())
            {
                txtDbLogLocation.Text = sf.FileName;
            }
        }

        public WPFCommonControl.SqlServerLoginControl.X_CollectionData Args { get; set; }
    }
}
