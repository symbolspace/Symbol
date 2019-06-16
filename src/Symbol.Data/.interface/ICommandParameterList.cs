/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 命令参数列表接口。
    /// </summary>
    public interface ICommandParameterList
        : System.Collections.Generic.IEnumerable<CommandParameter>
        , System.IDisposable {

        #region properties
        /// <summary>
        /// 获取数据库提供者。
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// 获取数量。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取Return参数对象。
        /// </summary>
        CommandParameter ReturnParameter { get; }

        /// <summary>
        /// 获取指定索引的参数。
        /// </summary>
        /// <param name="index">索引值，从0开始。</param>
        /// <returns></returns>
        CommandParameter this[int index] { get; }
        /// <summary>
        /// 获取指定名称的参数。
        /// </summary>
        /// <param name="name">参数名称，null或empty直接忽略。</param>
        /// <returns></returns>
        CommandParameter this[string name] { get; }

        #endregion

        #region methods

        ///// <summary>
        ///// 预处理参数
        ///// </summary>
        ///// <param name="parameterName">参数名称。</param>
        ///// <param name="value">值。</param>
        ///// <returns></returns>
        //object PreParameter(string parameterName, object value);

        #region NextName
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <returns>返回下一个参数的名称。</returns>
        string NextName();
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <param name="offset">偏移多少个参数，可能用于预留。</param>
        /// <returns>返回下一个参数的名称。</returns>
        string NextName(int offset);
        #endregion

        #region Create
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Create(object value);
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Create(object value, object properties);
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Create(string name, object value);
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Create(string name, object value, object properties);
        #endregion

        #region Add
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="item">参数对象。</param>
        /// <returns></returns>
        CommandParameter Add(CommandParameter item);
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Add(object value);
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Add(object value, object properties);

        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Add(string name, object value);
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        CommandParameter Add(string name, object value, object properties);
        #endregion
        #region AddRange
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">参数列表。</param>
        void AddRange(object[] items);
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">参数列表。</param>
        void AddRange(System.Collections.IEnumerable items);
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">参数列表。</param>
        void AddRange(ICommandParameterList items);
        #endregion

        #region Remove
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="name">参数名称，null或empty直接忽略。</param>
        bool Remove(string name);
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="index">索引值，从0开始。</param>
        bool Remove(int index);
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="item">参数对象。</param>
        bool Remove(CommandParameter item);
        #endregion

        /// <summary>
        /// 清空参数。
        /// </summary>
        void Clear();

        #region Get
        /// <summary>
        /// 获取指定索引的参数。
        /// </summary>
        /// <param name="index">索引值，从0开始。</param>
        /// <returns></returns>
        CommandParameter Get(int index);
        /// <summary>
        /// 获取指定名称的参数。
        /// </summary>
        /// <param name="name">参数名称，null或empty直接忽略。</param>
        /// <returns></returns>
        CommandParameter Get(string name);
        #endregion

        /// <summary>
        /// 输出为数组。
        /// </summary>
        /// <returns></returns>
        CommandParameter[] ToArray();

        #endregion
    }
}