using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBH.DBException
{
    public class DBConfigException : DBException
    {
        public DBConfigException()
            : base("数据库参数配置不正确:请检查配置文件。")
        {
        }

        public DBConfigException(string msg)
            : base(msg)
        {

        }

        public DBConfigException(Exception ex)
            : base("数据库参数配置不正确:请检查配置文件。"+ ex.Message, ex)
        {

        }

        public DBConfigException(string message, Exception ex)
            : base(message, ex)
        { }

    }
}
