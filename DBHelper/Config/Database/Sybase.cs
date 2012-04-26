using System.Configuration;

namespace DBH.Config
{
    public class Sybase : ConfigurationElement, IDatabase
    {
        [ConfigurationProperty("HostName")]
        public string HostName
        {
            get { return (string)this["HostName"]; }
            set { this["HostName"] = value; }
        }

        [ConfigurationProperty("Port")]
        public string Port
        {
            get { return (string)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("Database")]
        public string Database
        {
            get { return (string)this["Database"]; }
            set { this["Database"] = value; }
        }

        [ConfigurationProperty("UID")]
        public string UID
        {
            get { return (string)this["UID"]; }
            set { this["UID"] = value; }
        }

        [ConfigurationProperty("PWD")]
        public string PWD
        {
            get { return (string)this["PWD"]; }
            set { this["PWD"] = value; }
        }

        [ConfigurationProperty("ConnectTimeout")]
        public string ConnectTimeout
        {
            get { return (string)this["ConnectTimeout"]; }
            set { this["ConnectTimeout"] = value; }
        }

        [ConfigurationProperty("ConnectionLifetime")]
        public string ConnectionLifetime
        {
            get { return (string)this["ConnectionLifetime"]; }
            set { this["ConnectionLifetime"] = value; }
        }

        [ConfigurationProperty("Minpoolsize")]
        public string Minpoolsize
        {
            get { return (string)this["Minpoolsize"]; }
            set { this["Minpoolsize"] = value; }
        }

        [ConfigurationProperty("Maxpoolsize")]
        public string Maxpoolsize
        {
            get { return (string)this["Maxpoolsize"]; }
            set { this["Maxpoolsize"] = value; }
        }

        [ConfigurationProperty("Pooling")]
        public string Pooling
        {
            get { return (string)this["Pooling"]; }
            set { this["Pooling"] = value; }
        }

        [ConfigurationProperty("Language")]
        public string Language
        {
            get { return (string)this["Language"]; }
            set { this["Language"] = value; }
        }

        [ConfigurationProperty("DBCoding")]
        public string DBCoding
        {
            get
            {
                switch ((string)this["DBCoding"].ToString().ToLower())
                {
                    case "cp936":
                        return "GB2312";
                    //break;
                    case "utf-8":
                        return "utf-8";
                    case "iso-1":
                        return "ISO8859-1";
                    default:
                        return (string)this["DBCoding"];
                }

            }
            set { this["DBCoding"] = value; }
        }

        #region IDatabase 成员
        /// <summary>
        /// 获得数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetDBConnStr()
        {
            string conString =
                string.Format("Data Source='{0}';Port='{1}';UID='{2}';PWD='{3}';Database='{4}'; Language='{10}' Connect Timeout={5};Connection Lifetime={6};Pooling={7};Min pool size={8} ;Max pool size={9}; ", HostName,
                              Port, UID, PWD, Database, ConnectTimeout, ConnectionLifetime, Pooling, Minpoolsize, Maxpoolsize,Language);
            //;charset=cp936
            return conString;
        }

        #endregion
    }
}
