using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DBH.Helper
{
        /// <summary>
        /// SqlServer2000的数据操作
        /// </summary>
        public class SqlHelper : DBHelperBase
        {
            /// <summary>
            /// 默认构造函数
            /// </summary>
            /// <param name="myConnection">连接</param>
            public SqlHelper(SqlConnection myConnection)
                : base(myConnection)
            {
            }


            /// <summary>
            /// 获取命令
            /// </summary>
            /// <param name="cmdText"></param>
            /// <param name="commandType"></param>
            /// <returns></returns>
            protected override IDbCommand CreateCommand(string cmdText, CommandType commandType)
            {
                if (_IDbCommand == null)
                {
                    _IDbCommand = new SqlCommand();
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
                    _IDbDataAdapter = new SqlDataAdapter();
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
                SqlCommand _SqlCommand = (SqlCommand)CreateCommand(cmdText, CommandType.Text);
                _SqlCommand.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (DBHelperParm para in parameters)
                    {
                        _SqlCommand.Parameters.Add(new SqlParameter("@" + para.Key, para.Value));
                    }
                }
                try
                {
                    effectNum = _SqlCommand.ExecuteNonQuery();

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
                SqlCommand _SqlCommand = (SqlCommand)CreateCommand(cmdText, CommandType.Text);
                _SqlCommand.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (DBHelperParm para in parameters)
                    {
                        _SqlCommand.Parameters.Add(new SqlParameter("@" + para.Key, para.Value));
                    }
                }
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(_SqlCommand);
                try
                {
                    _SqlDataAdapter.Fill(dtRet);

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
                SqlCommand _Command = (SqlCommand)CreateCommand(_sCmdText, CommandType.Text);
                SqlDataAdapter _adapter = new SqlDataAdapter(_Command);
                SqlDataAdapter _adapter1 = new SqlDataAdapter(_Command);
                SqlCommandBuilder _builder = new SqlCommandBuilder(_adapter1);

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

                            _adapter.InsertCommand.Parameters.Add("@" + _dc.ColumnName, SqlDbType.Decimal, _dc.MaxLength, _dc.ColumnName);

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
