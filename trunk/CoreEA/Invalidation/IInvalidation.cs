using System;
using CoreEA.LoginInfo;
namespace CoreEA.Invalidation
{
    interface IInvalidation
    {
        bool InvalidLogin(BaseLoginInfo info);
        bool IsInvalidArguments(string arg);
        bool IsInvalidSql(string sql);
        bool IsMashTheInvalidChar { get; set; }


        //Sql Injection detection
        bool DetectSqlInjection(string whereClause);
        bool DetectSqlInjection(string whereClause, string orderBy);
    }
}
