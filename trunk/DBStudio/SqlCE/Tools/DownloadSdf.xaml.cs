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
using System.Net;
using System.IO;
using DBStudio.GlobalDefine;

namespace DBStudio.SqlCE.Tools
{
    /// <summary>
    /// Interaction logic for DownloadSdf.xaml
    /// </summary>
    public partial class DownloadSdf : BaseUI.BaseFadeDialog
    {
        public DownloadSdf()
        {
            InitializeComponent();

        }

        private void butDownload_Click(object sender, RoutedEventArgs e)
        {
            string uri=textBox1.Text;
            string file=textBox2.Text;
            if (string.IsNullOrEmpty(uri))
            {
                "Please input the URI".Notify();
                return;
            }

            if(File.Exists(file))
            {
                if(MessageBox.Show("The file is existed , do you want to override it ? ","Warning",MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,MessageBoxResult.No)!=MessageBoxResult.Yes)
                {
                    return;
                }
            }

            WebClient wb = new WebClient();
            wb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wb_DownloadFileCompleted);
            wb.DownloadFileAsync(new Uri(uri), file);
        }

        void wb_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            "Complete".Notify();
        }
    }
}
