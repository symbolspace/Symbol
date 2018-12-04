/*  
 *  reference to fastJson
 * 
 */

#pragma warning disable 1591

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Symbol;
using Symbol.Formatting;
using Symbol.Formatting.Json;
/// <summary>
/// JSON
/// </summary>
public static class JSON {
    
    #region fields
    /// <summary>
    /// Globally set-able parameters for controlling the serializer
    /// </summary>
    public static readonly JSONParameters Parameters = new JSONParameters();

    #endregion

    #region methods

    #region ToNiceJSON
    /// <summary>
    /// Create a formatted json string (beautified) from an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToNiceJSON(object obj) {
        string s = ToJSON(obj, Parameters); // use default params

        return Beautify(s);
    }
    /// <summary>
    /// Create a formatted json string (beautified) from an object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static string ToNiceJSON(object obj, JSONParameters param) {
        string s = ToJSON(obj, param);

        return Beautify(s);
    }
    #endregion
    #region ToJSON
    /// <summary>
    /// Create a json representation for an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToJSON(object obj) {
        return ToJSON(obj, Parameters);
    }
    /// <summary>
    /// Create a json representation for an object with parameter override on this call
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static string ToJSON(object obj, JSONParameters param) {
        param.FixValues();
        Type t = null;

        if (obj == null)
            return "null";

#if netcore
            if (obj.GetType().GetTypeInfo().IsGenericType)
#else
        if (obj.GetType().IsGenericType)
#endif
            t = Reflection.Instance.GetGenericTypeDefinition(obj.GetType());
        if (t == typeof(Dictionary<,>) || t == typeof(List<>))
            param.UsingGlobalTypes = false;

        // FEATURE : enable extensions when you can deserialize anon types
        if (param.EnableAnonymousTypes) { param.UseExtensions = false; param.UsingGlobalTypes = false; }
        return new JSONSerializer(param).ConvertToJSON(obj);
    }
    #endregion
    #region Parse
    /// <summary>
    /// Parse a json string and generate a Dictionary&lt;string,object&gt; or List&lt;object&gt; structure
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static object Parse(string json) {
        return new JsonParser(json).Decode();
    }
    #endregion
    #region ToDynamic
#if net40
    /// <summary>
    /// Create a .net4 dynamic object from the json string
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static dynamic ToDynamic(string json) {
        return new DynamicJson(json);
    }
#endif
    #endregion
    #region ToObject
    /// <summary>
    /// Create a typed generic object from the json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T ToObject<T>(string json) {
        return new JsonDeserializer(Parameters).ToObject<T>(json);
    }
    /// <summary>
    /// Create a typed generic object from the json with parameter override on this call
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static T ToObject<T>(string json, JSONParameters param) {
        return new JsonDeserializer(param).ToObject<T>(json);
    }
    /// <summary>
    /// Create an object from the json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static object ToObject(string json) {
        return new JsonDeserializer(Parameters).ToObject(json, null);
    }
    /// <summary>
    /// Create an object from the json with parameter override on this call
    /// </summary>
    /// <param name="json"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static object ToObject(string json, JSONParameters param) {
        return new JsonDeserializer(param).ToObject(json, null);
    }
    /// <summary>
    /// Create an object of type from the json
    /// </summary>
    /// <param name="json"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ToObject(string json, Type type) {
        return new JsonDeserializer(Parameters).ToObject(json, type);
    }
    #endregion
    #region FillObject
    /// <summary>
    /// Fill a given object with the json represenation
    /// </summary>
    /// <param name="input"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static object FillObject(object input, string json) {
        IDictionary<string, object> ht = new JsonParser(json).Decode() as IDictionary<string, object>;
        if (ht == null) return null;
        return new JsonDeserializer(Parameters).ParseDictionary(ht, null, input.GetType(), input);
    }
    #endregion
    #region DeepCopy
    /// <summary>
    /// Deep copy an object i.e. clone to a new object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object DeepCopy(object obj) {
        return new JsonDeserializer(Parameters).ToObject(ToJSON(obj));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopy<T>(T obj) {
        return new JsonDeserializer(Parameters).ToObject<T>(ToJSON(obj));
    }
    #endregion
    #region Beautify
    /// <summary>
    /// Create a human readable string from the json 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Beautify(string input) {
        return JsonFormatter.PrettyPrint(input);
    }
    #endregion
    #region RegisterCustomType
    /// <summary>
    /// Register custom type handlers for your own types not natively handled by _
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serializer"></param>
    /// <param name="deserializer"></param>
    public static void RegisterCustomType(Type type, JsonSerializeDelegate serializer, JsonDeserializeDelegate deserializer) {
        Reflection.Instance.RegisterCustomType(type, serializer, deserializer);
    }
    public static void RegisterCustomType<T>(JsonSerializeDelegate<T> serializer, JsonDeserializeDelegate<T> deserializer) where T:class,new() {
        Reflection.Instance.RegisterCustomType(typeof(T), p=>serializer((T)p), p=> {
            var model = new T();
            deserializer(model, p);
            return model;
        });
    }
    #endregion
    #region ClearReflectionCache
    /// <summary>
    /// Clear the internal reflection cache so you can start from new (you will loose performance)
    /// </summary>
    public static void ClearReflectionCache() {
        Reflection.Instance.ClearReflectionCache();
    }
    #endregion
    #region CreateLong
    internal static long CreateLong(string s, int index, int count) {
        long num = 0;
        bool neg = false;
        for (int x = 0; x < count; x++, index++) {
            char cc = s[index];

            if (cc == '-')
                neg = true;
            else if (cc == '+')
                neg = false;
            else {
                num *= 10;
                num += (int)(cc - '0');
            }
        }
        if (neg) num = -num;

        return num;
    }
    #endregion

    #endregion
}


#region types
namespace Symbol.Formatting.Json {



    #region JsonDeserializer
    internal class JsonDeserializer {

        #region fields
        private JSONParameters _params;
        private bool _usingglobals = false;
        private Dictionary<object, int> _circobj = new Dictionary<object, int>();
        private Dictionary<int, object> _cirrev = new Dictionary<int, object>();
        #endregion

        #region ctor
        public JsonDeserializer(JSONParameters param) {
            _params = param;
        }
        #endregion

        #region ToObject
        public T ToObject<T>(string json) {
            Type t = typeof(T);
            var o = ToObject(json, t);

            if (t.IsArray) {
                if ((o as ICollection).Count == 0) // edge case for "[]" -> T[]
                {
                    Type tt = t.GetElementType();
                    object oo = Array.CreateInstance(tt, 0);
                    return (T)oo;
                } else
                    return (T)o;
            } else
                return (T)o;
        }

        public object ToObject(string json) {
            return ToObject(json, null);
        }

        public object ToObject(string json, Type type) {
            //_params = Parameters;
            _params.FixValues();
            Type t = null;
#if netcore
        if (type != null && type.GetTypeInfo().IsGenericType)
#else
            if (type != null && type.IsGenericType)
#endif
                t = Reflection.Instance.GetGenericTypeDefinition(type);
            if (t == typeof(Dictionary<,>) || t == typeof(List<>))
                _params.UsingGlobalTypes = false;
            _usingglobals = _params.UsingGlobalTypes;

            object o = new JsonParser(json).Decode();
            if (o == null)
                return null;
            if (o is IDictionary) {
                if (type != null && t == typeof(Dictionary<,>)) // deserialize a dictionary
                    return RootDictionary(o, type);
                else // deserialize an object
                    return ParseDictionary(o as IDictionary<string, object>, null, type, null);
            } else if (o is List<object>) {
                if (type != null && t == typeof(Dictionary<,>)) // kv format
                    return RootDictionary(o, type);
                else if (type != null && t == typeof(List<>)) // deserialize to generic list
                    return RootList(o, type);
                else if (type != null && type.IsArray)
                    return RootArray(o, type);
                else if (type == typeof(Hashtable))
                    return RootHashTable((List<object>)o);
                else
                    return (o as List<object>).ToArray();
            } else if (type != null && o.GetType() != type)
                return ChangeType(o, type);

            return o;
        }
        #endregion

        #region [   p r i v a t e   m e t h o d s   ]
        private object RootHashTable(List<object> o) {
            Hashtable h = new Hashtable();

            foreach (IDictionary<string, object> values in o) {
                object key = values["k"];
                object val = values["v"];
                if (key is IDictionary<string, object>)
                    key = ParseDictionary((IDictionary<string, object>)key, null, typeof(object), null);

                if (val is IDictionary<string, object>)
                    val = ParseDictionary((IDictionary<string, object>)val, null, typeof(object), null);

                h.Add(key, val);
            }

            return h;
        }

        private object ChangeType(object value, Type conversionType) {
            if (conversionType == typeof(int)) {
                string s = value as string;
                if (s == null)
                    return (int)((long)value);
                else
                    return CreateInteger(s, 0, s.Length);
            } else if (conversionType == typeof(long)) {
                string s = value as string;
                if (s == null)
                    return (long)value;
                else
                    return JSON.CreateLong(s, 0, s.Length);
            } else if (conversionType == typeof(string))
                return (string)value;
#if netcore
        else if (conversionType.GetTypeInfo().IsEnum)
#else
            else if (conversionType.IsEnum)
#endif
                return CreateEnum(conversionType, value);

            else if (conversionType == typeof(DateTime)) {
                if (value is System.DateTime)
                    return (System.DateTime)value;
                return CreateDateTime((string)value);
            } else if (conversionType == typeof(DateTimeOffset))
                return CreateDateTimeOffset((string)value);

            else if (Reflection.Instance.IsTypeRegistered(conversionType))
                return Reflection.Instance.CreateCustom((string)value, conversionType);

            // 8-30-2014 - James Brooks - Added code for nullable types.
            if (IsNullable(conversionType)) {
                if (value == null)
                    return value;
                conversionType = UnderlyingTypeOf(conversionType);
            }

            // 8-30-2014 - James Brooks - Nullable Guid is a special case so it was moved after the "IsNullable" check.
            if (conversionType == typeof(Guid))
                return CreateGuid((string)value);

