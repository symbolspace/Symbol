/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Collections.Generic;
using System.Text;
using Symbol;

namespace System;

/// <summary>
/// 字符串扩展类。
/// </summary>
public static class StringExtensions {

    #region fields
    private static readonly System.Globalization.CompareInfo _invariantCompareInfo;
    #endregion

    #region cctor
    static StringExtensions() {
        _invariantCompareInfo = System.Globalization.CultureInfo.InvariantCulture.CompareInfo;
    }
    #endregion

    #region methods

    #region IsNullOrEmpty
    /// <summary>
    /// 是否为空或空字符串。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <returns>返回判断结果。</returns>
    public static bool IsNullOrEmpty(
#if !net20
        this 
#endif
        string value) {
        return string.IsNullOrEmpty(value);
    }
    #endregion

    #region IsWhiteSpace
    /// <summary>
    /// 是否全为空白字符。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <returns>返回判断结果。</returns>
    public static bool IsWhiteSpace(
#if !net20
        this 
#endif
        string value) {
        if (string.IsNullOrEmpty(value))
            return false;
        foreach (char c in value) {
            if (!char.IsWhiteSpace(c))
                return false;
        }
        return true;
    }
    #endregion
    #region Left
    /// <summary>
    /// 截取文本左侧。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="length">长度，长度不够时，只返回可用长度。</param>
    /// <returns>不会有null。</returns>
    public static string Left(
#if !net20
        this 
#endif
        string value, int length) {
        if (length<1 || string.IsNullOrEmpty(value))
            return "";
        if (value.Length < length) {
            length = value.Length;
        }
        return value.Substring(0, length);
    }
    #endregion
    #region Right
    /// <summary>
    /// 截取文本右侧。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="length">长度，长度不够时，只返回可用长度。</param>
    /// <returns>不会有null。</returns>
    public static string Right(
#if !net20
        this 
#endif
        string value, int length) {
        if (length<1 || string.IsNullOrEmpty(value))
            return "";
        int index;
        if (value.Length > length) {
            index = value.Length - length;
        } else {
            index = 0;
            length = value.Length;
        }
        return value.Substring(index, length);
    }
    #endregion
    #region Middle
    /// <summary>
    /// 截取文本中间。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="index">索引，从0开始。</param>
    /// <param name="length">长度，长度不够时，只返回可用长度。</param>
    /// <returns>不会有null。</returns>
    public static string Middle(
#if !net20
        this 
#endif
        string value, int index, int length) {
        if (       index < 0 
                || length < 1 
                || string.IsNullOrEmpty(value) 
                || index > value.Length - 1 
            )
            return "";
        if ((index + length) > value.Length) {
            length = value.Length - index;
        }
        return value.Substring(index, length);
    }
    #endregion

    #region Format
    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串。</param>
    /// <param name="args">包含零个或多个要格式化的对象的 System.Object 数组。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    /// <exception cref="System.ArgumentNullException">format 或 args 为 null。</exception>
    /// <exception cref="System.FormatException">format 无效。 - 或 - 用于指示要格式化的参数的数字小于零，或者大于等于 args 数组的长度。</exception>
    public static string Format(
#if !net20
        this 
#endif
        string format, params object[] args) {
        return string.Format(format, args);
    }
    /// <summary>
    /// 将指定的 System.String 中的格式项替换为指定的 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串。</param>
    /// <param name="arg0">要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中的第一个格式项已替换为 arg0 的 System.String 等效项。</returns>
    /// <exception cref="System.ArgumentNullException">format 为 null。</exception>
    /// <exception cref="System.FormatException">format 中的格式项无效。 - 或 - 用来表示要格式化的参数的数字小于零，或者大于等于要格式化的指定对象的数目。</exception>
    public static string Format(
#if !net20
        this 
#endif
        string format, object arg0) {
        return string.Format(format, arg0);
    }
    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。指定的参数提供区域性特定的格式设置信息。
    /// </summary>
    /// <param name="format">复合格式字符串。</param>
    /// <param name="provider">一个 System.IFormatProvider，它提供区域性特定的格式设置信息。</param>
    /// <param name="args">包含零个或多个要格式化的对象的 System.Object 数组。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    /// <exception cref="System.ArgumentNullException">format 或 args 为 null。</exception>
    /// <exception cref="System.FormatException">format 无效。 - 或 - 用于指示要格式化的参数的数字小于零，或者大于等于 args 数组的长度。</exception>
    public static string Format(
#if !net20
        this 
#endif
        string format, IFormatProvider provider, params object[] args) {
        return string.Format(provider, format, args);
    }
    /// <summary>
    /// 将指定的 System.String 中的格式项替换为两个指定的 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串。</param>
    /// <param name="arg0">第一个要格式化的 System.Object。</param>
    /// <param name="arg1">第二个要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中的第一个和第二个格式项已替换为 arg0 和 arg1 的 System.String 等效项。</returns>
    /// <exception cref="System.ArgumentNullException"> format 为 null。</exception>
    /// <exception cref="System.FormatException">format 无效。 - 或 - 用来表示要格式化的参数的数字小于零，或者大于等于要格式化的指定对象的数目。</exception>
    public static string Format(
#if !net20
        this 
#endif
        string format, object arg0, object arg1) {
        return string.Format(format, arg0, arg1);
    }
    /// <summary>
    /// 将指定的 System.String 中的格式项替换为三个指定的 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串。</param>
    /// <param name="arg0">第一个要格式化的 System.Object。</param>
    /// <param name="arg1">第二个要格式化的 System.Object。</param>
    /// <param name="arg2">第三个要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中的第一个、第二个和第三个格式项已替换为 arg0、arg1 和 arg2 的 System.String 等效项。</returns>
    /// <exception cref="System.ArgumentNullException"> format 为 null。</exception>
    /// <exception cref="System.FormatException">format 无效。 - 或 - 用来表示要格式化的参数的数字小于零，或者大于等于要格式化的指定对象的数目。</exception>
    public static string Format(
#if !net20
        this 
#endif
        string format, object arg0, object arg1, object arg2) {
        return string.Format(format, arg0, arg1, arg2);
    }
    #endregion
    #region CheckQuoted
    /// <summary>
    /// 自动清除首尾的引号（完全匹配）。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <returns>返回过滤后的文本结果。</returns>
    public static string CheckQuoted(
#if !net20
        this
#endif
        string value) {
        if (value!=null && value.Length > 2 && value[0] == '"' && value[value.Length - 1] == '"') {
            value = value.Substring(1, value.Length - 2);
        }
        return value;
    }
    #endregion

