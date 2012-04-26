using System;
using System.Collections.Generic;
using System.Text;

namespace DBH.Config
{
    internal interface IDatabase
    {
        string GetDBConnStr();
    }
}
