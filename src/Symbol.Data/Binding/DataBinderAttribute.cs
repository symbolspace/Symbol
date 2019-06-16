/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {
    
    /// <summary>
    /// 数据绑定特性基类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class DataBinderAttribute : Attribute, Symbol.Data.Binding.IDataBinder {

        #region fields
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<BindItem>> _list_key=new System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<BindItem>>();
        #endregion

        #region properties
        /// <summary>
        /// 获取源名称。
        /// </summary>
        public string SourceName { get; private set; }
        /// <summary>
        /// 获取过虑规则。
        /// </summary>
        public string Condition { get; private set; }
        /// <summary>
        /// 获取排序规则。
        /// </summary>
        public Symbol.Data.NoSQL.Sorter Sorter { get; private set; }
        /// <summary>
        /// 获取或设置输出字段。
        /// </summary>
        public string Field { get; set; }
        ///// <summary>
        ///// 获取或设置元素Path，设置后将对最终输出的值，调用一次Path。
        ///// </summary>
        //public string ElementPath { get; set; }
        /// <summary>
        /// 获取或设置允许缓存，默认为true。
        /// </summary>
        public bool AllowCache { get; set; }

        #endregion


        #region ctor
        /// <summary>
        /// 创建DataBinderAttribute实例。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        /// <param name="condition">过虑规则。</param>
        /// <param name="field">输出字段，输出为单值时。</param>
        /// <param name="sort">排序规则。</param>
        public DataBinderAttribute(string sourceName, string condition, string field = "*", string sort = "{}") {
            AllowCache = true;
            SourceName = sourceName;
            Field = field;
            //Condition = Symbol.Data.NoSQL.Condition.Parse(condition);
            Condition = condition;
            Sorter = Symbol.Data.NoSQL.Sorter.Parse(sort);
        }
        #endregion

        #region methods

        #region Bind
        /// <summary>
        /// 绑定数据。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        /// <param name="reader">数据读取对象。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="type">类型。</param>
        /// <param name="cache">缓存。</param>
        public static void Bind(IDataContext dataContext, IDataQueryReader reader, object entity, System.Type type, IDataBinderObjectCache cache) {
            if (entity == null)
                return;
            var list = TryGetBind(type);
            if (list == null || list.Count == 0)
                return;
            for (int i = 0; i < list.Count; i++) {
                list[i].Bind(dataContext, reader, entity, cache);
            }
        }


        /// <summary>
        /// 绑定数据。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        /// <param name="reader">数据查询读取器。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="type">实体中字段的类型。</param>
        /// <param name="cache">缓存。</param>
        /// <returns>返回绑定的数据。</returns>
        public abstract object Bind(IDataContext dataContext, IDataQueryReader reader, object entity, string field, Type type, IDataBinderObjectCache cache);
        #endregion

     
        #region BuildCacheKey
        /// <summary>
        /// 构造缓存键值。
        /// </summary>
        /// <param name="builder">select命令构造器。</param>
        /// <param name="tag">标记。</param>
        /// <param name="type">类型。</param>
        /// <returns>返回缓存键值。</returns>
        protected virtual string BuildCacheKey(Symbol.Data.ISelectCommandBuilder builder, string tag, Type type) {
            return string.Concat(
                    tag, ":",
                    type.AssemblyQualifiedName,"|",
                    builder.CommandText, "/",
                    Symbol.Serialization.Json.ToString(builder.Parameters, true)
                  );
        }
        /// <summary>
        /// 缓存键值操作。
        /// </summary>
        /// <param name="cache">缓存对象。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="func">缓存求值委托。</param>
        /// <returns>返回</returns>
        protected virtual object CacheFunc(IDataBinderObjectCache cache, object entity, string field, CacheValueFunc func) {
            if (cache != null && AllowCache) {
                object value;
                if (!cache.Get(entity, field, out value)) {
                    value = func();
                    cache.Set(entity, field, value);
                }
                return value;
            } else {
                return func();
            }
        }

        /// <summary>
        /// 缓存键值操作。
        /// </summary>
        /// <param name="cache">缓存对象。</param>
        /// <param name="builder">select命令构造器。</param>
        /// <param name="tag">标记。</param>
        /// <param name="type">类型。</param>
        /// <param name="func">缓存求值委托。</param>
        /// <returns>返回</returns>
        protected virtual object CacheFunc(IDataBinderObjectCache cache, Symbol.Data.ISelectCommandBuilder builder, string tag, Type type, CacheValueFunc func) {
            if (cache != null && AllowCache) {
                object value;
                string key = BuildCacheKey(builder, tag, type);
                if (!cache.Get(key, out value)) {
                    value = func();
                    cache.Set(key, value);
                }
                return value;
            } else {
                return func();
            }
        }
        #endregion

        #region TryGetBind
        static System.Collections.Generic.List<BindItem> TryGetBind(System.Type entityType) {
            string key = entityType.AssemblyQualifiedName;
            var list = _list_key.GetOrAdd(key, (p) => {
                if (entityType.IsValueType || entityType == typeof(string) || entityType == typeof(object) || TypeExtensions.IsNullableType(entityType)) {
                    return null;
                }
                var cache = new System.Collections.Generic.List<BindItem>();
                foreach (System.Reflection.PropertyInfo propertyInfo in entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.SetProperty)) {
                    var binder = AttributeExtensions.GetCustomAttribute<DataBinderAttribute>(propertyInfo);
                    if (binder == null)
                        continue;
                    BindItem bindItem = new BindItem() {
                        propertyInfo = propertyInfo,
                        binder = binder,
                        bindAction = binder.Bind,
                    };
                    cache.Add(bindItem);
                }
                return cache;
            });
            return list;
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// 缓存求值委托。
        /// </summary>
        /// <returns>返回缓存值。</returns>
        protected delegate object CacheValueFunc();
        delegate object BindAction(IDataContext dataContext, IDataQueryReader reader, object entity, string field, Type type, IDataBinderObjectCache cache);
        class BindItem {
            public System.Reflection.PropertyInfo propertyInfo;
            public DataBinderAttribute binder;
            public BindAction bindAction;

            public void Bind(IDataContext dataContext, IDataQueryReader reader, object entity, IDataBinderObjectCache cache) {
                if (cache != null && binder.AllowCache) {
                    object value;
                    if (!cache.Get(entity, propertyInfo.Name, out value)) {
                        value = binder.Bind(dataContext, reader, entity, propertyInfo.Name, propertyInfo.PropertyType, cache);
                        cache.Set(entity, propertyInfo.Name, value);
                    }
                    propertyInfo.SetValue(entity, value, null);
                } else {
                    var value2 = binder.Bind(dataContext, reader, entity, propertyInfo.Name, propertyInfo.PropertyType, cache);
                    propertyInfo.SetValue(entity, value2, null);
                }
            }
        }
        class ObjectMapper {
            private IDataContext _dataContext;
            private object _entity;
            private IDataQueryReader _reader;
            private FastObject _fastObject;
            public ObjectMapper(string expression, IDataContext dataContext, object entity, IDataQueryReader reader) {
                _fastObject = expression;
                _dataContext = dataContext;
                _entity = entity;
                _reader = reader;
            }

            public object Map() {
                if (_fastObject.Instance == null)
                    return null;
                if (_fastObject.Instance is System.Collections.IList list) {
                    MapArray(list);
                } else if (_fastObject.Instance is System.Collections.Generic.IDictionary<string, object> dictionary) {
                    MapObject(dictionary);
                    return _fastObject.Instance;
                }
                return _fastObject.Instance;
            }

            void MapObject(System.Collections.Generic.IDictionary<string, object> map) {
                var fastObject = new FastObject(map);
                foreach (var item in LinqHelper.ToArray(map)) {
                    if (item.Value == null)
                        continue;
                    if (item.Value is string text) {
                        MapValue(fastObject, item.Key, text);
                        continue;
                    }
                    if (item.Value is System.Collections.IList list) {
                        MapArray(list);
                        continue;
                    }
                    if (item.Value is System.Collections.Generic.IDictionary<string, object> dictionary) {
                        MapObject(dictionary);
                    }

                }
            }
            void MapArray(System.Collections.IList list) {
                FastObject fastObject = new FastObject(list);
                for (int i = 0; i < list.Count; i++) {
                    object value = list[i];
                    if (value == null)
                        continue;
                    if (value is string text) {
                        MapValue(fastObject, $"[{i}]", text);
                        continue;
                    }
                    if (value is System.Collections.IList list2) {
                        MapArray(list2);
                        continue;
                    }
                    if (value is System.Collections.Generic.IDictionary<string, object> dictionary) {
                        MapObject(dictionary);
                    }
                }
            }
            void MapValue(FastObject fastObject, string key, string expression) {
                if (string.IsNullOrEmpty(expression))
                    return;
                if (expression[0] != '$')
                    return;
                if (expression.StartsWith("$this.", StringComparison.OrdinalIgnoreCase)) {
                    string path = expression.Substring("$this.".Length);
                    fastObject[key] = FastObject.Path(_entity, path);
                }
                if (expression.StartsWith("$reader.", StringComparison.OrdinalIgnoreCase)) {
                    string p10 = expression.Substring("$reader.".Length);
                    if (p10.IndexOf('.') > -1) {
                        string name = p10.Split('.')[0];
                        string path = p10.Substring(name.Length + 1);
                        fastObject[key] = FastObject.Path(_reader.GetValue(name, null), path);
                    } else {
                        fastObject[key] = _reader.GetValue(p10, null);
                    }
                }
            }
            public void Dispose() {
                _dataContext = null;
                _entity = null;
                _reader = null;
                _fastObject = null;
            }

        }
        /// <summary>
        /// 映射对象值
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="dataContext"></param>
        /// <param name="entity">实体对象</param>
        /// <param name="reader">数据查询读取器。</param>
        /// <returns></returns>
        public static object MapObject(string expression, IDataContext dataContext, object entity, IDataQueryReader reader) {
            if (string.IsNullOrEmpty(expression))
                return null;
            var mapper = new ObjectMapper(expression, dataContext, entity, reader);
            var result = mapper.Map();
            mapper.Dispose();
            return result;
        }

        #endregion

    }
}