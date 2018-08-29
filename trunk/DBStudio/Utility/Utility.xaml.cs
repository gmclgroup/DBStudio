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
using DBStudio.GlobalDefine;
namespace DBStudio.Utility
{
    /// <summary>
    /// Interaction logic for Utility.xaml
    /// </summary>
    public partial class Utility : Page
    {
        public CoreE.UsedDatabaseType curDbType { get; set; }

        public Utility(CoreE.UsedDatabaseType  dbType)
        {
            curDbType = dbType;
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                "Please input the connection string first".Notify();
                return;
            }

            string str=textBox1.Text;

            CoreEA.ICoreEAHander core = new CoreEA.CoreE(curDbType).X_Handler;
            try
            {
                core.Open(str);
                if (core.IsOpened)
                {
                    "Connection is right".Notify();
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
    }
}