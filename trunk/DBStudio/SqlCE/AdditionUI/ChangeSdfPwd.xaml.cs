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
using System.Data.SqlServerCe;
using DBStudio.GlobalDefine;
using CoreEA.LoginInfo;
using System.Diagnostics.CodeAnalysis;

namespace DBStudio.SqlCE.AdditionUI
{
    /// <summary>
    /// Interaction logic for ChangeSdfPwd.xaml
    /// </summary>
    public partial class ChangeSdfPwd : BaseUI.BaseFadeDialog
    {

        public delegate void X_PressOK(object sender, KeyEventArgs e);


        /// <summary>
        /// Event to Mark the result of ShowDialog() is OK .
        /// Due to unknown reason ,we can't get the DialogResult as common way did
        /// So Add this event to the client can listen it. 
        /// </summary>
        #pragma warning disable 0067
        public event X_PressOK PressOK;
        #pragma warning restore 0067

        public string X_NewPwd { get; set; }
        public string Db { get; set; }


        public ChangeSdfPwd(string db)
        {
            Db = db;
            InitializeComponent();
        }

        private void butChange_Click(object sender, RoutedEventArgs e)
        {
            string oldPwd = txtOldPwd.Password;
            string newPwd = txtNewPwd1.Password;
            string newPwd2 = txtNewConfirmPwd.Password;
            if (!newPwd.Equals(newPwd2))
            {
                "PwdNotEqual".GetFromResourece().Notify();
                return;
            }

            try
            {
                #region Close Connection First
                if (null != App.MainEngineer)
                {
                    if (App.MainEngineer.IsOpened)
                    {
                        //If change pwd after the connection opened , it will access violation
                        //so close the Global Connection Frist, 
                        //Then after the changing pwd processing ,reopen the global connection
                        App.MainEngineer.Close();
                    }
                }
                #endregion 

                #region Change PWD
                using (SqlCeEngine eg = new SqlCeEngine())
                {
                    eg.LocalConnectionString = String.Format("Data Source={0};Password={1}", Db, oldPwd);
                    eg.Compact(string.Format("Data Source={0};Password={1}", Db, newPwd));
                }

                "ChangePwdOK".GetFromResourece().Notify();

                X_NewPwd = newPwd;
                #endregion 

                #region Reopen DB
                if (null != App.MainEngineer)
                {
                    App.MainEngineer.CurPwd = newPwd;

                    App.MainEngineer.Open(new LoginInfo_SSCE()
                    {
                        DbName = App.MainEngineer.CurDatabase,
                        Pwd = App.MainEngineer.CurPwd,
                        IsEncrypted = true,
                    });
                }
                #endregion 

                DialogResult = true;
            }
            catch (SqlCeException ee)
            {
                String.Format("{0}\r\n{1}", ee.Message, "This may the old password you input is not valid").Notify();
                return;
            }

            
        }
    }
}
