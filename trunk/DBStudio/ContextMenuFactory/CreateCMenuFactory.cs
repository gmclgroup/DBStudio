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
using DBStudio.GlobalDefine;
using System.Windows.Controls;
using System.Diagnostics;
using CoreEA;

namespace DBStudio.ContextMenuFactory
{
    class CreateCMenuFactory
    {
        private IMenu currentMenuCreator;

        internal CreateCMenuFactory(CoreE.UsedDatabaseType dbType)
        {
            switch (dbType)
            {
                case CoreE.UsedDatabaseType.SqlServer:
                    currentMenuCreator = new SqlServerMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.MySql:
                    currentMenuCreator = new MySqlMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.OleDb:
                    currentMenuCreator = new OleDbMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.SqlCE35:
                    currentMenuCreator = new SqlCeMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.Sqlite:
                    currentMenuCreator = new SqliteMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.Oracle:
                    currentMenuCreator = new OracleMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.CSV:
                    currentMenuCreator = new CSVMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.Excel:
                    currentMenuCreator = new ExcelMenuCreator();
                    break;
                case CoreE.UsedDatabaseType.Effiproz:
                    currentMenuCreator = new EffiprozMenuCreator();
                    break;
                default:
                    Debug.Assert(true,"Invalid Menu Creator");
                    break;
            }
        }

        internal ContextMenu GetMenu(TreeItemArgs args)
        {
            ContextMenu rtMenu = null;

            Debug.Assert(currentMenuCreator != null);

            currentMenuCreator.X_CurItemArgs = args;
            if (args == null)
            {
                args = new TreeItemArgs()
                {
                    ItemType = TreeItemType.NotDefined,
                };
            }

            switch (args.ItemType)
            {
                case TreeItemType.Database:
                    rtMenu = currentMenuCreator.CreateMenuForDataBase();
                    break;
                case TreeItemType.Table:
                    rtMenu = currentMenuCreator.CreateMenuForTable();
                    break;
                case TreeItemType.Column:
                    rtMenu = currentMenuCreator.CreateMenuForColumn();
                    break;
                case TreeItemType.Index:
                    rtMenu = currentMenuCreator.CreateMenuForIndex();
                    break;
                case TreeItemType.ColumnParentNode:
                    rtMenu = currentMenuCreator.CreateMenuForColumnParentNode();
                    break;
                case TreeItemType.IndexParentNode:
                    rtMenu = currentMenuCreator.CreateMenuForIndexParentNode();
                    break;
                case TreeItemType.StoredProcedure:
                    rtMenu = currentMenuCreator.CreateMenuForStoredProducers();
                    break;
                case TreeItemType.StoredProcedureParentNode:
                    rtMenu = currentMenuCreator.CreateMenuForSPParentNode();
                    break;
                case TreeItemType.SystemViewNode:
                    rtMenu = currentMenuCreator.CreateMenuForSViewNode();
                    break;
                case TreeItemType.SystemView_SchemaNode:
                    rtMenu = currentMenuCreator.CreateMenuForSV_SchemaNode();
                    break;
                case TreeItemType.SystemView_Schema_ColumnNode:
                    rtMenu = currentMenuCreator.CreateMenuForSV_Schema_ColumnNode();
                    break;
                case TreeItemType.NotDefined:
                    rtMenu = currentMenuCreator.CreateMenuForNotDefined();
                    break;
                case TreeItemType.ViewParent:
                    rtMenu = currentMenuCreator.CreateMenuForNotDefined();
                    break;
                case TreeItemType.View:
                    rtMenu = currentMenuCreator.CreateMenuForView();
                    break;
                case TreeItemType.TriggersParent:
                    rtMenu = currentMenuCreator.CreateMenuForNotDefined();
                    break;
                case TreeItemType.Triggers:
                    rtMenu = currentMenuCreator.CreateMenuForNotDefined();
                    break;
                default:
                    rtMenu = currentMenuCreator.CreateMenuForNotDefined();
                    break;
            }

            if (rtMenu != null)
            {
                rtMenu.Tag = currentMenuCreator;
            }
            return rtMenu;
        }
    }
}
