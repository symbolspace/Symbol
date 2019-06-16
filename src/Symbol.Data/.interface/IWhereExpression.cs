/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// Where表达式接口
    /// </summary>
    public interface IWhereExpression : System.IDisposable {

        #region properties
        /// <summary>
        /// 获取方言对象。
        /// </summary>
        IDialect Dialect { get; }

        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        IDataContext DataContext { get; }

        /// <summary>
        /// 获取或设置 添加IDbCommand参数委托，默认追加至Parameters。
        /// </summary>
        AddCommandParameterDelegate AddCommandParameter { get; set; }

        /// <summary>
        /// 获取where命令。
        /// </summary>
        System.Collections.Generic.Dictionary<string, WhereOperators> Items { get; }

        /// <summary>
        /// 获取生成的命令语句（不包含where）。
        /// </summary>
        string CommandText { get; }

        #endregion

        #region Where
        /// <summary>
        /// 清空where命令列表。
        /// </summary>
        /// <returns></returns>
        IWhereExpression Clear();

        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="expressions">表达式。</param>
        /// <returns></returns>
        IWhereExpression Where(WhereOperators @operator, params string[] expressions);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Where(string expression, string op = "and");
        #endregion

        #region WhereIf
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        IWhereExpression WhereIf(string expression, string value, string op = "and", WhereIfValueFilterDelegate<string> valueFilter = null);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        IWhereExpression WhereIf(string expression, string value, WhereOperators @operator, WhereIfValueFilterDelegate<string> valueFilter = null);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null。</param>
        /// <param name="min">最小值，不为空时，忽略小于min的值</param>
        /// <param name="max">最大值，不为空时，忽略大于max的值</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression WhereIf(string expression, decimal? value, decimal? min = null, decimal? max = null, string op = "and");
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        IWhereExpression WhereIf(string expression, object value, string op = "and", WhereIfValueFilterDelegate<object> valueFilter = null);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        IWhereExpression WhereIf(string expression, object value, WhereOperators @operator, WhereIfValueFilterDelegate<object> valueFilter = null);
        #endregion

        #region Where Extensions

        #region And Or
        /// <summary>
        /// And表达式。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IWhereExpression And(WhereExpressionAction action);
        /// <summary>
        /// Or表达式。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IWhereExpression Or(WhereExpressionAction action);
        #endregion

        #region Equals
        /// <summary>
        /// 完全匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        IWhereExpression Equals(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 完全匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <returns></returns>
        IWhereExpression Equals(WhereOperators @operator, string field, object value);
        #endregion

        #region Match
        /// <summary>
        /// 操作符匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <returns></returns>
        IWhereExpression Match(WhereOperators @operator, string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals);
        /// <summary>
        /// 操作符匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <returns></returns>
        IWhereExpression Match(WhereOperators @operator, string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals);

        /// <summary>
        /// 操作符匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Match(string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and");
        /// <summary>
        /// 操作符匹配（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Match(string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and");

        #endregion
        #region Eq NotEq Lt Lte Gt Gte
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Eq(string field, string value, string op = "and");
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Eq(string field, object value, string op = "and");
        /// <summary>
        /// 不等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression NotEq(string field, object value, string op = "and");
        /// <summary>
        /// 小于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Lt(string field, object value, string op = "and");
        /// <summary>
        /// 小于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Lte(string field, object value, string op = "and");
        /// <summary>
        /// 大于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Gt(string field, object value, string op = "and");
        /// <summary>
        /// 大于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Gte(string field, object value, string op = "and");
        #endregion

        #region Like
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        IWhereExpression Like(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Like(string field, string value, string op = "and");
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression Like(string field, string value, bool reverse, string op = "and");


        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        IWhereExpression StartsWith(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression StartsWith(string field, string value, string op = "and");
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression StartsWith(string field, string value, bool reverse, string op = "and");
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        IWhereExpression EndsWith(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression EndsWith(string field, string value, string op = "and");
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression EndsWith(string field, string value, bool reverse, string op = "and");
        #endregion

        #region In NotIn
        /// <summary>
        /// 包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        IWhereExpression In(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values);
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        IWhereExpression In(WhereOperators @operator, string field, System.Collections.IEnumerable values);
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression In(string field, System.Collections.IEnumerable values, string op = "and");
        /// <summary>
        /// 不包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        IWhereExpression NotIn(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values);
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        IWhereExpression NotIn(WhereOperators @operator, string field, System.Collections.IEnumerable values);
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        IWhereExpression NotIn(string field, System.Collections.IEnumerable values, string op = "and");
        #endregion

        #endregion

        #region NoSQL
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <returns></returns>
        IWhereExpression Query(object condition);
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <returns></returns>
        IWhereExpression Query(NoSQL.Condition condition);

        #endregion
    }
    /// <summary>
    /// Where表达式Action委托。
    /// </summary>
    /// <param name="expression">Where表达式对象。</param>
    public delegate void WhereExpressionAction(IWhereExpression expression);

}
