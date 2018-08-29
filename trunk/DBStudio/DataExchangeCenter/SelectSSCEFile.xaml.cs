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
using CoreEA.LoginInfo;
using System.IO;
using DBStudio.GlobalDefine;
using DBStudio.DocumentingDB;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectSSCEFile.xaml
    /// </summary>
    public partial class SelectSSCEFile : UserControl, ISrcControl, IStep
    {

        #region ISrcControl Members

        CoreEA.LoginInfo.BaseLoginInfo ISrcControl.X_Result
        {
            get
            {
                LoginInfo_SSCE info = new LoginInfo_SSCE()
                {
                    DbName = txtDbPath.Text,
                    Pwd = string.IsNullOrEmpty(txtPwd.Password) == true ? null : txtPwd.Password,
                    IsEncrypted = (bool)chkIsEncrypted.IsChecked,
                };

                return info;
            }
        }

        bool ISrcControl.X_CanForwardToNext
        {
            get
            {
                bool ret = false;

                if (!string.IsNullOrEmpty(txtDbPath.Text))
                {
                    if (File.Exists(txtDbPath.Text))
                    {
                        ret = true;
                    }
                    else
                    {
                        CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;

                        if (tempHandler.CreateDatabase(
                            new LoginInfo_SSCE()
                            {
                                DbName = txtDbPath.Text,
                                Pwd = txtPwd.Password,
                                IsEncrypted = !string.IsNullOrEmpty(txtPwd.Password),
                                IsCaseSensitive = true,
                            }
                            ))
                        {
                            ret = true;
                        }

                    }
                }

                return ret;
            }
        }

        void ISrcControl.X_ShowErrorTips()
        {
            labelWarning.Visibility = Visibility.Visible;
        }

        bool ISrcControl.X_IsSourceHandler
        {
            get
            {
                return true;
            }
        }

        #endregion

        public SelectSSCEFile()
        {
            InitializeComponent();
        }

        private void butSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = MyGlobal.SQLCE_FILE_FILTER;
            if (of.ShowDialog() == true)
            {
                this.txtDbPath.Text = of.FileName;
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
            myInfo.LoginInfo = ((LoginInfo_SSCE)((ISrcControl)this).X_Result);

            try
            {
                CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;

                tempHandler.Open(
                    new LoginInfo_SSCE()
                    {
                        DbName = txtDbPath.Text,
                        Pwd = txtPwd.Password,
                        IsEncrypted = !string.IsNullOrEmpty(txtPwd.Password),
                        IsCaseSensitive = true,
                    }
                    );
                if (tempHandler.IsOpened)
                {

                }
                else
                {
                    throw new Exception("Password not correct");
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
