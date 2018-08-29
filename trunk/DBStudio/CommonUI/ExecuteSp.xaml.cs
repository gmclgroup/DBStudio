using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBStudio.GlobalDefine;

namespace DBStudio.CommonUI
{
    /// <summary>
    /// Interaction logic for ExecuteSp.xaml
    /// </summary>
    public partial class ExecuteSp : Window
    {
        /// <summary>
        /// Treeview args
        /// </summary>
        public TreeItemArgs CurrentSelectItemDataContext { get; set; }

        public ExecuteSp()
        {
            InitializeComponent();
        }


    }
}
