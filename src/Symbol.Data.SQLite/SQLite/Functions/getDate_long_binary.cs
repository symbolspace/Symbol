/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591
    /// <summary>
    /// 获取当前日期和时间。
    /// </summary>
    [SQLiteFunction(Name = "getDate_long_binary", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate_long_binary : SQLiteFunction {
        public override object Invoke(object[] args) {
            return DateTime.Now.ToBinary();
        }
    }

#pragma warning restore CS1591
}