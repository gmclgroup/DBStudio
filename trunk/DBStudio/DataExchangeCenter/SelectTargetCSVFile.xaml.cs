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
using System.IO;
using CoreEA.LoginInfo;
using wf = System.Windows.Forms;
using Microsoft.Win32;
using DBStudio.GlobalDefine;
using ETL;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectTargetCSVFile.xaml
    /// </summary>
    public partial class SelectTargetCSVFile : UserControl, ISrcControl
    {
        public bool X_IsSourceHandler
        {
            get
            {
                return false;
            }
        }


        public SelectTargetCSVFile()
        {
            InitializeComponent();
        }
        public void X_ShowErrorTips()
        {
            labelWarning.Visibility = Visibility.Visible;
        }

        public BaseLoginInfo X_Result
        {
            get
            {
                LoginInfo_CSV info = new LoginInfo_CSV()
                {
                    Database=txtFolder.Text,
                    IsFirstRowIsColumnName=(bool)this.chkFirstRowIsColumnName.IsChecked,
                };

                return info;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
                bool ret = false;

                if (!string.IsNullOrEmpty(txtFolder.Text))
                {
                    if (Directory.Exists(txtFolder.Text))
                    {
                        ret = true;
                    }
                    else
                    {
                        Directory.CreateDirectory(txtFolder.Text);
                    }
                }

                //if (String.IsNullOrEmpty(txtFolder.Text))
                //{
                //    return false;
                //}
                //if (File.Exists(txtFolder.Text))
                //{
                //    "This file is existed".Show();
                //    return false;
                //}

                ret = true;
                return ret;
            }
        }

        private void butSelectCSVFolder_Click(object sender, RoutedEventArgs e)
        {
            wf.FolderBrowserDialog f = new wf.FolderBrowserDialog();
            if (f.ShowDialog() == wf.DialogResult.OK)
            {
                txtFolder.Text = f.SelectedPath;
            }
            //SaveFileDialog sf = new SaveFileDialog();
            //sf.Filter = MyGlobal.CVS_FILE_FILTER;
            //if ((bool)sf.ShowDialog())
            //{
            //    txtFolder.Text = sf.FileName;
            //}
        }
    }
}
