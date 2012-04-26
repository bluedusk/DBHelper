using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBH.Dialect;

using System.Collections;
using System.Data.Common;
using DBH.Config.Database;

namespace DBH.Helper.Impl
{
    internal class DBHelperImpl : IDBHelper
    {

        private string m_dataSourceName;
        private IDbConnection m_dbConnection;
        private IDbTransaction m_transaction;
        private IDbDialect m_dbDialect;
        private IDbCommand _IDbCommand;
        private IDbDataAdapter _IDbDataAdapter;
        private IDataReader _IDataReader;


        private IDbConnection DbConnection
        {
            get
            {
                if (m_dbConnection.State.Equals(ConnectionState.Closed))
                {
                    m_dbConnection.Open();
                }
                else if (m_dbConnection.State.Equals(ConnectionState.Broken))
                {
                    m_dbConnection.Close();
                    m_dbConnection.Open();
                }
                return m_dbConnection;
            }
        }

        #region Construction
        public DBHelperImpl(string dsName)
        {
            this.m_dataSourceName = dsName;
            //EbfDataSource dataSource = EbfDataSource.GetInstanceOf(dsName);
            //this.m_dbDialect = dataSource.DbDialect;
            this.m_dbDialect = DataSourceFactory.GetInstance(dsName).DbDialect;
            this.m_dbConnection = this.CreateDbConnection(dsName);
        }
        #endregion

        #region IDBHelper Implement
        public void BeginTransaction()
        {
            m_transaction = this.DbConnection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            m_transaction.Commit();
        }
        public void AbortTransaction()
        {
            if (m_transaction != null)
            {
                m_transaction.Rollback();
            }
        }
        private IDbCommand BuildSqlCommand(string sql, params object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[0];
            }
            int i = 0;
            int index = -1;
            Hashtable ht = new Hashtable();
            for (index = sql.IndexOf('?'); index != -1 && i < parameters.Length; index = sql.IndexOf('?'), i++)
            {
                string parameterName = "@p" + i;
                object parameterValue = parameters[i];
                if (parameterValue == null)
                {
                    parameterValue = "";
                }
                ht.Add(parameterName, parameterValue);
                sql = sql.Remove(index, 1).Insert(index, parameterName);
            }
            if (index != -1 || i != parameters.Length)
            {
                throw new Exception("语句中参数数量与所给参数数量不对应!");
            }
            return this.BuildSqlCommand(sql, ht);
        }

        private IDbCommand BuildSqlCommand(string sql, Hashtable parameters)
        {
            IDbCommand command = this.CreateCommand();
            foreach (string parameterName in parameters.Keys)
            {
                if (parameters[parameterName] == null)
                {
                    parameters[parameterName] = DBNull.Value;
                }

                IDbDataParameter parameter = CreateDbParameter(parameterName, parameters[parameterName]);

                command.Parameters.Add(parameter);
            }
            command.CommandText = sql;

            //处理编码等问题
            this.m_dbDialect.ProcessInputCommand(command);
            return command;
        }




        public DataTable ExecuteQuery(string sql, params object[] parameters)
        {
            IDbCommand sqlCommand = null;
            try
            {
                DataTable dt = new DataTable();
                sqlCommand = this.BuildSqlCommand(sql, parameters);
                IDbDataAdapter adapter = CreateDbDataAdapter(sqlCommand);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dt = ds.Tables[0];

                //处理编码等问题
                this.m_dbDialect.ProcessDataTable(dt);
                return dt;
            }
            finally
            {
                if (sqlCommand != null)
                {
                    try { sqlCommand.Dispose(); }
                    catch { }
                }
            }
        }

        public int ExecuteUpdate(string sql, params object[] parameters)
        {
            IDbCommand sqlCommand = null;
            try
            {
                sqlCommand = this.BuildSqlCommand(sql, parameters);
                return sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                if (sqlCommand != null)
                {
                    try { sqlCommand.Dispose(); }
                    catch { }
                }
            }
        }

        public int ExecuteUpdate(string sql, Hashtable parameters)
        {
            IDbCommand sqlCommand = null;
            try
            {
                sqlCommand = this.BuildSqlCommand(sql, parameters);
                return sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                if (sqlCommand != null)
                {
                    try { sqlCommand.Dispose(); }
                    catch { }
                }
            }
        }

