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

using ETL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLCS.Serialize;

namespace DBStudio.DocumentingDB.Exportors
{
    public class ExcelExportor:Exportor
    {
        internal override bool Export(string targetFile, object targetObject)
        {
            try
            {
                //ExcelDocument doc = new ExcelDocument();
                //ExcelTable table = doc.AddTable("Table 1");

                
                //List<DBObject> myData = ((DocDbObject)targetObject).DbObjectList as List<DBObject>;

                //foreach (var item in myData)
                //{
                //    ExcelRow row = table.AddRow();
                //    row.AddCell(item.TableName);
                //    row.AddCell(item.ColumnName);
                //    row.AddCell(item.DbType);
                //    row.AddCell(item.Length);
                //    row.AddCell(item.Description);
                //    row.AddCell(item.Format);
                //    row.AddCell(item.OrdinaryPosition);
                //    row.AddCell(eDateType.DateTime,item.RevisionDate);
                //    row.AddCell(item.RevisionNote);
                //}
         
                //doc.Save(targetFile);

                SerializedFile.SaveXml(targetFile, targetObject);

                return true;
            }
            catch (Exception ee)
            {
                ee.ThrowMyCustomizedException();
                return false;
            }
        }
    }
}
