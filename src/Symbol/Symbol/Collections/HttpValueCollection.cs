/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections.Generic;

namespace Symbol.Collections {
    /// <summary>
    /// Http传输key/value集合。
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class HttpValueCollection : System.Collections.Specialized.NameValueCollection {

        #region fields
        private System.Text.Encoding _encoding;
        #endregion

        #region properties

        /// <summary>
        /// 获取或设置编码（为空时将采用UTF8）。
        /// </summary>
        public System.Text.Encoding Encoding {
            get {
                if (_encoding == null)
                    _encoding = System.Text.Encoding.UTF8;
                return _encoding;
            }
            set { _encoding = value?? System.Text.Encoding.UTF8; }
        }

        #endregion


        #region ctor
        /// <summary>
        /// 创建HttpValueCollection实例（UTF8）。
        /// </summary>
        public HttpValueCollection() {
            Encoding = null;
        }
        /// <summary>
        /// 创建HttpValueCollection实例。
        /// </summary>
        /// <param name="encoding">编码（为空时将采用UTF8）</param>
        public HttpValueCollection(System.Text.Encoding encoding) {
            Encoding = encoding;
        }
        /// <summary>
        /// 创建HttpValueCollection实例（UTF8）。
        /// </summary>
        /// <param name="url">解析url中的查询参数（问号后面的参数），为空不解析</param>
        public HttpValueCollection(System.Uri url):this(url,null) {
        }
        /// <summary>
        /// 创建HttpValueCollection实例。
        /// </summary>
        /// <param name="url">解析url中的查询参数（问号后面的参数），为空不解析</param>
        /// <param name="encoding">编码（为空时将采用UTF8）</param>
        public HttpValueCollection(System.Uri url, System.Text.Encoding encoding) {
            Encoding = encoding;
            if (url != null) {
                ParseValues(url.PathAndQuery);
            }
        }
        /// <summary>
        /// 创建HttpValueCollection实例（UTF8）。
        /// </summary>
        /// <param name="urlOrFormData">网址或表单数据，为空不解析</param>
        public HttpValueCollection(string urlOrFormData) :this(urlOrFormData, null){
        }
        /// <summary>
        /// 创建HttpValueCollection实例。
        /// </summary>
        /// <param name="urlOrFormData">网址或表单数据，为空不解析</param>
        /// <param name="encoding">编码（为空时将采用UTF8）</param>
        public HttpValueCollection(string urlOrFormData, System.Text.Encoding encoding) {
            Encoding = encoding;
            ParseValues(urlOrFormData);
        }
        /// <summary>
        /// 创建HttpValueCollection实例（UTF8）。
        /// </summary>
        /// <param name="collection">包含参数的集合，为空自动忽略。</param>
        public HttpValueCollection(System.Collections.Specialized.NameValueCollection collection) : this(collection, null) {

        }
        /// <summary>
        /// 创建HttpValueCollection实例。
        /// </summary>
        /// <param name="collection">包含参数的集合，为空自动忽略。</param>
        /// <param name="encoding">编码（为空时将采用UTF8）</param>
        public HttpValueCollection(System.Collections.Specialized.NameValueCollection collection, System.Text.Encoding encoding) {
            Encoding = encoding;
            if (collection != null)
                Add(collection);
        }
        #endregion

        #region methods

        #region ParseValues
        /// <summary>
        /// 解析key/value（以&amp;为分割符，并且会解码）。
        /// </summary>
        /// <param name="values">包含key/value的文本</param>
        public void ParseValues(string values) {
            ParseValues(values, "&");
        }
        /// <summary>
        /// 解析key/value（以&amp;为分割符）。
        /// </summary>
        /// <param name="values">包含key/value的文本</param>
        /// <param name="needDecode">是否需要解码</param>
        public void ParseValues(string values, bool needDecode) {
            ParseValues(values, needDecode, "&");
        }
        /// <summary>
        /// 解析key/value（会解码）。
        /// </summary>
        /// <param name="values">包含key/value的文本</param>
        /// <param name="separator">分割符</param>
        public void ParseValues(string values, params string[] separator) {
            ParseValues(values, true, separator);
        }
        /// <summary>
        /// 解析key/value。
        /// </summary>
        /// <param name="values">包含key/value的文本</param>
        /// <param name="needDecode">是否需要解码</param>
        /// <param name="separator">分割符</param>
        public void ParseValues(string values, bool needDecode, params string[] separator) {
            if (separator == null || separator.Length == 0)
                Throw.ArgumentNull("separator");

            if (string.IsNullOrEmpty(values))
                return;
            if (values.StartsWith("?"))
                values = values.Substring(1);
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string item in values.Split(separator, StringSplitOptions.None)) {
                string key = string.Empty;
                string value = string.Empty;
                if (item.IndexOf('=') == -1) {
                    value = item;
                } else {
                    string[] array = item.Split('=');
                    key = array[0];
                    value = array[1];
                }
                if (keys.Add(key)) {
                    this.Remove(key);
                }
                Add(key, needDecode ? HttpUtility.UrlDecode(value, Encoding) : value);
            }
        }
        #endregion
        #region ReplaceValue
        /// <summary>
        /// 替换值。
        /// </summary>
        /// <param name="find">名称，比如abc</param>
        /// <param name="value">替换为</param>
        /// <returns>返回是否有过替换。</returns>
        public bool ReplaceValue(string find, string value) {
            return ReplaceValue(find, value, false);
        }
        /// <summary>
        /// 替换值。
        /// </summary>
        /// <param name="find">名称，不区分大小写。</param>
        /// <param name="value">替换为</param>
        /// <param name="nameEscape">是否对find参数转换，为true时，将变成{abc}</param>
        /// <returns>返回是否有过替换。</returns>
        public bool ReplaceValue(string find, string value, bool nameEscape) {
            if (string.IsNullOrEmpty(find))
                throw new ArgumentNullException("name");
            bool result = false;
            string[] keys = AllKeys;
            if (nameEscape)
                find = ('{' + find + '}');
            for (int i = 0; i < keys.Length; i++) {
                if (string.IsNullOrEmpty(this[keys[i]]))
                    continue;
                this[keys[i]] = StringExtensions.Replace(this[keys[i]], find, value, true);
                if (!result)
                    result = true;
            }
            return result;
        }
        #endregion

