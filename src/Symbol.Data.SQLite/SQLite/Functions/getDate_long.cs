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
    [SQLiteFunction(Name = "getDate_long", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate_long : SQLiteFunction {
        public override object Invoke(object[] args) {
            return DateTime.Now.Ticks;
        }
    }

#pragma warning restore CS1591
}