    #region Join
    /// <summary>
    /// 在指定 System.String 数组的每个元素之间串联指定的分隔符 System.String，从而产生单个串联的字符串。
    /// </summary>
    /// <param name="value">一个 System.String 数组。</param>
    /// <param name="separator">间隔字符。</param>
    /// <returns>System.String，包含与 separator 字符串交错的 value 的元素。</returns>
    /// <exception cref="System.ArgumentNullException">value 为 null。</exception>
    public static string Join(
#if !net20
        this 
#endif
        IEnumerable<string> value, char separator) {
        return Join(value, separator.ToString());
    }
    /// <summary>
    /// 在指定 System.String 数组的每个元素之间串联指定的分隔符 System.String，从而产生单个串联的字符串。
    /// </summary>
    /// <param name="value">一个 System.String 数组。</param>
    /// <param name="separator">间隔字符串。</param>
    /// <returns>System.String，包含与 separator 字符串交错的 value 的元素。</returns>
    /// <exception cref="System.ArgumentNullException">value 为 null。</exception>
    public static string Join(
#if !net20
        this 
#endif
        IEnumerable<string> value, string separator) {
        return Join(value, separator, 0, -1);
    }
    /// <summary>
    /// 在指定 System.String 数组的每个元素之间串联指定的分隔符 System.String，从而产生单个串联的字符串。参数指定要使用的第一个数组元素和元素数。
    /// </summary>
    /// <param name="value">一个 System.String 数组。</param>
    /// <param name="separator">间隔字符串。</param>
    /// <param name="startIndex">要使用的 value 中的第一个数组元素。</param>
    /// <param name="count">要使用的 value 的元素数。</param>
    /// <returns>System.String 对象，由通过 separator 联接的 value 中的字符串组成。如果 count 为零、value 没有元素或者separator 和 value 的全部元素为 System.String.Empty，则为 System.String.Empty。</returns>
    /// <exception cref="System.ArgumentNullException">value 为 null。</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">startIndex 或 count 小于 0。 - 或 - startIndex 加上 count 大于 value 中的元素数。</exception>
    /// <exception cref="System.OutOfMemoryException">内存不足。</exception>
    public static string Join(
#if !net20
        this 
#endif
        IEnumerable<string> value, string separator, int startIndex, int count) {
        if (value == null)
            throw new ArgumentNullException("value");

        StringBuilder builder = new StringBuilder();
        int index = startIndex;
        int length = 0;
        foreach (string item in LinqHelper.Skip(value,startIndex)) {
            if (builder.Length > 0)
                builder.Append(separator);
            builder.Append(item);
            index++;
            length++;
            if (count > 0 && length == count)
                break;
        }
        return builder.ToString();
    }
    #endregion

