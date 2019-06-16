/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 强制转为日期类型
    /// </summary>
    [SQLiteFunction(Name = "toDate", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate : SQLiteFunction {
        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            return TypeExtensions.Convert<DateTime?>(args[0]);
        }
    }

#pragma warning restore CS1591
}