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
using DBStudio.Bases;
using CodeBoxControl.Decorations;
using DBStudio.MyInterface;

namespace DBStudio.MyInterfaceImplementations
{
    /// <summary>
    /// Interaction logic for MyAdvTextEditor.xaml
    /// </summary>
    public partial class MyAdvTextEditor : UserControl, ISqlQueryTextor
    {
        public MyAdvTextEditor()
        {
            InitializeComponent();
            //Default value is to set true.
            //Allow format text style with AdvTextEditor
            //This value can be set outer .
            IsAllowFormatSqlContent = true;
            //Defult to set sql style
            // Color Built in functions Magenta
            MultiRegexWordDecoration builtInFunctions = new MultiRegexWordDecoration();
            builtInFunctions.Brush = new SolidColorBrush(Colors.Magenta);
            builtInFunctions.Words.AddRange(GetBuiltInFunctions());
            myBox.Decorations.Add(builtInFunctions);

            //Color global variables Magenta
            MultiStringDecoration globals = new MultiStringDecoration();
            globals.Brush = new SolidColorBrush(Colors.Magenta);
            globals.Strings.AddRange(GetGlobalVariables());
            myBox.Decorations.Add(globals);

            //Color most reserved words blue
            MultiRegexWordDecoration bluekeyWords = new MultiRegexWordDecoration();
            bluekeyWords.Brush = new SolidColorBrush(Colors.Blue);
            bluekeyWords.Words.AddRange(GetBlueKeyWords());
            myBox.Decorations.Add(bluekeyWords);

            MultiRegexWordDecoration grayKeyWords = new MultiRegexWordDecoration();
            grayKeyWords.Brush = new SolidColorBrush(Colors.Gray);
            grayKeyWords.Words.AddRange(GetGrayKeyWords());
            myBox.Decorations.Add(grayKeyWords);

            MultiRegexWordDecoration dataTypes = new MultiRegexWordDecoration();
            dataTypes.Brush = new SolidColorBrush(Colors.Blue);
            dataTypes.Words.AddRange(GetDataTypes());
            myBox.Decorations.Add(dataTypes);


            MultiRegexWordDecoration systemViews = new MultiRegexWordDecoration();
            systemViews.Brush = new SolidColorBrush(Colors.Green);
            systemViews.Words.AddRange(GetSystemViews());
            myBox.Decorations.Add(systemViews);

            MultiStringDecoration operators = new MultiStringDecoration();
            operators.Brush = new SolidColorBrush(Colors.Gray);
            operators.Strings.AddRange(GetOperators());
            myBox.Decorations.Add(operators);


            RegexDecoration quotedText = new RegexDecoration();
            quotedText.Brush = new SolidColorBrush(Colors.Red);
            quotedText.RegexString = "'.*?'";
            myBox.Decorations.Add(quotedText);

            RegexDecoration nQuote = new RegexDecoration();
            //nQuote.DecorationType = EDecorationType.TextColor;
            nQuote.Brush = new SolidColorBrush(Colors.Red);
            nQuote.RegexString = "N''";
            myBox.Decorations.Add(nQuote);


            //Color single line comments green
            RegexDecoration singleLineComment = new RegexDecoration();
            singleLineComment.DecorationType = EDecorationType.TextColor;
            singleLineComment.Brush = new SolidColorBrush(Colors.Green);
            singleLineComment.RegexString = "--.*";
            myBox.Decorations.Add(singleLineComment);

            //Color multiline comments green
            RegexDecoration multiLineComment = new RegexDecoration();
            multiLineComment.DecorationType = EDecorationType.TextColor;
            multiLineComment.Brush = new SolidColorBrush(Colors.Green);
            multiLineComment.RegexString = @"(?s:/\*.*?\*/)";
            myBox.Decorations.Add(multiLineComment);

            myBox.FontSize = Properties.Settings.Default.SqlEditorFontSize;
            myBox.FontFamily = new FontFamily(Properties.Settings.Default.SqlEditorFontName);
        }



        #region String data for text coloring
        public string[] GetBuiltInFunctions()
        {
            string[] funct = { "parsename", "db_name", "object_id", "count", "ColumnProperty", "LEN",
                             "CHARINDEX" ,"isnull" , "SUBSTRING" };
            return funct;
        }

        public string[] GetGlobalVariables()
        {

            string[] globals = { "@@fetch_status" };
            return globals;

        }

        public string[] GetDataTypes()
        {
            string[] dt = { "int", "sysname", "nvarchar", "char" };
            return dt;

        }


