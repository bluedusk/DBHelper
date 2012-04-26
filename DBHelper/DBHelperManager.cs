using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Helper;
using DBH.Config;
using System.Xml;
using DBH.DBException;
using System.Reflection;

namespace DBH
{
    public sealed class DBHelperManager
    {

        /// <summary>
        /// 不可实例化
        /// </summary>
        private DBHelperManager()
        {

        }

        //数据源名称和数据源对象的Dictionary
        private static Dictionary<string, DataSourceConfig> _DataSourceList;
        public static Dictionary<string, DataSourceConfig> DataSourceList
        {
            get { return DBHelperManager._DataSourceList; }
            set { DBHelperManager._DataSourceList = value; }
        }

        //缓存已经实例化过的Helper,这种缓存方式是否存在多线程问题
        private static Dictionary<string, IDBHelper> _DBHelperCache = new Dictionary<string, IDBHelper>();

        public static Dictionary<string, IDBHelper> DBHelperCache
        {
            get { return DBHelperManager._DBHelperCache; }
            set { DBHelperManager._DBHelperCache = value; }
        }

        #region Init DataSource

        public static void InitDataSource(Dictionary<string, DataSourceConfig> ds)
        {
            DataSourceList = ds;
        }

        public static Dictionary<string, DataSourceConfig> InitDS()
        {

            return InitDS("DBHelper.config");

        }
        public static Dictionary<string, DataSourceConfig> InitDS(string fileName)
        {

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + fileName);

            }
            catch
            {
                throw new DBFileNotFoundException();
            }


