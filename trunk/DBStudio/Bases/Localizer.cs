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
using System.Windows.Markup;
using System.Threading;
using DBStudio.GlobalDefine;
using System.Globalization;
using DBStudio.Properties;

namespace DBStudio.Bases
{
    [MarkupExtensionReturnType(typeof(string))]
    public class TranslateExtension : MarkupExtension
    {
        private string key;
        private DependencyObject targetObject;
        private DependencyProperty targetProperty;

        public TranslateExtension(string key)
        {
            this.key = key;
            Translator.CultureChanged += (sender, args) =>
            {
                if (targetObject != null && targetProperty != null)
                {
                    //targetObject.SetValue(targetProperty,
                    //      Resources.ResourceManager.GetObject(key));
                    targetObject.SetValue(targetProperty,
                       key.GetFromResourece());
                }
            };

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetHelper = (IProvideValueTarget)serviceProvider.
                   GetService(typeof(IProvideValueTarget));
            targetObject = targetHelper.TargetObject as DependencyObject;
            targetProperty = targetHelper.TargetProperty as DependencyProperty;

            //return Resources.ResourceManager.GetObject(key);
            return key.GetFromResourece();
        }
    }

    public static class Translator
    {
        internal static event EventHandler CultureChanged;

        public static CultureInfo Culture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                Thread.CurrentThread.CurrentUICulture = value;
                if (CultureChanged != null)
                {
                    CultureChanged(null, EventArgs.Empty);
                }
            }
        }
    }
}