        public string[] GetBlueKeyWords() // List from 
        {
            string[] res = {"ADD","EXISTS","PRECISION","ALL","EXIT","PRIMARY","ALTER","EXTERNAL",
                            "PRINT","FETCH","PROC","ANY","FILE","PROCEDURE","AS","FILLFACTOR",
                            "PUBLIC","ASC","FOR","RAISERROR","AUTHORIZATION","FOREIGN","READ","BACKUP",
                            "FREETEXT","READTEXT","BEGIN","FREETEXTTABLE","RECONFIGURE","BETWEEN","FROM",
                            "REFERENCES","BREAK","FULL","REPLICATION","BROWSE","FUNCTION","RESTORE",
                            "BULK","GOTO","RESTRICT","BY","GRANT","RETURN","CASCADE","GROUP","REVERT",
                            "CASE","HAVING","REVOKE","CHECK","HOLDLOCK","RIGHT","CHECKPOINT","IDENTITY",
                            "ROLLBACK","CLOSE","IDENTITY_INSERT","ROWCOUNT","CLUSTERED","IDENTITYCOL",
                            "ROWGUIDCOL","COALESCE","IF","RULE","COLLATE","IN","SAVE","COLUMN","INDEX",
                            "SCHEMA","COMMIT","INNER","SECURITYAUDIT","COMPUTE","INSERT","SELECT",
                            "CONSTRAINT","INTERSECT","SESSION_USER","CONTAINS","INTO","SET","CONTAINSTABLE",
                            "SETUSER","CONTINUE","JOIN","SHUTDOWN","CONVERT","KEY","SOME","CREATE",
                            "KILL","STATISTICS","CROSS","LEFT","SYSTEM_USER","CURRENT","LIKE","TABLE",
                            "CURRENT_DATE","LINENO","TABLESAMPLE","CURRENT_TIME","LOAD","TEXTSIZE",
                            "CURRENT_TIMESTAMP","MERGE","THEN","CURRENT_USER","NATIONAL","TO","CURSOR",
                            "NOCHECK","TOP","DATABASE","NONCLUSTERED","TRAN","DBCC","NOT","TRANSACTION",
                            "DEALLOCATE","NULL","TRIGGER","DECLARE","NULLIF","TRUNCATE","DEFAULT","OF",
                            "TSEQUAL","DELETE","OFF","UNION","DENY","OFFSETS","UNIQUE","DESC", "ON", 
                            "UNPIVOT","DISK","OPEN","UPDATE","DISTINCT","OPENDATASOURCE","UPDATETEXT",
                            "DISTRIBUTED","OPENQUERY","USE","DOUBLE","OPENROWSET","USER","DROP","OPENXML",
                            "VALUES","DUMP","OPTION","VARYING","ELSE","OR","VIEW","END","ORDER","WAITFOR",
                            "ERRLVL","OUTER","WHEN","ESCAPE","OVER","WHERE","EXCEPT","PERCENT","WHILE",
                            "EXEC","PIVOT","WITH","EXECUTE","PLAN","WRITETEXT", "GO", "ANSI_NULLS",
                            "NOCOUNT", "QUOTED_IDENTIFIER", "master"};

            return res;
        }


        public string[] GetGrayKeyWords()
        {
            string[] res = { "AND", "Null", "IS" };

            return res;

        }

        public string[] GetOperators()
        {
            string[] ops = { "=", "+", ".", ",", "-", "(", ")", "*", "<", ">" };

            return ops;

        }

        public string[] GetSystemViews()
        {
            string[] views = { "syscomments", "sysobjects", "sys.syscomments" };
            return views;
        }

        #endregion

        #region ISqlQueryTextor Members
#pragma warning disable 0067
        public event System.Windows.Forms.KeyEventHandler MyKeyDown;
#pragma warning restore 0067
        public event System.Windows.Input.KeyEventHandler MyAdvKeyDown;
        public void Dispose()
        {
            
        }

        public bool IsAllowFormatSqlContent
        {
            get
            {
                return this.myBox.IsAllowFormatSqlContent;
            }
            set
            {
                this.myBox.IsAllowFormatSqlContent = value;
            }
        }

        public string Text
        {
            get
            {
                return myBox.Text;
            }
            set
            {
                myBox.Text = value;
            }
        }

        public new void Focus()
        {
            myBox.Focus();
            
        }

        public string SelectedText
        {
            get
            {
                return myBox.SelectedText;
            }
        }

        /// <summary>
        /// Looks like do nothing
        /// </summary>
        public System.Drawing.Font Font
        {
            get
            {
                return new System.Drawing.Font(myBox.Name,(float) myBox.FontSize);
            }
            set
            {
                myBox.FontSize=value.Size;
                myBox.Name=value.Name;
            }
        }

        #endregion

        private void myBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (null != MyAdvKeyDown)
            {
                MyAdvKeyDown(sender, e);
            }
        }
    }
}
