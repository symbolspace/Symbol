/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 方言
    /// </summary>
    public abstract class Dialect : IDialect {

        #region fields
        private Collections.Generic.NameValueCollection<string> _keywords;
        #endregion

        #region properties
        /// <summary>
        /// 获取关键字列表。
        /// </summary>
        public Collections.Generic.NameValueCollection<string> Keywords {
            get { return _keywords; }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建Dialect实例。
        /// </summary>
        public Dialect() {
            _keywords = new Collections.Generic.NameValueCollection<string>(System.StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region methods

        #region IsKeyword
        /// <summary>
        /// 是否为关键字
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns></returns>
        public bool IsKeyword(string name) {
            return !string.IsNullOrEmpty(_keywords[name]);
        }
        #endregion

        #region KeywordAs
        /// <summary>
        /// 关键字As
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns></returns>
        public string KeywordAs(string name) {
            string value = _keywords[name];
            return string.IsNullOrEmpty(value) ? name : value;
        }
        #endregion
        #region KeywordAs
        /// <summary>
        /// 替换关键字。
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns></returns>
        public string ReplaceKeyword(string name) {
            if (string.IsNullOrEmpty(name))
                return "";
            foreach (var item in _keywords) {
                name = StringExtensions.Replace(name, item.Key, item.Value, true);
                if (string.IsNullOrEmpty(name))
                    return "";
            }
            return name;
        }
        #endregion

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        public abstract string PreName(string name);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        public virtual string PreName(string pairs, string spliter) {
            if (string.IsNullOrEmpty(pairs))
                return "";
            return PreName(pairs.Split(new string[] { spliter }, System.StringSplitOptions.RemoveEmptyEntries));
        }
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        public virtual string PreName(string[] pairs) {
            if (pairs == null || pairs.Length == 0)
                return "";

            for (int i = 0; i < pairs.Length; i++) {
                pairs[i] = PreName(pairs[i]);
            }
            return string.Join(".", pairs);
        }
        #endregion

        #region Like
        /// <summary>
        /// Like 值过滤器
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="left">允许起始</param>
        /// <param name="right">允许末尾</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        public virtual string LikeValueFilter(string value, bool left, bool right, bool reverse) {
            if (reverse)
                return value;
            if (left)
                value = "%" + value;
            if (right)
                value += "%";
            return value;
        }
        /// <summary>
        /// Like 语法
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="left">允许起始</param>
        /// <param name="right">允许末尾</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        public virtual string LikeGrammar(string field, bool left, bool right, bool reverse) {
            if (reverse) {
                field = PreName(field);
                if (left)
                    field = "'%'+" + field;
                if (right)
                    field += "+'%'";
                return "{0} like " + field;
            } else {
                return PreName(field) + " like {0}";
            }
        }
        #endregion

        #region DateTimeNowGrammar
        /// <summary>
        /// DateTime Now 语法
        /// </summary>
        /// <returns></returns>
        public abstract string DateTimeNowGrammar();
        #endregion

        #region MatchOperatorGrammar
        /// <summary>
        /// 匹配操作符语法
        /// </summary>
        /// <param name="matchOperator">匹配操作符</param>
        /// <returns></returns>
        public virtual string MatchOperatorGrammar(string matchOperator) {
            return matchOperator;
        }
        #endregion

        #region ParameterNameGrammar
        /// <summary>
        /// 参数名称语法。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns></returns>
        public virtual string ParameterNameGrammar(string name) {
            if (string.IsNullOrEmpty(name))
                return "";
            if (name[0] == '@')
                return name;
            return "@" + name;
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// 数据过滤器。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected delegate object ValueFilter(object value);
        #endregion
    }

}
