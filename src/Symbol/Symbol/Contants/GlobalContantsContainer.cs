using Symbol;
using Symbol.Contants;
using System.Collections;
using System.Collections.Generic;

[assembly: TypeImplementMap(typeof(IContantsContainer), typeof(GlobalContantsContainer), "Global")]

namespace Symbol.Contants;

/// <summary>
/// 实现：全局常量容器
/// </summary>
class GlobalContantsContainer : IContantsContainer
{
    private readonly HashSet<IContantsContainer> _list;
#if NET35
    private readonly IDictionary<IDictionary<string, object>, IContantsContainer> _list_dictionary_string_object;
    private readonly IDictionary<IDictionary, IContantsContainer> _list_dictionary;
#else
    private readonly System.Collections.Concurrent.ConcurrentDictionary<IDictionary<string, object>, IContantsContainer> _list_dictionary_string_object;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<IDictionary, IContantsContainer> _list_dictionary;
#endif
    /// <summary>
    /// 创建对象实例。
    /// </summary>
    public GlobalContantsContainer()
    {
        _list = new HashSet<IContantsContainer>();
#if NET35
        _list_dictionary_string_object = new Dictionary<IDictionary<string, object>, IContantsContainer>();
        _list_dictionary = new Dictionary<IDictionary, IContantsContainer>();
#else
        _list_dictionary_string_object = new System.Collections.Concurrent.ConcurrentDictionary<IDictionary<string, object>, IContantsContainer>();
        _list_dictionary = new System.Collections.Concurrent.ConcurrentDictionary<IDictionary, IContantsContainer>();
#endif
    }



    /// <summary>
    /// 获取常量名称集合。
    /// </summary>
    public IEnumerable<string> Keys
    {
        get
        {
            var keys = new HashSet<string>();
            foreach (var list in _list)
            {
                foreach (var key in list.Keys)
                {
                    if (keys.Add(key))
                        yield return key;
                }
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
    public bool Contains(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        foreach (var list in _list)
        {
            if (list.Contains(name))
                return true;
            continue;
        }
        return false;
    }
    /// <summary>
    /// 获取指定名称的常量取值。
    /// </summary>
    /// <param name="name">常量名称。</param>
    /// <returns>返回指定名称的常量取值。</returns>
    /// <remarks>常量名称为空或常量不存在，返回null。</remarks>
    public object GetValue(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        object value = null;
        foreach (var list in _list)
        {
            value = list.GetValue(name);
            if (value != null)
                break;
            continue;
        }
        return value;
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
        return GetAllValues().GetEnumerator();
    }
    IEnumerable GetAllValues()
    {
        foreach (var list in _list)
        {
            foreach (var item in list)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// 添加常量容器。
    /// </summary>
    /// <param name="container">包含常量的对象，为空不抛异常。</param>
    public void Add(IContantsContainer container)
    {
        if (container == null)
            return;
        if (_list.Contains(container))
            return;
        lock (_list)
        {
            _list.Add(container);
        }
    }
    /// <summary>
    /// 添加常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public void Add(IDictionary<string, object> list)
    {
        if (list == null)
            return;
        if (_list_dictionary_string_object.ContainsKey(list))
            return;
        lock (_list)
        {
            if (_list_dictionary_string_object.ContainsKey(list))
                return;
            var container = new DictionaryStringObjectContantsContainer<object>(list);
            _list.Add(container);
            IDictionaryExtensions.SetValue(_list_dictionary_string_object, list, container);
        }
    }
    /// <summary>
    /// 添加常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public void Add(IDictionary list)
    {
        if (list == null)
            return;
        if (_list_dictionary.ContainsKey(list))
            return;
        lock (_list)
        {
            if (_list_dictionary.ContainsKey(list))
                return;
            var container = new DictionaryContantsContainer(list);
            _list.Add(container);
            IDictionaryExtensions.SetValue(_list_dictionary, list, container);
        }
    }
    /// <summary>
    /// 移除常量容器。
    /// </summary>
    /// <param name="container">包含常量的对象，为空不抛异常。</param>
    public void Remove(IContantsContainer container)
    {
        if (container == null)
            return;
        if (!_list.Contains(container))
            return;
        lock (_list)
        {
            _list.Remove(container);
        }
    }
    /// <summary>
    /// 移除常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public void Remove(IDictionary<string, object> list)
    {
        if (list == null)
            return;
        if (!_list_dictionary_string_object.ContainsKey(list))
            return;
        lock (_list)
        {
            if (_list_dictionary_string_object.TryRemove(list, out IContantsContainer container))
            {
                _list.Remove(container);
            }
        }
    }
    /// <summary>
    /// 移除常量容器。
    /// </summary>
    /// <param name="list">包含常量的对象，为空不抛异常。</param>
    public void Remove(IDictionary list)
    {
        if (list == null)
            return;
        if (!_list_dictionary.ContainsKey(list))
            return;
        lock (_list)
        {
            if (_list_dictionary.TryRemove(list, out IContantsContainer container))
            {
                _list.Remove(container);
            }
        }
    }
}