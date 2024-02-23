using Symbol;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 静态扩展：事件
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// 触发实例事件
        /// </summary>
        /// <param name="instance">实例对象，不能为空。</param>
        /// <param name="eventName">事件名称，不能为空。</param>
        /// <param name="args">事件消息参数。</param>
        /// <exception cref="ArgumentNullException">参数instance或eventName为空。</exception>
        public static void RaiseEvent(
#if !NET20
        this 
#endif
        object instance, string eventName, params object[] args)
        {
            Throw.CheckArgumentNull(instance, nameof(instance));
            Throw.CheckArgumentNull(eventName, nameof(eventName));

            var type = instance.GetType();
        lb_Retry:
            FieldInfo fieldInfo = type.GetField(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);
            if (fieldInfo == null)
            {
                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    type = type.BaseType;
                    goto lb_Retry;
                }
                Throw.NotSupported($"“{instance.GetType().FullName}”未定义“{eventName}”事件");
            }
            var eventDelegate = fieldInfo.GetValue(instance) as MulticastDelegate;
            eventDelegate?.DynamicInvoke(args);
        }

    }
}