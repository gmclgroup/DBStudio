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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBStudio.CommonMethod;
using System.Diagnostics;
using System.Data.Common;
using System.Data;
using DBStudio.AdditionUI;
using System.Collections.ObjectModel;
using DBStudio.BaseUI;
using XLCS.Common;
using CustomControl.NewXLAGControl;
using System.Data.SqlServerCe;
using wf = System.Windows.Forms;
using System.IO;
using DBStudio.SqlCE.UI;
using DBStudio.SqlCE.AdditionUI;
using System.ServiceProcess;
using ETL;
using DBStudio.GlobalDefine;
using Microsoft.Windows.Controls;
using DBStudio.UI;
using System.Threading;
using System.Linq;
using System.Data.Linq;
using DBStudio.CommandFactory;
using System.Windows.Threading;
using DBStudio.ContextMenuFactory;
using DBStudio.SqlServer.DataExchange;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using DBStudio.CommonUI;
using DBStudio.SqlCE.Tools;
using DBStudio.SqlCE.Sync;
using Microsoft.Windows.Controls.Ribbon;
using CoreEA.LoginInfo;
using DBStudio.Utility;
using CoreEA.SchemaInfo;
using DBStudio.Bases;
using System.Windows.Media.Animation;
using DBStudio.CreateTableSchemaFactory;
using MPL.MyControls;

namespace DBStudio
{
    /// <summary>
    /// For Create Sql Command Ribbion Tab 
    /// </summary>
    public partial class RibbionIDE
    {
        #region CreateSql Script

        public static RibbonCommand CreateScriptCmd = new RibbonCommand();
        /// <summary>
        /// Same to CreateScriptCmd 
        /// but generate the sql script into one file
        /// </summary>
        public static RibbonCommand CreateScriptOfAllTableCmd = new RibbonCommand();

        #endregion


        private void InitSqlScriptCmd()
        {
            #region Generate Sql Script


            CreateScriptCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            CreateScriptCmd.LabelTitle = "TitleGenerateCreateScript".GetFromResourece();
            this.CommandBindings.Add(new CommandBinding(CreateScriptCmd, CreateScriptCmd_Execute, CreateScriptCmd_CanExecute));

            CreateScriptOfAllTableCmd.LargeImageSource = new BitmapImage(new Uri(@"Images\SqlCommandsImages\CreateTable.png", UriKind.Relative));
            CreateScriptOfAllTableCmd.LabelTitle = "TitleGenerateCreateScriptAll".GetFromResourece();
            this.CommandBindings.Add(new CommandBinding(CreateScriptOfAllTableCmd, CreateScriptOfAllTableCmd_Execute, CreateScriptOfAllTableCmd_CanExecute));


            #endregion
        }

        #region Sql Script Generate
        void CreateScriptCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentTreeArgs == null)
      || (CurrentTreeArgs.TableName.IsEmpty())
      )
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        void CreateScriptCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            string createSqlScript = string.Empty;

            BaseTableSchema schemaInfo = App.MainEngineer.GetTableSchemaInfoObject(CurrentTreeArgs.TableName);
            createSqlScript = App.MainEngineer.GetCreateTableString(schemaInfo);

            CurrentSqlQueryEditor.Text = createSqlScript;
        }

        void CreateScriptOfAllTableCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void CreateScriptOfAllTableCmd_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder st = new StringBuilder();
            foreach (var item in App.MainEngineer.GetTableListInDatabase())
            {
                BaseTableSchema schemaInfo = App.MainEngineer.GetTableSchemaInfoObject(item);
                st.Append(App.MainEngineer.GetCreateTableString(schemaInfo));
                st.AppendLine();

            }
            CurrentSqlQueryEditor.Text = st.ToString();
        }
        #endregion

        private RibbonTab GetGenerateSqlScriptCommandTab()
        {
            RibbonTab tab = new RibbonTab();
            try
            {
                tab.Label = "TitleGenerateScript".GetFromResourece();
                RibbonGroup groupCommon = new RibbonGroup();
                tab.Groups.Add(groupCommon);

                groupCommon.Controls.Add(new RibbonButton() { Command = CreateScriptCmd });
                groupCommon.Controls.Add(new RibbonButton() { Command = CreateScriptOfAllTableCmd });
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }

            return tab;
        }
    }
}
