using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DBH.Config;

namespace DBH.Util
{
    public class DBHelperInitializer
    {

        public static Dictionary<string, DataSourceConfig> DataSourceList = new Dictionary<string, DataSourceConfig>();

        public static void InitialDatabaseConfigs(string dbConfigFile)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("~/DBHelper.config");
            IEbfConfig dbConfigs = ConfigFactory.GetConfigOf(dbConfigFile);
            TagElement rootTag = dbConfigs.RootTag;
            CompositeTag dataSourceTag = (CompositeTag)dbConfigs.FindTag("dblinks");
            GetDataSource(dataSourceTag);
            BrokerFactory.InitDataSource(DataSourceList);
            if (rootTag is CompositeTag)
            {
                foreach (CompositeTag dbConfig in ((CompositeTag)rootTag).SubTags)
                {
                    if (!dbConfig.Name.Equals("dslist"))
                    {
                        InitalizeDatabaseConfig(dbConfig);
                    }
                }
            }
        }

        public static void GetDataSource(CompositeTag dataSourceTag)
        {
            foreach (CompositeTag childConfig in dataSourceTag.SubTags)
            {
                DataSourceConfig dsConfig = new DataSourceConfig();
                String id = childConfig.TryGetAttributeValueAt("id");
                dsConfig.dataSourceName = id;
                dsConfig.url = GetTagValue(childConfig, "connectionString");
                dsConfig.dialectClass = GetTagValue(childConfig, "dialectClass");
                dsConfig.providerName = GetTagValue(childConfig, "providerName");
                String tempValue = null;
                tempValue = GetTagValue(childConfig, "minSize");
                if (tempValue != null)
                {
                    dsConfig.minSize = Convert.ToInt32(tempValue);
                }
                tempValue = GetTagValue(childConfig, "maxSize");
                if (tempValue != null)
                {
                    dsConfig.maxSize = Convert.ToInt32(tempValue);
                }

                tempValue = GetTagValue(childConfig, "timeOut");
                if (tempValue != null)
                {
                    dsConfig.timeOut = Convert.ToInt32(tempValue);
                }

                tempValue = GetTagValue(childConfig, "usePool");
                if (tempValue != null)
                {
                    dsConfig.pooled = Convert.ToBoolean(tempValue);
                }

                if (DataSourceList.ContainsKey(id))
                {
                    DataSourceList[id] = dsConfig;
                }
                else
                {
                    DataSourceList.Add(id, dsConfig);
                }
            }

        }

        private static string GetTagValue(CompositeTag Tags, string key)
        {
            String tempValue = null;
            TagElement tempTag = null;
            tempTag = Tags.FindTag(key);
            if (tempTag == null)
            {
                return null;
            }
            else
            {
                return tempTag.TryGetAttributeValueAt("value");
            }
        }

        private static void InitalizeDatabaseConfig(CompositeTag dbConfig)
        {
            foreach (CompositeTag tableConfig in dbConfig.SubTags)
            {
                InitalizeTableConfig(tableConfig, dbConfig.TryGetAttributeValueAt("id"));
            }
        }

        private static void InitalizeTableConfig(CompositeTag tableConfig, string dsName)
        {
            List<string> pKeys = new List<string>();
            foreach (TagElement columnConfig in tableConfig.SubTags)
            {
                string isKey = columnConfig.TryGetAttributeValueAt("isKey");
                isKey = isKey.ToLower();
                if (isKey != null && isKey.Equals("yes"))
                {
                    pKeys.Add(columnConfig.TryGetAttributeValueAt("id"));
                }
            }
            TablesWithSchema.SetPrimaryKey(dsName, tableConfig.TryGetAttributeValueAt("id"), pKeys.ToArray());
        }
    }
}
