using System;
using System.Data;
using System.Data.Odbc;
using DBH.Helper;

namespace OdbcHelper
{
    /// <summary>
    /// OdbcDBHelper
    /// </summary>
    public class OdbcHelper : DBHelperBase
    {
        public OdbcHelper(OdbcConnection myConnection)
            : base(myConnection)
        {
        }
        public OdbcHelper(string ConnStr)
            : base(new OdbcConnection(ConnStr))
        {
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">过程名</param>
        /// <returns>返回执行正确的行数</returns>
        public override int ExecuteProcedureNoQuery(string procedureName, DBHelperParmCollection parameters)
        {
            int iRtn = -1;
            try
            {
                string realProcedureName = string.Format("{{CALL {0} (", procedureName);

                for (int i = 0; i < parameters.Count; i++)
                {
                    realProcedureName += "?,";
                }
                realProcedureName = realProcedureName.Substring(0, realProcedureName.Length - 1);
                realProcedureName += ")}";

                OdbcCommand _OdbcCommand = (OdbcCommand)CreateCommand(realProcedureName, CommandType.StoredProcedure);
                if (parameters != null)
                {
                    foreach (DBHelperParm para in parameters)
                    {
                        _OdbcCommand.Parameters.Add(new OdbcParameter("?" + para.Key, para.Value));
                    }
                }

                iRtn = _OdbcCommand.ExecuteNonQuery();

                // 设置异常问题
                if (iRtn == -1)
                    _ErrorMsg = "查无记录!";
            }
            catch (Exception ex)
            {
                _ErrorMsg = ex.Message;
            }
            return iRtn;
        }

        /// <summary>
        /// 执行存储过程，返回结果数据
        /// </summary>
        /// <param name="procedureName">过程名</param>
        public override DataTable ExecuteProcedureQuery(string procedureName, DBHelperParmCollection parameters)
        {
            try
            {
                DataTable dtbRtn = new DataTable();
                string realProcedureName = string.Format("{{CALL {0} (", procedureName);

                for (int i = 0; i < parameters.Count; i++)
                {
                    realProcedureName += "?,";
                }
                realProcedureName = realProcedureName.Substring(0, realProcedureName.Length - 1);
                realProcedureName += ")}";

                OdbcCommand _OdbcCommand = (OdbcCommand)CreateCommand(realProcedureName, CommandType.StoredProcedure);

                if (parameters != null)
                {
                    foreach (DBHelperParm para in parameters)
                    {
                        _IDbCommand.Parameters.Add(new OdbcParameter("?" + para.Key, para.Value));
                    }
                }
                OdbcDataAdapter _OdbcDataAdapter = new OdbcDataAdapter(_OdbcCommand);

                _OdbcDataAdapter.Fill(dtbRtn);

                return dtbRtn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override IDbCommand CreateCommand(string cmdText, CommandType commandType)
        {
            if (_IDbCommand == null)
            {
                _IDbCommand = new OdbcCommand();
            }
            _IDbCommand.Connection = IDbConnection;
            _IDbCommand.CommandText = cmdText;
            _IDbCommand.CommandType = commandType;

            return _IDbCommand;
        }

        /// <summary>
        /// 返回爱影响的行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override int ExecuteNoQuery(string cmdText, DBHelperParmCollection parameters)
        {
            int effectNum;
            OdbcCommand _OdbcCommand = (OdbcCommand)CreateCommand(cmdText, CommandType.Text);
            _OdbcCommand.Parameters.Clear();
            if (parameters != null)
            {
                foreach (DBHelperParm para in parameters)
                {
                    _OdbcCommand.Parameters.Add(new OdbcParameter("?" + para.Key, para.Value));
                }
            }
            try
            {
                effectNum = _OdbcCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return effectNum;
        }

        public override DataTable ExecuteQuery(string cmdText, DBHelperParmCollection parameters)
        {
            DataTable dtRet = new DataTable();
            OdbcCommand _OdbcCommand = (OdbcCommand)CreateCommand(cmdText, CommandType.Text);
            _OdbcCommand.Parameters.Clear();
            if (parameters != null)
            {
                foreach (DBHelperParm para in parameters)
                {
                    _OdbcCommand.Parameters.Add(new OdbcParameter("?" + para.Key, para.Value));
                }
            }
            OdbcDataAdapter _OdbcDataAdapter = new OdbcDataAdapter(_OdbcCommand);
            try
            {
                _OdbcDataAdapter.Fill(dtRet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtRet;
        }

        protected override IDbDataAdapter CreateAdapter(string cmdText)
        {
            if (_IDbDataAdapter == null)
            {
                _IDbDataAdapter = new OdbcDataAdapter();

            }
            _IDbDataAdapter.SelectCommand = CreateCommand(cmdText, CommandType.Text);

            return _IDbDataAdapter;
        }

        /// <summary>
        /// 插入数据通过Datatable
        /// </summary>
        /// <param name="_dt"></param>
        /// <returns>影响记录条数</returns>
        public override int DataTableInsert(DataTable _dt)
        {
            bool flag = false;
            int _nResult = 0;
            if (_dt == null)
                return _nResult;
            string _sCmdText = string.Format("select * from {0} where 1=2", _dt.TableName);
            OdbcCommand _Command = (OdbcCommand)CreateCommand(_sCmdText, CommandType.Text);
            OdbcDataAdapter _adapter = new OdbcDataAdapter(_Command);
            OdbcDataAdapter _adapter1 = new OdbcDataAdapter(_Command);
            OdbcCommandBuilder _builder = new OdbcCommandBuilder(_adapter1);
            DataTable dt = new DataTable(_dt.TableName);
            _adapter.Fill(dt);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            _dt.WriteXml(ms);

            ms.Seek(0, System.IO.SeekOrigin.Begin);
            dt.ReadXml(ms);
            ms.Dispose();

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
                            _adapter.InsertCommand.CommandText.Insert(_adapter.InsertCommand.CommandText.Length - 1, ",?");

                        _adapter.InsertCommand.Parameters.Add("@" + _dc.ColumnName, OdbcType.Decimal, _dc.MaxLength, _dc.ColumnName);

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
                _nResult = _adapter.Update(dt);
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
                dt.Dispose();
            }
            return _nResult;
        }
    }
}
