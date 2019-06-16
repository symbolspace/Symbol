/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 获取一个Guid（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "newId", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class newId : SQLiteFunction {
        public override object Invoke(object[] args) {
            return Guid.NewGuid().ToString("D");
        }
    }

#pragma warning restore CS1591
}