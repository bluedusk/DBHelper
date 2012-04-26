using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBH.DBException;
using System.Xml;
using System.Collections;
using DBH.Config;


namespace DBH.Helper
{

    public class DBHelper2
    {
        private static int initType = 0;
        public static object syncRoot = new object();
        private static bool InTransaction = false;
        private static string _DsName;
        public static string ConfigFileName = "DBHelper.config";

        //private static Dictionary<string, IDBHelper> _All = new Dictionary<string, IDBHelper>();

        ///// <summary>
        ///// 所有数据源
        ///// </summary>
        //public static Dictionary<string, IDBHelper> All
        //{
        //    get
        //    {
        //        if (DBHelperManager.DBHelperCache == null || DBHelperManager.DBHelperCache.Count == 0)
        //        {
        //            XmlConfigurator.Configure();
        //        }
        //        return DBHelperManager.DBHelperCache;
        //    }
        //    set
        //    {
        //        DBHelper2._All = value;
        //    }
        //}

        public static IDBHelper Get(string dsName)
        {

            return DBHelperManager.GetHelper(dsName);

        }


        public static string DsName
        {
            get { return DBHelper2._DsName; }
            set { DBHelper2._DsName = value; }
        }

        public static void Init(string configFileName)
        {

            ConfigFileName = configFileName;

        }



        private static IDBHelper CreateHelper()
        {
            IDBHelper _DBHelper = null;
            try
            {

                //使用指定的数据源,若DsName为空,则使用default
                _DBHelper = DBHelperManager.GetHelper(DsName);
                //取消指定
                DsName = string.Empty;


                _DBHelper.Open();
                return _DBHelper;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw new DBOpenException(ex.InnerException.Message, ex);
                else
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
            IDBHelper _DBHelper = null;
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
                if (_DBHelper != null && !InTransaction)
                    _DBHelper.Close();
            }
        }

        public void BegionTransaction()
        {
            IDBHelper _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                _DBHelper.BeginTransaction();

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public void CommitTransaction()
        {
            IDBHelper _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                _DBHelper.CommitTransaction();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void RollbackTransaction()
        {
            IDBHelper _DBHelper = null;
            try
            {

                _DBHelper = CreateHelper();
                _DBHelper.RollbackTransaction();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }

}
