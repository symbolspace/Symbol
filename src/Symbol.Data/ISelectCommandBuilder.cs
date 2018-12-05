/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Data;

namespace Symbol.Data {
    /// <summary>
    /// 查询命令构造器接口
    /// </summary>
    public interface ISelectCommandBuilder :
        System.IDisposable {

        #region properties
        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        IDataContext DataContext { get; }

        /// <summary>
        /// 获取或设置自动结束构造，为true时将在Dispose前自动触发Ended事件。
        /// </summary>
        bool AutoEnd { get; set; }
        /// <summary>
        /// 获取或设置 添加IDbCommand参数委托，默认追加至Parameters。
        /// </summary>
        AddCommandParameterDelegate AddCommandParameter { get; set; }
        /// <summary>
        /// 获取已收集的参数列表。
        /// </summary>
        object[] Parameters { get; }
        /// <summary>
        /// 获取当前表名
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// 获取生成的命令语句。
        /// </summary>
        string CommandText { get; }
        /// <summary>
        /// 获取仅Where部分。
        /// </summary>
        string WhereCommandText { get; }
        /// <summary>
        /// 获取delete命令语句。
        /// </summary>
        string DeleteCommmandText { get; }
        /// <summary>
        /// 获取order by命令语句。
        /// </summary>
        string OrderByCommandText { get; }
        /// <summary>
        /// 获取或设置数据取出条数。
        /// </summary>
        int TakeCount { get; set; }
        /// <summary>
        /// 获取或设置数据跳过条数。
        /// </summary>
        int SkipCount { get; set; }
        /// <summary>
        /// 获取字段列表。
        /// </summary>
        Symbol.Collections.Generic.HashSet<string> Fields { get; }
        /// <summary>
        /// 获取where语句之前的命令。
        /// </summary>
        Symbol.Collections.Generic.HashSet<string> WhereBefores { get; }
        /// <summary>
        /// 获取where命名。
        /// </summary>
        System.Collections.Generic.Dictionary<string, WhereOperators> Wheres { get; }
        /// <summary>
        /// 获取order by语句的命令。
        /// </summary>
        Symbol.Collections.Generic.HashSet<string> OrderBys { get; }

        #endregion

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


        #region Select
        /// <summary>
        /// 生成select 语句。
        /// </summary>
        /// <param name="fields">字段列表。</param>
        /// <returns></returns>
        ISelectCommandBuilder Select(params string[] fields);
        #endregion
        #region Where WhereBefore
        /// <summary>
        /// 生成where语句之前的命令。
        /// </summary>
        /// <param name="befores">命令列表。</param>
        /// <returns></returns>
        ISelectCommandBuilder WhereBefore(params string[] befores);
        /// <summary>
        /// 清空where命令列表。
        /// </summary>
        /// <returns></returns>
        ISelectCommandBuilder WhereClear();
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="expressions">表达式。</param>
        /// <returns></returns>
        ISelectCommandBuilder Where(WhereOperators @operator, params string[] expressions);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Where(string expression, string op = "and");
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
        ISelectCommandBuilder WhereIf(string expression, string value, string op = "and", WhereIfValueFilterDelegate<string> valueFilter = null);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        ISelectCommandBuilder WhereIf(string expression, string value, WhereOperators @operator, WhereIfValueFilterDelegate<string> valueFilter = null);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null。</param>
        /// <param name="min">最小值，不为空时，忽略小于min的值</param>
        /// <param name="max">最大值，不为空时，忽略大于max的值</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder WhereIf(string expression, decimal? value, decimal? min = null, decimal? max = null, string op = "and");
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        ISelectCommandBuilder WhereIf(string expression, object value, string op = "and", WhereIfValueFilterDelegate<object> valueFilter = null);
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        ISelectCommandBuilder WhereIf(string expression, object value, WhereOperators @operator, WhereIfValueFilterDelegate<object> valueFilter = null);
        #endregion

        #region Where Extensions

        #region Equals
        /// <summary>
        /// 完全匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        ISelectCommandBuilder Equals(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 完全匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <returns></returns>
        ISelectCommandBuilder Equals(WhereOperators @operator, string field, object value);
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
        ISelectCommandBuilder Match(WhereOperators @operator, string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals);
        /// <summary>
        /// 操作符匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <returns></returns>
        ISelectCommandBuilder Match(WhereOperators @operator, string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals);

        /// <summary>
        /// 操作符匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Match(string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and");
        /// <summary>
        /// 操作符匹配（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Match(string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and");

        #endregion
        #region Eq NotEq Lt Lte Gt Gte
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Eq(string field, string value, string op = "and");
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Eq(string field, object value, string op = "and");
        /// <summary>
        /// 不等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder NotEq(string field, object value, string op = "and");
        /// <summary>
        /// 小于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Lt(string field, object value, string op = "and");
        /// <summary>
        /// 小于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Lte(string field, object value, string op = "and");
        /// <summary>
        /// 大于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Gt(string field, object value, string op = "and");
        /// <summary>
        /// 大于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Gte(string field, object value, string op = "and");
        #endregion

        #region Like
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        ISelectCommandBuilder Like(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Like(string field, string value, string op = "and");
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder Like(string field, string value, bool reverse, string op = "and");
        

        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        ISelectCommandBuilder StartsWith(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder StartsWith(string field, string value, string op = "and");
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder StartsWith(string field, string value, bool reverse, string op = "and");
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        ISelectCommandBuilder EndsWith(WhereOperators @operator, string field, string value);
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder EndsWith(string field, string value, string op = "and");
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder EndsWith(string field, string value, bool reverse, string op = "and");
        #endregion

        #region In NotIn
        /// <summary>
        /// 包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        ISelectCommandBuilder In(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values);
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        ISelectCommandBuilder In(WhereOperators @operator, string field, System.Collections.IEnumerable values);
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder In(string field, System.Collections.IEnumerable values, string op = "and");
        /// <summary>
        /// 不包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        ISelectCommandBuilder NotIn(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values);
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        ISelectCommandBuilder NotIn(WhereOperators @operator, string field, System.Collections.IEnumerable values);
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        ISelectCommandBuilder NotIn(string field, System.Collections.IEnumerable values, string op = "and");
        #endregion

        #endregion

        #region NoSQL
        /// <summary>
        /// 引用关系（NoSQL）。
        /// </summary>
        /// <param name="refer">引用关系。</param>
        /// <param name="selfSource">主查询（表名，$self 为自动识别）</param>
        /// <param name="filter">过滤器。</param>
        /// <returns></returns>
        ISelectCommandBuilder Refer(object refer, string selfSource = "$self", CommandReferFilterDelegate filter = null);
        /// <summary>
        /// 引用关系（NoSQL）。
        /// </summary>
        /// <param name="refer">引用关系。</param>
        /// <param name="selfSource">主查询（表名，$self 为自动识别）</param>
        /// <param name="filter">过滤器。</param>
        /// <returns></returns>
        ISelectCommandBuilder Refer(NoSQL.Refer refer, string selfSource = "$self", CommandReferFilterDelegate filter = null);
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <param name="filter">过滤器</param>
        /// <returns></returns>
        ISelectCommandBuilder Query(object condition, CommandQueryFilterDelegate filter = null);
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <param name="filter">过滤器</param>
        /// <returns></returns>
        ISelectCommandBuilder Query(NoSQL.Condition condition, CommandQueryFilterDelegate filter = null);

        /// <summary>
        /// 排序（NoSQL）。
        /// </summary>
        /// <param name="sorter">排序对象</param>
        /// <returns></returns>
        ISelectCommandBuilder Sort(object sorter);
        /// <summary>
        /// 排序（NoSQL）。
        /// </summary>
        /// <param name="sorter">排序对象</param>
        /// <returns></returns>
        ISelectCommandBuilder Sort(NoSQL.Sorter sorter);
        #endregion


        #region OrderBy
        /// <summary>
        /// 生成order by命令（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="orderby">排序规则</param>
        /// <returns></returns>
        ISelectCommandBuilder OrderBy(string field, OrderBys orderby);
        /// <summary>
        /// 生成order by命令。
        /// </summary>
        /// <param name="orderBys">命令列表。</param>
        /// <returns></returns>
        ISelectCommandBuilder OrderBy(params string[] orderBys);
        #endregion

        #region Count Min Max Average Sum 

        /// <summary>
        /// 生成求count命令。
        /// </summary>
        /// <returns></returns>
        ISelectCommandBuilder Count();
        /// <summary>
        /// 生成求sum命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        ISelectCommandBuilder Sum(string field);
        /// <summary>
        /// 生成求sum命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        ISelectCommandBuilder Sum(string field, bool clear);
        /// <summary>
        /// 生成求min命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        ISelectCommandBuilder Min(string field);
        /// <summary>
        /// 生成求min命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        ISelectCommandBuilder Min(string field, bool clear);
        /// <summary>
        /// 生成求max命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        ISelectCommandBuilder Max(string field);
        /// <summary>
        /// 生成求max命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        ISelectCommandBuilder Max(string field, bool clear);
        /// <summary>
        /// 生成求avg命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        ISelectCommandBuilder Average(string field);
        /// <summary>
        /// 生成求avg命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        ISelectCommandBuilder Average(string field, bool clear);

        #endregion

        #region Take Skip Paging
        /// <summary>
        /// 生成take语法。
        /// </summary>
        /// <param name="count">取出条数。</param>
        /// <returns></returns>
        ISelectCommandBuilder Take(int count);
        /// <summary>
        /// 生成skip语法。
        /// </summary>
        /// <param name="skip">跳过条数。</param>
        /// <returns></returns>
        ISelectCommandBuilder Skip(int skip);
        /// <summary>
        /// 生成分页语法。
        /// </summary>
        /// <param name="size">每页大小，忽略小于1。</param>
        /// <param name="page">页码，从0开始，忽略小于0。</param>
        /// <returns></returns>
        ISelectCommandBuilder Paging(int size, int page);

        #endregion

        #region End
        /// <summary>
        /// 结束构造（触发Ended事件。）
        /// </summary>
        /// <returns></returns>
        ISelectCommandBuilder End();
        /// <summary>
        /// 已结束，构造已结束时触发，只会触发一次此事件。
        /// </summary>
        event System.EventHandler Ended;
        #endregion


        #region 查询

        #region CreateQuery
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery();
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(Action<IDbCommand> action);
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(Type type);
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(Type type, Action<IDbCommand> action);
        #endregion
        #region CreateQuery`1
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<T> CreateQuery<T>();
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="action">回调，可以用command对象进行操作，这发生在处理@params之前。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<T> CreateQuery<T>(Action<IDbCommand> action);
        #endregion

        /// <summary>
        /// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        /// </summary>
        /// <returns>返回第一条记录。</returns>
        object FirstOrDefault();
        /// <summary>
        /// 将查询快速读取并构造一个List对象。
        /// </summary>
        /// <returns>返回一个List对象。</returns>
        System.Collections.Generic.List<object> ToList();
        /// <summary>
        /// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        /// </summary>
        /// <returns>返回第一条记录。</returns>
        T FirstOrDefault<T>();
        /// <summary>
        /// 将查询快速读取并构造一个List对象。
        /// </summary>
        /// <returns>返回一个List对象。</returns>
        System.Collections.Generic.List<T> ToList<T>();

        #endregion

    }



    /// <summary>
    /// 添加IDbCommand参数委托
    /// </summary>
    /// <param name="value">参数</param>
    /// <returns>返回参数名称</returns>
    public delegate string AddCommandParameterDelegate(object value);
    /// <summary>
    /// WhereIf 值过滤器委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">值</param>
    /// <returns>返回处理后的值</returns>
    public delegate object WhereIfValueFilterDelegate<T>(T value);

    /// <summary>
    /// Command查询过滤器委托
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="condition">规则</param>
    /// <returns>返回是否继续。</returns>
    public delegate bool CommandQueryFilterDelegate(ISelectCommandBuilder builder, NoSQL.Condition condition);
    /// <summary>
    /// Command引用过滤器委托。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="refer">引用关系。</param>
    /// <returns></returns>
    public delegate bool CommandReferFilterDelegate(ISelectCommandBuilder builder, NoSQL.Refer refer);



}
