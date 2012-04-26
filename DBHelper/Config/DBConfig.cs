using System;
using System.Configuration;
using DBH.Config;

namespace DBH.Config
{
    /// <summary>
    /// 数据库配置类：
    /// 从配置文件web.config或app.config获得config对象
    /// config对象中包含了DB类型和连接串的定义
    /// </summary>
    public class DBConfig : ConfigurationSection
    {

        static string _DBConnStr = "";
        public static string DBType = ConfigurationManager.AppSettings.Get("DBType"); 

        [ConfigurationProperty("ODBC")]
        public   ODBC ODBC
        {
            get 
            { return (ODBC) this["ODBC"]; }
            set { this["ODBC"] = value; }
        }

        [ConfigurationProperty("SqlServer")]
        public SqlServer SqlServer
        {
            get { return (SqlServer) this["SqlServer"]; }
            set { this["SqlServer"] = value; }
        }

        [ConfigurationProperty("Sybase")]
        public Sybase Sybase
        {
            get { return (Sybase) this["Sybase"]; }
            set { this["Sybase"] = value; }
        }

        [ConfigurationProperty("MySql")]
        public MySql MySql
        {
            get { return (MySql) this["MySql"]; }
            set { this["MySql"] = value; }
        }

        [ConfigurationProperty("Access")]
        public    Access Access
        {
            get { return (Access) this["Access"]; }
            set { this["Access"] = value; }
        }

        [ConfigurationProperty("OleDb")]
        public OleDb OleDb
        {
            get { return (OleDb)this["OleDb"]; }
            set { this["OleDb"] = value; }
        }

        [ConfigurationProperty("MonoSybase")]
        public MonoSybase MonoSybase
        {
            get { return (MonoSybase)this["MonoSybase"]; }
            set { this["MonoSybase"] = value; }
        }

        private static DBConfig config = null;
        /// <summary>
        /// 获得数据库配置对象
        /// </summary>
        /// <returns></returns>
        public static DBConfig GetDBConfig()
        {
            if (config == null)
            {
                config = (DBConfig)ConfigurationManager.GetSection("DBConfig");
                
            }
            return config;
        }

        public static DBConfig GetDBConfig(string path,string dbType)
        {
            if (config == null)
            {
                ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
                ecfm.ExeConfigFilename = path;
                Configuration cf= ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
                config = (DBConfig)cf.GetSection("DBConfig");
                DBType = cf.AppSettings.Settings[dbType].Value;
                
            }
            return config;
        }

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        /// <returns></returns>
        public  string GetDBConnStr()
        {

            if (_DBConnStr == "")
            {
                switch (DBType)
                {
                    case "Sybase":
                        _DBConnStr = Sybase.GetDBConnStr();
                        break;
                    case "ODBC":
                        _DBConnStr = ODBC.GetDBConnStr();
                        break;
                    case "SqlServer":
                        _DBConnStr = SqlServer.GetDBConnStr();
                        break;
                    case "MySql":
                        _DBConnStr = MySql.GetDBConnStr();
                        break;
                    case "Access":
                        _DBConnStr = Access.GetDBConnStr();
                        break;
                    case "OleDb":
                        _DBConnStr = OleDb.GetDBConnStr();
                        break;
                    case "MonoSybase":
                        _DBConnStr = MonoSybase.GetDBConnStr();
                        break;
                    default:
                        throw new Exception("DBType参数配置不正确:请检查配置文件。");
                }
            }
            return _DBConnStr;
            
        }

        public static string GetDBEncoding(string _DBEncoding)
        {
           switch (_DBEncoding.ToLower())
                {
                    case "cp936":
                        return "GB2312";
                    //break;
                    case "utf-8":
                        return "utf-8";
                    case "iso-1":
                        return "ISO8859-1";
                    default:
                        return _DBEncoding;
                }
        }

    }
}
