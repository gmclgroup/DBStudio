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
using System.Runtime.InteropServices;
using ETL;

namespace DBStudio
{
    public class SqlParser
    {
        IVBSQLParser sqlParser = null;

        [ComImport, Guid("8F6C7662-E8A1-11D0-B9B3-2A92D0000000")]
        internal class vbSQLParser
        {
        }

        [Guid("8F6C7661-E8A1-11D0-B9B3-2A92D0000000"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        internal interface IVBSQLParser
        {
            [DispId(1)]
            string ParseSQLSyntax(string sql, SyntaxConstants syntax);
        }

        public SqlParser()
        {
            try
            {
                sqlParser = (IVBSQLParser)new vbSQLParser();
            }
            catch (COMException comEx)
            {
                comEx.HandleMyException();
            }
        }

        public string Parse(string sql, SyntaxConstants syntax)
        {
            return sqlParser.ParseSQLSyntax(sql, syntax);
        }

        [Guid("969F3D60-F816-11D0-B9C3-0020AFC2CD36")]
        public enum SyntaxConstants
        {
            SqlServer = 1,
            Oracle = 4
        }
    }
}
