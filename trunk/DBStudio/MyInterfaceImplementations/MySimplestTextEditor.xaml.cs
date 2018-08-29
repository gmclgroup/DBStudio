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
using DBStudio.MyInterface;
using wf = System.Windows.Forms;

namespace DBStudio.MyInterfaceImplementations
{
    /// <summary>
    /// This simpestTextEditor used to Replace the MyAdvTextEditor when encoutner performance issues
    /// but currently it is not using.
    /// </summary>
    public partial class MySimplestTextEditor : UserControl, ISqlQueryTextor
    {
        public MySimplestTextEditor()
        {
            InitializeComponent();
        }

        #region ISqlQueryTextor Members
#pragma warning disable 0067
        public event wf.KeyEventHandler MyKeyDown = null;
#pragma warning restore 0067
        /// <summary>
        /// Allow not implememnt this 
        /// </summary>
#pragma warning disable 0067
        public event System.Windows.Input.KeyEventHandler MyAdvKeyDown = null;
#pragma warning restore 0067

        void ISqlQueryTextor.Dispose()
        {
            this.txtBox.Text = string.Empty;    

        }

        string ISqlQueryTextor.Text
        {
            get
            {
                return txtBox.Text;
            }
            set
            {

                txtBox.Text = value;
            }
        }

        void ISqlQueryTextor.Focus()
        {
            txtBox.Focus();
        }

        string ISqlQueryTextor.SelectedText
        {
            get {return txtBox.SelectedText; }
        }

        /// <summary>
        /// Currently Do nothing
        /// </summary>
        System.Drawing.Font ISqlQueryTextor.Font
        {
            get;
            set;
        }

        public bool IsAllowFormatSqlContent { get; set; }

        #endregion

        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (null != MyAdvKeyDown)
            {
                MyAdvKeyDown(sender, e);
            }
        }
    }
}
