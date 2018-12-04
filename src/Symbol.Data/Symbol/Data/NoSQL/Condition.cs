/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 规则对象
    /// </summary>
    public class Condition : IConditionSetter {

        #region fields
        private ConditionCollection _children;
        private string _name;
        private ConditionTypes _type;
        private object _value;
        private Condition _parent;
        #endregion

        #region properties
        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// 获取类型
        /// </summary>
        public ConditionTypes Type { get { return _type; } }
        /// <summary>
        /// 获取值
        /// </summary>
        public object Value { get { return _value; } }
        /// <summary>
        /// 获取上级
        /// </summary>
        public Condition Parent { get { return _parent; } }
        void IConditionSetter.SetParent(Condition parent) {
            _parent = parent;
        }
        /// <summary>
        /// 获取子集
        /// </summary>
        public ConditionCollection Children { get { return _children; } }
        #endregion

        #region ctor
        /// <summary>
        /// 创建Condition实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <param name="value">值</param>
        public Condition(string name, ConditionTypes type, object value = null) {
            _children = new ConditionCollection(this);
            _name = name;
            _type = type;
            _value = value;
        }
        #endregion

        #region methods

        #region GetNames
        /// <summary>
        /// 获取名称子集列表（分割符为“.”）。
        /// </summary>
        /// <returns>返回名称的子集列表，如a.b 为 ["a","b"] 。</returns>
        public string[] GetNames() {
            return GetNames(".");
        }
        /// <summary>
        /// 获取名称子集列表。
        /// </summary>
        /// <param name="spliter">指定分割符，为null或空时自动为“.”。</param>
        /// <returns>返回名称的子集列表，如a.b 为 ["a","b"] 。</returns>
        public string[] GetNames(string spliter) {
            if (string.IsNullOrEmpty(_name))
                return new string[0];
            if (string.IsNullOrEmpty(spliter))
                spliter = ".";
            return _name.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion

        #region Parse
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Condition Parse(object value) {
            return new ConditionParser().Parse(value);
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 输出数据结构。
        /// </summary>
        /// <returns></returns>
        public object ToObject() {
            System.Collections.Generic.Dictionary<string, object> root = new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
            ToObject_Children(root, Children);
            return root;
        }
        void ToObject_Children(System.Collections.Generic.Dictionary<string, object> parent, ConditionCollection list) {
            foreach (Condition item in list) {
                if (item.Children.Count > 0) {
                    if (item.Children.IsArray) {
                        System.Collections.Generic.List<object> values = new System.Collections.Generic.List<object>();
                        parent.Add(item.Name, values);
                        foreach (Condition p in item.Children) {
                            System.Collections.Generic.Dictionary<string, object> root2 = new System.Collections.Generic.Dictionary<string, object>();
                            root2.Add(p.Name, p.ToObject());
                            values.Add(root2);
                        }
                    } else {
                        System.Collections.Generic.Dictionary<string, object> values = new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
                        parent.Add(item.Name, values);
                        ToObject_Children(values, item.Children);
                    }
                } else {
                    parent.Add(item.Name, item.Value);
                }
            }
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
        public static implicit operator Condition(string value) {
            return Parse(value);
        }
        /// <summary>
        /// 解析System.Collections.Generic.Dictionary&lt;string, object&gt;对象
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Condition(System.Collections.Generic.Dictionary<string, object> value) {
            return Parse(value);
        }
        #endregion
        #region explicit
        /// <summary>
        /// 从Condition转为json文本
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator string(Condition value) {
            return value == null ? "" : value.ToJson();
        }
        ///// <summary>
        ///// 从Condition转为System.Collections.Generic.Dictionary&lt;string, object&gt;
        ///// </summary>
        ///// <param name="value"></param>
        //public static explicit operator System.Collections.Generic.Dictionary<string, object>(Condition value) {
        //    return value == null
        //        ? new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase)
        //        : value.ToObject();
        //}
        #endregion

        #region Begin
        /// <summary>
        /// 开始新的规则对象
        /// </summary>
        /// <returns></returns>
        public static IConditionInstance Begin() {
            return new ConditionInstance();
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// Condition解析器
        /// </summary>
        public class ConditionParser {

            #region methods

            #region Parse
            /// <summary>
            /// 解析
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public Condition Parse(object value) {
                Condition result = value as Condition;
                if (result != null)
                    return result;
                result = new Condition("$root", ConditionTypes.Root);
                Parse(result, NodeValue.As(value));
                return result;
            }
            void Parse(Condition parent, object value) {
                Next(parent, NodeValue.As(value));
            }
            #endregion
            #region Next
            void Next(Condition parent, NodeValue jsonValue) {
                if (jsonValue.Type == NodeValueTypes.Dictionary) {
                    Next_Dictionary(parent, jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.Object) {
                    Next_Object(parent, jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.String) {
                    Next_String(parent, jsonValue);
                }
            }
            #endregion
            #region Next_Dictionary
            void Next_Dictionary(Condition parent, NodeValue jsonValue) {
                foreach (object item in (System.Collections.IEnumerable)jsonValue.Value) {
                    string key = FastWrapper.Get(item, "Key") as string;
                    if (string.IsNullOrEmpty(key))
                        continue;
                    ConditionTypes type = GetType(key);
                    if (type == ConditionTypes.Root)
                        continue;
                    object value = FastWrapper.Get(item, "Value");
                    Condition pair = new Condition(key, type);
                    if (Check_Pair(pair, value)) {
                        parent.Children.Add(pair);
                    }
                }
            }
            #endregion
            #region Next_Object
            void Next_Object(Condition parent, NodeValue jsonValue) {
                foreach (System.Reflection.PropertyInfo propertyInfo in jsonValue.ValueType.GetProperties()) {
                    string key = propertyInfo.Name;
                    ConditionTypes type = GetType(key);
                    if (type == ConditionTypes.Root)
                        continue;
                    object value = propertyInfo.GetValue(jsonValue.Value, new object[0]);
                    Condition pair = new Condition(key, type);
                    if (Check_Pair(pair, value)) {
                        parent.Children.Add(pair);
                    }
                }
            }
            #endregion
            #region Next_Array
            void Next_Array(Condition parent, NodeValue jsonValue, NodeValueTypes[] types) {
                foreach (object item in (System.Collections.IEnumerable)jsonValue.Value) {
                    NodeValue json = NodeValue.As(item);
                    if (System.Array.IndexOf(types, json.Type) > -1) {
                        Next(parent, json);
                    }
                }
            }
            #endregion
            #region Next_String
            void Next_String(Condition parent, NodeValue jsonValue) {
                string value = jsonValue.Value as string;
                if (string.IsNullOrEmpty(value))
                    return;
                if (value[0] == '[' || value[0] == '{') {
                    Parse(parent, Symbol.Serialization.Json.Parse(value));
                }
                if (value[0] == '$' || value[0]=='!') {
                    value = "{ \"" + value + "\":null }";
                    Parse(parent, Symbol.Serialization.Json.Parse(value));
                }
            }
            #endregion
            #region Check_Pair
            bool Check_Pair(Condition pair, object value) {
                NodeValue jsonValue = new NodeValue(value);
                if (pair.Type == ConditionTypes.Field) {
                    return Check_Pair_Field(pair, jsonValue);
                } else if (pair.Type == ConditionTypes.Logical) {
                    bool b = Check_Pair_Logical(pair, jsonValue);
                    if (b && pair._name[0] != '$')
                        pair._name = "$" + pair._name;
                    return b;
                }
                return false;
            }
            #endregion
            #region Check_Pair_Logical
            bool Check_Pair_Logical(Condition pair, NodeValue jsonValue) {
                switch (pair.Name[0] == '$' ? pair.Name.Substring(1).ToLower() : pair.Name) {
                    #region eq
                    case "=":
                    case "==":
                    case "eq": {
                            if (jsonValue.IsValue) {
                                pair._name = "eq";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    #endregion
                    #region in notin
                    case "in": {
                            if (jsonValue.Type == NodeValueTypes.Array && jsonValue.Length > 0) {
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    case "!in":
                    case "nin":
                    case "notin": {
                            if (jsonValue.Type == NodeValueTypes.Array && jsonValue.Length > 0) {
                                pair._name = "notin";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    #endregion
                    #region > < 
                    case ">":
                    case "gt": {
                            if (jsonValue.IsValue) {
                                pair._name = "gt";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    case "<":
                    case "lt": {
                            if (jsonValue.IsValue) {
                                pair._name = "lt";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    #endregion
                    #region !=
                    case "!=":
                    case "<>":
                    case "neq":
                    case "noteq":
                    case "nq":
                    case "!eq": {
                            if (jsonValue.IsValue) {
                                pair._name = "noteq";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    #endregion
                    #region >= <=
                    case ">=":
                    case "gteq":
                    case "gte": {
                            if (jsonValue.IsValue) {
                                pair._name = "gteq";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    case "<=":
                    case "lteq":
                    case "lte": {
                            if (jsonValue.IsValue) {
                                pair._name = "lteq";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    #endregion
                    #region null
                    case "nul":
                    case "nl":
                    case "null": {
                            pair._name = "null";
                            return true;
                        }
                    #endregion
                    #region null
                    case "notnull":
                    case "!nul":
                    case "!nl":
                    case "!null": {
                            pair._name = "notnull";
                            return true;
                        }
                    #endregion
                    #region && ||
                    case "&&":
                    case "and": {
                            pair._name = "and";
                            if (jsonValue.Type == NodeValueTypes.Dictionary) {
                                Next_Dictionary(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            if (jsonValue.Type == NodeValueTypes.Object) {
                                Next_Object(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            if (jsonValue.Type == NodeValueTypes.Array) {
                                Next_Array(pair, jsonValue, new NodeValueTypes[] { NodeValueTypes.Object, NodeValueTypes.Dictionary });
                                pair.Children.IsArray = true;
                                return pair.Children.Count > 0;
                            }
                            return false;
                        }
                    case "||":
                    case "or": {
                            pair._name = "or";
                            if (jsonValue.Type == NodeValueTypes.Dictionary) {
                                Next_Dictionary(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            if (jsonValue.Type == NodeValueTypes.Object) {
                                Next_Object(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            if (jsonValue.Type == NodeValueTypes.Array) {
                                Next_Array(pair, jsonValue, new NodeValueTypes[] { NodeValueTypes.Object, NodeValueTypes.Dictionary });
                                pair.Children.IsArray = true;
                                return pair.Children.Count > 0;
                            }

                            return false;
                        }
                    #endregion
                    #region not !
                    case "!":
                    case "not": {
                            pair._name = "not";
                            Next(pair, jsonValue);
                            return pair.Children.Count > 0;
                        }
                    #endregion
                    #region like start end
                    case "like": {
                            if (jsonValue.Type == NodeValueTypes.String) {
                                pair._name = "like";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    case "start": {
                            if (jsonValue.Type == NodeValueTypes.String) {
                                pair._name = "start";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    case "end": {
                            if (jsonValue.Type == NodeValueTypes.String) {
                                pair._name = "end";
                                pair._value = jsonValue.Value;
                                return true;
                            }
                            return false;
                        }
                    #endregion
                    #region min max count sum
                    case "min": {
                            if (jsonValue.Type == NodeValueTypes.Number) {
                                pair._name = "min";
                                pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                                return true;
                            } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                                pair._name = "min";
                                Next_Dictionary(pair, jsonValue);
                                return pair.Children.Count > 0;
                            } else if (jsonValue.Type == NodeValueTypes.Object) {
                                pair._name = "min";
                                Next_Object(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            return false;
                        }
                    case "max": {
                            if (jsonValue.Type == NodeValueTypes.Number) {
                                pair._name = "max";
                                pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                                return true;
                            } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                                pair._name = "max";
                                Next_Dictionary(pair, jsonValue);
                                return pair.Children.Count > 0;
                            } else if (jsonValue.Type == NodeValueTypes.Object) {
                                pair._name = "max";
                                Next_Object(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            return false;
                        }
                    case "count": {
                            if (jsonValue.Type == NodeValueTypes.Number) {
                                pair._name = "count";
                                pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                                return true;
                            } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                                pair._name = "count";
                                Next_Dictionary(pair, jsonValue);
                                return pair.Children.Count > 0;
                            } else if (jsonValue.Type == NodeValueTypes.Object) {
                                pair._name = "count";
                                Next_Object(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            return false;
                        }
                    case "sum": {
                            if (jsonValue.Type == NodeValueTypes.Number) {
                                pair._name = "sum";
                                pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                                return true;
                            } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                                pair._name = "sum";
                                Next_Dictionary(pair, jsonValue);
                                return pair.Children.Count > 0;
                            } else if (jsonValue.Type == NodeValueTypes.Object) {
                                pair._name = "sum";
                                Next_Object(pair, jsonValue);
                                return pair.Children.Count > 0;
                            }
                            return false;
                        }
                    #endregion
                    #region # ref
                    case "#":
                    case "ref": {
                            if (jsonValue.Type == NodeValueTypes.String) {
                                string text = (string)jsonValue.Value;
                                if (string.IsNullOrEmpty(text))
                                    return false;
                                pair._name = "ref";
                                pair._value = text;
                                //pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                                return true;
                            }
                            return false;
                        }
                    #endregion

                        #region other commands
                        //case "min": {

                        //    }
                        //case "max": {
                        //    }
                        //case "count": {
                        //    }
                        //case "len": {
                        //    }
                        //case "lenb": {
                        //    }
                        //case "group": {
                        //    }


                        //case "reg":
                        //case "regex": {
                        //    }
                        //
                        #endregion

                }
                return false;
            }
            #endregion
            #region Check_Pair_Field
            bool Check_Pair_Field(Condition pair, NodeValue jsonValue) {
                switch (jsonValue.Type) {
                    case NodeValueTypes.Array: {
                            pair.Children.Add(new Condition("$in", ConditionTypes.Logical, jsonValue.Value));
                            return true;
                        }
                    case NodeValueTypes.Dictionary: {
                            Next_Dictionary(pair, jsonValue);
                            return pair.Children.Count > 0;
                        }
                    case NodeValueTypes.Object: {
                            Next_Object(pair, jsonValue);
                            return pair.Children.Count > 0;
                        }
                    case NodeValueTypes.String: {
                            string value = jsonValue.Value as string;
                            if (string.IsNullOrEmpty(value))
                                return false;
                            if (value[0] == '[') {
                                if (Check_Pair_Field(pair, new NodeValue(Symbol.Serialization.Json.Parse(value))))
                                    return true;

                            }
                            if (value[0] == '{') {
                                if (Check_Pair_Field(pair, new NodeValue(Symbol.Serialization.Json.Parse(value))))
                                    return true;
                            }
                            pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                            return true;
                        }
                    case NodeValueTypes.Null: {
                            return false;
                        }
                    default: {
                            pair.Children.Add(new Condition("$eq", ConditionTypes.Logical, jsonValue.Value));
                            return true;
                        }
                }
            }
            #endregion
            #region GetType
            ConditionTypes GetType(string name) {
                if (string.IsNullOrEmpty(name) || name == "$" || string.Equals(name, "$root"))
                    return ConditionTypes.Root;
                char c = name[0];
                if (c >= '0' && c <= '9')
                    return ConditionTypes.Root;
                if (c >= 'a' && c <= 'z' || (c >= 'A' && c <= 'Z'))
                    return ConditionTypes.Field;
                if (c == '!' || c == '>' || c == '<' || c == '=' || c == '$') {
                    if (name.StartsWith("$self", StringComparison.OrdinalIgnoreCase))
                        return ConditionTypes.Field;
                    return ConditionTypes.Logical;
                }
                return ConditionTypes.Field;
                //return ConditionTypes.Logical;
            }
            #endregion

            #endregion

        }
        class ConditionInstance : IConditionInstance {
            private Condition _root;
            public ConditionInstance() {
                _root = new Condition("$root", ConditionTypes.Root);
            }
            public Condition Condition { get { return _root; } }
            void DoBlock(string @operator, ConditionBlockAction action) {
                if (action == null)
                    return;
                Condition item = _root.Children.Find(p => p.Name == @operator);
                if (item == null) {
                    if (@operator == "$and" && _root.Children.Count == 0) {
                        item = _root;
                    } else {
                        item = new Condition(@operator, ConditionTypes.Logical);
                    }
                }
                ConditionBlock block = new ConditionBlock(item);
                action(block);
                if (@operator == "$or" && item.Children.Count > 1)
                    item.Children.IsArray = true;
                if (item.Children.Count > 0 && item != _root)
                    _root.Children.Add(item);
            }
            public IConditionInstance And(ConditionBlockAction action) {
                DoBlock("$and", action);
                return this;
            }

            public IConditionInstance Not(ConditionBlockAction action) {
                DoBlock("$not", action);
                return this;
            }

            public IConditionInstance Or(ConditionBlockAction action) {
                DoBlock("$or", action);
                return this;
            }

            public string Json(bool formated = false) {
                return _root.ToJson(formated);
            }
        }
        class ConditionBlock : IConditionBlock {
            private Condition _condition;
            public ConditionBlock(Condition condition) {
                _condition = condition;
            }
            Condition AppendField(string name, string @operator, object value, bool checkNullValue = true) {
                if (string.IsNullOrEmpty(name) || (checkNullValue && value == null))
                    return null;
                Condition item = new Condition(name, ConditionTypes.Field);
                item.Children.Add(new Condition(@operator, ConditionTypes.Logical, value));
                _condition.Children.Add(item);
                return item;
            }
            public IConditionBlock End(string name, string value) {
                if (!string.IsNullOrEmpty(value))
                    AppendField(name, "$end", value);
                return this;
            }
            public IConditionBlock End(string name, string value, bool reverse) {
                if (!string.IsNullOrEmpty(value))
                    Like_Reverse(AppendField(name, "$end", value), reverse);
                return this;
            }

            public IConditionBlock Eq(string name, object value) {
                AppendField(name, "$eq", value);
                return this;
            }
            public IConditionBlock NotEq(string name, object value) {
                AppendField(name, "$noteq", value);
                return this;
            }
            public IConditionBlock Gt(string name, object value) {
                AppendField(name, "$gt", value);
                return this;
            }

            public IConditionBlock Gte(string name, object value) {
                AppendField(name, "$gte", value);
                return this;
            }

            public IConditionBlock In(string name, object value) {
                AppendField(name, "$in", value);
                return this;
            }

            public IConditionBlock Like(string name, string value) {
                if (!string.IsNullOrEmpty(value))
                    AppendField(name, "$like", value);
                return this;
            }
            public IConditionBlock Like(string name, string value, bool reverse) {
                if (!string.IsNullOrEmpty(value))
                    Like_Reverse(AppendField(name, "$like", value), reverse);
                return this;
            }

            public IConditionBlock Lt(string name, object value) {
                AppendField(name, "$lt", value);
                return this;
            }

            public IConditionBlock Lte(string name, object value) {
                AppendField(name, "$lte", value);
                return this;
            }

            public IConditionBlock NotIn(string name, object value) {
                AppendField(name, "$notin", value);
                return this;
            }

            public IConditionBlock Null(string name) {
                AppendField(name, "$null", null, false);
                return this;
            }

            public IConditionBlock Start(string name, string value) {
                if (!string.IsNullOrEmpty(value))
                    AppendField(name, "$start", value);
                return this;
            }
            void Like_Reverse(Condition item, bool reverse) {
                var reverse_q = new Condition("reverse", ConditionTypes.Field);
                reverse_q.Children.Add(new Condition("$eq", ConditionTypes.Logical, reverse));
                item.Children.Add(reverse_q);
            }
            public IConditionBlock Start(string name, string value, bool reverse) {
                if (!string.IsNullOrEmpty(value))
                    Like_Reverse(AppendField(name, "$start", value), reverse);
                return this;
            }
            void DoBlock(string @operator, ConditionBlockAction action) {
                if (action == null)
                    return;
                Condition item = _condition.Children.Find(p => p.Name == @operator);
                if (item == null) {
                    if (@operator == "$and" && _condition.Children.Count == 0) {
                        item = _condition;
                    } else {
                        item = new Condition(@operator, ConditionTypes.Logical);
                    }
                }
                ConditionBlock block = new ConditionBlock(item);
                action(block);
                if (@operator == "$or" && item.Children.Count > 1)
                    item.Children.IsArray = true;
                if (item.Children.Count > 0 && item != _condition)
                    _condition.Children.Add(item);
            }
            public IConditionBlock And(ConditionBlockAction action) {
                DoBlock("$and", action);
                return this;
            }

            public IConditionBlock Not(ConditionBlockAction action) {
                DoBlock("$not", action);
                return this;
            }

            public IConditionBlock Or(ConditionBlockAction action) {
                DoBlock("$or", action);
                return this;
            }
            public IConditionBlock Ref(string name, string targetName) {
                if (!string.IsNullOrEmpty(targetName)) {
                    AppendField(name, "$ref", targetName);
                }
                return this;
            }
        }
        #endregion

    }
    interface IConditionSetter {
        void SetParent(Condition parent);
    }

}