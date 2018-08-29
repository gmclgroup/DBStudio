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
using Microsoft.Win32;
using DBStudio.GlobalDefine;
using System.IO;
using CoreEA.LoginInfo;
using CoreEA.Args;
using DBStudio.DocumentingDB;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectSourceDbFile_OleDB.xaml
    /// </summary>
    public partial class SelectSourceDbFile_OleDB : UserControl, ISrcControl,IStep
    {
        public bool X_IsSourceHandler
        {
            get
            {
                return true;
            }
        }

        public BaseLoginInfo X_Result
        {
            get
            {
                BaseLoginInfo info = null;
                try
                {
                    string database = txtSrcFile.Text;


                    if ((bool)radAccess.IsChecked)
                    {
                        info = new LoginInfo_Oledb()
                        {
                            Database = database,
                            Pwd = pwd.Password,
                        };
                    }
                    else if ((bool)radCVS.IsChecked)
                    {
                        info = new LoginInfo_CSV()
                        {
                            Database = database,
                        };
                    }
                    else if ((bool)radExcel.IsChecked)
                    {
                        info = new LoginInfo_Excel()
                        {
                            Database = database,
                        };
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
                return info;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
                return !string.IsNullOrEmpty(txtSrcFile.Text);
            }
        }

        public void X_ShowErrorTips()
        {
            labelWarning.Visibility = Visibility.Visible;
        }

        public SelectSourceDbFile_OleDB()
        {
            InitializeComponent();
            this.txtSrcFile.Drop += new DragEventHandler(textBox1_Drop);
        }

        /// <summary>
        /// This ctor used when as a target control
        /// Used in DataExchangeWizard
        /// </summary>
        /// <param name="curType"></param>
        public SelectSourceDbFile_OleDB(UsingOleDbType curType)
        {
            InitializeComponent();
            switch (curType)
            {
                case UsingOleDbType.Access:
                    break;
                case UsingOleDbType.Excel:
                    break;
                case UsingOleDbType.CSV:
                    radAccess.Visibility = Visibility.Collapsed;
                    radExcel.Visibility = Visibility.Collapsed;
                    radCVS.IsChecked = true;
                    break;
                default:
                    break;
            }
            this.txtSrcFile.Drop += new DragEventHandler(textBox1_Drop);
        }


        public UsingOleDbType CurrentDbType
        {
            get
            {
                if ((bool)radAccess.IsChecked)
                {
                    return UsingOleDbType.Access;
                }
                else if ((bool)radCVS.IsChecked)
                {
                    return UsingOleDbType.CSV;
                }
                else if ((bool)radExcel.IsChecked)
                {
                    return UsingOleDbType.Excel;
                }
                return UsingOleDbType.Access;
            }
        }

        void textBox1_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (fileNames.Length == 1)
                {
                    // Check for a file (a directory will return false).
                    if (File.Exists(fileNames[0]))
                    {
                        // At this point we know there is a single file.
                        this.txtSrcFile.Text = fileNames[0];
                    }
                }

            }
        }

        public void Select(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();

            if ((bool)radAccess.IsChecked)
            {
                sf.Filter = MyGlobal.MDB_FILE_FILTER;
            }
            else if ((bool)radCVS.IsChecked)
            {
                sf.Filter = MyGlobal.CVS_FILE_FILTER;
            }
            else if ((bool)radExcel.IsChecked)
            {
                sf.Filter = MyGlobal.EXCEL_FILE_FILTER;
            }


            if (sf.ShowDialog() == true)
            {
                this.txtSrcFile.Text = sf.FileName;
            }
        }

        #region IStep Members

        public object MyDocDataContext
        {
            get;
            set;
        }

        bool IStep.CanLeave()
        {
            return ((ISrcControl)this).X_CanForwardToNext;
        }

        void IStep.Leave()
        {
            DocDbObject myInfo = (DocDbObject)MyDocDataContext;
            myInfo.LoginInfo = ((LoginInfo_Oledb)((ISrcControl)this).X_Result);

            try
            {
                CoreEA.ICoreEAHander tempHandler = null;

                if ((bool)radAccess.IsChecked)
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.OleDb).X_Handler;
                }
                else if ((bool)radCVS.IsChecked)
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.CSV).X_Handler;
                }
                else if ((bool)radExcel.IsChecked)
                {
                    tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.Excel).X_Handler;
                }

                tempHandler.Open(myInfo.LoginInfo);
                if (tempHandler.IsOpened)
                {

                }
                else
                {
                    throw new Exception("Password not correct");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }


        public object Result
        {
            get
            {
                return null;
            }
        }
        bool IStep.IsSource
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
