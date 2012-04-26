using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBH.DBException
{
    [Serializable]
    public class DBDataNotFoundException : Exception
    {
        public DBDataNotFoundException(string message)
            : base(message)
        {
        }

        public DBDataNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DBDataNotFoundException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
