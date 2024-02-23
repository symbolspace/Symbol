/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol;

/// <summary>
/// 提供在处理 Web 请求时用于编码和解码 URL 的方法。 此类不能被继承。
/// </summary>
public sealed class HttpUtility {

    #region fields
    private static readonly char[] _entityEndingChars;
    private static readonly System.Text.Encoding _gb2312;
    private static readonly System.Text.Encoding _gbk;
    private static readonly System.DateTime _jsMinDate;
    private static readonly long _jsMinDateLong;
    private static readonly string _encodeCasePattern2;
    private static readonly string _encodeCasePattern5;

    #endregion

    #region properties
    /// <summary>
    /// 获取GB2312编码（如果系统不支持将返回null）。
    /// </summary>
    public static System.Text.Encoding GB2312 {
        get { return _gb2312; }
    }
    /// <summary>
    /// 获取GBK编码（如果系统不支持将返回null）。
    /// </summary>
    public static System.Text.Encoding GBK {
        get { return _gbk; }
    }
    #endregion

    #region cctor
    static HttpUtility() {
        _entityEndingChars = new char[] { ';', '&' };
        _jsMinDate = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
        _jsMinDateLong = _jsMinDate.Ticks;
        _encodeCasePattern2 = @"(?<text>%[%a-fA-F0-9]{2})";
        _encodeCasePattern5 = @"(?<text>%u[a-fA-F0-9]{4})";

        try { _gb2312 = System.Text.Encoding.GetEncoding("gb2312"); } catch { }
        try { _gbk = System.Text.Encoding.GetEncoding("gbk"); } catch { }
    }
    #endregion

    #region methods

    #region JsTick
    /// <summary>
    /// 获取UTC时间戳（  Javascript：new Date.getTime() ）。
    /// </summary>
    /// <returns>返回UTC时间戳。</returns>
    public static long JsTick() {
        return JsTick(System.DateTime.Now, true);
    }
    /// <summary>
    /// 获取UTC时间戳（  Javascript：new Date.getTime() ）。
    /// </summary>
    /// <param name="millisceconds">是否包含毫秒</param>
    /// <returns>返回UTC时间戳。</returns>
    public static long JsTick(bool millisceconds) {
        return JsTick(System.DateTime.Now, millisceconds);
    }
    /// <summary>
    /// 获取UTC时间戳。
    /// </summary>
    /// <param name="time">指定日期时间</param>
    /// <param name="millisceconds">是否包含毫秒</param>
    /// <returns>返回UTC时间戳。</returns>
    public static long JsTick(System.DateTime time, bool millisceconds = true) {
        long result = (time.Ticks - _jsMinDateLong) / 10000L;
        if (!millisceconds)
            result /= 1000L;
        return result;
    }
    #endregion
    #region FromJsTick
    /// <summary>
    /// 将UTC时间戳转换为DateTime。
    /// </summary>
    /// <param name="tick">时间戳</param>
    /// <param name="millisceconds">是否包含毫秒</param>
    /// <returns>返回DateTime。</returns>
    public static System.DateTime FromJsTick(long tick, bool millisceconds = true) {
        System.TimeSpan time = new System.TimeSpan(millisceconds ? (tick * 10000L) : (tick * 10000000L));
        return _jsMinDate.Add(time);
    }
    #endregion

