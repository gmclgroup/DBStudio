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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DBStudio.CommonMethod;
using DBStudio.Utility;
using DBStudio.BaseUI;
using DBStudio.GlobalDefine;
using CoreEA;

namespace DBStudio.DynaControls
{
    /// <summary>
    /// Interaction logic for DbOpenedHistory.xaml
    /// </summary>
    public partial class DbOpenedHistory : BaseFadeDialog
    {
        public delegate void Del_AfterSelectedValue(string a);
        public Del_AfterSelectedValue AfterSelectedValue = null;

        public DbOpenedHistory()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            List<SSCEObjects> historyObject = SerializeClass.DatabaseHistoryInfo.SSCEHistory;
            foreach (var item in historyObject)
            {
                item.IsExisted = File.Exists(item.DbFileFullPath);
            }

            myHistoryGrid.DataContext = SerializeClass.DatabaseHistoryInfo.SSCEHistory;
        }

        private void DoSelect()
        {

            SSCEObjects curObject = myHistoryGrid.SelectedItem as SSCEObjects;
            if (curObject != null)
            {
                if (AfterSelectedValue != null)
                {
                    AfterSelectedValue(curObject.DbFileFullPath);

                }
                Close();
            }
        }


        private void myHistoryGrid_KeyDown(object sender, KeyEventArgs e)
        {
            DoSelect();
        }

        private void myHistoryGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoSelect();
        }
    }
}
