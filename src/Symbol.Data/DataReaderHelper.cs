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
        //#if netcore
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
            if (type != null && (type.IsValueType || reader.GetFieldType(0) == type || (reader.FieldCount==1 && type==typeof(string)) )) {
                //只拿第一列
                object value= reader.GetValue(0);
                if (value == DBNull.Value) {
                    return TypeExtensions.DefaultValue(type);
                }
                return TypeExtensions.Convert(value, type);
            }
            if (type != null) {
                if ((reader.FieldCount == 1 && (type.IsEnum || (TypeExtensions.IsNullableType(type) && TypeExtensions.GetNullableType(type).IsEnum))) //枚举
                    || (type.IsValueType || reader.GetFieldType(0) == type)) {//类型匹配
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
                        if (isJson && !property.PropertyType.IsValueType 
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
                        if (isJson && !fieldInfo.FieldType.IsValueType && fieldInfo.FieldType != typeof(string)) {
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