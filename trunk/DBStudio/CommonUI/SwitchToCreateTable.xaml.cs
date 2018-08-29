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

namespace DBStudio.CommonUI
{
    /// <summary>
    /// Interaction logic for SwitchToCreateTable.xaml
    /// </summary>
    public partial class SwitchToCreateTable : BaseUI.BaseFadeDialog
    {
        public SwitchToCreateTable()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CurCreateType = CreateType.NotSelected;
        }

        public enum CreateType
        {
            NotSelected,
            Simple,
            Normal,
        };

        public CreateType CurCreateType { get; set; }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            CurCreateType = CreateType.Simple;

            Close();
        }

        private void butCreateDetail_Click(object sender, RoutedEventArgs e)
        {
            CurCreateType = CreateType.Normal;

            Close();
        }

    }
}
