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
using ETL;
using DBStudio.GlobalDefine;
using System.Data;
using Microsoft.Win32;

namespace DBStudio.SqlCE.Tools
{
    /// <summary>
    /// Interaction logic for GenerateDbSchema.xaml
    /// </summary>
    public partial class GenerateDbSchema : BaseUI.BaseFadeDialog
    {

        public event EventHandler ShowSchemaDataInfo = null;

        public enum SchemaType
        {
            ColumnSchema,
            IndexSchema,
        }

        public enum OutputType
        {
            Xml,
            DataGrid,
        }



        public GenerateDbSchema()
        {
            InitializeComponent();

            tableList.Items.Clear();

            foreach (string table in App.MainEngineer.GetTableListInDatabase())
            {
                tableList.Items.Add(table);
            }
            if (tableList.Items.Count > 0)
            {
                tableList.SelectedIndex = 0;
            }

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            cmbGenType.Items.Add(SchemaType.ColumnSchema);
            cmbGenType.Items.Add(SchemaType.IndexSchema);
            cmbGenType.SelectedIndex = 0;


            cmbOutputMdoe.Items.Add(OutputType.Xml);
            cmbOutputMdoe.Items.Add(OutputType.DataGrid);

            cmbOutputMdoe.SelectedIndex = 1;
        }

        private void butGenerateSchema_Click(object sender, RoutedEventArgs e)
        {
            if (tableList.SelectedItems.Count < 1)
            {
                "DataTransferNoTableMsg".GetFromResourece().Notify();
                return;
            }

            foreach (var item in tableList.SelectedItems)
            {
                DataTable ds=null;
                switch ((SchemaType)cmbGenType.SelectedIndex)
                {
                    case SchemaType.ColumnSchema:
                        ds = App.MainEngineer.GetColumnInfoFromTable(item.ToString());
                        break;
                    case SchemaType.IndexSchema:
                        ds = App.MainEngineer.GetIndexInfoFromTable(item.ToString());
                        break;
                }

                switch ((OutputType)cmbOutputMdoe.SelectedIndex)
                {
                    case OutputType.Xml:
                        SaveFileDialog sf = new SaveFileDialog();
                        sf.Filter = "Xml file (*.xml)|*.xml|All Files(*.*)|*.*";
                        if ((bool)sf.ShowDialog() == true)
                        {
                            ds.WriteXml(sf.FileName);
                            "GenerateTableSchemaOK".GetFromResourece().Notify();
                        }
                        break;
                    case OutputType.DataGrid:
                        if (ShowSchemaDataInfo != null)
                        {
                            ShowSchemaDataInfo(ds, null);
                        }
                        break;
                }
            }
        }
    }
}
