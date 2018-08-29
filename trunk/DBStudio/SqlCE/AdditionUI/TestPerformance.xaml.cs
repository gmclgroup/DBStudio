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
using DBStudio.BaseUI;
using CustomControl.NewXLAGControl;
using System.Data.Common;
using System.Data.SqlServerCe;
using DBStudio.CommonMethod;
using System.Diagnostics;
using System.Data;
using XLCS.Common;
using DBStudio.GlobalDefine;
using CoreEA.Args;
using CoreEA.LoginInfo;
using CoreEA.SchemaInfo;

namespace DBStudio.AdditionUI
{
    /// <summary>
    /// Interaction logic for TestPerformance.xaml
    /// </summary>
    public partial class TestPerformance : BaseFadeDialog
    {
        public TestPerformance()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int recordCount = 10000;
            using (PopupForm p = new PopupForm())
            {
                p.X_NotifyStr = "Record Count";
                if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int outValue;
                    if (!int.TryParse(p.X_Result, out outValue))
                    {
                        return;
                    }
                    recordCount = outValue;
                }
                else
                {
                    return;
                }
            }

            string testDb=DateTime.Now.Second.ToString()+"test.sdf";
            string testTable="testTable";

            if(!App.MainEngineer.CreateDatabase(new LoginInfo_SSCE(){DbName=testDb}))
            {
                return;
            }
            App.MainEngineer.Open(new CoreEA.LoginInfo.LoginInfo_SSCE(){DbName=testDb,Pwd="",IsEncrypted=false});

            if(!App.MainEngineer.IsOpened)
            {
                throw new Exception("Can't Open");
            }

            //List<CreateTableArgs> argsList=new List<CreateTableArgs>();

            //CreateTableArgs args=new CreateTableArgs();
            //args.allowNulls = false;
            //args.dataLength = 0;
            //args.dataType="int";
            //args.fieldName="id";
            //args.isUnique = false;
            //args.isPrimaryKey = false;
            //argsList.Add(args);

            BaseTableSchema tableSchame = new BaseTableSchema();
            tableSchame.Columns.Add(new BaseColumnSchema() { ColumnName = "id", ColumnType = "int" });

            try
            {

                App.MainEngineer.CreateTable(tableSchame);

                string sqlCmd = string.Empty;
                SqlCeConnection conn = new SqlCeConnection(string.Format("Data source={0}", testDb));
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = conn;
                conn.Open();

                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < recordCount; i++)
                {
                    sqlCmd = string.Format("insert into {0} values ({1})", testTable, i);
                    cmd.CommandText = sqlCmd;
                    cmd.ExecuteNonQuery();
                }

                watch.Stop();
                long sqlDirectTime = watch.ElapsedMilliseconds;

                watch.Reset();
                watch.Start();

                cmd.CommandText = string.Format("INSERT INTO {0} VALUES (?)", testTable);
                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Prepare();
                for (int i = 0; i < recordCount; i++)
                {
                    cmd.Parameters[0].Value = i;
                    cmd.ExecuteNonQuery();
                }
                
                watch.Stop();
                long sqlParaTime = watch.ElapsedMilliseconds;
                watch.Reset();

                watch.Start();

                cmd.CommandText = testTable;
                cmd.CommandType = CommandType.TableDirect;
                SqlCeResultSet st = cmd.ExecuteResultSet(ResultSetOptions.Updatable);
                SqlCeUpdatableRecord rec = st.CreateRecord();
                for (int i = 0; i < recordCount; i++)
                {
                    rec.SetInt32(0, i);
                    
                    st.Insert(rec);
                }
                watch.Stop();
                long sqlceResultSetTime = watch.ElapsedMilliseconds;

                //watch.Start();

                //cmd.CommandText = testTable;
                //cmd.CommandType = CommandType.TableDirect;
                //SqlCeResultSet st = cmd.ExecuteResultSet(ResultSetOptions.Updatable);
                //SqlCeUpdatableRecord rec = 
                //for (int i = 0; i < recordCount; i++)
                //{
                //    rec.SetInt32(0, i);

                //    st.Insert(rec);
                //}
                //watch.Stop();
                long sqlceUpdateResultSetTime = 100;// watch.ElapsedMilliseconds;



                cmd.Dispose();
                conn.Close();

                MessageBox.Show(string.Format("Test Result is \r\nDirect sql command used {0} \r\nUse parameters used{1}\r\nUse SqlceResultSet used{2}\r\nUpdate Sqlce ResultSet{3}\r\n", sqlDirectTime, sqlParaTime, sqlceResultSetTime,sqlceUpdateResultSetTime));
            }
            catch (Exception ee)
            {
                ProcessException.DisplayErrors(ee);
            }
        }
    }
}
