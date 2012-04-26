using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DBH.Helper;

namespace DBH.Config
{
    /// <summary>
    /// 几种方式初始化DBHelper数据源
    /// 1. 使用默认配置文件Web.config,App.config
    /// 2. 指定配置文件路径
    /// 3. 指定配置节点XmlElement,XmlNode
    /// </summary>
    public sealed class XmlConfigurator
    {
        /// <summary>
        /// 不可初始化
        /// </summary>
        private XmlConfigurator()
        {

        }

        /// <summary>
        /// 使用默认配置文件初始化,App.config,Web.config
        /// </summary>
        static public void Configure()
        {
            
            XmlElement configElement = null;
            //读取App.config中的DataSourceConfig节点
            configElement = System.Configuration.ConfigurationManager.GetSection("DataSourceConfig") as XmlElement;
            if (configElement == null)
            {
                throw new Exception("No Config Section [DataSourceConfig] Found in Default Config File or Config File Not Exist.");
            }
            DBHelperManager.DataSourceList = DBHelperManager.InitDS(configElement);
            
        }


        /// <summary>
        /// 指定配置文件初始化
        /// </summary>
        /// <param name="filePath"></param>
        static public void Configure(string filePath)
        {
            DBHelperManager.DataSourceList = DBHelperManager.InitDS(filePath);

        }

        /// <summary>
        /// 指定配置节点初始化
        /// </summary>
        /// <param name="filePath"></param>
        static public void Configure(XmlElement element)
        {
            DBHelperManager.DataSourceList = DBHelperManager.InitDS(element);
        }

        /// <summary>
        /// 指定配置节点初始化
        /// </summary>
        /// <param name="filePath"></param>
        static public void Configure(XmlNode node)
        {
            DBHelperManager.DataSourceList = DBHelperManager.InitDS(node);
        }



    }
}
