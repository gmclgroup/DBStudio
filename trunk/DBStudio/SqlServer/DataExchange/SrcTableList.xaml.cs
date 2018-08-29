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
using System.ComponentModel;

namespace DBStudio.SqlServer.DataExchange
{
    /// <summary>
    /// Interaction logic for SrcTableList.xaml
    /// </summary>
    public partial class SrcTableList : BaseUI.BaseFadeDialog 
    {
        CoreEA.ICoreEAHander core;
        public SrcTableList(CoreEA.ICoreEAHander inCore)
        {
            core = inCore;
            InitializeComponent();

        }

        /// <summary>
        /// Background thread
        /// </summary>
        private void LoadDbInfo()
        {
            BackgroundWorker bg = new BackgroundWorker();
            this.listBox1.Items.Clear();
            bg.WorkerReportsProgress = true;
            bg.DoWork += delegate(object s1, DoWorkEventArgs se)
            {
                List<string> dbList = core.GetDatabaseList();

                foreach (string item in dbList)
                {
                    bg.ReportProgress(0,item);
                }

            };

            bg.ProgressChanged += delegate(object s2, ProgressChangedEventArgs args)
            {
                listBox1.Items.Add(args.UserState.ToString());
            };

            bg.RunWorkerAsync();
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.SelectedItems == null)
            {
                return;
            }
            List<string> tableList = new List<string>();
            foreach (string item in listBox1.SelectedItems)
            {
                tableList.Add(item);
            }

            TargetServer ts = new TargetServer(core, tableList);
            ts.ShowDialog();


        }
    }
}
