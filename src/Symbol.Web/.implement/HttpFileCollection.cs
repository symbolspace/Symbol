/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

#if netcore
using Microsoft.AspNetCore.Http;
#endif

namespace Symbol.Web {
    /// <summary>
    /// 提供对客户端上载文件的访问，并组织这些文件。
    /// </summary>
    public class HttpFileCollection :
        System.MarshalByRefObject,
        IHttpFileCollection,
        System.IDisposable {
        #region fields
        private System.Collections.Generic.Dictionary<string, HttpPostedFile> _values = null;
        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpFileCollection实例。
        /// </summary>
        public HttpFileCollection() {
            _values = new System.Collections.Generic.Dictionary<string, HttpPostedFile>(System.StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region IHttpCookieCollection 成员

        /// <summary>
        /// 获取一个字符串数组，该数组包含文件集合中所有成员的键（名称）。
        /// </summary>
        public string[] AllKeys {
            get { return LinqHelper.ToArray(_values.Keys); }
        }

        /// <summary>
        /// 获取实例中的所有键。
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Keys {
            get { return _values.Keys; }
        }

        /// <summary>
        /// 获取具有指定数字索引的对象。
        /// </summary>
        /// <param name="index">要从文件集合中获取的项索引。</param>
        /// <returns>按 index 指定的 IHttpPostedFile。</returns>
        public IHttpPostedFile this[int index] {
            get {
                string key = GetKey(index);
                if (key == null)
                    return null;
                return this[key];
            }
        }

        /// <summary>
        /// 从文件集合中获取具有指定名称的对象。
        /// </summary>
        /// <param name="name">要返回的项名称。</param>
        /// <returns>按 name 指定的 IHttpPostedFile。</returns>
        public IHttpPostedFile this[string name] {
            get {
                if (name == null || !_values.ContainsKey(name))
                    return null;
                return _values[name];
            }
        }

        /// <summary>
        /// 返回具有指定数字索引的 IHttpPostedFile 成员名称。
        /// </summary>
        /// <param name="index">要返回的对象名索引。</param>
        /// <returns>按 index 指定的 IHttpPostedFile 成员的名称。</returns>
        public string GetKey(int index) {
            if (_values.Count == 0 || index < 0 || index > (_values.Count - 1))
                return null;

            int i = 0;
            foreach (string key in _values.Keys) {
                if (i == index)
                    return key;
                i++;
            }
            return null;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">文件</param>
        public void Add(string name, HttpPostedFile value) {
            if (string.IsNullOrEmpty(name))
                return;
            if (_values.ContainsKey(name))
                return;
            _values.Add(name, value);
        }
#if netcore
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">文件</param>
        public void Add(string name, IFormFile value) {
            if (string.IsNullOrEmpty(name))
                return;
            if (_values.ContainsKey(name))
                return;
            var file = new HttpPostedFile(value);
            _values.Add(name, file);
        }
#endif
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="name">名称</param>
        public void Remove(string name) {
            if (name == null)
                return;
            HttpPostedFile item;
            if (_values.TryGetValue(name, out item)) {
                Remove(item);
                _values.Remove(name);
            }
        }
        /// <summary>
        /// 移除（按索引）
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveAt(int index) {
            if (_values.Count == 0 || index < 0 || index > (_values.Count - 1))
                return;
            string key = GetKey(index);
            if (key == null)
                return;
            HttpPostedFile item;
            if (_values.TryGetValue(key, out item)) {
                Remove(item);
                _values.Remove(key);
            }
        }
        void Remove(HttpPostedFile item) {
            if (item == null)
                return;
            item.Dispose();
        }
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() {
            foreach (HttpPostedFile item in _values.Values) {
                Remove(item);
            }
            _values.Clear();
        }

        #endregion

        #region ICollection 成员
        /// <summary>
        /// 从特定的 System.Array 索引处开始，将 System.Collections.ICollection 的元素复制到一个 System.Array
        /// </summary>
        /// <param name="array">作为从 System.Collections.ICollection 复制的元素的目标位置的一维 System.Array。System.Array 必须具有从零开始的索引。</param>
        /// <param name="index">array 中从零开始的索引，将在此处开始复制。</param>
        public void CopyTo(System.Array array, int index) {
            ((System.Collections.ICollection)_values.Values).CopyTo(array, index);
        }
        /// <summary>
        /// 获取集合的数量。
        /// </summary>
        public int Count {
            get { return _values.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized {
            get { return ((System.Collections.ICollection)_values).IsSynchronized; }
        }

        object System.Collections.ICollection.SyncRoot {
            get { return ((System.Collections.ICollection)_values).SyncRoot; }
        }

        #endregion

        #region IEnumerable 成员
        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator() {
            return _values.Keys.GetEnumerator();
        }

        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 
        /// </summary>
        ~HttpFileCollection() {
            Dispose(false);
        }
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        void Dispose(bool disposing) {
            if (disposing) {
                if (_values != null) {
                    Clear();
                    _values = null;
                }
                //System.GC.Collect(0);
                //System.GC.Collect();
            }
        }
        #endregion
    }
}