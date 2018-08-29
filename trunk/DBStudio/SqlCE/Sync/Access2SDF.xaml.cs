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
using DBStudio.CommonMethod;
using System.Security;
using System.IO;
using System.Data;
using XLCS.Common;
using System.Data.SqlServerCe;
using DBStudio.GlobalDefine;

namespace DBStudio.SqlCE.Sync
{
    /// <summary>
    /// Interaction logic for Access2SDF.xaml
    /// </summary>
    public partial class Access2SDF : BaseUI.BaseFadeDialog
    {
        public Access2SDF()
        {
            InitializeComponent();
            this.txtSrcFile.Drop += new DragEventHandler(textBox1_Drop);
        }

        void textBox1_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (fileNames.Length == 1)
                {
                    // Check for a file (a directory will return false).
                    if (File.Exists(fileNames[0]))
                    {
                        // At this point we know there is a single file.
                        this.txtSrcFile.Text = fileNames[0];
                    }
                }

            }
        }

        public void Select(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.MDB_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                this.txtSrcFile.Text = sf.FileName;
            }
        }


        public void SyncOledb2SSCE(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrEmpty(txtSrcFile.Text)) || (string.IsNullOrEmpty(txtTargetFile.Text)))
            {
                "ImportAccess_SelectFile".GetFromResourece().Notify();
                return;
            }
            string mdbFile = txtSrcFile.Text;
            string sdfFile = txtTargetFile.Text;
            try
            {
                if (CommonUtil.SyncMdbToSdf(mdbFile, sdfFile, (bool)checkBox1.IsChecked, txtPwd.Password))
                {
                    String.Format("ImportAccess_DoOK".GetFromResourece(), mdbFile, sdfFile).Notify();
                    Close();
                }
                else
                {
                    
                    "ImportData_ReadError".GetFromResourece().Notify();

                }
            }
            catch (Exception ee)
            {
                XLCS.Common.ProcessException.DisplayErrors(ee);
            }
        }

        /// <summary>
        /// Select Target Sql Ce Db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.SQLCE_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                this.txtTargetFile.Text = sf.FileName;
            }
        }


    }
}

