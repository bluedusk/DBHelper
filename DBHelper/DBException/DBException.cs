using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBH.DBException
{
    public abstract class DBException : Exception
    {

        public DBException(string msg)
            : base(msg)
        { }


        public DBException(string message, Exception ex)
            : base(message, ex)
        { }

    }
}
