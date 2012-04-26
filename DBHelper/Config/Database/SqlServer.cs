using System.Configuration;

namespace DBH.Config
{
    public class SqlServer : ConfigurationElement, IDatabase
    {
        [ConfigurationProperty("Server")]
        public string Server
        {
            get { return (string)this["Server"]; }
            set { this["Server"] = value; }
        }

        [ConfigurationProperty("Database")]
        public string Database
        {
            get { return (string)this["Database"]; }
            set { this["Database"] = value; }
        }

        [ConfigurationProperty("UserID")]
        public string UserID
        {
            get { return (string)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        [ConfigurationProperty("Password")]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        #region IDatabase 成员

        public string GetDBConnStr()
        {
            string conString =
                string.Format("Data Source={0};Initial Catalog={1}; User Id={2};Password={3}", Server,
                              Database, UserID, Password);
            return conString;
        }

        #endregion
    }
}
