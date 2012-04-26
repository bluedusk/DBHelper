using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBH.DBException;

namespace DBH.Helper
{
    public class DBHelper
    {
        //private DBHelperBase _DBHelper;//使用config文件
        private static string _DBType = string.Empty;
        private static string _ConnStr = string.Empty;
        private static string _DBEncoding = string.Empty;
        //Database Prefix
        public static string DBPrefix = "ABST_";


        /// <summary>
        /// 设置数据库前缀
        /// </summary>
        /// <param name="_DBPrefix"></param>
        public static void SetDBPrefix(string _DBPrefix)
        {
            DBPrefix = _DBPrefix;
        }
        /// <summary>
        /// 初始化数据库连接(使用字符串)
        /// </summary>
        /// <param name="DBType">数据库类型</param>
        /// <param name="ConnStr">数据库连接字符串</param>
        /// <param name="DBEncoding">数据库编码(仅对Sybase)</param>
        public static void SetDBConfig(string DBType, string ConnStr, string DBEncoding)
        {
            _DBType = DBType;
            _ConnStr = ConnStr;
            _DBEncoding = DBEncoding;
        }

        /// <summary>
        /// 设置dbhelper数据源
        /// </summary>
        /// <param name="dataSourceName"></param>
        public static void SetDBConfig(string dataSourceName)
        {
        
        }

        private static DBHelperBase CreateHelper()
        {
            DBHelperBase _DBHelper = null;
            try
            {
                if (_ConnStr.Length > 0)
                { _DBHelper = DBHelperFactory.CreateHelper(_DBType, _ConnStr, _DBEncoding); }
                else
                    _DBHelper = DBHelperFactory.CreateDBHelper();
                _DBHelper.Open();
                return _DBHelper;
            }
            catch (Exception ex)
            {
                throw new DBOpenException(ex);
            }

        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string sql)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }
        }

        public static DataTable ExecuteQuery(string sql,string tableName)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteQuery(sql,tableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(_DBHelper!=null)
                _DBHelper.Close();
            }
        }
        /// <summary>
        /// 查询操作 返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.GetDataSet(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }
        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <param name="sql">sql字符串</param>
        /// <param name="parms">参数列表</param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string sql, DBHelperParmCollection parms)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteQuery(sql, parms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }
        }



        /// <summary>
        /// 查询数据库，返回DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string sql)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteReader(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }

        }

        /// <summary>
        /// 非查询操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNoQuery(string sql)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteNoQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }
        }

        /// <summary>
        /// 非查询操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNoQuery(StringBuilder sbsql)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteNoQuery(sbsql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }
        }

        /// <summary>
        /// 非查询操作
        /// </summary>
        /// <param name="sql">sql字符串</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNoQuery(string sql, DBHelperParmCollection parameters)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteNoQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
            }
        }


        /// <summary>
        /// 非查询操作
        /// </summary>
        /// <param name="sql">sql字符串</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql)
        {
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                return _DBHelper.ExecuteScalar(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_DBHelper != null)
                    _DBHelper.Close();
            }
        }

        /// <summary>
        /// 执行Transaction，完成后关闭连接
        /// </summary>
        /// <param name="sqls">sql数组</param>
        /// <returns>总影响条数，返回0为事务执行失败</returns>
        public static int ExecuteTransaction(string[] sqls)
        {
            int i = 0;
            DBHelperBase _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                _DBHelper.BeginTransaction();
                foreach (string sql in sqls)
                {
                    _DBHelper.ExecuteNoQuery(sql);
                    i++;
                }

                _DBHelper.CommitTransaction();
                return i;
            }
            catch (Exception ex)
            {
                _DBHelper.RollbackTransaction();
                throw ex;
                
            }
            finally
            {
                if (_DBHelper != null)
                _DBHelper.Close();
                
            }

        }

        public static int ExecuteProcedureNoQuery(string procedureName, DBHelperParmCollection parameters) { throw new Exception("The method or operation is not implemented."); }
        public static DataTable ExecuteProcedureQuery(string procedureName, DBHelperParmCollection parameters) { throw new Exception("The method or operation is not implemented."); }
        public static int DataTableInsert(DataTable _dt) { throw new Exception("The method or operation is not implemented."); }
        public static int DataTableUpdate(DataTable _dt) { throw new Exception("The method or operation is not implemented."); }
        public static int DataTableDelete(DataTable _dt) { throw new Exception("The method or operation is not implemented."); }




    }
}
