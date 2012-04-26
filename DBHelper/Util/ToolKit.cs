using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DBH.Util
{
    public class ToolKit
    {

        public static Type GetType(string implClass)
        {
            if (string.IsNullOrEmpty(implClass))
            {
                throw new ArgumentNullException("输入参数implClass不能为null或空值");
            }

            string[] classNameArray = implClass.Split(new char[] { ':', '-' });
            Type theType = null;
            if (classNameArray.Length == 1)
            {
                theType = typeof(ToolKit).Assembly.GetType(implClass);
            }
            else
            {
                Assembly theAssembly = typeof(ToolKit).Assembly;

                if (theAssembly.FullName.StartsWith(classNameArray[0]))
                {
                    theType = theAssembly.GetType(classNameArray[1]);
                }
                else
                {
                    theAssembly = Assembly.Load(classNameArray[0]);
                    theType = theAssembly.GetType(classNameArray[1]);
                }
            }
            return theType;
        }
        public static object CreateInstance(string implClass, params object[] parameters)
        {
            Type theType = GetType(implClass);
            if (theType == null)
            {
                throw new Exception("implClass not defined :" + implClass);
            }

            return Activator.CreateInstance(theType, parameters);
        }
    }
}
