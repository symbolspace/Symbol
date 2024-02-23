/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 特性：忽略。
    /// </summary>
    /// <remarks>标记在序列化时忽略某成员的特性。</remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreAttribute : Attribute {
    }

}