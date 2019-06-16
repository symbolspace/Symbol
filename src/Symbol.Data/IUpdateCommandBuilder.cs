/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data {

    /// <summary>
    /// 更新命令构造器接口
    /// </summary>
    public interface IUpdateCommandBuilder : System.IDisposable {
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

        /// <summary>
        /// 获取方言对象。
        /// </summary>
        IDialect Dialect { get; }

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        [Obsolete("请更改为.Dialect.PreName(string name)")]
        string PreName(string name);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        [Obsolete("请更改为.Dialect.PreName(string pairs, string spliter)")]
        string PreName(string pairs, string spliter);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        [Obsolete("请更改为.Dialect.PreName(string[] pairs)")]
        string PreName(string[] pairs);
        #endregion


        /// <summary>
        /// 获取参数列表。
        /// </summary>
        /// <param name="values">附加参数列表。</param>
        /// <returns>返回附近加的参数列表。</returns>
        object[] GetValues(params object[] values);
        /// <summary>
        /// 获取命令语句。
        /// </summary>
        /// <param name="commandTextAfterFormat">语句结尾内容格式串。</param>
        /// <param name="args">参与 commandTextAfterFormat 的参数列表。</param>
        /// <returns>返回新的语句。</returns>
        string GetCommandText(string commandTextAfterFormat, params object[] args);
        /// <summary>
        /// 查询块，通常用于最终执行前 生成where。
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="where">where action</param>
        /// <param name="end">end action</param>
        /// <returns>返回处理结果。</returns>
        T QueryBlock<T>(UpdateCommandBuilderQueryBlockWhereAction where, UpdateCommandBuilderQueryBlockEndAction<T> end);
    }
    /// <summary>
    /// 更新命令构造器查询块where委托。
    /// </summary>
    /// <param name="whereBuilder">where构造器</param>
    /// <returns>返回是否继续。</returns>
    public delegate bool UpdateCommandBuilderQueryBlockWhereAction(ISelectCommandBuilder whereBuilder);
    /// <summary>
    /// 更新命令构造器查询块结果委托。
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="commandText">命令语句</param>
    /// <param name="values">参数列表</param>
    /// <returns>返回处理结果。</returns>
    public delegate T UpdateCommandBuilderQueryBlockEndAction<T>(string commandText, object[] values);
    /// <summary>
    /// 更新命令构造器过滤委托。
    /// </summary>
    /// <param name="builder">构造器。</param>
    public delegate void UpdateCommandBuilderFilter(IUpdateCommandBuilder builder);
}
