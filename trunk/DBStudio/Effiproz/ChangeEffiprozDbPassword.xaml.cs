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
using XLCS.Standard;

namespace DBStudio.Effiproz
{
    /// <summary>
    /// Interaction logic for ChangeEffiprozDbPassword.xaml
    /// </summary>
    public partial class ChangeEffiprozDbPassword : DBStudio.BaseUI.BaseFadeDialog
    {
        public string Result { get; set; }

        public ChangeEffiprozDbPassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = txtPwd.Password;
            DialogResult = true;
        }
    }
}
