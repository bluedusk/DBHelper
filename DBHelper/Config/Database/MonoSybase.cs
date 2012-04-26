using System.Configuration;

namespace DBH.Config
{
    public class MonoSybase : ConfigurationElement, IDatabase
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

        [ConfigurationProperty("Charset")]
        public string Charset
        {
            get { return (string)this["Charset"]; }
            set { this["Charset"] = value; }
        }

        #region IDatabase 成员

        public string GetDBConnStr()
        {
            string conString =
                string.Format("Data Source='{0},{1}';USER ID='{2}';PWD='{3}';Database='{4}';Connect Timeout={5};Connection Lifetime={6};Pooling={7};Min pool size={8} ;Max pool size={9}; Charset='{10}'", HostName,
                              Port, UID, PWD, Database, ConnectTimeout, ConnectionLifetime, Pooling, Minpoolsize, Maxpoolsize, Charset);
            //;charset=cp936
            return conString;
        }

        #endregion
    }
}