    #region FromBase64String
    /// <summary>
    /// 还原base64字符串
    /// </summary>
    /// <param name="text">base64字符串</param>
    /// <returns>返回还原后的字符串。</returns>
    /// <remarks>如果传入的值为空或空字符串，将自动返回string.Empty。</remarks>
    public static string FromBase64String(string text) {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(text));
    }
    #endregion
    #region ToBase64String
    /// <summary>
    /// 转换为base64字符串
    /// </summary>
    /// <param name="text">需要转换的文本</param>
    /// <returns>返回转换后的base64字符串。</returns>
    /// <remarks>如果传入的值为空或空字符串，将自动返回string.Empty。</remarks>
    public static string ToBase64String(string text) {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
    }
    #endregion

    #region AppendQuery
    /// <summary>
    /// 为一个Url追加参数
    /// </summary>
    /// <param name="url">被追加的Url</param>
    /// <param name="name">参数名称，为空时自动忽略</param>
    /// <param name="value">参数值</param>
    /// <param name="needEncode">是否需要UrlEncode处理</param>
    /// <param name="encoding">字符编码，默认是utf-8</param>
    public static void AppendQuery(ref string url, string name, string value, bool needEncode = true, string encoding = null) {
        AppendQuery(ref url, name, value, needEncode, needEncode ? (string.IsNullOrEmpty(encoding) ? null : System.Text.Encoding.GetEncoding(encoding)) : null);
    }
    /// <summary>
    /// 为一个Url追加参数
    /// </summary>
    /// <param name="url">被追加的Url</param>
    /// <param name="name">参数名称，为空时自动忽略</param>
    /// <param name="value">参数值</param>
    /// <param name="needEncode">是否需要UrlEncode处理</param>
    /// <param name="encoding">字符编码，默认是utf-8</param>
    public static void AppendQuery(ref string url, string name, string value, bool needEncode = true, System.Text.Encoding encoding = null) {
        if (string.IsNullOrEmpty(name))
            return;
        if (url.Length > 0) {
            url += url.IndexOf('?') == -1 ? '?' : '&';
        }
        url += name + '=';
        if (string.IsNullOrEmpty(value))
            return;
        if (needEncode) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            url += EncodeCase(UrlEncode(value, encoding), true);
        } else {
            url += value;
        }
    }
    #endregion
    #region GetQueryValue
    /// <summary>
    /// 获取查询中的参数（不会解码）。
    /// </summary>
    /// <param name="pathAndQuery">查询参数（问号后面的内容）或表单数据，为空时返回string.Empty</param>
    /// <param name="name">参数名称，为空时返回string.Empty。</param>
    /// <returns>返回匹配的值。</returns>
    public static string GetQueryValue(string pathAndQuery, string name) {
        return GetQueryValue(pathAndQuery, name, false);
    }
    /// <summary>
    /// 获取查询中的参数。
    /// </summary>
    /// <param name="pathAndQuery">查询参数（问号后面的内容）或表单数据，为空时返回string.Empty</param>
    /// <param name="name">参数名称，为空时返回string.Empty。</param>
    /// <param name="allowUrlDecode">允许解码，默认不解码，UTF8。</param>
    /// <returns>返回匹配的值。</returns>
    public static string GetQueryValue(string pathAndQuery, string name, bool allowUrlDecode = false) {
        if (string.IsNullOrEmpty(pathAndQuery) || string.IsNullOrEmpty(name))
            return string.Empty;
        var regex = new System.Text.RegularExpressions.Regex(string.Format("{0}=(?<value>[^&]*)", name), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!regex.IsMatch(pathAndQuery))
            return string.Empty;
        string result = regex.Match(pathAndQuery).Groups["value"].Value;
        if (!string.IsNullOrEmpty(result) && allowUrlDecode)
            result = UrlDecode(result);
        return result;
    }
    #endregion
    #region GetQueryValues
    /// <summary>
    /// 获取查询中的参数（多个参数，不会解码）。
    /// </summary>
    /// <param name="pathAndQuery">查询参数（问号后面的内容）或表单数据，为空时返回new string[0]</param>
    /// <param name="name">参数名称，为空时返回new string[0]。</param>
    /// <returns>返回匹配的值列表。</returns>
    public static string[] GetQueryValues(string pathAndQuery, string name) {
        return GetQueryValues(pathAndQuery, name, false);
    }
    /// <summary>
    /// 获取查询中的参数（多个参数）。
    /// </summary>
    /// <param name="pathAndQuery">查询参数（问号后面的内容）或表单数据，为空时返回new string[0]</param>
    /// <param name="name">参数名称，为空时返回new string[0]。</param>
    /// <param name="allowUrlDecode">允许解码，默认不解码，UTF8。</param>
    /// <returns>返回匹配的值列表。</returns>
    public static string[] GetQueryValues(string pathAndQuery, string name, bool allowUrlDecode = false) {
        if (string.IsNullOrEmpty(pathAndQuery) || string.IsNullOrEmpty(name))
            return new string[0];
        var regex = new System.Text.RegularExpressions.Regex(string.Format("{0}=(?<value>[^&]*)", name), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!regex.IsMatch(pathAndQuery))
            return new string[0];
        var matchCollection = regex.Matches(pathAndQuery);
        string[] result = new string[matchCollection.Count];
        for (int i = 0; i < matchCollection.Count; i++) {
            var match = matchCollection[i];
            result[i] = match.Groups["value"].Value;
            if (!string.IsNullOrEmpty(result[i]) && allowUrlDecode)
                result[i] = UrlDecode(result[i]);
        }
        return result;
    }
    #endregion
    #region ParseQueryString
    /// <summary>
    /// 解析查询字符串（对值会解码）。
    /// </summary>
    /// <param name="query">查询字符串（URL问号后面的内容），也可以是表单数据</param>
    /// <returns>永远不会为null。</returns>
    public static Symbol.Collections.HttpValueCollection ParseQueryString(string query) {
        return new Symbol.Collections.HttpValueCollection(query);
    }
    /// <summary>
    /// 解析查询字符串（对值会解码）。
    /// </summary>
    /// <param name="query">查询字符串（URL问号后面的内容），也可以是表单数据</param>
    /// <param name="encoding">编码，为空将采用UTF8</param>
    /// <returns>永远不会为null。</returns>
    public static Symbol.Collections.HttpValueCollection ParseQueryString(string query, System.Text.Encoding encoding) {
        return new Symbol.Collections.HttpValueCollection(query, encoding);
    }
    #endregion

    #region EncodeURIComponent
    /// <summary>
    /// URI编码（Javascript：encodeURIComponent）。
    /// </summary>
    /// <param name="text">需要编码的文本。</param>
    /// <returns>返回编码后的文本。</returns>
    public static string EncodeURIComponent(string text) {
        string result = EncodeCase(UrlEncode(text), true);
        if (result != null)
            result = result.Replace("+", "%20");
        return result;
    }
    #endregion
    #region EncodeCase
    /// <summary>
    /// 对已编码的文本大小写处理。
    /// </summary>
    /// <param name="text">已编码的文本，为空时直接返回string.Empty</param>
    /// <param name="isUpper">是否为大写字母（仅作用于%后面的16进制文本，如%CE）</param>
    /// <returns>返回处理后的文本。</returns>
    public static string EncodeCase(string text, bool isUpper) {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        return EncodeCaseInternal(EncodeCaseInternal(text, _encodeCasePattern2, isUpper), _encodeCasePattern5, isUpper);
    }
    private static string EncodeCaseInternal(string text, string pattern, bool isUpper) {
        if (string.IsNullOrEmpty(text))
            return text;
        if (!System.Text.RegularExpressions.Regex.IsMatch(text, pattern)) {
            return text;
        }

        string result = text;
        foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(text, pattern)) {
            string value = match.Groups["text"].Value;
            if (isUpper)
                value = value.ToUpper().Replace("%U", "%u");
            else
                value = value.ToLower();
            result = result.Replace(match.Groups["text"].Value, value);
        }
        return result;
    }
    #endregion

    #region UrlEncode
    /// <summary>
    /// Url值编码（UTF8）。
    /// </summary>
    /// <param name="data">为null或空数组直接返回string.Empty。</param>
    /// <returns>返回编码后的文本。</returns>
    public static string UrlEncode(byte[] data) {
        if (data == null || data.Length == 0) {
            return string.Empty;
        }
        return System.Text.Encoding.ASCII.GetString(UrlEncodeToBytes(data));
    }
    /// <summary>
    /// Url值编码（UTF8）。
    /// </summary>
    /// <param name="text">需要编码的文本，为空直接返回string.Empty。</param>
    /// <returns>返回编码后的文本。</returns>
    public static string UrlEncode(string text) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        return UrlEncode(text, System.Text.Encoding.UTF8);
    }
    /// <summary>
    /// Url值编码。
    /// </summary>
    /// <param name="text">需要编码的文本，为空直接返回string.Empty。</param>
    /// <param name="encoding">为空自动采用UTF8</param>
    /// <returns>返回编码后的文本。</returns>
    public static string UrlEncode(string text, System.Text.Encoding encoding) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        return UrlEncodeSpaces2(System.Text.Encoding.ASCII.GetString(UrlEncodeToBytes(text, encoding ?? System.Text.Encoding.UTF8)));
    }
    /// <summary>
    /// Url值编码。
    /// </summary>
    /// <param name="data">为null或空数组直接返回string.Empty。</param>
    /// <param name="offset">起始位置。</param>
    /// <param name="count">数据长度。</param>
    /// <returns>返回编码后的文本。</returns>
    public static string UrlEncode(byte[] data, int offset, int count) {
        if ((data == null || data.Length == 0) && (count == 0)) {
            return string.Empty;
        }
        return UrlEncodeSpaces2(System.Text.Encoding.ASCII.GetString(UrlEncodeToBytes(data, offset, count)));
    }
    #endregion
    #region UrlEncodeToBytes
    /// <summary>
    /// Url值编码（UTF8）。
    /// </summary>
    /// <param name="text">需要编码的文本，为空直接返回new byte[0]。</param>
    /// <returns>返回编码后的数据。</returns>
    public static byte[] UrlEncodeToBytes(string text) {
        if (string.IsNullOrEmpty(text)) {
            return new byte[0];
        }
        return UrlEncodeToBytes(text, System.Text.Encoding.UTF8);
    }
    /// <summary>
    /// Url值编码。
    /// </summary>
    /// <param name="data">为null或空数组直接返回new byte[0]。</param>
    /// <returns>返回编码后的数据。</returns>
    public static byte[] UrlEncodeToBytes(byte[] data) {
        if (data == null || data.Length == 0) {
            return new byte[0];
        }
        return UrlEncodeToBytes(data, 0, data.Length);
    }
    /// <summary>
    /// Url值编码。
    /// </summary>
    /// <param name="text">需要编码的文本，为空直接返回new byte[0]。</param>
    /// <param name="encoding">为空自动采用UTF8</param>
    /// <returns>返回编码后的数据。</returns>
    public static byte[] UrlEncodeToBytes(string text, System.Text.Encoding encoding) {
        if (string.IsNullOrEmpty(text)) {
            return new byte[0];
        }
        byte[] bytes = (encoding ?? System.Text.Encoding.UTF8).GetBytes(text);
        return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
    }
    /// <summary>
    /// Url值编码。
    /// </summary>
    /// <param name="data">为null或空数组直接返回new byte[0]。</param>
    /// <param name="offset">起始位置。</param>
    /// <param name="count">数据长度。</param>
    /// <returns>返回编码后的数据。</returns>
    public static byte[] UrlEncodeToBytes(byte[] data, int offset, int count) {
        if ((data == null || data.Length == 0) && (count == 0)) {
            return new byte[0];
        }
        if ((offset < 0) || (offset > data.Length - 1)) {
            Throw.ArgumentOutOfRange("offset", offset, 0, data.Length - 1);
        }
        if ((count < 0) || ((offset + count) > data.Length)) {
            Throw.ArgumentOutOfRange("count", offset, 0, data.Length - offset);
        }
        return UrlEncodeBytesToBytesInternal(data, offset, count, true);
    }
    #endregion
    #region UrlEncodeUnicode
    /// <summary>
    /// Url值编码（Unicode）。
    /// </summary>
    /// <param name="text">需要编码的文本，为空直接返回string.Empty。</param>
    /// <returns>返回编码后的文本。</returns>
    public static string UrlEncodeUnicode(string text) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        return UrlEncodeUnicodeStringToStringInternal(text, false);
    }
    #endregion
    #region UrlEncodeUnicodeToBytes
    /// <summary>
    /// Url值编码（Unicode）。
    /// </summary>
    /// <param name="text">需要编码的文本，为空直接返回new byte[0]。</param>
    /// <returns>返回编码后的数据。</returns>
    public static byte[] UrlEncodeUnicodeToBytes(string text) {
        if (string.IsNullOrEmpty(text)) {
            return new byte[0];
        }
        return System.Text.Encoding.ASCII.GetBytes(UrlEncodeUnicode(text));
    }
    #endregion
    #region UrlDecode
    /// <summary>
    /// Url值解码（UTF8）。
    /// </summary>
    /// <param name="text">需要解码的文本，为空直接返回string.Empty。</param>
    /// <returns>返回解码后的文本。</returns>
    public static string UrlDecode(string text) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        return UrlDecode(text, System.Text.Encoding.UTF8);
    }
    /// <summary>
    /// Url值解码。
    /// </summary>
    /// <param name="data">从byte[]中解码，为null或空数组直接返回string.Empty。</param>
    /// <param name="encoding">为空自动采用UTF8</param>
    /// <returns>返回解码后的文本。</returns>
    public static string UrlDecode(byte[] data, System.Text.Encoding encoding) {
        if (data == null || data.Length==0) {
            return string.Empty;
        }
        return UrlDecode(data, 0, data.Length, encoding?? System.Text.Encoding.UTF8);
    }
    /// <summary>
    /// Url值解码。
    /// </summary>
    /// <param name="text">需要解码的文本，为空直接返回string.Empty。</param>
    /// <param name="encoding">为空自动采用UTF8</param>
    /// <returns>返回解码后的文本。</returns>
    public static string UrlDecode(string text, System.Text.Encoding encoding) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        return UrlDecodeStringFromStringInternal(text, encoding ?? System.Text.Encoding.UTF8);
    }
    /// <summary>
    /// Url值解码。
    /// </summary>
    /// <param name="data">从byte[]中解码，为null或空数组直接返回string.Empty。</param>
    /// <param name="offset">起始位置。</param>
    /// <param name="count">数据长度。</param>
    /// <param name="encoding">为空自动采用UTF8</param>
    /// <returns>返回解码后的文本。</returns>
    public static string UrlDecode(byte[] data, int offset, int count, System.Text.Encoding encoding) {
        if ((data == null || data.Length==0) && (count == 0)) {
            return string.Empty;
        }
        if ((offset < 0) || (offset > data.Length-1)) {
            Throw.ArgumentOutOfRange("offset", offset, 0, data.Length - 1);
        }
        if ((count < 0) || ((offset + count) > data.Length)) {
            Throw.ArgumentOutOfRange("count", offset, 0, data.Length -offset);
        }
        return UrlDecodeStringFromBytesInternal(data, offset, count, encoding);
    }

    #endregion
    #region UrlDecodeToBytes
    /// <summary>
    /// Url值解码。
    /// </summary>
    /// <param name="data">从byte[]中解码，为null或空数组直接返回new byte[0]。</param>
    /// <returns>返回解码后的数据。</returns>
    public static byte[] UrlDecodeToBytes(byte[] data) {
        if (data == null || data.Length == 0) {
            return new byte[0];
        }
        return UrlDecodeToBytes(data, 0, (data != null) ? data.Length : 0);
    }
    /// <summary>
    /// Url值解码（UTF8）。
    /// </summary>
    /// <param name="text">需要解码的文本，为空直接返回new byte[0]。</param>
    /// <returns>返回解码后的数据。</returns>
    public static byte[] UrlDecodeToBytes(string text) {
        if (string.IsNullOrEmpty(text)) {
            return new byte[0];
        }
        return UrlDecodeToBytes(text, System.Text.Encoding.UTF8);
    }
    /// <summary>
    /// Url值解码。
    /// </summary>
    /// <param name="text">需要解码的文本，为空直接返回new byte[0]。</param>
    /// <param name="encoding">为空自动采用UTF8</param>
    /// <returns>返回解码后的数据。</returns>
    public static byte[] UrlDecodeToBytes(string text, System.Text.Encoding encoding) {
        if (string.IsNullOrEmpty(text)) {
            return new byte[0];
        }
        return UrlDecodeToBytes((encoding??System.Text.Encoding.UTF8).GetBytes(text));
    }

    /// <summary>
    /// Url值解码。
    /// </summary>
    /// <param name="data">从byte[]中解码，为null或空数组直接返回new byte[0]。</param>
    /// <param name="offset">起始位置。</param>
    /// <param name="count">数据长度。</param>
    /// <returns>返回解码后的数据。</returns>
    public static byte[] UrlDecodeToBytes(byte[] data, int offset, int count) {
        if ((data == null || data.Length == 0) && (count == 0)) {
            return new byte[0];
        }
        if ((offset < 0) || (offset > data.Length - 1)) {
            Throw.ArgumentOutOfRange("offset", offset, 0, data.Length - 1);
        }
        if ((count < 0) || ((offset + count) > data.Length)) {
            Throw.ArgumentOutOfRange("count", offset, 0, data.Length - offset);
        }

        return UrlDecodeBytesFromBytesInternal(data, offset, count);
    }
    #endregion

    #region Escape
    /// <summary>
    /// 转义（%u3F2D）。
    /// </summary>
    /// <param name="text">需要转义的内容，为空时直接返回string.Empty</param>
    /// <returns>返回转义后的文本。</returns>
    public static string Escape(string text) {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        string str2 = "0123456789ABCDEF";
        int length = text.Length;
        System.Text.StringBuilder builder = new System.Text.StringBuilder(length * 2);
        int num3 = -1;
        while (++num3 < length) {
            char ch = text[num3];
            int num2 = ch;
            if ((((0x41 > num2) || (num2 > 90)) && ((0x61 > num2) || (num2 > 0x7a))) && ((0x30 > num2) || (num2 > 0x39))) {
                switch (ch) {
                    case '@':
                    case '*':
                    case '_':
                    case '+':
                    case '-':
                    case '.':
                    case '/':
                        goto Label_0125;
                }
                builder.Append('%');
                if (num2 < 0x100) {
                    builder.Append(str2[num2 / 0x10]);
                    ch = str2[num2 % 0x10];
                } else {
                    builder.Append('u');
                    builder.Append(str2[(num2 >> 12) % 0x10]);
                    builder.Append(str2[(num2 >> 8) % 0x10]);
                    builder.Append(str2[(num2 >> 4) % 0x10]);
                    ch = str2[num2 % 0x10];
                }
            }
            Label_0125:
            builder.Append(ch);
        }
        return builder.ToString();
    }
    #endregion
    #region UnEscape
    /// <summary>
    /// 取消转义（%u3F2D）。
    /// </summary>
    /// <param name="text">已转义的内容（%u3F2D），为空时直接返回string.Empty</param>
    /// <returns>返回处理后的文本。</returns>
    public static string UnEscape(string text) {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        int length = text.Length;
        System.Text.StringBuilder builder = new System.Text.StringBuilder(length);
        int num6 = -1;
        while (++num6 < length) {
            char ch = text[num6];
            if (ch == '%') {
                int num2;
                int num3;
                int num4;
                int num5;
                if (((((num6 + 5) < length) && (text[num6 + 1] == 'u'))
                    && (((num2 = HexToInt(text[num6 + 2])) != -1)
                    && ((num3 = HexToInt(text[num6 + 3])) != -1)))
                    && (((num4 = HexToInt(text[num6 + 4])) != -1)
                    && ((num5 = HexToInt(text[num6 + 5])) != -1))) {
                    ch = (char)((((num2 << 12) + (num3 << 8)) + (num4 << 4)) + num5);
                    num6 += 5;
                } else if ((((num6 + 2) < length) && ((num2 = HexToInt(text[num6 + 1])) != -1)) && ((num3 = HexToInt(text[num6 + 2])) != -1)) {
                    ch = (char)((num2 << 4) + num3);
                    num6 += 2;
                }
            }
            builder.Append(ch);
        }
        return builder.ToString();
    }
    #endregion

    #region HtmlAttributeEncode
    #pragma warning disable 1591

    public static string HtmlAttributeEncode(string text) {
        if (text == null) {
            return null;
        }
        int num = IndexOfHtmlAttributeEncodingChars(text, 0);
        if (num == -1) {
            return text;
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder(text.Length + 5);
        int length = text.Length;
        int startIndex = 0;
        Label_002A:
        if (num > startIndex) {
            builder.Append(text, startIndex, num - startIndex);
        }
        switch (text[num]) {
            case '"':
                builder.Append("&quot;");
                break;

            case '&':
                builder.Append("&amp;");
                break;

            case '<':
                builder.Append("&lt;");
                break;
        }
        startIndex = num + 1;
        if (startIndex < length) {
            num = IndexOfHtmlAttributeEncodingChars(text, startIndex);
            if (num != -1) {
                goto Label_002A;
            }
            builder.Append(text, startIndex, length - startIndex);
        }
        return builder.ToString();
    }
    public static unsafe void HtmlAttributeEncode(string text, System.IO.TextWriter output) {
        if (text != null) {
            int num = IndexOfHtmlAttributeEncodingChars(text, 0);
            if (num == -1) {
                output.Write(text);
            } else {
                int num2 = text.Length - num;
                fixed (char* str = text) {
                    char* chPtr = str;
                    char* chPtr2 = chPtr;
                    while (num-- > 0) {
                        chPtr2++;
                        output.Write(chPtr2[0]);
                    }
                    while (num2-- > 0) {
                        chPtr2++;
                        char ch = chPtr2[0];
                        if (ch > '<') {
                            goto Label_00A2;
                        }
                        char ch2 = ch;
                        if (ch2 != '"') {
                            if (ch2 == '&') {
                                goto Label_008B;
                            }
                            if (ch2 != '<') {
                                goto Label_0098;
                            }
                            output.Write("&lt;");
                        } else {
                            output.Write("&quot;");
                        }
                        continue;
                        Label_008B:
                        output.Write("&amp;");
                        continue;
                        Label_0098:
                        output.Write(ch);
                        continue;
                        Label_00A2:
                        output.Write(ch);
                    }
                }
            }
        }
    }
    #pragma warning restore 1591

    #endregion
    #region HtmlEncode
    /// <summary>
    /// Html编码（处理&lt;&gt;等特殊字符）。
    /// </summary>
    /// <param name="text">需要编码的html内容，为空直接返回string.Empty</param>
    /// <returns>返回编码之后的文本。</returns>
    public static string HtmlEncode(string text) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        int num = IndexOfHtmlEncodingChars(text, 0);
        if (num == -1) {
            return text;
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder(text.Length + 5);
        int length = text.Length;
        int startIndex = 0;
        Label_002A:
        if (num > startIndex) {
            builder.Append(text, startIndex, num - startIndex);
        }
        char ch = text[num];
        if (ch > '>') {
            builder.Append("&#");
            builder.Append(((int)ch).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            builder.Append(';');
        } else {
            char ch2 = ch;
            if (ch2 != '"') {
                switch (ch2) {
                    case '<':
                        builder.Append("&lt;");
                        goto Label_00D5;

                    case '=':
                        goto Label_00D5;

                    case '>':
                        builder.Append("&gt;");
                        goto Label_00D5;

                    case '&':
                        builder.Append("&amp;");
                        goto Label_00D5;
                }
            } else {
                builder.Append("&quot;");
            }
        }
        Label_00D5:
        startIndex = num + 1;
        if (startIndex < length) {
            num = IndexOfHtmlEncodingChars(text, startIndex);
            if (num != -1) {
                goto Label_002A;
            }
            builder.Append(text, startIndex, length - startIndex);
        }
        return builder.ToString();

    }
    /// <summary>
    /// Html编码（处理&lt;&gt;等特殊字符）。
    /// </summary>
    /// <param name="text">需要编码的html内容，为空直接忽略。</param>
    /// <param name="output">输出容器</param>
    public static unsafe void HtmlEncode(string text, System.IO.TextWriter output) {
        if (string.IsNullOrEmpty(text))
            return;
        #region o code
        int num = IndexOfHtmlEncodingChars(text, 0);
        if (num == -1) {
            output.Write(text);
        } else {
            int num2 = text.Length - num;
            fixed (char* str = text) {//((char*)
                char* chPtr = str;
                char* chPtr2 = chPtr;
                while (num-- > 0) {
                    output.Write(chPtr2[0]);
                    chPtr2++;
                }
                while (num2-- > 0) {
                    char ch = chPtr2[0];
                    chPtr2++;
                    if (ch > '>') {
                        goto Label_00C4;
                    }
                    char ch2 = ch;
                    if (ch2 != '"') {
                        switch (ch2) {
                            case '<': {
                                    output.Write("&lt;");
                                    continue;
                                }
                            case '=':
                                goto Label_00BA;

                            case '>': {
                                    output.Write("&gt;");
                                    continue;
                                }
                            case '&':
                                goto Label_00AD;
                        }
                        goto Label_00BA;
                    }
                    output.Write("&quot;");
                    continue;
                    Label_00AD:
                    output.Write("&amp;");
                    continue;
                    Label_00BA:
                    output.Write(ch);
                    continue;
                    Label_00C4:
                    if ((ch >= '\x00a0') && (ch < 'Ā')) {
                        output.Write("&#");
                        output.Write(((int)ch).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                        output.Write(';');
                    } else {
                        output.Write(ch);
                    }
                }
            }
        }
        #endregion

    }
    #endregion
    #region HtmlDecode
    /// <summary>
    /// Html解码（处理&amp;lt;&amp;gt;等特殊字符）。
    /// </summary>
    /// <param name="text">需要解码的html，为空直接返回string.Empty</param>
    /// <returns>返回解码后的内容。</returns>
    public static string HtmlDecode(string text) {
        if (string.IsNullOrEmpty(text)) {
            return string.Empty;
        }
        if (text.IndexOf('&') < 0) {
            return text;
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        System.IO.StringWriter output = new System.IO.StringWriter(builder);
        HtmlDecode(text, output);
        return builder.ToString();
    }
    /// <summary>
    /// Html解码（处理&amp;lt;&amp;gt;等特殊字符）。
    /// </summary>
    /// <param name="text">需要解码的html，为空直接忽略</param>
    /// <param name="output">输出容器</param>
    public static void HtmlDecode(string text, System.IO.TextWriter output) {
        if (string.IsNullOrEmpty(text))
            return;
        if (text.IndexOf('&') < 0) {
            output.Write(text);
        } else {
            int length = text.Length;
            for (int i = 0; i < length; i++) {
                char ch = text[i];
                if (ch == '&') {
                    int num3 = text.IndexOfAny(_entityEndingChars, i + 1);
                    if ((num3 > 0) && (text[num3] == ';')) {
                        string entity = text.Substring(i + 1, (num3 - i) - 1);
                        if ((entity.Length > 1) && (entity[0] == '#')) {
                            try {
                                if ((entity[1] == 'x') || (entity[1] == 'X')) {
                                    ch = (char)int.Parse(entity.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                } else {
                                    ch = (char)int.Parse(entity.Substring(1));
                                }
                                i = num3;
                            } catch (System.FormatException) {
                                i++;
                            } catch (System.ArgumentException) {
                                i++;
                            }
                        } else {
                            i = num3;
                            char ch2 = HtmlEntities.Lookup(entity);
                            if (ch2 != '\0') {
                                ch = ch2;
                            } else {
                                output.Write('&');
                                output.Write(entity);
                                output.Write(';');
                                goto Label_0103;
                            }
                        }
                    }
                }
                output.Write(ch);
                Label_0103:;
            }
        }
    }
    #endregion

    #region UrlPathEncode
    //public static string UrlPathEncode(string str) {
    //    if (str == null) {
    //        return null;
    //    }
    //    int index = str.IndexOf('?');
    //    if (index >= 0) {
    //        return (UrlPathEncode(str.Substring(0, index)) + str.Substring(index));
    //    }
    //    return UrlEncodeSpaces(UrlEncodeNonAscii(str, System.Text.Encoding.UTF8));
    //}
    #endregion

    #endregion

    #region internals

    #region UrlEncodeBytesToBytesInternal
    private static byte[] UrlEncodeBytesToBytesInternal(byte[] data, int offset, int count, bool alwaysCreateReturnValue) {
        int num = 0;
        int num2 = 0;
        for (int i = 0; i < count; i++) {
            char ch = (char)data[offset + i];
            if (ch == ' ') {
                num++;
            } else if (!IsSafe(ch)) {
                num2++;
            }
        }
        if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0)) {
            return data;
        }
        byte[] buffer = new byte[count + (num2 * 2)];
        int num4 = 0;
        for (int j = 0; j < count; j++) {
            byte num6 = data[offset + j];
            char ch2 = (char)num6;
            if (IsSafe(ch2)) {
                buffer[num4++] = num6;
            } else if (ch2 == ' ') {
                buffer[num4++] = 43;
            } else {
                buffer[num4++] = 37;
                buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                buffer[num4++] = (byte)IntToHex(num6 & 15);
            }
        }
        return buffer;
    }
    #endregion
    #region UrlEncodeBytesToBytesInternalNonAscii
    private static byte[] UrlEncodeBytesToBytesInternalNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue) {
        int num = 0;
        for (int i = 0; i < count; i++) {
            if (IsNonAsciiByte(bytes[offset + i])) {
                num++;
            }
        }
        if (!alwaysCreateReturnValue && (num == 0)) {
            return bytes;
        }
        byte[] buffer = new byte[count + (num * 2)];
        int num3 = 0;
        for (int j = 0; j < count; j++) {
            byte b = bytes[offset + j];
            if (IsNonAsciiByte(b)) {
                buffer[num3++] = 37;
                buffer[num3++] = (byte)IntToHex((b >> 4) & 15);
                buffer[num3++] = (byte)IntToHex(b & 15);
            } else {
                buffer[num3++] = b;
            }
        }
        return buffer;
    }
    #endregion
    #region UrlEncodeUnicodeStringToStringInternal
    private static string UrlEncodeUnicodeStringToStringInternal(string s, bool ignoreAscii) {
        int length = s.Length;
        System.Text.StringBuilder builder = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; i++) {
            char ch = s[i];
            if ((ch & 65408) == 0) {
                if (ignoreAscii || IsSafe(ch)) {
                    builder.Append(ch);
                } else if (ch == ' ') {
                    builder.Append("%20");
                } else {
                    builder.Append('%');
                    builder.Append(IntToHex((ch >> 4) & '\x000f'));
                    builder.Append(IntToHex(ch & '\x000f'));
                }
            } else {
                builder.Append("%u");
                builder.Append(IntToHex((ch >> 12) & '\x000f'));
                builder.Append(IntToHex((ch >> 8) & '\x000f'));
                builder.Append(IntToHex((ch >> 4) & '\x000f'));
                builder.Append(IntToHex(ch & '\x000f'));
            }
        }
        return builder.ToString();
    }
    #endregion
    #region UrlEncodeNonAscii
    static string UrlEncodeNonAscii(string str, System.Text.Encoding e) {
        if (string.IsNullOrEmpty(str)) {
            return str;
        }
        if (e == null) {
            e = System.Text.Encoding.UTF8;
        }
        byte[] bytes = e.GetBytes(str);
        bytes = UrlEncodeBytesToBytesInternalNonAscii(bytes, 0, bytes.Length, false);
        return System.Text.Encoding.ASCII.GetString(bytes);
    }
    #endregion
    #region UrlEncodeSpaces
    static string UrlEncodeSpaces2(string str) {
        if ((str != null) && (str.IndexOf('+') >= 0)) {
            str = str.Replace("+", "%20");
        }
        return str;
    }
    static string UrlEncodeSpaces(string str) {
        if ((str != null) && (str.IndexOf(' ') >= 0)) {
            str = str.Replace(" ", "%20");
        }
        return str;
    }
    #endregion

    #region UrlDecodeBytesFromBytesInternal
    private static byte[] UrlDecodeBytesFromBytesInternal(byte[] buf, int offset, int count) {
        int length = 0;
        byte[] sourceArray = new byte[count];
        for (int i = 0; i < count; i++) {
            int index = offset + i;
            byte num4 = buf[index];
            if (num4 == 43) {
                num4 = 32;
            } else if ((num4 == 37) && (i < (count - 2))) {
                int num5 = HexToInt((char)buf[index + 1]);
                int num6 = HexToInt((char)buf[index + 2]);
                if ((num5 >= 0) && (num6 >= 0)) {
                    num4 = (byte)((num5 << 4) | num6);
                    i += 2;
                }
            }
            sourceArray[length++] = num4;
        }
        if (length < sourceArray.Length) {
            byte[] destinationArray = new byte[length];
            System.Array.Copy(sourceArray, destinationArray, length);
            sourceArray = destinationArray;
        }
        return sourceArray;
    }
    #endregion
    #region UrlDecodeStringFromBytesInternal
    private static string UrlDecodeStringFromBytesInternal(byte[] buf, int offset, int count, System.Text.Encoding e) {
        UrlDecoder decoder = new UrlDecoder(count, e);
        for (int i = 0; i < count; i++) {
            int index = offset + i;
            byte b = buf[index];
            if (b == 43) {
                b = 32;
            } else if ((b == 37) && (i < (count - 2))) {
                if ((buf[index + 1] == 117) && (i < (count - 5))) {
                    int num4 = HexToInt((char)buf[index + 2]);
                    int num5 = HexToInt((char)buf[index + 3]);
                    int num6 = HexToInt((char)buf[index + 4]);
                    int num7 = HexToInt((char)buf[index + 5]);
                    if (((num4 < 0) || (num5 < 0)) || ((num6 < 0) || (num7 < 0))) {
                        goto Label_00DA;
                    }
                    char ch = (char)((((num4 << 12) | (num5 << 8)) | (num6 << 4)) | num7);
                    i += 5;
                    decoder.AddChar(ch);
                    continue;
                }
                int num8 = HexToInt((char)buf[index + 1]);
                int num9 = HexToInt((char)buf[index + 2]);
                if ((num8 >= 0) && (num9 >= 0)) {
                    b = (byte)((num8 << 4) | num9);
                    i += 2;
                }
            }
            Label_00DA:
            decoder.AddByte(b);
        }
        return decoder.GetString();
    }
    #endregion
    #region UrlDecodeStringFromStringInternal
    private static string UrlDecodeStringFromStringInternal(string s, System.Text.Encoding e) {
        int length = s.Length;
        UrlDecoder decoder = new UrlDecoder(length, e);
        for (int i = 0; i < length; i++) {
            char ch = s[i];
            if (ch == '+') {
                ch = ' ';
            } else if ((ch == '%') && (i < (length - 2))) {
                if ((s[i + 1] == 'u') && (i < (length - 5))) {
                    int num3 = HexToInt(s[i + 2]);
                    int num4 = HexToInt(s[i + 3]);
                    int num5 = HexToInt(s[i + 4]);
                    int num6 = HexToInt(s[i + 5]);
                    if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0))) {
                        goto Label_0106;
                    }
                    ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                    i += 5;
                    decoder.AddChar(ch);
                    continue;
                }
                int num7 = HexToInt(s[i + 1]);
                int num8 = HexToInt(s[i + 2]);
                if ((num7 >= 0) && (num8 >= 0)) {
                    byte b = (byte)((num7 << 4) | num8);
                    i += 2;
                    decoder.AddByte(b);
                    continue;
                }
            }
            Label_0106:
            if ((ch & 65408) == 0) {
                decoder.AddByte((byte)ch);
            } else {
                decoder.AddChar(ch);
            }
        }
        return decoder.GetString();
    }

    #endregion


    #region IndexOfHtmlEncodingChars
    private static unsafe int IndexOfHtmlEncodingChars(string s, int startPos) {
        //return (int)GetHttpUtility().InvokeMember("IndexOfHtmlEncodingChars",
        //         BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod,
        //         null,
        //         null,
        //         new object[]{
        //             s,
        //             startPos
        //         });
        int num = s.Length - startPos;
        fixed (char* str = s) {
            char* chPtr = str;
            char* chPtr2 = chPtr + startPos;
            while (num > 0) {
                char ch = chPtr2[0];
                if (ch <= '>') {
                    switch (ch) {
                        case '<':
                        case '>':
                        case '"':
                        case '&':
                            return (s.Length - num);

                        case '=':
                            goto Label_007A;
                    }
                } else if ((ch >= '\x00a0') && (ch < 'Ā')) {
                    return (s.Length - num);
                }
                Label_007A:
                chPtr2++;
                num--;
            }
        }
        return -1;
    }
    #endregion
    #region IndexOfHtmlAttributeEncodingChars
    private static unsafe int IndexOfHtmlAttributeEncodingChars(string s, int startPos) {
        int num = s.Length - startPos;
        fixed (char* str = s) {
            char* chPtr = str;
            char* chPtr2 = chPtr + startPos;
            while (num > 0) {
                char ch = chPtr2[0];
                if (ch <= '<') {
                    switch (ch) {
                        case '"':
                        case '&':
                        case '<':
                            return (s.Length - num);
                    }
                }
                chPtr2++;
                num--;
            }
        }
        return -1;
    }
    #endregion
    #region IsNonAsciiByte
    private static bool IsNonAsciiByte(byte b) {
        if (b < 127) {
            return (b < 32);
        }
        return true;
    }
    #endregion

    #region HexToInt
    static int HexToInt(char c) {
        if ((c >= '0') && (c <= '9')) {
            return (c - '0');
        }
        if ((c >= 'A') && (c <= 'F')) {
            return (('\n' + c) - 0x41);
        }
        if ((c >= 'a') && (c <= 'f')) {
            return (('\n' + c) - 0x61);
        }
        return -1;
    }
    #endregion
    #region IntToHex
    static char IntToHex(int n) {
        if (n <= 9) {
            return (char)(n + 48);
        }
        return (char)((n - 10) + 97);
    }
    #endregion
    #region IsSafe
    static bool IsSafe(char ch) {
        if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9'))) {
            return true;
        }
        switch (ch) {
            case '\'':
            case '(':
            case ')':
            case '*':
            case '-':
            case '.':
            case '_':
            case '!':
                return true;
        }
        return false;
    }
    #endregion

    #endregion

    #region types
    private class UrlDecoder {
        // Fields
        private int _bufferSize;
        private byte[] _byteBuffer;
        private char[] _charBuffer;
        private System.Text.Encoding _encoding;
        private int _numBytes;
        private int _numChars;

        // Methods
        internal UrlDecoder(int bufferSize, System.Text.Encoding encoding) {
            this._bufferSize = bufferSize;
            this._encoding = encoding;
            this._charBuffer = new char[bufferSize];
        }

        internal void AddByte(byte b) {
            if (this._byteBuffer == null) {
                this._byteBuffer = new byte[this._bufferSize];
            }
            this._byteBuffer[this._numBytes++] = b;
        }

        internal void AddChar(char ch) {
            if (this._numBytes > 0) {
                this.FlushBytes();
            }
            this._charBuffer[this._numChars++] = ch;
        }

        private void FlushBytes() {
            if (this._numBytes > 0) {
                this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                this._numBytes = 0;
            }
        }

        internal string GetString() {
            if (this._numBytes > 0) {
                this.FlushBytes();
            }
            if (this._numChars > 0) {
                return new string(this._charBuffer, 0, this._numChars);
            }
            return string.Empty;
        }
    }
    class HtmlEntities {

        #region _entitiesList
        private static readonly string[] _entitiesList = new string[] {
                "\"-quot", "&-amp", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf", "\x00ab-laquo",
                "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm", "\x00bb-raquo",
                "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc", "\x00cb-Euml",
                "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute", "\x00db-Ucirc",
                "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc", "\x00eb-euml",
                "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute", "\x00fb-ucirc",
                "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "Œ-OElig", "œ-oelig", "Š-Scaron", "š-scaron", "Ÿ-Yuml", "ƒ-fnof", "ˆ-circ", "˜-tilde", "Α-Alpha", "Β-Beta", "Γ-Gamma", "Δ-Delta",
                "Ε-Epsilon", "Ζ-Zeta", "Η-Eta", "Θ-Theta", "Ι-Iota", "Κ-Kappa", "Λ-Lambda", "Μ-Mu", "Ν-Nu", "Ξ-Xi", "Ο-Omicron", "Π-Pi", "Ρ-Rho", "Σ-Sigma", "Τ-Tau", "Υ-Upsilon",
                "Φ-Phi", "Χ-Chi", "Ψ-Psi", "Ω-Omega", "α-alpha", "β-beta", "γ-gamma", "δ-delta", "ε-epsilon", "ζ-zeta", "η-eta", "θ-theta", "ι-iota", "κ-kappa", "λ-lambda", "μ-mu",
                "ν-nu", "ξ-xi", "ο-omicron", "π-pi", "ρ-rho", "ς-sigmaf", "σ-sigma", "τ-tau", "υ-upsilon", "φ-phi", "χ-chi", "ψ-psi", "ω-omega", "ϑ-thetasym", "ϒ-upsih", "ϖ-piv",
                " -ensp", " -emsp", " -thinsp", "‌-zwnj", "‍-zwj", "‎-lrm", "‏-rlm", "–-ndash", "—-mdash", "‘-lsquo", "’-rsquo", "‚-sbquo", "“-ldquo", "”-rdquo", "„-bdquo", "†-dagger",
                "‡-Dagger", "•-bull", "…-hellip", "‰-permil", "′-prime", "″-Prime", "‹-lsaquo", "›-rsaquo", "‾-oline", "⁄-frasl", "€-euro", "ℑ-image", "℘-weierp", "ℜ-real", "™-trade", "ℵ-alefsym",
                "←-larr", "↑-uarr", "→-rarr", "↓-darr", "↔-harr", "↵-crarr", "⇐-lArr", "⇑-uArr", "⇒-rArr", "⇓-dArr", "⇔-hArr", "∀-forall", "∂-part", "∃-exist", "∅-empty", "∇-nabla",
                "∈-isin", "∉-notin", "∋-ni", "∏-prod", "∑-sum", "−-minus", "∗-lowast", "√-radic", "∝-prop", "∞-infin", "∠-ang", "∧-and", "∨-or", "∩-cap", "∪-cup", "∫-int",
                "∴-there4", "∼-sim", "≅-cong", "≈-asymp", "≠-ne", "≡-equiv", "≤-le", "≥-ge", "⊂-sub", "⊃-sup", "⊄-nsub", "⊆-sube", "⊇-supe", "⊕-oplus", "⊗-otimes", "⊥-perp",
                "⋅-sdot", "⌈-lceil", "⌉-rceil", "⌊-lfloor", "⌋-rfloor", "〈-lang", "〉-rang", "◊-loz", "♠-spades", "♣-clubs", "♥-hearts", "♦-diams"
             };
        #endregion

        private static System.Collections.Hashtable _entitiesLookupTable;
        private static readonly object _lookupLockObject = new object();

        private HtmlEntities() {
        }

        #region Lookup
        internal static char Lookup(string entity) {
            if (_entitiesLookupTable == null) {
                lock (_lookupLockObject) {
                    if (_entitiesLookupTable == null) {
                        var hashtable = new System.Collections.Hashtable();
                        foreach (string str in _entitiesList) {
                            hashtable[str.Substring(2)] = str[0];
                        }
                        _entitiesLookupTable = hashtable;
                    }
                }
            }
            object obj2 = _entitiesLookupTable[entity];
            if (obj2 != null) {
                return (char)obj2;
            }
            return '\0';
        }
        #endregion
    }
    #endregion
    
}
