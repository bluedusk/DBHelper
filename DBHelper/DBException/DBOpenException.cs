using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBH.DBException
{
    public class DBOpenException : DBException
    {

        public DBOpenException()
            : base("数据库连接失败.")
        {

        }
        public DBOpenException(Exception ex)
            : base("数据库连接失败:"+ex.Message)
        {

        }
        public DBOpenException(string msg)
            : base("数据库连接失败:"+msg)
        {

        }

        public DBOpenException(string message, Exception ex)
            : base("数据库连接失败:"+message, ex)
        { }

    }
}
