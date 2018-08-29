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
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CoreEA;
using CoreEA.LoginInfo;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DBStudio.DocumentingDB
{
    [XmlRoot("DocumentingDatabaseRoot")]
    public class DocDbObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        [XmlIgnore]
        private string targetFile;
        [XmlIgnore]
        public string TargetFile
        {
            get
            {
                return targetFile;
            }
            set
            {
                targetFile = value;
            }
        }

        /// <summary>
        /// The Selected Table list need to generating
        /// </summary>
        [XmlIgnore]
        public List<string> SelectedTableNameCollection { get; set; }

        [XmlIgnore]
        private Exportor curExportor;
        [XmlIgnore]
        public Exportor CurExportor 
        {
            get
            {
                return curExportor;
            }
            set
            {
                curExportor = value;
            }
        }

        public DocDbObject()
        {
            DbObjectList = new List<DBObject>();
        }

        [XmlIgnore]
        private BaseLoginInfo loginInfo;
        [XmlIgnore]
        public BaseLoginInfo LoginInfo
        {
            get
            {
                return loginInfo;
            }
            set
            {
                loginInfo = value;
            }
        }

        [XmlIgnore]
        public CoreE.UsedDatabaseType SourceDbType
        {
            get;
            set;
        }

        [XmlElement("EachElement")]
        public List<DBObject> DbObjectList { get; set; }

    }

    public class DBObject
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int OrdinaryPosition { get; set; }
        public string Category { get; set; }
        public DateTime RevisionDate { get; set; }
        public string DbType { get; set; }
        public Int64 Length { get; set; }
        public int Format { get; set; }
        public string RevisionNote { get; set; }
        public string Description { get; set; }
        public bool IsUnique { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIndex { get; set; }
        public bool IsIdentity { get; set; }
    }
}
