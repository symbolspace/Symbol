/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 计算两个日期之间的相差天数，若其中一个参数无法转为日期，将直接返回为0。
    /// </summary>
    [SQLiteFunction(Name = "getDateOffset_days", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class getDateOffset_days : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);
            if (d1 == null || d2 == null)
                return 0;
            TimeSpan t = d1.Value.Date - d2.Value.Date;
            return (int)t.TotalDays;
        }
    }

#pragma warning restore CS1591
}