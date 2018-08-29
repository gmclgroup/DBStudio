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
using System.Diagnostics;
using wf=System.Windows.Forms;
using System.IO;
using DBStudio.CommonMethod;
using ETL;
using DBStudio.GlobalDefine;


namespace DBStudio.SqlCE.Tools
{
    /// <summary>
    /// Interaction logic for GenerateLinqContext.xaml
    /// </summary>
    public partial class GenerateLinqContext : BaseUI.BaseFadeDialog
    {

        public bool IsUsingWhenNotLogin { get; set; }

        public GenerateLinqContext()
        {
            InitializeComponent();
            ShowInTaskbar = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exeFile">Exe File</param>
        /// <param name="sdfFile">Sqlce DbFile </param>
        /// <param name="CodeFile">Generated Code File</param>
        /// <param name="codeFile">Database password</param>
        private void DoGenerateLinqContext(string exeFile, string sdfFile, string codeFile,string pwd,bool isDBML)
        {
            string arguments = string.Empty;
            string markType = string.Empty;
            if (isDBML)
            {
                markType = "dbml";
            }
            else
            {
                markType = "code";
            }

            //Add "" to file name allow accept file name contain blank space
            if (string.IsNullOrEmpty(pwd))
            {
                arguments = string.Format(" \"{0}\" /{1}:\"{2}\"", sdfFile,markType, codeFile);
            }
            else
            {
                arguments = string.Format(" \"{0}\" /password:{1} /{2}:\"{3}\"", sdfFile, pwd,markType, codeFile);
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = exeFile;
            info.Arguments = arguments;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            try
            {
                Process process = System.Diagnostics.Process.Start(info);
                process.WaitForExit();
                string stdout = process.StandardOutput.ReadToEnd();
                StringBuilder output = new StringBuilder();
                output.Append((stdout == "") ? "" : string.Format("\n\nOutput:\n{0}", stdout));
                string outputString = output.ToString();

                if (File.Exists(codeFile))
                {
                    ("Generate Linq Context Succesful ,the output code file name is \r\n " + codeFile).Notify();
                }
                else
                {
                    MessageBox.Show(stdout);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void butGenerateCurrentOpened_Click(object sender, RoutedEventArgs e)
        {

                txtPwd.Password = App.MainEngineer.CurPwd;
                txtSrcFile.Text = App.MainEngineer.CurDatabase;

                txtTargetLocation.Text = System.IO.Path.GetDirectoryName(txtSrcFile.Text);
                DOGenerateLinqContext(true);
        }

        private void butGenerate_Click(object sender, RoutedEventArgs e)
        {
            DOGenerateLinqContext(false);
        }

        /// <summary>
        /// Use option parameters in .net4.0
        /// </summary>
        /// <param name="isCurrentDB"></param>
        void DOGenerateLinqContext(bool isCurrentDB)
        {
            bool IsDBML = false;

            string exe = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Appendix\\Sqlmetal.exe";
            if (!File.Exists(exe))
            {
                "SqlMetalFileLost".GetFromResourece().Notify();
                return;
            }
            string sdfFile = txtSrcFile.Text;
            if (string.IsNullOrEmpty(sdfFile))
            {
                "SelectSSCEFileMsg".GetFromResourece().Notify();
                return;
            }
            string targetRoot = txtTargetLocation.Text;
            if (string.IsNullOrEmpty(targetRoot))
            {
                "SelectTargetMsg".GetFromResourece().Show();
                return;
            }
            if (!Directory.Exists(targetRoot))
            {
                "SelectToaDirConfirmMsg".Show();
                return;
            }

            string targetCodeFile = string.Empty;
            string extensionFileName = string.Empty;

            if ((bool)rdCSharp.IsChecked)
            {
                extensionFileName = "cs";
            }
            else if ((bool)rdVB.IsChecked)
            {
                extensionFileName = "vb";
            }
            else
            {
                extensionFileName = "dbml";
                IsDBML = true;
            }

            targetCodeFile = targetRoot + "\\" + System.IO.Path.GetFileNameWithoutExtension(sdfFile) + "." + extensionFileName;

            if (File.Exists(targetCodeFile))
            {
                if (!string.Format("OverrideFileConfrmMsg".GetFromResourece(), targetCodeFile).Confirm())
                {
                    return;
                }
            }

            string pwd = txtPwd.Password;
            string notifyMsg = isCurrentDB ? "GenerateLinqFileConfrimMsg_ForCurrent".GetFromResourece() : "GenerateLinqFileConfrimMsg".GetFromResourece();

            if (notifyMsg.Confirm())
            {
                DoGenerateLinqContext(exe, sdfFile, targetCodeFile, pwd, IsDBML);
            }
        }
        private void butSelectFile_Click(object sender, RoutedEventArgs e)
        {
            using (wf.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog())
            {
                of.Filter = "SqlCe Db File(*.sdf)|*.sdf|All Files(*.*)|*.*";
                if (of.ShowDialog() == wf.DialogResult.OK)
                {
                    txtSrcFile.Text = of.FileName;
                    txtTargetLocation.Text = System.IO.Path.GetDirectoryName(of.FileName);
                }
            }
        }

        private void butTargetLocationSelect_Click(object sender, RoutedEventArgs e)
        {
            using (wf.FolderBrowserDialog od = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (od.ShowDialog() == wf.DialogResult.OK)
                {
                    txtTargetLocation.Text = od.SelectedPath;
                }
            }

        }


        private void rdVB_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rdVB.IsChecked)
            {

            }

        }

        private void rdCSharp_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rdDBML_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}
