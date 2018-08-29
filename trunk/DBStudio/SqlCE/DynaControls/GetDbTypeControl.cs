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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBStudio.DynaControls
{
    public partial class GetDbTypeControl : XLCS.Standard.BaseDialog
    {
        public List<int> NotInList = new List<int>();
        public delegate void Del_AfterSelectedValue(string a);
        public Del_AfterSelectedValue AfterSelectedValue = null;
        public GetDbTypeControl()
        {
            InitializeComponent();

            this.Text = "";

            FormBorderStyle = FormBorderStyle.None;
            Button but = new Button();
            but.Click += delegate { Dispose(); };
            CancelButton = but;

            NotInList.Add(24);
            NotInList.Add(26);
            NotInList.Add(27);
            NotInList.Add(28);
            listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
            listBox1.KeyDown += new KeyEventHandler(listBox1_KeyDown);
            //string item = string.Empty;
            //This SqlDbType is used for standard sql db type
            //sqlce need specical
            //for (int i = 0; i < 35; i++)
            //{
            //    if (!NotInList.Contains(i))
            //    {
            //       // this.listBox1.Items.Add(((SqlDbType)i).ToString());
            //    }
            //}

            DataTable dbTypeDs = App.MainEngineer.GetSupportedDbType();
            foreach (DataRow item in dbTypeDs.Rows)
            {
                this.listBox1.Items.Add(item[0].ToString());
            }

            Load += new EventHandler(GetDbTypeControl_Load);
        }

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            DoSelect();
        }

        void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoSelect();
            }
        }

        private void DoSelect()
        {
            string result = string.Empty;

            if (this.listBox1.SelectedItem != null)
            {
                result = this.listBox1.SelectedItem.ToString();
                if (AfterSelectedValue != null)
                {
                    AfterSelectedValue(result);

                }
                Dispose();
            }
        }

        void GetDbTypeControl_Load(object sender, EventArgs e)
        {
            this.listBox1.Focus();
            this.listBox1.SetSelected(0, true);
        }
    }
}
