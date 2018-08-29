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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;
using DBStudio.BaseUI;
using DBStudio.GlobalDefine;
namespace DBStudio.UI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>

    public partial class About : BaseFadeDialog
    {

        public About()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(About_Loaded);


            txtVersion.Text = new System.Version(((AssemblyFileVersionAttribute)Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version).ToString();
        }

        void About_Loaded(object sender, RoutedEventArgs e)
        {
            List<MemberProfile> teamMemberList = new List<MemberProfile>();

            teamMemberList.Add(new MemberProfile()
             {
                 Name = "Leon",
                 RoleName = "LabelCoordinator".GetFromResourece(),
                 ProfileDetail = "ms44cnleon@gmail.com",
                 ProfileIcon = new BitmapImage(new Uri("..\\Images\\Profiles\\leon.png", UriKind.RelativeOrAbsolute)),
             }
             );
            teamMemberList.Add(new MemberProfile()
            {
                Name = "subbu",
                RoleName = "Developer",
                ProfileDetail = "subbu@etisbew.com",
                ProfileIcon = new BitmapImage(new Uri("..\\Images\\Profiles\\subbu.jpg", UriKind.RelativeOrAbsolute)),
            }
 );
            teamMemberList.Add(new MemberProfile()
            {
                Name = "Florian Haag",
                RoleName = "Graphic Designer",
                ProfileDetail = "fhaag@users.sourceforge.net",
                ProfileIcon = new BitmapImage(new Uri("Images\\execute.png", UriKind.RelativeOrAbsolute)),
            }
);
            teamMemberList.Add(new MemberProfile()
            {
                Name = "Sjur Hamre",
                RoleName = "Web Designer",
                ProfileDetail = "smhamre@gmail.com",
                ProfileIcon = new BitmapImage(new Uri("Images\\execute.png", UriKind.RelativeOrAbsolute)),
            }
);
            profileGrid.DataContext = teamMemberList;
        }




    }

    /// <summary>
    /// Profile information about team member
    /// </summary>
    public class MemberProfile
    {
        public string Name { get; set; }
        public string RoleName { get; set; }
        public ImageSource ProfileIcon { get; set; }
        public string ProfileDetail { get; set; }
    }
}