/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 强制转为日期类型
    /// </summary>
    [SQLiteFunction(Name = "toDate_long", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate_long : SQLiteFunction {
        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            long? ticks = TypeExtensions.Convert<long?>(args[0]);
            if (ticks == null)
                return null;
            return new DateTime(ticks.Value);
        }
    }

#pragma warning restore CS1591
}