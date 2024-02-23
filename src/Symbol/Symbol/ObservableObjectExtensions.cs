using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Symbol;

/// <summary>
/// 静态扩展：可观察对象
/// </summary>
public static class ObservableObjectExtensions
{


    /// <summary>
    /// 触发事件：<see cref="INotifyPropertyChanging.PropertyChanging"/>
    /// </summary>
    /// <param name="notify">可观察对象，不能为空。</param>
    /// <param name="propertyName">属性名称，不能为空。</param>
    /// <exception cref="ArgumentNullException">参数notify或propertyName为空。</exception>
    public static void RaisePropertyChanging(
#if !NET20
        this 
#endif
        INotifyPropertyChanging notify, string propertyName)
    {
        Throw.CheckArgumentNull(propertyName, nameof(propertyName));

        var e = new PropertyChangingEventArgs(propertyName);
        EventExtensions.RaiseEvent(notify, nameof(INotifyPropertyChanging.PropertyChanging), notify, e );
    }

    /// <summary>c
    /// 触发事件：<see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// </summary>
    /// <param name="notify">可观察对象，不能为空。</param>
    /// <param name="propertyName">属性名称，不能为空。</param>
    /// <exception cref="ArgumentNullException">参数notify或propertyName为空。</exception>
    public static void RaisePropertyChanged(
#if !NET20
        this 
#endif
        INotifyPropertyChanged notify, string propertyName)
    {
        Throw.CheckArgumentNull(propertyName, nameof(propertyName));
        var e = new PropertyChangedEventArgs(propertyName);

        EventExtensions.RaiseEvent(notify, nameof(INotifyPropertyChanged.PropertyChanged), notify, e);
    }

