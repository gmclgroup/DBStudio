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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using XLCS.Common;
using DBStudio.GlobalDefine;

namespace DBStudio.SqlCE.Tools
{
    /// <summary>
    /// Interaction logic for GenerateBusinessEntity.xaml
    /// </summary>
    public partial class GenerateBusinessEntity : BaseUI.BaseFadeDialog
    {
        List<string> errorTableList = new List<string>();

        public GenerateBusinessEntity()
        {
            InitializeComponent();
            //this.checkBox1.Triggers
            List<string> tableList = App.MainEngineer.GetTableListInDatabase();
            foreach (string item in tableList)
            {
                this.listBox1.Items.Add(item);
            }

            this.checkBox1.Checked += new RoutedEventHandler(checkBox1_Checked);
            this.textBox1.Text = "C:\\";
        }

        void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox1.IsChecked)
            {
                this.listBox1.SelectAll();
            }
            else
            {
                this.listBox1.UnselectAll();
            }
        }

        /// <summary>
        /// Start Generate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.Items.Count < 1)
            {
                return;
            }

            List<string> selectedList = new List<string>();
            foreach (string item in this.listBox1.SelectedItems)
            {
                selectedList.Add(item);
            }

            foreach (string item in selectedList)
            {
                if ((bool)checkBox2.IsChecked)
                {
                    processGenerate(item);
                }
            }
            if (errorTableList.Count == 0)
            {
                "Generate succesful".Notify();

            }
            else
            {
                StringBuilder st = new StringBuilder();
                foreach (String item in errorTableList)
                {

                    st.Append(item+"\r\n");
                }

                MessageBox.Show(String.Format("These tables are failure {0}",st.ToString()));
            }


        }

        private void processGenerate(string item)
        {
            try
            {
                CodeCompileUnit targetUnit = new CodeCompileUnit();

                CodeNamespace ns = new CodeNamespace(Properties.Settings.Default.MM_NamespaceName);

                ns.Imports.Add(new CodeNamespaceImport("System"));
                ns.Imports.Add(new CodeNamespaceImport("System.Text"));

                targetUnit.Namespaces.Add(ns);

                CodeTypeDeclaration ct = new CodeTypeDeclaration("Table_" + item);
                ct.IsClass = true;
                ct.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
                //ct.IsPartial = true;

                ns.Types.Add(ct);

                List<string> ColumnLists = App.MainEngineer.GetColumnNameListFromTable(item);

                if (ColumnLists.Count < 1)
                {
                    return;
                }

                foreach (string columnName in ColumnLists)
                {
                    CodeMemberField member = new CodeMemberField();
                    member.Name = "_"+columnName;
                    member.Type = new CodeTypeReference(typeof(string));
                    member.Attributes = MemberAttributes.Private|MemberAttributes.Static;
                    member.InitExpression = new CodePrimitiveExpression(columnName);
                    ct.Members.Add(member);

                    CodeMemberProperty p = new CodeMemberProperty();
                    p.Name = columnName;
                    p.Attributes = MemberAttributes.Public|MemberAttributes.Final|MemberAttributes.Static;
                    p.HasGet = true;
                    p.HasSet = false;
                    
                    p.Type = new CodeTypeReference(typeof(string));
                    //p.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_"+columnName)));
                    p.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("_"+columnName)));

                    ct.Members.Add(p);
                }

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                using (StreamWriter sm = new StreamWriter(String.Format("{0}\\{1}.cs", this.textBox1.Text, item)))
                {
                    provider.GenerateCodeFromCompileUnit(targetUnit, sm, options);
                }

            }
            catch (Exception ee)
            {
                errorTableList.Add(item);
                ProcessException.DisplayErrors(ee);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog s = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.textBox1.Text = s.SelectedPath;
                }
            }



        }


    }
}
