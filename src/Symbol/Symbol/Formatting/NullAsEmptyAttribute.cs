/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 特性：Null转Empty。
    /// </summary>
    /// <remarks>标记在序列化时将null输出为string.Empty。</remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class NullAsEmptyAttribute : Attribute {

    }

}