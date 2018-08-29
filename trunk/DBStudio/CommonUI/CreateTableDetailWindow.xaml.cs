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
using System.Data;
using System.Collections;
using DBStudio.CommonMethod;
using XLCS.Common;
using XLCS.RegularCommand;
using System.Diagnostics;
using DBStudio.GlobalDefine;
using CoreEA.Args;
using ETL;


namespace DBStudio.CommonUI
{
    /// <summary>
    /// Interaction logic for CreateTableDetailWindow.xaml
    /// </summary>
    /// 
    [Obsolete("This class is obsolete, use this window may be has risk")]
    public partial class CreateTableDetailWindow : BaseUI.BaseFadeDialog
    {
        public delegate void Del_AddNewTable();
        public Del_AddNewTable RaiseAddNewTable=null;

        private const string ColumnName_AllowNull = "Allow Nulls";
        private const string ColumnName_Unique = "Unique";
        private const string ColumnName_PrimaryKey = "Primary Key";
        private const string ColumnName_ColumName = "Column Name";
        private const string ColumnName_DataType = "Data Type";
        private const string ColumnName_Length = "Length";

        public enum EnteringType
        {
            CreateTable,
            ModifySchema,
        }

        public EnteringType CurEnteringType { get; set; }
        private string _curTablename = string.Empty;

        public CreateTableDetailWindow(EnteringType eType)
        {
            InitializeComponent();
            CurEnteringType = eType;
            InitSelf();
        }

        public CreateTableDetailWindow(EnteringType eType, string tableName)
        {
            InitializeComponent();
            CurEnteringType = eType;
            _curTablename = tableName;
            InitSelf();
        }

        private void InitSelf()
        {
            switch (CurEnteringType)
            {
                case EnteringType.CreateTable:
                    Title = "CreatTableDialogTitle".GetFromResourece();
                    InitUIForCreateTable();
                    break;
                case EnteringType.ModifySchema:
                    Title = "ModifyTableDialogTitle".GetFromResourece();
                    InitUIForCreateTable();
                    InitUIForModifySchema();
                    break;
                default:
                    break;
            }
            dataGridViewForCreateTable.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridControl1_CellEndEdit);
        }

        private void InitUIForModifySchema()
        {
            dataGridViewForCreateTable.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(dataGridControl1_DataError);
            System.Windows.Forms.DataGridViewCellStyle style = new System.Windows.Forms.DataGridViewCellStyle();
            {
                style.BackColor = System.Drawing.Color.Beige;
                style.ForeColor = System.Drawing.Color.Brown;
                style.Font = new System.Drawing.Font("Verdana", 8);

            }

            // Apply the style as the default cell style.
            dataGridViewForCreateTable.AlternatingRowsDefaultCellStyle = style;

            //List<CreateTableArgs> argsList = new List<CreateTableArgs>();

            DataTable ds = App.MainEngineer.GetColumnInfoFromTable(_curTablename);
            int index = 0;
            foreach (System.Data.DataRow item in ds.Rows)
            {
                index = dataGridViewForCreateTable.Rows.Add();
                System.Windows.Forms.DataGridViewRow row = dataGridViewForCreateTable.Rows[index];

                //row.Cells[0].Value = "dfd";
                row.Cells[ColumnName_ColumName].Value = item[MyGlobal.ColumnName].ToString();
                row.Cells[ColumnName_DataType].Value = item[MyGlobal.DataType].ToString();
                if (item[MyGlobal.DataLength] != DBNull.Value)
                {
                    row.Cells[ColumnName_Length].Value = item[MyGlobal.DataLength].ToString();
                }

                row.Cells[ColumnName_AllowNull].Value = item[MyGlobal.AllowNull].ToString();
                //row.Cells[ColumnName_PrimaryKey].Value = item[GlobalInfo.PrimaryKey].ToString();
                //row.Cells[ColumnName_Unique].Value = item[GlobalInfo.Unique].ToString();

            }

        }

        void dataGridControl1_DataError(object sender, System.Windows.Forms.DataGridViewDataErrorEventArgs e)
        {
            string msg = String.Format("ErrorInfo_CreateTableSchemaDialog".GetFromResourece(), e.RowIndex, e.ColumnIndex);
            Debug.WriteLine(msg);
            e.ThrowException=false;
            
        }


        private void InitUIForCreateTable()
        {
            this.dataGridViewForCreateTable.Columns.Add(ColumnName_ColumName, ColumnName_ColumName);

            System.Windows.Forms.DataGridViewComboBoxColumn t2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            t2.Name = t2.HeaderText = ColumnName_DataType;

            foreach (string item in GlobalDefine.MyGlobal.SSCE_SUPPORTED_DATATYPE)
            {
                t2.Items.Add(item);
            }

            this.dataGridViewForCreateTable.Columns.Add(t2);

            this.dataGridViewForCreateTable.Columns.Add(ColumnName_Length, ColumnName_Length);

            System.Windows.Forms.DataGridViewComboBoxColumn t3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            t3.Name = t3.HeaderText = ColumnName_AllowNull;

            t3.Items.Add("Yes");
            t3.Items.Add("No");
            t3.ValueMember = "No";
            this.dataGridViewForCreateTable.Columns.Add(t3);

            System.Windows.Forms.DataGridViewComboBoxColumn t4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            t4.Name = t4.HeaderText = ColumnName_Unique;
            t4.Items.Add("Yes");
            t4.Items.Add("No");
            t4.ValueMember = "No";
            this.dataGridViewForCreateTable.Columns.Add(t4);

            System.Windows.Forms.DataGridViewComboBoxColumn t5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            t5.Name = t5.HeaderText = ColumnName_PrimaryKey;
            t5.Items.Add("Yes");
            t5.Items.Add("No");
            t5.ValueMember = "No";
            this.dataGridViewForCreateTable.Columns.Add(t5);
        }

