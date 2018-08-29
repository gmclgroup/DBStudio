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

namespace DBStudio.CommandFactory
{
    internal abstract class CmdBase:ICmd
    {

        protected CmdBase()
        {

        }

        public class MyCmdArgs : EventArgs
        {
            public bool ExecuteOK { get; set; }

            public string ErrorMsg { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool IsNeedRefreshUI { get; set; }
        }

        public CoreEA.ICoreEAHander CurDBE { get; set; }

        public event EventHandler CmdExecutedOk;


        public void NotifyErrorAdnRefreshClientUI(string p)
        {
            if (CmdExecutedOk != null)
            {
                CmdExecutedOk(null, new MyCmdArgs()
                {
                    ExecuteOK = false,
                    ErrorMsg=p,
                    IsNeedRefreshUI = true,
                });
            }
        }

        public void NotifySuccessfulAndRefreshClientUI()
        {
            if (CmdExecutedOk != null)
            {
                CmdExecutedOk(null, new MyCmdArgs()
                {
                    ExecuteOK = true,
                    IsNeedRefreshUI=true,
                });
            }
        }

        public void NotifyErrorAndNotTORefreshClientUI(string p)
        {
            if (CmdExecutedOk != null)
            {
                CmdExecutedOk(null, new MyCmdArgs()
                {
                    ExecuteOK = false,
                    ErrorMsg = p,
                    IsNeedRefreshUI = false,
                });
            }
        }


        public void NotifySuccessful()
        {
            if (CmdExecutedOk != null)
            {
                CmdExecutedOk(null, new MyCmdArgs()
                {
                    ExecuteOK = true,
                    IsNeedRefreshUI = false,
                });
            }
        }

        public void NotifyError(string errorMsg)
        {
            if (CmdExecutedOk != null)
            {
                CmdExecutedOk(null, new MyCmdArgs()
                {
                    ExecuteOK = false,
                    ErrorMsg=errorMsg,
                });
            }
        }

        public abstract object ExecuteWithResult(string cmdStr);

        //public abstract object ExecuteTransList();

    }
}
