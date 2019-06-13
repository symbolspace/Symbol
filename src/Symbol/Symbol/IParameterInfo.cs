/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Symbol;
using Symbol.Formatting;

namespace Symbol {



    /// <summary>
    /// 参数信息接口。
    /// </summary>
    public interface IParameterInfo : ICustomAttributeProvider {

        /// <summary>
        /// 获取参数名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取参数别名（尝试寻找AliasNameAttribute和[Const("Alias","xxx")]）。
        /// </summary>
        string AliasName { get; }
        /// <summary>
        /// 获取是否为输入。
        /// </summary>
        bool IsIn { get; }
        /// <summary>
        /// 获取是否为输出，带有out关键字。
        /// </summary>
        bool IsOut { get; }
        /// <summary>
        /// 获取是否为可选参数。
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// 获取参数类型。
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 获取默认参数（没有时返回System.DBNull.Value）。
        /// </summary>
        object DefaultValue { get; }

    }

    /// <summary>
    /// 方法参数信息。
    /// </summary>
    public abstract class CustomAttributeProvider<T> : System.Reflection.ICustomAttributeProvider where T : System.Reflection.ICustomAttributeProvider {

        #region fields
        /// <summary>
        /// 目标对象。
        /// </summary>
        protected T _target;
        /// <summary>
        /// 参数别名。
        /// </summary>
        protected string _aliasName;
        #endregion

        #region properties
        /// <summary>
        /// 获取参数名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取参数别名（尝试寻找AliasNameAttribute和[Const("Alias","xxx")]）。
        /// </summary>
        public virtual string AliasName {
            get {
                if (string.IsNullOrEmpty(_aliasName)) {
                    _aliasName = GetAliasName();
                }
                return _aliasName;
            }
        }
        /// <summary>
        /// 获取目标对象。
        /// </summary>
        public T Target { get { return _target; } }

        #endregion


        #region ctor
        /// <summary>
        /// 创建CustomAttributeProvider实例。
        /// </summary>
        /// <param name="target">目标对象。</param>
        protected CustomAttributeProvider(T target) {
            _target = target;
        }
        #endregion

        #region methods

        /// <summary>
        /// 获取参数别名（尝试寻找AliasNameAttribute和[Const("Alias","xxx")]）。
        /// </summary>
        /// <returns>返回参数别名，未找到返回原名称。</returns>
        protected virtual string GetAliasName() {
            var name = AttributeExtensions.GetCustomAttribute<AliasAttribute>(_target)?.Name;
            if (string.IsNullOrEmpty(name))
                name = ConstAttributeExtensions.Const(_target, "Alias");
            if (string.IsNullOrEmpty(name))
                name = Name;
            return name;
        }

        /// <summary>
        /// 获取该参数上定义的指定类型的自定义属性。
        /// </summary>
        /// <param name="attributeType">由类型标识的自定义属性。</param>
        /// <param name="inherit">对于该类型的对象，该参数被忽略。</param>
        /// <returns>Object 类型数组，该数组包含指定类型的自定义属性。</returns>
        public object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return _target?.GetCustomAttributes(attributeType, inherit);
        }

        /// <summary>
        /// 获取该参数上定义的指定类型的自定义属性。
        /// </summary>
        /// <param name="inherit">对于该类型的对象，该参数被忽略。</param>
        /// <returns>Object 类型数组，该数组包含指定类型的自定义属性。</returns>
        public object[] GetCustomAttributes(bool inherit) {
            return _target?.GetCustomAttributes(inherit);
        }
        /// <summary>
        /// 确定该成员上是否定义了指定类型的自定义特性。
        /// </summary>
        /// <param name="attributeType">由类型标识的自定义属性。</param>
        /// <param name="inherit">对于该类型的对象，该参数被忽略。</param>
        /// <returns>如果该成员上定义了一个或多个 attributeType 实例，则为 true；否则为 false。</returns>
        public bool IsDefined(Type attributeType, bool inherit) {
            if (_target == null)
                return false;
            return Symbol.AttributeExtensions.IsDefined(_target, attributeType, inherit);
        }

