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
using System.Data.OleDb;
using DBStudio.CommonMethod;
using System.IO;
using DBStudio.GlobalDefine;


namespace DBStudio.Utility
{
    /// <summary>
    /// Interaction logic for BatchConvertXlsToAccess.xaml
    /// </summary>
    public partial class BatchConvertXlsToAccess : BaseUI.BaseFadeDialog
    {
        public enum ConvertMode
        {
            SingleFile,
            Folder,
        };

        public ConvertMode CurConvertMode { get; set; }

        public BatchConvertXlsToAccess(ConvertMode mode)
        {
            InitializeComponent();
            checkBox1.Checked += new RoutedEventHandler(checkBox1_Checked);
            CurConvertMode = mode;
            InitSelf();
        }

        private void InitSelf()
        {
            switch (CurConvertMode)
            {
                case ConvertMode.SingleFile:
                    label1.Content = "Excel File";
                    label2.Content = "Access File";
                    button1.Content = "Convert";
                    checkBox1.IsChecked = false;
                    break;
                case ConvertMode.Folder:
                    label1.Content = "Excel Folder";
                    label2.Content = "Access Folder";
                    button1.Content = "Batch Convert";
                    checkBox1.IsChecked = true;
                    break;
            }

        }

        void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox1.IsChecked)
            {
                CurConvertMode = ConvertMode.Folder;
            }
            else
            {
                CurConvertMode = ConvertMode.SingleFile;
            }
            InitSelf();
        }

        private void ConvertSingleFile(string xlsFile, string mdbFile)
        {
            if (!File.Exists(xlsFile))
            {
                "Source file not existed".Notify();
                return;
            }

            string connStr =
String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;", xlsFile);

            if (!File.Exists(mdbFile))
            {
                string createMDBFileString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbFile;
                ADOX.Catalog category = new ADOX.Catalog();
                category.Create(createMDBFileString);

                /*here is the old way to create by offfice com */
                //Microsoft.Office.Interop.Access.ApplicationClass a = new Microsoft.Office.Interop.Access.ApplicationClass();
                //a.NewCurrentDatabase(mdbFile);
            }

            CoreEA.ICoreEAHander excelEg = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.OleDb).X_Handler;

            excelEg.Open(new CoreEA.LoginInfo.LoginInfo_Oledb() { Database=xlsFile,});
            if (!excelEg.IsOpened)
            {
                "Can't open excel file.the action will cancel".Notify();
                return;
            }

            OleDbConnection conn = new OleDbConnection(connStr);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            conn.Open();
            string cmdText = string.Empty;
            foreach (string item in excelEg.GetTableListInDatabase())
            {
                cmdText =
                        String.Format("SELECT * INTO [MS Access;Database={0}].[{1}] FROM [{2}]", mdbFile, item, item);
                cmd.CommandText = cmdText;
                    cmd.ExecuteNonQuery();

            }


            conn.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try{
            switch (CurConvertMode)
            {
                case ConvertMode.SingleFile:
                    string xlsFile = textBox1.Text;
                    string mdbFile = textBox2.Text;
                    ConvertSingleFile(xlsFile, mdbFile);
                    break;
                case ConvertMode.Folder:
                    string xlsFolder = textBox1.Text;
                    string mdbFolder = textBox2.Text;
                    foreach (string item in Directory.GetFiles(xlsFolder))
                    {
                        ConvertSingleFile(item,string.Format("{0}\\{1}.{2}",mdbFolder,System.IO.Path.GetFileName(item),".mdb"));
                    }

                    break;
                default:
                    break;
            }
            MessageBox.Show("Convert Completed");
            }
            catch (Exception ee)
            {
                XLCS.Common.ProcessException.DisplayErrors(ee);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            switch (CurConvertMode)
            {
                case ConvertMode.SingleFile:
                    using (System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog())
                    {
                        if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            textBox1.Text = of.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                case ConvertMode.Folder:
                    using (System.Windows.Forms.FolderBrowserDialog of = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            textBox1.Text = of.SelectedPath;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
            }
            
     
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            switch (CurConvertMode)
            {
                case ConvertMode.SingleFile:
                    using (System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog())
                    {
                        if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            textBox2.Text = of.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                case ConvertMode.Folder:
                    using (System.Windows.Forms.FolderBrowserDialog of = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            textBox2.Text = of.SelectedPath;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
            }

        }
    }
}
