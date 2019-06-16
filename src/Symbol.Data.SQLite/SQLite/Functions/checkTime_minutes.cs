/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 判断指定的日期距离当前时间相差的分钟数是否在指定的范围内，如果其中一个参数无法被转换，将直接返回false。
    /// </summary>
    [SQLiteFunction(Name = "checkTime_minutes", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class checkTime_minutes : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            int? min = TypeExtensions.Convert<int?>(args[1]);
            if (d1 == null || min == null)
                return false;
            if ((DateTime.Now - d1.Value).TotalMinutes > min)
                return false;
            return true;
        }
    }

#pragma warning restore CS1591
}