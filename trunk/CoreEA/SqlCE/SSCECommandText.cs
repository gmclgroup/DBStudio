using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SqlCE
{
    class SSCECommandText:CommandTextBase
    {
        internal SSCECommandText() :
            base(new SqlCERobot())
        {

        }
    }
}
