using System;
using System.Collections.Generic;
using System.Text;
using CoreEA.LoginInfo;

namespace CoreEA.Invalidation
{
    class InvalidationBase : CoreEA.Invalidation.IInvalidation
    {

        private bool isFilterSqlKeyWords=false;
        /// <summary>
        /// Indicate whether filter the sql keywords or not . 
        /// If true , EA will replace sql sentences (such as Insert into ,Update,etc..)
        /// with quote string (\'insert into')
        /// 
        /// Recomment : set true;
        /// </summary>
        public bool IsFilterSqlKeyWords
        {
            get { return isFilterSqlKeyWords; }
            set { isFilterSqlKeyWords = value; }
        }

        private bool isMaskTheInvalidChar = false;
        /// <summary>
        /// If set true, the invalidation processin will 
        /// convert the invalid char to mash char 
        /// Such as if ' is invalid char , it will convert to "
        /// </summary>
        public virtual bool IsMashTheInvalidChar
        {
            get { return isMaskTheInvalidChar; }
            set { isMaskTheInvalidChar = value; }
        }


        static readonly int NotFindChar=-1;

        /// <summary>
        /// Check whether the sql cmd is invalid or not
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInvalidSql(string sql)
        {
            if ((sql == null) || (sql.Length == 0))
            {
                GlobalDefine.SP.LastErrorMsg = GlobalDefine.SP.X_InvalidParameters;
                return true;
            }

            //
            if (IsFilterSqlKeyWords)
            {
                FilterSqlKeywords(sql);
            }
            return false;
        }

        /// <summary>
        /// Do filter sql keywords ,Current not complete
        /// </summary>
        /// <param name="sql"></param>
        private void FilterSqlKeywords(string sql)
        {
            
        }

        /// <summary>
        /// Check whether the arguments send to the method is invalid or not
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual bool IsInvalidArguments(string arg)
        {
            //Notice here , can not be checked null or empty
            //if ((arg == null) || (arg.Length == 0))
            //{
            //    GlobalDefine.SP.LastErrorMsg = GlobalDefine.SP.X_InvalidParameters;
            //    return true;
            //}


            if (arg.IndexOf("'") != NotFindChar)
            {
                if (IsMashTheInvalidChar)
                {
                    arg = arg.Replace("'", "\"");
                }
                else
                {
                    return true;
                }
            }

            return false;

        }

        public virtual bool InvalidLogin(BaseLoginInfo info)
        {
            if (info == null)
            {
                return true;
            }

            return false;
        }

        #region SqlInjection
        private static readonly System.Text.RegularExpressions.Regex regSystemThreats =
                new System.Text.RegularExpressions.Regex(@"\s?;\s?|\s?drop\s|\s?grant\s|^'|\s?--|\s?union\s|\s?delete\s|\s?truncate\s|\s?sysobjects\s?|\s?xp_.*?|\s?syslogins\s?|\s?sysremote\s?|\s?sysusers\s?|\s?sysxlogins\s?|\s?sysdatabases\s?|\s?aspnet_.*?|\s?exec\s?|",
                    System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        /// <summary>
        /// A helper method to attempt to discover known SqlInjection attacks.  
        /// For use when using one of the flexible non-parameterized access methods, such as GetPaged()
        /// </summary>
        /// <param name="whereClause">string of the whereClause to check</param>
        /// <returns>true if found, false if not found </returns>
        public virtual bool DetectSqlInjection(string whereClause)
        {
            return regSystemThreats.IsMatch(whereClause);
        }

        /// <summary>
        /// A helper method to attempt to discover known SqlInjection attacks.  
        /// For use when using one of the flexible non-parameterized access methods, such as GetPaged()
        /// </summary>
        /// <param name="whereClause">string of the whereClause to check</param>
        /// <param name="orderBy">string of the orderBy clause to check</param>
        /// <returns>true if found, false if not found </returns>
        public virtual bool DetectSqlInjection(string whereClause, string orderBy)
        {
            return regSystemThreats.IsMatch(whereClause) || regSystemThreats.IsMatch(orderBy);
        }
        #endregion 
		
    }
}