            return InitDS(xmlDoc.DocumentElement.SelectSingleNode("DataSourceList"));

        }
        public static Dictionary<string, DataSourceConfig> InitDS(XmlNode xn)
        {
            XmlNodeList xnl = xn.SelectNodes("DataSource");
            return InitDS(xnl);
        }
        public static Dictionary<string, DataSourceConfig> InitDS(XmlElement element)
        {
            XmlNodeList xnl = element.SelectNodes("DataSource");
            return InitDS(xnl);
        }

        /// <summary>
        /// Init DataSource List;
        /// Init DBHelper Cache
        /// </summary>
        /// <param name="xnl"></param>
        /// <returns></returns>
        private static Dictionary<string, DataSourceConfig> InitDS(XmlNodeList xnl)
        {
            System.Collections.Generic.Dictionary<string, DataSourceConfig> dsTable = new Dictionary<string, DataSourceConfig>();
            try
            {
                foreach (XmlNode xl in xnl)
                {
                    DataSourceConfig dsConfig = new DataSourceConfig();
                    string dsName = xl.Attributes["name"].Value;
                    string dialect = xl.Attributes["dialect"].Value;
                    dsConfig.dataSourceName = dsName;
                    dsConfig.dialectClass = dialect;
                    XmlNodeList parmNodeList = xl.SelectNodes("parm");
                    int i = 0;
                    object[] parmList = new object[parmNodeList.Count];
                    foreach (XmlNode parmNode in parmNodeList)
                    {
                        //参数个数要与构造函数对应 否则要进行转换
                        string id = parmNode.Attributes["id"].Value;
                        string value = parmNode.Attributes["value"].Value;
                        parmList[i] = value;
                        i++;
                    }
                    dsConfig.Parameters = parmList;
                    dsTable.Add(dsName, dsConfig);

                    //添加到DBHelperCache
                    string[] classNameArray = dsConfig.dialectClass.Split(new char[] { ':', '-' });
                    string className = classNameArray[1];
                    string assemblyName = classNameArray[0];
                    Type type = null;
                    if (string.IsNullOrEmpty(dsConfig.dialectClass))
                    {
                        throw new ArgumentNullException("配置文件错误：请检查Dialect配置");
                    }

                    try
                    {
                        type = Assembly.Load(assemblyName).GetType(className, true);
                        IDBHelper instance = Activator.CreateInstance(type, dsConfig.Parameters) as IDBHelper;
                        DBHelperCache.Add(dsName, instance);
                    }
                    catch (Exception)
                    {
                        //如果Assembly.Load失败，不做处理
                    }





                }


            }
            catch (Exception ex)
            {

                throw new DBConfigException(ex);
            }

            return dsTable;
        }
        #endregion


        static void AddDBHelper(string dsName, DataSourceConfig dsConfig)
        {

            if (string.IsNullOrEmpty(dsConfig.dialectClass))
            {
                throw new ArgumentNullException("配置文件错误：请检查[dsConfig]配置");
            }
            try
            {
                string[] classNameArray = dsConfig.dialectClass.Split(new char[] { ':', '-' });
                string className = classNameArray[1];
                string assemblyName = classNameArray[0];
                Type type = null;
                type = Assembly.Load(assemblyName).GetType(className, true);
                IDBHelper instance = Activator.CreateInstance(type, dsConfig.Parameters) as IDBHelper;

                DBHelperCache.Add(dsName, instance);
            }
            catch (ArgumentNullException)
            {
                throw new Exception("DBHelper添加失败：数据源名[dsName]不能为空");
            }
            catch (ArgumentException)
            {
                throw new Exception("DBHelper添加失败：已存在同名的数据源");
            }


        }

        /// <summary>
        /// Create DBHelper
        /// </summary>
        /// <param name="dsName"></param>
        /// <param name="dsConfig"></param>
        static void CreateDBHelper(string dsName, DataSourceConfig dsConfig)
        {

            if (string.IsNullOrEmpty(dsConfig.dialectClass))
            {
                throw new ArgumentNullException("配置文件错误：请检查[dsConfig]配置");
            }
            try
            {
                string[] classNameArray = dsConfig.dialectClass.Split(new char[] { ':', '-' });
                string className = classNameArray[1];
                string assemblyName = classNameArray[0];
                Type type = null;
                type = Assembly.Load(assemblyName).GetType(className, true);
                IDBHelper instance = Activator.CreateInstance(type, dsConfig.Parameters) as IDBHelper;

                DBHelperCache.Add(dsName, instance);
            }
            catch (ArgumentNullException)
            {
                throw new Exception("DBHelper添加失败：数据源名[dsName]不能为空");
            }
            catch (ArgumentException)
            {
                throw new Exception("DBHelper添加失败：已存在同名的数据源");
            }


        }

        public static IDBHelper GetHelper()
        {
            return GetHelper("default");
        }
        /// <summary>
        /// 根据数据源名称获得相应的DBHelper
        /// 若缓存中有则直接返回        /// 
        /// </summary>
        /// <param name="dsName">data source name(lowcase)</param>
        /// <returns></returns>
        public static IDBHelper GetHelper(string dsName)
        {
            //If Null, init data source list first;
            if (DataSourceList == null)
            {
                //默认使用DBHelper.config，这里可以按需求修改为默认使用App.config
                XmlConfigurator.Configure("DBHelper.config");
            }
            if (string.IsNullOrEmpty(dsName))
            {
                dsName = "default";
            }
            dsName = dsName.ToLower();//配置文件中dsName要小写
            if (DataSourceList.ContainsKey(dsName))
            {
                //实例化
                DataSourceConfig dsConfig = DataSourceList[dsName];
                string[] classNameArray = dsConfig.dialectClass.Split(new char[] { ':', '-' });
                string className = classNameArray[1];
                string assemblyName = classNameArray[0];
                Type type = null;
                if (string.IsNullOrEmpty(dsConfig.dialectClass))
                {
                    throw new ArgumentNullException("配置文件错误：请检查Dialect配置");
                }
                type = Assembly.Load(assemblyName).GetType(className, true);
                IDBHelper instance = Activator.CreateInstance(type, dsConfig.Parameters) as IDBHelper;

                return instance;
            }
            else
            {
                throw new Exception("Not found data source [" + dsName + "], please check config file.");
            }

        }



    }
}
