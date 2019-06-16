/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 获取当前日期和时间（js new Date().getTime() ）。
    /// </summary>
    [SQLiteFunction(Name = "getDate_long_js", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate_long_js : SQLiteFunction {
        public override object Invoke(object[] args) {
            return HttpUtility.JsTick();
        }
    }

#pragma warning restore CS1591
}