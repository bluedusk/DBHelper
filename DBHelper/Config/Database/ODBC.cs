using System.Configuration;

namespace DBH.Config
{
    public class ODBC : ConfigurationElement, IDatabase
    {

        [ConfigurationProperty("DSN")]
        public string DSN
        {
            get
            {
                return (string)this["DSN"];
            }
            set
            {
                this["DSN"] = value;
            }
        }
        [ConfigurationProperty("PWD")]
        public string PWD
        {
            get
            {
                return (string)this["PWD"];
            }
            set
            {
                this["PWD"] = value;
            }
        }
        [ConfigurationProperty("UID")]
        public string UID
        {
            get
            {
                return (string)this["UID"];
            }
            set
            {
                this["UID"] = value;
            }
        }

        public string GetDBConnStr()
        {
            string conString = string.Format("DSN={0};UID={1};PWD={2}", DSN, UID, PWD);
            return conString;
        }
    }
}
