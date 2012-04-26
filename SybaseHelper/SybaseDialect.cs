using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Dialect;
using System.Data;
using System.Data.Common;
using DBH.Helper;
using Sybase.Data.AseClient;

namespace SybaseHelper
{
    public class SybaseDialect : IDbDialect
    {
        #region IDbDialect 成员

        public IDbConnection CreateDbConnection(string dsName)
        {
            return new DBConnection(dsName);
        }

        public IDbConnection CreateRealDbConnection()
        {
            return new AseConnection();
        }

        public void DeriveParameters(IDbCommand spCommand)
        {
            //低版本的ASE ADO驱动不适用
            AseCommandBuilder.DeriveParameters((AseCommand)spCommand);
        }

        public IDbDataParameter CreateDbParameter(string parameterName, object value)
        {
            IDbDataParameter dbParameter = null;

            if (value == null)
            {
                dbParameter = new AseParameter(parameterName, DBNull.Value);
            }
            else if (value is byte[])
            {
                dbParameter = new AseParameter(parameterName, AseDbType.Image);
                dbParameter.Value = value;
            }
            else
            {
                dbParameter = new AseParameter(parameterName, value);
            }

            if (dbParameter.Value == System.DBNull.Value)
            {
                dbParameter.DbType = DbType.String;
            }

            return dbParameter;
        }

        public DbDataAdapter CreateDbDataAdapter(IDbCommand dbCommand)
        {
            return new AseDataAdapter((AseCommand)dbCommand);
        }

        public void ProcessInputCommand(IDbCommand command)
        {
            if (command == null)
            {
                return;
            }
            command.CommandText = EncodIngHelper.ConvertCP936ToISO(command.CommandText);
            foreach (IDbDataParameter parameter in command.Parameters)
            {
                if (parameter.Value != System.DBNull.Value && parameter.Value is string && (parameter.Direction == ParameterDirection.Input || parameter.Direction == ParameterDirection.InputOutput))
                {
                    parameter.Value = EncodIngHelper.ConvertCP936ToISO(parameter.Value as string);
                }
            }
        }

        public void ProcessOutputCommand(IDbCommand command)
        {
            if (command == null)
            {
                return;
            }
            foreach (IDbDataParameter parameter in command.Parameters)
            {
                if (parameter.Value != DBNull.Value && parameter.Value is string && (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.ReturnValue))
                {
                    parameter.Value = EncodIngHelper.ConvertISOToCP936(parameter.Value as string);
                }
            }
        }

        public void ProcessDataSet(DataSet dataSet)
        {
            foreach (DataTable dt in dataSet.Tables)
            {
                ProcessDataTable(dt);
            }
        }

        public void ProcessDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != null && row[i] is string && row[i] != System.DBNull.Value && row[i].ToString() != "")
                    {
                        row[i] = EncodIngHelper.ConvertISOToCP936(row[i].ToString());
                    }
                }
            }
        }
    }

    internal static class EncodIngHelper
    {
        public static Encoding GB2312Encoding = System.Text.Encoding.GetEncoding("GB2312");
        public static Encoding ISO8859_1 = System.Text.Encoding.GetEncoding("ISO8859-1");

        public static string ConvertISOToCP936(string ISOStr)
        {
            if (ISOStr == null || ISOStr == "")
            {
                return ISOStr;
            }
            else
            {
                return GB2312Encoding.GetString(ISO8859_1.GetBytes(ISOStr));
            }
        }

        public static string ConvertCP936ToISO(string cp936Str)
        {
            if (cp936Str == null || cp936Str == "")
            {
                return cp936Str;
            }
            else
            {
                return ISO8859_1.GetString(GB2312Encoding.GetBytes(cp936Str));
            }
        }
    

        #endregion
    }
}
