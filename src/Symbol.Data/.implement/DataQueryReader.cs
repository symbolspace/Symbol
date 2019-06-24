/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;
using System.Reflection;

namespace Symbol.Data {

    /// <summary>
    /// 数据查询读取器基类
    /// </summary>
    public abstract class DataQueryReader : IDataQueryReader {

        #region fields
        private ICommand _command;
        private string _commandText;
        private IConnection _connection;

        #endregion

        #region properties
        /// <summary>
        /// 获取命令对象。
        /// </summary>
        public ICommand Command { get { return ThreadHelper.InterlockedGet(ref _command); } }
        /// <summary>
        /// 获取当前查询命令语句。
        /// </summary>
        public string CommandText { get { return ThreadHelper.InterlockedGet(ref _commandText); } }
        /// <summary>
        /// 获取连接对象。
        /// </summary>
        public IConnection Connection { get { return ThreadHelper.InterlockedGet(ref _connection); } }
        /// <summary>
        /// 获取读取器是否已关闭。
        /// </summary>
        public abstract bool IsClosed { get; }

        /// <summary>
        /// 获取当前行的嵌套深度。
        /// </summary>
        /// <remarks>嵌套的级别，最外面的表的深度为零。</remarks>
        public abstract int Depth { get; }
        /// <summary>
        /// 获取读取器当前字段数量。
        /// </summary>
        public abstract int FieldCount { get; }


        /// <summary>
        /// 获取指定字段的值。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段的值，若字段不存在，则为空。</returns>
        public virtual object this[string name] { get { return GetValue(name); } }
        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        public virtual object this[int index] { get { return GetValue(index); } }

        #endregion


        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        /// <param name="command">命令对象。</param>
        public DataQueryReader(IConnection connection, ICommand command)
            : this(connection, command, command?.Text) {
        }
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        /// <param name="command">命令对象。</param>
        /// <param name="commandText">当前查询命令语句。</param>
        public DataQueryReader(IConnection connection, ICommand command, string commandText) {
            _connection = connection;
            _command = command;
            _commandText = commandText;

            command?.DataContext?.DisposableObjects?.Add(this);

        }
        #endregion

        #region methods

        /// <summary>
        /// 检测指定字段是否存在。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回false。</param>
        /// <returns>如果字段存在，则为true。</returns>
        public virtual bool Exists(string name) {
            return GetIndex(name) == -1;
        }
        /// <summary>
        /// 获取指定字段当前从0开始的索引顺序。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回-1。</param>
        /// <returns>返回字段索引顺序，若字段不存在，则为-1。</returns>
        public abstract int GetIndex(string name);
        /// <summary>
        /// 获取指定索引的字段名称。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应的字段名称，若字段不存在，则为空。</returns>
        public abstract string GetName(int index);

        /// <summary>
        /// 获取指定字段的类型。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段类型，若字段不存在，则为空。</returns>
        public virtual System.Type GetType(string name) {
            return GetType(GetIndex(name));
        }
        /// <summary>
        /// 获取指定索引对应字段的类型。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的类型，若字段不存在，则为空。</returns>
        public abstract System.Type GetType(int index);


        /// <summary>
        /// 获取指定字段的数据类型名称。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段数据类型名称，若字段不存在，则为空。</returns>
        public virtual string GetDataTypeName(string name) {
            return GetDataTypeName(GetIndex(name));
        }
        /// <summary>
        /// 获取指定索引对应字段的数据类型名称。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的数据类型名称，若字段不存在，则为空。</returns>
        public abstract string GetDataTypeName(int index);

        /// <summary>
        /// 获取指定字段的值。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段的值，若字段不存在，则为空。</returns>
        public virtual object GetValue(string name) {
            return GetValue(GetIndex(name));
        }
        /// <summary>
        /// 获取指定字段的值。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <param name="type">目标类型，尝试转换为此类型，为空则保持原状。</param>
        /// <returns>返回字段的值，若字段不存在，则为空。</returns>
        public virtual object GetValue(string name, System.Type type) {
            return GetValue(GetIndex(name), type);
        }
        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        public abstract object GetValue(int index);
        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <param name="type">目标类型，尝试转换为此类型，为空则保持原状。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        public virtual object GetValue(int index, System.Type type) {
            if (index < 0 || index > FieldCount - 1)
                return TypeExtensions.DefaultValue(type);
            object value = GetValue(index);
            if (IsNullValue(value))
                return TypeExtensions.DefaultValue(type);

            if (type == null) {
                if (TryParseJSON(value, out object jsonObject)){
                    return jsonObject;
                }
                return value;
            }
            if (TryConvertValue(GetType(index), value, index, type, out object target)) {
                return target;
            }
            return value;
        }

