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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBStudio.BaseUI;
using ETL;
using System.Diagnostics;
using DBStudio.GlobalDefine;

namespace DBStudio.DocumentingDB
{
    /// <summary>
    /// Interaction logic for DocumentingDB.xaml
    /// </summary>
    public partial class DocumentingDBWizard : BaseFadeDialog
    {
        public WizardManager Manager { get; set; }

        public DocumentingDBWizard()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DDBWindow_Loaded);
        }


        void DDBWindow_Loaded(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.RemoveRange(0, contentGrid.Children.Count);
            contentGrid.Children.Add((UIElement)Manager.FirstStep);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void backCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Manager == null)
            {
                e.CanExecute = false;
                return;
            }
            Debug.WriteLine(Manager.CanPrevious);
            e.CanExecute = Manager.CanPrevious;
            Debug.WriteLine(e.CanExecute);
            Debug.WriteLine("--");
        }

        private void backCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            contentGrid.Children.RemoveRange(0, contentGrid.Children.Count);
            contentGrid.Children.Add((UIElement)Manager.PreviousStep);
            nextButton.Content = "NextButtonText".GetFromResourece();
        }

        private void nextCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Manager == null)
            {
                e.CanExecute = false;
                return;
            }
           
            if (nextButton.Content.ToString() == "FinishButtonText".GetFromResourece())
            {
                if (Manager.CurrentStep.CanLeave())
                {
                    e.CanExecute = true;
                }
            }
            else
            {
                if (Manager.CurrentStep.CanLeave())
                {
                    e.CanExecute = Manager.CanNext;
                }
            }
        }

        private void nextCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (nextButton.Content.ToString()== "FinishButtonText".GetFromResourece())
            {
                Manager.CurrentStep.Leave();
                DoFinish();
            }

            IStep nextStep=Manager.NextStep;
            if (nextStep == null)
            {
                return;
            }
            if (nextStep == Manager.LastStep)
            {
                nextButton.Content ="FinishButtonText".GetFromResourece();
            }
            else
            {
                nextButton.Content = "NextButtonText".GetFromResourece();   
            }

            contentGrid.Children.RemoveRange(0, contentGrid.Children.Count);
            contentGrid.Children.Add((UIElement)nextStep);

        }

        private void DoFinish()
        {
            Manager.Export();
            "TitleCompleteDocDbOK".GetFromResourece().Notify();

            
            //Save Category
            Manager.SaveCategory();

            Close();
        }
    }
}
