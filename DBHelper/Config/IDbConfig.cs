using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBH.Config
{
    /// <summary>
    /// deprecated
    /// </summary>
    public interface IDBConfig
    {
        string GetConnectionString();
        string GetDBType();
    }
}
