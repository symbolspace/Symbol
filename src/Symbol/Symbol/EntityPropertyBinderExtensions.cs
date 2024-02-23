using System;
using System.Collections.Generic;

namespace Symbol;

/// <summary>
/// 静态扩展：实体属性绑定器。
/// </summary>
public static class EntityPropertyBinderExtensions
{
    /// <summary>
    /// 构建属性值获取器委托。
    /// </summary>
    /// <param name="properties">属性清单。</param>
    /// <returns>返回委托。</returns>
    public static EntityBinderPropertyValueGetter PropertyValueGetter(
#if !NET20
        this 
#endif
        IDictionary<string, object> properties)
    {
        EntityBinderPropertyValueGetter getter = (propertyName) =>
        {
            if (properties == null || string.IsNullOrEmpty(propertyName) || properties.Count == 0)
                return null;
            properties.TryGetValue(propertyName, out object value);
            if (value is DBNull)
                value = null;
            return value;
        };
        return getter;
    }

    /// <summary>
    /// 构建属性值获取器委托（泛型）。
    /// </summary>
    /// <typeparam name="TValue">值的类型。</typeparam>
    /// <param name="properties">属性清单。</param>
    /// <returns>返回委托。</returns>
    public static EntityBinderPropertyValueGetter PropertyValueGetter<TValue>(
#if !NET20
        this 
#endif
        IDictionary<string, TValue> properties)
    {
        EntityBinderPropertyValueGetter getter = (propertyName) =>
        {
            if (properties == null || string.IsNullOrEmpty(propertyName) || properties.Count == 0)
                return default(TValue);
            properties.TryGetValue(propertyName, out TValue value);
            if (value is DBNull || value == null)
                value = default;
            return value;
        };
        return getter;
    }
}