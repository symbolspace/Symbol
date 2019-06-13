/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

/// <summary>
/// 线程辅助类。
/// </summary>
public class ThreadHelper {

    #region fields
    private static System.Threading.Mutex _instance;
    private static System.Collections.Concurrent.ConcurrentDictionary<string, ParallelLockContext> _list_parallelLock;
    #endregion

    #region cctor
    static ThreadHelper() {
        _list_parallelLock = new System.Collections.Concurrent.ConcurrentDictionary<string, ParallelLockContext>(System.StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region methods

    #region InterlockedGet
    /// <summary>
    /// 原子级获取变量值。
    /// </summary>
    /// <typeparam name="T">仅限class类型。</typeparam>
    /// <param name="var">变量，等同于：System.Threading.Interlocked.CompareExchange(ref var, null, null)。</param>
    /// <returns>返回变量的原始值。</returns>
    public static T InterlockedGet<T>(ref T @var) where T : class {
        return System.Threading.Interlocked.CompareExchange(ref @var, null, null);
    }
    /// <summary>
    /// 原子级设置变量值。
    /// </summary>
    /// <typeparam name="T">仅限class类型。</typeparam>
    /// <param name="var">变量，等同于：System.Threading.Interlocked.Exchange(ref var, value)。</param>
    /// <param name="value">值</param>
    /// <returns>返回变量的原始值。</returns>
    public static T InterlockedSet<T>(ref T @var,T value) where T : class {
        return System.Threading.Interlocked.Exchange(ref @var, value);
    }
    #endregion

    #region Sleep
    /// <summary>
    /// 线程休眠。
    /// </summary>
    /// <param name="millisecondsTimeout">时间，毫秒，&gt;-1时才执行。</param>
    public static void Sleep(int millisecondsTimeout) {
        if (millisecondsTimeout > -1)
            System.Threading.Thread.Sleep(millisecondsTimeout);
    }
    #endregion

    #region Delay
    /// <summary>
    /// 延迟执行代码段（注意这将会在后台线程中执行）。
    /// </summary>
    /// <param name="millisecondsTimeout">延时（毫秒）。</param>
    /// <param name="action">需要执行的代码。</param>
    public static System.Threading.Thread Delay(int millisecondsTimeout, System.Threading.ThreadStart action) {
        return Delay(millisecondsTimeout, action,
#if !netcore
            System.Threading.ApartmentState.Unknown,
#endif
            true);
    }
#if !netcore
    /// <summary>
    /// 延迟执行代码段（注意这将会在后台线程中执行）。
    /// </summary>
    /// <param name="millisecondsTimeout">延时（毫秒）。</param>
    /// <param name="action">需要执行的代码。</param>
    /// <param name="apartmentState">线程启动前的单元状态。</param>
    /// <param name="isBackground">是否为后台线程。</param>
    public static System.Threading.Thread Delay(
        int millisecondsTimeout, 
        System.Threading.ThreadStart action,
        System.Threading.ApartmentState apartmentState = System.Threading.ApartmentState.Unknown,
#else
    /// <summary>
    /// 延迟执行代码段（注意这将会在后台线程中执行）。
    /// </summary>
    /// <param name="millisecondsTimeout">延时（毫秒）。</param>
    /// <param name="action">需要执行的代码。</param>
    /// <param name="isBackground">是否为后台线程。</param>
    public static System.Threading.Thread Delay(
        int millisecondsTimeout, 
        System.Threading.ThreadStart action,
#endif
        bool isBackground = true) {
        System.Threading.Thread thread = new System.Threading.Thread(() => {
            if (millisecondsTimeout > -1)
                System.Threading.Thread.Sleep(millisecondsTimeout);
            action();
        }) { IsBackground = isBackground };
#if !netcore
        if (apartmentState != System.Threading.ApartmentState.Unknown)
            thread.SetApartmentState(apartmentState);
#endif
        thread.Start();
        return thread;
    }
    #endregion

    #region IsPrevInstance
    /// <summary>
    /// 判断某个互斥体已经存在。
    /// </summary>
    /// <param name="instanceName">互斥体实例名称。</param>
    /// <returns>返回判断结果。</returns>
    public static bool IsPrevInstance(string instanceName) {
        bool createdNew;
        _instance = new System.Threading.Mutex(true, instanceName, out createdNew); //同步基元变量
        if (createdNew)
            return false;
        return true;
    }
    #endregion
    #region ClosePrevInstance
    /// <summary>
    /// 关闭上次判断的互斥体，需要在调用IsPrevInstance之后。
    /// </summary>
    public static void ClosePrevInstance() {
        if (_instance != null) {
#if !netcore
            _instance.Close();
#endif
#if !net20 && !net35
            _instance.Dispose();
#endif
            _instance = null;
        }
    }
    #endregion

    #region Block System.Threading.ThreadStart => void x();
    /// <summary>
    /// 区块排它锁
    /// </summary>
    /// <param name="state">用于排它锁的对象，不能是结构类型和string。</param>
    /// <param name="action">进入锁之后的回调。</param>
    public static void Block(object state, System.Threading.ThreadStart action) {
        Symbol.CommonException.CheckArgumentNull(state, "state");
        Symbol.CommonException.CheckArgumentNull(action, "action");
        bool lockToken = false;
#if net20 || net35
        System.Threading.Monitor.Enter(state);
        lockToken=true;
#else
        System.Threading.Monitor.Enter(state, ref lockToken);
#endif
        try {
            action();
        } finally {
            if (lockToken) {
                System.Threading.Monitor.Exit(state);
            }
        }
    }
    #endregion
    #region Block BlockAction<T> => void x(T);
    /// <summary>
    /// 区块排它锁
    /// </summary>
    /// <param name="state">用于排它锁的对象，不能是结构类型和string。</param>
    /// <param name="action">进入锁之后的回调。</param>
    /// <param name="arg">参数。</param>
    public static void Block<T>(object state, BlockAction<T> action, T arg) {
        Symbol.CommonException.CheckArgumentNull(state, "state");
        Symbol.CommonException.CheckArgumentNull(action, "action");
        bool lockToken = false;
#if net20 || net35
        System.Threading.Monitor.Enter(state);
        lockToken=true;
#else
        System.Threading.Monitor.Enter(state, ref lockToken);
#endif
        try {
            action(arg);
        } finally {
            if (lockToken) {
                System.Threading.Monitor.Exit(state);
            }
        }
    }
    #endregion
    #region Block BlockFunc<T> => T x();
    /// <summary>
    /// 区块排它锁，带返回值。
    /// </summary>
    /// <param name="state">用于排它锁的对象，不能是结构类型和string。</param>
    /// <param name="action">进入锁之后的回调。</param>
    public static T Block<T>(object state, BlockFunc<T> action) {
        Symbol.CommonException.CheckArgumentNull(state, "state");
        Symbol.CommonException.CheckArgumentNull(action, "action");
        bool lockToken = false;
#if net20 || net35
        System.Threading.Monitor.Enter(state);
        lockToken=true;
#else
        System.Threading.Monitor.Enter(state, ref lockToken);
#endif
        try {
            T result = action();
            return result;
        } finally {
            if (lockToken) {
                System.Threading.Monitor.Exit(state);
            }
        }
    }

    #endregion
    #region Block BlockFunc<TArg, TResult> => TResult x(TArg);
    /// <summary>
    /// 区块排它锁，带返回值。
    /// </summary>
    /// <param name="state">用于排它锁的对象，不能是结构类型和string。</param>
    /// <param name="action">进入锁之后的回调。</param>
    /// <param name="arg">参数。</param>
    public static TResult Block<TArg, TResult>(object state, BlockFunc<TArg, TResult> action, TArg arg) {
        Symbol.CommonException.CheckArgumentNull(state, "state");
        Symbol.CommonException.CheckArgumentNull(action, "action");
        bool lockToken = false;
#if net20 || net35
        System.Threading.Monitor.Enter(state);
        lockToken=true;
#else
        System.Threading.Monitor.Enter(state, ref lockToken);
#endif
        try {
            TResult result = action(arg);
            return result;
        } finally {
            if (lockToken) {
                System.Threading.Monitor.Exit(state);
            }
        }
    }
    #endregion

    #region ParallelLock
    /// <summary>
    /// 创建并行锁（同name，全局唯一）。
    /// </summary>
    /// <param name="name">锁的名称，不区分大小写。</param>
    /// <returns>返回并行锁对象。</returns>
    public static ParallelLockContext ParallelLock(string name){
        return ParallelLock(name, null);
    }
    /// <summary>
    /// 创建并行锁（同name+value，全局唯一）。
    /// </summary>
    /// <param name="name">锁的名称，不区分大小写。</param>
    /// <param name="value">当前锁的值，同一个值，只有一个锁。</param>
    /// <returns>返回并行锁对象。</returns>
    public static ParallelLockContext ParallelLock<T>(string name, T value) where T:struct{
        return ParallelLock(name, value.ToString());
    }
    /// <summary>
    /// 创建并行锁（同name+value，全局唯一）。
    /// </summary>
    /// <param name="name">锁的名称，不区分大小写。</param>
    /// <param name="value">当前锁的值，同一个值，只有一个锁。</param>
    /// <returns>返回并行锁对象。</returns>
    public static ParallelLockContext ParallelLock(string name, string value) {
        Symbol.CommonException.CheckArgumentNull(name, "name");

        string key = name + "|" + value;

        return _list_parallelLock.GetOrAdd(key, (p) => {
            return new ParallelLockContext(name, value);
        });
    }
    #endregion

    #endregion

    #region types
    /// <summary>
    /// 区块排它锁（带返回值）委托。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <returns></returns>
    public delegate T BlockFunc<T>();
    /// <summary>
    /// 区块排它锁委托。
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="arg">参数。</param>
    public delegate void BlockAction<T>(T arg);
    /// <summary>
    /// 区块排它锁（带返回值）委托。
    /// </summary>
    /// <typeparam name="TArg">参数类型。</typeparam>
    /// <typeparam name="TResult">返回值类型。</typeparam>
    /// <param name="arg">参数。</param>
    /// <returns></returns>
    public delegate TResult BlockFunc<TArg, TResult>(TArg arg);

    /// <summary>
    /// 并行锁上下文。
    /// </summary>
    public class ParallelLockContext {
        #region fields
        private string _name;
        private string _value;
        private int _count;
        private System.DateTime? _beginTime;
        private System.DateTime? _endTime;
        private object _sync;
        #endregion

        #region properties

        /// <summary>
        /// 获取锁的名称。
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// 获取锁的键值。
        /// </summary>
        public string Value { get { return _value; } }
        /// <summary>
        /// 获取锁的调用次数。
        /// </summary>
        public int Count { get { return _count; } }
        /// <summary>
        /// 获取开始时间。
        /// </summary>
        public System.DateTime? BeginTime { get { return _beginTime; } }
        /// <summary>
        /// 获取结束时间。
        /// </summary>
        public System.DateTime? EndTime { get { return _endTime; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建 ParallelLockContext 实例。
        /// </summary>
        /// <param name="name">锁的名称。</param>
        /// <param name="value">锁的键值。</param>
        public ParallelLockContext(string name,string value) {
            _name = name;
            _value = value;
            _count = 0;
            _sync = new object();
        }
        #endregion

        #region methods

        #region plus

        void Begin() {
            System.Threading.Interlocked.Increment(ref _count);
            _endTime = System.DateTime.Now;
            if (_beginTime == null)
                _beginTime = _endTime;
        }
        void End() {
            _endTime = System.DateTime.Now;
        }

        #endregion

        #region Block System.Threading.ThreadStart => void x();
        /// <summary>
        /// 区块排它锁
        /// </summary>
        /// <param name="action">进入锁之后的回调。</param>
        public void Block(System.Threading.ThreadStart action) {
            Symbol.CommonException.CheckArgumentNull(action, "action");
            Begin();
            try {
                ThreadHelper.Block(_sync, action);
            } finally {
                End();
            }
        }
        #endregion
        #region Block BlockAction<T> => void x(T);
        /// <summary>
        /// 区块排它锁
        /// </summary>
        /// <param name="action">进入锁之后的回调。</param>
        /// <param name="arg">参数。</param>
        public void Block<T>(BlockAction<T> action, T arg) {
            Symbol.CommonException.CheckArgumentNull(action, "action");
            Begin();
            try {
                ThreadHelper.Block<T>(_sync, action, arg);
            } finally {
                End();
            }
        }
        #endregion
        #region Block BlockFunc<T> => T x();
        /// <summary>
        /// 区块排它锁，带返回值。
        /// </summary>
        /// <param name="action">进入锁之后的回调。</param>
        public T Block<T>(BlockFunc<T> action) {
            Symbol.CommonException.CheckArgumentNull(action, "action");
            Begin();
            try {
                return ThreadHelper.Block<T>(_sync, action);
            } finally {
                End();
            }
        }

        #endregion
        #region Block BlockFunc<TArg, TResult> => TResult x(TArg);
        /// <summary>
        /// 区块排它锁，带返回值。
        /// </summary>
        /// <param name="action">进入锁之后的回调。</param>
        /// <param name="arg">参数。</param>
        public TResult Block<TArg, TResult>(BlockFunc<TArg, TResult> action, TArg arg) {
            Symbol.CommonException.CheckArgumentNull(action, "action");
            Begin();
            try {
                return ThreadHelper.Block(_sync, action, arg);
            } finally {
                End();
            }
        }
        #endregion

        #endregion

    }
    #endregion

}