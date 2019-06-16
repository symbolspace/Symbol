/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Routing {


    /// <summary>
    /// 路由。
    /// </summary>
    public class Route {

        #region fields
        private string _url;
        private System.Text.RegularExpressions.Regex _regex;
        private System.Collections.Generic.Dictionary<string, string> _list_type;
        private string[] _names;
        private string _weightUrl;
        #endregion

        #region properties
        /// <summary>
        /// 获取原始网址。
        /// </summary>
        [Const("原始网址")]
        [Formatting.Alias("url")]
        public string Url { get { return _url; } }
        /// <summary>
        /// 获取权重网址。
        /// </summary>
        [Const("权重网址")]
        [Formatting.Alias("weightUrl")]
        public string WeightUrl { get { return _weightUrl; } }
        /// <summary>
        /// 获取变量数量。
        /// </summary>
        [Const("变量数量")]
        [Formatting.Alias("count")]
        public int Count { get { return _list_type.Count; } }
        /// <summary>
        /// 获取变量名称列表。
        /// </summary>
        [Const("变量名称列表")]
        [Formatting.Alias("names")]
        public string[] Names {
            get { return _names; }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建Route实例。
        /// </summary>
        protected Route() {
        }
        #endregion

        #region methods

        #region Parse
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="url">包含规则url</param>
        /// <returns>任何解析不成功都返回null。</returns>
        /// <remarks>
        ///     {name:type}
        ///         type=num/int/uint/byte/float/bool/string/date/datetime/time/guid
        ///     num：整数、负数、小数;
        ///     int：整数；
        ///     uint：无符号整数；
        ///     byte：0-255；
        ///     float：整数、小数；
        ///     bool：yes/no/true/false/1/0；
        ///     date：yyyy-MM－dd，不能小于1970年；
        ///     datetime：yyyy-MM－dd HH:mm:ss，支持毫秒（在末尾加小数点）；
        ///     time：HH:mm:ss，支持毫秒（在末尾加小数点）；
        ///     guid：GUID，只要是有效的GUID就行；
        ///     string：a-z、A-Z、0-9、减号、小数点、空格、冒号；
        /// </remarks>
        public static Route Parse(string url) {
            if (string.IsNullOrEmpty(url) || url.IndexOf('{') == -1 || url.IndexOf('}') == -1)
                return null;
            string weightUrl = url;
            string pattern = "^" + url + "$";
            var types= new System.Collections.Generic.Dictionary<string, string>();
            foreach (System.Text.RegularExpressions.Match p in System.Text.RegularExpressions.Regex.Matches(url, "\\{([a-zA-Z_]+[a-zA-Z_0-9:]*)\\}")) {
                string name = p.Groups[1].Value;
                string weight = "ZZZZ";
                string ra = "[a-zA-Z0-9_\\-\\.\\s:]+";
                string type = "string";
                if (name.IndexOf(':') > -1) {
                    string[] pair = name.Split(':');
                    name = pair[0];
                    type = pair[1];
                    if (type == "num" || type=="float") {
                        ra = "[\\-]{0,1}[0-9]+[\\.]{0,1}[0-9]*";
                        weight = "0106";
                    } else if (type == "int") {
                        ra = "[\\-]{0,1}[0-9]+";
                        weight = "0105";
                    } else if (type == "uint") {
                        ra = "[0-9]+";
                        weight = "0104";
                    } else if (type == "byte") {
                        ra = "[0-9]{1,3}";
                        weight = "0103";
                    } else if (type == "bool") {
                        ra = "yes|no|true|false|1|0";
                        weight = "0101";
                    } else if (type == "date") {
                        ra = "[0-9]{2,4}-[0-9]{1,2}-[0-9]{1,2}";
                        weight = "0201";
                    } else if (type == "datetime") {
                        ra = "[0-9]{2,4}-[0-9]{1,2}-[0-9]{1,2} [0-9:\\.]{5,18}";
                        weight = "0200";
                    } else if (type == "time") {
                        ra = "[0-9:\\.]{5,22}";
                        weight = "0202";
                    } else if (type == "guid") {
                        ra = "[0-9a-f\\{\\}\\-]{32,38}";
                        weight = "0100";
                    }
                }
                if (!types.TryGetValue(name, out string v)) {
                    string p1 = "(?<" + name + ">" + ra + ")";
                    pattern = pattern.Replace(p.Value, p1);
                    types.Add(name, type);
                }
                url = url.Replace(p.Value, "{" + name + "}");
                weightUrl = weightUrl.Replace(p.Value, weight);
            }
            Route route = new Route() {
                _url = url,
                _regex = new System.Text.RegularExpressions.Regex(pattern),
                _list_type = types,
                _names = LinqHelper.ToArray(types.Keys),
                _weightUrl = weightUrl,
            };
            return route;
        }
        #endregion

        #region IsMatch
        /// <summary>
        /// 是否匹配。
        /// </summary>
        /// <param name="url">需要匹配的url。</param>
        /// <returns></returns>
        public bool IsMatch(string url) {
            if (string.IsNullOrEmpty(url))
                return false;
            return _regex.IsMatch(url);
        }
        #endregion
        #region Match
        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="url">需要匹配的url。</param>
        /// <returns>任何匹配不成功时都返回null。</returns>
        public Symbol.Collections.Generic.NameValueCollection<object> Match(string url) {
            if (string.IsNullOrEmpty(url))
                return null;
            System.Text.RegularExpressions.Match match = _regex.Match(url);
            if (!match.Success) {
                return null;
            }
            Symbol.Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>();
            foreach (var item in _list_type) {
                string value = match.Groups[item.Key]?.Value;
                if (string.IsNullOrEmpty(value))
                    return null;
                if (item.Value == "byte") {
                    byte? v= TypeExtensions.Convert<byte?>(value);
                    if (v == null)
                        return null;
                    values[item.Key] = v.Value;
                    continue;
                } else if (item.Value == "date") {
                    DateTime? v = TypeExtensions.Convert<DateTime?>(value);
                    if (v == null || v.Value.Year < 1970)
                        return null;
                    values[item.Key] = v.Value;
                    continue;
                } else if (item.Value == "datetime") {
                    DateTime? v = TypeExtensions.Convert<DateTime?>(value);
                    if (v == null || v.Value.Year < 1970)
                        return null;
                    values[item.Key] = v.Value;
                    continue;
                } else if (item.Value == "time") {
                    TimeSpan? v = TypeExtensions.Convert<TimeSpan?>(value);
                    if (v == null || v.Value.TotalMilliseconds<0)
                        return null;
                    values[item.Key] = v.Value;
                    continue;
                } else if (item.Value == "guid") {
                    Guid? v = TypeExtensions.Convert<Guid?>(value);
                    if (v == null)
                        return null;
                    values[item.Key] = v.Value;
                    continue;
                }
                values[item.Key] = value;

            }
            return values;
        }
        #endregion
        #region Build
        /// <summary>
        /// 构造Url
        /// </summary>
        /// <param name="values">可空，用于替换的参数，实体类/匿名类/字典/JSON</param>
        /// <returns>仅替换发现的参数。</returns>
        public string Build(object values) {
            if (values == null)
                return _url;

            var vars = Symbol.Collections.Generic.NameValueCollection<object>.As(values);
            string url = _url;
            foreach (var item in vars) {
                url = StringExtensions.Replace(url, "{" + item.Key + "}", item.Value == null ? "" : item.Value.ToString(), true);
            }
            return url;
        }
        #endregion

        #region ToString
        /// <summary>
        /// {Url},{Names}
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0},[{1}]", _url, string.Join(",", _names));
        }
        #endregion

        #endregion
    }

}