    /// <summary>
    /// 可观察对象：设置属性值。
    /// </summary>
    /// <typeparam name="TNotify">可观察对象类型，必须继承<see cref="INotifyPropertyChanging"/>、<see cref="INotifyPropertyChanged"/>。</typeparam>
    /// <typeparam name="TValue">任意类型</typeparam>
    /// <param name="notify">可观察对象，不能为空。</param>
    /// <param name="fieldValue">当前属性值。</param>
    /// <param name="newValue">新的属性值。</param>
    /// <param name="propertyName">属性名称，不能为空。</param>
    /// <exception cref="ArgumentNullException">参数notify或propertyName为空。</exception>
    /// <returns>返回操作结果，为true表示值有变化。</returns>
    public static bool SetProperty<TNotify, TValue>(
#if !NET20
        this 
#endif
        TNotify notify,
#if NET6_0 || NET7_0 || NETSTANDARD2_1
        [System.Diagnostics.CodeAnalysis.NotNullIfNotNull("newValue")]
#endif
        ref TValue fieldValue,
        TValue newValue,
#if !(NET20 || NET35 || NET40)
        [CallerMemberName]
#endif
        string propertyName = null
        )
        where TNotify : INotifyPropertyChanging, INotifyPropertyChanged
    {
        Throw.CheckArgumentNull(notify, nameof(notify));

        if (string.IsNullOrEmpty(propertyName))
        {
            propertyName = StackTraceExtensions.GetCallMemberName(new StackTrace(), 1);
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (propertyName.StartsWith("get_") || propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4);
                }
            }
        }
        Throw.CheckArgumentNull(propertyName, nameof(propertyName));

        if (EqualityComparer<TValue>.Default.Equals(fieldValue, newValue))
        {
            return false;
        }
        RaisePropertyChanging(notify, propertyName);
        fieldValue = newValue;
        RaisePropertyChanged(notify, propertyName);

        return true;

    }
    /// <summary>
    /// 可观察对象：设置属性值。
    /// </summary>
    /// <typeparam name="TNotify">可观察对象类型，必须继承<see cref="INotifyPropertyChanging"/>、<see cref="INotifyPropertyChanged"/>。</typeparam>
    /// <typeparam name="TValue">任意类型</typeparam>
    /// <param name="notify">可观察对象，不能为空。</param>
    /// <param name="fieldValue">当前属性值。</param>
    /// <param name="newValue">新的属性值。</param>
    /// <param name="comparer">比较器，不能为空</param>
    /// <param name="propertyName">属性名称，不能为空。</param>
    /// <exception cref="ArgumentNullException">参数notify或propertyName或comparer为空。</exception>
    /// <returns>返回操作结果，为true表示值有变化。</returns>
    public static bool SetProperty<TNotify, TValue>(
#if !NET20
        this 
#endif
        TNotify notify,
#if NET6_0 || NET7_0 || NETSTANDARD2_1
        [System.Diagnostics.CodeAnalysis.NotNullIfNotNull("newValue")]
#endif
        ref TValue fieldValue,
        TValue newValue,
        IEqualityComparer<TValue> comparer,
#if !(NET20 || NET35 || NET40)
        [CallerMemberName]
#endif
        string propertyName = null
        )
        where TNotify : INotifyPropertyChanging, INotifyPropertyChanged
    {
        Throw.CheckArgumentNull(notify, nameof(notify));
        Throw.CheckArgumentNull(comparer, nameof(comparer));

        if (comparer.Equals(fieldValue, newValue))
        {
            return false;
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            propertyName = StackTraceExtensions.GetCallMemberName(new StackTrace(), 1);
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (propertyName.StartsWith("get_") || propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4);
                }
            }
        }
        Throw.CheckArgumentNull(propertyName, nameof(propertyName));

        RaisePropertyChanging(notify, propertyName);
        fieldValue = newValue;
        RaisePropertyChanged(notify, propertyName);

        return true;

    }
    /// <summary>
    /// 可观察对象：设置属性值。
    /// </summary>
    /// <typeparam name="TNotify">可观察对象类型，必须继承<see cref="INotifyPropertyChanging"/>、<see cref="INotifyPropertyChanged"/>。</typeparam>
    /// <typeparam name="TValue">任意类型</typeparam>
    /// <param name="notify">可观察对象，不能为空。</param>
    /// <param name="oldValue">当前属性值。</param>
    /// <param name="newValue">新的属性值。</param>
    /// <param name="callback">回调，不能为空</param>
    /// <param name="propertyName">属性名称，不能为空。</param>
    /// <exception cref="ArgumentNullException">参数notify或propertyName或callback为空。</exception>
    /// <returns>返回操作结果，为true表示值有变化。</returns>
    public static bool SetProperty<TNotify, TValue>(
#if !NET20
        this 
#endif
        TNotify notify,
        TValue oldValue,
        TValue newValue,
        Action<TValue> callback,
#if !(NET20 || NET35 || NET40)
        [CallerMemberName]
#endif
        string propertyName = null
        )
        where TNotify : INotifyPropertyChanging, INotifyPropertyChanged
    {
        Throw.CheckArgumentNull(notify, nameof(notify));
        Throw.CheckArgumentNull(callback, nameof(callback));

        if (EqualityComparer<TValue>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            propertyName = StackTraceExtensions.GetCallMemberName(new StackTrace(), 1);
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (propertyName.StartsWith("get_") || propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4);
                }
            }
        }
        Throw.CheckArgumentNull(propertyName, nameof(propertyName));

        RaisePropertyChanging(notify, propertyName);
        callback(newValue);
        RaisePropertyChanged(notify, propertyName);

        return true;

    }
    /// <summary>
    /// 可观察对象：设置属性值。
    /// </summary>
    /// <typeparam name="TNotify">可观察对象类型，必须继承<see cref="INotifyPropertyChanging"/>、<see cref="INotifyPropertyChanged"/>。</typeparam>
    /// <typeparam name="TValue">任意类型</typeparam>
    /// <param name="notify">可观察对象，不能为空。</param>
    /// <param name="oldValue">当前属性值。</param>
    /// <param name="newValue">新的属性值。</param>
    /// <param name="comparer">比较器，不能为空</param>
    /// <param name="callback">回调，不能为空</param>
    /// <param name="propertyName">属性名称，不能为空。</param>
    /// <exception cref="ArgumentNullException">参数notify或propertyName或comparer或callback为空。</exception>
    /// <returns>返回操作结果，为true表示值有变化。</returns>
    public static bool SetProperty<TNotify, TValue>(
#if !NET20
        this 
#endif
        TNotify notify,
        TValue oldValue,
        TValue newValue,
        IEqualityComparer<TValue> comparer,
        Action<TValue> callback,
#if !(NET20 || NET35 || NET40)
        [CallerMemberName]
#endif
        string propertyName = null
        )
        where TNotify : INotifyPropertyChanging, INotifyPropertyChanged
    {
        Throw.CheckArgumentNull(notify, nameof(notify));
        Throw.CheckArgumentNull(comparer, nameof(comparer));
        Throw.CheckArgumentNull(callback, nameof(callback));

        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            propertyName = StackTraceExtensions.GetCallMemberName(new StackTrace(), 1);
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (propertyName.StartsWith("get_") || propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4);
                }
            }
        }
        Throw.CheckArgumentNull(propertyName, nameof(propertyName));

        RaisePropertyChanging(notify, propertyName);
        callback(newValue);
        RaisePropertyChanged(notify, propertyName);

        return true;

    }
}