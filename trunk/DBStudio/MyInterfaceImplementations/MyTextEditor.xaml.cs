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
using ICSharpCode.TextEditor;
using wf = System.Windows.Forms;
using DBStudio.Bases;
using System.Diagnostics;
using DBStudio.MyInterface;

namespace DBStudio.MyInterfaceImplementations
{
    /// <summary>
    /// This highlight text editor depend on COM 
    /// 
    /// </summary>
    public partial class MyTextEditor : UserControl, ISqlQueryTextor
    {
        public bool IsRTF { get; set; }
        public event wf.KeyEventHandler MyKeyDown = null;

        /// <summary>
        /// Allow not implememnt this 
        /// </summary>
#pragma warning disable 0067
        public event System.Windows.Input.KeyEventHandler MyAdvKeyDown = null;
#pragma warning restore 0067

        string lastText = string.Empty;


        public bool IsAllowFormatSqlContent { get; set; }

        #region ISqlQueryTextor Members

        public System.Drawing.Font Font
        {
            get
            {
                return myEditControl.Font;
            }
            set
            {
                myEditControl.Font = value;
            }
        }

        public void Dispose()
        {

        }

        public new void Focus()
        {
            myEditControl.Focus();

        }


        public string Text
        {
            get
            {
                return myEditControl.Text;
            }
            set
            {
                myEditControl.Text = value;
            }
        }

        public string SelectedText
        {
            get
            {
                string selectedText=string.Empty;

                if (IsRTF)
                {
                    selectedText = ((wf.RichTextBox)myEditControl).SelectedText;
                }
                else
                {
                    throw new NotImplementedException();
                }

                return selectedText;
            }
        }

        #endregion
        private System.Windows.Forms.Control myEditControl = null;
        protected System.Windows.Forms.Control X_MyEditControl
        {
            get
            {
                if (myEditControl == null)
                {
                    if (IsRTF)
                    {
                        wf.RichTextBox temp = new wf.RichTextBox();
                        temp.KeyDown += new System.Windows.Forms.KeyEventHandler(temp_KeyDown);
                        temp.WordWrap = true;
                        myEditControl = temp;
                    }
                    else
                    {
                        TextEditorControl temp = new TextEditorControl();
                        //ISharp Textor can't handle the keydown event.
                        temp.KeyDown+=new System.Windows.Forms.KeyEventHandler(temp_KeyDown);
                        temp.SetHighlighting("SQL");
                        myEditControl = temp;
                    }
                }
                return myEditControl;
            }
            set
            {
                myEditControl = value;
            }
        }

        void temp_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            
            if (MyKeyDown != null)
            {
                MyKeyDown(sender,e);
            }
        }


        public string ConvertColor(System.Drawing.Color color)
        {
            return @"\red" + color.R + @"\green" + color.G + @"\blue" + color.B + ";";
        }

        public MyTextEditor()
        {
            InitializeComponent();
            IsRTF = Properties.Settings.Default.UseRTFQueryTextor;
            IsRTF = true;

            if (IsRTF)
            {

                X_MyEditControl.TextChanged += (sender, e) =>
                {

                    System.Windows.Forms.RichTextBox myRichControl = sender
                        as System.Windows.Forms.RichTextBox;
                    string curText = myRichControl.Text;

                    if (curText.EndsWith("\n"))
                    {
                        return;
                    }

                    if ((!string.IsNullOrEmpty(curText)) && (curText != lastText))
                    {
                        Debug.WriteLine("Current Start Position " + myRichControl.SelectionStart);
                        Debug.WriteLine("Current Start SelectionIndent" + myRichControl.SelectionIndent);

                        int lastPosition = myRichControl.SelectionStart;

                        SqlParser parser = new SqlParser();
                        string rtf = parser.Parse(curText, DBStudio.SqlParser.SyntaxConstants.SqlServer);


                        // 1: Keyword (blue)
                        // 3: System Function (green)
                        // 5: operator (75,75,75)
                        // 6: Text (red)
                        // 7: Number (cyan)
                        // 8: Comment (magenta)
                        // 10: Standard Text (black)

                        string colorTable = @"{\colortbl;" +
                            ConvertColor(System.Drawing.Color.Blue) +
                            ConvertColor(System.Drawing.Color.Black) +
                            ConvertColor(System.Drawing.Color.Green) +
                            ConvertColor(System.Drawing.Color.Black) +
                            ConvertColor(System.Drawing.Color.FromArgb(75, 75, 75)) +
                            ConvertColor(System.Drawing.Color.Red) +
                            ConvertColor(System.Drawing.Color.OrangeRed) +
                            ConvertColor(System.Drawing.Color.Gray) +
                            ConvertColor(System.Drawing.Color.Black) +
                            ConvertColor(System.Drawing.Color.Black) +
                            @"}";

                        lastText = curText;
                        myRichControl.Rtf = @"{\rtf1" + colorTable + @rtf + @"}";

                        myRichControl.SelectionStart = lastPosition;
                    }
                };
            }

            windowsFormsHost1.Child = X_MyEditControl;
        }


    }
}
