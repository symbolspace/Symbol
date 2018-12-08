/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 排序对象
    /// </summary>
    public class Sorter {

        #region fields
        private Symbol.Collections.Generic.NameValueCollection<string> _values;
        #endregion

        #region ctor
        /// <summary>
        /// 创建Sorter实例
        /// </summary>
        public Sorter() {
            _values = new Collections.Generic.NameValueCollection<string>();
        }
        #endregion

        #region methods

        #region Parse
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Sorter Parse(object value) {
            return new SorterParser().Parse(value);
        }
        #endregion
        #region ToObject
        /// <summary>
        /// 输出数据结构。
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IDictionary<string, object> ToObject() {
            System.Collections.Generic.Dictionary<string, object> root = new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
            foreach (System.Collections.Generic.KeyValuePair<string, string> item in _values) {
                root.Add(item.Key, item.Value);
            }
            return root;
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
        /// 输出JSON文本。
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
        public static implicit operator Sorter(string value) {
            return Parse(value);
        }
        /// <summary>
        /// 解析string[]
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Sorter(string[] value) {
            return Parse(value);
        }
        /// <summary>
        /// 解析System.Collections.Generic.List&lt;string&gt;对象
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Sorter(System.Collections.Generic.List<string> value) {
            return Parse(value);
        }
        /// <summary>
        /// 解析System.Collections.Generic.Dictionary&lt;string, object&gt;对象
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Sorter(System.Collections.Generic.Dictionary<string, object> value) {
            return Parse(value);
        }
        #endregion
        #region explicit
        /// <summary>
        /// 从Sorter转为json文本
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator string(Sorter value) {
            return value == null ? "" : value.ToJson();
        }
        /// <summary>
        /// 从Sorter转为string[]
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator string[] (Sorter value) {
            return value == null ? new string[0] : LinqHelper.ToArray(value._values.Keys);
        }
        ///// <summary>
        ///// 从Sorter转为System.Collections.Generic.Dictionary&lt;string, object&gt;
        ///// </summary>
        ///// <param name="value"></param>
        //public static explicit operator System.Collections.Generic.Dictionary<string, object>(Sorter value) {
        //    return value == null
        //        ? new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase)
        //        : value.ToObject();
        //}
        #endregion

        #endregion

        #region types
        /// <summary>
        /// Sort解析器
        /// </summary>
        public class SorterParser {

            #region methods

            #region Parse
            /// <summary>
            /// 解析
            /// </summary>
            /// <param name="value">仅支持string[]、string、{}、object、string(json)</param>
            /// <returns></returns>
            public Sorter Parse(object value) {
                Sorter result = value as Sorter;
                if (result != null)
                    return result;
                return Parse(NodeValue.As(value));
            }
            Sorter Parse(NodeValue jsonValue) {
                if (jsonValue == null)
                    return new Sorter();
                if (jsonValue.Type == NodeValueTypes.Array) {
                    return Parse_Array(jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.Dictionary) {
                    return Parse_Dictionary(jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.Object) {
                    return Parse_Object(jsonValue);
                } else if (jsonValue.Type == NodeValueTypes.String) {
                    return Parse_String(jsonValue);
                }
                return new Sorter();
            }
            #endregion
            #region Parse_Array
            Sorter Parse_Array(NodeValue jsonValue) {
                Sorter result = new Sorter();
                foreach (object element in (System.Collections.IEnumerable)jsonValue.Value) {
                    NodeValue item = new NodeValue(element);
                    if (item.Type != NodeValueTypes.String) {
                        continue;
                    }
                    string value = item.Value as string;
                    if (string.IsNullOrEmpty(value))
                        continue;
                    if (!Filter_Name(value))
                        continue;
                    if (value.IndexOf(' ') > 0) {
                        result._values[value] = WrapValue(null);
                    } else {
                        string[] pair = value.Split(' ');
                        if (pair.Length == 2) {
                            result._values[pair[0]] = WrapValue(new NodeValue(pair[1]));
                        }
                    }
                }
                return result;
            }
            #endregion
            #region Parse_Object
            Sorter Parse_Object(NodeValue jsonValue) {
                Sorter result = new Sorter();
                foreach (System.Reflection.PropertyInfo propertyInfo in jsonValue.ValueType.GetProperties()) {
                    string key = propertyInfo.Name;
                    if (!Filter_Name(key))
                        continue;
                    NodeValue value = new NodeValue(propertyInfo.GetValue(jsonValue.Value, new object[0]));
                    result._values[key] = WrapValue(value);
                }
                return result;
            }
            #endregion
            #region Parse_Dictionary
            Sorter Parse_Dictionary(NodeValue jsonValue) {
                Sorter result = new Sorter();
                foreach (object item in (System.Collections.IEnumerable)jsonValue.Value) {
                    string key = FastWrapper.Get(item, "Key") as string;
                    if (string.IsNullOrEmpty(key))
                        continue;
                    if (!Filter_Name(key))
                        continue;

                    NodeValue value = new NodeValue(FastWrapper.Get(item, "Value"));
                    result._values[key] = WrapValue(value);
                }
                return result;
            }
            #endregion
            #region Parse_String
            Sorter Parse_String(NodeValue jsonValue) {
                string value = jsonValue.Value as string;
                if (string.IsNullOrEmpty(value))
                    return new Sorter();
                if (value[0] == '[' || value[0] == '{') {
                    return Parse(JSON.Parse(value));
                }
                Sorter result = new Sorter();
                if (Filter_Name(value))
                    result._values.Add(value, WrapValue(null));
                return result;
            }
            #endregion
            #region Filter_Name
            bool Filter_Name(string name) {
                if (name.IndexOf('\'') > -1 || name.IndexOf('"') > -1)
                    return false;

                return true;
            }
            #endregion
            #region WrapValue
            string WrapValue(NodeValue jsonValue) {
                if (jsonValue == null || jsonValue.Type == NodeValueTypes.Null) {
                    return "asc";
                } else if (jsonValue.Type == NodeValueTypes.Number) {
                    return TypeExtensions.Convert<long>(jsonValue.Value, 0) == 0 ? "asc" : "desc";
                } else if (jsonValue.Type == NodeValueTypes.String) {
                    string value = (string)jsonValue.Value;
                    if (string.IsNullOrEmpty(value) || value.StartsWith("asc", StringComparison.OrdinalIgnoreCase))
                        return "asc";
                    return "desc";
                } else if (jsonValue.Type == NodeValueTypes.Boolean) {
                    return ((bool)jsonValue.Value) ? "desc" : "asc";
                } else {
                    return "asc";
                }
            }
            #endregion

            #endregion
        }
        #endregion

    }

}