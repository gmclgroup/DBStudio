using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.Excel
{
    class ExcelCommandText:CommandTextBase
    {
        internal ExcelCommandText() :
            base(new ExcelRobot())
        {
        }
    }
}
