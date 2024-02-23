using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Symbol.Contants;

/// <summary>
/// 静态：常量容器。
/// </summary>
public static class ContantsContainer
{
    private static readonly GlobalContantsContainer _global;

    static ContantsContainer()
    {
        SanInternal();
        _global = (GlobalContantsContainer)TypeImplementMap.GetTargetSingleton<IContantsContainer>("Global");
        RegisterAll();
    }

    /// <summary>
    /// 扫描程序集。
    /// </summary>
    /// <remarks>匹配规则：*.Contants.*.dll</remarks>
    public static void Scan()
    {
        SanInternal();
        RegisterAll();
    }
    static void SanInternal()
    {
        TypeImplementMap.Scan();
        TypeImplementMap.Scan("*.Contants.*.dll");
    }
    /// <summary>
    /// 扫描程序集。
    /// </summary>
    /// <param name="assembly"></param>
    public static void Scan(Assembly assembly)
    {
        TypeImplementMap.Scan(assembly);
        RegisterAll();
    }

    /// <summary>
    /// 注册全部
    /// </summary>
    /// <remarks>向全局注册实例（内部带有去重），可以多次调用此方法。</remarks>
    public static void RegisterAll()
    {
        foreach (var instance in TypeImplementMap.GetTargetSingletons<IContantsContainer>())
        {
            Add(instance);
        }
    }

    /// <summary>
    /// 获取全局对象。
    /// </summary>
    public static IContantsContainer Global
    {
        get { return _global; }
    }

    /// <summary>
    /// 获取常量名称集合。
    /// </summary>
    public static IEnumerable<string> Keys { get { return _global.Keys; } }

    /// <summary>
    /// 是否包含指定名称的常量。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量是否存在。</returns>
    /// <remarks>常量名称为空或常量不存在，返回false。</remarks>
    public static bool Contains(string name)
    {
        return _global.Contains(name);
    }
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回null。</remarks>
    public static object GetValue(string name)
    {
        return _global.GetValue(name);
    }
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <typeparam name="T">常量的类型。</typeparam>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回default(T)。</remarks>
    public static T GetValue<T>(string name)
    {
        return _global.GetValue<T>(name);
    }
    /// <summary>
    /// 获取指定名称的常量取值（仅限结构类型）。
    /// </summary>
    /// <typeparam name="T">常量的类型。</typeparam>
    /// <param name="name">常量名称。</param>
    /// <param name="defaultValue">默认值。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回defaultValue。</remarks>
    public static T GetValue<T>(string name, T defaultValue) where T : struct
    {
        return _global.GetValue(name, defaultValue);
    }

    /// <summary>
    /// 添加常量容器。
    /// </summary>
    /// <param name="container">包含常量的对象，为空不抛异常。</param>
    public static void Add(IContantsContainer container)
    {
        if (container == _global)
            return;
        _global.Add(container);
    }
    /// <summary>
    /// 添加常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public static void Add(IDictionary<string, object> list)
    {
        _global.Add(list);
    }
    /// <summary>
    /// 添加常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public static void Add(Hashtable list)
    {
        _global.Add(list);
    }

    /// <summary>
    /// 移除常量容器。
    /// </summary>
    /// <param name="container">包含常量的对象，为空不抛异常。</param>
    public static void Remove(IContantsContainer container)
    {
        _global.Remove(container);
    }
    /// <summary>
    /// 移除常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public static void Remove(IDictionary<string, object> list)
    {
        _global.Remove(list);
    }
    /// <summary>
    /// 移除常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public static void Remove(Hashtable list)
    {
        _global.Remove(list);
    }

}
