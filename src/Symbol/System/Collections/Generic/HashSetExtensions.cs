/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using Symbol;

namespace System.Collections.Generic;

/// <summary>
/// HashSet&lt;T&gt;的扩展类
/// </summary>
public static class HashSetExtensions {

    #region methods

    #region ToHashSet
    /// <summary>
    /// 将可遍历枚举转换为HashSet&lt;T&gt;对象。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="source">可遍历枚举。</param>
    /// <returns>返回一个HashSet&lt;T&gt;对象。</returns>
    public static HashSet<T> ToHashSet<T>(
#if !net20
        this 
#endif
        IEnumerable<T> source) {
        return ToHashSet<T>(source, null);
    }
    /// <summary>
    /// 将可遍历枚举转换为HashSet&lt;T&gt;对象。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="source">可遍历枚举。</param>
    /// <param name="predicate">筛选规则。</param>
    /// <returns>返回一个HashSet&lt;T&gt;对象。</returns>
    public static HashSet<T> ToHashSet<T>(
#if !net20
        this 
#endif
        IEnumerable<T> source, Predicate<T> predicate) {
        Throw.CheckArgumentNull(source, "source");

        var result = new HashSet<T>();
        foreach (T item in source) {
            if (predicate == null || predicate(item))
                result.Add(item);
        }
        return result;
    }
    #endregion

    #endregion

}