using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace DBH.Helper
{
    /// <summary>
    /// DBHelper 扩展类
    /// </summary>

    public class DBHelperEx : DBHelper
    {
        /// <summary>
        /// 返回单行单列数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>单行单列数据</returns>
        public static string GetString(string sql)
        {
            DataTable dataTable = ExecuteQuery(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0][0].ToString();
            }
            return null;
        }


        /// <summary>
        /// 返回单行记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static ArrayList GetDataRow(string sql)
        {
            DataTable dataTable = ExecuteQuery(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ArrayList tmp = new ArrayList();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    tmp.Add(dataTable.Rows[0][i].ToString());
                }
                return tmp;
            }
            return null;
        }

        /// <summary>
        /// 返回单列记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static ArrayList GetDataCol(string sql)
        {
            DataTable dataTable = ExecuteQuery(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ArrayList tmp = new ArrayList();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    tmp.Add(dataTable.Rows[i][0].ToString());
                }
                return tmp;
            }
            return null;
        }

        /// <summary>
        /// 返回List List中元素为String数组
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static ArrayList GetStringList(string sql)
        {
            DataTable dataTable = ExecuteQuery(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ArrayList tmp = new ArrayList();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string[] tmpS = new string[dataTable.Columns.Count];
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        tmpS[j] = dataTable.Rows[i][j].ToString();
                    }
                    tmp.Add(tmpS);
                }
                return tmp;
            }
            return null;
        }

    }

}
