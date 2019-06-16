/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 格式化json文本。
    /// </summary>
    [SQLiteFunction(Name = "json_format", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class json_format : SQLiteFunction {
        public override object Invoke(object[] args) {
            return JSON.Beautify(args[0] as string);
        }
    }

#pragma warning restore CS1591
}