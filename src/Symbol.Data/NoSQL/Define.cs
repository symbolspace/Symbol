/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Collections;
using System.Collections.Generic;
namespace Symbol.Data.NoSQL {



    /*
     [
        "*"// ignore 
        "t.*",//auto rename
        {"a.id":"t.id"},//{ a:{ id:  "$t.id" } }
        { "a":{ "id": { "t":"id" } } },//format
        { "a.count":{ "$count":1 } },
        { "a.money":{"$sum":"t.money" } },
        { "
     ]
    */
    /// <summary>
    /// 定义对象。
    /// </summary>
    /// <remarks></remarks>
    public class Define : DefineEntryChildren {


        #region ctor
        /// <summary>
        /// 创建 Define 实例。
        /// </summary>
        public Define() {
        }
        #endregion


        #region methods

        #region Parse
        /// <summary>
        /// 解析。 
        /// </summary>
        /// <param name="value">任意对象。</param>
        /// <returns></returns>
        public static Define Parse(object value) {
            return new DefineParser().Parse(value);
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 输出数据结构。
        /// </summary>
        /// <returns></returns>
        public object ToObject() {
            return ToObjectList();
        }
        #endregion
        #region ToJson
        /// <summary>
        /// 输出JSON字符串。
        /// </summary>
        /// <returns></returns>
        public string ToJson() {
            return Symbol.Serialization.Json.ToString(ToObject());
        }
        /// <summary>
        /// 输出JSON字符串。
        /// </summary>
        /// <param name="formated">是否格式化。</param>
        /// <returns></returns>
        public string ToJson(bool formated) {
            return Symbol.Serialization.Json.ToString(ToObject(), false, formated);
        }
        #endregion
        #region ToString
        /// <summary>
        /// 输出JSON字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToJson();
        }
        #endregion

