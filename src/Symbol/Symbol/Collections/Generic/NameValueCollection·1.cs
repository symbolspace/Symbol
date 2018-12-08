/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Symbol.Collections.Generic {

    /// <summary>
    /// 名称与值的集合类（名称不区分大小写）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(false)]
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    //[Symbol.IO.Packing.CustomPackage(typeof(NameValueCollection<>.NameValueCollection_T_CustomPackage))]
    public class NameValueCollection<T> : 
        IDictionary<string, T>, 
        System.Collections.IDictionary
#if !netcore
        , System.Runtime.Serialization.ISerializable
        , System.Runtime.Serialization.IDeserializationCallback
#endif
        //, IXmlSerializable
    {
        #region fields
        private Dictionary<string, T> _dictionary;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置属性反射处理器。
        /// </summary>
        public PropertyDescrtionHander<System.ComponentModel.PropertyDescriptor, T, T> PropertyDescriptor { get; set; }
        /// <summary>
        /// 获取或设置当值为null时的默认值。
        /// </summary>
        public T NullValue { get; set; }
        /// <summary>
        /// 获取当前集合成员数。
        /// </summary>
        public int Count {get {return this._dictionary.Count;}}
        /// <summary>
        /// 获取或设置名称对应的值。
        /// </summary>
        /// <param name="key">唯一名称，不区分大小写。</param>
        /// <returns>返回对应的值。</returns>
        public T this[string key] {
            get {
                if (key == null)
                    return default(T);
                T obj2;
                this.TryGetValue(key, out obj2);
                return obj2;
            }
            set {this._dictionary[key] = value;}
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建 NameValueCollection 的实例。
        /// </summary>
        public NameValueCollection() {
            CreateDictionary(null);
        }
        /// <summary>
        /// 创建 NameValueCollection 的实例。
        /// </summary>
        /// <param name="comparer">比较器。</param>
        public NameValueCollection(IEqualityComparer<string> comparer) {
            CreateDictionary(comparer);
        }
        /// <summary>
        /// 创建 NameValueCollection 的实例。
        /// </summary>
        /// <param name="dictionary">附加的字典集合。</param>
        public NameValueCollection(IDictionary<string, T> dictionary) {
            CreateDictionary(null);
            SetValues(dictionary);
        }
        /// <summary>
        /// 创建 NameValueCollection 的实例。
        /// </summary>
        /// <param name="values">附加的任意数据，详细说明请参考SetValues方法。</param>
        public NameValueCollection(object values)
            {
            if (values is IEqualityComparer<string> comparer) {
                CreateDictionary(values as IEqualityComparer<string>);
            } else {
                SetValues(values);
            }
        }
        /// <summary>
        /// 创建 NameValueCollection 的实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">序列化上下文。</param>
        protected NameValueCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            _dictionary= FastWrapper.CreateInstance<Dictionary<string,T>>(info,context);
        }
        #endregion

        #region methods
        void CreateDictionary(IEqualityComparer<string> comparer) {
            if (_dictionary != null)
                return;
            if (comparer == null)
                comparer = StringComparer.OrdinalIgnoreCase;
            _dictionary = new Dictionary<string, T>(comparer);
        }
        #region Add
        /// <summary>
        /// 添加一项（无法添加重复的项，建议直接用 this[key]=value）。
        /// </summary>
        /// <param name="key">唯一名称，不区分大小写</param>
        /// <param name="value"></param>
        public void Add(string key, T value) {
            this._dictionary.Add(key, value);
        }
        #endregion
        #region SetValues
        /// <summary>
        /// 设置当前集合中的值（不会清空现有的值。）
        /// </summary>
        /// <param name="values">支持类型。IDictionary&lt;string, Tgt;、System.Collections.Specialized.NameValueCollection、匿名对象、普通类对象。</param>
        public void SetValues(object values) {
            lb_Retry:
            if (values == null) {
                CreateDictionary(null);
                return;
            }
            if (values is IDictionary<string, T>) {
                CreateDictionary(null);
                foreach (KeyValuePair<string, T> item in (IDictionary<string, T>)values) {
                    this[item.Key] = item.Value;
                }
            } else if (values is System.Collections.Specialized.NameValueCollection) {
                CreateDictionary(null);
                System.Collections.Specialized.NameValueCollection collection = (System.Collections.Specialized.NameValueCollection)values;
                foreach (string item in collection.Keys) {
                    this[item] = ConvertValue(collection[item]);
                }
            } else if(values is string){
                string value =( values as string)?.Trim();
                if (string.IsNullOrEmpty(value)) {
                    CreateDictionary(null);
                    return;
                }
                if (value[0] == '{') {
                    CreateDictionary(StringComparer.Ordinal);
                    values = JSON.Parse(value);
                    goto lb_Retry;
                } else {
                    CreateDictionary(null);
                }
            } else {
                CreateDictionary(StringComparer.Ordinal);
                foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(values)) {
                    T obj2 = ConvertValue(descriptor.GetValue(values));
                    if (PropertyDescriptor != null)
                        obj2 = PropertyDescriptor(descriptor, obj2);
                    this[descriptor.Name] = obj2;
                }
            }
        }
        #endregion
        #region ConvertValue
        T ConvertValue(object value) {
            if (typeof(T) == typeof(object))
                return (T)value;
            return TypeExtensions.Convert<T>(value);
        }
        #endregion
        #region Clear
        /// <summary>
        /// 清空集合。
        /// </summary>
        public void Clear() {
            this._dictionary.Clear();
        }
        #endregion
        #region ContainsKey
        /// <summary>
        /// 检查是否存在指定的名称。
        /// </summary>
        /// <param name="key">唯一名称，不区分大小写。</param>
        /// <returns>返回检查结果。</returns>
        public bool ContainsKey(string key) {
            return this._dictionary.ContainsKey(key);
        }
        #endregion
        #region ContainsValue
        /// <summary>
        /// 检查是否存在指定的值。
        /// </summary>
        /// <param name="value">需要检查的值。</param>
        /// <returns>返回检查结果。</returns>
        public bool ContainsValue(T value) {
            return this._dictionary.ContainsValue(value);
        }
        #endregion
        #region GetEnumerator
        /// <summary>
        /// 获取可循环的Enumerator。
        /// </summary>
        /// <returns>返回一个Enumerator。</returns>
        public Dictionary<string, T>.Enumerator GetEnumerator() {
            return this._dictionary.GetEnumerator();
        }
        #endregion
        #region Remove
        /// <summary>
        /// 移除指定名称的项。
        /// </summary>
        /// <param name="key">唯一名称，不区分大小写。</param>
        /// <returns>如果存在并移除成功返回true，反之不存在。</returns>
        public bool Remove(string key) {
            return this._dictionary.Remove(key);
        }
        #endregion
        #region RemoveKeys
        /// <summary>
        /// 移除指定的key(批量操作)。
        /// </summary>
        /// <param name="keys">名称列表,不区分大小写。</param>
        /// <returns>返回移除成功的个数。</returns>
        public int RemoveKeys(params string[] keys) {
            if (keys == null || keys.Length == 0)
                return 0;
            int count = 0;
            for (int i = 0; i < keys.Length; i++) {
                if (string.IsNullOrEmpty(keys[i]))
                    continue;
                if (_dictionary.Remove(keys[i]))
                    count++;
            }
            return count;
        }
        #endregion

        void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item) {
            ((ICollection<KeyValuePair<string, T>>)_dictionary).Add(item);
        }

        bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item) {
            return ((ICollection<KeyValuePair<string, T>>)_dictionary).Contains(item);
        }

        void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex) {
            ((ICollection<KeyValuePair<string, T>>)_dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item) {
            return ((ICollection<KeyValuePair<string, T>>)_dictionary).Remove(item);
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator() {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        /// <summary>
        /// 尝试获取值。
        /// </summary>
        /// <param name="key">唯一名称，不区分大小写。</param>
        /// <param name="value">输出值。</param>
        /// <returns>如果成功找到返回true，反之不存在。</returns>
        public bool TryGetValue(string key, out T value) {
            return this._dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取Key
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public string GetKey(T value) {
            foreach (var item in _dictionary) {
                if (EqualityComparer<T>.Default.Equals(item.Value, value))
                    return item.Key;
            }
            return null;
        }
        /// <summary>
        /// 获取所有名称列表。
        /// </summary>
        public Dictionary<string, T>.KeyCollection Keys {
            get {
                return this._dictionary.Keys;
            }
        }
        /// <summary>
        /// 获取所有名称列表（数组）。
        /// </summary>
        public string[] AllKeys {
            get {
                return LinqHelper.ToArray(_dictionary.Keys);
            }
        }

        bool ICollection<KeyValuePair<string, T>>.IsReadOnly {
            get {
                return ((ICollection<KeyValuePair<string, T>>)_dictionary).IsReadOnly;
            }
        }

        ICollection<string> IDictionary<string, T>.Keys {
            get {
                return this._dictionary.Keys;
            }
        }

        ICollection<T> IDictionary<string, T>.Values {
            get {
                return this._dictionary.Values;
            }
        }
        /// <summary>
        /// 获取所有的值列表。
        /// </summary>
        public Dictionary<string, T>.ValueCollection Values {
            get {
                return this._dictionary.Values;
            }
        }

        #region ISerializable 成员
#if !netcore
        /// <summary>
        /// 从序列化信息中获取数据。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">序列化上下文。</param>
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            _dictionary.GetObjectData(info, context);
        }
#endif

        #endregion

        #region IDeserializationCallback 成员
#if !netcore
        /// <summary>
        /// 当反序列化时回调，
        /// </summary>
        /// <param name="sender">调用者。</param>
        public void OnDeserialization(object sender) {
            _dictionary.OnDeserialization(sender);
        }
#endif

        #endregion

        #region IXmlSerializable 成员

        //System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
        //    return new System.Xml.Schema.XmlSchema() { Version = "1.0" };
        //}
        //void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
        //    //IDictionaryHelper.FromBodyXml(this, reader.ReadString());
        //    //IDictionaryHelper.FromBodyXml(this, reader.ReadInnerXml());
        //    var elementName = this.GetType().Name;
        //    if (!reader.IsStartElement(elementName))
        //        return;
        //    reader.ReadStartElement(elementName);
        //    IDictionaryHelper.FromXml(this, reader);
        //    reader.ReadEndElement();
        //}

        //void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
        //    //writer.WriteString(IDictionaryHelper.ToBodyXml(this));
        //    IDictionaryHelper.ToXml(this, writer);
        //}

        #endregion

        /// <summary>
        /// 将一个对象尝试转换为 NameValueCollection&lt;T&gt;。
        /// </summary>
        /// <param name="values">任意对象，可以为null。</param>
        /// <returns>返回转换结果，永远不会返回null。</returns>
        public static NameValueCollection<T> As(object values) {
            NameValueCollection<T> result = values as NameValueCollection<T>;
            if (result == null)
                result = new NameValueCollection<T>(values);
            return result;
        }

        #endregion

        #region IDictionary 成员

        void System.Collections.IDictionary.Add(object key, object value) {
            System.Collections.IDictionary dic = (System.Collections.IDictionary)_dictionary;
            if (dic.Contains(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        bool System.Collections.IDictionary.Contains(object key) {
            return ((System.Collections.IDictionary)_dictionary).Contains(key);
        }

        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() {
            return ((System.Collections.IDictionary)_dictionary).GetEnumerator();
        }

        bool System.Collections.IDictionary.IsFixedSize {
            get { return ((System.Collections.IDictionary)_dictionary).IsFixedSize; }
        }

        bool System.Collections.IDictionary.IsReadOnly {
            get { return ((System.Collections.IDictionary)_dictionary).IsReadOnly; }
        }

        System.Collections.ICollection System.Collections.IDictionary.Keys {
            get { return ((System.Collections.IDictionary)_dictionary).Keys; }
        }

        void System.Collections.IDictionary.Remove(object key) {
            ((System.Collections.IDictionary)_dictionary).Remove(key);
        }

        System.Collections.ICollection System.Collections.IDictionary.Values {
            get { return ((System.Collections.IDictionary)_dictionary).Values; }
        }

        object System.Collections.IDictionary.this[object key] {
            get {
                System.Collections.IDictionary dic = (System.Collections.IDictionary)_dictionary;
                if (dic.Contains(key))
                    return dic[key];
                return null;
            }
            set {
                System.Collections.IDictionary dic = (System.Collections.IDictionary)_dictionary;
                if (dic.Contains(key))
                    dic[key] = value;
                else
                    dic.Add(key, value);
            }
        }

        #endregion

        #region ICollection 成员

        void System.Collections.ICollection.CopyTo(Array array, int index) {
            ((System.Collections.IDictionary)_dictionary).CopyTo(array, index);
        }

        bool System.Collections.ICollection.IsSynchronized {
            get { return ((System.Collections.IDictionary)_dictionary).IsSynchronized; }
        }

        object System.Collections.ICollection.SyncRoot {
            get { return ((System.Collections.IDictionary)_dictionary).SyncRoot; }
        }

        #endregion

        #region types
        //class NameValueCollection_T_CustomPackage : Symbol.IO.Packing.ICustomPackage {
        //    #region ICustomPackage 成员

        //    public byte[] Save(object instance) {
        //        Type type = instance.GetType();
        //        Type t1 = type.GetGenericArguments()[0];
        //        System.Collections.IEnumerable collection = (System.Collections.IEnumerable)instance;
        //        IO.Packing.TreePackage package = new IO.Packing.TreePackage();
        //        package.Attributes.Add("T1", t1.AssemblyQualifiedName);
        //        foreach (object item in collection) {
        //            string key = (string)FastWrapper.Get(item, "Key");
        //            object value = FastWrapper.Get(item, "Value");
        //            package.Add(key, value);
        //        }
        //        return package.Save();
        //    }

        //    public object Load(byte[] buffer) {
        //        IO.Packing.TreePackage package = IO.Packing.TreePackage.Load(buffer);
        //        Type t1 = FastWrapper.GetWarpperType((string)package.Attributes["T1"]);
        //        Type type = typeof(NameValueCollection<>).MakeGenericType(t1);
        //        object result = FastWrapper.CreateInstance(type);
        //        foreach (string key in package.Keys) {
        //            FastWrapper.MethodInvoke(type, "Add", result, key, package[key]);
        //        }
        //        return result;
        //    }

        //    #endregion
        //}

        /// <summary>
        /// 属性反射处理器。
        /// </summary>
        /// <typeparam name="T1">类型1</typeparam>
        /// <typeparam name="T2">类型2</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <returns>返回值。</returns>
        public delegate TResult PropertyDescrtionHander<T1, T2, TResult>(T1 arg1, T2 arg2);
        #endregion
    }


}
partial class FastWrapper {

    #region methods

    #region As
    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="values">可空，实体类/匿名类/字典/JSON</param>
    /// <returns>不会有null。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> As(object values) {
        return Symbol.Collections.Generic.NameValueCollection<object>.As(values);
    }
    #endregion
    #region Combine
    /// <summary>
    /// 将多个对象组合在一起（仅限第一层属性）
    /// </summary>
    /// <param name="objects">用于组装的对象列表，实体类/匿名类/字典/JSON</param>
    /// <returns>不会有null。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> Combine(params object[] objects) {
        return Combine((System.Collections.IEnumerable)objects);
    }
    /// <summary>
    /// 将多个对象组合在一起（仅限第一层属性）
    /// </summary>
    /// <param name="objects">用于组装的对象列表，实体类/匿名类/字典/JSON</param>
    /// <returns>不会有null。</returns>
    public static Symbol.Collections.Generic.NameValueCollection<object> Combine(System.Collections.IEnumerable objects) {
        Symbol.Collections.Generic.NameValueCollection<object> values = As(null);
        if (objects != null) {
            foreach (var item in objects) {
                values.SetValues(item);
            }
        }
        return values;
    }
    #endregion

    #endregion

}
