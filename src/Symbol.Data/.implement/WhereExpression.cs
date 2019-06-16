/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// Where表达式
    /// </summary>
    public class WhereExpression :
        IWhereExpression,
        System.IDisposable {

        #region fields
        /// <summary>
        /// where列表
        /// </summary>
        protected System.Collections.Generic.Dictionary<string, WhereOperators> _wheres;
        /// <summary>
        /// 当前数据上下文对象。
        /// </summary>
        protected IDataContext _dataContext;
        private IDialect _dialect;

        private AddCommandParameterDelegate _addCommandParameter;
        private int _layer;
        private string _layerLeft;
        #endregion

        #region properties

        /// <summary>
        /// 获取方言对象。
        /// </summary>
        public IDialect Dialect { get { return _dialect; } }

        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return _dataContext; } }

        /// <summary>
        /// 获取或设置 添加IDbCommand参数委托，默认追加至Parameters。
        /// </summary>
        public virtual AddCommandParameterDelegate AddCommandParameter {
            get { return _addCommandParameter; }
            set {
                CommonException.CheckArgumentNull(value, "value");
                _addCommandParameter = value;
            }
        }

        /// <summary>
        /// 获取where命令。
        /// </summary>
        public System.Collections.Generic.Dictionary<string, WhereOperators> Items { get { return _wheres; } }

        /// <summary>
        /// 获取生成的命令语句（不包含where）。
        /// </summary>
        public virtual string CommandText {
            get {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                BuildWhere(builder);
                return builder.ToString();
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建WhereExpression实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="dialect">方言对象。</param>
        /// <param name="addCommandParameter"></param>
        public WhereExpression(IDataContext dataContext, IDialect dialect, AddCommandParameterDelegate addCommandParameter)
            : this(dataContext, dialect, addCommandParameter, 1) {
        }
        /// <summary>
        /// 创建WhereExpression实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="dialect">方言对象。</param>
        /// <param name="addCommandParameter"></param>
        /// <param name="layer">层</param>
        public WhereExpression(IDataContext dataContext,IDialect dialect, AddCommandParameterDelegate addCommandParameter, int layer) {
            _dataContext = dataContext;
            _dialect = dialect;
            _addCommandParameter = addCommandParameter;
            dataContext.DisposableObjects?.Add(this);
            _wheres = new System.Collections.Generic.Dictionary<string, WhereOperators>(System.StringComparer.OrdinalIgnoreCase);
            _layer = Math.Max(layer,1);
            _layerLeft = "".PadLeft(4 * _layer, ' ');
        }
        #endregion

        #region methods

        #region BuildWhere
        /// <summary>
        /// 构造where脚本。
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected virtual void BuildWhere(System.Text.StringBuilder builder) {
            if (Items.Count == 0)
                return;
            bool isFirstWhere = true;
            foreach (System.Collections.Generic.KeyValuePair<string, WhereOperators> expression in Items) {
                builder.Append(_layerLeft);
                if (isFirstWhere) {
                    isFirstWhere = false;
                } else if (expression.Value != WhereOperators.None) {
                    builder.Append(expression.Value.ToString().ToLower());
                    builder.Append(" ");
                }
                builder.Append(expression.Key).AppendLine();
            }
        }
        #endregion

        #region Where
        /// <summary>
        /// 清空where命令列表。
        /// </summary>
        /// <returns></returns>
        public virtual IWhereExpression Clear() {
            _wheres.Clear();
            return this;
        }

        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="expressions">表达式。</param>
        /// <returns></returns>
        public virtual IWhereExpression Where(WhereOperators @operator, params string[] expressions) {
            if (expressions != null) {
                foreach (string expression in expressions) {
                    if (string.IsNullOrEmpty(expression))
                        continue;
                    string key = _dialect.ReplaceKeyword(expression);
                    if (_wheres.ContainsKey(key))
                        continue;
                    _wheres.Add(key, @operator);
                }
            }
            return this;
        }
        /// <summary>
        /// 生成where命令。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Where(string expression, string op = "and") {
            if (!string.IsNullOrEmpty(expression)) {
                if (string.IsNullOrEmpty(op))
                    op = "and";
                return Where(TypeExtensions.Convert<WhereOperators>(op), expression);
            }
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
        public virtual IWhereExpression WhereIf(string expression, string value, string op = "and", WhereIfValueFilterDelegate<string> valueFilter = null) {
            if (!string.IsNullOrEmpty(expression) && !string.IsNullOrEmpty(value))
                return Where(string.Format(expression, AddCommandParameter(valueFilter == null ? value : valueFilter(value))), op);
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
        public virtual IWhereExpression WhereIf(string expression, string value, WhereOperators @operator, WhereIfValueFilterDelegate<string> valueFilter = null) {
            if (!string.IsNullOrEmpty(expression) && !string.IsNullOrEmpty(value))
                return Where(@operator, string.Format(expression, AddCommandParameter(valueFilter == null ? value : valueFilter(value))));
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
        public virtual IWhereExpression WhereIf(string expression, decimal? value, decimal? min = null, decimal? max = null, string op = "and") {
            if (value != null && (min == null || value >= min) && (max == null || value <= max))
                return Where(string.Format(expression, AddCommandParameter(value)), op);
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
        public virtual IWhereExpression WhereIf(string expression, object value, string op = "and", WhereIfValueFilterDelegate<object> valueFilter = null) {
            if (!string.IsNullOrEmpty(expression) && value != null) {
                if (value is string && string.IsNullOrEmpty((string)value))
                    return this;
                return Where(string.Format(expression, AddCommandParameter(valueFilter == null ? value : valueFilter(value))), op);
            }
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
        public virtual IWhereExpression WhereIf(string expression, object value, WhereOperators @operator, WhereIfValueFilterDelegate<object> valueFilter = null) {
            if (!string.IsNullOrEmpty(expression) && value != null) {
                if (value is string && string.IsNullOrEmpty((string)value))
                    return this;
                return Where(@operator, string.Format(expression, AddCommandParameter(valueFilter == null ? value : valueFilter(value))));
            }
            return this;
        }

        #endregion

        #region And Or
        /// <summary>
        /// 创建新实例。
        /// </summary>
        /// <returns></returns>
        protected virtual IWhereExpression CreateInstance() {
            return new WhereExpression(_dataContext, _dialect, _addCommandParameter, _layer+1);
        }

        /// <summary>
        /// And表达式。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual IWhereExpression And(WhereExpressionAction action) {
            if (action == null)
                return this;
            using (IWhereExpression expression = CreateInstance()) {
                action(expression);
                string commandText = expression.CommandText;
                if (!string.IsNullOrEmpty(commandText)) {
                    commandText = $"(\r\n{commandText}{_layerLeft})";
                }

                Where(commandText, "and");
            }
            return this;
        }
        /// <summary>
        /// Or表达式。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual IWhereExpression Or(WhereExpressionAction action) {
            if (action == null)
                return this;
            using (IWhereExpression expression = CreateInstance()) {
                action(expression);
                string commandText = expression.CommandText;
                if (!string.IsNullOrEmpty(commandText)) {
                    commandText = $"(\r\n{commandText}{_layerLeft})";
                }
                Where(commandText, "or");
            }
            return this;
        }
        #endregion

        #region Equals
        /// <summary>
        /// 完全匹配（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public virtual IWhereExpression Equals(WhereOperators @operator, string field, string value) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + "={0}", value, @operator);
            return this;
        }
        /// <summary>
        /// 完全匹配（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <returns></returns>
        public virtual IWhereExpression Equals(WhereOperators @operator, string field, object value) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + "={0}", value, @operator);
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
        public virtual IWhereExpression Match(WhereOperators @operator, string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar(EnumExtensions.GetProperty(matchOperator, "Keyword")) + "{0}", value, @operator);
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
        public virtual IWhereExpression Match(WhereOperators @operator, string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar(EnumExtensions.GetProperty(matchOperator, "Keyword")) + "{0}", value, @operator);
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
        public virtual IWhereExpression Match(string field, string value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar(EnumExtensions.GetProperty(matchOperator, "Keyword")) + "{0}", value, op);
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
        public virtual IWhereExpression Match(string field, object value, MatchOpertaors matchOperator = MatchOpertaors.Equals, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar(EnumExtensions.GetProperty(matchOperator, "Keyword")) + "{0}", value, op);
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
        public virtual IWhereExpression Eq(string field, string value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar("=") + "{0}", value, op);
            return this;
        }
        /// <summary>
        /// 等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Eq(string field, object value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar("=") + "{0}", value, op);
            return this;
        }
        /// <summary>
        /// 不等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression NotEq(string field, object value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar("!=") + "{0}", value, op);
            return this;
        }
        /// <summary>
        /// 小于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Lt(string field, object value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar("<") + "{0}", value, op);
            return this;
        }
        /// <summary>
        /// 小于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Lte(string field, object value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar("<=") + "{0}", value, op);
            return this;
        }
        /// <summary>
        /// 大于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Gt(string field, object value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar(">") + "{0}", value, op);
            return this;
        }
        /// <summary>
        /// 大于等于（自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Gte(string field, object value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.PreName(field) + _dialect.MatchOperatorGrammar(">=") + "{0}", value, op);
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
        public virtual IWhereExpression Like(WhereOperators @operator, string field, string value) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, true, true, false), value, @operator, p => _dialect.LikeValueFilter(p, true, true, false));
            return this;
        }
        /// <summary>
        /// 模糊匹配（like %value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression Like(string field, string value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, true, true, false), value, op, p => _dialect.LikeValueFilter(p, true, true, false));
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
        public virtual IWhereExpression Like(string field, string value, bool reverse, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, true, true, reverse), value, op, p => _dialect.LikeValueFilter(p, true, true, reverse));
            return this;
        }

        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public virtual IWhereExpression StartsWith(WhereOperators @operator, string field, string value) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, false, true, false), value, @operator, p => _dialect.LikeValueFilter(p, false, true, false));
            return this;
        }
        /// <summary>
        /// 模糊匹配（like value%，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression StartsWith(string field, string value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, false, true, false), value, op, p => _dialect.LikeValueFilter(p, false, true, false));
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
        public virtual IWhereExpression StartsWith(string field, string value, bool reverse, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, false, true, reverse), value, op, p => _dialect.LikeValueFilter(p, false, true, reverse));
            return this;
        }

        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public virtual IWhereExpression EndsWith(WhereOperators @operator, string field, string value) {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, true, false, false), value, @operator, p => _dialect.LikeValueFilter(p, true, false, false));
            return this;
        }
        /// <summary>
        /// 模糊匹配（like %value，自动忽略空或空文本）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="value">文本内容</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression EndsWith(string field, string value, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, true, false, false), value, op, p => _dialect.LikeValueFilter(p, true, false, false));
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
        public virtual IWhereExpression EndsWith(string field, string value, bool reverse, string op = "and") {
            if (!string.IsNullOrEmpty(field))
                return WhereIf(_dialect.LikeGrammar(field, true, false, reverse), value, op, p => _dialect.LikeValueFilter(p, true, false, reverse));
            return this;
        }

        #endregion

        #region In NotIn
        /// <summary>
        /// 将数组添加到参数列表。
        /// </summary>
        /// <param name="values">通常是数组或List</param>
        /// <returns></returns>
        protected virtual string ArrayToParameter(System.Collections.IEnumerable values) {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (values is string text) {
                text = text.Trim();
                if (text[0] != '[') {
                    text = "[" + text + "]";
                }
                values = JSON.Parse(text) as System.Collections.IEnumerable;
            }
            if (values != null) {
                foreach (object value in values) {
                    if (value == null || (value is string && (string)value == ""))
                        continue;
                    if (builder.Length > 0)
                        builder.Append(',');
                    builder.Append(AddCommandParameter(value));
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        public virtual IWhereExpression In(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values) {
            return In(@operator, field, (System.Collections.IEnumerable)values);
        }
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        public virtual IWhereExpression In(WhereOperators @operator, string field, System.Collections.IEnumerable values) {
            if (string.IsNullOrEmpty(field) || values == null)
                return this;
            string args = ArrayToParameter(values);
            if (args.Length > 0) {
                return Where(@operator, _dialect.PreName(field) + " in(" + args + ")");
            }
            return this;
        }
        /// <summary>
        /// 包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression In(string field, System.Collections.IEnumerable values, string op = "and") {
            if (string.IsNullOrEmpty(field) || values == null)
                return this;
            string args = ArrayToParameter(values);
            if (args.Length > 0) {
                return Where(_dialect.PreName(field) + " in(" + args + ")", op);
            }
            return this;
        }

        /// <summary>
        /// 不包含（自动忽略空或空文本）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">文本内容列表</param>
        /// <returns></returns>
        public virtual IWhereExpression NotIn(WhereOperators @operator, string field, System.Collections.Generic.IEnumerable<string> values) {
            return NotIn(@operator, field, (System.Collections.IEnumerable)values);

        }
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="operator">逻辑操作符。</param>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <returns></returns>
        public virtual IWhereExpression NotIn(WhereOperators @operator, string field, System.Collections.IEnumerable values) {
            if (string.IsNullOrEmpty(field) || values == null)
                return this;
            string args = ArrayToParameter(values);
            if (args.Length > 0) {
                return Where(@operator, "not " + _dialect.PreName(field) + " in(" + args + ")");
            }
            return this;
        }
        /// <summary>
        /// 不包含（自动忽略空）。
        /// </summary>
        /// <param name="field">列，例：aa</param>
        /// <param name="values">内容列表</param>
        /// <param name="op">逻辑操作符：and、or，不区分大小写。</param>
        /// <returns></returns>
        public virtual IWhereExpression NotIn(string field, System.Collections.IEnumerable values, string op = "and") {
            if (string.IsNullOrEmpty(field) || values == null)
                return this;
            string args = ArrayToParameter(values);
            if (args.Length > 0) {
                return Where("not " + _dialect.PreName(field) + " in(" + args + ")", op);
            }
            return this;
        }

        #endregion

        #region Query
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <returns></returns>
        public virtual IWhereExpression Query(object condition) {
            if (condition == null)
                return this;
            return Query(NoSQL.Condition.Parse(condition));
        }
        /// <summary>
        /// 查询规则（NoSQL）。
        /// </summary>
        /// <param name="condition">规则</param>
        /// <returns></returns>
        public virtual IWhereExpression Query(NoSQL.Condition condition) {
            if (condition == null || condition.Type != NoSQL.ConditionTypes.Root || condition.Children.Count == 0)
                return this;

            if (condition.Children.Count == 0)
                return this;
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            using (System.IO.StringWriter writer = new System.IO.StringWriter(builder)) {
                QueryChildren(condition.Children, writer);
                writer.Flush();
            }
            if (builder.Length > 0) {
                string commandText = builder.ToString();
            lb_Next:
                bool b = false;
                if (!string.IsNullOrEmpty(commandText)) {
                    if (commandText.StartsWith(" and (", System.StringComparison.OrdinalIgnoreCase)) {
                        b = true;
                        commandText = commandText.Substring(6, commandText.Length - 8);
                    }
                    if (commandText.StartsWith(" or (", System.StringComparison.OrdinalIgnoreCase)) {
                        b = true;
                        commandText = commandText.Substring(5, commandText.Length - 7);
                    }
                    if (b)
                        goto lb_Next;
                }
                if (!string.IsNullOrEmpty(commandText)) {
                    Where(WhereOperators.And, " ( " + commandText + " ) ");
                }
            }
            return this;
        }
        /// <summary>
        /// 查询规则（NoSQL）子集。
        /// </summary>
        /// <param name="list">规则列表</param>
        /// <param name="writer">输出对象</param>
        /// <param name="innerOperation">内联操作符</param>
        protected virtual void QueryChildren(NoSQL.ConditionCollection list, System.IO.TextWriter writer, string innerOperation = " and ") {
            bool b = true;
            foreach (NoSQL.Condition item in list) {
                if (item.Type == NoSQL.ConditionTypes.Field) {
                    if (list.Owner.Type == NoSQL.ConditionTypes.Field) {
                        //throw new System.NotSupportedException("暂不支持 NoSQL.Condition 多级字段:" +list.Owner.Name+"."+ item.Name);
                        continue;
                    }
                    if (item.Children.Count == 1 && item.Children[0].Type == NoSQL.ConditionTypes.Field) {
                        continue;
                    }
                    if (!b)
                        writer.Write(innerOperation);
                    if (QueryChildrenFieldPre(item, writer, b ? "" : innerOperation, ref b))
                        continue;
                    writer.Write(_dialect.PreName(item.GetNames()));
                    QueryChildren(item.Children, writer);
                } else if (item.Type == NoSQL.ConditionTypes.Logical) {
                    QueryChildrenLogical(item, writer, b ? "" : innerOperation);
                }
                b = false;
            }
        }
        /// <summary>
        /// 查询规则（NoSQL）子集字段预处理。
        /// </summary>
        /// <param name="item">规则</param>
        /// <param name="writer">输出对象</param>
        /// <param name="innerOperation">内联操作符</param>
        /// <param name="firstOperation">第一个操作</param>
        /// <returns>返加是否处理。</returns>
        protected virtual bool QueryChildrenFieldPre(NoSQL.Condition item, System.IO.TextWriter writer, string innerOperation, ref bool firstOperation) {
            switch (item.Children[0].Name) {
                case "$min":
                case "$max":
                case "$sum":
                case "$count": {
                        if (item.Children.Count != 1)
                            return false;
                        writer.Write("{0}({1})", item.Children[0].Name.Substring(1), _dialect.PreName(item.GetNames()));
                        QueryChildren(item.Children[0].Children, writer);
                        return true;
                    }
                case "$notnull": {
                        if (item.Children.Count != 1)
                            return false;
                        writer.Write("not {0} is null", _dialect.PreName(item.GetNames()));
                        return true;
                    }
                case "$ref": {
                        if (item.Children.Count != 1)
                            return false;
                        writer.Write("{0}={1}", _dialect.PreName(item.GetNames()), _dialect.PreName(item.Children[0].Value as string, "."));
                        return true;
                    }
                case "$like": {
                        firstOperation = false;
                        string value = item.Children[0].Value as string;
                        bool reverse = TypeExtensions.Convert(QueryChildrenFieldPre_Extend(item, "reverse"), false);
                        writer.Write(_dialect.LikeGrammar(_dialect.PreName(item.GetNames()), true, true, reverse),
                            AddCommandParameter(_dialect.LikeValueFilter(value, true, true, reverse)));
                        return true;
                    }
                case "$start": {
                        firstOperation = false;
                        string value = item.Children[0].Value as string;
                        bool reverse = TypeExtensions.Convert(QueryChildrenFieldPre_Extend(item, "reverse"), false);
                        writer.Write(_dialect.LikeGrammar(_dialect.PreName(item.GetNames()), false, true, reverse),
                            AddCommandParameter(_dialect.LikeValueFilter(value, false, true, reverse)));
                        return true;
                    }
                case "$end": {
                        firstOperation = false;
                        string value = item.Children[0].Value as string;
                        bool reverse = TypeExtensions.Convert(QueryChildrenFieldPre_Extend(item, "reverse"), false);
                        writer.Write(_dialect.LikeGrammar(_dialect.PreName(item.GetNames()), true, false, reverse),
                            AddCommandParameter(_dialect.LikeValueFilter(value, true, false, reverse)));
                        return true;
                    }

                default:
                    return false;
            }
        }
        object QueryChildrenFieldPre_Extend(NoSQL.Condition item, string name) {
            if (item.Children.Count < 2)
                return null;
            return item.Children.Find(p => string.Equals(p.Name, name, System.StringComparison.OrdinalIgnoreCase))?.Children[0].Value;
        }
        /// <summary>
        /// 查询规则（NoSQL）子集自动处理值。
        /// </summary>
        /// <param name="item">规则</param>
        /// <param name="writer">输出对象</param>
        protected virtual void QueryChildrenAutoValue(NoSQL.Condition item, System.IO.TextWriter writer) {
            if (item.Children.Count > 0) {
                //throw new System.NotSupportedException("暂不支持 NoSQL.Condition 多级字段:"+item.Name);
                QueryChildren(item.Children, writer);
            } else {
                writer.Write(AddCommandParameter(item.Value));
            }
        }
        /// <summary>
        /// 查询规则（NoSQL）子集数组。
        /// </summary>
        /// <param name="item">规则</param>
        /// <param name="format">格式串</param>
        /// <param name="writer">输出对象</param>
        protected virtual void QueryChildrenArray(NoSQL.Condition item, string format, System.IO.TextWriter writer) {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (object value in (System.Collections.IEnumerable)item.Value) {
                if (value == null || (value is string && (string)value == ""))
                    continue;
                if (builder.Length > 0)
                    builder.Append(',');
                builder.Append(AddCommandParameter(value));
            }
            if (builder.Length == 0)
                return;
            writer.Write(format, builder.ToString());
        }
        /// <summary>
        /// 查询规则（NoSQL）子集逻辑操作。
        /// </summary>
        /// <param name="item">规则</param>
        /// <param name="writer">输出对象</param>
        /// <param name="innerOperation">内联操作符</param>
        protected void QueryChildrenLogical(NoSQL.Condition item, System.IO.TextWriter writer, string innerOperation) {
            switch (item.Name) {
                case "$eq": {
                        writer.Write(_dialect.MatchOperatorGrammar("="));
                        QueryChildrenAutoValue(item, writer);
                        break;
                    }
                case "$gt": {
                        writer.Write(_dialect.MatchOperatorGrammar(">"));
                        QueryChildrenAutoValue(item, writer);
                        break;
                    }
                case "$gteq": {
                        writer.Write(_dialect.MatchOperatorGrammar(">="));
                        QueryChildrenAutoValue(item, writer);
                        break;
                    }
                case "$lt": {
                        writer.Write(_dialect.MatchOperatorGrammar("<"));
                        QueryChildrenAutoValue(item, writer);
                        break;
                    }
                case "$lteq": {
                        writer.Write(_dialect.MatchOperatorGrammar("<="));
                        QueryChildrenAutoValue(item, writer);
                        break;
                    }
                case "$noteq": {
                        writer.Write(_dialect.MatchOperatorGrammar("!="));
                        QueryChildrenAutoValue(item, writer);
                        break;
                    }
                case "$like":
                case "$start":
                case "$end":
                    break;
                //case "$like": {
                //        string value = item.Value as string;
                //        writer.Write(LikeGrammar(item.Parent.Name,false),
                //            AddCommandParameter(LikeValueFilter(value, true, true)));
                //        break;
                //    }
                //case "$start": {
                //        string value = item.Value as string;
                //        writer.Write(LikeGrammar(item.Parent.Name, false),
                //            AddCommandParameter(LikeValueFilter(value, false, true)));
                //        break;
                //    }
                //case "$end": {
                //        string value = item.Value as string;
                //        writer.Write(LikeGrammar(item.Parent.Name, false),
                //            AddCommandParameter(LikeValueFilter(value, true, false)));
                //        break;
                //    }
                case "$null": {
                        writer.Write(" is null");
                        break;
                    }
                case "$in": {
                        QueryChildrenArray(item, " in({0})", writer);
                        break;
                    }
                case "$notin": {
                        QueryChildrenArray(item, " not in({0})", writer);
                        break;
                    }
                case "$and": {
                        writer.Write(" and (");
                        QueryChildren(item.Children, writer);
                        writer.Write(") ");
                        break;
                    }
                case "$or": {
                        if (item.Children.IsArray) {
                            writer.Write(" and (");
                            QueryChildren(item.Children, writer, " or ");
                            writer.Write(") ");
                        } else {
                            writer.Write(" or (");
                            QueryChildren(item.Children, writer);
                            writer.Write(") ");
                        }
                        break;
                    }
                case "$not": {
                        writer.Write(" not (");
                        QueryChildren(item.Children, writer);
                        writer.Write(") ");
                        break;
                    }
                case "$min":
                case "$max":
                case "$sum":
                case "$count": {

                        break;
                    }
                default:
                    throw new System.NotSupportedException("暂不支持NoSQL.Condition.Name=" + item.Name);
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放对象占用的所有资源。
        /// </summary>
        public virtual void Dispose() {
            _wheres?.Clear();
            _wheres = null;
            _dataContext = null;
            _addCommandParameter = null;
            _dialect = null;
            _layerLeft = null;
        }
        #endregion

        #endregion

    }

}