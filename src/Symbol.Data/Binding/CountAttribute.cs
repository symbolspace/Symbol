/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 求数量数据绑定特性。
    /// </summary>
    public class CountAttribute : DataBinderAttribute {

        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        /// <param name="condition">过虑规则。</param>
        public CountAttribute(string sourceName, string condition)
            : base(sourceName, condition, "1", "{}") {
        }
        #endregion

        #region methods

        #region Bind
        /// <summary>
        /// 绑定数据。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        /// <param name="reader">数据查询读取器。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="type">实体中字段的类型。</param>
        /// <param name="cache">缓存。</param>
        /// <returns>返回绑定的数据。</returns>
        public override object Bind(IDataContext dataContext, IDataQueryReader reader, object entity, string field, Type type, IDataBinderObjectCache cache) {
            using (var builder = dataContext.CreateSelect(SourceName)) {
                builder.Count();
               
                var condition = MapObject(Condition, dataContext, entity, reader);
                builder.Query(condition);
                return CacheFunc(cache, builder, "count", type, () => {
                    var value = dataContext.ExecuteScalar(builder.CommandText, builder.Parameters);
                    if (value == null && type.IsValueType)
                        return TypeExtensions.DefaultValue(type);
                    return TypeExtensions.Convert(value, type);
                });
            }
        }
        #endregion

        #endregion
    }
}