using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBH.Helper;

namespace DBH
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDBHelper
    {



        IDbConnection IDbConnection
        {
            get;
          
        }
        void Open();
        void Close();
        DataTable ExecuteQuery(string sqlTxt);
        int ExecuteNoQuery(string sqlTxt);
        int ExecuteNoQuery(string sqlTxt, DBHelperParmCollection parameters);
        DataTable ExecuteQuery(string cmdText, DBHelperParmCollection parameters);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();


    }
}
