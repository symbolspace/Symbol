namespace Symbol;

/// <summary>
/// 接口：实体属性绑定器。
/// </summary>
public interface IEntityPropertyBinder
{
    /// <summary>
    /// 绑定属性。
    /// </summary>
    /// <param name="propertyValueGetter">属性取值获取器。</param>
    void BindProperty(EntityBinderPropertyValueGetter propertyValueGetter);
}

/// <summary>
/// 委托：实例绑定器属性值获取器。
/// </summary>
/// <param name="propertyName">属性名称。</param>
/// <returns>返回该属性的取值，不存在则返回为null。</returns>
public delegate object EntityBinderPropertyValueGetter(string propertyName);