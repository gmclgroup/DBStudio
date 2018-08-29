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
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using DBStudio.CommonMethod;
using XLCS.Serialize;
using DBStudio.GlobalDefine;
using CoreEA;
using System.Xml.Serialization;
using ETL;
using System.Diagnostics;

namespace DBStudio.Utility
{
    internal class SerializeClass
    {



        /// <summary>
        /// Opened Database History Info
        /// </summary>
        /// <param name="info"></param>
        public static HistoryObject DatabaseHistoryInfo
        {
            get
            {
                if (!File.Exists(Config.SSCEDbHistoryFileFullPath))
                {
                    StreamWriter sw = new StreamWriter(Config.SSCEDbHistoryFileFullPath);
                    XmlSerializer xs = new XmlSerializer(typeof(HistoryObject));
                    xs.Serialize(sw, new HistoryObject());
                    sw.Close();
                    sw.Dispose();
                }

                FileStream file = new FileStream(Config.SSCEDbHistoryFileFullPath, FileMode.Open);

                XmlSerializer ssssss = new XmlSerializer(typeof(HistoryObject));
                HistoryObject ret = ssssss.Deserialize(file) as HistoryObject;
                file.Close();
                file.Dispose();

                Debug.Assert(null != ret);
                return ret;
            }
            set
            {

                try
                {
                    StreamWriter sw = new StreamWriter(Config.SSCEDbHistoryFileFullPath);
                    XmlSerializer xs = new XmlSerializer(typeof(HistoryObject));
                    xs.Serialize(sw, value);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();

                }
                catch (Exception ee)
                {
                    ee.Message.Show();
                }
            }
        }

        #region Old Methods to save opned database information
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="list"></param>
        //internal static void SaveHistory(Dictionary<string, CoreEA.CoreE.UsedDatabaseType> list)
        //{
        //    SerializedFile.SaveFile(Config.SSCEDbHistoryFileFullPath, list);
        //}

        ///// <summary>
        ///// Must return not null object
        ///// </summary>
        ///// <returns></returns>
        //internal static Dictionary<string, CoreEA.CoreE.UsedDatabaseType> LoadHistoryListObjectFromFile()
        //{
        //    Dictionary<string, CoreEA.CoreE.UsedDatabaseType> info =
        //        SerializedFile.OpenFile(Config.SSCEDbHistoryFileFullPath)
        //        as Dictionary<string, CoreEA.CoreE.UsedDatabaseType>;

        //    if (info == null)
        //    {
        //        info = new Dictionary<string, CoreEA.CoreE.UsedDatabaseType>();
        //    }

        //    return info;
        //}

        //internal abstract class IHistoryObjects
        //{
        //};

        //internal class SqlServerHistoryObjects : IHistoryObjects
        //{
        //    public string Server { get; set; }
        //    public DateTime LatestAccesTime { get; set; }
        //}

        //internal class SSCEHistoryObjects:IHistoryObjects
        //{
        //    public string Name { get; set; }
        //    public bool IsExisted { get; set; }
        //    public DateTime LatestVisitTime { get; set; }
        //}


        ///// <summary>
        ///// This Method wrapped the GetHistoryList method.
        ///// Give rich content show to the user
        ///// Recommend using this method with wpf databinding
        ///// </summary>
        ///// <returns></returns>
        //internal static List<IHistoryObjects> GetHistoryObjects(CoreE.UsedDatabaseType dbType)
        //{
        //    List<IHistoryObjects> ot = new List<IHistoryObjects>();

        //    Dictionary<string,CoreE.UsedDatabaseType> result = LoadHistoryListObjectFromFile();
        //    foreach (KeyValuePair<string, CoreE.UsedDatabaseType> item in result)
        //    {
        //        switch (dbType)
        //        {
        //            case CoreE.UsedDatabaseType.SqlServer:
        //                if (dbType == item.Value)
        //                {
        //                    ot.Add(new SqlServerHistoryObjects()
        //                    {
        //                        Server = item.Key,
        //                    }
        //                    );
        //                }
        //                break;
        //            case CoreE.UsedDatabaseType.MySql:
        //                break;
        //            case CoreE.UsedDatabaseType.OleDb:
        //                break;
        //            case CoreE.UsedDatabaseType.SqlCE35:
        //                if (dbType == item.Value)
        //                {
        //                    ot.Add(new SSCEHistoryObjects()
        //                    {
        //                        Name = item.Key,
        //                        IsExisted = File.Exists(item.Key),
        //                        LatestVisitTime = new FileInfo(item.Key).LastAccessTimeUtc,
        //                    });
        //                }
        //                break;
        //            case CoreE.UsedDatabaseType.Oracle:
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return ot;
        //}

        #endregion
    }
}
