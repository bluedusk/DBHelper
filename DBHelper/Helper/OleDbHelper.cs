using System;
using System.Data;
using System.Data.OleDb;
namespace DBH.Helper
{
        /// <summary>
        /// Sybase 的数据操作
        /// </summary>
        public class OleDbHelper : DBHelperBase
        {
            public OleDbHelper(OleDbConnection myConnection)
                : base(myConnection)
            {
            }
            protected override IDbCommand CreateCommand(string cmdText, CommandType commandType)
            {
                if (_IDbCommand == null)
                {
                    _IDbCommand = new OleDbCommand();
                }
                _IDbCommand.Connection = IDbConnection;
                _IDbCommand.CommandText = cmdText;
                _IDbCommand.CommandType = commandType;

                return _IDbCommand;
            }

            protected override IDbDataAdapter CreateAdapter(string cmdText)
            {
                if (_IDbDataAdapter == null)
                {
                    _IDbDataAdapter = new OleDbDataAdapter();
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
                OleDbCommand _OleDbCommand = (OleDbCommand)CreateCommand(cmdText, CommandType.Text);
                _OleDbCommand.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (DBHelperParm para in parameters)
                    {
                        _OleDbCommand.Parameters.Add(new OleDbParameter("?" + para.Key, para.Value));
                    }
                }
                try
                {
                    effectNum = _OleDbCommand.ExecuteNonQuery();
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
                OleDbCommand _OleDbCommand = (OleDbCommand)CreateCommand(cmdText, CommandType.Text);
                _OleDbCommand.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (DBHelperParm para in parameters)
                    {
                        _OleDbCommand.Parameters.Add(new OleDbParameter("?" + para.Key, para.Value));
                    }
                }
                OleDbDataAdapter _OdbcDataAdapter = new OleDbDataAdapter(_OleDbCommand);
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
                OleDbCommand _Command = (OleDbCommand)CreateCommand(_sCmdText, CommandType.Text);
                OleDbDataAdapter _adapter = new OleDbDataAdapter(_Command);
                OleDbDataAdapter _adapter1 = new OleDbDataAdapter(_Command);
                OleDbCommandBuilder _builder = new OleDbCommandBuilder(_adapter1);

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

                            _adapter.InsertCommand.Parameters.Add("@" + _dc.ColumnName, OleDbType.Decimal, _dc.MaxLength, _dc.ColumnName);

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
                    _adapter.InsertCommand.Transaction = _Command.Transaction;
                    _Command.CommandText = "delete from " + _dt.TableName;
                    _Command.ExecuteNonQuery();
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
        }
}
