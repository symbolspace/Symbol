using System.Collections;
using System.Collections.Generic;

namespace Symbol.Contants;

/// <summary>
/// 实现：IDictionary常量容器
/// </summary>
public class DictionaryContantsContainer : IContantsContainer
{
    /// <summary>
    /// 对象集合。
    /// </summary>
    protected readonly IDictionary _list;

    /// <summary>
    /// 创建对象实例。
    /// </summary>
    public DictionaryContantsContainer()
    {
        _list = new Hashtable();
    }

    /// <summary>
    /// 创建对象实例。
    /// </summary>
    /// <param name="list">对象实例。</param>
    public DictionaryContantsContainer(IDictionary list)
    {
        Throw.CheckArgumentNull(list, nameof(list));
        _list = list;
    }

    /// <summary>
    /// 获取常量名称集合。
    /// </summary>
    public IEnumerable<string> Keys
    {
        get
        {
            foreach(object key in _list.Keys)
            {
                yield return ConvertExtensions.Convert<string>(key);
            }
        }
    }

    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回null。</remarks>
    public object this[string name] { get { return GetValue(name); } }

    /// <summary>
    /// 是否包含指定名称的常量。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量是否存在。</returns>
    /// <remarks>常量名称为空或常量不存在，返回false。</remarks>
    public virtual bool Contains(string name)
    {
        if(string.IsNullOrEmpty(name)) 
            return false;
        return _list.Contains(name);
    }
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回null。</remarks>
    public virtual object GetValue(string name)
    {
        if(string.IsNullOrEmpty(name))
            return null;
        return IDictionaryExtensions.GetValue(_list, (object)name);
    }
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <typeparam name="T">常量的类型。</typeparam>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回default(T)。</remarks>
    public T GetValue<T>(string name)
    {
        return ConvertExtensions.Convert<T>(GetValue(name));
    }
    /// <summary>
    /// 获取指定名称的常量取值（仅限结构类型）。
    /// </summary>
    /// <typeparam name="T">常量的类型。</typeparam>
    /// <param name="name">常量名称。</param>
    /// <param name="defaultValue">默认值。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回defaultValue。</remarks>
    public T GetValue<T>(string name, T defaultValue) where T : struct
    {
        return ConvertExtensions.Convert(GetValue(name), defaultValue);
    }

    /// <summary>
    /// 获取枚举器。
    /// </summary>
    /// <returns>返回枚举器对象。</returns>
    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}
