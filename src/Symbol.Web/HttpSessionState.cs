/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对会话状态值以及会话级别设置和生存期管理方法的访问。
    /// </summary>
    public class HttpSessionState :
        System.MarshalByRefObject,
        IHttpSessionState {
        
        #region fields
        private int _timeout;
        private string _sessionId;
        private System.Collections.Generic.Dictionary<string, object> _values;
        private System.Timers.Timer _timer;

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, HttpSessionState> _globals;
        #endregion

        #region cctor
        static HttpSessionState() {
            _globals = new System.Collections.Concurrent.ConcurrentDictionary<string, HttpSessionState>(System.StringComparer.OrdinalIgnoreCase);
        }
        #endregion
        #region ctor
        /// <summary>
        /// 创建HttpSessionState实例。
        /// </summary>
        /// <param name="sessionId">会话的唯一标识符。</param>
        /// <param name="timeout">在会话状态提供程序终止会话之前各请求之间所允许的时间（以分钟为单位）。</param>
        public HttpSessionState(string sessionId, int timeout) {
            _sessionId = sessionId;
            _timeout = timeout;
            _values = new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
            _timer = new System.Timers.Timer(timeout < 1 ? 1 : timeout * 60D * 1000D);
            _timer.AutoReset = false;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            if (_timer.Interval > 1)
                _timer.Start();

            Abandon();
        }
        void UpdateTimer() {
            if (_timer.Enabled) {
                _timer.Stop();
                if (_timer.Interval > 1)
                    _timer.Start();
            }
        }
        #endregion

        #region methods
        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            _timer.Elapsed -= _timer_Elapsed;
            _timer.Stop();
            _timer.Dispose();
            _values.Clear();
        }
        /// <summary>
        /// 创建全局唯一对象。
        /// </summary>
        /// <param name="sessionId">会话的唯一标识符。</param>
        /// <param name="timeout">在会话状态提供程序终止会话之前各请求之间所允许的时间（以分钟为单位）。</param>
        /// <param name="isNewSession">是否为新对象。</param>
        /// <returns></returns>
        public static IHttpSessionState Create(string sessionId, int timeout, out bool isNewSession) {
            if (string.IsNullOrEmpty(sessionId)) {
                isNewSession = true;
                return Create(timeout);
            }

            HttpSessionState result;
            bool b = false;
            if (!_globals.TryGetValue(sessionId, out result)) {
                ThreadHelper.Block(_globals, () => {
                    if (!_globals.TryGetValue(sessionId, out result)) {
                        b = true;
                        result = Create(timeout);
                    }
                });
            }
            isNewSession = b;

            return result;
        }
        static HttpSessionState Create(int timeout) {
            string sessionId = System.Guid.NewGuid().ToString("N");
            HttpSessionState o = new HttpSessionState(sessionId, timeout);
            _globals.TryAdd(sessionId, o);
            return o;
        }
        #endregion

        #region IHttpSessionState 成员

        /// <summary>
        /// 获取一个值，该值指示会话是否是与当前请求一起创建的。
        /// </summary>
        public bool IsNewSession {
            get { return false; }
        }

        /// <summary>
        /// 获取会话的唯一标识符。
        /// </summary>
        public string SessionID {
            get { return _sessionId; }
        }
        /// <summary>
        /// 获取并设置在会话状态提供程序终止会话之前各请求之间所允许的时间（以分钟为单位）。
        /// </summary>
        public int Timeout {
            get {
                return _timeout;
            }
            set {
                if (_timeout != value) {
                    _timeout = value;
                    if (value > 1 && _timer != null) {
                        _timer.Interval = value * 60D * 1000D;
                        if (!_timer.Enabled)
                            _timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个字符串数组，该数组包含此 Session 集合中的所有键（Session 名称）。
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
        /// 按数字索引获取或设置会话值。
        /// </summary>
        /// <param name="index">会话值的数字索引。</param>
        /// <returns>存储在指定索引处的会话状态值；如果该项不存在，则为 null。</returns>
        public object this[int index] {
            get {
                UpdateTimer();
                string key = GetKey(index);
                if (key == null)
                    return null;
                return this[key];
            }
            set {
                UpdateTimer();
                string key = GetKey(index);
                if (key == null)
                    return;
                this[key] = value;
            }
        }

        /// <summary>
        /// 按名称获取或设置会话值。
        /// </summary>
        /// <param name="name">会话值的键名。</param>
        /// <returns>具有指定名称的会话状态值；如果该项不存在，则为 null。</returns>
        public object this[string name] {
            get {
                UpdateTimer();
                if (name == null || !_values.ContainsKey(name))
                    return null;
                return _values[name];
            }
            set {
                UpdateTimer();
                if (name == null)
                    return;
                if (_values.ContainsKey(name))
                    _values[name] = value;
                else
                    _values.Add(name, value);
            }
        }

        /// <summary>
        /// 取消当前会话（实际效果是当前请求内还可以用（所有数据会清空），只是在下次请求时，它已经是新的了，原来的数据全没了。）。
        /// </summary>
        public void Abandon() {
            UpdateTimer();
            HttpSessionState o;
            _globals.TryRemove(_sessionId,out o);
            Clear();
        }

        /// <summary>
        /// 从会话状态集合中移除所有的键和值。
        /// </summary>
        public void Clear() {
            UpdateTimer();
            _values.Clear();
        }

        /// <summary>
        /// 向会话状态集合添加一个新项。
        /// </summary>
        /// <param name="name">要添加到会话状态集合的项的名称。</param>
        /// <param name="value">要添加到会话状态集合的项的值。</param>
        public void Add(string name, object value) {
            UpdateTimer();
            this[name] = value;
        }

        /// <summary>
        /// 删除会话状态集合中的项。
        /// </summary>
        /// <param name="name">要从会话状态集合中删除的项的名称。</param>
        public void Remove(string name) {
            UpdateTimer();
            if (name == null)
                return;
            _values.Remove(name);
        }

        /// <summary>
        /// 删除会话状态集合中指定索引处的项。
        /// </summary>
        /// <param name="index">要从会话状态集合中移除的项的索引。</param>
        public void RemoveAt(int index) {
            UpdateTimer();
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
        string GetKey(int index) {
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
            get {
                UpdateTimer();
                return _values.Count;
            }
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