            // 2016-04-02 - Enrico Padovani - proper conversion of byte[] back from string
            if (conversionType == typeof(byte[]))
                return Convert.FromBase64String((string)value);
            return TypeExtensions.Convert(value, conversionType);
            //return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        private object CreateDateTimeOffset(string value) {
            //                   0123456789012345678 9012 9/3 0/4  1/5
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnn  _   +   00:00
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;
            int ms = 0;
            int th = 0;
            int tm = 0;

            year = CreateInteger(value, 0, 4);
            month = CreateInteger(value, 5, 2);
            day = CreateInteger(value, 8, 2);
            hour = CreateInteger(value, 11, 2);
            min = CreateInteger(value, 14, 2);
            sec = CreateInteger(value, 17, 2);

            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);
            int p = 20;
            if (ms > 0)
                p = 24;
            th = CreateInteger(value, p + 1, 2);
            tm = CreateInteger(value, p + 1 + 2 + 1, 2);

            if (value[p] == '-')
                th = -th;

            return new DateTimeOffset(year, month, day, hour, min, sec, ms, new TimeSpan(th, tm, 0));
        }

        private bool IsNullable(Type t) {
#if netcore
        if (!t.GetTypeInfo().IsGenericType) return false;
#else
            if (!t.IsGenericType) return false;
#endif
            Type g = t.GetGenericTypeDefinition();
            return (g.Equals(typeof(Nullable<>)));
        }

        private Type UnderlyingTypeOf(Type t) {
            return t.GetGenericArguments()[0];
        }

        private object RootList(object parse, Type type) {
            Type[] gtypes = Reflection.Instance.GetGenericArguments(type);
            IList o = (IList)Reflection.Instance.FastCreateInstance(type);
            DoParseList(parse, gtypes[0], o);
            return o;
        }

        private void DoParseList(object parse, Type it, IList o) {
            foreach (var k in (IList)parse) {
                _usingglobals = false;
                object v = k;
                if (k is IDictionary<string, object>)
                    v = ParseDictionary(k as IDictionary<string, object>, null, it, null);
                else
                    v = ChangeType(k, it);

                o.Add(v);
            }
        }

        private object RootArray(object parse, Type type) {
            Type it = type.GetElementType();
            IList o = (IList)Reflection.Instance.FastCreateInstance(typeof(List<>).MakeGenericType(it));
            DoParseList(parse, it, o);
            var array = Array.CreateInstance(it, o.Count);
            o.CopyTo(array, 0);

            return array;
        }

        private object RootDictionary(object parse, Type type) {
            Type[] gtypes = Reflection.Instance.GetGenericArguments(type);
            Type t1 = null;
            Type t2 = null;
            if (gtypes != null) {
                t1 = gtypes[0];
                t2 = gtypes[1];
            }
            var arraytype = t2.GetElementType();
            if (parse is IDictionary<string, object>) {
                IDictionary o = (IDictionary)Reflection.Instance.FastCreateInstance(type);

                foreach (var kv in (IDictionary<string, object>)parse) {
                    object v;
                    object k = ChangeType(kv.Key, t1);

                    if (kv.Value is IDictionary<string, object>)
                        v = ParseDictionary(kv.Value as IDictionary<string, object>, null, t2, null);

                    else if (t2.IsArray && t2 != typeof(byte[]))
                        v = CreateArray((List<object>)kv.Value, t2, arraytype, null);

                    else if (kv.Value is IList)
                        v = CreateGenericList((List<object>)kv.Value, t2, t1, null);

                    else
                        v = ChangeType(kv.Value, t2);

                    o.Add(k, v);
                }

                return o;
            }
            if (parse is List<object>)
                return CreateDictionary(parse as List<object>, type, gtypes, null);

            return null;
        }

        internal object ParseDictionary(IDictionary<string, object> d, IDictionary<string, object> globaltypes, Type type, object input) {
            object tn = "";
            if (type == typeof(NameValueCollection))
                return CreateNV(d);
            if (type == typeof(StringDictionary))
                return CreateSD(d);

            if (d.TryGetValue("$i", out tn)) {
                object v = null;
                _cirrev.TryGetValue((int)(long)tn, out v);
                return v;
            }

            if (d.TryGetValue("$types", out tn)) {
                _usingglobals = true;
                globaltypes = new Dictionary<string, object>();
                foreach (var kv in (IDictionary<string, object>)tn) {
                    globaltypes.Add((string)kv.Value, kv.Key);
                }
            }

            bool found = d.TryGetValue("$type", out tn);
#if !SILVERLIGHT && !netcore
            if (found == false && type == typeof(System.Object)) {
                return d;   // CreateDataset(d, globaltypes);
            }
#endif
            if (found) {
                if (_usingglobals) {
                    object tname = "";
                    if (globaltypes != null && globaltypes.TryGetValue((string)tn, out tname))
                        tn = tname;
                }
                type = Reflection.Instance.GetTypeFromCache((string)tn);
            }
            Symbol.CommonException.CheckArgumentNull(type, "type");

            string typename = type.AssemblyQualifiedName;
            object o = input;
            if (o == null) {
                if (_params.ParametricConstructorOverride) {
#if netcore
                o = TypeExtensions.DefaultValue(type);
#else
                    o = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#endif
                } else
                    o = Reflection.Instance.FastCreateInstance(type);
            }
            int circount = 0;
            if (_circobj.TryGetValue(o, out circount) == false) {
                circount = _circobj.Count + 1;
                _circobj.Add(o, circount);
                _cirrev.Add(circount, o);
            }

            Dictionary<string, CachePropertyInfo> props = Reflection.Instance.GetProperties(type, typename);//, Reflection.Instance.IsTypeRegistered(type));
            foreach (var kv in d) {
                var n = kv.Key;
                var v = kv.Value;
                if (string.Equals(n, "$map", StringComparison.OrdinalIgnoreCase)) {
                    ProcessMap(o, props, (IDictionary<string, object>)d["$map"]);
                    continue;
                }
                CachePropertyInfo pi;
                if (!props.TryGetValue(n, out pi))
                    if (!props.TryGetValue("lower." + n.ToLower(), out pi))
                        continue;
                if (pi.CanWrite) {
                    //object v = d[n];

                    if (v != null) {
                        object oset = null;

                        switch (pi.Type) {
                            case PropertyInfoTypes.Int: oset = (int)((long)v); break;
                            case PropertyInfoTypes.Long: oset = (long)v; break;
                            case PropertyInfoTypes.String: oset = (string)v; break;
                            case PropertyInfoTypes.Bool: oset = (bool)v; break;
                            case PropertyInfoTypes.DateTime: {
                                    if (v is System.DateTime)
                                        oset = v;
                                    else
                                        oset = CreateDateTime((string)v); break;
                                }
                            case PropertyInfoTypes.Enum: oset = CreateEnum(pi.MemberType, v); break;
                            case PropertyInfoTypes.Guid: oset = CreateGuid((string)v); break;

                            case PropertyInfoTypes.Array:
                                if (!pi.IsValueType)
                                    oset = CreateArray((List<object>)v, pi.MemberType, pi.ElementType, globaltypes);
                                // what about 'else'?
                                break;
                            case PropertyInfoTypes.ByteArray: oset = Convert.FromBase64String((string)v); break;
#if !netcore
                            case PropertyInfoTypes.Hashtable: // same case as Dictionary
#endif
                            case PropertyInfoTypes.Dictionary: oset = CreateDictionary((List<object>)v, pi.MemberType, pi.GenericTypes, globaltypes); break;
                            case PropertyInfoTypes.StringKeyDictionary: oset = CreateStringKeyDictionary((IDictionary<string, object>)v, pi.MemberType, pi.GenericTypes, globaltypes); break;
                            case PropertyInfoTypes.NameValue: oset = CreateNV((IDictionary<string, object>)v); break;
                            case PropertyInfoTypes.StringDictionary: oset = CreateSD((IDictionary<string, object>)v); break;
                            case PropertyInfoTypes.Custom: oset = Reflection.Instance.CreateCustom((string)v, pi.MemberType); break;
                            default: {
                                    if (pi.IsGenericType && pi.IsValueType == false && v is List<object>)
                                        oset = CreateGenericList((List<object>)v, pi.MemberType, pi.ElementType, globaltypes);

                                    else if ((pi.IsClass || pi.IsStruct || pi.IsInterface) && v is IDictionary<string, object>)
                                        oset = ParseDictionary((IDictionary<string, object>)v, globaltypes, pi.MemberType, pi.Getter(o));

                                    else if (v is List<object>)
                                        oset = CreateArray((List<object>)v, pi.MemberType, typeof(object), globaltypes);

                                    else if (pi.IsValueType)
                                        oset = ChangeType(v, pi.ChangeType);

                                    else
                                        oset = v;
                                }
                                break;
                        }

                        o = pi.Setter(o, oset);
                    }
                }
            }
            return o;
        }

