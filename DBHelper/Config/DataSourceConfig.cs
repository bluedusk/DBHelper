using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBH.Config
{
    public class DataSourceConfig:IDataSourceConfig
    {//TODO 删除无用属性
        private bool m_pooled = true;
        private string m_user;
        private string m_password;
        private string m_dataSourceName;
        private string m_dataBaseName;
        private string m_driverClass;
        private string m_connStr;
        private string m_dialectClass;
        private string m_providerName;
        object[] _Parameters;

        public object[] Parameters
        {
            get { return _Parameters; }
            set { _Parameters = value; }
        }


        private Dictionary<string, string> _ParmDict;

        public Dictionary<string, string> ParmDict
        {
            get { return _ParmDict; }
            set { _ParmDict = value; }
        }

        public string providerName
        {
            get { return m_providerName; }
            set { m_providerName = value; }
        }

        private int m_minSize = 5;
        public int minSize
        {
            get { return m_minSize; }
            set { m_minSize = value; }
        }

        private int m_maxSize = 10;
        public int maxSize
        {
            get { return m_maxSize; }
            set { m_maxSize = value; }
        }

        private int m_timeOut = 1000;
        public int timeOut
        {
            get { return m_timeOut; }
            set { m_timeOut = value; }
        }

        public string dialectClass
        {
            get { return m_dialectClass; }
            set { m_dialectClass = value; }
        }


        public string user
        {
            get { return m_user; }
            set { m_user = value; }
        }

        public string password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        public string dataSourceName
        {
            get { return m_dataSourceName; }
            set { m_dataSourceName = value; }
        }

        public string dataBaseName
        {
            get { return m_dataBaseName; }
            set { m_dataBaseName = value; }
        }

        public string driverClass
        {

            get { return m_driverClass; }
            set { m_driverClass = value; }
        }

        public string connStr
        {
            get { return m_connStr; }
            set { m_connStr = value; }
        }
        public bool pooled
        {
            get { return m_pooled; }
            set { m_pooled = value; }
        }




    }
}
