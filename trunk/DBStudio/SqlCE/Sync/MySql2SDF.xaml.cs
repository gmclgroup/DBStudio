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
using DBStudio.CommonMethod;
using XLCS.Common;
using System.Data;
using Microsoft.Win32;
using System.Diagnostics;
using System.Data.SqlServerCe;
using DBStudio.GlobalDefine;

namespace DBStudio.SqlCE.Sync
{
    /// <summary>
    /// Interaction logic for MySql2SDF.xaml
    /// </summary>
    public partial class MySql2SDF : BaseUI.BaseFadeDialog
    {
        public MySql2SDF()
        {
            InitializeComponent();
            Title = "Import MySql to SSCE";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

#if DEBUG
            button1.Visibility = Visibility.Visible;
#else
            button1.Visibility = Visibility.Hidden;
#endif

        }

        private void SyncMysql2SSCE(object sender, RoutedEventArgs e)
        {
            string server = txtServername.Text;
            string database = this.txtDbName.Text;
            string username = this.txtUsername.Text;
            string pwd = passwordBox1.Password;

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database)
                || string.IsNullOrEmpty(username))
            {
                "ImportEachElement".GetFromResourece().Notify();
                return;
            }
            string sdfFile = string.Empty;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = MyGlobal.SQLCE_FILE_FILTER;
            if (sf.ShowDialog() == true)
            {
                sdfFile = sf.FileName;
            }
            else
            {
                return;
            }
            bool isNeedCopyData=false;
            if((checkBox1.IsChecked.HasValue) && (checkBox1.IsChecked.Value == true))
            {
                isNeedCopyData=true;
            }

            if (CommonUtil.SyncDataFromMysqlToSqlCE(server, database, username, pwd, sdfFile, isNeedCopyData))
            {
                MessageBox.Show(String.Format("ImportAccess_DoOK".GetFromResourece(), database, sdfFile));
            }

        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.txtServername.Text = "localhost";
            this.txtDbName.Text = "test";
            this.txtUsername.Text = "root";
            passwordBox1.Password = "test";
        }
    }
}
