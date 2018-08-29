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
using DBStudio.DocumentingDB;
using CoreEA.LoginInfo;
using Microsoft.Win32;
using DBStudio.GlobalDefine;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectEffiproz.xaml
    /// </summary>
    public partial class SelectEffiproz : UserControl ,ISrcControl, IStep
    {

        /// <summary>
        /// Current Connection Type of combobox value
        /// change it carefully
        /// </summary>
        ConnectionType currentDBConnectionType;

        public SelectEffiproz()
        {
            InitializeComponent();
        }

        #region ISrcControl Members

        CoreEA.LoginInfo.BaseLoginInfo ISrcControl.X_Result
        {
            get
            {
                LoginInfo_Effiproz info = new LoginInfo_Effiproz()
                    {
                        InitialCatalog = DbFullName,
                        Username = txtUserName.Text,
                        Password = txtPwd.Password,
                        DBConnectionType=currentDBConnectionType,
                    };

                return info;
            }
        }

        bool ISrcControl.X_CanForwardToNext
        {
            get
            {
                bool ret = false;
                if (!string.IsNullOrEmpty(DbFullName))
                {
                    ret = true;
                }
                return ret;
            }
        }

        void ISrcControl.X_ShowErrorTips()
        {
            //labelWarning.Visibility = Visibility.Visible;
        }

        bool ISrcControl.X_IsSourceHandler
        {
            get
            {
                return true;
            }
        }

        #endregion


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
            myInfo.LoginInfo = ((LoginInfo_Effiproz)((ISrcControl)this).X_Result);

            try
            {
                CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Effiproz).X_Handler;

                tempHandler.Open(
                    new LoginInfo_Effiproz()
                    {
                        InitialCatalog =DbFullName,
                        Username=txtUserName.Text,
                        Password=txtPwd.Password,
                        DBConnectionType = currentDBConnectionType,
                    }
                    );
                if (tempHandler.IsOpened)
                {

                }
                else
                {
                    throw new Exception("can't open ");
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
                return true;
            }
        }
        #endregion

        public string DbFullName
        {
            get
            {

                string dbName = string.Empty;
                switch (currentDBConnectionType)
                {
                    case ConnectionType.File:
                        if (string.IsNullOrEmpty(txtDbLocation.Text))
                        {
                            return string.Empty;
                        }
                        string rootPath = System.IO.Path.GetDirectoryName(txtDbLocation.Text);
                        dbName = rootPath + "\\"+System.IO.Path.GetFileNameWithoutExtension(txtDbLocation.Text);
                        break;
                    case ConnectionType.Memory:
                        if (string.IsNullOrEmpty(txtDBName.Text))
                        {
                            return string.Empty;
                        }
                        dbName = txtDBName.Text;
                        break;
                    default:
                        throw new Exception("invalid mode");
                }
                return dbName;
            }
        }

        private void cmbDbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)e.AddedItems[0]).Content.ToString() == "File")
            {
                currentDBConnectionType = ConnectionType.File;
                if (panel1 != null)
                {
                    panel1.Visibility = System.Windows.Visibility.Visible;
                    panel2.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                currentDBConnectionType = ConnectionType.Memory;
                panel2.Visibility = System.Windows.Visibility.Visible;
                panel1.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void butSelectPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.Effiproz_FILE_FILTER;
            if ((bool)sf.ShowDialog())
            {

                txtDbLocation.Text = sf.FileName;
            }
        }
    }
}
