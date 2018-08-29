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
using WPFCommonControl;
using CoreEA.LoginInfo;
using DBStudio.DocumentingDB;

namespace DBStudio.DataExchangeCenter
{
    /// <summary>
    /// Interaction logic for SelectSqlServerSource.xaml
    /// </summary>
    public partial class SelectSqlServerSource : UserControl, ISrcControl,IStep
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
                SqlServerLoginControl.X_CollectionData curResult = sqlServerLoginControl1.X_Result;

                LoginInfo_SqlServer ret = new LoginInfo_SqlServer()
                {
                    X_Database = curResult.DbName,
                    X_Server = curResult.Server,
                    X_UserName = curResult.UID,
                    X_Pwd = curResult.PWD,
                    X_CurDbConnectionMode = curResult.CurType.CurConnMode,
                    IsTrustedConn = curResult.IsTrust,
                };

                return ret;
            }
        }

        public bool X_CanForwardToNext
        {
            get
            {
                SqlServerLoginControl.X_CollectionData curResult = sqlServerLoginControl1.X_Result;
                if (string.IsNullOrEmpty(curResult.DbName))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(curResult.Server))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(curResult.UID))
                {
                    return false;
                }

                return true;
            }
        }

        public void X_ShowErrorTips()
        {
            labelWarning.Visibility = Visibility.Visible;
        }

        public SelectSqlServerSource()
        {
            InitializeComponent();
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
            myInfo.LoginInfo = ((LoginInfo_SqlServer)((ISrcControl)this).X_Result);

            try
            {
                CoreEA.ICoreEAHander tempHandler = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlServer).X_Handler;
                SqlServerLoginControl.X_CollectionData curResult = sqlServerLoginControl1.X_Result;

                tempHandler.Open(
                    new LoginInfo_SqlServer()
                    {
                    X_Database = curResult.DbName,
                    X_Server = curResult.Server,
                    X_UserName = curResult.UID,
                    X_Pwd = curResult.PWD,
                    X_CurDbConnectionMode = curResult.CurType.CurConnMode,
                    IsTrustedConn = curResult.IsTrust,
                    }
                    );

                if (tempHandler.IsOpened)
                {

                }
                else
                {
                    throw new Exception("Connection not correct");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        object IStep.Result
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
