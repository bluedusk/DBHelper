using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBH.Helper;
using System.Data;
using MySql.Data.MySqlClient;

namespace MySqlHelper
{
    public class MySqlHelper : DBHelperBase
    {
        public MySqlHelper(MySqlConnection myConnection)
            : base(myConnection)
        {
        }
        public MySqlHelper(string ConnStr)
            : base(new MySqlConnection(ConnStr))
        {
        }
        public override int ExecuteProcedureNoQuery(string procedureName, DBHelperParmCollection parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataTable ExecuteProcedureQuery(string procedureName, DBHelperParmCollection parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override IDbCommand CreateCommand(string cmdText, CommandType commandType)
        {
            if (_IDbCommand == null)
            {
                _IDbCommand = new MySqlCommand();
            }
            _IDbCommand.Connection = IDbConnection;
            _IDbCommand.CommandText = cmdText;
            _IDbCommand.CommandType = CommandType.Text;

            return _IDbCommand;
        }

        protected override IDbDataAdapter CreateAdapter(string cmdText)
        {
            if (_IDbDataAdapter == null)
            {
                _IDbDataAdapter = new MySqlDataAdapter();
            }
            _IDbDataAdapter.SelectCommand = CreateCommand(cmdText, CommandType.Text);

            return _IDbDataAdapter;
        }

        public override int ExecuteNoQuery(string cmdText, DBHelperParmCollection parameters)
        {
            int effectNum;
            MySqlCommand _MySqlCommand = (MySqlCommand)CreateCommand(cmdText, CommandType.Text);
            _MySqlCommand.Parameters.Clear();
            if (parameters != null)
            {
                foreach (DBHelperParm dbPara in parameters)
                {
                    _MySqlCommand.Parameters.Add(new MySqlParameter("?" + dbPara.Key, dbPara.Value));
                }
            }
            try
            {
                effectNum = _MySqlCommand.ExecuteNonQuery();
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
            MySqlCommand _MySqlCommand = (MySqlCommand)CreateCommand(cmdText, CommandType.Text);
            _MySqlCommand.Parameters.Clear();
            if (parameters != null)
            {
                foreach (DBHelperParm dbPara in parameters)
                {
                    _MySqlCommand.Parameters.Add(new MySqlParameter("?" + dbPara.Key, dbPara.Value));
                }
            }
            MySqlDataAdapter _MySqlDataAdapter = new MySqlDataAdapter(_MySqlCommand);
            try
            {
                _MySqlDataAdapter.Fill(dtRet);
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
            MySqlCommand _Command = (MySqlCommand)CreateCommand(_sCmdText, CommandType.Text);
            MySqlDataAdapter _adapter = new MySqlDataAdapter(_Command);
            MySqlDataAdapter _adapter1 = new MySqlDataAdapter(_Command);
            MySqlCommandBuilder _builder = new MySqlCommandBuilder(_adapter1);

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

                        _adapter.InsertCommand.Parameters.Add("@" + _dc.ColumnName, MySqlDbType.Decimal, _dc.MaxLength, _dc.ColumnName);

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
