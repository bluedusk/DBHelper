using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DBH.Helper
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    public class DBConnection : IDbConnection
    {
        private IDbConnection m_dbConnection;
        private IDbTransaction m_transaction;

        private string m_dataSourceName;
        private ConnectionState m_state = ConnectionState.Closed;

        public DBConnection(string dsName)
        {
            this.m_dataSourceName = dsName;
        }

        public void ChangeDatabase(string databaseName)
        {
            m_dbConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            if (m_transaction != null)
            {
                m_transaction.Dispose();
            }

            ConnectionPool.ReleaseDbConnection(m_dbConnection);
            m_dbConnection = null;
            m_state = ConnectionState.Closed;
        }

        public string ConnectionString
        {
            get
            {
                if (m_dbConnection == null)
                {
                    return null;
                }
                return m_dbConnection.ConnectionString;
            }
            set
            {
                throw new Exception("readonly!");
            }
        }

        public string Database
        {
            get
            {
                if (m_dbConnection == null)
                {
                    return null;
                }
                return m_dbConnection.Database;
            }
        }

        public void Open()
        {
            m_dbConnection = ConnectionPool.GetDbConnection();
            m_state = ConnectionState.Open;
        }

        public System.Data.ConnectionState State
        {
            get { return m_state; }
        }

        private DBConnectionPool ConnectionPool
        {
            get
            {
                return DBConnectionPool.GetInstance(m_dataSourceName);
            }
        }

        //public IDbTransaction Transaction
        //{
        //    get
        //    {
        //        return m_transaction;
        //    }
        //}


        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            m_transaction = m_dbConnection.BeginTransaction(il);
            return m_transaction;
        }

        public IDbTransaction BeginTransaction()
        {
            m_transaction = m_dbConnection.BeginTransaction();
            return m_transaction;
        }

        public int ConnectionTimeout
        {
            get
            {
                if (m_dbConnection == null)
                {
                    return 0;
                }

                return m_dbConnection.ConnectionTimeout;
            }
        }

        public IDbCommand CreateCommand()
        {
            return m_dbConnection.CreateCommand();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}

