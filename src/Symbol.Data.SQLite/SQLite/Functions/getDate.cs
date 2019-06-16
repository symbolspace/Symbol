/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 获取当前日期和时间（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "getDate", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate : SQLiteFunction {
        public override object Invoke(object[] args) {
            return DateTime.Now;
        }
    }

#pragma warning restore CS1591
}