        void dataGridControl1_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            
            object celValue = dataGridViewForCreateTable.CurrentCell.Value;
            
            if (celValue == null)
            {
                return;
            }
            try
            {
                if ((e.ColumnIndex == 1) && (dataGridViewForCreateTable.CurrentRow.Cells[2].Value==null))
                {
                    string ttt = celValue.ToString();
                    if (ttt == "nvarchar")
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[1].Value = "nvarchar";
                        dataGridViewForCreateTable.CurrentRow.Cells[2].Value = "100";
                    }
                    if (ttt == "int")
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[1].Value = "int";
                        dataGridViewForCreateTable.CurrentRow.Cells[2].Value = "8";
                    }
                    if (ttt == "integer")
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[1].Value = "integer";
                        dataGridViewForCreateTable.CurrentRow.Cells[2].Value = "8";
                    }
                    if (ttt == "smallint")
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[1].Value = "smallint";
                        dataGridViewForCreateTable.CurrentRow.Cells[2].Value = "4";
                    }

                    if (dataGridViewForCreateTable.CurrentRow.Cells[ColumnName_Unique].Value==null)
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[ColumnName_Unique].Value = "No";
                    }
                    if (dataGridViewForCreateTable.CurrentRow.Cells[ColumnName_PrimaryKey] == null)
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[ColumnName_PrimaryKey].Value = "No";
                    }
                    if (dataGridViewForCreateTable.CurrentRow.Cells[ColumnName_AllowNull].Value == null)
                    {
                        dataGridViewForCreateTable.CurrentRow.Cells[ColumnName_AllowNull].Value = "No";
                    }
                }
            }
            catch (Exception ee)
            {
                ProcessException.DisplayErrors(ee);
            }

   
        }


        private void butDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.dataGridViewForCreateTable.Rows.Count <= 0)
            {
                return;
            }

            if (dataGridViewForCreateTable.CurrentRow != null)
            {
                dataGridViewForCreateTable.Rows.Remove(dataGridViewForCreateTable.CurrentRow);
            }
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            switch (CurEnteringType)
            {
                case EnteringType.CreateTable:
                    DoOKWhenCreateTable();
                    break;
                case EnteringType.ModifySchema:
                    DoOKWhenModifyTableSchema();
                    break;
                default:
                    throw new Exception("ErrorMsg_InvalidType".GetFromResourece());
                    //break;
            }

        }
        private void DoOKWhenModifyTableSchema()
        {
            "NotCompletedFeatureNotifyMsg".GetFromResourece().Show();
        }

        private void DoOKWhenCreateTable()
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                "CreatTableSchema_NeedTableName".GetFromResourece().Notify();
                return;
            }
            string tableName = textBox1.Text;
            if (dataGridViewForCreateTable.Rows.Count > 0)
            {
                List<CreateTableArgs> createTableArgsList = new List<CreateTableArgs>();

                foreach (System.Windows.Forms.DataGridViewRow row in this.dataGridViewForCreateTable.Rows)
                {
                    CreateTableArgs args = new CreateTableArgs();
                    if (row.Cells[ColumnName_AllowNull].Value != null)
                    {
                        try
                        {
                            args.allowNulls = CommonUtil.SwitchToBool(row.Cells[ColumnName_AllowNull].Value);
                            args.isPrimaryKey = CommonUtil.SwitchToBool(row.Cells[ColumnName_PrimaryKey].Value);
                            args.isUnique = CommonUtil.SwitchToBool(row.Cells[ColumnName_Unique].Value);
                            if (row.Cells[ColumnName_DataType].Value == null)
                            {
                                "Data Type Can not be empty".Notify();
                                return;
                            }
                            args.dataType = row.Cells[ColumnName_DataType].Value.ToString();
                            if (row.Cells[ColumnName_Length].Value == null)
                            {
                                "Column Length Can not be empty".Notify();
                                return;
                            }
                            args.dataLength = int.Parse(row.Cells[ColumnName_Length].Value.ToString());
                            
                            if (row.Cells[ColumnName_ColumName].Value == null)
                            {
                                "Column Name Can not be empty".Notify();
                                return;
                            }
                            args.fieldName = row.Cells[ColumnName_ColumName].Value.ToString();

                            //Some type no need length properties ,so set empty 
                            //These types include int,interge,
                            if (GlobalDefine.NoLenghtType.Collections.Contains(args.dataType))
                            {
                                args.dataLength = 0;
                            }
                            createTableArgsList.Add(args);
                        }
                        catch (Exception ee)
                        {
                            ProcessException.DisplayErrors(ee);
                        }
                    }
                }

                if (App.MainEngineer.CreateTableWithSchemaInfo(createTableArgsList, tableName))
                {
                    if (RaiseAddNewTable != null)
                    {
                        RaiseAddNewTable();
                    }

                    "CreateTableSchmea_CreateTableOkMsg".GetFromResourece().Notify();

                    Close();
                }

            }
        }
    }
}
