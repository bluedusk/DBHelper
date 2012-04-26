using System.Configuration;

namespace DBH.Config
{
    public class OleDb : ConfigurationElement, IDatabase
    {
        [ConfigurationProperty("Provider",
           DefaultValue = "Sybase.ASEOLEDBProvider.2",
           IsRequired = true)]
        public string Provider
        {
            get { return (string)this["Provider"]; }
            set { this["Provider"] = value; }
        }

        [ConfigurationProperty("DataSource")]
        public string DataSource
        {
            get { return (string)this["DataSource"]; }
            set { this["DataSource"] = value; }
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

        [ConfigurationProperty("Pool")]
        public string Pool
        {
            get { return (string)this["Pool"]; }
            set { this["Pool"] = value; }
        }

        #region IDatabase 成员

        public string GetDBConnStr()
        {
            string conString = string.Format("Provider={0};User ID={1};Data Source={2};Password={3};Persist Security Info=False", Provider, UserID, DataSource, Password);
            return conString;
        }

        #endregion
    }
}
