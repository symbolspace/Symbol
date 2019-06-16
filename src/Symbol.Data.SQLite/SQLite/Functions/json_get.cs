/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 从一个JSON文本中获取指定路径的数据。
    /// </summary>
    [SQLiteFunction(Name = "json_get", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class json_get : SQLiteFunction {
        public override object Invoke(object[] args) {
            string path = args[1] as string;
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(path))
                return null;
            object json = JSON.Parse(args[0] as string);
            return FastObject.Path(json, path);
        }
    }

#pragma warning restore CS1591
}