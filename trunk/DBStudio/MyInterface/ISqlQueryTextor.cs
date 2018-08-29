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
using wf=System.Windows.Forms;

namespace DBStudio.MyInterface
{
    interface ISqlQueryTextor
    {
        /// <summary>
        /// Should implement in the winform mode
        /// </summary>
        event wf.KeyEventHandler MyKeyDown;
        /// <summary>
        /// Should implement in the WPF
        /// </summary>
        event System.Windows.Input.KeyEventHandler MyAdvKeyDown;
        void Dispose();
        string Text { set; get; }
        void Focus();
        string SelectedText { get; }
        System.Drawing.Font Font { set; get; }

        /// <summary>
        /// This mean whether format the text or not 
        /// True mean format
        /// False mean not to format
        /// Recommed set false when sql script content large than 100k
        /// Otherwise the UI will be locked according to large amount of text block need to formatted
        /// </summary>
        bool IsAllowFormatSqlContent { get; set; }

    }
}
