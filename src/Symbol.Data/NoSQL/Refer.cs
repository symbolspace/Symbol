/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 引用关系对象。 [ { "name": { "source": "sourceField" }, "by": { "target": "targetField" } } ]
    /// </summary>
    public class Refer : System.Collections.Generic.IEnumerable<ReferEntry> {

        #region fields
        private System.Collections.Generic.List<ReferEntry> _list;
        private System.Collections.Generic.Dictionary<string, ReferEntry> _list_name;
        private ReferEntry _this;
        #endregion

        #region properties
        /// <summary>
        /// 获取当前数量。
        /// </summary>
        public int Count { get { return _list.Count; } }

        /// <summary>
        /// 获取$this对象。
        /// </summary>
        public ReferEntry This { get { return _this; } }

        /// <summary>
        /// 获取指定索引位置的对象。
        /// </summary>
        /// <param name="index">从0开始的索引值。</param>
        /// <returns>非法访问返回null。</returns>
        public ReferEntry this[int index] {
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
        public ReferEntry this[string name] {
            get {
                ReferEntry item;
                if (string.IsNullOrEmpty(name) || !_list_name.TryGetValue(name, out item))
                    return null;
                return item;
                
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建Refer实例。
        /// </summary>
        public Refer() {
            _list = new System.Collections.Generic.List<ReferEntry>();
            _list_name = new System.Collections.Generic.Dictionary<string, ReferEntry>(System.StringComparer.OrdinalIgnoreCase);

            _this= CreateThis("$self", "id");
            Add_Body(_this);
        }
        #endregion


        #region methods

        #region CreateThis
        ReferEntry CreateThis(string name, string field) {
            return new ReferEntry(name,field);
        }
        #endregion
        #region Add
        /// <summary>
        /// 添加（自动检测重复）。
        /// </summary>
        /// <param name="item">为null时自动忽略。</param>
        public void Add(ReferEntry item) {
            if (item == null)
                return;
            if (item.IsThis) {
                _this.Source = item.Source;
                _this.SourceField = item.SourceField;
                return;
            }
            ReferEntry original = this[item.Name];
            if (original == null) {
                Add_Body(item);
            } else {
                original.Source = item.Source;
                original.SourceField = item.SourceField;
                original.Target = item.Target;
                original.TargetField = item.TargetField;
            }
        }
        void Add_Body(ReferEntry item) {
            _list.Add(item);
            _list_name.Add(item.Name, item);
        }
        #endregion

        #region Parse
        /// <summary>
        /// 解析。 [ { "name": { "source": "sourceField" }, "by": { "target": "targetField" } } ]
        /// </summary>
        /// <param name="value">任意对象。</param>
        /// <returns></returns>
        public static Refer Parse(object value) {
            return new ReferParser().Parse(value);
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 输出数据结构。
        /// </summary>
        /// <returns></returns>
        public object ToObject() {
            return LinqHelper.ToList(LinqHelper.Select(_list, p => p.ToObject()));
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

        #region GetEnumerator
        /// <summary>
        /// 返回循环访问的枚举器。
        /// </summary>
        /// <returns>返回循环访问的枚举器。</returns>
        public IEnumerator<ReferEntry> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
        #endregion

        #region implicit
        /// <summary>
        /// 解析string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Refer(string value) {
            return Parse(value);
        }
        /// <summary>
        /// 解析System.Collections.Generic.Dictionary&lt;string, object&gt;对象
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Refer(System.Collections.Generic.Dictionary<string, object> value) {
            return Parse(value);
        }
        #endregion
        #region explicit
        /// <summary>
        /// 从Refer转为json文本
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator string(Refer value) {
            return value == null ? "" : value.ToJson();
        }
        #endregion

        #region Begin
        /// <summary>
        /// 开始新的引用关系对象
        /// </summary>
        /// <returns></returns>
        public static IReferInstance Begin() {
            return new ReferInstance();
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// Refer解析器。
        /// </summary>
        public class ReferParser {

            #region methods

            #region Parse
            /// <summary>
            /// 解析
            /// </summary>
            /// <param name="value">仅支持string[]、string、{}、object、string(json)</param>
            /// <returns></returns>
            public Refer Parse(object value) {
                Refer result = value as Refer;
                if (result != null)
                    return result;
                return Parse(NodeValue.As(value));
            }
            Refer Parse(NodeValue jsonValue) {
                if (jsonValue == null)
                    return new Refer();
                if (jsonValue.Type == NodeValueTypes.Array) {
                    return Parse_Array(jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                    Refer refer = new Refer();
                    Parse_Dictionary(refer, jsonValue);
                    return refer;
                } else if (jsonValue.Type == NodeValueTypes.Object) {
                    Refer refer = new Refer();
                    Parse_Object(refer, jsonValue);
                    return refer;
                } else if (jsonValue.Type == NodeValueTypes.String) {
                    return Parse_String(jsonValue);
                }
                return new Refer();
            }
            #endregion
            #region Parse_Array
            Refer Parse_Array(NodeValue jsonValue) {
                Refer result = new Refer();
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
            #region Parse_By
            bool Parse_Pairs(string[] pairs,int offset, NodeValue jsonValue) {
                bool b = true;
            lb_Retury:
                if (jsonValue.Type == NodeValueTypes.Object) {
                    foreach (System.Reflection.PropertyInfo propertyInfo in jsonValue.ValueType.GetProperties()) {
                        string key = propertyInfo.Name;
                        object value = propertyInfo.GetValue(jsonValue.Value, new object[0]);
                        pairs[offset] = key;
                        pairs[offset+1] = value as string;
                        goto lb_Result;
                    }
                    return false;
                }
                if (jsonValue.Type == NodeValueTypes.Dictionary) {
                    foreach (object item in (System.Collections.IEnumerable)jsonValue.Value) {
                        string key = FastWrapper.Get(item, "Key") as string;
                        if (string.IsNullOrEmpty(key))
                            continue;
                        object value = FastWrapper.Get(item, "Value");
                        pairs[offset] = key;
                        pairs[offset + 1] = value as string;
                        goto lb_Result;
                    }
                    return false;
                }
                if (jsonValue.Type == NodeValueTypes.Array) {
                    int n = 0;
                    foreach (object element in (System.Collections.IEnumerable)jsonValue.Value) {
                        pairs[offset+ n] = element as string;
                        n++;
                        if (n == 2)
                            goto lb_Result;
                    }
                    return false;
                }
                if (!b)
                    return false;
                if (jsonValue.Type == NodeValueTypes.String) {
                    string text = jsonValue.Value as string;
                    if (string.IsNullOrEmpty(text))
                        return false;
                    if (text[0] == '[' || text[0] == '{') {
                        jsonValue = new NodeValue(Symbol.Serialization.Json.Parse(text));
                        b = false;
                        goto lb_Retury;
                    }
                }
                lb_Result:
                return !string.IsNullOrEmpty(pairs[offset]) && !string.IsNullOrEmpty(pairs[offset + 1]);
            }
            #endregion
            #region Create_Pairs
            ReferEntry Create_Pairs(string[] pairs) {
                if (string.IsNullOrEmpty(pairs[0])
                    || string.IsNullOrEmpty(pairs[1])
                    || string.IsNullOrEmpty(pairs[2]))
                    return null;
                bool isThis = string.Equals(pairs[0], "$this", StringComparison.OrdinalIgnoreCase);
                if (!isThis) {
                    if (string.IsNullOrEmpty(pairs[3])
                    || string.IsNullOrEmpty(pairs[4]))
                        return null;
                }
                return new ReferEntry(pairs[0], pairs[1], pairs[2], pairs[3], pairs[4]);
            }
            #endregion
            #region Parse_Object
            void Parse_Object(Refer refer, NodeValue jsonValue) {
                string[] pairs = new string[5];
                foreach (System.Reflection.PropertyInfo propertyInfo in jsonValue.ValueType.GetProperties()) {
                    string key = propertyInfo.Name;
                    if (string.Equals(key, "by", StringComparison.OrdinalIgnoreCase)) {
                        Parse_Pairs(pairs,3, new NodeValue(propertyInfo.GetValue(jsonValue.Value, new object[0])));
                    } else {
                        pairs[0] = key;
                        Parse_Pairs(pairs, 1, new NodeValue(propertyInfo.GetValue(jsonValue.Value, new object[0])));
                    }
                }
                refer.Add(Create_Pairs(pairs));

            }
            #endregion
            #region Parse_Dictionary
            void Parse_Dictionary(Refer refer, NodeValue jsonValue) {
                string[] pairs = new string[5];
                foreach (object item in (System.Collections.IEnumerable)jsonValue.Value) {
                    string key = FastWrapper.Get(item, "Key") as string;
                    if (string.IsNullOrEmpty(key))
                        continue;
                    object value = FastWrapper.Get(item, "Value");
                    if (string.Equals(key, "by", StringComparison.OrdinalIgnoreCase)) {
                        Parse_Pairs(pairs, 3, new NodeValue(value));
                    } else {
                        pairs[0] = key;
                        Parse_Pairs(pairs, 1, new NodeValue(value));
                    }
                }
                refer.Add(Create_Pairs(pairs));
            }
            #endregion
            #region Parse_String
            Refer Parse_String(NodeValue jsonValue) {
                string value = jsonValue.Value as string;
                if (string.IsNullOrEmpty(value))
                    return new Refer();
                if (value[0] == '[' || value[0] == '{') {
                    return Parse(Symbol.Serialization.Json.Parse(value));
                }
                return new Refer();
            }
            #endregion
            #endregion

        }
        class ReferInstance : IReferInstance {
            private Refer _root;
            private int _asIndex;
            public ReferInstance() {
                _root = new Refer();
                _asIndex = 1;
            }
            public Refer Refer { get { return _root; } }
            

            public string Json(bool formated = false) {
                return _root.ToJson(formated);
            }

            public IReferInstance Refence(string name, string source) {
                return Refence(name, source, "id", "$self", name + "Id");
            }

            public IReferInstance Refence(string name, string source, string targetField) {
                return Refence(name, source, "id", "$self", targetField);
            }

            public IReferInstance Refence(string name, string source, string sourceField, string targetField) {
                return Refence(name, source, sourceField, "$self", targetField);
            }

            public IReferInstance Refence(string name, string source, string sourceField, string target, string targetField) {
                if (string.IsNullOrEmpty(source))
                    return this;
                if (string.IsNullOrEmpty(name))
                    name = "t" + (++_asIndex);
                if (_root[name] != null)
                    return this;
                if (string.IsNullOrEmpty(sourceField))
                    sourceField = "id";
                if (string.IsNullOrEmpty(target))
                    sourceField = "$self";
                if (string.IsNullOrEmpty(targetField))
                    sourceField = name+"Id";
                _root.Add(new ReferEntry(name, source,sourceField,target,targetField));
                return this;
            }
        }

        #endregion

    }
  
}