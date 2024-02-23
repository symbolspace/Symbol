using System;
using System.Diagnostics;

namespace Symbol;

/// <summary>
/// 特性：类型实现映射
/// </summary>
[DebuggerDisplay("[{FamilyName}]{BaseType}|{TargetType}")]
[Serializable]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public class TypeImplementMapAttribute : Attribute {
    /// <summary>
    /// 获取基础类型。
    /// </summary>
    public Type BaseType { get; private set; }
    /// <summary>
    /// 获取目标类型。
    /// </summary>
    public Type TargetType { get; private set; }
    /// <summary>
    /// 获取或设置家族名称。
    /// </summary>
    /// <remarks>同一个家族表示为同系列，默认为程序集名称，不区分大小写。</remarks>
    public string FamilyName { get; private set; }

    /// <summary>
    /// 创建实例。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="targetType">目标类型。</param>
    public TypeImplementMapAttribute(Type baseType, Type targetType)
        : this(baseType, targetType, null) {

    }
    /// <summary>
    /// 创建实例。
    /// </summary>
    /// <param name="baseType">基础类型。</param>
    /// <param name="targetType">目标类型。</param>
    /// <param name="familyName">家族名称，同一个家族表示为同系列，默认为程序集名称。</param>
    public TypeImplementMapAttribute(Type baseType, Type targetType, string familyName) {
        Throw.CheckArgumentNull(baseType, nameof(baseType));
        Throw.CheckArgumentNull(targetType, nameof(targetType));
        if (!TypeExtensions.IsInheritFrom(targetType, baseType)) {
            Throw.InvalidCast(string.Format("“{0}”未继承自“{1}”。", targetType.FullName, baseType.FullName));
        }
        BaseType = baseType;
        TargetType = targetType;
        if (string.IsNullOrEmpty(familyName)) {
            FamilyName = targetType.Assembly.FullName;
        } else {
            FamilyName = familyName;
        }
    }
}