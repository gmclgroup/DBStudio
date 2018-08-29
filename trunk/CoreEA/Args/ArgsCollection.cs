
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreEA.Args
{

    /// <summary>
    /// Swith open db mode args
    /// </summary>
    public class OpenModeClass
    {
        private OpenMode _mode;
        /// <summary>
        /// 
        /// </summary>
        public OpenMode mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private string _modeDisplayName;
        /// <summary>
        /// Read Write
        /// Read Only
        /// Shared Read
        /// Exclusive
        /// </summary>
        public string modeDisplayName
        {
            get { return _modeDisplayName; }
            set { _modeDisplayName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return modeDisplayName;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum CurDbServerConnMode
    {
        /// <summary>
        /// 
        /// 
        /// </summary>
        SqlServer2000,
        /// <summary>
        /// 
        /// </summary>
        Local,
        /// <summary>
        /// 
        /// </summary>
        Standard,
        
        /// <summary>
        /// 
        /// </summary>
        SqlServer2005Express,
        /// <summary>
        /// 
        /// </summary>
        OleDb,
        /// <summary>
        /// 
        /// </summary>
        AttachFile,
        /// <summary>
        /// 
        /// </summary>
        SqlServer2008Express,
        /// <summary>
        /// Support Mars(Mutliple Active Result)
        /// This type will solve such error 
        /// [There is already an open DataReader associated with this Command which must be closed first]
        /// </summary>
        SqlServer2005,
    };

    /// <summary>
    /// For SSCE Opening Mode
    /// </summary>
    public enum OpenMode
    {
        /// <summary>
        ///   	Permits multiple processes to open and modify the database.
        /// </summary>
        ReadWrite,
        /// <summary>
        /// Lets you open a read-only copy of the database
        /// </summary>
        ReadOnly,
        /// <summary>
        /// Does not permit other processes to open or modify the database.
        /// </summary>
        Exclusive,
        /// <summary>
        /// Permits other processes to read, but not modify, the database while you have it open.
        /// </summary>
        SharedRead,

    };

    /// <summary>
    /// Args for create table and modify table schema using
    /// </summary>
    public class CreateTableArgs
    {
        private string _fieldName;
        /// <summary>
        /// 
        /// </summary>
        public string fieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }
        private string _dataType;
        /// <summary>
        /// 
        /// </summary>
        public string dataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        private int _dataLength;
        /// <summary>
        /// 
        /// </summary>
        public int dataLength
        {
            get { return _dataLength; }
            set { _dataLength = value; }
        }

        private bool _isUnique;
        /// <summary>
        /// 
        /// </summary>
        public bool isUnique
        {
            get { return _isUnique; }
            set { _isUnique = value; }
        }
        private bool _isPrimaryKey;
        /// <summary>
        /// 
        /// </summary>
        public bool isPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }

        private bool _allowNulls;
        /// <summary>
        /// 
        /// </summary>
        public bool allowNulls
        {
            get { return _allowNulls; }
            set { _allowNulls = value; }
        }
    }
}
