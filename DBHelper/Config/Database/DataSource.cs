using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Helper;
using DBH.Dialect;
using System.Collections;
using DBH.Util;

namespace DBH.Config.Database
{
    class DataSourceFactory
    {
        //private XMLConfig databaseConfig;
        private string m_dataSourceName;
        private int m_minSize;
        private int m_maxSize;
        private int m_timeOut;
        private string m_connectionString;
        private string m_providerName;
        private IDbDialect m_dbDialect;
        private bool m_usePool = false;

        private static Hashtable m_dataSourcePool = new Hashtable();

        public static DataSourceFactory GetInstance(string name)
        {
            if (!m_dataSourcePool.ContainsKey(name))
            {
                lock (m_dataSourcePool.SyncRoot)
                {
                    if (!m_dataSourcePool.ContainsKey(name))
                    {
                        m_dataSourcePool.Add(name, new DataSourceFactory(name));
                    }
                }
            }
            return (DataSourceFactory)m_dataSourcePool[name];
        }

        private DataSourceFactory(string name)
        {
            name = name.ToLower();
            DataSourceConfig dsConfig = DBHelperFactory2.DataSourceMap[name];
            this.m_dataSourceName = dsConfig.dataSourceName;
            this.m_connectionString = dsConfig.connStr;
            this.m_providerName = dsConfig.providerName;
            string dbDialectClass = dsConfig.dialectClass;
            m_dbDialect = (IDbDialect)ToolKit.CreateInstance(dbDialectClass);

            //默认配置
            this.m_minSize = 50;
            this.m_maxSize = 100;
            this.m_usePool = true;
            this.m_timeOut = 1000;
            try { this.m_minSize = dsConfig.minSize; }
            catch { }
            try { this.m_maxSize = dsConfig.maxSize; }
            catch { }
            try { this.m_timeOut = dsConfig.timeOut; }
            catch { }
            try { this.m_usePool = dsConfig.pooled; }
            catch { }

        }

          public int MinSize
        {
            get
            {
                return m_minSize;
            }
        }

        public int MaxSize
        {
            get
            {
                return m_maxSize;
            }
        }

        public int Timeout
        {
            get
            {
                return m_timeOut;
            }
        }

        public string Name
        {
            get
            {
                return m_dataSourceName;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.m_connectionString;
            }
        }

        public string ProviderName
        {
            get
            {
                return m_providerName;
            }
        }

        public bool UseEbfPool
        {
            get
            {
                return m_usePool;
            }
        }

        public IDbDialect DbDialect
        {
            get
            {
                return m_dbDialect;
            }
        }

    }
}