        #region implicit
        /// <summary>
        /// 解析string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Define(string value) {
            return Parse(value);
        }
        /// <summary>
        /// 解析System.Collections.Generic.Dictionary&lt;string, object&gt;对象
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Define(System.Collections.Generic.Dictionary<string, object> value) {
            return Parse(value);
        }
        #endregion
        #region explicit
        /// <summary>
        /// 从Refer转为json文本
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator string(Define value) {
            return value == null ? "" : value.ToJson();
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// Define解析器。
        /// </summary>
        public class DefineParser {

            #region methods

            #region Parse
            /// <summary>
            /// 解析
            /// </summary>
            /// <param name="value">仅支持string[]、string、{}、object、string(json)</param>
            /// <returns></returns>
            public Define Parse(object value) {
                Define result = value as Define;
                if (result != null)
                    return result;
                return Parse(NodeValue.As(value));
            }
            Define Parse(NodeValue jsonValue, Define define=null) {
                if (jsonValue == null)
                    return define??new Define();

                if (jsonValue.Type == NodeValueTypes.Array) {
                    return Parse_Array(jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                    Define refer = new Define();
                    Parse_Dictionary(refer, jsonValue);
                    return refer;
                } else if (jsonValue.Type == NodeValueTypes.Object) {
                    Define refer = new Define();
                    Parse_Object(refer, jsonValue);
                    return refer;
                } else if (jsonValue.Type == NodeValueTypes.String) {
                    return Parse_String(jsonValue);
                }
                return new Define();
            }
            #endregion
            #region Parse_Array
            Define Parse_Array(NodeValue jsonValue) {
                Define result = new Define();
                foreach (object element in (System.Collections.IEnumerable)jsonValue.Value) {
                    NodeValue item = new NodeValue(element);
                    bool b = true;
                    lb_Retury:
                    if (item.Type == NodeValueTypes.Object) {
                        Parse_Object(result, item);
                        continue;
                    }
                    if (item.Type == NodeValueTypes.Dictionary) {
                        Parse_Dictionary(result, item);
                        continue;
                    }
                    if (!b)
                        continue;
                    if (item.Type == NodeValueTypes.String) {
                        string text = item.Value as string;
                        if (string.IsNullOrEmpty(text))
                            continue;
                        Parse_String(item, result);
                        if (text[0] == '[' || text[0] == '{') {
                            item = new NodeValue(Symbol.Serialization.Json.Parse(text));
                            b = false;
                            goto lb_Retury;
                        }
                        continue;
                    }

                }
                return result;
            }
            #endregion
            #region Parse_Object
            void Parse_Object(Define define, NodeValue jsonValue) {
                //string[] pairs = new string[5];
                //foreach (System.Reflection.PropertyInfo propertyInfo in jsonValue.ValueType.GetProperties()) {
                //    string key = propertyInfo.Name;
                //    if (string.Equals(key, "by", StringComparison.OrdinalIgnoreCase)) {
                //        Parse_Pairs(pairs, 3, new JsonValue(propertyInfo.GetValue(jsonValue.Value, new object[0])));
                //    } else {
                //        pairs[0] = key;
                //        Parse_Pairs(pairs, 1, new JsonValue(propertyInfo.GetValue(jsonValue.Value, new object[0])));
                //    }
                //}
                //define.Add(Create_Pairs(pairs));

            }
            #endregion
            #region Parse_Dictionary
            void Parse_Dictionary(Define define, NodeValue jsonValue) {
                //string[] pairs = new string[5];
                //foreach (object item in (System.Collections.IEnumerable)jsonValue.Value) {
                //    string key = FastWrapper.Get(item, "Key") as string;
                //    if (string.IsNullOrEmpty(key))
                //        continue;
                //    object value = FastWrapper.Get(item, "Value");
                //    if (string.Equals(key, "by", StringComparison.OrdinalIgnoreCase)) {
                //        Parse_Pairs(pairs, 3, new JsonValue(value));
                //    } else {
                //        pairs[0] = key;
                //        Parse_Pairs(pairs, 1, new JsonValue(value));
                //    }
                //}
                //define.Add(Create_Pairs(pairs));
            }
            #endregion
            #region Parse_String
            Define Parse_String(NodeValue jsonValue, Define define = null) {
                string value = jsonValue.Value as string;
                if (string.IsNullOrEmpty(value))
                    return define ?? new Define();
                //if (value[0] == '[' || value[0] == '{') {
                //    return Parse(Symbol.Serialization.Json.Parse(value), define);
                //}
                if (define == null)
                    define = new Define();
                Parse_String(define, value);
                return define;
            }
            void Parse_String(Define define, string value) {
                if (value == "*") {
                    define.Add(new DefineEntry("*", null, DefineValueTypes.Field));
                    return;
                }
                string[] pair = value.Split('=');
                if (pair.Length == 2) {
                    DefineEntry item = new DefineEntry(pair[0], pair[1], DefineValueTypes.Field);
                    ParseValue(item);
                    define.Add(item);
                    return;
                }
                {
                    DefineEntry item = new DefineEntry(pair[0], pair[0], DefineValueTypes.Field);
                    ParseValue(item);
                    define.Add(item);
                }
            }
            #endregion
            #region ParseValue
            void ParseValue(DefineEntry item) {

            }
            #endregion

            #endregion
        }
        #endregion
    }
    /// <summary>
    /// 定义对象Entry 子集接口。
    /// </summary>
    public interface IDefineEntryChildren : System.Collections.Generic.IEnumerable<DefineEntry> {
        /// <summary>
        /// 获取当前数量。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 获取指定索引位置的对象。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        /// <returns>非法访问返回null。</returns>
        DefineEntry this[int index] { get; }
        /// <summary>
        /// 获取指定名称的对象。
        /// </summary>
        /// <param name="name">名称，为null或空直接返回null。</param>
        /// <returns>不存在返回null。</returns>
        DefineEntry this[string name] { get; }

        #region Add
        /// <summary>
        /// 添加（自动检测重复）。
        /// </summary>
        /// <param name="item"></param>
        void Add(DefineEntry item);
        #endregion
    }
    /// <summary>
    /// 定义对象Entry子集。
    /// </summary>
    public class DefineEntryChildren : IDefineEntryChildren {
        #region fields
        private System.Collections.Generic.List<DefineEntry> _list;
        private System.Collections.Generic.Dictionary<string, DefineEntry> _list_name;
        #endregion

        #region properties
        /// <summary>
        /// 获取当前数量。
        /// </summary>
        public int Count { get { return _list.Count; } }
        /// <summary>
        /// 获取指定索引位置的对象。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        /// <returns>非法访问返回null。</returns>
        public DefineEntry this[int index] {
            get {
                if (index < -1 || index > _list.Count - 1)
                    return null;
                return _list[index];
            }
        }
        /// <summary>
        /// 获取指定名称的对象。
        /// </summary>
        /// <param name="name">名称，为null或空直接返回null。</param>
        /// <returns>不存在返回null。</returns>
        public DefineEntry this[string name] {
            get {
                DefineEntry item;
                if (string.IsNullOrEmpty(name) || !_list_name.TryGetValue(name, out item))
                    return null;
                return item;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建 DefineEntryChildren 实例。
        /// </summary>
        public DefineEntryChildren() {
            _list = new System.Collections.Generic.List<DefineEntry>();
            _list_name = new System.Collections.Generic.Dictionary<string, DefineEntry>(System.StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region methods

        #region Add
        /// <summary>
        /// 添加（自动检测重复）。
        /// </summary>
        /// <param name="item"></param>
        public void Add(DefineEntry item) {
#warning add
        }
        #endregion

        #region GetEnumerator
        /// <summary>
        /// 返回循环访问的枚举器。
        /// </summary>
        /// <returns>返回循环访问的枚举器。</returns>
        public IEnumerator<DefineEntry> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 输出数据结构。
        /// </summary>
        /// <returns></returns>
        protected void ToObject(System.Collections.Generic.IDictionary<string,object> o) {
            if (_list.Count == 0)
                return;
            o.Add("items", ToObjectList());
        }
        /// <summary>
        /// 输出数据结构。
        /// </summary>
        /// <returns></returns>
        protected System.Collections.Generic.List<object> ToObjectList() {
            return LinqHelper.ToList(LinqHelper.Select(_list, p => p.ToObject()));
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 定义对象Entry。
    /// </summary>
    public class DefineEntry : DefineEntryChildren {
        #region fields
        //private static readonly DefineEntry _any = new DefineEntry("*", null, DefineValueTypes.Field);
        private bool _isAny;
        private string _name;
        private string[] _names;
        private object _value;
        private DefineValueTypes _valueType;
        #endregion

        #region properties

        /// <summary>
        /// 获取是否为任意定义。
        /// </summary>
        public bool IsAny { get { return _isAny; } }
        /// <summary>
        /// 获取或设置名称。
        /// </summary>
        public string Name {
            get { return _name; }
            private set {
                CommonException.CheckArgumentNull(value, "value");
                _name = value;
                _names = value.Split('.');
            }
        }
        /// <summary>
        /// 获取或设置值。
        /// </summary>
        public object Value {
            get { return _value; }
            set{
                _value = value;
            }
        }
        /// <summary>
        /// 获取或设置值类型。
        /// </summary>
        public DefineValueTypes ValueType {
            get { return _valueType; }
            set{
                _valueType = value;
            }
        }


        #endregion

        #region ctor
        /// <summary>
        /// 创建 DefineEntry 实例。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="value">值。</param>
        /// <param name="valueType">值类型。</param>
        public DefineEntry(string name, object value, DefineValueTypes valueType= DefineValueTypes.Field) {
            Name = name;
            Value = value;
            _valueType = valueType;
            _isAny = name == "*";
        }
        #endregion

        #region methods

        #region ToObject
        /// <summary>
        /// 输出数据结构。 
        /// </summary>
        /// <returns></returns>
        public object ToObject() {
            System.Collections.Generic.IDictionary<string, object> o = new System.Collections.Generic.Dictionary<string, object>();
            o.Add(_name, _value);
            ToObject(o);
            return o;
        }
        #endregion
        #region ToJson
        /// <summary>
        /// 输出JSON字符串。 
        /// </summary>
        /// <returns>返回JSON字符串。</returns>
        public string ToJson() {
            return ToJson(false);
        }
        /// <summary>
        /// 输出JSON字符串。
        /// </summary>
        /// <param name="formated">是否格式化。</param>
        /// <returns>返回JSON字符串。</returns>
        public string ToJson(bool formated) {
            return Symbol.Serialization.Json.ToString(ToObject(), false, formated);
        }
        #endregion
        #region ToString
        /// <summary>
        /// 输出JSON字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToJson();
        }
        #endregion

        #endregion
    }
}