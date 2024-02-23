/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Symbol;

/// <summary>
/// 为枚举的成员定义一些额外的属性值。
/// </summary>
[System.Diagnostics.DebuggerDisplay("Key = {Key}, Value={Value}")]
[Obsolete("请更改为ConstAttribute")]
[Serializable]
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class EnumPropertyAttribute : ConstAttribute {

    #region ctor
    /// <summary>
    /// 定义一个属性（名称为：Text）
    /// </summary>
    /// <param name="value">属性值。</param>
    public EnumPropertyAttribute(string value)
        : base(value) {
    }
    /// <summary>
    /// 定义一个属性
    /// </summary>
    /// <param name="key">属性名称。</param>
    /// <param name="value">属性值。</param>
    public EnumPropertyAttribute(string key, string value)
        :base(key,value){
    }
    #endregion

}