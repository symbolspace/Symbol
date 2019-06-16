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
    [SQLiteFunction(Name = "toDate_long_binary", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate_long_binary : SQLiteFunction {

        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            long? binary = TypeExtensions.Convert<long?>(args[0]);
            if (binary == null)
                return null;
            return DateTime.FromBinary(binary.Value);
        }
    }

#pragma warning restore CS1591
}