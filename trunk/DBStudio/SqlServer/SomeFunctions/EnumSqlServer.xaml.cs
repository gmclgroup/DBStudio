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
using XLCS.Common;
using ETL;
using DBStudio.GlobalDefine;
using System.Threading;
using DBStudio.BaseUI;

namespace DBStudio.SqlServer.SomeFunctions
{
    /// <summary>
    /// Interaction logic for EnumSqlServer.xaml
    /// </summary>
    public partial class EnumSqlServer : BaseFadeDialog
    {
        public event EventHandler SelectedSqlServer;
        public static RoutedUICommand SearchCmd = new RoutedUICommand();

        public EnumSqlServer()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(SearchCmd, SearchCmd_Executed, SearchCmd_CanExecute));
            butSearch.Command = SearchCmd;
            
            mySqlserverList.MouseDoubleClick += mySqlserverList_MouseDoubleClick;
        }

        public void SearchCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mySqlserverList.DataContext = null;
            progressBar.Visibility = System.Windows.Visibility.Visible;
            Thread t = new Thread(new ThreadStart(GetSqlServers));
            t.Start();

        }
        public void SearchCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtServer.Text.IsEmpty())
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        void mySqlserverList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mySqlserverList.SelectedItem != null)
            {
                string sqlName = mySqlserverList.SelectedItem.ToString();
                if (SelectedSqlServer != null)
                {
                    SelectedSqlServer(null, new MyEventArgs() { MyText = sqlName });
                }
                Close();
            }
        }

        private void GetSqlServers()
        {
            string[] ret = XLCS.Db.SqlLocator.GetServers();
            if (ret != null)
            {
                if (ret.Length == 0)
                {
                    "TitileNoSqlServerInstanceName".GetFromResourece().Show();
                    return;
                }

                List<string> serverList = ret.ToList();
                this.Dispatcher.BeginInvoke((ThreadStart)delegate
                {
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    mySqlserverList.DataContext = serverList;
                });
            }

        }
    }
}
