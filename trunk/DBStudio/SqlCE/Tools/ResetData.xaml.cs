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
using wf = System.Windows.Forms;
using DBStudio.CommonMethod;
using DBStudio.GlobalDefine;
using ETL;
using CoreEA.LoginInfo;


namespace DBStudio.SqlCE.Tools
{
    /// <summary>
    /// Interaction logic for ResetData.xaml
    /// </summary>
    public partial class ResetData : BaseUI.BaseFadeDialog
    {
        public ResetData()
        {
            InitializeComponent();
            ShowInTaskbar = false;
        }

        private void butSelect_Click(object sender, RoutedEventArgs e)
        {
            using (wf.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog())
            {
                of.Filter = "SqlCe Db File(*.sdf)|*.sdf|All Files(*.*)|*.*";
                if (of.ShowDialog() == wf.DialogResult.OK)
                {
                    textBox1.Text = of.FileName;
                }
            }
        }

        private void but_ResetData_Click(object sender, RoutedEventArgs e)
        {
            string db = textBox1.Text;
            if (string.IsNullOrEmpty(db))
            {
                "SelectSSCEFileMsg".GetFromResourece().Notify();
                return;
            }

            if (!("ResetDataConfrimMsg".GetFromResourece()).Confirm())
            {
                return;
            }

            string pwd = passwordBox1.Password;
            CoreEA.ICoreEAHander h = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            try
            {
                h.Open(new LoginInfo_SSCE() { DbName = db, Pwd = pwd, IsEncrypted = (bool)checkBox1.IsChecked });

                foreach (string table in h.GetTableListInDatabase(db))
                {
                    h.RemoveAllData(table);
                }
                h.Close();
                "ResetDataOK".GetFromResourece().Notify();
            }
            catch (Exception eee)
            {
                eee.HandleMyException();
            }
        }

        /// <summary>
        /// Clear/Delete all the data of current opened database
        /// It's will may be a risk action 
        /// Please confirm doing that . 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_ResetCurrent_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!("ResetDataCurrentFileConfrimMsg".GetFromResourece()).Confirm())
                {
                    return;
                }

                CoreEA.ICoreEAHander h = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;

                h.Open(new CoreEA.LoginInfo.LoginInfo_SSCE()
                {
                    DbName=App.MainEngineer.CurDatabase,
                    Pwd=App.MainEngineer.CurPwd,
                    IsEncrypted=true,
                });

                foreach (string table in App.MainEngineer.GetTableListInDatabase())
                {
                    h.RemoveAllData(table);
                }

                h.Close();

                "ResetDataOK".GetFromResourece().Notify();
            }
            catch (Exception eee)
            {
                XLCS.Common.ProcessException.DisplayErrors(eee);
            }
        }
    }
}
