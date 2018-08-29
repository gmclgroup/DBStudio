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
using CoreEA;
using ETL;
namespace DBStudio.DocumentingDB
{
    /// <summary>
    /// Interaction logic for SelectTableWindow.xaml
    /// </summary>
    public partial class SelectTableWindow : UserControl, IStep
    {
        List<string> tableList;

        public SelectTableWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SelectTableWindow_Loaded);
        }

        void SelectTableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DocDbObject myData = MyDocDataContext as DocDbObject;

            CoreEA.CoreE core = new CoreEA.CoreE(myData.SourceDbType);
            ICoreEAHander myHandler = null;
            myHandler = core.X_Handler;
            try
            {
                myHandler.Open(myData.LoginInfo);
                if (myHandler.IsOpened)
                {
                    tableList = myHandler.GetTableListInDatabase();


                    listControl.ItemsSource = tableList;


                }
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }
            finally
            {
                if (myHandler.IsOpened)
                {
                    myHandler.Close();
                }
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
            if (listControl.SelectedItems.Count < 1)
            {
                return false;
            }
            return true;
        }

        public void Leave()
        {
            List<string> tableList = new List<string>();
            foreach (var item in listControl.SelectedItems)
            {
                tableList.Add(item.ToString());
            }

            DocDbObject myData = MyDocDataContext as DocDbObject;
            myData.SelectedTableNameCollection = tableList;
        }

        public object Result
        {
            get { return null; }
        }

        public bool IsSource
        {
            get { return false; }
        }

        #endregion


        /// <summary>
        /// 
        /// 20100419:The below problem has been solved.
        /// ==========================
        /// Here we had a problem 
        /// The datacontext is string collection
        /// present in UI will be a checkbox 
        /// so how can I emurate the string collection to control the check-status of checkbox in the UI 
        /// ==========================
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            if ((bool)chkCheckAllTable.IsChecked)
            {
                //foreach (ListViewItem item in listControl.Items)
                //{
                //    CheckBox curItem = (CheckBox)item.Content;
                //    curItem.IsChecked = true;
                //}
                foreach (var item in tableList)
                {
                    listControl.SelectedItems.Add(item);
                }
            }
            else
            {
                //should not to enter here 
            }
        }

        private void chkCheckAllTable_Unchecked(object sender, RoutedEventArgs e)
        {
            listControl.SelectedItems.Clear();
            //foreach (var item in tableList)
            //{
            //    listControl.SelectedItems.Remove(item);
            //}
        }
    }
}
