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
using XLCS.Common;
using DBStudio.Utility;
using DBStudio.GlobalDefine;

namespace DBStudio.UI
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : BaseUI.BaseFadeDialog
    {
        public enum MainIDEStyle
        {
            Old,
            Normal,
        };

        public MainIDEStyle CurMainIDEStyle { get; set; }

        public Options()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.chkIsUseSpeakInsteadOfMsg.IsChecked = Properties.Settings.Default.UseSpeakInsteadOfMsgBox;
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default["AutoBackupOldVersionFileBeforeUpgrade"] = (bool)checkBox1.IsChecked;
                Properties.Settings.Default.Save();


                Properties.Settings.Default.UseSpeakInsteadOfMsgBox = (bool)chkIsUseSpeakInsteadOfMsg.IsChecked;
                Properties.Settings.Default.CmdTextFontSize = (int)txtFontSize.Value;
                Properties.Settings.Default.Save();
                Close();
            }
            catch (Exception ee)
            {
                ProcessException.DisplayErrors(ee);
            }
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Clear all saved info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butClearDbOpendHistoryInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HistoryObject historyObject = SerializeClass.DatabaseHistoryInfo;

                historyObject.MySqlHistory.Clear();
                historyObject.SqlServerHistory.Clear();
                historyObject.OleDbHistory.Clear();
                historyObject.SSCEHistory.Clear();

                SerializeClass.DatabaseHistoryInfo = historyObject;
                
                "ClearedOpendHistoryInfo".GetFromResourece().Notify();
            }
            catch (Exception ee)
            {
                ee.Notify();
            }
        }
    }
}
