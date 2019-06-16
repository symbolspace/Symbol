/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 方言接口
    /// </summary>
    public interface IDialect {

        #region properties
        /// <summary>
        /// 获取关键字列表。
        /// </summary>
        Collections.Generic.NameValueCollection<string> Keywords { get; }

        #endregion

        #region methods

        /// <summary>
        /// 是否为关键字
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns></returns>
        bool IsKeyword(string name);
        /// <summary>
        /// 关键字As
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns></returns>
        string KeywordAs(string name);
        /// <summary>
        /// 替换关键字。
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns></returns>
        string ReplaceKeyword(string name);

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string name);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string pairs, string spliter);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string[] pairs);
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
        string LikeValueFilter(string value, bool left, bool right, bool reverse);
        /// <summary>
        /// Like 语法
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="left">允许起始</param>
        /// <param name="right">允许末尾</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        string LikeGrammar(string field, bool left, bool right, bool reverse);
        #endregion

        #region DateTimeNowGrammar
        /// <summary>
        /// DateTime Now 语法
        /// </summary>
        /// <returns></returns>
        string DateTimeNowGrammar();
        #endregion

        #region MatchOperatorGrammar
        /// <summary>
        /// 匹配操作符语法
        /// </summary>
        /// <param name="matchOperator">匹配操作符</param>
        /// <returns></returns>
        string MatchOperatorGrammar(string matchOperator);
        #endregion

        /// <summary>
        /// 参数名称语法。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns></returns>
        string ParameterNameGrammar(string name);

        #endregion

    }

}
