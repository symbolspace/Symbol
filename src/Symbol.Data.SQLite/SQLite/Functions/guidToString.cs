/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// guid 转为字符串，32位char
    /// </summary>
    [SQLiteFunction(Name = "guidToString", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class guidToString : SQLiteFunction {
        public override object Invoke(object[] args) {
            Guid g;
            if (args[0] is Guid) {
                g = (Guid)args[0];
            } else {
                string p1 = args[0] as string;
                if (string.IsNullOrEmpty(p1))
                    return string.Empty;
                g = new Guid(p1);
            }
            return g.ToString("N");
        }
    }

#pragma warning restore CS1591
}