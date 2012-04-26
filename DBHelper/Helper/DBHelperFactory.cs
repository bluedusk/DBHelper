using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Config;

using Sybase.Data.AseClient;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Configuration;
using DBH.DBException;
using System.Threading;
using System.Xml;

namespace DBH.Helper
{
    /// <summary>
    /// 数据库操作类 工厂方法
    /// </summary>
    public static class DBHelperFactory
    {
        private static DBConfig config = DBConfig.GetDBConfig();
        public readonly static string DBType = ConfigurationManager.AppSettings.Get("DBType");
        static string _DBEncodeing = DBConfig.GetDBConfig().Sybase.DBCoding;
        static DBHelperBase dbHelper = null;

        public static DBHelperBase CreateDBHelper()
        {
            DBHelperBase dbh;
            if (config != null)
            {
                string conString = config.GetDBConnStr();
                switch (DBType.ToLower())
                {
                    case "odbc":
                        dbh = new OdbcHelper(new OdbcConnection(conString));
                        break;
                    case "sqlserver":
                        dbh = new SqlHelper(new SqlConnection(conString));
                        break;
                    case "sybase":
                        dbh = new SybaseHelper(new AseConnection(conString));
                        break;                 
                    case "access":
                        dbh = new OleDbHelper(new OleDbConnection(conString));
                        break;
                    case "oledb":
                        dbh = new OleDbHelper(new OleDbConnection(conString));
                        break;
                    default:
                        throw new DBConfigException();

                }
                dbh.Open();
                return dbh;

            }
            else//config is null
                return null;
        }

        public static DBHelperBase CreateHelper(string DBType, string connStr, string DBEncoding)
        {

            switch (DBType.ToLower())
            {
                case "odbc":
                    return new OdbcHelper(new OdbcConnection(connStr));
                case "sqlserver":
                    return new SqlHelper(new SqlConnection(connStr));
                case "sybase":
                    return new SybaseHelper(new AseConnection(connStr), DBEncoding);
                //case "mysql":
                //    dbhelper = new MySqlHelper(new MySqlConnection(connStr));
                //    break;
                case "access":
                    return new OleDbHelper(new OleDbConnection(connStr));
                case "oledb":
                    return new OleDbHelper(new OleDbConnection(connStr));
                default:
                    throw new DBConfigException();

            }


        }

       


    }
}
