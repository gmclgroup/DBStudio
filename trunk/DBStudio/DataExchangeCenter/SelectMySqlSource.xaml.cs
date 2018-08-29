//=============================================================================
//    DBStudio
//    Copyright (C) 2006  ms44

//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation; either
//    version 2 of the License, or (at your option) any later version.

//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.

//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//    If you have any questions ,please contact me via 54715112@qq.com
//===============================================================================

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
using DBStudio.DocumentingDB;
using DBStudio.GlobalDefine;
using ETL;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectMySqlSource.xaml
    /// </summary>
    public partial class SelectMySqlSource : UserControl, ISrcControl, IStep
    {
        public bool X_IsSourceHandler
        {
            get
            {
                return true;
            }
        }

        public BaseLoginInfo X_Result
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
            labelWarning.Visibility = Visibility.Visible;
        }


        public SelectMySqlSource()
        {
            InitializeComponent();

#if DEBUG
            txtServername.Text = "localhost";
            txtUsername.Text = "root";
            passwordBox1.Password = "noway";
#else
#endif
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
                        Database=txtDbName.Text,
                        Pwd=passwordBox1.Password,
                        Server=txtServername.Text,
                        Username=txtUsername.Text,
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
                return true;
            }
        }

        #endregion

        private void butEnumDbs_Click(object sender, RoutedEventArgs e)
        {
            txtDbName.DataContext = null;


            DocDbObject myInfo = (DocDbObject)MyDocDataContext;
            myInfo.LoginInfo = ((LoginInfo_MySql)((ISrcControl)this).X_Result);

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
    }
}
