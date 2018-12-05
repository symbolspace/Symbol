/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 数据绑定对象缓存接口
    /// </summary>
    public interface IDataBinderObjectCache : IDisposable {

        /// <summary>
        /// 获取（按Key）
        /// </summary>
        /// <param name="key">键值。</param>
        /// <param name="value">输出缓存。</param>
        /// <returns>返回true表示有缓存。</returns>
        bool Get(string key, out object value);
        /// <summary>
        /// 设置（按Key）
        /// </summary>
        /// <param name="key">键值。</param>
        /// <param name="value">缓存数据。</param>
        void Set(string key, object value);

        /// <summary>
        /// 获取（按对象和字段）
        /// </summary>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="value">输出缓存。</param>
        /// <returns>返回true表示有缓存。</returns>
        bool Get(object entity, string field, out object value);
        /// <summary>
        /// 设置（按对象和字段）
        /// </summary>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="value">缓存数据。</param>
        void Set(object entity, string field, object value);
    }


}