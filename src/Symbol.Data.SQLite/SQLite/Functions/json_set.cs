/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 对JSON文本的指定路径进行赋值，并返回操作后的JSON文本。
    /// </summary>
    [SQLiteFunction(Name = "json_set", Arguments = 3, Type = FunctionTypes.Scalar)]
    public class json_set : SQLiteFunction {
        public override object Invoke(object[] args) {
            string path = args[1] as string;
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(path))
                return args[0];
            object json = JSON.Parse(args[0] as string);
            FastObject.Path(json, path, args[2]);
            return JSON.ToJSON(json);
        }
    }

#pragma warning restore CS1591
}