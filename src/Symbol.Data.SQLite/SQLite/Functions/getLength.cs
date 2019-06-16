/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 获取文本的长度，null或空文本将返回0 。
    /// </summary>
    [SQLiteFunction(Name = "getLength", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class getLength : SQLiteFunction {
        public override object Invoke(object[] args) {
            string p1 = TypeExtensions.Convert<string>(args[0]);
            if (string.IsNullOrEmpty(p1))
                return 0;
            return p1.Length;
        }
    }

#pragma warning restore CS1591
}