        /// <summary>
        /// 检测指定字段的值是否为空、DBNull。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull，则为true。</returns>
        public virtual bool IsNullValue(string name) {
            return IsNullValue(GetValue(name));
        }
        /// <summary>
        /// 检测指定索引顺序对应字段的值是否为空、DBNull。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull，则为true。</returns>
        public virtual bool IsNullValue(int index) {
            return IsNullValue(GetValue(index));
        }
        /// <summary>
        /// 检测指定值是否为空、DBNull。
        /// </summary>
        /// <param name="value">任意值。</param>
        /// <returns>返回检测结果，如果值为空、DBNull，则为true。</returns>
        protected virtual bool IsNullValue(object value) {
            return value == null || value is System.DBNull;
        }

        /// <summary>
        /// 检测指定字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        public virtual bool IsNullOrEmpty(string name) {
            return IsNullOrEmpty(name, false);
        }
        /// <summary>
        /// 检测指定字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回true。</param>
        /// <param name="trim">是否对文本进行trim操作。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        public virtual bool IsNullOrEmpty(string name, bool trim) {
            return IsNullOrEmpty(GetValue(name), trim);
        }
        /// <summary>
        /// 检测指定索引顺序对应字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        public virtual bool IsNullOrEmpty(int index) {
            return IsNullOrEmpty(index, false);
        }
        /// <summary>
        /// 检测指定索引顺序对应字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为true。</param>
        /// <param name="trim">是否对文本进行trim操作。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        public virtual bool IsNullOrEmpty(int index, bool trim) {
            return IsNullOrEmpty(GetValue(index), trim);
        }
        /// <summary>
        /// 检测指定值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="value">任意值。</param>
        /// <param name="trim">是否对文本进行trim操作。</param>
        /// <returns>返回检测结果，如果值为空、DBNull，则为true。</returns>
        protected virtual bool IsNullOrEmpty(object value, bool trim) {
            if (value == null || value is System.DBNull)
                return true;
            if (value is string text) {
                if (trim)
                    return string.IsNullOrEmpty(text?.Trim());
                return string.IsNullOrEmpty(text);
            }
            return false;
        }

        /// <summary>
        /// 使读取器前进到下一个结果。
        /// </summary>
        /// <returns>如果存在更多结果集，则为true。</returns>
        /// <remarks>用于处理多个结果，默认情况下，数据读取器定位在第一个结果。</remarks>
        public abstract bool NextResult();
        /// <summary>
        /// 让读取器前进到下一条记录。
        /// </summary>
        /// <returns>如果存在更多的记录，则为true。</returns>
        public abstract bool Read();

        /// <summary>
        /// 关闭读取器。
        /// </summary>
        public abstract void Close();


        /// <summary>
        /// 尝试转换值。
        /// </summary>
        /// <param name="type">字段类型。</param>
        /// <param name="value">字段的值。</param>
        /// <param name="index">字段的索引值，从0开始。</param>
        /// <param name="targetType">目标类型。</param>
        /// <param name="target">输出转换结果。</param>
        /// <returns>返回尝试结果，为true表示成功。</returns>
        protected virtual bool TryConvertValue(System.Type type, object value, int index, System.Type targetType, out object target) {
            if (targetType != null) {
                target = TypeExtensions.Convert(value, targetType);
                return true;
            }
            target = null;
            return false;
        }

        /// <summary>
        /// 尝试解析为JSON对象。
        /// </summary>
        /// <param name="value">值，若为字符串，则尝试解析，仅支持{}和[]。</param>
        /// <param name="jsonObject">输出解析结果。</param>
        /// <returns>返回尝试结果，为true表示成功。</returns>
        protected virtual bool TryParseJSON(object value, out object jsonObject) {
            return TryParseJSON(value, null, out jsonObject);
        }
        /// <summary>
        /// 尝试解析为JSON对象。
        /// </summary>
        /// <param name="value">值，若为字符串，则尝试解析，仅支持{}和[]。</param>
        /// <param name="type">JSON对象类型，为空则为弱类型。</param>
        /// <param name="jsonObject">输出解析结果。</param>
        /// <returns>返回尝试结果，为true表示成功。</returns>
        protected virtual bool TryParseJSON(object value, System.Type type, out object jsonObject) {
            if (value is string text) {
                text = text?.Trim();
                if ((text.StartsWith("{") && text.EndsWith("}")) || (text.StartsWith("[") && text.EndsWith("]"))) {
                    if (type == null)
                        jsonObject = JSON.Parse(text);
                    else
                        jsonObject = JSON.ToObject(text, type);
                    return true;
                }
            }
            jsonObject = null;
            return false;
        }

        /// <summary>
        /// 映射为实体对象：首字段尝试。
        /// </summary>
        /// <param name="type">类型，为空则尝试失败。</param>
        /// <param name="value">输出结果。</param>
        /// <returns>返回是否尝试成功。</returns>
        protected virtual bool ToObject_Try_FirstField(System.Type type, out object value) {
            value = null;

            //必须有类型。
            if (type == null)
                return false;

            //值类型、枚举、字符串
            if (type.IsValueType || type.IsEnum || type == typeof(string))
                goto lb_GetValue;

            //可为空类型，取掉包装
            var nullableType = TypeExtensions.GetNullableType(type);
            //值类型、枚举
            if (nullableType.IsValueType || nullableType.IsEnum)
                goto lb_GetValue;

            //剩下的没有可能性
            return false;


            lb_GetValue:

            if (FieldCount == 0)
                goto lb_DefaultValue; //没字段，取默认值
            //读取第一个字段的值
            value = GetValue(0);
            //有值直接返回
            if (value != null) {
                if (TryParseJSON(value, out object jsonObject)) {
                    value = jsonObject;
                    return true;
                }
                value = TypeExtensions.Convert(value, type);
                return true;
            }

            lb_DefaultValue:
            //默认值
            value = TypeExtensions.DefaultValue(type);
            return true;
        }

        delegate void DictionaryAddAction(string key, object value);
        /// <summary>
        /// 映射为实体对象：字典尝试。
        /// </summary>
        /// <param name="type">类型，为空则尝试失败。</param>
        /// <param name="result">输出结果。</param>
        /// <returns>返回是否尝试成功。</returns>
        protected virtual bool ToObject_Try_Dictionary(System.Type type, out object result) {
            result = null;

            DictionaryAddAction dictionaryAdd;
            if (type == null || type==typeof(object)) {
                //默认类型
                var values = new Symbol.Collections.Generic.NameValueCollection<object>();
                result = values;
                dictionaryAdd = values.Add;
            } else if (TypeExtensions.IsInheritFrom(type, typeof(Collections.Generic.NameValueCollection<object>))) {
                //继承关系
                var values = FastWrapper.CreateInstance<Collections.Generic.NameValueCollection<object>>(type, new object[0]);
                result = values;
                dictionaryAdd = values.Add;
            } else if (TypeExtensions.IsInheritFrom(type, typeof(System.Collections.Generic.IDictionary<string, object>))) {
                //泛型
                var values = FastWrapper.CreateInstance<System.Collections.Generic.IDictionary<string, object>>(type, new object[0]);
                result = values;
                dictionaryAdd = (key, value) => {
                    if (values.TryGetValue(key, out object p)) {
                        values[key] = value;
                    } else {
                        values.Add(key, value);
                    }
                };
            } else if (TypeExtensions.IsInheritFrom(type, typeof(System.Collections.IDictionary))) {
                //非泛型
                var values = FastWrapper.CreateInstance<System.Collections.IDictionary>(type, new object[0]);
                result = values;
                dictionaryAdd = (key, value) => {
                    if (values.Contains(key)) {
                        values[key] = value;
                    } else {
                        values.Add(key, value);
                    }
                };
            } else {
                //剩下的没有可能性
                return false;
            }

            for (int i = 0; i < FieldCount; i++) {
                string name = GetName(i);
                object value = GetValue(i);
                if (TryParseJSON(value, out object jsonObject)) {
                    value = jsonObject;
                }
                dictionaryAdd(string.IsNullOrEmpty(name) ? ("无列名" + i) : name, value);
            }
            return true;
        }
        /// <summary>
        /// 映射为实体对象：实体尝试。
        /// </summary>
        /// <param name="type">类型，为空则尝试失败。</param>
        /// <param name="result">输出结果。</param>
        /// <returns>返回是否尝试成功。</returns>
        protected virtual bool ToObject_Try_Entity(System.Type type, out object result) {
            result = FastWrapper.CreateInstance(type);
            //没有字段？
            if (FieldCount == 0)
                return true;

            for (int i = 0; i < FieldCount; i++) {
                string name = GetName(i);

                var propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                if (propertyInfo != null) {
                    object value = GetValue(i, propertyInfo.PropertyType);
                    propertyInfo.SetValue(result, value, null);
                    continue;
                }

                var fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                if (fieldInfo != null) {
                    object value = GetValue(i, fieldInfo.FieldType);
                    fieldInfo.SetValue(result, value);
                    continue;
                }
            }
            return true;

        }

        /// <summary>
        /// 映射为实体对象。
        /// </summary>
        /// <param name="type">类型，为空则为字典类型（<see cref="Collections.Generic.NameValueCollection{T}"/>）。</param>
        /// <returns>如果type为基础类型，并且与第0个字段类型相等，则返回该字段的值；否则为type对应的实体对象。</returns>
        /// <remarks>如果读取器已关闭，则返type的默认值。</remarks>
        public virtual object ToObject(System.Type type) {
            if (IsClosed)
                return TypeExtensions.DefaultValue(type);

            //首字段尝试
            {
                if (ToObject_Try_FirstField(type, out object value))
                    return value;
            }
            //字典尝试
            {
                if (ToObject_Try_Dictionary(type, out object value))
                    return value;
            }
            //实体尝试
            {
                if (ToObject_Try_Entity(type, out object value))
                    return value;
            }

            //用不上的代码
            return TypeExtensions.DefaultValue(type);
        }


        #region Dispose
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public virtual void Dispose() {
            ThreadHelper.InterlockedSet(ref _connection, null);
            ThreadHelper.InterlockedSet(ref _command, null);
            ThreadHelper.InterlockedSet(ref _commandText, null);
        }

        #endregion


        #endregion

    }

}