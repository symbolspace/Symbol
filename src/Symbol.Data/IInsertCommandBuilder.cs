/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 插入命令构造器接口
    /// </summary>
    public interface IInsertCommandBuilder : System.IDisposable {
        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        IDataContext DataContext { get; }

        /// <summary>
        /// 获取当前表名
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// 获取生成的命令语句。
        /// </summary>
        string CommandText { get; }
        /// <summary>
        /// 获取字段列表（包括字段对应的数据）。
        /// </summary>
        Symbol.Collections.Generic.NameValueCollection<object> Fields { get; }
        /// <summary>
        /// 获取已移除字段列表（生成脚本时忽略这些字段）。
        /// </summary>
        Symbol.Collections.Generic.HashSet<string> RemovedFields { get; }
        /// <summary>
        /// 获取纯参数列表。
        /// </summary>
        object[] Values { get; }

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string name);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string pairs, string spliter);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string[] pairs);
        #endregion

    }
    /// <summary>
    /// 插入命令构造器过滤委托。
    /// </summary>
    /// <param name="builder">构造器。</param>
    public delegate void InsertCommandBuilderFilter(IInsertCommandBuilder builder);

}