        private StringDictionary CreateSD(IDictionary<string, object> d) {
            StringDictionary nv = new StringDictionary();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        private NameValueCollection CreateNV(IDictionary<string, object> d) {
            NameValueCollection nv = new NameValueCollection();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        private void ProcessMap(object obj, Dictionary<string, CachePropertyInfo> props, IDictionary<string, object> dic) {
            foreach (KeyValuePair<string, object> kv in dic) {
                CachePropertyInfo p;
                if (!props.TryGetValue(kv.Key, out p))
                    if (!props.TryGetValue("lower." + kv.Key.ToLower(), out p))
                        continue;

                object o = p.Getter(obj);
                Type t = Type.GetType((string)kv.Value);
                if (t == typeof(Guid))
                    p.Setter(obj, CreateGuid((string)o));
            }
        }

        private int CreateInteger(string s, int index, int count) {
            int num = 0;
            bool neg = false;
            for (int x = 0; x < count; x++, index++) {
                char cc = s[index];

                if (cc == '-')
                    neg = true;
                else if (cc == '+')
                    neg = false;
                else {
                    num *= 10;
                    num += (int)(cc - '0');
                }
            }
            if (neg) num = -num;

            return num;
        }

        private object CreateEnum(Type pt, object v) {
            // FEATURE : optimize create enum
#if !SILVERLIGHT
            return Enum.Parse(pt, v.ToString());
#else
        return Enum.Parse(pt, v, true);
#endif
        }

        private Guid CreateGuid(string s) {
            if (s.Length > 30)
                return new Guid(s);
            else
                return new Guid(Convert.FromBase64String(s));
        }

        private DateTime CreateDateTime(string value) {
            bool utc = false;
            //                   0123456789012345678 9012 9/3
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnn  Z
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;
            int ms = 0;

            year = CreateInteger(value, 0, 4);
            month = CreateInteger(value, 5, 2);
            day = CreateInteger(value, 8, 2);
            hour = CreateInteger(value, 11, 2);
            min = CreateInteger(value, 14, 2);
            sec = CreateInteger(value, 17, 2);
            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);

            if (value[value.Length - 1] == 'Z')
                utc = true;

            if (_params.UseUTCDateTime == false && utc == false)
                return new DateTime(year, month, day, hour, min, sec, ms);
            else
                return new DateTime(year, month, day, hour, min, sec, ms, DateTimeKind.Utc).ToLocalTime();
        }

        private object CreateArray(List<object> data, Type pt, Type bt, IDictionary<string, object> globalTypes) {
            if (bt == null)
                bt = typeof(object);

            Array col = Array.CreateInstance(bt, data.Count);
            var arraytype = bt.GetElementType();
            // create an array of objects
            for (int i = 0; i < data.Count; i++) {
                object ob = data[i];
                if (ob == null) {
                    continue;
                }
                if (ob is IDictionary)
                    col.SetValue(ParseDictionary((IDictionary<string, object>)ob, globalTypes, bt, null), i);
                else if (ob is ICollection)
                    col.SetValue(CreateArray((List<object>)ob, bt, arraytype, globalTypes), i);
                else
                    col.SetValue(ChangeType(ob, bt), i);
            }

            return col;
        }

        private object CreateGenericList(List<object> data, Type pt, Type bt, IDictionary<string, object> globalTypes) {
            if (pt != typeof(object)) {
                IList col = (IList)Reflection.Instance.FastCreateInstance(pt);
                var it = pt.GetGenericArguments()[0];
                // create an array of objects
                foreach (object ob in data) {
                    if (ob is IDictionary)
                        col.Add(ParseDictionary((IDictionary<string, object>)ob, globalTypes, bt, null));

                    else if (ob is List<object>) {
#if netcore
                    if (bt.GetTypeInfo().IsGenericType)
#else
                        if (bt.IsGenericType)
#endif
                            col.Add((List<object>)ob);//).ToArray());
                        else
                            col.Add(((List<object>)ob).ToArray());
                    } else
                        col.Add(ChangeType(ob, it));
                }
                return col;
            }
            return data;
        }

        private object CreateStringKeyDictionary(IDictionary<string, object> reader, Type pt, Type[] types, IDictionary<string, object> globalTypes) {
            var col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type arraytype = null;
            Type t2 = null;
            if (types != null)
                t2 = types[1];

            Type generictype = null;
            var ga = t2.GetGenericArguments();
            if (ga.Length > 0)
                generictype = ga[0];
            arraytype = t2.GetElementType();

            foreach (KeyValuePair<string, object> values in reader) {
                var key = values.Key;
                object val = null;

                if (values.Value is IDictionary<string, object>)
                    val = ParseDictionary((IDictionary<string, object>)values.Value, globalTypes, t2, null);

                else if (types != null && t2.IsArray) {
                    if (values.Value is Array)
                        val = values.Value;
                    else
                        val = CreateArray((List<object>)values.Value, t2, arraytype, globalTypes);
                } else if (values.Value is IList)
                    val = CreateGenericList((List<object>)values.Value, t2, generictype, globalTypes);

                else
                    val = ChangeType(values.Value, t2);

                col.Add(key, val);
            }

            return col;
        }

        private object CreateDictionary(List<object> reader, Type pt, Type[] types, IDictionary<string, object> globalTypes) {
            IDictionary col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type t1 = null;
            Type t2 = null;
            if (types != null) {
                t1 = types[0];
                t2 = types[1];
            }

            foreach (IDictionary<string, object> values in reader) {
                object key = values["k"];
                object val = values["v"];

                if (key is IDictionary<string, object>)
                    key = ParseDictionary((IDictionary<string, object>)key, globalTypes, t1, null);
                else
                    key = ChangeType(key, t1);

                if (typeof(IDictionary).IsAssignableFrom(t2))
                    val = RootDictionary(val, t2);
                else if (val is IDictionary<string, object>)
                    val = ParseDictionary((IDictionary<string, object>)val, globalTypes, t2, null);
                else
                    val = ChangeType(val, t2);

                col.Add(key, val);
            }

            return col;
        }

        #endregion
    }
    #endregion
    #region JsonParser
    internal sealed class JsonParser {

        #region fields
        private readonly string _rawString;
        private readonly System.Text.StringBuilder _builder = new System.Text.StringBuilder(); // used for inner string parsing " \"\r\n\u1234\'\t " 
        private JsonNodeTypes _lastNodeType = JsonNodeTypes.None;
        private int _index;
        #endregion

        #region ctor
        internal JsonParser(string json) {
            this._rawString = json;
        }
        #endregion

        #region methods

        #region Decode
        public object Decode() {
            return ParseValue();
        }
        #endregion
        #region ParseObject
        private System.Collections.Generic.IDictionary<string, object> ParseObject() {
            var table = new Symbol.Collections.Generic.NameValueCollection<object>(StringComparer.Ordinal);

            ConsumeToken(); // {

            while (true) {
                switch (LookAhead()) {

                    case JsonNodeTypes.Comma:
                        ConsumeToken();
                        break;

                    case JsonNodeTypes.Curly_Close:
                        ConsumeToken();
                        return table;

                    default: {
                            // name
                            string name = null;
                            if (_lastNodeType == JsonNodeTypes.String)
                                name = ParseString();
                            else if (_lastNodeType == JsonNodeTypes.String2)
                                name = ParseString2();
                            else if (_lastNodeType == JsonNodeTypes.String3)
                                name = ParseString3();
                            else if (_lastNodeType == JsonNodeTypes.Number) {
                                name = ParseNumber().ToString();
                            }
                            //string name = lookAheadToken== Token.String?  ParseString():ParseString2();

                            // :
                            if (NextNode() != JsonNodeTypes.Colon) {
                                throw new System.Exception("Expected colon at index " + _index);
                            }

                            // value
                            object value = ParseValue();

                            table[name] = value;
                        }
                        break;
                }
            }
        }
        #endregion
        #region ParseArray
        private System.Collections.Generic.List<object> ParseArray() {
            System.Collections.Generic.List<object> array = new System.Collections.Generic.List<object>();
            ConsumeToken(); // [

            while (true) {
                switch (LookAhead()) {
                    case JsonNodeTypes.Comma:
                        ConsumeToken();
                        break;

                    case JsonNodeTypes.Squared_Close:
                        ConsumeToken();
                        return array;

                    default:
                        array.Add(ParseValue());
                        break;
                }
            }
        }
        #endregion
        #region PreString
        private object PreString(string value) {
            if (value == null || value.Length == 0)
                return value;
            {
                /* "\/Date(-62135596800000)\/"   */
                if (!value.StartsWith("/Date(") || !value.EndsWith(")/"))
                    return value;
                string text = value.Substring(6, value.Length - 8);
                long n;
                if (!long.TryParse(text, out n))
                    return value;
                return HttpUtility.FromJsTick(n);
            }
        }
        #endregion
        #region ParseValue
        private object ParseValue() {
            switch (LookAhead()) {
                case JsonNodeTypes.Number:
                    return ParseNumber();

                case JsonNodeTypes.String:
                    //return ParseString();
                    return PreString(ParseString());
                case JsonNodeTypes.String2:
                    //return ParseString();
                    return PreString(ParseString2());
                case JsonNodeTypes.String3:
                    string p = ParseString3();
                    if (string.Equals(p, "null", StringComparison.OrdinalIgnoreCase) || string.Equals(p, "undefined", StringComparison.OrdinalIgnoreCase))
                       return null;
                    //return ParseString();
                    return PreString(p);

                case JsonNodeTypes.Curly_Open:
                    return ParseObject();

                case JsonNodeTypes.Squared_Open:
                    return ParseArray();

                case JsonNodeTypes.True:
                    ConsumeToken();
                    return true;

                case JsonNodeTypes.False:
                    ConsumeToken();
                    return false;

                case JsonNodeTypes.Null:
                case JsonNodeTypes.Undefined:
                    ConsumeToken();
                    return null;
            }

