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
using System.IO;
using DBStudio.GlobalDefine;
using Microsoft.Win32;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectTargetSqliteDB.xaml
    /// </summary>
    public partial class SelectTargetSqliteDB : UserControl, ISrcControl
    {
        public SelectTargetSqliteDB()
        {
            InitializeComponent();
        }

        public CoreEA.LoginInfo.BaseLoginInfo X_Result
        {
            get {
                LoginInfo_Sqlite info = new LoginInfo_Sqlite()
                {
                    DbFile = txtTargetSqliteFile.Text,
                };

                return info;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
                bool ret = false;
                if (!string.IsNullOrEmpty(txtTargetSqliteFile.Text))
                {
                    if (File.Exists(txtTargetSqliteFile.Text))
                    {
                        ret = true;
                    }
                    else
                    {
                        CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;
                        if (tempHandler.CreateDatabase(new LoginInfo_Sqlite() {
                            DbFile = txtTargetSqliteFile.Text, 
                            
                            }))
                        {
                            ret = true;
                        }
                        
                    }
                }

                return ret;
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

        private void butSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.SQLCE_FILE_FILTER;
            sf.CheckFileExists = false;
            if (sf.ShowDialog() == true)
            {
                this.txtTargetSqliteFile.Text = sf.FileName;
            }
        }
    }
}
