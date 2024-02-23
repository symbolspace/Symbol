using System.Collections;
using System.Collections.Generic;

namespace Symbol.Contants;

/// <summary>
/// 接口：常量容器
/// </summary>
public interface IContantsContainer :
    IEnumerable
{
    /// <summary>
    /// 获取常量名称集合。
    /// </summary>
    IEnumerable<string> Keys { get; }

    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回null。</remarks>
    object this[string name] { get; }

    /// <summary>
    /// 是否包含指定名称的常量。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量是否存在。</returns>
    /// <remarks>常量名称为空或常量不存在，返回false。</remarks>
    bool Contains(string name);
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回null。</remarks>
    object GetValue(string name);
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <typeparam name="T">常量的类型。</typeparam>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回default(T)。</remarks>
    T GetValue<T>(string name);
    /// <summary>
    /// 获取指定名称的常量取值（仅限结构类型）。
    /// </summary>
    /// <typeparam name="T">常量的类型。</typeparam>
    /// <param name="name">常量名称。</param>
    /// <param name="defaultValue">默认值。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回defaultValue。</remarks>
    T GetValue<T>(string name, T defaultValue) where T : struct;
}
