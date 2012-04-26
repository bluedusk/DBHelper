using System.Configuration;

namespace DBH.Config
{
    public class MySql : ConfigurationElement, IDatabase
    {
        [ConfigurationProperty("Server")]
        public string Server
        {
            get { return (string)this["Server"]; }
            set { this["Server"] = value; }
        }

        [ConfigurationProperty("Password")]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("UserID")]
        public string UserID
        {
            get { return (string)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        [ConfigurationProperty("Database")]
        public string Database
        {
            get { return (string)this["Database"]; }
            set { this["Database"] = value; }
        }

        [ConfigurationProperty("Pool")]
        public string Pool
        {
            get { return (string)this["Pool"]; }
            set { this["Pool"] = value; }
        }

        #region IDatabase 成员

        public string GetDBConnStr()
        {
            string conString = string.Format("Server={0};User Id={1};Password={2};Database={3}", Server,
                                             UserID, Password, Database, Pool);
            return conString;
        }

        #endregion
    }
}
