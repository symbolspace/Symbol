/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 计算两个日期之间的相差分钟数（带小数），若其中一个参数无法转为日期，将直接返回为0。
    /// </summary>
    [SQLiteFunction(Name = "getDateOffset_minutes", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class getDateOffset_minutes : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);
            if (d1 == null || d2 == null)
                return 0D;
            TimeSpan t = d1.Value - d2.Value;
            return t.TotalMinutes;
        }
    }

#pragma warning restore CS1591
}