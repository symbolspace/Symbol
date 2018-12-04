/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Data;
using System.Reflection;

namespace Symbol.Data {
    /// <summary>
    /// IDataReader辅助类。
    /// </summary>
    public static class DataReaderHelper {

        #region methods

        #region Fields
        //        /// <summary>
        //        /// 获取当前DataReader中的列信息。
        //        /// </summary>
        //        /// <param name="reader"></param>
        //        /// <returns>返回所有列。</returns>
        //        public static Symbol.Collections.Generic.HashSet<DataFieldInfo> Fields(IDataReader reader) {
        //            return Fields(reader, null);
        //        }
        //        /// <summary>
        //        /// 获取当前DataReader中的列信息。
        //        /// </summary>
        //        /// <param name="reader"></param>
        //        /// <param name="type">实体类型。</param>
        //        /// <returns>返回所有列。</returns>
        //        public static Symbol.Collections.Generic.HashSet<DataFieldInfo> Fields(IDataReader reader, Type type) {

        //            CommonException.CheckArgumentNull(reader, "reader");

        //            Symbol.Collections.Generic.HashSet<DataFieldInfo> result = new Symbol.Collections.Generic.HashSet<DataFieldInfo>(DataFieldInfo.Comparer);
        //            if (reader.IsClosed)
        //                return result;
        //            bool isExtensibleModel = (type == null ? false : TypeExtensions.IsInheritFrom(type,typeof(IExtensibleModel)));
        //#if NETDNX
        //            bool isEntityType = (type != null && reader.FieldCount > 0 && (reader.GetFieldType(0) == type || type.GetTypeInfo().IsValueType));
        //#else
        //            bool isEntityType = (type != null && reader.FieldCount > 0 && (reader.GetFieldType(0) == type || type.IsValueType));
        //#endif

        //            for (int i = 0; i < reader.FieldCount; i++) {
        //                string name = reader.GetName(i);
        //                if (type != null) {
        //                    if (string.IsNullOrEmpty(name))
        //                        continue;
        //                    if (TypeExtensions.GetValueMemberType(type,name) == null && !isExtensibleModel && !isEntityType)
        //                        continue;
        //                }
        //                Type type2 = reader.GetFieldType(i);
        //                if (string.Equals(reader.GetDataTypeName(i), "char(1)", StringComparison.OrdinalIgnoreCase)
        //                    || string.Equals(reader.GetDataTypeName(i), "nchar(1)", StringComparison.OrdinalIgnoreCase))
        //                    type2 = typeof(char);
        //                DataFieldInfo field = new DataFieldInfo(i, name, type2, reader.GetDataTypeName(i));
        //                result.Add(field);

        //            }
        //            return result;
        //        }
        #endregion

        #region Current
        //        /// <summary>
        //        /// 映射DataReader当前数据记录。
        //        /// </summary>
        //        /// <param name="reader"></param>
        //        /// <param name="fields">列信息。</param>
        //        /// <returns>返回映射结果。</returns>
        //        public static object Current(IDataReader reader, Symbol.Collections.Generic.HashSet<DataFieldInfo> fields) {
        //            return Current(reader, fields, null);
        //        }
        //        /// <summary>
        //        /// 映射DataReader当前数据记录。
        //        /// </summary>
        //        /// <param name="reader"></param>
        //        /// <param name="fields">列信息。</param>
        //        /// <param name="type">实体类型。</param>
        //        /// <returns>返回映射结果。</returns>
        //        public static object Current(IDataReader reader, Symbol.Collections.Generic.HashSet<DataFieldInfo> fields, Type type) {
        //            CommonException.CheckArgumentNull(reader, "reader");
        //            CommonException.CheckArgumentNull(fields, "fields");

        //            if (reader.IsClosed)
        //                return null;

        //#if NETDNX
        //            bool isEntityType = (type != null && fields.Count > 0 && (LinqHelper.FirstOrDefault(fields).Type == type || type.GetTypeInfo().IsValueType));
        //#else
        //            bool isEntityType = (type != null && fields.Count > 0 && (LinqHelper.FirstOrDefault(fields).Type == type || type.IsValueType));
        //#endif
        //            object result = null;
        //            Symbol.Collections.Generic.NameValueCollection<object> dic = null;
        //            IExtensibleModel extensiableModel = null;
        //            if (type == null) {
        //                dic = new Symbol.Collections.Generic.NameValueCollection<object>();
        //                result = dic;
        //            } else if (!isEntityType) {
        //                result = Activator.CreateInstance(type);
        //                if (TypeExtensions.IsInheritFrom(type,typeof(IExtensibleModel))) {
        //                    extensiableModel = (IExtensibleModel)result;
        //                } else if (TypeExtensions.IsInheritFrom(type, typeof(Symbol.Collections.Generic.NameValueCollection<object>))) {
        //                    dic = (Symbol.Collections.Generic.NameValueCollection<object>)result;
        //                }

