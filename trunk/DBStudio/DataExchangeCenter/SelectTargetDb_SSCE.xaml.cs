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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using DBStudio.GlobalDefine;
using CoreEA.LoginInfo;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectTargetDb_SSCE.xaml
    /// </summary>
    public partial class SelectTargetDb_SqlCe : UserControl, ISrcControl
    {
        public bool X_IsSourceHandler
        {
            get
            {
                return false;
            }
        }


        public void X_ShowErrorTips()
        {
            labelWarning.Visibility = Visibility.Visible;
        }

        public BaseLoginInfo X_Result
        {
            get
            {
                LoginInfo_SSCE info = new LoginInfo_SSCE()
                {
                    DbName=txtSSCEFilePath.Text,
                };

                return info;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
                bool ret = false;

                if (!string.IsNullOrEmpty(txtSSCEFilePath.Text))
                {
                    if (File.Exists(txtSSCEFilePath.Text))
                    {
                        ret = true;
                    }
                    else
                    {
                        CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
                        if (tempHandler.CreateDatabase(new LoginInfo_SSCE() { DbName = txtSSCEFilePath.Text ,Pwd="",IsEncrypted=false,IsCaseSensitive=true}))
                        {
                            ret = true;
                        }

                    }
                }

                return ret;
            }
        }

        public SelectTargetDb_SqlCe()
        {
            InitializeComponent();
        }

        private void butSelectSSCEFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.SQLCE_FILE_FILTER;
            sf.CheckFileExists = false;
            if (sf.ShowDialog() == true)
            {
                this.txtSSCEFilePath.Text = sf.FileName;
            }
        }
    }
}
