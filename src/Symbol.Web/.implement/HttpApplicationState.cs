/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 启用应用程序中多个会话和请求之间的全局信息共享（仅网站内）。
    /// </summary>
    public class HttpApplicationState :
        System.MarshalByRefObject,
        IHttpApplicationState {
        
        #region fields
        private System.Collections.Generic.Dictionary<string, object> _values = null;

        private static readonly System.Collections.Generic.Dictionary<IHttpApplication, HttpApplicationState> _globals = null;
        private static readonly object _syncGlobals = new object();
        #endregion

        #region cctor
        static HttpApplicationState() {
            _globals = new System.Collections.Generic.Dictionary<IHttpApplication, HttpApplicationState>();
        }
        #endregion
        #region ctor
        /// <summary>
        /// 创建HttpApplicationState实例。
        /// </summary>
        public HttpApplicationState() {
            _values = new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Create
        /// <summary>
        /// 创建应用实例对象对应的唯一集合。
        /// </summary>
        /// <param name="applicationInstance">应用实例对象。</param>
        /// <returns></returns>
        public static HttpApplicationState Create(IHttpApplication applicationInstance) {
            lock (_syncGlobals) {
                if (_globals.ContainsKey(applicationInstance)) {
                    HttpApplicationState result = new HttpApplicationState();
                    _globals.Add(applicationInstance, result);
                    return result;
                }
                return _globals[applicationInstance];
            }
        }
        #endregion

        #region IHttpApplicationState 成员

        /// <summary>
        /// 获取集合中的访问键。
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
        /// 通过索引获取单个对象。
        /// </summary>
        /// <param name="index">集合中对象的数字索引。</param>
        /// <returns>index 所引用的对象。</returns>
        public object this[int index] {
            get {
                string key = GetKey(index);
                if (key == null)
                    return null;
                return this[key];
            }
            set {
                string key = GetKey(index);
                if (key == null)
                    return;
                this[key] = value;
            }
        }

        /// <summary>
        /// 通过名称获取单个对象的值。
        /// </summary>
        /// <param name="name">集合中的对象名。</param>
        /// <returns>name 所引用的对象。</returns>
        public object this[string name] {
            get {
                if (name == null || !_values.ContainsKey(name))
                    return null;
                return _values[name];
            }
            set {
                if (name == null)
                    return;
                if (_values.ContainsKey(name))
                    _values[name] = value;
                else
                    _values.Add(name, value);
            }
        }

        /// <summary>
        /// 从集合中移除所有对象。
        /// </summary>
        public void Clear() {
            _values.Clear();
        }

        /// <summary>
        /// 将新的对象添加到集合中。
        /// </summary>
        /// <param name="name">要添加到集合中的对象名。</param>
        /// <param name="value">对象的值。</param>
        public void Add(string name, object value) {
            this[name] = value;
        }

        /// <summary>
        /// 从集合中移除命名对象。
        /// </summary>
        /// <param name="name">要从集合中移除的对象名。</param>
        public void Remove(string name) {
            if (name == null)
                return;
            _values.Remove(name);
        }

        /// <summary>
        /// 按索引从集合中移除一个对象。
        /// </summary>
        /// <param name="index">要移除的项在集合中的位置。</param>
        public void RemoveAt(int index) {
            if (_values.Count == 0 || index < 0 || index > (_values.Count - 1))
                return;
            string key = GetKey(index);
            if (key == null)
                return;
            _values.Remove(key);
        }

        int GetIndex(string name) {
            if (name == null || !_values.ContainsKey(name))
                return -1;
            return System.Array.FindIndex(LinqHelper.ToArray(_values.Keys), p => string.Equals(p, name, System.StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 通过索引获取对象名。
        /// </summary>
        /// <param name="index">应用程序状态对象的索引。</param>
        /// <returns>保存应用程序状态对象所使用的名称。</returns>
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
        /// 锁定变量的访问以促进访问同步（目前没有加锁，与其容易堵死不如混乱着用吧）。
        /// </summary>
        public void Lock() {
        }
        /// <summary>
        /// 取消锁定变量的访问以促进访问同步。
        /// </summary>
        public void UnLock() {
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
    }
}