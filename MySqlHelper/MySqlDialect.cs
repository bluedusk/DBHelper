using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Dialect;

namespace MySqlHelper
{
    public class MySqlDialect:IDbDialect
    {
        #region IDbDialect 成员

        public System.Data.IDbConnection CreateDbConnection(string dsName)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDbConnection CreateRealDbConnection()
        {
            throw new NotImplementedException();
        }

        public System.Data.Common.DbDataAdapter CreateDbDataAdapter(System.Data.IDbCommand dbCommand)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDbDataParameter CreateDbParameter(string parameterName, object value)
        {
            throw new NotImplementedException();
        }

        public void DeriveParameters(System.Data.IDbCommand spCommand)
        {
            throw new NotImplementedException();
        }

        public void ProcessDataSet(System.Data.DataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public void ProcessDataTable(System.Data.DataTable dt)
        {
            throw new NotImplementedException();
        }

        public void ProcessInputCommand(System.Data.IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public void ProcessOutputCommand(System.Data.IDbCommand command)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
