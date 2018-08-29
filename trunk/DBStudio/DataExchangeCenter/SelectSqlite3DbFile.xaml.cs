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
using Microsoft.Win32;
using DBStudio.GlobalDefine;
using CoreEA.LoginInfo;
using DBStudio.DocumentingDB;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectSqlite3DbFile.xaml
    /// </summary>
    public partial class SelectSqlite3DbFile : UserControl,ISrcControl,IStep
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

                LoginInfo_Sqlite info=new LoginInfo_Sqlite()
                {
                    DbFile = txtSqlFile.Text,
                };

                return info;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
                return !string.IsNullOrEmpty(txtSqlFile.Text);
            }
        }

        public void X_ShowErrorTips()
        {
            labelWarning.Visibility = Visibility.Visible;
        }


        public SelectSqlite3DbFile()
        {
            InitializeComponent();
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = MyGlobal.SQLite_FILE_FILTER;
            if (of.ShowDialog() == true)
            {
                txtSqlFile.Text = of.FileName;
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
            myInfo.LoginInfo = ((LoginInfo_Sqlite)((ISrcControl)this).X_Result);
            try
            {
                CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Sqlite).X_Handler;

                tempHandler.Open(
                    new LoginInfo_Sqlite()
                    {
                        DbFile=txtSqlFile.Text,
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
    }
}
