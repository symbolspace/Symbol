/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Collections.Generic;
using System.Text;

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
    private static string ReplaceInternal(string expression, string find, string replacement,int start, int count, bool ignoreCase) {
        if (count < -1) {
            throw new ArgumentOutOfRangeException("Count");
        }
        if (start <= 0) {
            throw new ArgumentOutOfRangeException("Start");
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

    #region Splits
    /// <summary>
    /// 分割文本。
    /// </summary>
    /// <param name="value">需要分割的文本。</param>
    /// <param name="chars">字符集。</param>
    /// <returns>返回分割后的数组。</returns>
    public static string[] Splits(
#if !net20
        this 
#endif
        string value, params char[] chars) {
        return Splits(value, true, chars);
    }
    /// <summary>
    /// 分割文本。
    /// </summary>
    /// <param name="value">需要分割的文本。</param>
    /// <param name="removeEmptyEntries">分割参数。</param>
    /// <param name="chars">字符集。</param>
    /// <returns>返回分割后的数组。</returns>
    public static string[] Splits(
#if !net20
        this 
#endif
        string value, bool removeEmptyEntries, params char[] chars) {
        if (string.IsNullOrEmpty(value))
            return new string[0];
        
        if (chars == null)
            chars = new char[] { ',', '\r', '\n' };
        return value.Split(chars, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }
    /// <summary>
    /// 分割文本。
    /// </summary>
    /// <param name="value">需要分割的文本。</param>
    /// <param name="spliters">分割符。</param>
    /// <returns>返回分割后的数组。</returns>
    public static string[] Splits(
#if !net20
        this 
#endif
        string value, params string[] spliters) {
        return Splits(value, true, spliters);
    }
    /// <summary>
    /// 分割文本。
    /// </summary>
    /// <param name="value">需要分割的文本。</param>
    /// <param name="removeEmptyEntries">分割参数。</param>
    /// <param name="spliters">分割符。</param>
    /// <returns>返回分割后的数组。</returns>
    public static string[] Splits(
#if !net20
        this 
#endif
        string value, bool removeEmptyEntries, params string[] spliters) {
        if (string.IsNullOrEmpty(value))
            return new string[0];

        if (spliters == null)
            spliters = new string[] { ",", "\r", "\n" };
        return value.Split(spliters, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }
    /// <summary>
    /// 分割文本。
    /// </summary>
    /// <param name="expression">需要分割的文本。</param>
    /// <param name="delimiter">分割符。</param>
    /// <param name="Limit"></param>
    /// <param name="ignoreCase">是否忽略大小写。</param>
    /// <returns>返回分割后的数组。</returns>
    public static string[] Split(
#if !net20
        this 
#endif
        string expression,string delimiter=" ", int Limit=-1,bool ignoreCase=false){
        if (string.IsNullOrEmpty(expression))
            return new string[0];

        string[] strArray;
        try {
            int length;
            if ((expression == null) || (expression.Length == 0)) {
                return new string[] { "" };
            }
            if (Limit == -1) {
                Limit = expression.Length + 1;
            }
            if (delimiter == null) {
                length = 0;
            } else {
                length = delimiter.Length;
            }
            if (length == 0) {
                return new string[] { expression };
            }
            strArray = SplitHelper(expression, delimiter, Limit,ignoreCase ? 1:0);
        } catch (Exception exception) {
            throw exception;
        }
        return strArray;
    }
    private static string[] SplitHelper(string sSrc, string sFind, int cMaxSubStrings, int Compare) {
        System.Globalization.CompareInfo invariantCompareInfo;
        int num2 = 0;
        System.Globalization.CompareOptions ordinal;
        int length;
        int num5 = 0;
        int num6 = 0;
        if (sFind == null) {
            length = 0;
        } else {
            length = sFind.Length;
        }
        if (sSrc == null) {
            num6 = 0;
        } else {
            num6 = sSrc.Length;
        }
        if (length == 0) {
            return new string[] { sSrc };
        }
        if (num6 == 0) {
            return new string[] { sSrc };
        }
        int num = 20;
        if (num > cMaxSubStrings) {
            num = cMaxSubStrings;
        }
        string[] strArray = new string[num + 1];
        if (Compare == 0) {
            ordinal = System.Globalization.CompareOptions.Ordinal;
            invariantCompareInfo = _invariantCompareInfo;
        } else {
            invariantCompareInfo = GetCultureInfo().CompareInfo;
            ordinal = System.Globalization.CompareOptions.IgnoreWidth | System.Globalization.CompareOptions.IgnoreKanaType | System.Globalization.CompareOptions.IgnoreCase;
        }
        while (num5 < num6) {
            string str;
            int num4 = invariantCompareInfo.IndexOf(sSrc, sFind, num5, num6 - num5, ordinal);
            if ((num4 == -1) || ((num2 + 1) == cMaxSubStrings)) {
                str = sSrc.Substring(num5);
                if (str == null) {
                    str = "";
                }
                strArray[num2] = str;
                break;
            }
            str = sSrc.Substring(num5, num4 - num5);
            if (str == null) {
                str = "";
            }
            strArray[num2] = str;
            num5 = num4 + length;
            num2++;
            if (num2 > num) {
                num += 20;
                if (num > cMaxSubStrings) {
                    num = cMaxSubStrings + 1;
                }
                strArray = (string[])CopyArray((Array)strArray, new string[num + 1]);
            }
            strArray[num2] = "";
            if (num2 == cMaxSubStrings) {
                str = sSrc.Substring(num5);
                if (str == null) {
                    str = "";
                }
                strArray[num2] = str;
                break;
            }
        }
        if ((num2 + 1) == strArray.Length) {
            return strArray;
        }
        return (string[])CopyArray((Array)strArray, new string[num2 + 1]);
    }
    static Array CopyArray(Array arySrc, Array aryDest) {
        if (arySrc != null) {
            int length = arySrc.Length;
            if (length == 0) {
                return aryDest;
            }
            if (aryDest.Rank != arySrc.Rank) {
                throw new Exception("Rank !=");
            }
            int num8 = aryDest.Rank - 2;
            for (int i = 0; i <= num8; i++) {
                if (aryDest.GetUpperBound(i) != arySrc.GetUpperBound(i)) {
                    throw new Exception("UpperBound !=");
                }
            }
            if (length > aryDest.Length) {
                length = aryDest.Length;
            }
            if (arySrc.Rank > 1) {
                int rank = arySrc.Rank;
                int num7 = arySrc.GetLength(rank - 1);
                int num6 = aryDest.GetLength(rank - 1);
                if (num6 != 0) {
                    int num5 = Math.Min(num7, num6);
                    int num9 = (arySrc.Length / num7) - 1;
                    for (int j = 0; j <= num9; j++) {
                        Array.Copy(arySrc, j * num7, aryDest, j * num6, num5);
                    }
                }
                return aryDest;
            }
            Array.Copy(arySrc, aryDest, length);
        }
        return aryDest;
    }
    #endregion

    #region MaxLength
    /// <summary>
    /// 截取文本最大长度（超出标志：...）
    /// </summary>
    /// <param name="value">要处理的文本</param>
    /// <param name="maxLength">超出后的标志</param>
    /// <returns>返回处理后的文本。</returns>
    public static string MaxLength(
#if !net20
        this 
#endif
        string value, int maxLength) {
        return MaxLength(value, maxLength, "...");
    }
    /// <summary>
    /// 截取文本最大长度
    /// </summary>
    /// <param name="value">要处理的文本</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="endFlag">超出后的标志</param>
    /// <returns>返回处理后的文本。</returns>
    public static string MaxLength(
#if !net20
        this 
#endif
        string value, int maxLength, string endFlag) {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;
        return value.Substring(0, maxLength) + endFlag;
    }
    #endregion
    #region MaxLength2
    /// <summary>
    /// [双字节]截取文本最大长度（超出标志：...）
    /// </summary>
    /// <param name="value">要处理的文本</param>
    /// <param name="maxLength">超出后的标志</param>
    /// <returns>返回处理后的文本。</returns>
    /// <remarks>中文按两位处理，字母数字按1位处理。</remarks>
    public static string MaxLength2(
#if !net20
        this 
#endif
        string value, int maxLength) {
        return MaxLength2(value, maxLength, "...");
    }
    /// <summary>
    /// [双字节]截取文本最大长度
    /// </summary>
    /// <param name="value">要处理的文本</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="endFlag">超出后的标志</param>
    /// <returns>返回处理后的文本。</returns>
    /// <remarks>中文按两位处理，字母数字按1位处理。</remarks>
    public static string MaxLength2(
#if !net20
        this 
#endif
        string value, int maxLength, string endFlag) {
        if (string.IsNullOrEmpty(value))
            return value;

        byte[] data = HttpUtility.GBK.GetBytes(value);
        //int currentLength = data.Length;
        //if (currentLength <= length || length == 0) {
        //    return title;
        //}
        maxLength -= 2;
        List<byte> list = new List<byte>();
        int current = 0;
        for (int i = 0; i < data.Length; i++) {
            list.Add(data[i]);
            if (data[i] >= 160) {//有中文等双码字符
                list.Add(data[i + 1]);
                i++;
                current += 2;
            } else {
                current++;
            }
            if (current >= maxLength) {
                break;
            }
        }
        string result = HttpUtility.GBK.GetString(list.ToArray());
        if (current > maxLength) {
            result += endFlag;
        }
        return result;
    }
    #endregion

    #endregion

}