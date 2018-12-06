/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供操作 HTTP Cookie 的类型安全方法。
    /// </summary>
    public class HttpCookieCollection :
        System.MarshalByRefObject,
        IHttpCookieCollection {
        #region fields
        private System.Collections.Generic.Dictionary<string, IHttpCookie> _values = null;

        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpCookieCollection实例
        /// </summary>
        public HttpCookieCollection() {
            _values = new System.Collections.Generic.Dictionary<string, IHttpCookie>(System.StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region IHttpCookieCollection 成员
        /// <summary>
        /// 获取一个字符串数组，该数组包含此 Cookie 集合中的所有键（Cookie 名称）。
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
        /// 从 Cookie 集合中获取具有指定数字索引的 Cookie。
        /// </summary>
        /// <param name="index">要从集合中检索的 Cookie 索引。</param>
        /// <returns>按 index 指定的 IHttpCookie。</returns>
        public IHttpCookie this[int index] {
            get {
                string key = GetKey(index);
                if (key == null)
                    return null;
                return this[key];
            }
        }

        /// <summary>
        /// 从 Cookie 集合中获取具有指定名称的 Cookie。
        /// </summary>
        /// <param name="name">要检索的 Cookie 名称。</param>
        /// <returns>由 name 指定的 IHttpCookie。</returns>
        public IHttpCookie this[string name] {
            get {
                if (name == null || !_values.ContainsKey(name))
                    return null;
                return _values[name];
            }
        }

        /// <summary>
        /// 获取具有指定数字索引的 Cookie 值（简化操作，直接获取值，未找到就是null。）。
        /// </summary>
        /// <param name="index">要从集合中检索的 Cookie 索引。</param>
        /// <returns>按 index 指定的 IHttpCookie 的Value。</returns>
        public string GetValue(int index) {
            IHttpCookie item = this[index];
            if (item == null)
                return null;
            return item.Value;
        }

        /// <summary>
        /// 获取具有指定名称的 Cookie 值（简化操作，直接获取值，未找到就是null。）。
        /// </summary>
        /// <param name="name">要检索的 Cookie 名称。</param>
        /// <returns>由 name 指定的 IHttpCookie 的Value。</returns>
        public string GetValue(string name) {
            IHttpCookie item = this[name];
            if (item == null)
                return null;
            return item.Value;
        }

        /// <summary>
        /// 返回指定数字索引处的 Cookie 键（名称）。
        /// </summary>
        /// <param name="index">要从集合中检索的键索引。</param>
        /// <returns>按 index 指定的 Cookie 的名称。</returns>
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
        /// 将指定的 Cookie 添加到此 Cookie 集合中。
        /// </summary>
        /// <param name="name">Cookie 名称。</param>
        /// <param name="value">Cookie 值。</param>
        /// <param name="path">路径，默认为 / ，如果设置为指定路径，将只有访问指定的路径时，客户端才会发送此Cookie。</param>
        /// <param name="domain">Cookie 限定的域名，一旦设置，只有访问此域名才会发送此Cookie。</param>
        /// <returns>返回创建的 IHttpCookie 。</returns>
        public IHttpCookie Add(string name, string value, string path = "/", string domain = null) {
            if (string.IsNullOrEmpty(name))
                return null;
            IHttpCookie item;
            if (_values.TryGetValue(name,out item)) {
                item.Value = value;
                item.Path = string.IsNullOrEmpty(path) ? "/" : path;
                item.Domain = domain;
            } else {
                item = new HttpCookie() {
                    Name = name,
                    Value = value,
                    Path = path,
                    Domain = domain,
                    Expires = System.DateTime.Now.AddYears(10),
                    HttpOnly = false,
                    Secure = false,
                };
                if (string.IsNullOrEmpty(item.Path))
                    item.Path = "/";
                _values.Add(name, item);
            }
            return item;
        }

        /// <summary>
        /// 从集合中移除具有指定名称的 Cookie。
        /// </summary>
        /// <param name="name">要从集合中移除的 Cookie 名称。</param>
        public void Remove(string name) {
            if (name == null)
                return;
            _values.Remove(name);
        }

        /// <summary>
        /// 从集合中移除具有数字索引的 Cookie。
        /// </summary>
        /// <param name="index">要从集合中移除的 Cookie 索引。</param>
        public void RemoveAt(int index) {
            if (_values.Count == 0 || index < 0 || index > (_values.Count - 1))
                return;
            string key = GetKey(index);
            if (key == null)
                return;
            _values.Remove(key);
        }

        /// <summary>
        /// 清除 Cookie 集合中的所有 Cookie。
        /// </summary>
        public void Clear() {
            _values.Clear();
        }

        #endregion

        #region ICollection 成员
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(System.Array array, int index) {
            ((System.Collections.ICollection)_values.Values).CopyTo(array, index);
        }
        /// <summary>
        /// 获取集合数量。
        /// </summary>
        public int Count {
            get { return _values.Count; }
        }
        /// <summary>
        /// 
        /// </summary>
        bool System.Collections.ICollection.IsSynchronized {
            get { return ((System.Collections.ICollection)_values).IsSynchronized; }
        }
        /// <summary>
        /// 
        /// </summary>
        object System.Collections.ICollection.SyncRoot {
            get { return ((System.Collections.ICollection)_values).SyncRoot; }
        }

        #endregion

        #region IEnumerable 成员
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator() {
            return _values.Keys.GetEnumerator();
        }

        #endregion
    }
}