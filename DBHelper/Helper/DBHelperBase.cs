using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBH.Dialect;


namespace DBH.Helper
{
    /// <summary>
    /// 数据操作抽象类基类
    /// </summary>
    public abstract class DBHelperBase : IDBHelper
    {
        #region Basic attributes and methods
        private string m_dataSourceName;
        private IDbConnection _IDbConnection;
        protected IDbCommand _IDbCommand;
        protected IDbDataAdapter _IDbDataAdapter;
        protected IDbTransaction _IDbTransaction;
        protected IDataReader _IDataReader;
        private IDbDialect _DBDialect;

        private int _InTransaction = 0;
        /// <summary>
        /// 事务计数器
        /// </summary>
        protected int InTransaction
        {
            get { return _InTransaction; }
            set { _InTransaction = value; }
        }
        /// <summary>
        /// Sql命令文本
        /// </summary>
        protected string _SqlText;

        /// <summary>
        /// 错误信息
        /// </summary>
        protected string _ErrorMsg;
        /// <summary>
        /// 最近执行的命令文本
        /// </summary>
        public string SqlText
        {
            get { return _SqlText; }
        }

        /// <summary>
        /// 最近执行命令时的错误
        /// </summary>
        public string ErrorMsg
        {
            get { return _ErrorMsg; }
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private IDbConnection DbConnection
        {
            get
            {
                if (_IDbConnection.State.Equals(ConnectionState.Closed))
                {
                    _IDbConnection.Open();
                }
                else if (_IDbConnection.State.Equals(ConnectionState.Broken))
                {
                    _IDbConnection.Close();
                    _IDbConnection.Open();
                }
                return _IDbConnection;
            }
        }


        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DBHelperBase(IDbConnection myConnection)
        {
            _IDbConnection = myConnection;
        }


        #endregion

        #region Database Operations


        /// <summary>
        /// 关闭数据操作连接
        /// </summary>
        public void Close()
        {
            //在事务中不关闭连接
            if (_IDbTransaction == null && _IDbConnection.State == ConnectionState.Open)
            {
                _IDbConnection.Close();
            }

        }

        /// <summary>
        /// 打开数据操作连接
        /// </summary>
        public void Open()
        {
            if (_IDbConnection.State == ConnectionState.Closed)
                _IDbConnection.Open();
        }


        /// <summary>
        /// 开始事务
        /// </summary>
        [Obsolete("This is a deprecated method.", false)]
        public void BeginTransaction()
        {
            _IDbTransaction = _IDbConnection.BeginTransaction();
            CreateCommand(string.Empty, CommandType.Text);
            _IDbCommand.Transaction = _IDbTransaction;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        [Obsolete("This is a deprecated method.", false)]
        public void CommitTransaction()
        {
            _IDbTransaction.Commit();
            _IDbTransaction.Dispose();
            _IDbTransaction = null;

        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        [Obsolete("This is a deprecated method.", false)]
        public void RollbackTransaction()
        {
            _IDbTransaction.Rollback();
            _IDbTransaction.Dispose();
            _IDbTransaction = null;

        }

        /// <summary>
        /// 执行Sql语句 返回DataReader
        /// </summary>
        /// <returns></returns>
        [Obsolete("This is a deprecated method.")]
        public virtual IDataReader ExecuteReader(string cmdText)
        {

            try
            {
                _IDbCommand = CreateCommand(cmdText, CommandType.Text);
                _IDataReader = _IDbCommand.ExecuteReader();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return _IDataReader;

        }
        /// <summary>
        /// 执行Sql语句，不返回
        /// </summary>
        /// <param name="cmdText">Sql语句</param>
        /// <returns>返回执行正确的行数</returns>
        [Obsolete("This is a deprecated method.")]
        public virtual int ExecuteNoQuery(string cmdText)
        {
            int iRtn;
            try
            {

                _IDbCommand = CreateCommand(cmdText, CommandType.Text);
                iRtn = _IDbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return iRtn;
        }
        /// <summary>
        /// 执行Sql语句，返回结果数据
        /// </summary>
        /// <param name="cmdText">Sql语句</param>
        /// <returns>返回执行结果</returns>
        [Obsolete("This is a deprecated method.")]
        public virtual DataTable ExecuteQuery(string cmdText, string talbeName)
        {

            DataSet ds = new DataSet();
            try
            {
                _IDbDataAdapter = CreateAdapter(cmdText);
                _IDbDataAdapter.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteQuery ERROR:" + ex.Message + ex.TargetSite.ToString(), ex);
            }
            finally
            {
                ds.Dispose();
            }


        }
        /// <summary>
        /// 执行Sql语句,返回结果数据,使用DataSet
        /// </summary>
        /// <param name="cmdText">Sql语句</param>
        /// <returns>返回执行结果</returns>
        [Obsolete("This is a deprecated method.")]
        public virtual DataTable ExecuteQuery(string cmdText)
        {
            DataSet ds = new DataSet();
            try
            {

                _IDbDataAdapter = CreateAdapter(cmdText);
                _IDbDataAdapter.Fill(ds);
                return ds.Tables[0];

            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteQuery ERROR:" + ex.Message + ex.TargetSite.ToString(), ex);
            }
            finally
            {
                ds.Dispose();
            }

        }


        /// <summary>
        /// 执行Sql语句，返回结果数据Dataset
        /// </summary>
        /// <param name="cmdText">Sql语句</param>
        /// <returns>返回执行结果</returns>
        public virtual DataSet GetDataSet(string cmdText)
        {
            DataSet ds = new DataSet();
            try
            {
                _IDbDataAdapter = CreateAdapter(cmdText);

                _IDbDataAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteQuery ERROR:" + ex.Message + ex.TargetSite.ToString(), ex);
            }

            return ds;


        }
        /// <summary>
        /// 执行存储过程，不返回
        /// </summary>
        /// <param name="procedureName">过程名</param>
        /// <returns>返回执行正确的行数</returns>
        [Obsolete("This is a deprecated method.")]
        public virtual int ExecuteProcedureNoQuery(string procedureName, DBHelperParmCollection parameters) { throw new Exception("The method or operation is not implemented."); }
        /// <summary>
        /// 执行存储过程，返回结果数据
        /// </summary>
        [Obsolete("This is a deprecated method.")]
        public virtual DataTable ExecuteProcedureQuery(string procedureName, DBHelperParmCollection parameters) { throw new Exception("The method or operation is not implemented."); }

        /// <summary>
        /// 获得相应的数据命令
        /// </summary>
        protected virtual IDbCommand CreateCommand(string cmdText, CommandType commandType) { throw new Exception("The method or operation is not implemented."); }
        protected virtual IDbDataAdapter CreateAdapter(string cmdText) { throw new Exception("The method or operation is not implemented."); }
        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <returns></returns>
        [Obsolete("This is a deprecated method.")]
        public virtual object ExecuteScalar(string cmdText)
        {
            object retObj;
            _IDbCommand = CreateCommand(cmdText, CommandType.Text);
            retObj = _IDbCommand.ExecuteScalar();


            return retObj;
        }

        public virtual int ExecuteNoQuery(string cmdText, DBHelperParmCollection parameters) { throw new Exception("The method or operation is not implemented."); }
        public virtual DataTable ExecuteQuery(string cmdText, DBHelperParmCollection parameters) { throw new Exception("The method or operation is not implemented."); }
        public virtual int DataTableInsert(DataTable _dt) { throw new Exception("The method or operation is not implemented."); }
        public virtual int DataTableUpdate(DataTable _dt) { throw new Exception("The method or operation is not implemented."); }
        public virtual int DataTableDelete(DataTable _dt) { throw new Exception("The method or operation is not implemented."); }

        #endregion

        #region IDBHelper 成员

        public IDbConnection IDbConnection
        {
            get
            {
                return this.DbConnection;
            }

        }

        #endregion
    }
}
