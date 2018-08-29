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
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using DBStudio.CommonMethod;
using DBStudio.Utility;
using System.Collections.Generic;
using DBStudio.UI;
using DBStudio.BaseUI;
using CustomControl.NewXLAGControl;
using System.Data.SqlServerCe;
using System.ComponentModel;
using System.Xml;
using System.Text;
using XLCS.Webs;
using XLCS.Common;
using System.Reflection;
using XLCS.Xml;
using DBStudio.SqlCE.AdditionUI;
using DBStudio.GlobalDefine;
using XLCS.Sqlce;
using CoreEA.Args;
using CoreEA.LoginInfo;
using DBStudio.SqlServer;
using wf = System.Windows.Forms;
using System.Diagnostics;
using DBStudio.DataExchangeCenter;
using CoreEA;
using DBStudio.Bases;
using ETL;

namespace DBStudio.SqlCE
{
    /// <summary>
    /// Interaction logic for CEEntry.xaml
    /// </summary>
    public partial class CEEntry : UserControl
    {

        public CEEntry()
        {
            InitializeComponent();
            InitSelf();
            
            Debug.WriteLine(Translator.Culture);
        }

        private void InitSelf()
        {
            this.AllowDrop = true;

            this.Drop += (sender, e) =>
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
                                txtDbPath.Text = fileNames[0];
                            }
                        }

                    }
                };

            RefreshLastOpenedDbsStatus();

            this.txtDbPath.SpellCheck.IsEnabled = true;

            this.cmbOpenMode.Items.Add(new OpenModeClass() { mode = OpenMode.ReadWrite, modeDisplayName = "Read Write" });
            this.cmbOpenMode.Items.Add(new OpenModeClass() { mode = OpenMode.ReadOnly, modeDisplayName = "Read Only" });
            this.cmbOpenMode.Items.Add(new OpenModeClass() { mode = OpenMode.SharedRead, modeDisplayName = "Shared Read" });
            this.cmbOpenMode.Items.Add(new OpenModeClass() { mode = OpenMode.Exclusive, modeDisplayName = "Exclusive" });
            this.cmbOpenMode.SelectedIndex = 0;
        }

        private void RefreshLastOpenedDbsStatus()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.LastestOpenedDb))
            {
                
                butOpenLatest.Visibility = Visibility.Hidden;
            }
            else
            {
                if (File.Exists(Properties.Settings.Default.LastestOpenedDb))
                {
                    butOpenLatest.Visibility = Visibility.Visible;
                }
                else
                {
                    butOpenLatest.Visibility = Visibility.Hidden;
                }
            }

            if (!File.Exists(Config.SSCEDbHistoryFileFullPath))
            {
                this.butViewOpened.Visibility = Visibility.Hidden;
            }
            else
            {
                this.butViewOpened.Visibility = Visibility.Visible;

            }

        }

        private void butDetectFileVersion_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDbPath.Text))
            {
                "Please select db file first".Notify();
                return;
            }
            string db = txtDbPath.Text;
            if (!File.Exists(db))
            {
                "File not existed,please check it ".Show();
                return;
            }
            string result = String.Format("Version : Sql Ce {0}", UndocumentedFunctions.GetCeVersion(db));

            result.Notify();
        }

        private void butSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = MyGlobal.SQLCE_FILE_FILTER;
            if (of.ShowDialog() == true)
            {
                this.txtDbPath.Text = of.FileName;
            }
        }

        private void butChangePwd_Click(object sender, RoutedEventArgs e)
        {
            string db = this.txtDbPath.Text;
            if (string.IsNullOrEmpty(db))
            {
                "Please select database file first".Notify();
                return;
            }

            ChangeSdfPwd ch = new ChangeSdfPwd(db);
            ch.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if ((bool)ch.ShowDialog() == true)
            {
                txtPwd.Password = ch.X_NewPwd;
            }
        }

        private void butSync_Click(object sender, RoutedEventArgs e)
        {
            DataExchangeWizard dc = new DataExchangeWizard();
            dc.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dc.ShowDialog();
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            SqlCE.UI.CreateEmptyCEDB cb = new SqlCE.UI.CreateEmptyCEDB();
            cb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            cb.CreateDbAfterSuccesful += (dbFullPath,pwd) =>
                {
                    if (!string.IsNullOrEmpty(dbFullPath))
                    {
                        txtDbPath.Text = dbFullPath;

                    }
                    if (!string.IsNullOrEmpty(pwd))
                    {
                        txtPwd.Password = pwd;

                    }
                };

            cb.ShowDialog();
        }



        private void RefreshAfterCreateSuccess(string dbFullPath, string pwd)
        {
            if (!string.IsNullOrEmpty(dbFullPath))
            {
                txtDbPath.Text = dbFullPath;

            }
            if (!string.IsNullOrEmpty(pwd))
            {
                txtPwd.Password = pwd;

            }

        }

        [Description("Here the creation of MainEnginer should obey this rules ,do not refactor it ")]
        private void butOpen_Click(object sender, RoutedEventArgs e)
        {

            string dbName = txtDbPath.Text;
            string dbPwd = txtPwd.Password;

            if (string.IsNullOrEmpty(dbName))
            {
                "Please input database name".Notify();
                return;
            }

            if (Utility.DetectNetworkDriver.IsPathOnNetworkDrive(dbName))
            {
                "SQLCE do not support open from network".Notify();
                return;
            }
            OpenModeClass CurOpenMode = null;
            if (cmbOpenMode.SelectedItem == null)
            {
                CurOpenMode = cmbOpenMode.Items[0] as OpenModeClass;
            }
            else
            {
                CurOpenMode = cmbOpenMode.SelectedItem as OpenModeClass;
            }

            uint maxDbSize = (uint)txtMaxDBSize.Value;
            //If open db ok ,and get tables info ok , then do 
            //otherwise do nothing
            try
            {
                App.MainEngineer = new CoreEA.CoreE(CoreEA.CoreE.UsedDatabaseType.SqlCE35).X_Handler;
                App.MainEngineer.Open(new LoginInfo_SSCE() { DbName = dbName, Pwd = txtPwd.Password,
                    CurOpenMode = CurOpenMode.mode,
                    MaxDbSize=maxDbSize});
            }
            catch (SqlCeInvalidDatabaseFormatException sqe)
            {
                Debug.WriteLine("*****************"+sqe.Message);

#region 
                if (wf.DialogResult.Yes == wf.MessageBox.Show("Your database format is old version ,do you want to upgrade it ,so this software can modify it?", "Warning",
                    wf.MessageBoxButtons.YesNo, wf.MessageBoxIcon.Question, wf.MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        if (Properties.Settings.Default.AutoBackupOldVersionFileBeforeUpgrade)
                        {
                            if (!Directory.Exists(Config.AutoBackupFolder))
                            {
                                Directory.CreateDirectory(Config.AutoBackupFolder);
                            }

                            File.Copy(dbName, Config.AutoBackupFolder + Path.GetFileName(dbName), true);
                        }
                    }
                    catch(Exception ee)
                    {
#if DEBUG
                        throw ee;
#else
                        ee.Message.Show();

#endif

                    }
 
                    
                        SqlCeEngine d=new SqlCeEngine(CoreEA.ConnSTR.DbConnectionString.SSCE.GetSSCEConnectionString(
                            dbName,dbPwd,(bool)chkIsEncrypted.IsChecked,CurOpenMode));

                    try
                    {
                        d.Upgrade();
                        "Upgrade Successful".Notify();
                    }
                    catch(Exception dee)
                    {
                        dee.Message.Notify();
                        App.ResetMainEngineer();
                        return;
                    }
                    goto ReOpen;

                }
                else
                {
                    App.ResetMainEngineer();
                    return ;
                }

#endregion 

            }
            catch (Exception eee)
            {
                eee.Message.Notify();
            }

    ReOpen:
            if (App.MainEngineer.IsOpened)
            {
                App.MainEngineer.CurDatabase = dbName;
                App.MainEngineer.CurPwd = txtPwd.Password;

                //this.Hide();
                txtDbPath.Text = string.Empty;
                txtPwd.Password = string.Empty;
                Properties.Settings.Default.LastestOpenedDb = dbName;
                Properties.Settings.Default.LastestOpenedDbPwd = dbPwd;
                Properties.Settings.Default.Save();

                #region Save to Opened History Info

                SSCEObjects ssceItem = new SSCEObjects();
                ssceItem.DbFileFullPath = dbName;
                if (!SerializeClass.DatabaseHistoryInfo.SSCEHistory.IsContainSubValue(ssceItem.DbFileFullPath))
                {
                    HistoryObject oldObject = SerializeClass.DatabaseHistoryInfo;
                    ssceItem.LatestVisitTime = DateTime.Now;
                    oldObject.SSCEHistory.Add(ssceItem);

                    SerializeClass.DatabaseHistoryInfo = oldObject;
                }
                #endregion 


                RefreshLastOpenedDbsStatus();

                RibbionIDE ide = new RibbionIDE();

                ide.WindowState = WindowState.Maximized;
                ide.BringIntoView();
                ide.ShowDialog();
            }
            else
            {
                App.ResetMainEngineer();
            }
        }


        private void butViewOpened_Click(object sender, RoutedEventArgs e)
        {
            DynaControls.DbOpenedHistory history = new DynaControls.DbOpenedHistory();
            history.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            history.AfterSelectedValue += (args) =>
            {
                if (!string.IsNullOrEmpty(args))
                {
                    this.txtDbPath.Text = args;
                }
            };

            history.ShowDialog();
        }

        private void butOpenLatest_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.LastestOpenedDb))
            {
                return;
            }

            txtDbPath.Text = Properties.Settings.Default.LastestOpenedDb;
            txtPwd.Password = Properties.Settings.Default.LastestOpenedDbPwd;
            butOpen_Click(null, null);
        }

        private void txtDbPath_Drop(object sender, DragEventArgs e)
        {
            string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            if (fileNames.Length > 0)
            {
                txtDbPath.Text = fileNames[0];
            }

        }

        private void txtPwd_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                butOpen_Click(null, null);
            }
        }

        private void butOpenExampleData_Click(object sender, RoutedEventArgs e)
        {
            string exampleFile=AppDomain.CurrentDomain.SetupInformation.ApplicationBase+"ExampleData\\Northwind.sdf";
            if (!File.Exists(exampleFile))
            {
                "ErrorExampleDataLost".GetFromResourece().Show();
                return;
            }
            txtDbPath.Text = exampleFile;
            butOpen_Click(null, null);
        }

        private void butCompact_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                string connStr = CoreEA.ConnSTR.DbConnectionString.SSCE.GetSSCEConnectionString(txtDbPath.Text,
                    txtPwd.Password, (bool)chkIsEncrypted.IsChecked);
                string targetFile = Guid.NewGuid().ToString();
                targetFile+=".sdf";

                SqlCeEngine ce = new SqlCeEngine(connStr);
                string connStr2 = CoreEA.ConnSTR.DbConnectionString.SSCE.GetSSCEConnectionString(targetFile,
                    "",false);

                ce.Compact(connStr2);
                ("Compact successful to" + targetFile).Show();
            }
            catch(Exception eee)
            {
                eee.HandleMyException();
            }
        }
    }
}
