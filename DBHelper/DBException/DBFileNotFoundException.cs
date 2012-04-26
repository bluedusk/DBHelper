using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBH.DBException
{
    public class DBFileNotFoundException : DBException
    {
        public DBFileNotFoundException()
            : base("没有找到数据库配置文件.")
        {
        }

        public DBFileNotFoundException(string msg)
            : base(msg)
        {

        }

        public DBFileNotFoundException(string message, Exception ex)
            : base(message, ex)
        { }
    }
}
