/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 方言
    /// </summary>
    public class PostgreSQLDialect : Dialect {

        #region methods

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        public override string PreName(string name) {
            name = KeywordAs(name);
            if (name.IndexOfAny(new char[] { '"', '.', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                return name;
            if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                return name;
            if (name.StartsWith("[") && name.EndsWith("]"))
                name = name.Substring(1, name.Length - 2);
            return '"' + name + '"';
        }
        #endregion

        /// <summary>
        /// Like 值过滤器
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="left">允许起始</param>
        /// <param name="right">允许末尾</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        public override string LikeValueFilter(string value, bool left, bool right, bool reverse) {
            if (reverse)
                return value;
            return string.Format("{0}{1}{2}", left ? "" : "^", value.Replace("^", "\\^").Replace("$", "\\$"), right ? "" : "$");
        }
        /// <summary>
        /// Like 语法
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="left">允许起始</param>
        /// <param name="right">允许末尾</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        public override string LikeGrammar(string field, bool left, bool right, bool reverse) {
            if (reverse) {
                field = PreName(field);
                if (!left)
                    field = "'^'+" + field;
                if (!right)
                    field += "+'$'";
                return "{0} ~* " + field;
            }
            return PreName(field) + " ~* {0}";
        }

        #region DateTimeNowGrammar
        /// <summary>
        /// DateTime Now 语法
        /// </summary>
        /// <returns></returns>
        public override string DateTimeNowGrammar() {
            return "now()";
        }
        #endregion

        #endregion
    }
}