        public DataTable ExecuteQuery(string sql, int pageId, int pageSize, params object[] parameters)
        {
            IDbCommand sqlCommand = null;
            IDataReader dbDataReader = null;
            try
            {
                DataTable dt = new DataTable();

                sqlCommand = BuildSqlCommand(sql, parameters);
                dbDataReader = sqlCommand.ExecuteReader();

                for (int i = 0; i < dbDataReader.FieldCount; i++)
                {
                    dt.Columns.Add(dbDataReader.GetName(i), dbDataReader.GetFieldType(i));
                }

                int iRow = 0;
                int maxRows = (pageId + 1) * pageSize;
                while (dbDataReader.Read())
                {
                    if (maxRows > 0)
                    {
                        if (iRow < maxRows - pageSize)
                        {
                            iRow++;
                            continue;
                        }
                        else if (iRow >= maxRows)
                        {
                            iRow++;
                            break;
                        }
                        else
                        {
                            iRow++;
                        }
                    }
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < dbDataReader.FieldCount; i++)
                    {
                        object obj = dbDataReader.GetValue(i);
                        string filedName = dbDataReader.GetName(i);
                        row[filedName] = obj;
                    }
                    dt.Rows.Add(row);
                }

                //处理编码等问题
                this.m_dbDialect.ProcessDataTable(dt);

                return dt;
            }
            finally
            {
                if (dbDataReader != null)
                {
                    try { dbDataReader.Close(); }
                    catch { }
                }
                if (sqlCommand != null)
                {
                    try { sqlCommand.Dispose(); }
                    catch { }
                }
            }
        }

        public DataTable ExecuteQuery(out IDbDataAdapter da, string sql, params object[] parameters)
        {
            IDbCommand sqlCommand = null;
            try
            {
                sqlCommand = this.BuildSqlCommand(sql, parameters);
                da = this.CreateDbDataAdapter(sqlCommand);

                //DataTable dt = new DataTable();
                //da.Fill(dt);

                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                //处理编码等问题
                this.m_dbDialect.ProcessDataTable(dt);

                return dt;
            }
            finally
            {
                //TODO:关闭Command是否会导致游标失效
                if (sqlCommand != null)
                {
                    try { sqlCommand.Dispose(); }
                    catch { }
                }
            }
        }

        //private object Raw_GetStoredProcedureParameters(params object[] p)
        //{
        //    IDbCommand spCommand = this.CreateCommand();
        //    spCommand.CommandType = CommandType.StoredProcedure;
        //    spCommand.CommandText = p[0] as string;

        //    this.DeriveParameters(spCommand);
        //    //spCommand.Dispose();

        //    return spCommand.Parameters;
        //}

        //private IDbCommand BuildStoredProcedureCommand(StoredProcedure proc, Hashtable table)
        //{
        //    ObjectPool spParamPool = ObjectPool.GetInstance(proc.SPName);
        //    IDataParameterCollection parameters = (IDataParameterCollection)spParamPool.GetAndCacheObject(Raw_GetStoredProcedureParameters, proc.SPName);

        //    IDbCommand spCommand = this.CreateCommand();
        //    spCommand.CommandType = CommandType.StoredProcedure;
        //    spCommand.CommandText = proc.SPName;
        //    spCommand.Parameters.Clear();

        //    foreach (StoredProcedureParameter pd in proc.ParameterList)
        //    {
        //        IDbDataParameter dbParameter = this.CreateDbParameter(pd.ParemName, null);
        //        string pName = pd.ParemName;
        //        ParameterDirection pDirection = pd.PDirection;
        //        dbParameter.ParameterName = pName;
        //        dbParameter.DbType = pd.DbType;
        //        dbParameter.Direction = pDirection;
        //        if (pDirection == ParameterDirection.Input)
        //        {
        //            dbParameter.Value = table[pName];
        //        }
        //        else if (pDirection == ParameterDirection.InputOutput)
        //        {
        //            dbParameter.Value = table[pName];
        //            dbParameter.Size = pd.Size;
        //        }
        //        else if (pDirection == ParameterDirection.Output)
        //        {
        //            dbParameter.Size = pd.Size;
        //        }
        //        spCommand.Parameters.Add(dbParameter);
        //    }

        //    //处理编码等问题
        //    this.m_dbDialect.ProcessInputCommand(spCommand);

        //    return spCommand;
        //}

        //public int ExecuteStoredProcedure(StoredProcedure proc, Hashtable table)
        //{
        //    IDbCommand spCommand = null;

        //    try
        //    {
        //        spCommand = BuildStoredProcedureCommand(proc, table);

        //        int ret = -1;
        //        try
        //        {
        //            ret = spCommand.ExecuteNonQuery();

        //            //处理编码等问题
        //            this.m_dbDialect.ProcessOutputCommand(spCommand);

        //        }
        //        finally
        //        {
        //            foreach (StoredProcedureParameter pd in proc.ParameterList)
        //            {
        //                ParameterDirection pDirection = pd.PDirection;
        //                if ((pDirection == ParameterDirection.Output) || (pDirection == ParameterDirection.InputOutput) || (pDirection == ParameterDirection.ReturnValue))
        //                {
        //                    string pName = pd.ParemName;
        //                    table[pName] = ((IDbDataParameter)spCommand.Parameters[pName]).Value;
        //                }
        //            }
        //        }
        //        return ret;
        //    }
        //    finally
        //    {
        //        if (spCommand != null)
        //        {
        //            try { spCommand.Dispose(); }
        //            catch { }
        //        }
        //    }
        //}

        //public DataSet ExecuteStoredProcedureDataSet(StoredProcedure proc, Hashtable table)
        //{
        //    IDbCommand spCommand = null;
        //    DbDataAdapter adapter = null;

        //    try
        //    {
        //        spCommand = BuildStoredProcedureCommand(proc, table);

        //        adapter = this.CreateDbDataAdapter(spCommand);
        //        DataSet dataSet = new DataSet();

        //        try
        //        {
        //            adapter.Fill(dataSet);

        //            //处理编码等问题
        //            this.m_dbDialect.ProcessDataSet(dataSet);

        //            //处理编码等问题
        //            this.m_dbDialect.ProcessOutputCommand(spCommand);

        //        }
        //        finally
        //        {
        //            foreach (StoredProcedureParameter pd in proc.ParameterList)
        //            {
        //                ParameterDirection pDirection = pd.PDirection;
        //                if ((pDirection == ParameterDirection.Output) || (pDirection == ParameterDirection.InputOutput) || (pDirection == ParameterDirection.ReturnValue))
        //                {
        //                    string pName = pd.ParemName;
        //                    table[pName] = ((IDbDataParameter)spCommand.Parameters[pName]).Value;
        //                }
        //            }
        //        }
        //        return dataSet;
        //    }
        //    finally
        //    {
        //        if (adapter != null)
        //        {
        //            try { adapter.Dispose(); }
        //            catch { }
        //        }
        //        if (spCommand != null)
        //        {
        //            try { spCommand.Dispose(); }
        //            catch { }
        //        }
        //    }
        //}

        public DataRow NewDataRow(string tableName)
        {
            return TablesWithSchema.GetPrimaryKey(m_dataSourceName, tableName).NewRow();
        }

        public int Delete(DataRow obj)
        {
            return this.Delete(obj, obj.Table.PrimaryKey);
        }
        private int Delete(DataRow obj, params DataColumn[] columns)
        {
            DataTable dt = obj.Table;
            string tableName = dt.TableName.Trim();
            if (tableName == "")
            {
                throw new Exception("输入的DataRow所在的表没有表名,要求输入的DataRow必须通过NewDataRow方法获取!");
            }
            StringBuilder sqlBuilder = new StringBuilder(String.Format("delete from {0} where ", tableName));
            ArrayList parameters = new ArrayList();
            bool flag = false;
            foreach (DataColumn dataColumn in columns)
            {
                if (obj[dataColumn] != null && obj[dataColumn] != System.DBNull.Value && obj[dataColumn].ToString().Trim() != "")
                {
                    sqlBuilder.Append(dataColumn.ColumnName + "=? and ");
                    parameters.Add(obj[dataColumn]);
                    flag = true;
                }
            }
            //未避免误删除,现禁止有未赋值主键列存在
            if (columns.Length == 0)
            {
                throw new Exception("不能对无主键表进行删除操作!");
            }
            if (parameters.Count != columns.Length)
            {
                throw new Exception("未对所有主键赋值,无法进行删除操作!");
            }
            if (flag)
            {
                sqlBuilder.Remove(sqlBuilder.Length - 4, 4);
            }
            return this.ExecuteUpdate(sqlBuilder.ToString(), parameters.ToArray());
        }
        public int Update(DataRow obj)
        {
            return this.Update(obj, obj.Table.PrimaryKey);
        }
        private int Update(DataRow obj, params DataColumn[] columns)
        {
            DataTable dt = obj.Table;
            string tableName = dt.TableName.Trim();
            if (tableName == "")
            {
                throw new Exception("输入的DataRow所在的表没有表名,要求输入的DataRow必须通过NewDataRow方法获取!");
            }
            StringBuilder sqlBuilder = new StringBuilder(String.Format("update {0} set  where ", tableName));
            Hashtable parameters = new Hashtable();
            int j = 0;
            bool flag1 = false;
            bool flag2 = false;
            int index = 12 + tableName.Length;
            for (int i = 0; i < obj.Table.Columns.Count; i++)
            {
                if (obj[obj.Table.Columns[i]] != null && obj[obj.Table.Columns[i]] != System.DBNull.Value)//有可能修改为空字符串
                {
                    if (isPk(dt, obj.Table.Columns[i]))
                    {
                        sqlBuilder.Append(obj.Table.Columns[i].ColumnName + "=@" + obj.Table.Columns[i].ColumnName + " and ");
                        j++;
                        flag1 = true;
                    }
                    else
                    {
                        sqlBuilder.Insert(index, obj.Table.Columns[i].ColumnName + "=@" + obj.Table.Columns[i].ColumnName + ",");
                        index += obj.Table.Columns[i].ColumnName.Length * 2 + 3;
                        flag2 = true;
                    }
                    parameters.Add("@" + obj.Table.Columns[i].ColumnName, obj[obj.Table.Columns[i]]);
                }
            }
            //未避免误修改,要修改的行必须为所有主键赋值
            if (j != dt.PrimaryKey.Length)
            {
                throw new Exception("未对所有主键赋值,无法进行更新操作!");
            }
            if (j == 0)
            {
                throw new Exception("不能对无主键表进行更新操作!");
            }
            if (flag1)
            {
                sqlBuilder.Remove(sqlBuilder.Length - 5, 5);
            }
            if (flag2)
            {
                sqlBuilder.Remove(index - 1, 1);
            }
            return this.ExecuteUpdate(sqlBuilder.ToString(), parameters);
        }
        private bool isPk(DataTable dt, DataColumn dc)
        {
            foreach (DataColumn dc1 in dt.PrimaryKey)
            {
                if (dc1.ColumnName == dc.ColumnName)
                {
                    return true;
                }
            }
            return false;
        }
        public int Insert(DataRow obj)
        {
            return this.Insert(obj, obj.Table.PrimaryKey);
        }
        private int Insert(DataRow obj, params DataColumn[] columns)
        {
            DataTable dt = obj.Table;
            string tableName = dt.TableName.Trim();
            if (tableName == "")
            {
                throw new Exception("输入的DataRow所在的表没有表名,要求输入的DataRow必须通过NewDataRow方法获取!");
            }
            StringBuilder sqlBuilder = new StringBuilder(String.Format("insert into {0} () values ()", tableName));
            ArrayList parameters = new ArrayList();
            bool flag = false;
            int index = 14 + tableName.Length;
            for (int i = 0; i < obj.Table.Columns.Count; i++)
            {
                if (obj[obj.Table.Columns[i]] != null && obj[obj.Table.Columns[i]] != System.DBNull.Value)//有可能插入空字符串
                {
                    flag = true;
                    sqlBuilder.Insert(index, obj.Table.Columns[i].ColumnName + ",");
                    index += obj.Table.Columns[i].ColumnName.Length + 1;
                    sqlBuilder.Insert(sqlBuilder.Length - 1, "?,");
                    parameters.Add(obj[obj.Table.Columns[i]]);
                }
            }
            if (flag)
            {
                sqlBuilder.Remove(index - 1, 1);
                sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            }
            return this.ExecuteUpdate(sqlBuilder.ToString(), parameters.ToArray());
        }
        public int Save(DataRow obj)
        {
            int result = this.Update(obj);
            if (result == 0)
            {
                result = this.Insert(obj);
            }
            return result;
        }
        #endregion

        #region Disposable Implement
        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            if (this.m_transaction != null)
            {
                try
                {
                    this.m_transaction.Dispose();
                }
                catch { }
            }

            if (this.m_dbConnection.State != ConnectionState.Closed)
            {
                try
                {
                    this.m_dbConnection.Dispose();
                }
                catch { }
            }
            BrokerFactory.DropCurrentBroker();
        }
        #endregion

        private IDbCommand CreateCommand()
        {
            IDbCommand command = this.DbConnection.CreateCommand();
            if (command.Transaction == null && m_transaction != null)
            {
                command.Transaction = m_transaction;
            }
            return command;
        }

        private IDbConnection CreateDbConnection(string dsName)
        {
            return this.m_dbDialect.CreateDbConnection(dsName);
        }

        private void DeriveParameters(IDbCommand spCommand)
        {
            this.m_dbDialect.DeriveParameters(spCommand);
        }

        private IDbDataParameter CreateDbParameter(string parameterName, object value)
        {
            return this.m_dbDialect.CreateDbParameter(parameterName, value);
        }

        private DbDataAdapter CreateDbDataAdapter(IDbCommand dbCommand)
        {
            return this.m_dbDialect.CreateDbDataAdapter(dbCommand);
        }


    }
}
