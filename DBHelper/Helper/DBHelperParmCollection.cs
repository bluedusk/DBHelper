using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DBH.Helper
{
    /// <summary>
    /// 数据操作参数集合
    /// </summary>
    public class DBHelperParmCollection : ArrayList
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dbHelperParm">数据操作参数</param>
        public void Add(DBHelperParm dbHelperParm)
        {
            base.Add(dbHelperParm);
        }

        /// <summary>
        /// 获取索引位置
        /// </summary>
        /// <param name="dbHelperParm"></param>
        /// <returns></returns>
        public int IndexOf(DBHelperParm dbHelperParm)
        {
            return base.IndexOf(dbHelperParm);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="dbHelperParm"></param>
        public void Remove(DBHelperParm dbHelperParm)
        {
            base.Remove(dbHelperParm);
        }
    }
}
