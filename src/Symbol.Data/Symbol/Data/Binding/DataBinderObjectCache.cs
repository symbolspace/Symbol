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
        private System.Collections.Generic.Dictionary<object, System.Collections.Generic.Dictionary<string, object>> _list_object;
        private System.Collections.Generic.Dictionary<string, object> _list_key;
        #endregion

        #region properties

        #endregion

        #region ctor
        /// <summary>
        /// 创建DataBinderObjectCache实例
        /// </summary>
        public DataBinderObjectCache() {
            _list_object = new System.Collections.Generic.Dictionary<object, System.Collections.Generic.Dictionary<string, object>>();
            _list_key = new System.Collections.Generic.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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
            return _list_key.TryGetValue(key, out value);
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
            object value2;
            //if (!_list_key.TryGetValue(key, out value2)) {
                ThreadHelper.Block(_list_key, () => {
                    if (!_list_key.TryGetValue(key, out value2)) {
                        _list_key.Add(key, value);
                    } else {
                        _list_key[key] = value;
                    }
                });
            //} else {
            //    ThreadHelper.Block(_list_key, () => {
            //        _list_key[key] = value;
            //    });
            //}
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


            System.Collections.Generic.Dictionary<string, object> list;
            if (!_list_object.TryGetValue(entity, out list))
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
            System.Collections.Generic.Dictionary<string, object> list;
            if (!_list_object.TryGetValue(entity, out list)) {
                ThreadHelper.Block(_list_object, () => {
                    if (!_list_object.TryGetValue(entity, out list)) {
                        list = new System.Collections.Generic.Dictionary<string, object>();
                        _list_object.Add(entity, list);
                    }
                });
            }
            object value2;
            //if (!list.TryGetValue(field, out value2)) {
                ThreadHelper.Block(list, () => {
                    if (!list.TryGetValue(field, out value2)) {
                        list.Add(field, value);
                    } else {
                        list[field] = value;
                    }
                });
            //} else {
            //    ThreadHelper.Block(list, () => {
            //        list[field] = value;
            //    });
            //}
        }
        #endregion

        #endregion


        #region Dispose
        /// <summary>
        /// 释放所有资源。
        /// </summary>
        public void Dispose() {
            if (_list_object != null) {
                _list_object.Clear();
            }
            _list_object = null;
            if (_list_key != null) {
                _list_key.Clear();
            }
            _list_key = null;
        }
        #endregion

        #endregion


    }

}