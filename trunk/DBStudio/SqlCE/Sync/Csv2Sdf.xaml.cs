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
using DBStudio.CommonMethod;
using Microsoft.Win32;
using DBStudio.GlobalDefine;

namespace DBStudio.SqlCE.Sync
{
    /// <summary>
    /// Interaction logic for Csv2Sdf.xaml
    /// </summary>
    public partial class Csv2Sdf : BaseUI.BaseFadeDialog
    {
        public Csv2Sdf()
        {
            InitializeComponent();
        }

        private void SyncCSV2SSCE(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrEmpty(txtSrcFile.Text)) || (string.IsNullOrEmpty(txtTargetFile.Text)))
            {
                "ImportAccess_SelectFile".GetFromResourece().Notify();
                return;
            }
            string csvFile = txtSrcFile.Text;
            string sdfFile = txtTargetFile.Text;
            try
            {
                if (CommonUtil.ConvertCSVToSdf(csvFile, sdfFile, (bool)checkBox1.IsChecked, txtPwd.Password,(bool)chkIsFirstRowIsColumeName.IsChecked))
                {
                    MessageBox.Show(String.Format("ImportAccess_DoOK".GetFromResourece(), csvFile, sdfFile));
                    Close();
                }
            }
            catch (Exception ee)
            {
                XLCS.Common.ProcessException.DisplayErrors(ee);
            }
        }

        private void butSelectSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = MyGlobal.CVS_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                this.txtSrcFile.Text = sf.FileName;
            }
        }

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
