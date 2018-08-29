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
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using DBStudio.CommonMethod;
using ETL;
using CoreEA;
using System.Resources;
using System.Diagnostics;
using System.Windows.Threading;
using MPL;
using System.Threading;
using DBStudio.GlobalDefine;
using MPL.Diagnostics;

namespace DBStudio
{
	public partial class App: System.Windows.Application
	{



        /// <summary>
        /// Reset when each db management page closing
        /// </summary>
        internal static void ResetMainEngineer()
        {
            if (mainEngineer != null)
            {
                mainEngineer.Close();
                mainEngineer.Dispose();
                mainEngineer = null;
            }
        }

        internal static Window MainEntry;

        public static ResourceManager MainResources { get; set; }

        private static ICoreEAHander mainEngineer = null;

        private SplashWindows splashWindow;

        public static ConfigInfo MyConfigInfo { get; set; }

        /// <summary>
        /// Original The MainEngineer is not singleton.
        /// But here ,we only permit one mainegineer exist.
        /// Anything used this engineer need call ResetMainEngineer() method
        /// </summary>
        public static ICoreEAHander MainEngineer
        {
            get
            {
                //Debug.Assert(_mainEngineer != null);

                return mainEngineer;
            }
            set
            {
                //Here if value is null , it was reset the main enginer 
                if (mainEngineer == null)
                {
                    mainEngineer = value;
                }
                else
                {
                    throw new Exception("GenerateCoreEAError".GetFromResourece());

                }
            }
        }

        App()
        {
            //StartupUri = new Uri("NewEntry.xaml",UriKind.Relative);
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.Logger().Info("1: First enter startup method");

            //ShowSplash();
            App.Current.Exit += new ExitEventHandler(Current_Exit);

            this.Logger().Info("2: Prepare for performance log");
#if DEBUG
            //PerformanceUtility.LoggingEnabled = true;
            PerformanceUtility.LoggingEnabled = false;
#endif

            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.LoadConfigFile);
            #region Load Config Info

            this.Logger().Info("3: First enter startup method");

            string configFilePath = MyGlobal.ConfigFilePath;
            if (!File.Exists(configFilePath))
            {
                ConfigInfo cInfo = new ConfigInfo();
                XLCS.Serialize.SerializedFile.SaveXml(configFilePath, cInfo);
            }

            App.MyConfigInfo = XLCS.Serialize.SerializedFile.OpenXml(configFilePath, typeof(ConfigInfo)) as ConfigInfo;

            #endregion
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.LoadConfigFile);

            this.Logger().Info("4: Set application shutdown mode");

            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            this.Logger().Info("5: Read .config file to get the entry point");


            MainEntry = new NewPortal();
            this.Logger().Info("5-1 : Load NewPortal");
            
            MainEntry.BringIntoView();
            this.Logger().Info("5-3 : Active Window");
            MainEntry.Activate();
            try
            {
                MainEntry.ShowDialog();
                this.Logger().Info("6 : Call base starup method");
                base.OnStartup(e);
            }
            catch(InvalidOperationException ee)
            {
                ee.Message.Show();
                App.Current.Shutdown();
            }
    
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                XLCS.Serialize.SerializedFile.SaveXml(MyGlobal.ConfigFilePath, App.MyConfigInfo);

#if DEBUG
                PerformanceUtility.DumpReport();
#endif
            }
            catch(Exception ee)
            {
#if DEBUG
                throw ee;

#else
                ee.Message.Show();
#endif
            }
        }

        private void ShowSplash()
        {
            splashWindow = new SplashWindows();
            splashWindow.Show();
            splashWindow.Activate();
        }

   
	}
}
