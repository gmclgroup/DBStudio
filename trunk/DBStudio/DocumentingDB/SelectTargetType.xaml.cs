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
using DBStudio.DocumentingDB.Exportors;
using ETL;
namespace DBStudio.DocumentingDB
{
    /// <summary>
    /// Interaction logic for SelectTargetType.xaml
    /// </summary>
    public partial class SelectTargetType : UserControl, IStep
    {
        public SelectTargetType()
        {
            InitializeComponent();


            Loaded += new RoutedEventHandler(SelectTargetType_Loaded);
        }

        void SelectTargetType_Loaded(object sender, RoutedEventArgs e)
        {
            cmbTargetType.Items.Add(new ComboBoxItem()
            {
                Content = "Xml",
                Tag = Exportor.XmlExportor,
            });
            cmbTargetType.Items.Add(new ComboBoxItem()
            {
                Content = "Excel",
                Tag = Exportor.ExcelExportor,
            });

            cmbTargetType.SelectedIndex = 0;
        }

        #region IStep Members


        public object MyDocDataContext
        {
            get;
            set;
        }

        public bool IsSource
        {
            get
            {
                return true;
            }
        }

        public object Result
        {
            get
            {
                return null; 
            }

        }

        public bool CanLeave()
        {
            if (myFile.X_HasText)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Leave()
        {
            DocDbObject myData = (DocDbObject)MyDocDataContext;
            myData.CurExportor = ((ComboBoxItem)cmbTargetType.SelectedItem).Tag as Exportor;
            myData.TargetFile = myFile.X_FileName;
        }

        #endregion

        private void cmbTargetType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = cmbTargetType.SelectedItem as ComboBoxItem;
            if (item.Tag is ExcelExportor)
            {
                myFile.X_OpenFileDialogFilter = "xls file(*.xls)|*.xls|all file(*.*)|*.*"; ;
            }
            else if (item.Tag is XmlExportor)
            {
                myFile.X_OpenFileDialogFilter = "xml file(*.xml)|*.xml|all file(*.*)|*.*";
            }
        }
    }
}
