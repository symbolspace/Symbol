/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SqlServer 方言
    /// </summary>
    public class SqlServerDialect : Dialect {

        #region methods

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        public override string PreName(string name) {
            name = KeywordAs(name);
            if (name.IndexOfAny(new char[] { '[', ']', '(', ')', '=', ' ', ':', '>', '<' }) > -1)
                return name;
            if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9\\-\\.]+$"))
                return name;
            if (name.IndexOfAny(new char[] { '.' }) > -1)
                return name;
            return '[' + name + ']';
        }
        #endregion

        #region DateTimeNowGrammar
        /// <summary>
        /// DateTime Now 语法
        /// </summary>
        /// <returns></returns>
        public override string DateTimeNowGrammar() {
            return "getDate()";
        }
        #endregion

        #endregion
    }
}

