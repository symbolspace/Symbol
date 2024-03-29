﻿/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting {

    /// <summary>
    /// 特性：枚举定义名称扩展。
    /// </summary>
    /// <remarks>标记在序列化时额外输出枚举成员ToString值，比如type成员会额外输出typeString。</remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ExtraEnumStringAttribute : Attribute {

    }

}