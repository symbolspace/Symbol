/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 查询命令构造器基类
    /// </summary>
    public abstract class SelectCommandBuilder :
        ISelectCommandBuilder,
        System.IDisposable {

        #region fields
        /// <summary>
        /// 当前表名
        /// </summary>
        protected string _tableName;
        /// <summary>
        /// 字段列表
        /// </summary>
        protected Symbol.Collections.Generic.HashSet<string> _fields;
        /// <summary>
        /// where before列表
        /// </summary>
        protected Symbol.Collections.Generic.HashSet<string> _whereBefores;
        /// <summary>
        /// 排序列表
        /// </summary>
        protected Symbol.Collections.Generic.HashSet<string> _orderbys;
        /// <summary>
        /// 当前数据上下文对象。
        /// </summary>
        protected IDataContext _dataContext;

        private IDialect _dialect;


        private bool _ended = false;

        private System.Collections.Generic.List<object> _parameters;
        private IWhereExpression _whereExpression;
        private static readonly string[] _customTableChars = new string[] { "@", "select ", " where", " as", "*", " from", "(" };

        #endregion

        #region properties
        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return _dataContext; } }

        /// <summary>
        /// 获取或设置自动结束构造，为true时将在Dispose前自动触发Ended事件。
        /// </summary>
        public bool AutoEnd { get; set; }
        /// <summary>
        /// 获取或设置 添加IDbCommand参数委托，默认追加至Parameters。
        /// </summary>
        public virtual AddCommandParameterDelegate AddCommandParameter {
            get { return _whereExpression?.AddCommandParameter; }
            set {
                if (_whereExpression == null)
                    return;
                _whereExpression.AddCommandParameter = value ?? AddCommandParameterDefault;
            }
        }
        /// <summary>
        /// 获取已收集的参数列表。
        /// </summary>
        public object[] Parameters { get { return _parameters == null ? new object[0] : _parameters.ToArray(); } }

        /// <summary>
        /// 获取当前表名
        /// </summary>
        public string TableName { get { return _tableName; } }
        /// <summary>
        /// 获取生成的命令语句。
        /// </summary>
        public virtual string CommandText { get { return BuilderCommandText(); } }
        /// <summary>
        /// 获取仅Where部分。
        /// </summary>
        public virtual string WhereCommandText {
            get {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                BuildWhere(builder);
                return builder.ToString();
            }
        }
        /// <summary>
        /// 获取delete命令语句。
        /// </summary>
        public virtual string DeleteCommmandText {
            get {
                if (IsCustomTable)
                    return "";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.Append("delete").AppendFormat(" from {0}", _dialect.PreName(_tableName));
                BuildWhere(builder);
                return builder.ToString();
            }
        }
        /// <summary>
        /// 获取order by命令语句。
        /// </summary>
        public virtual string OrderByCommandText {
            get {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                BuildOrderBy(builder);
                return builder.ToString();
            }
        }
        /// <summary>
        /// 获取where命令。
        /// </summary>
        public System.Collections.Generic.Dictionary<string, WhereOperators> Wheres { get { return _whereExpression?.Items; } }

        /// <summary>
        /// 获取或设置数据取出条数。
        /// </summary>
        public int TakeCount { get; set; }
        /// <summary>
        /// 获取或设置数据跳过条数。
        /// </summary>
        public int SkipCount { get; set; }
        /// <summary>
        /// 获取字段列表。
        /// </summary>
        public Symbol.Collections.Generic.HashSet<string> Fields { get { return _fields; } }
        /// <summary>
        /// 获取where语句之前的命令。
        /// </summary>
        public Symbol.Collections.Generic.HashSet<string> WhereBefores { get { return _whereBefores; } }
        /// <summary>
        /// 获取order by语句的命令。
        /// </summary>
        public Symbol.Collections.Generic.HashSet<string> OrderBys { get { return _orderbys; } }
        /// <summary>
        /// 获取是否为自定义表。
        /// </summary>
        public virtual bool IsCustomTable {
            get {
                foreach (string item in _customTableChars)
                    if (_tableName.IndexOf(item, System.StringComparison.OrdinalIgnoreCase) != -1)
                        return true;
                return false;
            }
        }
        /// <summary>
        /// 获取方言对象。
        /// </summary>
        public IDialect Dialect { get { return _dialect; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建SelectCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="tableName">表名，如果commandText有值，将忽略此参数。</param>
        /// <param name="commandText">命令脚本。</param>
        public SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText) {
            _dataContext = dataContext;
            dataContext.DisposableObjects?.Add(this);
            _tableName = tableName;
            _fields = new Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            _whereBefores = new Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            _orderbys = new Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            TakeCount = -1;
            _parameters = new System.Collections.Generic.List<object>();
            _dialect = dataContext.Provider.CreateDialect();
            _whereExpression = CreateWhereExpression();
            _whereExpression.AddCommandParameter = AddCommandParameterDefault;
            if (!string.IsNullOrEmpty(commandText)) {
                Parse(commandText);
            }
            _dialect.Keywords.Add("$self", _tableName);
        }
        #endregion

        #region methods

        /// <summary>
        /// 创建WhereExpression对象。
        /// </summary>
        /// <returns>返回WhereExpression对象。</returns>
        protected virtual IWhereExpression CreateWhereExpression() {
            IWhereExpression whereExpression = new WhereExpression(_dataContext, _dialect, _whereExpression == null ? AddCommandParameterDefault : _whereExpression.AddCommandParameter);
            return whereExpression;
        }

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        [Obsolete("请更改为.Dialect.PreName(string name)")]
        public virtual string PreName(string name) {
            return _dialect.PreName(name);
        }
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        [Obsolete("请更改为.Dialect.PreName(string pairs, string spliter)")]
        public virtual string PreName(string pairs, string spliter) {
            return _dialect.PreName(pairs, spliter);
        }
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        [Obsolete("请更改为.Dialect.PreName(string[] pairs)")]
        public virtual string PreName(string[] pairs) {
            return _dialect.PreName(pairs);
        }
        #endregion
        #region Parse
        /// <summary>
        /// 解析命令脚本。
        /// </summary>
        /// <param name="commandText">命令脚本。</param>
        protected abstract void Parse(string commandText);
        #endregion
        #region AddCommandParameterDefault
        /// <summary>
        /// 默认添加参数方法
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <returns>返回参数名称。</returns>
        protected virtual string AddCommandParameterDefault(object value) {
            _parameters.Add(value);
            return "@p" + _parameters.Count;
        }
        #endregion

        #region Build
        /// <summary>
        /// 构造命令脚本。
        /// </summary>
        /// <returns>返回命令脚本。</returns>
        protected abstract string BuilderCommandText();
        /// <summary>
        /// 构造select脚本
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildSelect(System.Text.StringBuilder builder) {
            builder.AppendLine(" select ");
            BuildSelectFields(builder);
            BuildFrom(builder);
        }
        /// <summary>
        /// 构造select 字段脚本
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildSelectFields(System.Text.StringBuilder builder) {
            bool isFirstField = true;
            if (_fields.Count == 0 || LinqHelper.Any(_fields, p => p == "*")) {
                if (!isFirstField)
                    builder.Append(',').AppendLine();
                builder.Append("    ").Append("*");
            } else {
                foreach (string field in LinqHelper.Where(_fields, p => !string.IsNullOrEmpty(p))) {
                    if (isFirstField)
                        isFirstField = false;
                    else
                        builder.Append(',').AppendLine();
                    builder.Append("    ").Append(_dialect.PreName(field));
                }
            }
        }
        /// <summary>
        /// 构造from脚本
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildFrom(System.Text.StringBuilder builder) {
            builder.AppendLine().Append(" from ").Append(_dialect.PreName(_tableName)).Append(" ").AppendLine();
        }
        /// <summary>
        /// 构造where before脚本。
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildWhereBefore(System.Text.StringBuilder builder) {
            if (WhereBefores.Count == 0)
                return;
            builder.AppendLine();
            foreach (string whereBefore in WhereBefores) {
                builder.Append(" ").AppendLine(whereBefore);
            }
            builder.AppendLine();
        }
        /// <summary>
        /// 构造where脚本。
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildWhere(System.Text.StringBuilder builder) {
            if (Wheres.Count == 0)
                return;
            builder.AppendLine(" where ");
            builder.Append(_whereExpression.CommandText);
        }
        /// <summary>
        /// 构造order by脚本。
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildOrderBy(System.Text.StringBuilder builder) {
            if (OrderBys.Count == 0)
                return;
            builder.AppendLine(" order by ");
            bool isFirstOrderBy = true;
            foreach (string orderBy in OrderBys) {
                if (isFirstOrderBy)
                    isFirstOrderBy = false;
                else
                    builder.AppendLine(",");
                builder.Append("    ").Append(orderBy);
            }

        }

        #endregion

        #region Select
        /// <summary>
        /// 生成select 语句。
        /// </summary>
        /// <param name="fields">字段列表。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Select(params string[] fields) {
            _fields.Clear();
            if (fields != null && fields.Length > 0) {
                foreach (string p in fields) {
                    if (string.IsNullOrEmpty(p))
                        continue;
                    _fields.Add(p);
                    //if (p.IndexOf(':') > -1) {

                    //}
                }
            }
            return this;
        }
        #endregion

        #region Count Sum Min Max

        /// <summary>
        /// 生成求count命令。
        /// </summary>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Count() {
            _orderbys.Clear();
            _fields.Clear();
            _fields.Add("count(1)");
            return this;
        }
        /// <summary>
        /// 生成求sum命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Sum(string field) {
            return Sum(field, true);
        }
        /// <summary>
        /// 生成求sum命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Sum(string field, bool clear) {
            CommonException.CheckArgumentNull(field, "field");

            if (clear) {
                _orderbys.Clear();
                _fields.Clear();
            }
            _fields.Add(string.Format("sum({0})", _dialect.PreName(field)));
            return this;
        }
        /// <summary>
        /// 生成求min命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Min(string field) {
            return Min(field, true);
        }
        /// <summary>
        /// 生成求min命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Min(string field, bool clear) {
            CommonException.CheckArgumentNull(field, "field");
            if (clear) {
                _orderbys.Clear();
                _fields.Clear();
            }
            _fields.Add(string.Format("min({0})", _dialect.PreName(field)));
            return this;
        }
        /// <summary>
        /// 生成求max命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Max(string field) {
            return Max(field, true);
        }
        /// <summary>
        /// 生成求max命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Max(string field, bool clear) {
            CommonException.CheckArgumentNull(field, "field");
            if (clear) {
                _orderbys.Clear();
                _fields.Clear();
            }
            _fields.Add(string.Format("max({0})", _dialect.PreName(field)));
            return this;
        }
        /// <summary>
        /// 生成求avg命令（清空所有字段）。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Average(string field) {
            return Average(field, true);
        }
        /// <summary>
        /// 生成求avg命令。
        /// </summary>
        /// <param name="field">字段名称。</param>
        /// <param name="clear">是否清空所有字段</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Average(string field, bool clear) {
            CommonException.CheckArgumentNull(field, "field");
            if (clear) {
                _orderbys.Clear();
                _fields.Clear();
            }
            _fields.Add(string.Format("avg({0})", _dialect.PreName(field)));
            return this;
        }
        #endregion

        #region Take Skip
        /// <summary>
        /// 生成take语法。
        /// </summary>
        /// <param name="count">取出条数。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Take(int count) {
            TakeCount = count;
            return this;
        }
        /// <summary>
        /// 生成skip语法。
        /// </summary>
        /// <param name="skip">跳过条数。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Skip(int skip) {
            SkipCount = skip;
            return this;
        }
        /// <summary>
        /// 生成分页语法。
        /// </summary>
        /// <param name="size">每页大小，忽略小于1。</param>
        /// <param name="page">页码，从0开始，忽略小于0。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Paging(int size, int page) {
            if (size > 0 && page > -1) {
                TakeCount = size;
                SkipCount = size * page;
            }
            return this;
        }

        #endregion

        #region Where
        #region And Or

        /// <summary>
        /// And表达式。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder And(WhereExpressionAction action) {
            _whereExpression?.And(action);
            return this;
        }
        /// <summary>
        /// Or表达式。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Or(WhereExpressionAction action) {
            _whereExpression?.Or(action);
            return this;
        }
        #endregion
        /// <summary>
        /// 生成where语句之前的命令。
        /// </summary>
        /// <param name="befores">命令列表。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder WhereBefore(params string[] befores) {
            if (befores != null && befores.Length > 0) {
                foreach (string p in befores) {
                    if (string.IsNullOrEmpty(p))
                        continue;
                    string p10 = StringExtensions.Replace(p, "$self", _dialect.PreName(_tableName), true);
                    _whereBefores.Add(p10);
                }
            }
            return this;
        }
        /// <summary>
        /// 清空where命令列表。
        /// </summary>
        /// <returns></returns>
        public virtual ISelectCommandBuilder WhereClear() {
            _whereExpression.Clear();
            return this;
        }

        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="expressions">表达式。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Where(WhereOperators @operator, params string[] expressions) {
            _whereExpression?.Where(@operator, expressions);
            return this;
        }
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Where(string expression, string op = "and") {
            _whereExpression?.Where(expression, op);
            return this;
        }
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
        public virtual ISelectCommandBuilder WhereIf(string expression, string value, string op = "and", WhereIfValueFilterDelegate<string> valueFilter = null) {
            _whereExpression?.WhereIf(expression, value, op, valueFilter);
            return this;
        }
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder WhereIf(string expression, string value, WhereOperators @operator, WhereIfValueFilterDelegate<string> valueFilter = null) {
            _whereExpression?.WhereIf(expression, value, @operator, valueFilter);
            return this;
        }
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null。</param>
        /// <param name="min">最小值，不为空时，忽略小于min的值</param>
        /// <param name="max">最大值，不为空时，忽略大于max的值</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder WhereIf(string expression, decimal? value, decimal? min = null, decimal? max = null, string op = "and") {
            _whereExpression?.WhereIf(expression, value, min, max, op);
            return this;
        }
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder WhereIf(string expression, object value, string op = "and", WhereIfValueFilterDelegate<object> valueFilter = null) {
            _whereExpression?.WhereIf(expression, value, op, valueFilter);
            return this;
        }
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">带格式串的表达式。</param>
        /// <param name="value">值，忽略null和string.Empty。</param>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="valueFilter">值过虑器，value不为null或string.Empty时。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder WhereIf(string expression, object value, WhereOperators @operator, WhereIfValueFilterDelegate<object> valueFilter = null) {
            _whereExpression?.WhereIf(expression, value, @operator, valueFilter);
            return this;
        }

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
        public virtual ISelectCommandBuilder Equals(WhereOperators @operator, string field, string value) {
            _whereExpression?.Equals(@operator, field, value);
            return this;
        }
        /// <summary>
        /// 完全匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Equals(WhereOperators @operator, string field, object value) {
            _whereExpression?.Equals(@operator, field, value);
            return this;
        }
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
        public virtual ISelectCommandBuilder Match(WhereOperators @operator, string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals) {
            _whereExpression?.Match(@operator, field, value, matchOperator);
            return this;
        }
        /// <summary>
        /// 操作符匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Match(WhereOperators @operator, string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals) {
            _whereExpression?.Match(@operator, field, value, matchOperator);
            return this;
        }

        /// <summary>
        /// 操作符匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Match(string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and") {
            _whereExpression?.Match(field, value, matchOperator, op);
            return this;
        }
        /// <summary>
        /// 操作符匹配（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="matchOperator">匹配操作符</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Match(string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and") {
            _whereExpression?.Match(field, value, matchOperator, op);
            return this;
        }
        #endregion

        #region Eq NotEq Lt Lte Gt Gte
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Eq(string field, string value, string op = "and") {
            _whereExpression?.Eq(field, value, op);
            return this;
        }
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Eq(string field, object value, string op = "and") {
            _whereExpression?.Eq(field, value, op);
            return this;
        }
        /// <summary>
        /// 不等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder NotEq(string field, object value, string op = "and") {
            _whereExpression?.NotEq(field, value, op);
            return this;
        }
        /// <summary>
        /// 小于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Lt(string field, object value, string op = "and") {
            _whereExpression?.Lt(field, value, op);
            return this;
        }
        /// <summary>
        /// 小于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Lte(string field, object value, string op = "and") {
            _whereExpression?.Lte(field, value, op);
            return this;
        }
        /// <summary>
        /// 大于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Gt(string field, object value, string op = "and") {
            _whereExpression?.Gt(field, value, op);
            return this;
        }
        /// <summary>
        /// 大于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Gte(string field, object value, string op = "and") {
            _whereExpression?.Gte(field, value, op);
            return this;
        }


        #endregion

        #region Like 
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Like(WhereOperators @operator, string field, string value) {
            _whereExpression?.Like(@operator, field, value);
            return this;
        }
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Like(string field, string value, string op = "and") {
            _whereExpression?.Like(field, value, op);
            return this;
        }
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Like(string field, string value, bool reverse, string op = "and") {
            _whereExpression?.Like(field, value, reverse, op);
            return this;
        }

        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder StartsWith(WhereOperators @operator, string field, string value) {
            _whereExpression?.StartsWith(@operator, field, value);
            return this;
        }
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder StartsWith(string field, string value, string op = "and") {
            _whereExpression?.StartsWith(field, value, op);
            return this;
        }
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder StartsWith(string field, string value, bool reverse, string op = "and") {
            _whereExpression?.StartsWith(field, value, reverse, op);
            return this;
        }

        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder EndsWith(WhereOperators @operator, string field, string value) {
            _whereExpression?.EndsWith(@operator, field, value);
            return this;
        }
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder EndsWith(string field, string value, string op = "and") {
            _whereExpression?.EndsWith(field, value, op);
            return this;
        }
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder EndsWith(string field, string value, bool reverse, string op = "and") {
            _whereExpression?.EndsWith(field, value, reverse, op);
            return this;
        }

        #endregion

        #region In NotIn
        /// <summary>
        /// 包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder In(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values) {
            _whereExpression?.In(@operator, field, values);
            return this;
        }
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder In(WhereOperators @operator, string field, System.Collections.IEnumerable values) {
            _whereExpression?.In(@operator, field, values);
            return this;
        }
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder In(string field, System.Collections.IEnumerable values, string op = "and") {
            _whereExpression?.In(field, values, op);
            return this;
        }

        /// <summary>
        /// 不包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder NotIn(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values) {
            _whereExpression?.NotIn(@operator, field, values);
            return this;
        }
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder NotIn(WhereOperators @operator, string field, System.Collections.IEnumerable values) {
            _whereExpression?.NotIn(@operator, field, values);
            return this;
        }
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder NotIn(string field, System.Collections.IEnumerable values, string op = "and") {
            _whereExpression.NotIn(field, values, op);
            return this;
        }

        #endregion

        #endregion

        #region OrderBy
        /// <summary>
        /// 生成order by命令。
        /// </summary>
        /// <param name="orderBys">命令列表。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder OrderBy(params string[] orderBys) {
            System.Collections.Generic.ICollectionExtensions.AddRange(_orderbys, orderBys);
            return this;
        }
        /// <summary>
        /// 生成order by命令（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="orderby">排序规则</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder OrderBy(string field, OrderBys orderby) {
            if (string.IsNullOrEmpty(field))
                return this;
            return OrderBy(_dialect.PreName(field) + " " + EnumExtensions.GetProperty(orderby, "Keyword"));
        }
        #endregion

        #region NoSQL

        #region Refer
        /// <summary>
        /// 引用关系（NoSQL）。
        /// </summary>
        /// <param name="refer">引用关系。</param>
        /// <param name="selfSource">主查询（表名，$self 为自动识别）</param>
        /// <param name="filter">过滤器。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Refer(object refer, string selfSource = "$self", CommandReferFilterDelegate filter = null) {
            if (refer == null)
                return this;
            return Refer(NoSQL.Refer.Parse(refer), selfSource, filter);
        }
        /// <summary>
        /// 引用关系（NoSQL）。
        /// </summary>
        /// <param name="refer">引用关系。</param>
        /// <param name="selfSource">主查询（表名，$self 为自动识别）</param>
        /// <param name="filter">过滤器。</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Refer(NoSQL.Refer refer, string selfSource = "$self", CommandReferFilterDelegate filter = null) {
            if (refer == null || (filter != null && !filter(this, refer)))
                return this;

            if (string.IsNullOrEmpty(selfSource) || string.Equals(selfSource, "$self", System.StringComparison.OrdinalIgnoreCase))
                selfSource = _tableName;

            foreach (NoSQL.ReferEntry item in refer) {
                if (item.IsThis) {
                    continue;
                }
                _whereBefores.Add(string.Format(" left join {0} as {1} on {1}.{2}={3}.{4}",
                    _dialect.PreName(PreSelfName(item.Source, selfSource), "."),
                    _dialect.PreName(item.Name),
                    _dialect.PreName(item.SourceField),
                    _dialect.PreName(PreSelfName(item.Target, selfSource)),
                    _dialect.PreName(item.TargetField, "."))
                );
            }
            return this;
        }
        /// <summary>
        /// $self预处理。
        /// </summary>
        /// <param name="name">如果为$self时，返回self变量的值。</param>
        /// <param name="self">$self的真实值</param>
        /// <returns></returns>
        protected string PreSelfName(string name, string self) {
            if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase)) {
                return self;
            }
            return name;
        }
        #endregion
        #region Query
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <param name="filter">过滤器</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Query(object condition, CommandQueryFilterDelegate filter = null) {
            if (condition == null)
                return this;
            return Query(NoSQL.Condition.Parse(condition), filter);
        }
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <param name="filter">过滤器</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Query(NoSQL.Condition condition, CommandQueryFilterDelegate filter = null) {
            if (condition == null || condition.Type != NoSQL.ConditionTypes.Root || condition.Children.Count == 0)
                return this;
            if (filter != null && !filter(this, condition))
                return this;
            _whereExpression?.Query(condition);
            return this;
        }
        #endregion

        #region Sort
        /// <summary>
        /// 排序（NoSQL）。
        /// </summary>
        /// <param name="sorter">排序对象</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Sort(object sorter) {
            return Sort(NoSQL.Sorter.Parse(sorter));
        }
        /// <summary>
        /// 排序（NoSQL）。
        /// </summary>
        /// <param name="sorter">排序对象</param>
        /// <returns></returns>
        public virtual ISelectCommandBuilder Sort(NoSQL.Sorter sorter) {
            if (sorter != null) {
                foreach (System.Collections.Generic.KeyValuePair<string, object> pair in sorter.ToObject()) {
                    OrderBy(_dialect.PreName(pair.Key, ".") + " " + pair.Value);
                }
            }
            return this;
        }
        #endregion


        #endregion

        #region End
        /// <summary>
        /// 结束构造（触发Ended事件。）
        /// </summary>
        /// <returns></returns>
        public ISelectCommandBuilder End() {
            OnEnd();
            return this;
        }

        /// <summary>
        /// 结束构造（触发Ended事件。）
        /// </summary>
        protected virtual void OnEnd() {
            if (!_ended) {
                _ended = true;
                Ended?.Invoke(this, System.EventArgs.Empty);
            }
        }
        /// <summary>
        /// 已结束，构造已结束时触发，只会触发一次此事件。
        /// </summary>
        public event System.EventHandler Ended;
        #endregion

        #region 查询

        #region CreateQuery
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery() {
            return _dataContext?.CreateQuery(CommandText, Parameters);
        }
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(Action<IDbCommand> action) {
            return _dataContext?.CreateQuery(CommandText, action, Parameters);
        }
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(Type type) {
            return _dataContext?.CreateQuery(type, CommandText, Parameters);
        }
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(Type type, Action<IDbCommand> action) {
            return _dataContext?.CreateQuery(type, CommandText, action, Parameters);
        }
        #endregion
        #region CreateQuery`1
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<T> CreateQuery<T>() {
            return _dataContext?.CreateQuery<T>(CommandText, Parameters);
        }
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="action">回调，可以用command对象进行操作，这发生在处理@params之前。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<T> CreateQuery<T>(Action<IDbCommand> action) {
            return _dataContext?.CreateQuery<T>(CommandText, action, Parameters);
        }
        #endregion

        /// <summary>
        /// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        /// </summary>
        /// <returns>返回第一条记录。</returns>
        public virtual object FirstOrDefault() {
            var q = CreateQuery();
            if (q == null)
                return null;
            using (q) {
                return q.FirstOrDefault();
            }
        }
        /// <summary>
        /// 将查询快速读取并构造一个List对象。
        /// </summary>
        /// <returns>返回一个List对象。</returns>
        public virtual System.Collections.Generic.List<object> ToList() {
            var q = CreateQuery();
            if (q == null)
                return null;
            using (q) {
                return q.ToList();
            }
        }
        /// <summary>
        /// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        /// </summary>
        /// <returns>返回第一条记录。</returns>
        public virtual T FirstOrDefault<T>() {
            var q = CreateQuery<T>();
            if (q == null)
                return default(T);
            using (q) {
                return q.FirstOrDefault();
            }
        }
        /// <summary>
        /// 将查询快速读取并构造一个List对象。
        /// </summary>
        /// <returns>返回一个List对象。</returns>
        public virtual System.Collections.Generic.List<T> ToList<T>() {
            var q = CreateQuery<T>();
            if (q == null)
                return null;
            using (q) {
                return q.ToList();
            }
        }

        #endregion



        #region Dispose
        /// <summary>
        /// 释放对象占用的所有资源。
        /// </summary>
        public virtual void Dispose() {
            if (AutoEnd)
                OnEnd();
            _tableName = null;
            _fields?.Clear();
            _fields = null;
            _whereBefores?.Clear();
            _whereBefores = null;
            _whereExpression?.Clear();
            _whereExpression?.Dispose();
            _whereExpression = null;
            _orderbys?.Clear();
            _orderbys = null;
            _dataContext = null;

            _parameters?.Clear();
            _parameters = null;

            _dialect = null;
            
        }
        #endregion

        #endregion

    }

}