namespace DBH.Helper
{
    /// <summary>
    /// 数据操作参数
    /// </summary>
    public class DBHelperParm
    {
        private string _Key;
        private object _Value;

        public DBHelperParm()
        {
        }

        public DBHelperParm(string key, object value)
        {
            _Key = key;
            _Value = value;
        }

        /// <summary>
        /// 键
        /// </summary>
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }
}
