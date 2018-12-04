/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// 插入命令构造器基类
    /// </summary>
    public abstract class InsertCommandBuilder :
        IInsertCommandBuilder,
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
        public InsertCommandBuilder(IDataContext dataContext, string tableName) {
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
            System.Text.StringBuilder builderValues = new System.Text.StringBuilder();
            builder.Append(" insert into ").Append(PreName(_tableName, "." )).AppendLine("( ");
            bool isFirst = true;
            int i = 0;
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in _fields) {
                i++;
                if (isFirst) {
                    isFirst = false;
                } else {
                    builder.Append(',').AppendLine();
                    builderValues.Append(',').AppendLine();
                }
                builder.Append("    ").Append(PreName(item.Key));
                builderValues.Append("    @p" + i);
            }
            builder.AppendLine().AppendLine(" ) values ( ");
            builder.AppendLine(builderValues.ToString());
            builder.Append(" ) ");
            builderValues = null;
            return builder.ToString();
        }
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