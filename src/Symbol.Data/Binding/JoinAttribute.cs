/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// Join数据绑定特性（将字段的值拼接起来，以spliter分隔）。
    /// </summary>
    public class JoinAttribute : ListAttribute {

        #region properties
        /// <summary>
        /// 获取或设置分隔符。
        /// </summary>
        public string Spliter { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建JoinAttribute实例。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        /// <param name="condition">过虑规则。</param>
        /// <param name="field">输出字段，输出为单值时。</param>
        /// <param name="spliter">分隔符。</param>
        /// <param name="sort">排序规则。</param>
        public JoinAttribute(string sourceName, string condition, string field, string spliter = "，", string sort = "{}")
            : base(sourceName, condition, field, sort) {
            Spliter = spliter;
        }
        #endregion

        #region methods

        #region Bind
        /// <summary>
        /// 绑定数据。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        /// <param name="dataReader">数据读取对象。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="type">实体中字段的类型。</param>
        /// <param name="cache">缓存。</param>
        /// <returns>返回绑定的数据。</returns>
        public override object Bind(IDataContext dataContext, System.Data.IDataReader dataReader, object entity, string field, Type type, IDataBinderObjectCache cache) {
            var list = (System.Collections.Generic.IList<string>)base.Bind(dataContext, dataReader, entity, field, typeof(System.Collections.Generic.List<string>), cache);
#if net20 || net35
            return StringExtensions.Join(list, Spliter);
#else
            return string.Join(Spliter, list);
#endif
        }
        #endregion

        #endregion

    }
}