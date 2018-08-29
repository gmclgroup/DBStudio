using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.Effiproz
{
    class EffiprozCommandText:CommandTextBase
    {
        internal EffiprozCommandText() :
            base(new EffiprozRobot())
        {
        }

    }
}
