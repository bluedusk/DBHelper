using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DBH.Config;

namespace DBH.Util
{

        public static class EncodingHelper
        {

            public static Encoding DefaultCoding = Encoding.Default;
            //public static Encoding DBCoding = Encoding.GetEncoding(DBConfig.GetDBConfig().Sybase.DBCoding);


            ///// <summary>
            ///// Encoding 转换
            ///// </summary>
            ///// <param name="ob"></param>
            ///// <returns></returns>
            //public static object Default2DB(object ob)
            //{
            //    if ((((ob != null) && (ob is string)) && (ob != DBNull.Value)) && (ob.ToString() != ""))
            //    {
            //        ob = DBCoding.GetString(DefaultCoding.GetBytes(ob.ToString()));

            //    }
            //    return ob;
            //}

            ///// <summary>
            ///// Encoding 转换
            ///// </summary>
            ///// <param name="ob"></param>
            ///// <returns></returns>
            //public static object DB2Default(object ob)
            //{
   
            //    if ((((ob != null) && (ob is string)) && (ob != DBNull.Value)) && (ob.ToString() != ""))
            //    {
            //        ob = DefaultCoding.GetString(DBCoding.GetBytes(ob.ToString()));

            //    }

            //    return ob;
            //}

            /// <summary>
            /// Encoding 转换
            /// </summary>
            /// <param name="ob"></param>
            /// <returns></returns>
            public static object Default2DB(object ob,string DBEnoding)
            {
                Encoding DBCoding = Encoding.GetEncoding(DBEnoding);
                if ((((ob != null) && (ob is string)) && (ob != DBNull.Value)) && (ob.ToString() != ""))
                {
                    ob = DBCoding.GetString(DefaultCoding.GetBytes(ob.ToString()));

                }
                return ob;
            }

            /// <summary>
            /// Encoding 转换
            /// </summary>
            /// <param name="ob"></param>
            /// <returns></returns>
            public static object DB2Default(object ob, string DBEnoding)
            {
                Encoding DBCoding = Encoding.GetEncoding(DBEnoding);
                if ((((ob != null) && (ob is string)) && (ob != DBNull.Value)) && (ob.ToString() != ""))
                {
                    ob = DefaultCoding.GetString(DBCoding.GetBytes(ob.ToString()));

                }

                return ob;
            }
        }
}
