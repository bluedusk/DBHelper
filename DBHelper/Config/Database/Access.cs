using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DBH.Config
{
    public class Access : ConfigurationElement, IDatabase
    {
        [ConfigurationProperty("Provider",
            DefaultValue = "Microsoft.Jet.oledb.4.0",
           IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\",
            MinLength = 1,
            MaxLength = 60)]
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

        #region IDatabase 成员

        public string GetDBConnStr()
        {
            string conString = string.Format("Provider={0};Data Source ={1}", Provider, DataSource);
            return conString;
        }

        #endregion
    }
}
