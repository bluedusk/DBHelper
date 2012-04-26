using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using System.Data;
using DBH.Config.Database;

namespace DBH.Helper
{
    public class DBConnectionPool
    {
         private static Hashtable m_connectionPoolTable = new Hashtable();
        //TODO: 处理Connection在workingPool中停留超时逻辑

        private DataSourceFactory m_dataSource;
        private ArrayList m_workingPool = new ArrayList();
        private ArrayList m_freePool = new ArrayList();

        private int m_waitingConnectionCount = 0;

        private ArrayList m_suspendedThreadPool = new ArrayList();

        public static DBConnectionPool GetInstance(string dsName)
        {
            if (!m_connectionPoolTable.ContainsKey(dsName))
            {
                lock (m_connectionPoolTable.SyncRoot)
                {
                    if (!m_connectionPoolTable.ContainsKey(dsName))
                    {
                        m_connectionPoolTable[dsName] = new DBConnectionPool(dsName);
                    }
                }
            }

            return (DBConnectionPool)m_connectionPoolTable[dsName];
        }

        private DBConnectionPool(string dsName)
        {
            this.m_dataSource = DataSourceFactory.GetInstance(dsName);
            if (m_dataSource.UseEbfPool)
            {
                for (int i = 0; i < m_dataSource.MinSize; i++)
                {
                    m_freePool.Add(CreateRealDbConnection());
                }
            }
        }

        private IDbConnection CreateRealDbConnection()
        {
            IDbConnection dbConnection = m_dataSource.DbDialect.CreateRealDbConnection();
            dbConnection.ConnectionString = m_dataSource.ConnectionString;
            dbConnection.Open();

            return dbConnection;
        }

        private IDbConnection BorrowDbConnectionFromPool()
        {
            IDbConnection conn = null;

            //freePool未空
            if (m_freePool.Count > 0)
            {
                lock (m_freePool.SyncRoot)
                {
                    if (m_freePool.Count > 0)
                    {
                        conn = (IDbConnection)m_freePool[0];
                        m_freePool.RemoveAt(0);
                    }
                }
                if (conn != null)
                {
                    lock (m_workingPool.SyncRoot)
                    {
                        m_workingPool.Add(conn);
                    }

                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Open();
                    }
                    return conn;
                }
            }

            //workPool未满
            if (m_workingPool.Count < m_dataSource.MaxSize)
            {
                conn = CreateRealDbConnection();
                lock (m_workingPool.SyncRoot)
                {
                    if (m_workingPool.Count < m_dataSource.MaxSize)
                    {
                        m_workingPool.Add(conn);
                        return conn;
                    }
                }
                conn.Dispose();
            }

            lock (m_freePool.SyncRoot)
            {
                m_waitingConnectionCount++;
                System.Threading.Monitor.Wait(m_freePool.SyncRoot, 500);
            }

            return GetDbConnection();
        }

        public IDbConnection GetDbConnection()
        {
            if (m_dataSource.UseEbfPool)
            {
                return BorrowDbConnectionFromPool();
            }
            else
            {
                return CreateRealDbConnection();
            }
        }

        private void ReturnDbConnectionToPool(IDbConnection dbConnection)
        {
            int workingPoolCount = 0;
            lock (m_workingPool.SyncRoot)
            {
                m_workingPool.Remove(dbConnection);
                workingPoolCount = m_workingPool.Count;
            }

            if (workingPoolCount >= m_dataSource.MinSize || m_freePool.Count < m_dataSource.MinSize)
            {
                lock (m_freePool.SyncRoot)
                {
                    if (workingPoolCount >= m_dataSource.MinSize || m_freePool.Count < m_dataSource.MinSize)
                    {
                        m_freePool.Add(dbConnection);
                        if (m_waitingConnectionCount > 0)
                        {
                            m_waitingConnectionCount--;
                            System.Threading.Monitor.Pulse(m_freePool.SyncRoot);
                        }
                        dbConnection = null;
                    }
                }
            }

            if (dbConnection != null)
            {
                dbConnection.Dispose();
            }
        }

        public void ReleaseDbConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
            {
                throw new Exception("释放数据库连接资源时，传入的连接参数不能为null");
            }

            if (m_dataSource.UseEbfPool)
            {
                ReturnDbConnectionToPool(dbConnection);
            }
            else
            {
                dbConnection.Dispose();
            }
        }
    }
}