        #endregion

    }
    /// <summary>
    /// 方法参数信息。
    /// </summary>
    public class MethodParameterInfo : CustomAttributeProvider<System.Reflection.ParameterInfo>, IParameterInfo {

        #region properties
        /// <summary>
        /// 获取参数名称。
        /// </summary>
        public override string Name { get { return _target?.Name; } }
        /// <summary>
        /// 获取是否为输入。
        /// </summary>
        public bool IsIn { get { return _target?.IsIn ?? false; } }
        /// <summary>
        /// 获取是否为输出，带有out关键字。
        /// </summary>
        public bool IsOut { get { return _target?.IsOut ?? false; } }
        /// <summary>
        /// 获取是否为可选参数。
        /// </summary>
        public bool IsOptional { get { return _target?.IsOptional ?? false; } }

        /// <summary>
        /// 获取参数类型。
        /// </summary>
        public Type Type { get { return _target?.ParameterType; } }
        /// <summary>
        /// 获取默认参数（没有时返回System.DBNull.Value）。
        /// </summary>
        public object DefaultValue { get { return _target?.DefaultValue; } }
        #endregion


        #region ctor
        private MethodParameterInfo(System.Reflection.ParameterInfo target)
            : base(target) {
        }
        #endregion

        #region methods

        #region implicit
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="value">参数对象。</param>
        public static implicit operator MethodParameterInfo(System.Reflection.ParameterInfo value) {
            return value == null ? null : new MethodParameterInfo(value);
        }
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="info">参数对象。</param>
        public static MethodParameterInfo As(System.Reflection.ParameterInfo info) {
            if (info == null)
                return null;
            return new MethodParameterInfo(info);
        }

        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="list">参数列表。</param>
        public static MethodParameterInfo[] As(System.Reflection.ParameterInfo[] list) {
            if (list == null || list.Length == 0)
                return new MethodParameterInfo[0];
            MethodParameterInfo[] array = new MethodParameterInfo[list.Length];
            for (int i = 0; i < list.Length; i++) {
                array[i] = list[i];
            }
            return array;
        }
        #endregion
        #region explicit
        /// <summary>
        /// 快速转换。
        /// </summary>
        /// <param name="value">实例。</param>
        public static explicit operator System.Reflection.ParameterInfo(MethodParameterInfo value) {
            return value?._target;
        }
        #endregion

        #endregion

    }
    /// <summary>
    /// 属性参数信息。
    /// </summary>
    public class PropertyParameterInfo : CustomAttributeProvider<System.Reflection.PropertyInfo>, IParameterInfo {

        #region properties
        /// <summary>
        /// 获取参数名称。
        /// </summary>
        public override string Name { get { return _target?.Name; } }
        /// <summary>
        /// 获取是否为输入。
        /// </summary>
        public bool IsIn { get { return _target?.CanWrite ?? false; } }
        /// <summary>
        /// 获取是否为输出，带有out关键字。
        /// </summary>
        public bool IsOut { get { return _target?.CanRead ?? false; } }
        /// <summary>
        /// 获取是否为可选参数。
        /// </summary>
        public bool IsOptional { get { return _target?.PropertyType.IsClass ?? false; } }

        /// <summary>
        /// 获取参数类型。
        /// </summary>
        public Type Type { get { return _target?.PropertyType; } }
        /// <summary>
        /// 获取默认参数（没有时返回System.DBNull.Value）。
        /// </summary>
        public object DefaultValue { get { return System.DBNull.Value; } }
        #endregion


        #region ctor
        private PropertyParameterInfo(System.Reflection.PropertyInfo target)
            : base(target) {
        }
        #endregion

        #region methods

        #region implicit
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="value">参数对象。</param>
        public static implicit operator PropertyParameterInfo(System.Reflection.PropertyInfo value) {
            return value == null ? null : new PropertyParameterInfo(value);
        }
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="info">参数对象。</param>
        public static PropertyParameterInfo As(System.Reflection.PropertyInfo info) {
            if (info == null)
                return null;
            return new PropertyParameterInfo(info);
        }

        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="list">参数列表。</param>
        public static PropertyParameterInfo[] As(System.Reflection.PropertyInfo[] list) {
            if (list == null || list.Length == 0)
                return new PropertyParameterInfo[0];
            PropertyParameterInfo[] array = new PropertyParameterInfo[list.Length];
            for (int i = 0; i < list.Length; i++) {
                array[i] = list[i];
            }
            return array;
        }
        #endregion
        #region explicit
        /// <summary>
        /// 快速转换。
        /// </summary>
        /// <param name="value">实例。</param>
        public static explicit operator System.Reflection.PropertyInfo(PropertyParameterInfo value) {
            return value?._target;
        }
        #endregion

        #endregion

    }
    /// <summary>
    /// 字段参数信息。
    /// </summary>
    public class FieldParameterInfo : CustomAttributeProvider<System.Reflection.FieldInfo>, IParameterInfo {

        #region properties
        /// <summary>
        /// 获取参数名称。
        /// </summary>
        public override string Name { get { return _target?.Name; } }
        /// <summary>
        /// 获取是否为输入。
        /// </summary>
        public bool IsIn { get { return _target != null; } }
        /// <summary>
        /// 获取是否为输出，带有out关键字。
        /// </summary>
        public bool IsOut { get { return _target != null; } }
        /// <summary>
        /// 获取是否为可选参数。
        /// </summary>
        public bool IsOptional { get { return _target?.FieldType.IsClass ?? false; } }

        /// <summary>
        /// 获取参数类型。
        /// </summary>
        public Type Type { get { return _target?.FieldType; } }
        /// <summary>
        /// 获取默认参数（没有时返回System.DBNull.Value）。
        /// </summary>
        public object DefaultValue { get { return System.DBNull.Value; } }
        #endregion


        #region ctor
        private FieldParameterInfo(System.Reflection.FieldInfo target)
            : base(target) {
        }
        #endregion

        #region methods

        #region implicit
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="value">参数对象。</param>
        public static implicit operator FieldParameterInfo(System.Reflection.FieldInfo value) {
            return value == null ? null : new FieldParameterInfo(value);
        }
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="info">参数对象。</param>
        public static FieldParameterInfo As(System.Reflection.FieldInfo info) {
            if (info == null)
                return null;
            return new FieldParameterInfo(info);
        }

        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="list">参数列表。</param>
        public static FieldParameterInfo[] As(System.Reflection.FieldInfo[] list) {
            if (list == null || list.Length == 0)
                return new FieldParameterInfo[0];
            FieldParameterInfo[] array = new FieldParameterInfo[list.Length];
            for (int i = 0; i < list.Length; i++) {
                array[i] = list[i];
            }
            return array;
        }
        #endregion
        #region explicit
        /// <summary>
        /// 快速转换。
        /// </summary>
        /// <param name="value">实例。</param>
        public static explicit operator System.Reflection.FieldInfo(FieldParameterInfo value) {
            return value?._target;
        }
        #endregion

        #endregion

    }

    /// <summary>
    /// 参数绑定器。
    /// </summary>
    public class ParameterInfoBinder : System.IDisposable {

        #region fields
        private System.Collections.Generic.IList<IParameterInfo> _paramters;
        private System.Collections.Generic.IList<IParameterInfo> _otherParamters;
        private TypeExtensions.ConvertValue _convertValue;
        private object _bodyData;
        private object[] _datas;

        private TryGetValueFunc[] _tryGetValues;

        #endregion

        #region properties
        /// <summary>
        /// 获取或设置参数列表。
        /// </summary>
        public System.Collections.Generic.IList<IParameterInfo> Paramters {
            get { return _paramters; }
            set { _paramters = value; }
        }
        /// <summary>
        /// 获取或设置其它参数列表（DataBody模式时将排除这些参数）。
        /// </summary>
        public System.Collections.Generic.IList<IParameterInfo> OtherParamters {
            get { return _otherParamters; }
            set { _otherParamters = value; }
        }
        /// <summary>
        /// 获取或设置Body数据（默认为Datas[0]）。
        /// </summary>
        public object BodyData {
            get { return _bodyData; }
            set {
                _bodyData = value;
                _tryGetValues = null;
            }
        }
        /// <summary>
        /// 获取或设置数据列表。
        /// </summary>
        public object[] Datas {
            get { return _datas; }
            set {
                _datas = value;
                if (_bodyData == null && value != null && value.Length > 0)
                    _bodyData = value[0];
                _tryGetValues = null;
            }
        }
        /// <summary>
        /// 获取或设置值转换器。
        /// </summary>
        public TypeExtensions.ConvertValue ConvertValue {
            get { return _convertValue; }
            set { _convertValue = value ?? ConvertValue_Default; }
        }


        #endregion

        #region ctor
        /// <summary>
        /// 创建实ParameterInfoBinder例。
        /// </summary>
        public ParameterInfoBinder() {
            _convertValue = ConvertValue_Default;
        }
        /// <summary>
        /// 创建实ParameterInfoBinder例。
        /// </summary>
        /// <param name="paramters">参数列表。</param>
        /// <param name="convertValue">值转换器。</param>
        /// <param name="datas">数据列表。</param>
        public ParameterInfoBinder(System.Collections.Generic.IList<IParameterInfo> paramters, TypeExtensions.ConvertValue convertValue, object[] datas) {
            _paramters = paramters;
            _convertValue = convertValue ?? ConvertValue_Default;
            Datas = datas;
        }
        #endregion

        #region methods

        #region ToArray
        /// <summary>
        /// 将绑定后的参数以数组方式输出。
        /// </summary>
        /// <returns></returns>
        public object[] ToArray() {
            if (_paramters == null || _paramters.Count == 0)
                return new object[0];

            Init();
            var list = new object[_paramters.Count];
            for (int i = 0; i < _paramters.Count; i++) {
                list[i] = TryGetValue(i, _paramters[i]);
            }
            return list;
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 将绑定后的参数以抽象对象方式输出
        /// </summary>
        /// <returns></returns>
        public Symbol.Collections.Generic.NameValueCollection<object> ToObject() {
            var list = FastWrapper.As(null);
            if (_paramters == null || _paramters.Count == 0)
                return list;
            Init();
            for (int i = 0; i < _paramters.Count; i++) {
                list[_paramters[i].Name] = TryGetValue(i, _paramters[i]);
            }
            return list;
        }
        #endregion

        #region ConvertValue_Default
        object ConvertValue_Default(object value, System.Type type) {
            if (type.IsValueType && !TypeExtensions.IsNullableType(type)) {
                return TypeExtensions.Convert(value, typeof(System.Nullable<>).MakeGenericType(type));
            } else {
                return TypeExtensions.Convert(value, type);
            }
        }
        #endregion

        #region TryGetValue whens
        bool TryGetValue_Empty(int p1, IParameterInfo p2, object p3, out object value) {
            value = null;
            return false;
        }
        bool TryGetValue_As(int p1, IParameterInfo p2, object p3, out object value) {
            var p10 = p3.GetType();
            if (p2.Type == p10 || TypeExtensions.IsInheritFrom(p2.Type, p10)) {
                value = p3;
                return true;
            }
            value = null;
            return false;
        }
        bool TryGetValue_Any(int p1, IParameterInfo p2, object p3, out object value) {
            var p10 = p3.GetType();
            if (p2.Type == p10 || TypeExtensions.IsInheritFrom(p2.Type, p10)) {
                value = p3;
                return true;
            }
            return FastWrapper.TryGet(p10, p2.Name, p3, new object[0], out value);
        }
        bool TryGetValue_IDictionary_string_object(int p1, IParameterInfo p2, object p3, out object value) {
            var p10 = (System.Collections.Generic.IDictionary<string, object>)p3;
            if (p10.TryGetValue(p2.Name, out value)) {
                return true;
            }
            return TryGetValue_As(p1, p2, p3, out value);
        }
        bool TryGetValue_IDictionary(int p1, IParameterInfo p2, object p3, out object value) {
            var p10 = (System.Collections.IDictionary)p3;
            if (p10.Contains(p2.Name)) {
                value = p10[p2.Name];
                return true;
            }
            return TryGetValue_As(p1, p2, p3, out value);
        }
        bool TryGetValue_IList(int p1, IParameterInfo p2, object p3, out object value) {
            var p10 = (System.Collections.IList)p3;
            if (p1 < p10.Count) {
                value = p10[p1];
                return true;
            }
            return TryGetValue_As(p1, p2, p3, out value);
        }
        bool TryGetValue_NameValueCollection(int p1, IParameterInfo p2, object p3, out object value) {
            var p10 = (System.Collections.Specialized.NameValueCollection)p3;

            if (p2.Type.IsArray || (p2.Type != typeof(string) && TypeExtensions.IsInheritFrom(p2.Type, typeof(System.Collections.IEnumerable)))) {
                string[] values = p10.GetValues(p2.Name);
                if (values != null && values.Length > 0) {
                    value = values;
                    return true;
                }
            } else {
                string values = p10[p2.Name];
                if (!string.IsNullOrEmpty(values)) {
                    value = values;
                    return true;
                }
            }

            return TryGetValue_As(p1, p2, p3, out value);
        }
        #endregion

        #region Init
        void Init() {
            if (_tryGetValues != null)
                return;
            if (_datas == null | _datas.Length == 0)
                return;
            _tryGetValues = new TryGetValueFunc[_datas.Length];
        }
        void Init_Build(int index, object data) {
            if (data == null) {
                _tryGetValues[index] = TryGetValue_Empty;
                return;
            }
            //if (TypeExtensions.IsAnonymousType(data.GetType())) {
            //    _tryGetValues[index] = TryGetValue_Any;
            //    return;
            //}
            if (data is System.Collections.Generic.IDictionary<string, object>) {
                _tryGetValues[index] = TryGetValue_IDictionary_string_object;
            } else if (data is System.Collections.IDictionary) {
                _tryGetValues[index] = TryGetValue_IDictionary;
            } else if (data is System.Collections.IList) {
                _tryGetValues[index] = TryGetValue_IList;
            } else if (data is System.Collections.Specialized.NameValueCollection) {
                _tryGetValues[index] = TryGetValue_NameValueCollection;
            } else {
                _tryGetValues[index] = TryGetValue_Any;
            }
        }
        #endregion

        #region PreConvertValue
        object PreConvertValue(object value, System.Type type) {
            if (type != typeof(string) && value is string) {
                string text = ((string)value).Trim();
                if (text.Length > 0 && (
                           (text[0] == '{' && text[text.Length - 1] == '}')
                        || (text[0] == '[' && text[text.Length - 1] == ']')
                    )
                 ) {
                    return JSON.Parse(text) ?? value;
                }
            }
            return value;
        }
        #endregion

        #region TryGetValue
        object TryGetValue(int index, IParameterInfo paramter) {
            object value = null;
            bool finded = false;
            if (_tryGetValues != null) {
                for (int i = 0; i < _tryGetValues.Length; i++) {
                    if (_tryGetValues[i] == null)
                        Init_Build(i, _datas[i]);
                    if (_tryGetValues[i](index, paramter, _datas[i], out value)) {
                        finded = true;
                        if (value != null) {
                            value = _convertValue(PreConvertValue(value, paramter.Type), paramter.Type);
                            break;
                        }
                    }
                }
            }
            if (!finded) {
                var dataBodyAttribute = Symbol.AttributeExtensions.GetCustomAttribute<DataBodyAttribute>(paramter, true);
                if (dataBodyAttribute != null) {
                    var datas = FastWrapper.Combine(_bodyData);
                    value = datas;
                    for (int i = 0; i < _paramters.Count; i++) {
                        datas.Remove(_paramters[i].Name);
                    }
                    if (_otherParamters != null) {
                        for (int i = 0; i < _otherParamters.Count; i++) {
                            datas.Remove(_otherParamters[i].Name);
                        }
                    }
                    if (!string.IsNullOrEmpty(dataBodyAttribute.InvalidKeys)) {
                        datas.RemoveKeys(dataBodyAttribute.InvalidKeys.Split(',', ';', '，', '；', '|', '｜', ' '));
                    }
                    value = _convertValue(PreConvertValue(value, paramter.Type), paramter.Type);
                    //value = _datas[0];
                }
            }
            if (value == null) {

                try {
                    value = paramter.DefaultValue;
                } catch (System.FormatException) { }
                if (value == null || value is System.DBNull) {
                    value = TypeExtensions.DefaultValue(paramter.Type);
                }
            }
            return value;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            _datas = null;
            _bodyData = null;
            _convertValue = null;
            _paramters = null;
            _otherParamters = null;
            _tryGetValues = null;
        }
        #endregion

        #endregion

        #region types
        delegate bool TryGetValueFunc(int index, IParameterInfo paramter, object data, out object value);
        #endregion
    }

    /// <summary>
    /// IParameterInfo 列表。
    /// </summary>
    public class ParameterInfoList : System.Collections.Generic.IList<IParameterInfo> {

        #region fields

        private System.Collections.Generic.List<IParameterInfo> _list;
        private bool _ignoreCase;
        private bool _nameOnly;
        #endregion

        #region properties

        /// <summary>
        /// 获取或设置名称是否唯一。
        /// </summary>
        public bool NameOnly {
            get { return _nameOnly; }
            set { _nameOnly = value; }
        }
        /// <summary>
        /// 获取或设置是否忽略大小写。
        /// </summary>
        public bool IgnoreCase {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        /// <summary>
        /// 获取数量。
        /// </summary>
        public int Count { get { return _list.Count; } }
        bool System.Collections.Generic.ICollection<IParameterInfo>.IsReadOnly { get { return false; } }

        /// <summary>
        /// 获取指定名称的元素（匹配的第一个）。
        /// </summary>
        /// <param name="name">名称，为null或empty，直接返回null。</param>
        /// <returns>返名匹配的元素。</returns>
        public IParameterInfo this[string name] {
            get {
                return Find(name);
            }
        }
        /// <summary>
        /// 获取或设置指定索引器的值。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        /// <returns></returns>
        public IParameterInfo this[int index] {
            get {
                if (index < 0 || index > _list.Count - 1)
                    return null;
                return _list[index];
            }
            set {
                if (index < 0 || index > _list.Count - 1)
                    return;
                if (value == null) {
                    RemoveAt(index);
                    return;
                }
                if (Contains(value))
                    return;
                _list[index] = value;
            }
        }


        #endregion

        #region ctor
        /// <summary>
        /// 创建ParameterInfoList实例。
        /// </summary>
        /// <param name="nameOnly">名称唯一。</param>
        /// <param name="ignoreCase">不区分大小写。</param>
        public ParameterInfoList(bool nameOnly, bool ignoreCase) {
            _list = new System.Collections.Generic.List<IParameterInfo>();
            _nameOnly = nameOnly;
            _ignoreCase = ignoreCase;
        }
        /// <summary>
        /// 创建ParameterInfoList实例。
        /// </summary>
        /// <param name="list">列表。</param>
        /// <param name="nameOnly">名称唯一。</param>
        /// <param name="ignoreCase">不区分大小写。</param>
        public ParameterInfoList(System.Collections.Generic.IList<IParameterInfo> list, bool nameOnly, bool ignoreCase)
            : this(nameOnly, ignoreCase) {
            AddRange(list);
        }
        #endregion



        #region methods 

        #region Add AddRange Insert
        /// <summary>
        /// 添加元素到列表末尾（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="item"></param>
        public void Add(System.Reflection.ParameterInfo item) {
            Add(MethodParameterInfo.As(item));
        }
        /// <summary>
        /// 添加元素到列表末尾（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="item"></param>
        public void Add(System.Reflection.PropertyInfo item) {
            Add(PropertyParameterInfo.As(item));
        }
        /// <summary>
        /// 添加元素到列表末尾（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="item"></param>
        public void Add(System.Reflection.FieldInfo item) {
            Add(FieldParameterInfo.As(item));
        }
        /// <summary>
        /// 添加元素到列表末尾（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="item"></param>
        public void Add(IParameterInfo item) {
            if (item == null)
                return;
            if (Contains(item))
                return;
            _list.Add(item);
        }
        /// <summary>
        /// 批量添加（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="list">自动忽略null和元素null。</param>
        public void AddRange(System.Reflection.ParameterInfo[] list) {
            AddRange(MethodParameterInfo.As(list));
        }
        /// <summary>
        /// 批量添加（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="list">自动忽略null和元素null。</param>
        public void AddRange(System.Reflection.PropertyInfo[] list) {
            AddRange(PropertyParameterInfo.As(list));
        }/// <summary>
         /// 批量添加（名称唯一模式时，按名称匹配是否存在）。
         /// </summary>
         /// <param name="list">自动忽略null和元素null。</param>
        public void AddRange(System.Reflection.FieldInfo[] list) {
            AddRange(FieldParameterInfo.As(list));
        }
        /// <summary>
        /// 批量添加（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="list">自动忽略null和元素null。</param>
        public void AddRange(System.Collections.Generic.IEnumerable<IParameterInfo> list) {
            if (list == null)
                return;
            foreach (var item in list) {
                Add(item);
            }
        }
        /// <summary>
        /// 将元素插入到指定索引位置（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        /// <param name="item">为null时自动忽略。</param>
        public void Insert(int index, IParameterInfo item) {
            if (item == null)
                return;
            if (index < 0) {
                Add(item);
                return;
            }
            if (Contains(item))
                return;
            _list.Insert(index, item);
        }
        /// <summary>
        /// 批量插入（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        /// <param name="list">自动忽略null和元素null。</param>
        public void InsertRange(int index, System.Collections.Generic.IEnumerable<IParameterInfo> list) {
            if (list == null)
                return;
            if (index < 0) {
                AddRange(list);
                return;
            }
            foreach (var item in list) {
                Insert(index++, item);
            }
        }
        #endregion

        #region Contains IndexOf Find
        /// <summary>
        /// 检查指定元素是否存在（名称唯一模式时，按名称匹配）。
        /// </summary>
        /// <param name="item">为null直接返回false。</param>
        /// <returns>返回true表示存在。</returns>
        public bool Contains(IParameterInfo item) {
            if (item == null)
                return false;
            if (_nameOnly) {
                StringComparison comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                return _list.Exists(p => string.Equals(p.Name, item.Name, comparison));
            }
            return _list.Contains(item);
        }
        /// <summary>
        /// 搜索指定的对象的索引位置（名称唯一模式时，按名称匹配）。
        /// </summary>
        /// <param name="item">为null直接返回-1。</param>
        /// <returns>返回搜索到的索引位置，未找到时返回-1。</returns>
        public int IndexOf(IParameterInfo item) {
            if (item == null || _list.Count == 0)
                return -1;
            if (_nameOnly) {
                StringComparison comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                return _list.FindIndex(p => string.Equals(p.Name, item.Name, comparison));
            }
            return _list.IndexOf(item);
        }

        /// <summary>
        /// 搜索指定名称的元素。 
        /// </summary>
        /// <param name="name">名称，为null或emtpy，直接返回null。</param>
        /// <returns></returns>
        public IParameterInfo Find(string name) {
            if (string.IsNullOrEmpty(name))
                return null;
            StringComparison comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return _list.Find(p => string.Equals(p.Name, name, comparison));
        }

        #endregion

        #region Clear Remove RemoveAt RemoveAll
        /// <summary>
        /// 清空列表。
        /// </summary>
        public void Clear() {
            _list.Clear();
        }

        /// <summary>
        /// 移除指定元素（名称唯一模式时，按名称匹配）。
        /// </summary>
        /// <param name="item">为null直接返回false。</param>
        /// <returns>返回是否成功。</returns>
        public bool Remove(IParameterInfo item) {
            if (item == null)
                return false;
            if (_nameOnly) {
                StringComparison comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                return _list.RemoveAll(p => string.Equals(p.Name, item.Name, comparison)) > 0;
            }
            return _list.Remove(item);
        }

        /// <summary>
        /// 移除指定索引处的元素。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        public void RemoveAt(int index) {
            if (index < 0 || index > _list.Count - 1)
                return;
            _list.RemoveAt(index);
        }

        /// <summary>
        /// 批量移除（名称唯一模式时，按名称匹配是否存在）。
        /// </summary>
        /// <param name="list">自动忽略null和元素null。</param>
        public void RemoveAll(System.Collections.Generic.IEnumerable<IParameterInfo> list) {
            if (list == null)
                return;
            foreach (var item in list) {
                Remove(item);
            }
        }
        /// <summary>
        /// 批量移除（按类型匹配）。
        /// </summary>
        /// <param name="list">自动忽略null和元素null。</param>
        public void RemoveAll(params System.Type[] list) {
            if (list == null || list.Length == 0)
                return;
            foreach (var item in list) {
                if (item == null)
                    continue;
                _list.RemoveAll(p => p.Type == item || TypeExtensions.IsInheritFrom(p.Type, item));
            }
        }


        #endregion

        #region CopyTo ToArray ToList
        /// <summary>
        /// 将列表复制到一维数组。
        /// </summary>
        /// <param name="array">目标数组。</param>
        /// <param name="arrayIndex">目标数组起始位置。</param>
        public void CopyTo(IParameterInfo[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 将列表输出复制到新数组中。
        /// </summary>
        /// <returns>返回一个新数组。</returns>
        public IParameterInfo[] ToArray() {
            return _list.ToArray();
        }
        /// <summary>
        /// 将列表输出复制到新列表中。
        /// </summary>
        /// <returns>返回一个新列表。</returns>
        public System.Collections.Generic.List<IParameterInfo> ToList() {
            return LinqHelper.ToList(_list);
        }
        #endregion

        #region GetEnumerator
        /// <summary>
        /// 返回循环访问的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IParameterInfo> GetEnumerator() {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
        #endregion

        #region implicit
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="value">参数对象。</param>
        public static implicit operator ParameterInfoList(IParameterInfo[] value) {
            return new ParameterInfoList(value, true, false);
        }
        /// <summary>
        /// 快速包装。
        /// </summary>
        /// <param name="list">列表。</param>
        public static ParameterInfoList As(System.Collections.Generic.IList<IParameterInfo> list) {
            if (list == null)
                return null;
            return new ParameterInfoList(list, true, false);
        }
        #endregion
        #region explicit
        /// <summary>
        /// 快速转换。
        /// </summary>
        /// <param name="value">实例。</param>
        public static explicit operator IParameterInfo[] (ParameterInfoList value) {
            return value?.ToArray();
        }
        /// <summary>
        /// 快速转换。
        /// </summary>
        /// <param name="value">实例。</param>
        public static explicit operator System.Collections.Generic.List<IParameterInfo>(ParameterInfoList value) {
            return value?.ToList();
        }
        #endregion


        #endregion
    }

}

partial class FastWrapper {

    #region methods

    #region BuildParameters

    #region Type
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="type">类型，为null时直接返回new object[0]</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Type type, params object[] datas) {
        return BuildParameters(type, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="type">方法，为null时直接返回new object[0]</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Type type, TypeExtensions.ConvertValue converValue, params object[] datas) {
        if (type == null)
            return new object[0];
        return BuildParameters(GetProperties(type, BindingFlags.Public | BindingFlags.Instance, true, true), converValue, datas);
    }
    #endregion
    #region PropertyInfo
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Reflection.PropertyInfo[] paramters, params object[] datas) {
        return BuildParameters(paramters, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Reflection.PropertyInfo[] paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(PropertyParameterInfo.As(paramters), converValue, datas)) {
            return binder.ToArray();
        }
    }
    #endregion
    #region MethodInfo
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="methodInfo">方法，为null时直接返回new object[0]</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Reflection.MethodInfo methodInfo, params object[] datas) {
        return BuildParameters(methodInfo, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="methodInfo">方法，为null时直接返回new object[0]</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Reflection.MethodInfo methodInfo, TypeExtensions.ConvertValue converValue, params object[] datas) {
        if (methodInfo == null)
            return new object[0];
        return BuildParameters(methodInfo.GetParameters(), converValue, datas);
    }
    #endregion
    #region ParameterInfo
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Reflection.ParameterInfo[] paramters, params object[] datas) {
        return BuildParameters(paramters, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Reflection.ParameterInfo[] paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(MethodParameterInfo.As(paramters), converValue, datas)) {
            return binder.ToArray();
        }
    }
    #endregion
    #region IParameterInfo
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(IParameterInfo[] paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(paramters, converValue, datas)) {
            return binder.ToArray();
        }
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static object[] BuildParameters(System.Collections.Generic.IList<IParameterInfo> paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(paramters, converValue, datas)) {
            return binder.ToArray();
        }
    }
    #endregion

    #endregion
    #region BuildParameterObject

    #region Type
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="type">类型，为null时直接返回null。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Type type, params object[] datas) {
        return BuildParameterObject(type, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="type">方法，为null时直接返回null。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Type type, TypeExtensions.ConvertValue converValue, params object[] datas) {
        if (type == null)
            return null;
        return BuildParameterObject(GetProperties(type, BindingFlags.Public | BindingFlags.Instance, true, true), converValue, datas);
    }
    #endregion
    #region PropertyInfo
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Reflection.PropertyInfo[] paramters, params object[] datas) {
        return BuildParameterObject(paramters, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Reflection.PropertyInfo[] paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(PropertyParameterInfo.As(paramters), converValue, datas)) {
            return binder.ToObject();
        }
    }
    #endregion
    #region MethodInfo
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="methodInfo">方法，为null时直接返回null。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Reflection.MethodInfo methodInfo, params object[] datas) {
        return BuildParameterObject(methodInfo, null, datas);
    }
    /// <summary>
    /// 构造参数列表。
    /// </summary>
    /// <param name="methodInfo">方法，为null时直接返回null。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数列表。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Reflection.MethodInfo methodInfo, TypeExtensions.ConvertValue converValue, params object[] datas) {
        if (methodInfo == null)
            return null;
        return BuildParameterObject(methodInfo.GetParameters(), converValue, datas);
    }
    #endregion
    #region ParameterInfo
    /// <summary>
    /// 构造参数对象。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Reflection.ParameterInfo[] paramters, params object[] datas) {
        return BuildParameterObject(paramters, null, datas);
    }
    /// <summary>
    /// 构造参数对象。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Reflection.ParameterInfo[] paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(MethodParameterInfo.As(paramters), converValue, datas)) {
            return binder.ToObject();
        }
    }
    #endregion

    #region IParameterInfo
    /// <summary>
    /// 构造参数对象。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(IParameterInfo[] paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(paramters, converValue, datas)) {
            return binder.ToObject();
        }
    }
    /// <summary>
    /// 构造参数对象。
    /// </summary>
    /// <param name="paramters">参数定义列表。</param>
    /// <param name="converValue">转换数据委托，默认为TypeExtensions.Convert。</param>
    /// <param name="datas">可用数据源。</param>
    /// <returns>返回参数对象。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> BuildParameterObject(System.Collections.Generic.IList<IParameterInfo> paramters, TypeExtensions.ConvertValue converValue, params object[] datas) {
        using (var binder = new ParameterInfoBinder(paramters, converValue, datas)) {
            return binder.ToObject();
        }
    }
    #endregion

    #endregion

    #endregion
}