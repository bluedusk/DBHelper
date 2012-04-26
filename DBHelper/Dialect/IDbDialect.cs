using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace DBH.Dialect
{
    //预留
    public interface IDbDialect
    {
        IDbConnection CreateDbConnection(string dsName);
        IDbConnection CreateRealDbConnection();
        DbDataAdapter CreateDbDataAdapter(IDbCommand dbCommand);
        IDbDataParameter CreateDbParameter(string parameterName, object value);
        void DeriveParameters(IDbCommand spCommand);

        void ProcessDataSet(System.Data.DataSet dataSet);
        void ProcessDataTable(System.Data.DataTable dt);
        void ProcessInputCommand(System.Data.IDbCommand command);
        void ProcessOutputCommand(System.Data.IDbCommand command);
    }
}