        #region ToUri
        /// <summary>
        /// 输出为URI。
        /// </summary>
        /// <param name="url">原始URI，输出时会替换查询字符串（问号后面的内容）。</param>
        /// <returns>返回构造好的URI。</returns>
        public Uri ToUri(string url) {
            return ToUri(new Uri(url));
        }
        /// <summary>
        /// 输出为URI。
        /// </summary>
        /// <param name="url">原始URI，输出时会替换查询字符串（问号后面的内容）。</param>
        /// <returns>返回构造好的URI。</returns>
        public Uri ToUri(Uri url) {
            UriBuilder builder = new UriBuilder(url);
            builder.Query = ToString();
            if (builder.Port == 80)
                builder.Port = -1;
            return builder.Uri;
        }
        #endregion
        #region ToString
        /// <summary>
        /// 输出为文本（key=value&amp;key1=value1）。
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToString("=", "&");
        }
        /// <summary>
        /// 输出为文本。
        /// </summary>
        /// <param name="valueSpliter">key/value分割符。</param>
        /// <param name="nameSpliter">多个分割符。</param>
        /// <returns></returns>
        public string ToString(string valueSpliter, string nameSpliter) {
            Throw.CheckArgumentNull(valueSpliter, "valueSpliter");
            Throw.CheckArgumentNull(nameSpliter, "nameSpliter");

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            foreach (string name in AllKeys) {
                string[] values = GetValues(name);
                if (values == null)
                    continue;
                bool emptyName = string.IsNullOrEmpty(name);
                foreach (string value in values) {
                    if (builder.Length > 0)
                        builder.Append(nameSpliter);
                    if (!emptyName) {
                        builder.Append(name);
                        builder.Append(valueSpliter);
                    }
                    builder.Append(HttpUtility.UrlEncode(value, Encoding));
                }
            }

            return builder.ToString();
        }
        #endregion

        #endregion
    }
}