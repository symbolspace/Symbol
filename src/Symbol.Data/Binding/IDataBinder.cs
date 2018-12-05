/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 数据绑定接口
    /// </summary>
    public interface IDataBinder {
        /// <summary>
        /// 获取源名称。
        /// </summary>
        string SourceName { get; }
        /// <summary>
        /// 获取过虑规则。
        /// </summary>
        Symbol.Data.NoSQL.Condition Condition { get; }
        /// <summary>
        /// 获取排序规则。
        /// </summary>
        Symbol.Data.NoSQL.Sorter Sorter { get; }
        /// <summary>
        /// 获取或设置输出字段。
        /// </summary>
        string Field { get; set; }
        ///// <summary>
        ///// 获取或设置元素Path，设置后将对最终输出的值，调用一次Path。
        ///// </summary>
        //string ElementPath { get; set; }
        /// <summary>
        /// 获取或设置允许缓存，默认为true。
        /// </summary>
        bool AllowCache { get; set; }


        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        /// <param name="dataReader">数据读取对象。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="type">实体中字段的类型。</param>
        /// <param name="cache">缓存。</param>
        /// <returns>返回绑定的数据。</returns>
        object Bind(IDataContext dataContext, System.Data.IDataReader dataReader, object entity, string field, Type type, IDataBinderObjectCache cache);
    }
}