            throw new System.Exception("Unrecognized token at index" + _index);
        }
        #endregion
        #region ParseString
        private string ParseString() {
            ConsumeToken(); // "

            _builder.Length = 0;

            int runIndex = -1;
            int l = _rawString.Length;
            //fixed (char* p = json)
            string p = _rawString;
            {
                while (_index < l) {
                    var c = p[_index++];

                    if (c == '"') {
                        if (runIndex != -1) {
                            if (_builder.Length == 0)
                                return _rawString.Substring(runIndex, _index - runIndex - 1);

                            _builder.Append(_rawString, runIndex, _index - runIndex - 1);
                        }
                        return _builder.ToString();
                    }

                    if (c != '\\') {
                        if (runIndex == -1)
                            runIndex = _index - 1;

                        continue;
                    }

                    if (_index == l) break;

                    if (runIndex != -1) {
                        _builder.Append(_rawString, runIndex, _index - runIndex - 1);
                        runIndex = -1;
                    }

                    switch (p[_index++]) {
                        case '"':
                            _builder.Append('"');
                            break;

                        case '\\':
                            _builder.Append('\\');
                            break;

                        case '/':
                            _builder.Append('/');
                            break;

                        case 'b':
                            _builder.Append('\b');
                            break;

                        case 'f':
                            _builder.Append('\f');
                            break;

                        case 'n':
                            _builder.Append('\n');
                            break;

                        case 'r':
                            _builder.Append('\r');
                            break;

                        case 't':
                            _builder.Append('\t');
                            break;

                        case 'u': {
                                int remainingLength = l - _index;
                                if (remainingLength < 4) break;

                                // parse the 32 bit hex into an integer codepoint
                                uint codePoint = ParseUnicode(p[_index], p[_index + 1], p[_index + 2], p[_index + 3]);
                                _builder.Append((char)codePoint);

                                // skip 4 chars
                                _index += 4;
                            }
                            break;
                    }
                }
            }

            throw new System.Exception("Unexpectedly reached end of string");
        }
        #endregion
        #region ParseString2
        private string ParseString2() {
            ConsumeToken(); // "

            _builder.Length = 0;

            int runIndex = -1;
            int l = _rawString.Length;
            //fixed (char* p = json)
            string p = _rawString;
            {
                while (_index < l) {
                    var c = p[_index++];

                    if (c == '\'') {
                        if (runIndex != -1) {
                            if (_builder.Length == 0)
                                return _rawString.Substring(runIndex, _index - runIndex - 1);

                            _builder.Append(_rawString, runIndex, _index - runIndex - 1);
                        }
                        return _builder.ToString();
                    }

                    if (c != '\\') {
                        if (runIndex == -1)
                            runIndex = _index - 1;

                        continue;
                    }

                    if (_index == l) break;

                    if (runIndex != -1) {
                        _builder.Append(_rawString, runIndex, _index - runIndex - 1);
                        runIndex = -1;
                    }

                    switch (p[_index++]) {
                        case '"':
                            _builder.Append('"');
                            break;

                        case '\\':
                            _builder.Append('\\');
                            break;

                        case '/':
                            _builder.Append('/');
                            break;

                        case 'b':
                            _builder.Append('\b');
                            break;

                        case 'f':
                            _builder.Append('\f');
                            break;

                        case 'n':
                            _builder.Append('\n');
                            break;

                        case 'r':
                            _builder.Append('\r');
                            break;

                        case 't':
                            _builder.Append('\t');
                            break;

                        case 'u': {
                                int remainingLength = l - _index;
                                if (remainingLength < 4) break;

                                // parse the 32 bit hex into an integer codepoint
                                uint codePoint = ParseUnicode(p[_index], p[_index + 1], p[_index + 2], p[_index + 3]);
                                _builder.Append((char)codePoint);

                                // skip 4 chars
                                _index += 4;
                            }
                            break;
                    }
                }
            }

            throw new System.Exception("Unexpectedly reached end of string");
        }
        #endregion
        #region ParseString3
        private string ParseString3() {
            ConsumeToken(); // "

            _builder.Length = 0;

            int runIndex = -1;
            int l = _rawString.Length;
            //fixed (char* p = json)
            string p = _rawString;
            {
                while (_index < l) {
                    var c = p[_index++];

                    if (c == ' ' || c == ',' || c == ':' || (c == '/' && (p[_index] == '/' || p[_index] == '*'))) {
                        _index--;
                        if (runIndex != -1) {
                            if (_builder.Length == 0)
                                return _rawString.Substring(runIndex, _index - runIndex);

                            _builder.Append(_rawString, runIndex, _index - runIndex);
                        }
                        return _builder.ToString();
                    }

                    if (c != '\\') {
                        if (runIndex == -1)
                            runIndex = _index - 1;

                        continue;
                    }

                    if (_index == l) break;

                    if (runIndex != -1) {
                        _builder.Append(_rawString, runIndex, _index - runIndex - 1);
                        runIndex = -1;
                    }

                    switch (p[_index++]) {
                        case '"':
                            _builder.Append('"');
                            break;

                        case '\\':
                            _builder.Append('\\');
                            break;

                        case '/':
                            _builder.Append('/');
                            break;

                        case 'b':
                            _builder.Append('\b');
                            break;

                        case 'f':
                            _builder.Append('\f');
                            break;

                        case 'n':
                            _builder.Append('\n');
                            break;

                        case 'r':
                            _builder.Append('\r');
                            break;

                        case 't':
                            _builder.Append('\t');
                            break;

                        case 'u': {
                                int remainingLength = l - _index;
                                if (remainingLength < 4) break;

                                // parse the 32 bit hex into an integer codepoint
                                uint codePoint = ParseUnicode(p[_index], p[_index + 1], p[_index + 2], p[_index + 3]);
                                _builder.Append((char)codePoint);

                                // skip 4 chars
                                _index += 4;
                            }
                            break;
                    }
                }
            }

            throw new System.Exception("Unexpectedly reached end of string");
        }
        #endregion
        #region ParseSingleChar
        private uint ParseSingleChar(char c1, uint multipliyer) {
            uint p1 = 0;
            if (c1 >= '0' && c1 <= '9')
                p1 = (uint)(c1 - '0') * multipliyer;
            else if (c1 >= 'A' && c1 <= 'F')
                p1 = (uint)((c1 - 'A') + 10) * multipliyer;
            else if (c1 >= 'a' && c1 <= 'f')
                p1 = (uint)((c1 - 'a') + 10) * multipliyer;
            return p1;
        }
        #endregion
        #region ParseUnicode
        private uint ParseUnicode(char c1, char c2, char c3, char c4) {
            uint p1 = ParseSingleChar(c1, 0x1000);
            uint p2 = ParseSingleChar(c2, 0x100);
            uint p3 = ParseSingleChar(c3, 0x10);
            uint p4 = ParseSingleChar(c4, 1);

            return p1 + p2 + p3 + p4;
        }
        #endregion
        #region CreateLong
        private long CreateLong(string s) {
            long num = 0;
            bool neg = false;
            foreach (char cc in s) {
                if (cc == '-')
                    neg = true;
                else if (cc == '+')
                    neg = false;
                else {
                    num *= 10;
                    num += (int)(cc - '0');
                }
            }

            return neg ? -num : num;
        }
        #endregion
        #region ParseNumber
        private object ParseNumber() {
            ConsumeToken();

            // Need to start back one place because the first digit is also a token and would have been consumed
            var startIndex = _index - 1;
            bool dec = false;
            do {
                if (_index == _rawString.Length)
                    break;
                var c = _rawString[_index];

                if ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E') {
                    if (c == '.' || c == 'e' || c == 'E')
                        dec = true;
                    if (++_index == _rawString.Length)
                        break;//throw new Exception("Unexpected end of string whilst parsing number");
                    continue;
                }
                break;
            } while (true);

            if (dec) {
                string s = _rawString.Substring(startIndex, _index - startIndex);
                return double.Parse(s, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            return JSON.CreateLong(_rawString, startIndex, _index - startIndex);
        }
        #endregion
        #region LookAhead
        private JsonNodeTypes LookAhead() {
            if (_lastNodeType != JsonNodeTypes.None) return _lastNodeType;

            return _lastNodeType = NextNodeCore();
        }
        #endregion
        #region ConsumeToken
        private void ConsumeToken() {
            _lastNodeType = JsonNodeTypes.None;
        }
        #endregion
        #region NextNode
        private JsonNodeTypes NextNode() {
            var result = _lastNodeType != JsonNodeTypes.None ? _lastNodeType : NextNodeCore();

            _lastNodeType = JsonNodeTypes.None;

            return result;
        }
        #endregion
        #region NextNodeCore
        private JsonNodeTypes NextNodeCore() {
            char c;

            // Skip past whitespace
            do {
                c = _rawString[_index];

                if (c == '/' && _rawString[_index + 1] == '/') // c++ style single line comments
                {
                    //index++;
                    //index++;
                    _index += 2;
                    do {
                        c = _rawString[_index];
                        if (c == '\r' || c == '\n') break; // read till end of line
                    }
                    while (++_index < _rawString.Length);
                }
                if (c == '/' && _rawString[_index + 1] == '*') {// c++ style block comments

                    _index += 2;
                    do {
                        c = _rawString[_index];
                        if (c == '*' && _rawString[_index + 1] == '/') {
                            _index += 2;
                            c = _rawString[_index];
                            break;// block end;
                        }
                    } while (++_index < _rawString.Length);
                }
                if (c > ' ') break;
                if (c != ' ' && c != '\t' && c != '\n' && c != '\r') break;

            } while (++_index < _rawString.Length);

            if (_index == _rawString.Length) {
                throw new System.Exception("Reached end of string unexpectedly");
            }

            c = _rawString[_index];

            _index++;

            switch (c) {
                case '{':
                    return JsonNodeTypes.Curly_Open;

                case '}':
                    return JsonNodeTypes.Curly_Close;

                case '[':
                    return JsonNodeTypes.Squared_Open;

                case ']':
                    return JsonNodeTypes.Squared_Close;

                case ',':
                    return JsonNodeTypes.Comma;
                case '\'':
                    return JsonNodeTypes.String2;
                case '"':
                    return JsonNodeTypes.String;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                case '+':
                case '.':
                    return JsonNodeTypes.Number;

                case ':':
                    return JsonNodeTypes.Colon;

                case 'f':
                    if (_rawString.Length - _index >= 4 &&
                        _rawString[_index + 0] == 'a' &&
                        _rawString[_index + 1] == 'l' &&
                        _rawString[_index + 2] == 's' &&
                        _rawString[_index + 3] == 'e') {
                        _index += 4;
                        return JsonNodeTypes.False;
                    }
                    //break;
                    _index--;
                    return JsonNodeTypes.String3;
                case 't':
                    if (_rawString.Length - _index >= 3 &&
                        _rawString[_index + 0] == 'r' &&
                        _rawString[_index + 1] == 'u' &&
                        _rawString[_index + 2] == 'e') {
                        _index += 3;
                        return JsonNodeTypes.True;
                    }
                    //break;
                    _index--;
                    return JsonNodeTypes.String3;

                case 'n':
                    if (_rawString.Length - _index >= 3 &&
                        _rawString[_index + 0] == 'u' &&
                        _rawString[_index + 1] == 'l' &&
                        _rawString[_index + 2] == 'l') {
                        _index += 3;
                        return JsonNodeTypes.Null;
                    }
                    _index--;
                    return JsonNodeTypes.String3;
                case 'u':
                    if (_rawString.Length - _index >= 8 &&//undefined
                        _rawString[_index + 0] == 'n' &&
                        _rawString[_index + 1] == 'd' &&
                        _rawString[_index + 2] == 'e' &&
                        _rawString[_index + 3] == 'f' &&
                        _rawString[_index + 4] == 'i' &&
                        _rawString[_index + 5] == 'n' &&
                        _rawString[_index + 6] == 'e' &&
                        _rawString[_index + 7] == 'd') {
                        _index += 8;
                        return JsonNodeTypes.Undefined;
                    }
                    _index--;
                    return JsonNodeTypes.String3;
                default:
                    _index--;
                    return JsonNodeTypes.String3;
                    //break;
            }
            throw new System.Exception("Could not find token at index " + --_index);
        }
        #endregion

        #endregion

        #region types

        #region JsonNodeTypesc
        enum JsonNodeTypes {
            None = -1,           // Used to denote no Lookahead available
            Curly_Open,
            Curly_Close,
            Squared_Open,
            Squared_Close,
            Colon,
            Comma,
            String,
            String2,
            String3,
            Number,
            True,
            False,
            Null,
            Undefined
        }
        #endregion

        #endregion
    }
    #endregion
    #region JsonFormatter
    internal static class JsonFormatter {
        #region fields
        public static string Indent = "   ";
        #endregion

