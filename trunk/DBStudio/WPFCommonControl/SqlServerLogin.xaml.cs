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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using CoreEA.Args;
using CoreEA.LoginInfo;
using Microsoft.Win32;
using System.Diagnostics;

namespace WPFCommonControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    [Description("Do not refactor this control to MPL or others, because it rely on XCoreEA")]
    public partial class SqlServerLoginControl : UserControl
    {
        private const string ATTACH_FILE = "Attach File";

        public enum CurUIMode
        {
            ForSqlServer,
            ForMySql,
        }

        public CurUIMode X_CurUIMode { get; set; }

        public void SetServerName(string servername)
        {
            cmbServername.Text = servername;
        }

        public void SetMySqlMode()
        {
            cmbConnMode.SelectedIndex = 5;
            X_CurUIMode = CurUIMode.ForMySql;
            txtPort.IsEnabled = false;
            cmbConnMode.IsEditable=false;
            cmbConnMode.IsReadOnly = true;
            cmbConnMode.IsEnabled = false;  
            chkIsTrustedConn.Visibility = Visibility.Collapsed;
        }

        public class X_CollectionData
        {
            public string Server { get; set; }
            public string DbName { get; set; }
            public string TableName { get; set; }
            public string UID { get; set; }
            public string PWD { get; set; }
            public int Port { get; set; }
            public bool IsTrust { get; set; }
            public X_LoginDbParas CurType { get; set; }
            public bool IsUseFileDirectly
            {
                get
                {
                    if (string.IsNullOrEmpty(DbFileLocation))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            public string DbFileLocation { get; set; }
        }

        /// <summary>
        /// Return Object 
        /// </summary>
        public X_CollectionData X_Result
        {
            get
            {
                int port;
                int.TryParse(txtPort.Text, out port);
                if (port == 0) port = 1433;

                X_CollectionData resultData = new X_CollectionData()
                {
                    Server = cmbServername.Text,
                    DbName = cmbDbName.Text,
                    TableName = string.Empty,
                    UID = txtUsername.Text,
                    PWD = passwordBox1.Password,
                    IsTrust = (bool)chkIsTrustedConn.IsChecked,
                    Port=port,
                    CurType = cmbConnMode.SelectedItem as X_LoginDbParas,
                    DbFileLocation=txtDbFileLocation.Text,
                };

                return resultData;
            }
        }

        /// <summary>
        /// Parasmeters when CoreEA using
        /// </summary>
        public class X_LoginDbParas
        {
            public string Name { get; set; }
            public CurDbServerConnMode CurConnMode { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }

        public SqlServerLoginControl()
        {
            InitializeComponent();

            //If in design mode ,then do not load following codes
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            InitSelf();
#if DEBUG
            TestCode();
            
#endif
            //Default mode 
            X_CurUIMode = CurUIMode.ForSqlServer;
        }

        private void TestCode()
        {
            //cmbServername.Text = "222.191.251.55";
            //cmbDbName.Text = "cyma_sap";
            //txtUsername.Text = "cyma_sap_f";

        }

        public void SetPwd(string pwd)
        {
            passwordBox1.Password = pwd;
        }

        private void InitSelf()
        {

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.Local,
                Name = "Local"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.SqlServer2000,
                Name = "Sql Server 2000"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.OleDb,
                Name = "Ole DB"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.SqlServer2005Express,
                Name = "Sql Server 2005 Express"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.SqlServer2008Express,
                Name = "Sql Server 2008 Express"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.Standard,
                Name = "Standard"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.SqlServer2005,
                Name = "Sql Server2005 (MAR)"
            });

            this.cmbConnMode.Items.Add(new X_LoginDbParas()
            {
                CurConnMode = CurDbServerConnMode.AttachFile,
                Name = ATTACH_FILE
            });

            this.cmbConnMode.SelectedIndex = 6;
        }

        private void butGetDbList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbServername.Text))
            {
                cmbServername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsername.Focus();
                return;
            }

            CoreEA.ICoreEAHander core=null;

            switch (X_CurUIMode)
            {
                case CurUIMode.ForSqlServer:
                    core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;
                    break;
                case CurUIMode.ForMySql:
                    core = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;
                    break;
            }

            LoginInfo_SqlServer info=new LoginInfo_SqlServer();
            X_LoginDbParas args = cmbConnMode.SelectedItem as X_LoginDbParas;

            if (args.Name == ATTACH_FILE)
            {
                info = new LoginInfo_SqlServer()
                {
                    X_Server=cmbServername.Text,
                    AttchFile = txtDbFileLocation.Text,
                    //attach file mode need trust connection
                    IsTrustedConn=true,
                };
            }
            else
            {

                info = new LoginInfo_SqlServer()
                    {
                        X_Server = cmbServername.Text,
                        X_UserName = txtUsername.Text,
                        X_Pwd = passwordBox1.Password,
                        IsTrustedConn = (bool)chkIsTrustedConn.IsChecked,
                    };
            }

            if (cmbConnMode.SelectedItem != null)
            {
                info.X_CurDbConnectionMode = ((X_LoginDbParas)cmbConnMode.SelectedItem).CurConnMode;
            }

            try
            {
                core.Open(info);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }

            if (core.IsOpened)
            {
                cmbDbName.Items.Clear();

                foreach (var item in core.GetDatabaseList())
                {
                    cmbDbName.Items.Add(item);
                }

                if (cmbDbName.Items.Count > 1)
                {
                    cmbDbName.SelectedIndex = 0;
                }

                core.Close();
                core.Dispose();
            }

        }

        public void SetServerNameList(List<string> serverList)
        {
            serverList.ForEach((server) =>
                {
                    cmbServername.Items.Add(server);
                });

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if((cmbConnMode!=null)&&(cmbConnMode.SelectedItem != null))
            {
                X_LoginDbParas args = cmbConnMode.SelectedItem as X_LoginDbParas;

                if (args.Name == ATTACH_FILE)
                {
                    e.CanExecute = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(cmbServername.Text))
                    {
                        e.CanExecute = false;
                        return;
                    }
                    if (string.IsNullOrEmpty(txtUsername.Text))
                    {
                        e.CanExecute = false;
                        return;
                    }

                    e.CanExecute = true;
                }
            }

        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            butGetDbList_Click(null, null);
        }

        private void passwordBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmbDbName.Items.Count < 1)
            {
                butGetDbList_Click(null, null);
            }
        }

        private void butSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "mdf (*.mdf)|*.mdf|All Files(*.*)|*.*";
            if ((bool)of.ShowDialog())
            {
                txtDbFileLocation.Text = of.FileName;
            }
        }

        private void cmbConnMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            X_LoginDbParas args = cmbConnMode.SelectedItem as X_LoginDbParas;

            if (args.Name == ATTACH_FILE)
            {
                panelDatabaseFile.Visibility = System.Windows.Visibility.Visible;
                //paneServername.Visibility = System.Windows.Visibility.Collapsed;
                panelUsername.Visibility = System.Windows.Visibility.Collapsed;
                panelPassword.Visibility = System.Windows.Visibility.Collapsed;
                panelDatabase.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                panelDatabaseFile.Visibility = System.Windows.Visibility.Collapsed;
                panelUsername.Visibility = System.Windows.Visibility.Visible;
                panelPassword.Visibility = System.Windows.Visibility.Visible;
                panelDatabase.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
