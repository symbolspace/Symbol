/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 将日期转换为数字格式：20130729，如果无法转换为日期类型，将直接返回0。
    /// </summary>
    [SQLiteFunction(Name = "getDayNumber", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class getDayNumber : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d = TypeExtensions.Convert<DateTime?>(args[0]);
            if (d == null)
                return 0;
            return DateTimeExtensions.ToDayNumber(d.Value);
        }
    }

#pragma warning restore CS1591
}