        #region methods

        #region AppendIndent
        public static void AppendIndent(System.Text.StringBuilder builder, int count) {
            for (; count > 0; --count)
                builder.Append(Indent);
        }
        #endregion
        #region PrettyPrint
        public static string PrettyPrint(string input) {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            int depth = 0;
            int len = input.Length;
            char[] chars = input.ToCharArray();
            for (int i = 0; i < len; ++i) {
                char ch = chars[i];

                if (ch == '\"') // found string span
                {
                    bool str = true;
                    while (str) {
                        output.Append(ch);
                        ch = chars[++i];
                        if (ch == '\\') {
                            output.Append(ch);
                            ch = chars[++i];
                        } else if (ch == '\"')
                            str = false;
                    }
                }

                switch (ch) {
                    case '{':
                    case '[':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, ++depth);
                        break;
                    case '}':
                    case ']':
                        output.AppendLine();
                        AppendIndent(output, --depth);
                        output.Append(ch);
                        break;
                    case ',':
                        output.Append(ch);
                        output.AppendLine();
                        AppendIndent(output, depth);
                        break;
                    case ':':
                        output.Append(" : ");
                        break;
                    default:
                        if (!char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }
        #endregion

        #endregion
    }
    #endregion
    #region JSONSerializer
    internal sealed class JSONSerializer {

        #region fields
        private System.Text.StringBuilder _output = new System.Text.StringBuilder();
        //private StringBuilder _before = new StringBuilder();
        private int _before;
        private int _MAX_DEPTH = 20;
        private int _current_depth = 0;
        private Dictionary<string, int> _globalTypes = new Dictionary<string, int>();
        private Dictionary<object, int> _cirobj = new Dictionary<object, int>();
        private JSONParameters _params;
        private bool _useEscapedUnicode = false;
        private bool _typesWritten = false;
        private static readonly long DatetimeMinTimeTicks;

        #endregion

        #region cctor
        static JSONSerializer() {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DatetimeMinTimeTicks = time.Ticks;
        }
        #endregion
        #region ctor
        internal JSONSerializer(JSONParameters param) {
            _params = param;
            _useEscapedUnicode = _params.UseEscapedUnicode;
            _MAX_DEPTH = _params.SerializerMaxDepth;
        }
        #endregion

        #region methods

        #region ConvertToJSON
        internal string ConvertToJSON(object obj) {
            WriteValue(obj);

            if (_params.UsingGlobalTypes && _globalTypes != null && _globalTypes.Count > 0) {
                System.Text.StringBuilder buildeer = new System.Text.StringBuilder();
                buildeer.Append("\"$types\":{");
                var pendingSeparator = false;
                foreach (var kv in _globalTypes) {
                    if (pendingSeparator) buildeer.Append(',');
                    pendingSeparator = true;
                    buildeer.Append('\"');
                    buildeer.Append(kv.Key);
                    buildeer.Append("\":\"");
                    buildeer.Append(kv.Value);
                    buildeer.Append('\"');
                }
                buildeer.Append("},");
                _output.Insert(_before, buildeer.ToString());
            }
            return _output.ToString();
        }
        #endregion
        #region WriteValue
        private void WriteValue(object obj) {
            if (obj == null || obj is DBNull)
                _output.Append("null");

            else if (obj is string || obj is char)
                WriteString(obj.ToString());

            else if (obj is Guid)
                WriteGuid((Guid)obj);

            else if (obj is bool)
                _output.Append(((bool)obj) ? "true" : "false"); // conform to standard

            else if (
                obj is int || obj is long ||
                obj is decimal ||
                obj is byte || obj is short ||
                obj is sbyte || obj is ushort ||
                obj is uint || obj is ulong
            )
                _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));

            else if (obj is double || obj is Double) {
                double d = (double)obj;
                if (double.IsNaN(d))
                    _output.Append("\"NaN\"");
                else
                    _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));
            } else if (obj is float || obj is Single) {
                float d = (float)obj;
                if (float.IsNaN(d))
                    _output.Append("\"NaN\"");
                else
                    _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));
            } else if (obj is DateTime)
                WriteDateTime((DateTime)obj);

            else if (obj is DateTimeOffset)
                WriteDateTimeOffset((DateTimeOffset)obj);
            else if (obj is TimeSpan) {
                WriteString(obj.ToString());
            } else if (_params.KVStyleStringDictionary == false && obj is IDictionary &&
#if netcore
            obj.GetType().GetTypeInfo().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))
#else
            obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))
#endif

                WriteStringDictionary((IDictionary)obj);
#if net40
            else if (_params.KVStyleStringDictionary == false && obj is System.Dynamic.ExpandoObject)
                WriteStringDictionary((IDictionary<string, object>)obj);
