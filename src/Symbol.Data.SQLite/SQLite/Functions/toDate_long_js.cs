/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 强制转为日期类型(js new Date(long); )
    /// </summary>
    [SQLiteFunction(Name = "toDate_long_js", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate_long_js : SQLiteFunction {
        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            long? ticks = TypeExtensions.Convert<long?>(args[0]);
            if (ticks == null)
                return null;
            return HttpUtility.FromJsTick(ticks.Value);
        }
    }

#pragma warning restore CS1591
}