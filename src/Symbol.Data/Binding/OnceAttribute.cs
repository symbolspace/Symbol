/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 单条数据绑定特性。
    /// </summary>
    public class OnceAttribute : DataBinderAttribute {

        #region ctor
        /// <summary>
        /// 创建OnceAttribute实例。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        /// <param name="condition">过虑规则。</param>
        /// <param name="field">输出字段，输出为单值时。</param>
        /// <param name="sort">排序规则。</param>
        public OnceAttribute(string sourceName, string condition, string field = "*", string sort = "{}")
            : base(sourceName, condition, field, sort) {
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
            bool isSingleValue = (type == typeof(string) || type.IsValueType || TypeExtensions.IsNullableType(type));

            if (isSingleValue && (string.IsNullOrEmpty(Field) || Field == "*"))
                Field = "id";

            using (var builder = dataContext.CreateSelect(SourceName)) {
                //PreSelectBuilder(dataContext, dataReader, entity, builder, cache);
                if (isSingleValue) {
                    builder.Select(Field);
                }
                var conditiion = MapObject(Condition, dataContext, entity, dataReader);
                builder.Query(conditiion).Sort(Sorter);
                return CacheFunc(cache, builder, "once", type, () => {
                    var q = dataContext.CreateQuery(type, builder.CommandText, builder.Parameters);
                    q.DataBinderObjectCache = cache;
                    return TypeExtensions.Convert(q.FirstOrDefault(), type);
                });
            }
        }
        #endregion

        #endregion
    }
}