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
    [SQLiteFunction(Name = "getDate_long_value", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class getDate_long_value : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d = TypeExtensions.Convert<DateTime?>(args[0]);
            if (d == null)
                return 0L;
            return d.Value.Ticks;
        }
    }

#pragma warning restore CS1591
}