        //            }
        //            foreach (DataFieldInfo field in fields) {
        //                if (string.IsNullOrEmpty(field.Name) && dic == null)
        //                    continue;
        //                Type type2 = null;
        //                if (dic == null) {
        //                    type2 = TypeExtensions.GetValueMemberType(type,field.Name, true);
        //                    if (type2 == null && extensiableModel == null && !isEntityType)
        //                        continue;
        //                }
        //                string key = string.IsNullOrEmpty(field.Name) ? field.Index.ToString() : field.Name;
        //                object value = TypeExtensions.Convert(reader.GetValue(field.Index),field.Type);

        //                if (field.IsJson) {
        //                    string text = value as string;
        //                    //if (!string.IsNullOrEmpty(text)) {
        //                    value = Symbol.Serialization.Json.Parse(text, isEntityType ? type : type2);
        //                    //}
        //                }
        //                if (dic != null) {
        //                    dic[key] = value;
        //                } else {
        //                    if (isEntityType) {
        //#if NETDNX
        //                        if (field.IsJson && !type.GetTypeInfo().IsValueType && type != typeof(string)) {
        //#else
        //                        if (field.IsJson && !type.IsValueType && type != typeof(string)) {
        //#endif
        //                            //result = Symbol.Serialization.ObjectConverter.ConvertObjectToType(value, type, new Serialization.JavaScriptSerializer());
        //                            result = value;
        //                        } else {
        //                            result = TypeExtensions.Convert(value, type);
        //                        }
        //                        break;
        //                    } else {
        //                        if (type2 == null) {
        //                            if (extensiableModel.Extendeds == null)
        //                                extensiableModel.Extendeds = new Symbol.Collections.Generic.NameValueCollection<object>();
        //                            int extIndex = field.Name.IndexOf("ext_");
        //                            string name = field.Name;
        //                            if (extIndex != -1)
        //                                name = field.Name.Substring(extIndex + 4);
        //                            //var name = (field.Name.StartsWith("ext_") ? field.Name.Substring(4) : field.Name);
        //                            extensiableModel.Extendeds[name] = value;
        //                        } else {
        //                            object value2 = null;
        //                            if (field.IsJson) {
        //                                //result = Symbol.Serialization.Json.Parse(value, type2);
        //                                //result = Symbol.Serialization.ObjectConverter.ConvertObjectToType(value, type2, new Serialization.JavaScriptSerializer());
        //                                value2 = value;
        //                            } else {
        //                                value2 = TypeExtensions.Convert(value, type2);
        //                            }
        //                            //if (field.Type == typeof(string) &&  TypeExtensions.IsInheritFrom(type2,typeof(IDictionary<string, object>))) {
        //                            //    if (string.IsNullOrEmpty((string)value))
        //                            //        value2 = new Symbol.Collections.Generic.NameValueCollection<object>();
        //                            //    //else
        //                            //    //    value2 = IDictionaryHelper.FromXml<NameValueCollection>((string)value);
        //                            //} else {
        //                            //    value2 = TypeExtensions.Convert(value,type2);
        //                            //}
        //                            //reader.GetDataTypeName(field.Index)
        //                            FastWrapper.Set(result,key, value2, BindingFlags.IgnoreCase);
        //                        }
        //                    }
        //                }
        //            }
        //            return result;
        //        }
        //        /// <summary>
        //        /// 映射DataReader当前数据记录。
        //        /// </summary>
        //        /// <param name="reader"></param>
        //        /// <returns>返回映射结果。</returns>
        //        public static object Current(
        //#if !net20
        //            this
        //#endif
        //            IDataReader reader) {
        //            return Current(reader, (Type)null);
        //        }

