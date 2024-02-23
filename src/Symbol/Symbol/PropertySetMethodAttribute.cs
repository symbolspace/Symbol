using System;

namespace Symbol;

/// <summary>
/// 特性：属性设置方法。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PropertySetMethodAttribute : Attribute
{
    /// <summary>
    /// 获取属性名称。
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 创建实例。
    /// </summary>
    /// <param name="name">属性名称，不能为空。</param>
    public PropertySetMethodAttribute(string name)
    {
        Throw.CheckArgumentNull(name, nameof(name));
        Name = name;
    }
}