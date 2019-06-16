/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 数据绑定对象缓存
    /// </summary>
    public class DataBinderObjectCache : IDataBinderObjectCache {

        #region fields
        private System.Collections.Concurrent.ConcurrentDictionary<object, System.Collections.Concurrent.ConcurrentDictionary<string, object>> _list_object;
        private System.Collections.Concurrent.ConcurrentDictionary<string, object> _list_key;
        #endregion

        #region properties

        #endregion

        #region ctor
        /// <summary>
        /// 创建DataBinderObjectCache实例
        /// </summary>
        public DataBinderObjectCache() {
            _list_object = new System.Collections.Concurrent.ConcurrentDictionary<object, System.Collections.Concurrent.ConcurrentDictionary<string, object>>();
            _list_key = new System.Collections.Concurrent.ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region methods


        #region key

        #region Get
        /// <summary>
        /// 获取（按Key）
        /// </summary>
        /// <param name="key">键值。</param>
        /// <param name="value">输出缓存。</param>
        /// <returns>返回true表示有缓存。</returns>
        public bool Get(string key, out object value) {
            value = null;
            if (string.IsNullOrEmpty(key))
                return false;
            return ThreadHelper.InterlockedGet(ref _list_key)?.TryGetValue(key, out value) ?? false;
        }
        #endregion
        #region Set
        /// <summary>
        /// 设置（按Key）
        /// </summary>
        /// <param name="key">键值。</param>
        /// <param name="value">缓存数据。</param>
        public void Set(string key, object value) {
            if (string.IsNullOrEmpty(key))
                return;
            var list = ThreadHelper.InterlockedGet(ref _list_key);
            if (list == null)
                return;
            if (!list.TryAdd(key, value)) {
                list.TryUpdate(key, value, value);
            }
        }
        #endregion

        #endregion

        #region entity,field

        
        #region Get
        /// <summary>
        /// 获取（按对象和字段）
        /// </summary>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="value">输出缓存。</param>
        /// <returns>返回true表示有缓存。</returns>
        public bool Get(object entity, string field, out object value) {
            value = null;
            if (entity == null || string.IsNullOrEmpty(field))
                return false;

            var list_object = ThreadHelper.InterlockedGet(ref _list_object);
            if (list_object == null)
                return false;
            System.Collections.Concurrent.ConcurrentDictionary<string, object> list;
            if (!list_object.TryGetValue(entity, out list))
                return false;
            return list.TryGetValue(field, out value);
        }
        #endregion
        #region Set
        /// <summary>
        /// 设置（按对象和字段）
        /// </summary>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="value">缓存数据。</param>
        public void Set(object entity, string field, object value) {
            if (entity == null || string.IsNullOrEmpty(field))
                return;
            var list_object = ThreadHelper.InterlockedGet(ref _list_object);
            if (list_object == null)
                return;
            var list = list_object.GetOrAdd(entity, (p) => new System.Collections.Concurrent.ConcurrentDictionary<string, object>());
            if (!list.TryAdd(field, value)) {
                list.TryUpdate(field, value, value);
            }
        }
        #endregion

        #endregion


        #region Dispose
        /// <summary>
        /// 释放所有资源。
        /// </summary>
        public void Dispose() {
            ThreadHelper.InterlockedSet(ref _list_object, null)?.Clear();
            ThreadHelper.InterlockedSet(ref _list_key, null)?.Clear();
        }
        #endregion

        #endregion


    }

}