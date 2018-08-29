using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.Firebird
{
    class FirebirdCommandText:CommandTextBase
    {
        internal FirebirdCommandText() :
            base(new FriebirdRobot())
        {
        }
    }
}
