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
using CoreEA.LoginInfo;
using ETL;
using DBStudio.DocumentingDB;
using System.Diagnostics;


namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectTargetMySql.xaml
    /// </summary>
    public partial class SelectTargetMySql : UserControl, IStep, ISrcControl
    {
        public SelectTargetMySql()
        {
            InitializeComponent();
        }

        public CoreEA.LoginInfo.BaseLoginInfo X_Result
        {
            get
            {
                LoginInfo_MySql info = new LoginInfo_MySql();

                info.Database = txtDbName.Text;
                info.Pwd = passwordBox1.Password;
                info.Server = txtServername.Text;
                info.Username = txtUsername.Text;

                return info;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
 
                //if (string.IsNullOrEmpty(txtDbName.Text))
                //{
                //    return false;
                //}
                if (string.IsNullOrEmpty(txtServername.Text))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(txtUsername.Text))
                {
                    return false;
                }

                return true;
            }
        }

        public void X_ShowErrorTips()
        {
            throw new NotImplementedException();
        }

        public bool X_IsSourceHandler
        {
            get { return false; }
        }

        private void butEnumDbs_Click(object sender, RoutedEventArgs e)
        {
            txtDbName.DataContext = null;
            try
            {
                CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;

                tempHandler.Open(
                    new LoginInfo_MySql()
                    {
                        Pwd = passwordBox1.Password,
                        Server = txtServername.Text,
                        Username = txtUsername.Text,
                    }
                    );
                if (tempHandler.IsOpened)
                {
                    List<string> dbList = tempHandler.GetDatabaseList();

                    txtDbName.DataContext = dbList;

                    txtDbName.SelectedIndex = 0;
                }
                else
                {
                    throw new Exception("Connection not correct");
                }
            }
            catch (Exception ee)
            {
                ee.Message.Show();
            }
        }

        #region IStep Members

        public object MyDocDataContext
        {
            get;
            set;
        }

        public bool CanLeave()
        {
            return ((ISrcControl)this).X_CanForwardToNext;
        }

        public void Leave()
        {
            DocDbObject myInfo = (DocDbObject)MyDocDataContext;
            myInfo.LoginInfo = ((LoginInfo_MySql)((ISrcControl)this).X_Result);

            try
            {
                CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.MySql).X_Handler;

                tempHandler.Open(
                    new LoginInfo_MySql()
                    {
                        Database = txtDbName.Text,
                        Pwd = passwordBox1.Password,
                        Server = txtServername.Text,
                        Username = txtUsername.Text,
                    }
                    );
                if (tempHandler.IsOpened)
                {

                }
                else
                {
                    throw new Exception("Connection not correct");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }



        public object Result
        {
            get
            {
                return null;
            }
        }
        public bool IsSource
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
