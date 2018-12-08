/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// 插入命令构造器基类
    /// </summary>
    public abstract class UpdateCommandBuilder :
        IUpdateCommandBuilder,
        System.IDisposable {

        #region fields

        /// <summary>
        /// 当前表名
        /// </summary>
        protected string _tableName;
        /// <summary>
        /// 已移除字段列表
        /// </summary>
        protected Symbol.Collections.Generic.HashSet<string> _removedFields;
        /// <summary>
        /// 字段列表
        /// </summary>
        protected Symbol.Collections.Generic.NameValueCollection<object> _fields;
        /// <summary>
        /// 当前数据上下文对象。
        /// </summary>
        protected IDataContext _dataContext;

        #endregion

        #region properties
        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return _dataContext; } }

        /// <summary>
        /// 获取当前表名
        /// </summary>
        public string TableName { get { return _tableName; } }
        /// <summary>
        /// 获取生成的命令语句。
        /// </summary>
        public virtual string CommandText { get { return BuilderCommandText(); } }
        /// <summary>
        /// 获取字段列表（包括字段对应的数据）。
        /// </summary>
        public Symbol.Collections.Generic.NameValueCollection<object> Fields { get { return _fields; } }
        /// <summary>
        /// 获取已移除字段列表（生成脚本时忽略这些字段）。
        /// </summary>
        public Symbol.Collections.Generic.HashSet<string> RemovedFields { get { return _removedFields; } }

        /// <summary>
        /// 获取纯参数列表。
        /// </summary>
        public virtual object[] Values {
            get {
                PreValues();
                return LinqHelper.ToArray(_fields.Values);
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建InsertCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="tableName">表名。</param>
        public UpdateCommandBuilder(IDataContext dataContext, string tableName) {
            _dataContext = dataContext;
            dataContext.DisposableObjects?.Add(this);
            _tableName = tableName;
            _removedFields = new Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            _fields = new Collections.Generic.NameValueCollection<object>();
            _fields.NullValue = System.DBNull.Value;
            _fields.PropertyDescriptor = FieldValueWrapper;
        }
        #endregion

        #region methods

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

        #region PreValues
        /// <summary>
        /// 预处理参数列表。
        /// </summary>
        protected virtual void PreValues() {
            PreRemoveFields();
        }
        #endregion

        #region PreRemoveFields
        /// <summary>
        /// 预处理：移除忽略的字段
        /// </summary>
        protected virtual void PreRemoveFields() {
            foreach (string key in _removedFields) {
                _fields.Remove(key);
            }
        }
        #endregion

        #region FieldValueWrapper
        /// <summary>
        /// 字段值包装处理。
        /// </summary>
        /// <param name="propertyDescriptor">反射对象。</param>
        /// <param name="value">值。</param>
        /// <returns></returns>
        protected virtual object FieldValueWrapper(System.ComponentModel.PropertyDescriptor propertyDescriptor, object value) {
            CommandParameter result = new CommandParameter() {
                Name = propertyDescriptor.Name,
                RealType = TypeExtensions.IsNullableType(propertyDescriptor.PropertyType) ? TypeExtensions.GetNullableType(propertyDescriptor.PropertyType) : propertyDescriptor.PropertyType,
                Value = value,
            };
            System.Type type = propertyDescriptor.ComponentType;
            if (!TypeExtensions.IsAnonymousType(type) && (propertyDescriptor.IsReadOnly || propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name).GetSetMethod() == null))
                _removedFields.Add(propertyDescriptor.Name);
            //else if (TypeExtensions.IsInheritFrom(propertyDescriptor.ComponentType, typeof(IExtensibleModel)) && propertyDescriptor.Name == "Extendeds")
            //    _removedFields.Add(propertyDescriptor.Name);

            return result;
        }
        #endregion
        #region PreFieldValueWrapper
        /// <summary>
        /// 预处理：字段值包装处理
        /// </summary>
        /// <param name="propertyDescriptor">反射对象。</param>
        /// <param name="value">值。</param>
        /// <param name="commandParameter">参数对象。</param>
        protected virtual void PreFieldValueWrapper(System.ComponentModel.PropertyDescriptor propertyDescriptor, object value, CommandParameter commandParameter) {

        }
        #endregion

        #region BuilderCommandText
        /// <summary>
        /// 构造命令脚本。
        /// </summary>
        /// <returns>返回命令脚本。</returns>
        protected virtual string BuilderCommandText() {
            PreRemoveFields();
            if (_fields.Count == 0)
                return string.Empty;
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append(" update ")
                   .Append(PreName(_tableName, "." ))
                   .AppendLine()
                   .AppendLine(" set ");
            int i = 0;
            int pIndex = 0;
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in LinqHelper.ToArray(_fields)) {
                if (PreFieldValue(builder, item.Key, item.Value, ref i, ref pIndex))
                    continue;
                i++;
                if (i > 1) {
                    builder.AppendLine(",");
                }
                builder.Append("    ").Append(PreName(item.Key)).Append("=@p").Append(++pIndex);
            }

            return builder.ToString();
        }
        #endregion

        #region PreFieldValue
        /// <summary>
        /// 预处理：字段值
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        /// <param name="name">字段名称。</param>
        /// <param name="value">字段值。</param>
        /// <param name="i">顺序。</param>
        /// <param name="pIndex">参数顺序。</param>
        /// <returns>返回是否过滤。</returns>
        protected virtual bool PreFieldValue(System.Text.StringBuilder builder, string name, object value, ref int i, ref int pIndex) {
            var p = value as CommandParameter;
            NoSQL.NodeValue nodeValue = new NoSQL.NodeValue(p == null ? value : p.Value);
            if (nodeValue.Type == NoSQL.NodeValueTypes.Dictionary) {
                return PreFieldValue_Dictionary(builder, name, nodeValue, ref i, ref pIndex);
            }
            if (nodeValue.Type == NoSQL.NodeValueTypes.String) {
                return PreFieldValue_String(builder, name, nodeValue, ref i, ref pIndex);
            }
            return false;

        }
        /// <summary>
        /// 预处理：字段值－字典
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        /// <param name="name">字段名称。</param>
        /// <param name="nodeValue">值包装</param>
        /// <param name="i">顺序。</param>
        /// <param name="pIndex">参数顺序。</param>
        /// <returns>返回是否过滤。</returns>
        protected virtual bool PreFieldValue_Dictionary(System.Text.StringBuilder builder, string name, NoSQL.NodeValue nodeValue, ref int i, ref int pIndex) {
            if (nodeValue.Type == NoSQL.NodeValueTypes.Null)
                return false;
            foreach (object item in (System.Collections.IEnumerable)nodeValue.Value) {
                string key = FastWrapper.Get(item, "Key") as string;
                if (string.IsNullOrEmpty(key))
                    continue;
                NoSQL.NodeValue value = new NoSQL.NodeValue(FastWrapper.Get(item, "Value"));
                if (
                    string.Equals(key, "$add", System.StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "$+", System.StringComparison.OrdinalIgnoreCase)) {
                    i++;
                    if (i > 1) {
                        builder.AppendLine(",");
                    }

                    builder.AppendFormat("    {0}={0}+@p{1}", PreName(name), ++pIndex);
                    _fields[name] = value.Value;
                    return true;
                } else if (
                     string.Equals(key, "$minus", System.StringComparison.OrdinalIgnoreCase)
                     || string.Equals(key, "$-", System.StringComparison.OrdinalIgnoreCase)) {
                    i++;
                    if (i > 1) {
                        builder.AppendLine(",");
                    }
                    builder.AppendFormat("    {0}={0}-@p{1}", PreName(name), ++pIndex);
                    _fields[name] = value.Value;
                    return true;
                } else if (
                    string.Equals(key, "$now", System.StringComparison.OrdinalIgnoreCase)) {
                    i++;
                    if (i > 1) {
                        builder.AppendLine(",");
                    }
                    builder.Append("    ").Append(PreName(name)).Append('=').Append(DateTimeNowGrammar());
                    _fields.Remove(name);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 预处理：字段值－文本
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        /// <param name="name">字段名称。</param>
        /// <param name="nodeValue">值包装</param>
        /// <param name="i">顺序。</param>
        /// <param name="pIndex">参数顺序。</param>
        /// <returns>返回是否过滤。</returns>
        protected virtual bool PreFieldValue_String(System.Text.StringBuilder builder, string name, NoSQL.NodeValue nodeValue, ref int i, ref int pIndex) {
            string text = ((string)nodeValue.Value)?.Trim();
            if (string.IsNullOrEmpty(text))
                return false;
            if (text.StartsWith("{") && text.EndsWith("}")) {
                return PreFieldValue_Dictionary(builder, name, new NoSQL.NodeValue(JSON.Parse(text)), ref i, ref pIndex);
            }
            return false;
        }

        /// <summary>
        /// Like 语法
        /// </summary>
        /// <returns></returns>
        protected abstract string DateTimeNowGrammar();
        #endregion

        #region GetValues
        /// <summary>
        /// 获取参数列表。
        /// </summary>
        /// <param name="values">附加参数列表。</param>
        /// <returns>返回附近加的参数列表。</returns>
        public virtual object[] GetValues(params object[] values) {
            System.Collections.Generic.List<object> result = new System.Collections.Generic.List<object>();
            result.AddRange(_fields.Values);
            if (values != null)
                result.AddRange(values);
            return result.ToArray();
        }
        #endregion

        #region GetCommandText
        /// <summary>
        /// 获取命令语句。
        /// </summary>
        /// <param name="commandTextAfterFormat">语句结尾内容格式串。</param>
        /// <param name="args">参与 commandTextAfterFormat 的参数列表。</param>
        /// <returns>返回新的语句。</returns>
        public virtual string GetCommandText(string commandTextAfterFormat, params object[] args) {
            string result = BuilderCommandText();
            if (string.IsNullOrEmpty(commandTextAfterFormat))
                return result;
            result += "\r\n ";
            if (args != null && args.Length > 0)
                result += string.Format(commandTextAfterFormat, args);
            else
                result += commandTextAfterFormat;
            return result;
        }
        #endregion

        #region QueryBlock
        /// <summary>
        /// 查询块，通常用于最终执行前 生成where。
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="where">where action</param>
        /// <param name="end">end action</param>
        /// <returns>返回处理结果。</returns>
        public T QueryBlock<T>(UpdateCommandBuilderQueryBlockWhereAction where, UpdateCommandBuilderQueryBlockEndAction<T> end) {
            PreRemoveFields();
            using (ISelectCommandBuilder whereBuilder = CreateSelect()) {
                System.Collections.Generic.List<object> array = new System.Collections.Generic.List<object>();
                System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
                whereBuilder.AddCommandParameter = (p) => {
                    array.Add(p);
                    string name = "@p_" + (array.Count + _fields.Count);
                    names.Add(name);
                    return name;
                };
                whereBuilder.Select("1");
                if (where != null) {
                    if (!where(whereBuilder))
                        return default(T);
                }
                if (end != null) {
                    string commandText = BuilderCommandText();
                    string whereCommandText = whereBuilder.WhereCommandText;
                    for (int i = 0; i < names.Count; i++) {
                        string name = "@p" + (i + 1 + _fields.Count);
                        whereCommandText = whereCommandText.Replace(names[i], name);
                    }
                    return end(commandText + whereCommandText, GetValues(array.ToArray()));
                }
                return default(T);
            }
        }
        #endregion

        #region CreateSelect
        /// <summary>
        /// 创建Select命令构造器。
        /// </summary>
        /// <returns></returns>
        protected abstract ISelectCommandBuilder CreateSelect();
        #endregion

        #region Dispose
        /// <summary>
        /// 释放对象占用的所有资源。
        /// </summary>
        public virtual void Dispose() {
            _tableName = null;
            _removedFields?.Clear();
            _removedFields = null;
            _fields?.Clear();
            _fields = null;
            _dataContext = null;
        }
        #endregion

        #endregion

    }


}