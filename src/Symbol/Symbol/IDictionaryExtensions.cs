using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Symbol;

/// <summary>
/// 静态扩展：IDictionary。
/// </summary>
public static class IDictionaryExtensions
{
    /// <summary>
    /// 从Dictionary中获取指定key的值。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="key">key</param>
    /// <returns>返回指定key的值，若key为null或不存在时返回default(TValue)。</returns>
    public static object GetValue(
#if !NET20
        this 
#endif
        IDictionary list, object key)
    {
        if (list == null || key == null || !list.Contains(key))
            return null;
        return list[key];
    }
    /// <summary>
    /// 从Dictionary中获取指定key的值。
    /// </summary>
    /// <typeparam name="TKey">Key类型</typeparam>
    /// <typeparam name="TValue">Value类型</typeparam>
    /// <param name="list"></param>
    /// <param name="key">key</param>
    /// <returns>返回指定key的值，若key为null或不存在时返回default(TValue)。</returns>
    public static TValue GetValue<TKey, TValue>(
#if !NET20
        this 
#endif
        IDictionary<TKey, TValue> list, TKey key)
    {
        if (list == null || key == null)
            return default(TValue);

        list.TryGetValue(key, out TValue value);
        return value;
    }
    /// <summary>
    /// 在Dictionary中设置指定key的值。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="key">key</param>
    /// <param name="value">值。</param>
    /// <returns>返回操作是否成功，list或key为空时直接返回为false。</returns>
    public static bool SetValue(
#if !NET20
        this 
#endif
        IDictionary list, object key, object value)
    {
        if(list ==null || key == null)
            return false;

        if (list.Contains(key))
            list[key] = value;
        else
            list.Add(key, value);
        return true;
    }
    /// <summary>
    /// 在Dictionary中设置指定key的值。
    /// </summary>
    /// <typeparam name="TKey">Key类型</typeparam>
    /// <typeparam name="TValue">Value类型</typeparam>
    /// <param name="list"></param>
    /// <param name="key">key</param>
    /// <param name="value">值。</param>
    /// <returns>返回操作是否成功，list或key为空时直接返回为false。</returns>
    public static bool SetValue<TKey, TValue>(
#if !NET20
        this 
#endif
        IDictionary<TKey, TValue> list, TKey key, TValue value)
    {
        if(list ==null || key == null )
            return false;

        if (list.ContainsKey(key))
            list[key] = value;
        else
            list.Add(key, value);
        return true;
    }


    /// <summary>
    /// 设置当前集合中的值（不会清空现有的值。）
    /// </summary>
    /// <param name="list"></param>
    /// <param name="values">支持类型。IDictionary&lt;string, Tgt;、System.Collections.Specialized.NameValueCollection、匿名对象、普通类对象、JSON文本。</param>
    public static void SetValues(
#if !NET20
        this 
#endif
        IDictionary<string, object> list, object values)
    {
        SetValues(list, values, null);
    }

    /// <summary>
    /// 设置当前集合中的值（不会清空现有的值。）
    /// </summary>
    /// <param name="list"></param>
    /// <param name="values">支持类型。IDictionary&lt;string, Tgt;、System.Collections.Specialized.NameValueCollection、匿名对象、普通类对象、JSON文本。</param>
    /// <param name="propertyConvertFunc">属性转换器，可为空。</param>
    /// <param name="nullValue">空值。</param>
    public static void SetValues(
#if !NET20
        this 
#endif
        IDictionary<string, object> list, object values, Func<PropertyDescriptor, object,  object> propertyConvertFunc, object nullValue =null)
    {
    lb_Retry:
        if (values == null)
        {
            return;
        }
        if (values is IDictionary<string, object>)
        {
            foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)values)
            {
                object value = item.Value;
                if (value == null)
                    value = nullValue;
                SetValue(list, item.Key, value);
            }
        }
        else if(values is IDictionary dictionary)
        {
            foreach (object key in dictionary.Keys)
            {
                object value = dictionary[key];
                if (value == null)
                    value = nullValue;
                SetValue(list, ConvertExtensions.Convert<string>(key), value);
            }
        }
        else if (values is System.Collections.Specialized.NameValueCollection)
        {
            System.Collections.Specialized.NameValueCollection collection = (System.Collections.Specialized.NameValueCollection)values;
            foreach (string key in collection.Keys)
            {
                object value = collection[key];
                if (value == null)
                    value = nullValue;
                SetValue(list, key, value);
            }
        }
        else if (values is string)
        {
            string value = (values as string)?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (value[0] == '{')
            {
                values = JSON.Parse(value);
                goto lb_Retry;
            }
        }
        else
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                object value = descriptor.GetValue(values);
                if (propertyConvertFunc != null)
                    value = propertyConvertFunc(descriptor, value);
                if (value == null)
                    value = nullValue;
                SetValue(list, descriptor.Name, value);
            }
        }
    }

    /// <summary>
    /// 尝试从Dictionary中移除指定的key，并且输出存在的值。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="key">key</param>
    /// <param name="value">返回指定key的值，若key为null或不存在时返回default(TValue)。</param>
    /// <returns>返回操作是否成功。</returns>
    public static object TryRemove(
#if !NET20
        this 
#endif
        IDictionary list, object key, out object value)
    {
        if (list == null || key == null || !list.Contains(key))
        {
            value = null;
            return false;
        }
        value = list[key];
        list.Remove(key);
        return true;
    }
    /// <summary>
    /// 尝试从Dictionary中移除指定的key，并且输出存在的值。
    /// </summary>
    /// <typeparam name="TKey">Key类型</typeparam>
    /// <typeparam name="TValue">Value类型</typeparam>
    /// <param name="list"></param>
    /// <param name="key">key</param>
    /// <param name="value">返回指定key的值，若key为null或不存在时返回default(TValue)。</param>
    /// <returns>返回操作是否成功。</returns>
    public static bool TryRemove<TKey, TValue>(
#if !NET20
        this 
#endif
        IDictionary<TKey, TValue> list, TKey key, out TValue value)
    {
        if (list == null || key == null)
        {
            value = default(TValue);
            return false;
        }

        if(list.TryGetValue(key, out value))
        {
            list.Remove(key);
            return true;
        }
        return false;
    }


    /// <summary>
    /// 移除指定的key(批量操作)。
    /// </summary>
    /// <param name="list"></param>
    /// <param name="keys">名称列表,不区分大小写。</param>
    /// <returns>返回移除成功的个数。</returns>
    public static int RemoveKeys(
#if !NET20
        this 
#endif
        IDictionary<string, object> list, params string[] keys)
    {
        if (list == null || keys == null || keys.Length == 0 || list.Count == 0)
            return 0;
        int count = 0;
        for (int i = 0; i < keys.Length; i++)
        {
            if (string.IsNullOrEmpty(keys[i]))
                continue;
            if (list.Remove(keys[i]))
                count++;
        }
        return count;
    }

}


/// <summary>
/// 属性反射处理器。
/// </summary>
/// <typeparam name="T1">类型1</typeparam>
/// <typeparam name="T2">类型2</typeparam>
/// <typeparam name="TResult">返回值类型</typeparam>
/// <param name="arg1">参数1</param>
/// <param name="arg2">参数2</param>
/// <returns>返回值。</returns>
public delegate TResult PropertyDescrtionHander<T1, T2, TResult>(T1 arg1, T2 arg2);