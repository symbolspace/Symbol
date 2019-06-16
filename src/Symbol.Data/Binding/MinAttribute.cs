/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 求Min数据绑定特性。
    /// </summary>
    public class MinAttribute : DataBinderAttribute {

        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        /// <param name="field">字段。</param>
        /// <param name="condition">过虑规则。</param>
        public MinAttribute(string sourceName, string field, string condition)
            : base(sourceName, condition, field) {
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
                builder.Min(string.IsNullOrEmpty(Field) ? "id" : Field);

                var condition = MapObject(Condition, dataContext, entity, reader);
                builder.Query(condition);
                return CacheFunc(cache, builder, "min", type, () => {
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