        /// <summary>
        /// 映射DataReader当前数据记录（单个字段）。
        /// </summary>
        /// <param name="reader">数据读取对象。</param>
        /// <param name="field">字段名称。</param>
        /// <param name="tryParseJson">尝试将文本转换为JSON对象。</param>
        /// <returns>返回字段的值，如果字段不存在，将返回null。</returns>
        public static object Current(
#if !net20
            this
#endif
            IDataReader reader, string field, bool tryParseJson=true) {
            if (reader==null || string.IsNullOrEmpty(field) || reader.IsClosed)
                return null;
            int i = reader.GetOrdinal(field);
            if (i == -1)
                return null;
            object value = reader.GetValue(i);
            if (value == DBNull.Value) {
                return null;
            }
            Type type = reader.GetFieldType(i);
            if (type == typeof(byte[]) && string.Equals(reader.GetDataTypeName(i), "timestamp", StringComparison.OrdinalIgnoreCase)) {
                byte[] buffer = (byte[])value;
                Array.Reverse(buffer);
                return buffer;
            }

            string dataTypeName = reader.GetDataTypeName(i);
            if (string.Equals(reader.GetDataTypeName(i), "char(1)", StringComparison.OrdinalIgnoreCase)
                || string.Equals(reader.GetDataTypeName(i), "nchar(1)", StringComparison.OrdinalIgnoreCase)) {
                value = reader.GetChar(i);
            }

            bool isJson = false;
            if (type == typeof(string)) {
                isJson = string.Equals(dataTypeName, "jsonb", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(dataTypeName, "json", StringComparison.OrdinalIgnoreCase);
            }
            lb_Json_retry:
            if (isJson) {
                value = Symbol.Serialization.Json.Parse(value as string, true);
            } else if (tryParseJson && value is string) {
                string text = ((string)value).Trim();
                if (text != null && ((text.StartsWith("{") && text.EndsWith("}")) || (text.StartsWith("[") && text.EndsWith("]")))) {
                    isJson = true;
                    goto lb_Json_retry;
                }
            }
            return value;
        }

        /// <summary>
        /// 映射DataReader当前数据记录。
        /// </summary>
        /// <param name="reader">数据读取对象。</param>
        /// <param name="type">实体类型。</param>
        /// <returns>返回映射结果。</returns>
        public static object Current(
#if !net20
            this
#endif
            IDataReader reader, Type type) {
            CommonException.CheckArgumentNull(reader,"reader");
            if (reader.IsClosed)
                return null;
#if NETDNX
            if (type != null && (type.GetTypeInfo().IsValueType || reader.GetFieldType(0) == type)) {
#else
            if (type != null && (type.IsValueType || reader.GetFieldType(0) == type || (reader.FieldCount==1 && type==typeof(string)) )) {
#endif
                //只拿第一列
                object value= reader.GetValue(0);
                if (value == DBNull.Value) {
                    return TypeExtensions.DefaultValue(type);
                }
                return TypeExtensions.Convert(value, type);
            }
            if (type != null) {
#if NETDNX
                if ((reader.FieldCount == 1 && (type.GetTypeInfo().IsEnum || (TypeExtensions.IsNullableType(type) && TypeExtensions.GetNullableType(type).GetTypeInfo().IsEnum))) //枚举
                    || (type.GetTypeInfo().IsValueType || reader.GetFieldType(0) == type)) {//类型匹配
#else
                if ((reader.FieldCount == 1 && (type.IsEnum || (TypeExtensions.IsNullableType(type) && TypeExtensions.GetNullableType(type).IsEnum))) //枚举
                    || (type.IsValueType || reader.GetFieldType(0) == type)) {//类型匹配
#endif
                    object value = reader.GetValue(0);
                    return TypeExtensions.Convert((value == DBNull.Value ? TypeExtensions.DefaultValue(type) : value), type);
                }
            }
            Symbol.Collections.Generic.NameValueCollection<object> values = null;
            object result = null;
            if (type == null) {
                values = new Symbol.Collections.Generic.NameValueCollection<object>();
                result = values;
            } else {
                result = FastWrapper.CreateInstance(type);
                //IExtensibleModel extensiableModel = result as IExtensibleModel;
                //if (extensiableModel != null) {
                //    values = extensiableModel.Extendeds = new Symbol.Collections.Generic.NameValueCollection<object>();
                if (result is Symbol.Collections.Generic.NameValueCollection<object>) {
                    values = (Symbol.Collections.Generic.NameValueCollection<object>)result;
                    type = null;
                }
            }

            for (int i = 0; i < reader.FieldCount; i++) {
                string name = reader.GetName(i);
                //int extIndex = name.IndexOf("ext_");
                //if (extIndex != -1)
                //    name = name.Substring(extIndex + 4);
                bool isJson = false;
                if (reader.GetFieldType(i) == typeof(string)) {
                    string dataTypeName = reader.GetDataTypeName(i);
                    isJson = string.Equals(dataTypeName, "jsonb", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(dataTypeName, "json", StringComparison.OrdinalIgnoreCase);
                }

                object value = reader[i];
                if (value == DBNull.Value) {
                    value = null;
                }
                //if (isJson) {
                //    string text = value as string;
                //    if (!string.IsNullOrEmpty(text)) {
                //        value = Symbol.Serialization.Json.Parse(text);
                //    }
                //} else 
                if (reader.GetFieldType(i) == typeof(byte[]) && string.Equals(reader.GetDataTypeName(i), "timestamp", StringComparison.OrdinalIgnoreCase)) {
                    byte[] buffer = (byte[])value;
                    Array.Reverse(buffer);
                }
                if (string.IsNullOrEmpty(name)) {
                    if (values != null)
                        values.Add("无列名" + i, value);
                    continue;
                }
                if (type != null) {
                    PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    if (property != null) {
                        if (!isJson) {
                            isJson = TypeExtensions.Convert<bool>(ConstAttributeExtensions.Const(property, "SaveAsJson"), false);
                        }
                        if (!isJson && reader.GetFieldType(i) == typeof(string) && 
                            (!property.PropertyType.IsValueType&& property.PropertyType != typeof(string))) {
                            isJson = true;
                        }
#if NETDNX
                        if (isJson && !property.PropertyType.GetTypeInfo().IsValueType 
#else
                        if (isJson && !property.PropertyType.IsValueType 
#endif
                            && property.PropertyType != typeof(string)) {
                            //value = Symbol.Serialization.ObjectConverter.ConvertObjectToType(value, property.PropertyType, new Serialization.JavaScriptSerializer());
                            value = Symbol.Serialization.Json.Parse(value as string, property.PropertyType);
                        } else {
                            value = TypeExtensions.Convert(value, property.PropertyType);
                        }
                        property.SetValue(result, value, null);
                        continue;
                    }

                    FieldInfo fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    if (fieldInfo != null) {
#if NETDNX
                        if (isJson && !fieldInfo.FieldType.GetTypeInfo().IsValueType && fieldInfo.FieldType != typeof(string)) {
#else
                        if (isJson && !fieldInfo.FieldType.IsValueType && fieldInfo.FieldType != typeof(string)) {
#endif
                            //value = Symbol.Serialization.ObjectConverter.ConvertObjectToType(value, fieldInfo.FieldType, new Serialization.JavaScriptSerializer());
                            value = Symbol.Serialization.Json.Parse(value as string, fieldInfo.FieldType);
                        } else {
                            value = TypeExtensions.Convert(value, fieldInfo.FieldType);
                        }

                        fieldInfo.SetValue(result, value);
                        continue;
                    }

                }
                if (values == null)
                    continue;
                if (value != null) {
                    lb_Json_retry:
                    if (isJson) {
                        value = Symbol.Serialization.Json.Parse(value as string, true);
                    } else if (string.Equals(reader.GetDataTypeName(i), "char(1)", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(reader.GetDataTypeName(i), "nchar(1)", StringComparison.OrdinalIgnoreCase)) {
                        value = reader.GetChar(i);
                    } else if (value is string) {
                        string text = ((string)value).Trim();
                        if (text != null && ((text.StartsWith("{") && text.EndsWith("}")) || (text.StartsWith("[") && text.EndsWith("]")))) {
                            isJson = true;
                            goto lb_Json_retry;
                        }
                    }
                }
                values[name] = value;
                if (isJson && reader.FieldCount == 1 && (type == null || type == typeof(object))){
                    return value;
                }
                //values[name] = reader.GetValue(i);
                //if (values[name] == DBNull.Value)
                //    values[name] = null;
                //else if (values[name] != null) {
                //    if (string.Equals(reader.GetDataTypeName(i), "char(1)", StringComparison.OrdinalIgnoreCase)
                //        || string.Equals(reader.GetDataTypeName(i), "nchar(1)", StringComparison.OrdinalIgnoreCase)) {
                //        values[name] = reader.GetChar(i);
                //    }
                //}
            }

            return result;
        }
        #endregion

        #endregion

    }
}