    #region Replace
    /// <summary>
    /// 将指定的字符替换为新的字符。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="oldChar">旧的字符。</param>
    /// <param name="newChar">新的字符。</param>
    /// <param name="ignoreCase">忽略大小写。</param>
    /// <returns>返回替换后的字符串。</returns>
    public static string Replace(
#if !net20
        this 
#endif
        string value, char oldChar, char newChar, bool ignoreCase) {
        if (!ignoreCase) {
            return value.Replace(oldChar, newChar);
        } else {
            return ReplaceInternal(value, oldChar.ToString(), newChar.ToString(), 1, -1, true);
        }
    }
    /// <summary>
    /// 将指定的文本替换为新的文本。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="oldValue">旧的文本。</param>
    /// <param name="newValue">新的文本。</param>
    /// <param name="ignoreCase">忽略大小写。</param>
    /// <returns>返回替换后的字符串。</returns>
    public static string Replace(
#if !net20
        this 
#endif
        string value, string oldValue, string newValue, bool ignoreCase) {
        if (!ignoreCase) {
            return value.Replace(oldValue, newValue);
        } else {
            return ReplaceInternal(value, oldValue, newValue, 1, -1, ignoreCase);
        }
    }
    private static System.Globalization.CultureInfo GetCultureInfo() {
        return System.Globalization.CultureInfo.CurrentCulture;
    }
    //from vb
    private static string ReplaceInternal(string expression, string find, string replacement,int start, int count, bool ignoreCase) {
        if (count < -1) {
            Throw.ArgumentOutOfRange("count", count, 0, null);
        }
        if (start <= 0) {
            Throw.ArgumentOutOfRange("start", start, 1, null);
        }
        if ((expression == null) || (start > expression.Length)) {
            return null;
        }
        if (start != 1) {
            expression = expression.Substring(start - 1);
        }
        if (((find == null) || (find.Length == 0)) || (count == 0)) {
            return expression;
        }
        if (count == -1) {
            count = expression.Length;
        }

        System.Globalization.CompareOptions ordinal;
        System.Globalization.CompareInfo compareInfo;
        int num5 = 0;
        int length = expression.Length;
        int num2 = find.Length;
        System.Text.StringBuilder builder = new System.Text.StringBuilder(length);
        if (ignoreCase) {
            compareInfo = GetCultureInfo().CompareInfo;
            ordinal = System.Globalization.CompareOptions.IgnoreWidth | System.Globalization.CompareOptions.IgnoreKanaType | System.Globalization.CompareOptions.IgnoreCase;
        } else {
            compareInfo = _invariantCompareInfo;
            ordinal = System.Globalization.CompareOptions.Ordinal;
        }
        while (num5 < length) {
            int num4 = 0;
            if (num4 == count) {
                builder.Append(expression.Substring(num5));
                break;
            }
            int num3 = compareInfo.IndexOf(expression, find, num5, ordinal);
            if (num3 < 0) {
                builder.Append(expression.Substring(num5));
                break;
            }
            builder.Append(expression.Substring(num5, num3 - num5));
            builder.Append(replacement);
            num4++;
            num5 = num3 + num2;
        }
        return builder.ToString();
    }
    #endregion

    #region Append
    /// <summary>
    /// 追加内容（当text有值时才会追加，并且仅当value原本也有值才会追加间隔符。）。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="separator">间隔符。</param>
    /// <param name="text">内容。</param>
    /// <returns>返回追加过后的内容。</returns>
    public static string Append(
#if !net20
        this 
#endif
        string value, char separator, string text) {
        return Append(value, separator.ToString(), text);
    }
    /// <summary>
    /// 追加内容（当text有值时才会追加，并且仅当value原本也有值才会追加间隔符。）。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="separator">间隔符。</param>
    /// <param name="text">内容。</param>
    /// <returns>返回追加过后的内容。</returns>
    public static string Append(
#if !net20
        this 
#endif
        string value, string separator, string text) {
        return Append(value, separator, text, string.Empty);
    }
    /// <summary>
    /// 追加内容（当text有值时才会追加，并且仅当value原本也有值才会追加间隔符。）。
    /// </summary>
    /// <param name="value">当前值。</param>
    /// <param name="separator">间隔符。</param>
    /// <param name="text">内容。</param>
    /// <param name="format">对text进行格式化,这样有助于在text有值时，出现更高级的输出结果。</param>
    /// <returns>返回追加过后的内容。</returns>
    public static string Append(
#if !net20
        this 
#endif
        string value, string separator, string text, string format) {
        if (string.IsNullOrEmpty(text))
            return value;
        if (value == null)
            value = string.Empty;
        if (value.Length > 0 && !string.IsNullOrEmpty(separator))
            value += separator;
        value += string.IsNullOrEmpty(format) ? text : string.Format(format, text);
        return value;
    }
    #endregion

    #region FormatEntity
    /// <summary>
    /// 支持实体取值路径的格式串
    /// </summary>
    /// <param name="format">格式串，处理{}格式串，支持深度路径。</param>
    /// <param name="entity">额外实体，传递数据的对象</param>
    /// <returns></returns>
    public static string FormatEntity(
#if !net20
        this
#endif
        string format, object entity) {

        if (string.IsNullOrEmpty(format) || entity == null)
            return format ?? "";
        foreach (var key in Symbol.Text.StringExtractHelper.RulesStringsStartEnd(format, "{", new string[] { "}" }, 0, true, true, true)) {
            var name = Symbol.Text.StringExtractHelper.StringsStartEnd(key, "{", ":", "}")?.Trim();
            if (string.IsNullOrEmpty(name)) {
                format = format.Replace(key, "");
                continue;
            }
            object p = FastObject.Path(entity, name);
            string v = string.Format(key.Replace(name, "0"), p);
            format = format.Replace(key, v);
        }
        return format;
    }
    #endregion

    #endregion

}