#endif
            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);

            else if (obj is byte[])
                WriteBytes((byte[])obj);

            else if (obj is StringDictionary)
                WriteSD((StringDictionary)obj);

            else if (obj is NameValueCollection)
                WriteNV((NameValueCollection)obj);
            else if (obj is System.Text.Encoding) {
                WriteString(((System.Text.Encoding)obj).WebName);
            } else if (obj is IEnumerable)
                WriteArray((IEnumerable)obj);

            else if (obj is Enum)
                WriteEnum((Enum)obj);

            else if (Reflection.Instance.IsTypeRegistered(obj.GetType()))
                WriteCustom(obj);

            else
                WriteObject(obj);
        }
        #endregion
        #region WriteDateTimeOffset
        private void WriteDateTimeOffset(DateTimeOffset d) {
            Write_Date_value(d.DateTime);
            _output.Append(" ");
            if (d.Offset.Hours > 0)
                _output.Append("+");
            else
                _output.Append("-");
            _output.Append(d.Offset.Hours.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(":");
            _output.Append(d.Offset.Minutes);

            _output.Append('\"');
        }
        #endregion
        #region WriteNV
        private void WriteNV(NameValueCollection nameValueCollection) {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (string key in nameValueCollection) {
                if (_params.SerializeNullValues == false && (nameValueCollection[key] == null)) {
                } else {
                    if (pendingSeparator) _output.Append(',');
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(key.ToLower(), nameValueCollection[key]);
                    else
                        WritePair(key, nameValueCollection[key]);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }
        #endregion
        #region WriteSD
        private void WriteSD(StringDictionary stringDictionary) {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in stringDictionary) {
                if (_params.SerializeNullValues == false && (entry.Value == null)) {
                } else {
                    if (pendingSeparator) _output.Append(',');

                    string k = (string)entry.Key;
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(k.ToLower(), entry.Value);
                    else
                        WritePair(k, entry.Value);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }
        #endregion
        #region WriteCustom
        private void WriteCustom(object obj) {
            JsonSerializeDelegate s;
            Reflection.Instance._customSerializer.TryGetValue(obj.GetType(), out s);
            WriteStringFast(s(obj));
        }
        #endregion
        #region WriteEnum
        private void WriteEnum(Enum e) {
            // FEATURE : optimize enum write
            if (_params.UseValuesOfEnums)
                WriteValue(Convert.ToInt64(e));
            else
                WriteStringFast(e.ToString());
        }
        #endregion
        #region WriteGuid
        private void WriteGuid(Guid g) {
            if (_params.UseFastGuid == false)
                WriteStringFast(g.ToString());
            else
                WriteBytes(g.ToByteArray());
        }
        #endregion
        #region WriteBytes
        private void WriteBytes(byte[] bytes) {
#if !SILVERLIGHT && !netcore
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
#else
        WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length));
#endif
        }
        #endregion
        #region WriteDateTime
        private void WriteDateTime(DateTime dateTime) {
            // datetime format standard : yyyy-MM-dd HH:mm:ss
            DateTime dt = dateTime;
            if (_params.UseUTCDateTime)
                dt = dateTime.ToUniversalTime();

            Write_Date_value(dt);


        }
        #endregion
        #region Write_Date_value
        private void Write_Date_value(DateTime dt) {
            if (_params.UseJsonDateTime) {
                _output.Append("\"\\/Date(");
                _output.Append((long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 0x2710));
                _output.Append(")\\/\"");
                //_output.AppendFormat("\\/Date({0})\\/", JsTick(dt));
            } else if (_params.UseJavascriptDateTime) {
                _output.Append("new Date(");
                _output.Append((long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 0x2710));
                _output.Append(")");
            } else {
                _output.Append('\"');
                _output.Append(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
                _output.Append('-');
                _output.Append(dt.Month.ToString("00", NumberFormatInfo.InvariantInfo));
                _output.Append('-');
                _output.Append(dt.Day.ToString("00", NumberFormatInfo.InvariantInfo));
                _output.Append('T'); // strict ISO date compliance 
                _output.Append(dt.Hour.ToString("00", NumberFormatInfo.InvariantInfo));
                _output.Append(':');
                _output.Append(dt.Minute.ToString("00", NumberFormatInfo.InvariantInfo));
                _output.Append(':');
                _output.Append(dt.Second.ToString("00", NumberFormatInfo.InvariantInfo));
                if (_params.DateTimeMilliseconds) {
                    _output.Append('.');
                    _output.Append(dt.Millisecond.ToString("000", NumberFormatInfo.InvariantInfo));
                }
                if (_params.UseUTCDateTime)
                    _output.Append('Z');

                _output.Append('\"');
                //_output.Append('\"');
            }
        }
        #endregion

        #region WriteObject
        private void WriteObject(object obj) {
            int i = 0;
            if (_cirobj.TryGetValue(obj, out i) == false)
                _cirobj.Add(obj, _cirobj.Count + 1);
            else {
                if (_current_depth > 0 && _params.InlineCircularReferences == false) {
                    //_circular = true;
                    _output.Append("{\"$i\":");
                    _output.Append(i.ToString());
                    _output.Append("}");
                    return;
                }
            }
            if (_params.UsingGlobalTypes == false)
                _output.Append('{');
            else {
                if (_typesWritten == false) {
                    _output.Append('{');
                    _before = _output.Length;
                    //_output = new StringBuilder();
                } else
                    _output.Append('{');
            }
            _typesWritten = true;
            _current_depth++;
            if (_current_depth > _MAX_DEPTH)
                throw new Exception("Serializer encountered maximum depth of " + _MAX_DEPTH);


            Dictionary<string, string> map = new Dictionary<string, string>();
            Type t = obj.GetType();
            bool append = false;
            if (_params.UseExtensions) {
                if (_params.UsingGlobalTypes == false)
                    WritePairFast("$type", Reflection.Instance.GetTypeAssemblyName(t));
                else {
                    int dt = 0;
                    string ct = Reflection.Instance.GetTypeAssemblyName(t);
                    if (_globalTypes.TryGetValue(ct, out dt) == false) {
                        dt = _globalTypes.Count + 1;
                        _globalTypes.Add(ct, dt);
                    }
                    WritePairFast("$type", dt.ToString());
                }
                append = true;
            }

            Getters[] g = Reflection.Instance.GetGetters(t, _params.ShowReadOnlyProperties, _params.IgnoreAttributes);
            int c = g.Length;
            for (int ii = 0; ii < c; ii++) {
                var p = g[ii];
                object o = p.Getter(obj);
                if (_params.SerializeNullValues == false && (o == null || o is DBNull)) {
                    //append = false;
                } else {
                    if (append)
                        _output.Append(',');
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(p.LowerCaseName, o);
                    else
                        WritePair(p.Name, o);
                    if (o != null && _params.UseExtensions) {
                        Type tt = o.GetType();
                        if (tt == typeof(System.Object))
                            map.Add(p.Name, tt.ToString());
                    }
                    append = true;
                }
            }
            if (map.Count > 0 && _params.UseExtensions) {
                _output.Append(",\"$map\":");
                WriteStringDictionary(map);
            }
            _output.Append('}');
            _current_depth--;
        }
        #endregion
        #region WritePairFast
        private void WritePairFast(string name, string value) {
            WriteStringFast(name);

            _output.Append(':');

            WriteStringFast(value);
        }
        #endregion
        #region WritePair
        private void WritePair(string name, object value) {
            WriteString(name);

            _output.Append(':');

            WriteValue(value);
        }
        #endregion
        #region WriteArray
        private void WriteArray(IEnumerable array) {
            _output.Append('[');

            bool pendingSeperator = false;

            foreach (object obj in array) {
                if (pendingSeperator) _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.Append(']');
        }
        #endregion
        #region WriteStringDictionary
        private void WriteStringDictionary(IDictionary dic) {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic) {
                if (_params.SerializeNullValues == false && (entry.Value == null)) {
                } else {
                    if (pendingSeparator) _output.Append(',');

                    string k = (string)entry.Key;
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(k.ToLower(), entry.Value);
                    else
                        WritePair(k, entry.Value);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }
        #endregion
        #region WriteStringDictionary
        private void WriteStringDictionary(IDictionary<string, object> dic) {
            _output.Append('{');
            bool pendingSeparator = false;
            foreach (KeyValuePair<string, object> entry in dic) {
                if (_params.SerializeNullValues == false && (entry.Value == null)) {
                } else {
                    if (pendingSeparator) _output.Append(',');
                    string k = entry.Key;

                    if (_params.SerializeToLowerCaseNames)
                        WritePair(k.ToLower(), entry.Value);
                    else
                        WritePair(k, entry.Value);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }
        #endregion
        #region WriteDictionary
        private void WriteDictionary(IDictionary dic) {
            //_output.Append('[');
            _output.Append('{');
            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic) {
                if (pendingSeparator) _output.Append(',');

                WriteValue(entry.Key);
                _output.Append(':');
                WriteValue(entry.Value);

                //WritePair("k", entry.Key);
                //_output.Append(",");
                //WritePair("v", entry.Value);
                //_output.Append('}');

                pendingSeparator = true;
            }
            //_output.Append(']');
            _output.Append('}');
        }
        #endregion
        #region WriteStringFast
        private void WriteStringFast(string s) {
            _output.Append('\"');
            _output.Append(s);
            _output.Append('\"');
        }
        #endregion
        #region WriteString
        private void WriteString(string s) {
            _output.Append('\"');

            int runIndex = -1;
            int l = s.Length;
            for (var index = 0; index < l; ++index) {
                var c = s[index];

                if (_useEscapedUnicode) {
                    if (c >= ' ' && c < 128 && c != '\"' && c != '\\') {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                } else {
                    if (c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\')// && c != ':' && c!=',')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }

                if (runIndex != -1) {
                    _output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c) {
                    case '\t': _output.Append("\\t"); break;
                    case '\r': _output.Append("\\r"); break;
                    case '\n': _output.Append("\\n"); break;
                    case '"':
                    case '\\': _output.Append('\\'); _output.Append(c); break;
                    default:
                        if (_useEscapedUnicode) {
                            _output.Append("\\u");
                            _output.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        } else
                            _output.Append(c);

                        break;
                }
            }

            if (runIndex != -1)
                _output.Append(s, runIndex, s.Length - runIndex);

            _output.Append('\"');
        }
        #endregion

        #endregion

    }
    #endregion
    #region PropertyInfoTypes
    internal enum PropertyInfoTypes {
        Int,
        Long,
        String,
        Bool,
        DateTime,
        Enum,
        Guid,

        Array,
        ByteArray,
        Dictionary,
        StringKeyDictionary,
        NameValue,
        StringDictionary,
#if !SILVERLIGHT && !netcore
        Hashtable,
        DataSet,
        DataTable,
#endif
        Custom,
        Unknown,
    }
    #endregion
    #region CachePropertyInfo
    internal struct CachePropertyInfo {
        public Type MemberType;
        public Type ElementType;
        public Type ChangeType;
        public Reflection.GenericSetter Setter;
        public Reflection.GenericGetter Getter;
        public Type[] GenericTypes;
        public string Name;
        public PropertyInfoTypes Type;
        public bool CanWrite;

        public bool IsClass;
        public bool IsValueType;
        public bool IsGenericType;
        public bool IsStruct;
        public bool IsInterface;
    }
    #endregion
    #region Getters
    struct Getters {
        public string Name;
        public string LowerCaseName;
        public Reflection.GenericGetter Getter;
    }
    #endregion
    #region Reflection
    internal sealed class Reflection {

        #region fields
        // Sinlgeton pattern 4 from : http://csharpindepth.com/articles/general/singleton.aspx
        private static readonly Reflection instance = new Reflection();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        private SafeDictionary<Type, string> _tyname = new SafeDictionary<Type, string>();
        private SafeDictionary<string, Type> _typecache = new SafeDictionary<string, Type>();
        private SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
        private SafeDictionary<Type, Getters[]> _getterscache = new SafeDictionary<Type, Getters[]>();
        private SafeDictionary<string, Dictionary<string, CachePropertyInfo>> _propertycache = new SafeDictionary<string, Dictionary<string, CachePropertyInfo>>();
        private SafeDictionary<Type, Type[]> _genericTypes = new SafeDictionary<Type, Type[]>();
        private SafeDictionary<Type, Type> _genericTypeDef = new SafeDictionary<Type, Type>();

        #region bjson custom types
        internal UnicodeEncoding _unicode = new UnicodeEncoding();
        internal UTF8Encoding _utf8 = new UTF8Encoding();
        #endregion
        #region json custom types
        // JSON custom
        internal SafeDictionary<Type, JsonSerializeDelegate> _customSerializer = new SafeDictionary<Type, JsonSerializeDelegate>();
        internal SafeDictionary<Type, JsonDeserializeDelegate> _customDeserializer = new SafeDictionary<Type, JsonDeserializeDelegate>();
        #endregion

        #endregion

        #region properties
        public static Reflection Instance { get { return instance; } }
        #endregion

        #region methods

        #region CreateCustom
        internal object CreateCustom(string v, Type type) {
            JsonDeserializeDelegate d;
            _customDeserializer.TryGetValue(type, out d);
            return d(v);
        }
        #endregion
        #region RegisterCustomType
        internal void RegisterCustomType(Type type, JsonSerializeDelegate serializer, JsonDeserializeDelegate deserializer) {
            if (type != null && serializer != null && deserializer != null) {
                _customSerializer.Add(type, serializer);
                _customDeserializer.Add(type, deserializer);
                // reset property cache
                Instance.ResetPropertyCache();
            }
        }
        #endregion
        #region IsTypeRegistered
        internal bool IsTypeRegistered(Type t) {
            if (_customSerializer.Count == 0)
                return false;
            JsonSerializeDelegate s;
            return _customSerializer.TryGetValue(t, out s);
        }
        #endregion

        #region GetGenericTypeDefinition
        public Type GetGenericTypeDefinition(Type t) {
            Type tt = null;
            if (_genericTypeDef.TryGetValue(t, out tt))
                return tt;
            else {
                tt = t.GetGenericTypeDefinition();
                _genericTypeDef.Add(t, tt);
                return tt;
            }
        }
        #endregion
        #region GetGenericArguments
        public Type[] GetGenericArguments(Type t) {
            Type[] tt = null;
            if (_genericTypes.TryGetValue(t, out tt))
                return tt;
            else {
                tt = t.GetGenericArguments();
                _genericTypes.Add(t, tt);
                return tt;
            }
        }
        #endregion
        #region GetProperties
        public System.Collections.Generic.Dictionary<string, CachePropertyInfo> GetProperties(Type type, string typename) {
            System.Collections.Generic.Dictionary<string, CachePropertyInfo> result = null;
            if (_propertycache.TryGetValue(typename, out result)) {
                return result;
            } else {
                result = new System.Collections.Generic.Dictionary<string, CachePropertyInfo>();
                var bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                PropertyInfo[] pr = type.GetProperties(bf);
                foreach (PropertyInfo p in pr) {
                    if (p.GetIndexParameters().Length > 0)// Property is an indexer
                        continue;

                    CachePropertyInfo d = CreatePropertyInfo(p.PropertyType, p.Name);
                    d.Setter = Reflection.CreateSetMethod(type, p);
                    if (d.Setter != null)
                        d.CanWrite = true;
                    d.Getter = Reflection.CreateGetMethod(type, p);
                    result.Add(p.Name, d);
                    string lowerName = "lower." + p.Name.ToLower();
                    if (!result.ContainsKey(lowerName))
                        result.Add(lowerName, d);
                }
                FieldInfo[] fi = type.GetFields(bf);
                foreach (FieldInfo f in fi) {
                    CachePropertyInfo d = CreatePropertyInfo(f.FieldType, f.Name);
                    if (f.IsLiteral == false) {
                        d.Setter = Reflection.CreateSetField(type, f);
                        if (d.Setter != null)
                            d.CanWrite = true;
                        d.Getter = Reflection.CreateGetField(type, f);
                        result.Add(f.Name, d);

                        string lowerName = "lower." + f.Name.ToLower();
                        if (!result.ContainsKey(lowerName))
                            result.Add(lowerName, d);
                    }
                }

                _propertycache.Add(typename, result);
                return result;
            }
        }
        #endregion
        #region CreatePropertyInfo
        private CachePropertyInfo CreatePropertyInfo(Type type, string name) {
            CachePropertyInfo d = new CachePropertyInfo();
            PropertyInfoTypes d_type = PropertyInfoTypes.Unknown;
#if netcore
        var type2 = type.GetTypeInfo();
#else
            var type2 = type;
#endif

            if (type == typeof(int) || type == typeof(int?)) d_type = PropertyInfoTypes.Int;
            else if (type == typeof(long) || type == typeof(long?)) d_type = PropertyInfoTypes.Long;
            else if (type == typeof(string)) d_type = PropertyInfoTypes.String;
            else if (type == typeof(bool) || type == typeof(bool?)) d_type = PropertyInfoTypes.Bool;
            else if (type == typeof(DateTime) || type == typeof(DateTime?)) d_type = PropertyInfoTypes.DateTime;
            else if (type2.IsEnum) d_type = PropertyInfoTypes.Enum;
            else if (type == typeof(Guid) || type == typeof(Guid?)) d_type = PropertyInfoTypes.Guid;
            else if (type == typeof(StringDictionary)) d_type = PropertyInfoTypes.StringDictionary;
            else if (type == typeof(NameValueCollection)) d_type = PropertyInfoTypes.NameValue;
            else if (type.IsArray) {
                d.ElementType = type.GetElementType();
                if (type == typeof(byte[]))
                    d_type = PropertyInfoTypes.ByteArray;
                else
                    d_type = PropertyInfoTypes.Array;
            } else if (type.Name.Contains("Dictionary")) {
                d.GenericTypes = Reflection.Instance.GetGenericArguments(type);
                if (d.GenericTypes.Length > 0 && d.GenericTypes[0] == typeof(string))
                    d_type = PropertyInfoTypes.StringKeyDictionary;
                else
                    d_type = PropertyInfoTypes.Dictionary;
            }
            else if (IsTypeRegistered(type))
                d_type = PropertyInfoTypes.Custom;

            if (type2.IsValueType && !type2.IsPrimitive && !type2.IsEnum && type != typeof(decimal))
                d.IsStruct = true;

            d.IsInterface = type2.IsInterface;
            d.IsClass = type2.IsClass;
            d.IsValueType = type2.IsValueType;
            if (type2.IsGenericType) {
                d.IsGenericType = true;
                d.ElementType = type.GetGenericArguments()[0];
            }

            d.MemberType = type;
            d.Name = name;
            d.ChangeType = GetChangeType(type);
            d.Type = d_type;

            return d;
        }
        #endregion
        #region GetChangeType
        private Type GetChangeType(Type conversionType) {
#if netcore
        if (conversionType.GetTypeInfo().IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
#else
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
#endif
                return Reflection.Instance.GetGenericArguments(conversionType)[0];

            return conversionType;
        }
        #endregion

        #region [   PROPERTY GET SET   ]

        internal string GetTypeAssemblyName(Type t) {
            string val = "";
            if (_tyname.TryGetValue(t, out val))
                return val;
            else {
                string s = t.AssemblyQualifiedName;
                _tyname.Add(t, s);
                return s;
            }
        }

        internal Type GetTypeFromCache(string typename) {
            Type val = null;
            if (_typecache.TryGetValue(typename, out val))
                return val;
            else {
                Type t = Type.GetType(typename);
                //if (t == null) // RaptorDB : loading runtime assemblies
                //{
                //    t = Type.GetType(typename, (name) => {
                //        return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                //    }, null, true);
                //}
                _typecache.Add(typename, t);
                return t;
            }
        }

        internal object FastCreateInstance(Type objtype) {
            try {
                CreateObject c = null;
                if (_constrcache.TryGetValue(objtype, out c)) {
                    return c();
                } else {
#if netcore
                if (objtype.GetTypeInfo().IsClass)
#else
                    if (objtype.IsClass)
#endif
                {
                        DynamicMethod dynMethod = new DynamicMethod("DM_" + System.Guid.NewGuid().ToString("N"), objtype, null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                        ilGen.Emit(OpCodes.Ret);
                        c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrcache.Add(objtype, c);
                    } else // structs
                        {
                        DynamicMethod dynMethod = new DynamicMethod("DM_" + System.Guid.NewGuid().ToString("N"), typeof(object), null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        var lv = ilGen.DeclareLocal(objtype);
                        ilGen.Emit(OpCodes.Ldloca_S, lv);
                        ilGen.Emit(OpCodes.Initobj, objtype);
                        ilGen.Emit(OpCodes.Ldloc_0);
                        ilGen.Emit(OpCodes.Box, objtype);
                        ilGen.Emit(OpCodes.Ret);
                        c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrcache.Add(objtype, c);
                    }
                    return c();
                }
            } catch (Exception exc) {
                throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'",
                    objtype.FullName, objtype.AssemblyQualifiedName), exc);
            }
        }

        internal static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo) {
            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod dynamicSet = new DynamicMethod(type.FullName + ".set_" + fieldInfo.Name, typeof(object), arguments, type);

            ILGenerator il = dynamicSet.GetILGenerator();

#if netcore
        if (!type.GetTypeInfo().IsClass) // structs
#else
            if (!type.IsClass) // structs
#endif
        {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
#if netcore
            if (fieldInfo.FieldType.GetTypeInfo().IsClass)
#else
                if (fieldInfo.FieldType.IsClass)
#endif
                    il.Emit(OpCodes.Castclass, fieldInfo.FieldType);
                else
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            } else {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
#if netcore
            if (fieldInfo.FieldType.GetTypeInfo().IsValueType)
#else
                if (fieldInfo.FieldType.IsValueType)
#endif
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);
            }
            return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
        }

        internal static GenericSetter CreateSetMethod(Type type, PropertyInfo propertyInfo) {
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;

            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod setter = new DynamicMethod(TypeExtensions.FullName2(type) + ".set_" + propertyInfo.Name, typeof(object), arguments);
            ILGenerator il = setter.GetILGenerator();

#if netcore
        if (!type.GetTypeInfo().IsClass) // structs
#else
            if (!type.IsClass) // structs
#endif
        {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
#if netcore
            if (propertyInfo.PropertyType.GetTypeInfo().IsClass)
#else
                if (propertyInfo.PropertyType.IsClass)
#endif
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                else
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Call, setMethod, null);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
            } else {
                if (!setMethod.IsStatic) {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                    il.Emit(OpCodes.Ldarg_1);
#if netcore
                if (propertyInfo.PropertyType.GetTypeInfo().IsClass)
#else
                    if (propertyInfo.PropertyType.IsClass)
#endif
                        il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                    else
                        il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    il.EmitCall(OpCodes.Callvirt, setMethod, null);
                    il.Emit(OpCodes.Ldarg_0);
                } else {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
#if netcore
                if (propertyInfo.PropertyType.GetTypeInfo().IsClass)
#else
                    if (propertyInfo.PropertyType.IsClass)
#endif
                        il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                    else
                        il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    il.Emit(OpCodes.Call, setMethod);
                }
            }

            il.Emit(OpCodes.Ret);

            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }

        internal static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo) {
            if (TypeExtensions.IsAnonymousType(type)) {
                GenericGetter getter_any = fieldInfo.GetValue;
                return getter_any;
            }

            DynamicMethod dynamicGet = new DynamicMethod(TypeExtensions.FullName2(type) + ".get_" + fieldInfo.Name, typeof(object), new Type[] { typeof(object) }, type);

            ILGenerator il = dynamicGet.GetILGenerator();

#if netcore
        if (!type.GetTypeInfo().IsClass) // structs
#else
            if (!type.IsClass) // structs
#endif
        {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
#if netcore
            if (fieldInfo.FieldType.GetTypeInfo().IsValueType)
#else
                if (fieldInfo.FieldType.IsValueType)
#endif
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            } else {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
#if netcore
            if (fieldInfo.FieldType.GetTypeInfo().IsValueType)
#else
                if (fieldInfo.FieldType.IsValueType)
#endif
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
        }

        internal static GenericGetter CreateGetMethod(Type type, PropertyInfo propertyInfo) {
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            if (TypeExtensions.IsAnonymousType(type)) {
                GenericGetter getter_any = (p) => getMethod.Invoke(p, new object[0]);
                return getter_any;
            }
            DynamicMethod getter = new DynamicMethod(TypeExtensions.FullName2(type) + ".get_" + propertyInfo.Name, typeof(object), new Type[] { typeof(object) }, type);

            ILGenerator il = getter.GetILGenerator();

#if netcore
        if (!type.GetTypeInfo().IsClass) // structs
#else
            if (!type.IsClass) // structs
#endif
                {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
#if netcore
        if (propertyInfo.PropertyType.GetTypeInfo().IsValueType)
#else
                if (propertyInfo.PropertyType.IsValueType)
#endif
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            } else {
                if (!getMethod.IsStatic) {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                    il.EmitCall(OpCodes.Callvirt, getMethod, null);
                } else
                    il.Emit(OpCodes.Call, getMethod);

#if netcore
            if (propertyInfo.PropertyType.GetTypeInfo().IsValueType)
#else
                if (propertyInfo.PropertyType.IsValueType)
#endif
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }
        void ScanExtras(List<Getters> getters, GenericGetter g, IParameterInfo parameterInfo) {
            if (g == null)
                return;

#if netcore
        Type typeInfo = parameterInfo.Type.GetTypeInfo();
#else
            Type typeInfo = parameterInfo.Type;
#endif
            if (typeInfo.IsEnum
                || (TypeExtensions.IsNullableType(parameterInfo.Type) && TypeExtensions.GetNullableType(parameterInfo.Type).IsEnum)
            ) {
                ScanExtras_Enum(getters, g, parameterInfo);
                return;
            }
            ScanExtras_ExtraPath(getters, g, parameterInfo);
        }
        void ScanExtras_Enum(List<Getters> getters, GenericGetter g, IParameterInfo parameterInfo) {
            if (AttributeExtensions.IsDefined<ExtraEnumStringAttribute>(parameterInfo)) {
                // add tostring
                getters.Add(new Getters() {
                    Getter = (p) => {
                        var value = g(p);
                        return value == null ? "" : value.ToString();
                    },
                    LowerCaseName = parameterInfo.Name.ToLower() + "string",
                    Name = parameterInfo.Name + "String",
                });
            }
            if (AttributeExtensions.IsDefined<ExtraEnumTextAttribute>(parameterInfo)) {
                // add toname
                getters.Add(new Getters() {
                    Getter = (p) => {
                        var value = g(p);
                        return value == null ? "" : EnumExtensions.ToName(TypeExtensions.Convert<long>(value));
                    },
                    LowerCaseName = parameterInfo.Name.ToLower() + "text",
                    Name = parameterInfo.Name + "Text",
                });
            }

        }

        void ScanExtras_ExtraPath(List<Getters> getters, GenericGetter g, IParameterInfo parameterInfo) {
            foreach (var info in AttributeExtensions.GetCustomAttributes<ExtraPathAttribute>(parameterInfo, false)) {
                ScanExtras_ExtraPath_Item(getters, g, parameterInfo, info);
            }
        }
        void ScanExtras_ExtraPath_Item(List<Getters> getters, GenericGetter g, IParameterInfo parameterInfo, ExtraPathAttribute info) {
            if (string.IsNullOrEmpty(info.Name) || string.IsNullOrEmpty(info.Path))
                return;
            GenericGetter getter;
            if (string.IsNullOrEmpty(info.Format)) {
                getter = (p) => {
                    return FastObject.Path(g(p), info.Path) ?? info.DefaultValue;
                };
            } else {
                string format = info.Format.IndexOf('{') == -1 ? "{0:" + info.Format + "}" : info.Format;
                getter = (p) => {
                    object value = FastObject.Path(g(p), info.Path) ?? info.DefaultValue;
                    if (value == null)
                        return null;
                    return string.Format(format, value);
                };
            }
            getters.Add(new Getters() {
                Getter = getter,
                LowerCaseName = info.Name.ToLower(),
                Name = info.Name,
            });
        }
        internal Getters[] GetGetters(Type type, bool ShowReadOnlyProperties, List<Type> IgnoreAttributes) {
            Getters[] val = null;
            if (_getterscache.TryGetValue(type, out val))
                return val;

            //bool isAnonymous = IsAnonymousType(type);

            var bf = BindingFlags.Public | BindingFlags.Instance;// | BindingFlags.Static;
                                                                 //if (ShowReadOnlyProperties)
                                                                 //    bf |= BindingFlags.NonPublic;
            PropertyInfo[] props = type.GetProperties(bf);
            List<Getters> getters = new List<Getters>();
            foreach (PropertyInfo p in props) {
                if (p.GetIndexParameters().Length > 0) {// Property is an indexer
                    continue;
                }
                if (!p.CanWrite && (ShowReadOnlyProperties == false))//|| isAnonymous == false))
                    continue;
                if (IgnoreAttributes != null) {
                    bool found = false;
                    foreach (var ignoreAttr in IgnoreAttributes) {
                        if (Symbol.AttributeExtensions.IsDefined(p, ignoreAttr, false)) {
                            found = true;
                            break;
                        }
                    }
                    if (found) {

                        ScanExtras(getters, CreateGetMethod(type, p), PropertyParameterInfo.As(p));
                        continue;
                    }
                }
                GenericGetter g = CreateGetMethod(type, p);
                if (g != null) {
                    IParameterInfo parameterInfo = PropertyParameterInfo.As(p);
                    g = GetterFilter(g, parameterInfo);
                    getters.Add(new Getters { Getter = g, Name = p.Name, LowerCaseName = p.Name.ToLower() });
                    ScanExtras(getters, g, parameterInfo);
                }
            }

            FieldInfo[] fi = type.GetFields(bf);
            foreach (var f in fi) {
                if (IgnoreAttributes != null) {
                    bool found = false;
                    foreach (var ignoreAttr in IgnoreAttributes) {
                        if (f.IsDefined(ignoreAttr, false)) {
                            found = true;
                            break;
                        }
                    }
                    if (found) {
                        ScanExtras(getters, CreateGetField(type, f), FieldParameterInfo.As(f));
                        continue;
                    }
                }
                if (f.IsLiteral == false) {
                    GenericGetter g = CreateGetField(type, f);
                    if (g != null) {
                        IParameterInfo parameterInfo = FieldParameterInfo.As(f);
                        g = GetterFilter(g, parameterInfo);
                        getters.Add(new Getters { Getter = g, Name = f.Name, LowerCaseName = f.Name.ToLower() });
                        ScanExtras(getters, g, parameterInfo);
                    }
                }
            }
            val = getters.ToArray();
            _getterscache.Add(type, val);
            return val;
        }
        GenericGetter GetterFilter(GenericGetter g, IParameterInfo parameterInfo) {
            return GetterFilter_NullAsEmpty(
                    GetterFilter_Format(g, parameterInfo),
                    parameterInfo);
        }
        GenericGetter GetterFilter_Format(GenericGetter g, IParameterInfo parameterInfo) {
            var info = AttributeExtensions.GetCustomAttribute<FormatAttribute>(parameterInfo, false);
            if (info == null || string.IsNullOrEmpty(info.Format))
                return g;
            var old_g = g;
            string format = info.Format.IndexOf('{') == -1 ? "{0:" + info.Format + "}" : info.Format;
            return (x) => {
                var v = old_g(x);
                if (v == null)
                    return null;
                return string.Format(format, v);
            };
        }
        GenericGetter GetterFilter_NullAsEmpty(GenericGetter g, IParameterInfo parameterInfo) {
            if (!AttributeExtensions.IsDefined<NullAsEmptyAttribute>(parameterInfo, false))
                return g;

            var old_g = g;
            return (x) => {
                var v = old_g(x);
                if (v == null)
                    v = "";
                return v;
            };
        }

        //private static bool IsAnonymousType(Type type)
        //{
        //    // may break in the future if compiler defined names change...
        //    const string CS_ANONYMOUS_PREFIX = "<>f__AnonymousType";
        //    const string VB_ANONYMOUS_PREFIX = "VB$AnonymousType";

        //    if (type == null)
        //        throw new ArgumentNullException("type");

        //    if (type.Name.StartsWith(CS_ANONYMOUS_PREFIX, StringComparison.Ordinal) || type.Name.StartsWith(VB_ANONYMOUS_PREFIX, StringComparison.Ordinal))
        //    {
        //        return type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        //    }

        //    return false;
        //}
        #endregion
        #region ResetPropertyCache
        internal void ResetPropertyCache() {
            _propertycache = new SafeDictionary<string, Dictionary<string, CachePropertyInfo>>();
        }
        #endregion
        #region ClearReflectionCache
        internal void ClearReflectionCache() {
            _tyname = new SafeDictionary<Type, string>();
            _typecache = new SafeDictionary<string, Type>();
            _constrcache = new SafeDictionary<Type, CreateObject>();
            _getterscache = new SafeDictionary<Type, Getters[]>();
            _propertycache = new SafeDictionary<string, Dictionary<string, CachePropertyInfo>>();
            _genericTypes = new SafeDictionary<Type, Type[]>();
            _genericTypeDef = new SafeDictionary<Type, Type>();
        }
        #endregion

        #endregion

        #region types
        internal delegate object GenericSetter(object target, object value);
        internal delegate object GenericGetter(object obj);
        private delegate object CreateObject();
        #endregion
    }
    #region SafeDictionary
    class SafeDictionary<TKey, TValue> {

        #region fields
        private readonly object _padlock = new object();
        private readonly Dictionary<TKey, TValue> _values;
        #endregion

        #region properties

        #region Count
        public int Count { get { lock (_padlock) return _values.Count; } }
        #endregion
        #region this[TKey key]
        public TValue this[TKey key] {
            get {
                lock (_padlock)
                    return _values[key];
            }
            set {
                lock (_padlock)
                    _values[key] = value;
            }
        }
        #endregion

        #endregion

        #region ctor
        public SafeDictionary(int capacity) {
            _values = new Dictionary<TKey, TValue>(capacity);
        }
        public SafeDictionary() {
            _values = new Dictionary<TKey, TValue>();
        }
        #endregion

        #region methods

        #region TryGetValue
        public bool TryGetValue(TKey key, out TValue value) {
            lock (_padlock)
                return _values.TryGetValue(key, out value);
        }
        #endregion
        #region Add
        public void Add(TKey key, TValue value) {
            lock (_padlock) {
                if (_values.ContainsKey(key) == false)
                    _values.Add(key, value);
            }
        }
        #endregion

        #endregion
    }
    #endregion

}
#endregion




#endregion

#pragma warning restore 1591
