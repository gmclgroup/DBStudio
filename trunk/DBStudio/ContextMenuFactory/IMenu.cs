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
using System.Windows.Controls;
using DBStudio.GlobalDefine;

namespace DBStudio.ContextMenuFactory
{
    interface IMenu
    {
        TreeItemArgs X_CurItemArgs { get; set; }

        /// <summary>
        /// 执行命令或者显示命令事件
        /// </summary>
        event EventHandler ItemClickWithSqlCmdEvent;

        /// <summary>
        /// 显示结果事件
        /// </summary>
        event EventHandler ItemClickWithResult;

        /// <summary>
        /// 刷新系统视图节点
        /// </summary>
        event EventHandler RefreshSystemViewNodeEvent;
        /// <summary>
        /// 刷新所有节点
        /// </summary>
        event EventHandler RefreshAllNodeEvent;

        #region For ContextMenu Using

        ContextMenu CreateMenuForDataBase();

        ContextMenu CreateMenuForTable();

        ContextMenu CreateMenuForStoredProducers();

        ContextMenu CreateMenuForIndexParentNode();

        ContextMenu CreateMenuForColumnParentNode();

        ContextMenu CreateMenuForIndex();

        ContextMenu CreateMenuForColumn();

        ContextMenu CreateMenuForSPParentNode();

        ContextMenu CreateMenuForSViewNode();

        ContextMenu CreateMenuForSV_SchemaNode();

        ContextMenu CreateMenuForSV_Schema_ColumnNode();

        ContextMenu CreateMenuForNotDefined();

        ContextMenu CreateMenuForView();
        #endregion 

        /// <summary>
        /// Copy table ,schema and data
        /// </summary>
        /// <param name="oldTableName"></param>
        /// <returns></returns>
        bool CopyTable(string oldTableName, string newTableName);

    }
}
