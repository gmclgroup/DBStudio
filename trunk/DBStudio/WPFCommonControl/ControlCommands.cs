using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WPFCommonControl
{
    public static class ControlCommands
    {
        public static RoutedUICommand GetTableListCmd = new RoutedUICommand("GetTableListCmd", "GetTableListCmd", typeof(SqlServerLoginControl));

    }
}
