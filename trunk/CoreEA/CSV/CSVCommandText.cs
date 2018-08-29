using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.CSV
{
    class CSVCommandText:CommandTextBase
    {
        internal CSVCommandText() :
            base(new CSVRobot())
        {
        }
    }
}
