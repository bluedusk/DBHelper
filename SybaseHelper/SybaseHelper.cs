using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Helper;
using Sybase.Data.AseClient;
using System.Data;
using DBH.Util;
using DBH.Config;

namespace SybaseHelper
{
    /// <summary>
    /// Sybase 的数据操作
    /// </summary>
    public class SybaseHelper : DBHelperBase
    {
        private string _DBEncodeing;
        

        public SybaseHelper(AseConnection myConnection, string DBEncoding)
            : base(myConnection)
        {
            _DBEncodeing = GetDBEncoding(DBEncoding);
        }
        public SybaseHelper(string ConnStr, string DBEncoding)
            : base(new AseConnection(ConnStr))
        {
            _DBEncodeing = GetDBEncoding(DBEncoding);
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
        protected override IDbCommand CreateCommand(string cmdText, CommandType commandType)
        {
            if (_IDbCommand == null)
            {
                _IDbCommand = new AseCommand();
            }

            _IDbCommand.Connection = IDbConnection;
            _IDbCommand.CommandText = EncodingHelper.Default2DB(cmdText, _DBEncodeing).ToString();
            _IDbCommand.CommandType = commandType;

            return _IDbCommand;
        }


        public override DataTable ExecuteQuery(string cmdText, string talbeName)
        {

            DataTable dtbRtn = base.ExecuteQuery(cmdText, talbeName);

            ProcessDataTable(dtbRtn);


            return dtbRtn;
        }


        [Obsolete("This is a deprecated method.")]
        public override DataTable ExecuteQuery(string cmdText)
        {
            DataTable dtbRtn = base.ExecuteQuery(cmdText);

            ProcessDataTable(dtbRtn);


            return dtbRtn;
        }

        [Obsolete("This is a deprecated method.")]
        public override int ExecuteNoQuery(string cmdText)
        {
            int iRtn = base.ExecuteNoQuery(cmdText);

            return iRtn;
        }

        protected override IDbDataAdapter CreateAdapter(string cmdText)
        {
            if (_IDbDataAdapter == null)
            {
                _IDbDataAdapter = new AseDataAdapter();
            }
            _IDbDataAdapter.SelectCommand = CreateCommand(cmdText, CommandType.Text);

            return _IDbDataAdapter;
        }

        public override int ExecuteProcedureNoQuery(string procedureName, DBHelperParmCollection parameters)
        {

            throw new Exception("The method or operation is not implemented.");
        }

        public override DataTable ExecuteProcedureQuery(string procedureName, DBHelperParmCollection parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override int ExecuteNoQuery(string cmdText, DBHelperParmCollection parameters)
        {
            int effectNum;
            AseCommand _AseCommand = (AseCommand)CreateCommand(cmdText, CommandType.Text);
            _AseCommand.Parameters.Clear();
            if (parameters != null)
            {
                foreach (DBHelperParm para in parameters)
                {

                    _AseCommand.Parameters.Add(new AseParameter("?"+para.Key, EncodingHelper.Default2DB(para.Value, _DBEncodeing)));
                }
            }
            try
            {
                effectNum = _AseCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            _AseCommand.Parameters.Clear();
            return effectNum;
        }

        public override DataTable ExecuteQuery(string cmdText, DBHelperParmCollection parameters)
        {
            DataTable dtRet = new DataTable();
            AseCommand _AseCommand = (AseCommand)CreateCommand(cmdText, CommandType.Text);
            _AseCommand.Parameters.Clear();
            if (parameters != null)
            {
                foreach (DBHelperParm para in parameters)
                {


                    _AseCommand.Parameters.Add(new AseParameter("?" + para.Key, EncodingHelper.Default2DB(para.Value, _DBEncodeing)));
                }
            }
            AseDataAdapter _OdbcDataAdapter = new AseDataAdapter(_AseCommand);
            try
            {
                ProcessDataTable(dtRet);
                _OdbcDataAdapter.Fill(dtRet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtRet;
        }
        /// <summary>
        /// 更新DataTable的方式插入数据
        /// </summary>
        /// <param name="_dt">DataTable</param>
        /// <returns>插入记录条数</returns>
        public override int DataTableInsert(DataTable _dt)
        {
            bool flag = false;
            int _nResult = 0;
            if (_dt == null)
                return _nResult;
            string _sCmdText = string.Format("select * from {0} where 1=2", _dt.TableName);
            AseCommand _Command = (AseCommand)CreateCommand(_sCmdText, CommandType.Text);
            AseDataAdapter _adapter = new AseDataAdapter(_Command);
            AseDataAdapter _adapter1 = new AseDataAdapter(_Command);
            AseCommandBuilder _builder = new AseCommandBuilder(_adapter1);

            _adapter.InsertCommand = _builder.GetInsertCommand();

            if (_adapter.InsertCommand.Parameters.Count < _dt.Columns.Count)
            {
                flag = true;//因为表中有自增字段，所以CommandBuild生成的InserttCommand的参数中少了自增字段
                foreach (DataColumn _dc in _dt.Columns)
                {
                    if (!_adapter.InsertCommand.Parameters.Contains(_dc.ColumnName))
                    {
                        _adapter.InsertCommand.CommandText =
                            _adapter.InsertCommand.CommandText.Insert(_adapter.InsertCommand.CommandText.IndexOf(") VALUES"), ',' + _dc.ColumnName);

                        _adapter.InsertCommand.CommandText =
                            _adapter.InsertCommand.CommandText.Insert(_adapter.InsertCommand.CommandText.Length - 1, ",@" + _dc.ColumnName);

                        _adapter.InsertCommand.Parameters.Add("@" + _dc.ColumnName, AseDbType.Decimal, _dc.MaxLength, _dc.ColumnName);

                        if (_adapter.InsertCommand.Parameters.Count >= _dt.Columns.Count)
                            break;
                    }
                }
            }

            if (flag)
                this.ExecuteNoQuery(string.Format("SET IDENTITY_INSERT {0} on", _dt.TableName));

            this.BeginTransaction();
            try
            {
                _Command.CommandText = "delete from " + _dt.TableName;
                _Command.ExecuteNonQuery();
                _adapter.InsertCommand.Transaction = _Command.Transaction;
                _nResult = _adapter.Update(_dt);
                this.CommitTransaction();
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                throw ex;
            }
            finally
            {
                if (flag)
                    this.ExecuteNoQuery(string.Format("SET IDENTITY_INSERT {0} OFF", _dt.TableName));
            }
            return _nResult;
        }

        public override DataSet GetDataSet(string cmdText)
        {
            DataSet ds = base.GetDataSet(cmdText);
            ProcessDataSet(ds);
            return ds;
        }

        public void ProcessDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if ((((row[i] != null) && (row[i] is string)) && (row[i] != DBNull.Value)) && (row[i].ToString() != ""))
                    {

                        row[i] = EncodingHelper.DB2Default(row[i], _DBEncodeing).ToString();
                    }
                }
            }
        }

        public void ProcessDataReader(IDataReader dr)
        {

            while (dr.Read())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    //dr[i]= EncodingHelper.DB2Default(dr.GetValue(i)); 

                }
            }
        }

        public void ProcessDataSet(DataSet dataSet)
        {
            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {

                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if ((((row[i] != null) && (row[i] is string)) && (row[i] != DBNull.Value)) && (row[i].ToString() != ""))
                        {

                            row[i] = EncodingHelper.DB2Default(row[i], _DBEncodeing).ToString();
                        }
                    }

                }
            }
        }


    }
}
