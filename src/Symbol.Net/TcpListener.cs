/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Net {

    /// <summary>
    /// 基于TCP的监听器。
    /// </summary>
    public sealed class TcpListener :System.IDisposable{

        #region fields
        private ILog _log;
        private ServerSetting _serverSetting;
        private System.DateTime? _beginTime;
        private System.DateTime? _endTime;

        private System.Net.Sockets.Socket _listener_v4;
        private System.Net.Sockets.Socket _listener_v6;
        private int _running_v4;
        private int _running_v6;
        private int _disposed = 0;

        #endregion

        #region properties

        #region OSSupportsIPv4
        /// <summary>
        /// 指示基础操作系统和网络适配器是否支持 Internet 协议第 4 版 (IPv4)。
        /// </summary>
        /// <remarks>如果操作系统和网络适配器支持 IPv4 协议，则为 true；否则为 false。</remarks>
        public static bool OSSupportsIPv4 {
            get {
#if net20 || net35
#pragma warning disable CS0618
                return System.Net.Sockets.Socket.SupportsIPv4;
#pragma warning restore CS0618
#else
                return System.Net.Sockets.Socket.OSSupportsIPv4;
#endif
            }
        }
        #endregion
        #region OSSupportsIPv6
        /// <summary>
        /// 指示基础操作系统和网络适配器是否支持 Internet 协议第 6 版 (IPv6)。
        /// </summary>
        /// <remarks>如果操作系统和网络适配器支持 IPv6 协议，则为 true；否则为 false。</remarks>
        public static bool OSSupportsIPv6 {
            get {
#if net20
#pragma warning disable CS0618
                return System.Net.Sockets.Socket.SupportsIPv6;
#pragma warning restore CS0618
#else
                return System.Net.Sockets.Socket.OSSupportsIPv6;
#endif
            }
        }
        #endregion

        /// <summary>
        /// 是否已释放。
        /// </summary>
        public bool IsDisposed {
            get {
                return System.Threading.Interlocked.CompareExchange(ref _disposed, 2, 2) == 2;
            }
        }

        /// <summary>
        /// 获取是否正在运行中。
        /// </summary>
        public bool Running {
            get { return RunningV4 || RunningV6; }
        }
        /// <summary>
        /// 获取v4是否正在运行中。
        /// </summary>
        public bool RunningV4 {
            get { return System.Threading.Interlocked.CompareExchange(ref _running_v4, 1, 1) == 1; }
        }
        /// <summary>
        /// 获取v6是否正在运行中。
        /// </summary>
        public bool RunningV6 {
            get { return System.Threading.Interlocked.CompareExchange(ref _running_v6, 1, 1) == 1; }
        }
        /// <summary>
        /// 获取或设置日志对象。
        /// </summary>
        public ILog Log {
            get { return System.Threading.Interlocked.CompareExchange(ref _log, null, null) ?? LogBase.Empty; }
            set {
                System.Threading.Interlocked.Exchange(ref _log, value??LogBase.Empty);
            }
        }
        /// <summary>
        /// 获取服务器配置对象。
        /// </summary>
        public ServerSetting ServerSetting {
            get { return System.Threading.Interlocked.CompareExchange(ref _serverSetting, null, null); }
        }

        /// <summary>
        /// 获取开始时间（未开始时为null）。
        /// </summary>
        public System.DateTime? BeginTime {
            get {
                return _beginTime;
            }
        }
        /// <summary>
        /// 获取结束时间（未开始时为null，未结束时为Now）。
        /// </summary>
        public System.DateTime? EndTime {
            get {
                if (_beginTime == null)
                    return null;
                return _endTime ?? System.DateTime.Now;
            }
        }
        /// <summary>
        /// 获取运行时长。
        /// </summary>
        public TimeSpan RunTime {
            get {
                System.DateTime? beginTime = BeginTime;
                System.DateTime? endTime = EndTime;
                if (beginTime == null)
                    return TimeSpan.Zero;
                return (endTime.Value - beginTime.Value);
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建TcpListener实例。
        /// </summary>
        /// <param name="port">监听端口，自适应ipv4/ipv6。</param>
        public TcpListener(int port)
            : this(ServerSetting.CreateDefault(port)) {

        }
        /// <summary>
        /// 创建TcpListener实例。
        /// </summary>
        /// <param name="setting">设置项。</param>
        public TcpListener(ServerSetting setting) {
            Throw.CheckArgumentNull(setting, "setting");
            _serverSetting = setting;
            setting.Readonly = true;

            _log = LogBase.Empty;
        }
        #endregion

        #region methods

        #region Start
        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start() {
            if (Running) {
                Log.Error(">正在监听中，重复的开始请求");
                return;
            }
            Log.Info(">开始监听");
#if NETDNX
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
#else
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
#endif

            Starting?.Invoke(this, EventArgs.Empty);
            if (Start_IPv4()) {
                System.Threading.Interlocked.CompareExchange(ref _running_v4, 1, 0);
            }
            if (Start_IPv6()) {
                System.Threading.Interlocked.CompareExchange(ref _running_v6, 1, 0);
            }
            Log.Info(">监听完成");
            Started?.Invoke(this, EventArgs.Empty);
            if (Running) {
                StartAccept();
                _beginTime = System.DateTime.Now;
            }
        }
#if NETDNX
        void CurrentDomain_ProcessExit(object sender, EventArgs e) {
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
#else
        void CurrentDomain_DomainUnload(object sender, EventArgs e) {
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
#endif
            Stop();
        }


        #region Start_IPv4
        bool Start_IPv4() {
            if (!OSSupportsIPv4) {
                Log.Error(":( 操作系统不支持IPv4");
                return false;
            }
            if (_serverSetting.IP != null && _serverSetting.IP.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) {
                Log.Debug("?_? 不需要监听IPv4");
                return false;
            }
            System.Net.IPAddress ip = _serverSetting.IP;
            if (ip == null)
                ip = System.Net.IPAddress.Any;
            try {
                _listener_v4 = new System.Net.Sockets.Socket(ip.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                _listener_v4.Bind(new System.Net.IPEndPoint(ip, _serverSetting.Port));
                SetNatTraversal(_listener_v4, _serverSetting.AllowNatTraversal);
                _listener_v4.Listen(_serverSetting.MaxConnection);
                Log.Debug("^v^ 监听 {0}:{1} 成功 ", ip, _serverSetting.Port);
                return true;
            } catch (Exception error) {
                Log.Error("*_* 监听 {0}:{1} 错误：{2}", ip, _serverSetting.Port, error.Message);
                return false;
            }
        }
        #endregion
        #region Start_IPv6
        bool Start_IPv6() {
            if (!OSSupportsIPv6) {
                Log.Error(":( 操作系统不支持IPv6");
                return false;
            }
            if (_serverSetting.IP != null && _serverSetting.IP.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6) {
                Log.Debug("?_? 不需要监听IPv6");
                return false;
            }
            System.Net.IPAddress ip = _serverSetting.IP;
            if (ip == null)
                ip = System.Net.IPAddress.IPv6Any;
            try {
                _listener_v6 = new System.Net.Sockets.Socket(ip.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                _listener_v6.Bind(new System.Net.IPEndPoint(ip, _serverSetting.Port));
                SetNatTraversal(_listener_v6, _serverSetting.AllowNatTraversal);
                _listener_v6.Listen(_serverSetting.MaxConnection);
                Log.Debug("^v^ 监听 {0}:{1} 成功 ", ip, _serverSetting.Port);
                return true;
            } catch (Exception error) {
                Log.Error("*_* 监听 {0}:{1} 错误：{2}", ip, _serverSetting.Port, error.Message);
                return false;
            }
        }
        #endregion
        #endregion
        #region Stop
        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop() {
            if(!Running){
                Log.Error(">未开始监听，无需停止");
                return;
            }
#if NETDNX
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
#else
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
#endif

            Log.Info(">准备停止监听");
            Stopping?.Invoke(this, EventArgs.Empty);
            if (RunningV4) {
                if (Stop_IPv4()) {
                    System.Threading.Interlocked.CompareExchange(ref _running_v4, 0, 1);
                }
            }
            if (RunningV6) {
                if (Stop_IPv6()) {
                    System.Threading.Interlocked.CompareExchange(ref _running_v6, 0, 1);
                }
            }
            Log.Info(">停止监听完成");
            Stoped?.Invoke(this, EventArgs.Empty);
            _endTime = System.DateTime.Now;
        }
        #region Stop_IPv4
        bool Stop_IPv4() {
            var listener = System.Threading.Interlocked.CompareExchange(ref _listener_v4, null, _listener_v4);
            if (listener == null) {
                Log.Info(">IPv4监听已经停止");
                return false;
            }
            Log.Info(">停止 IPv4端口");
            try { listener.Close(); } catch { }
#if !net20 && !net35
            try { listener.Dispose(); } catch { }
#endif
            return true;
        }
        #endregion
        #region Stop_IPv6
        bool Stop_IPv6() {
            var listener = System.Threading.Interlocked.CompareExchange(ref _listener_v6, null, _listener_v6);
            if (listener == null) {
                Log.Info(">IPv6监听已经停止");
                return false;
            }
            Log.Info(">停止 IPv6端口");
            try { listener.Close(); } catch { }
#if !net20 && !net35
            try { listener.Dispose(); } catch { }
#endif
            return true;
        }
        #endregion
        #endregion

        #region StartAccept
        void StartAccept() {
            if (RunningV4) {
                ThreadHelper.Delay(0, () => StartAccept_Body(true));
            }
            if (RunningV6) {
                ThreadHelper.Delay(0, () => StartAccept_Body(false));
            }
        }
        void StartAccept_Body(bool v4) {
            if (v4) {
                while (RunningV4) {
                    if (!StartAccept_Body_Accept(v4))
                        break;
                }
            } else {
                while (RunningV6) {
                    if (!StartAccept_Body_Accept(v4))
                        break;

                }
            }
        }
        bool StartAccept_Body_Accept(bool v4) {
            System.Net.Sockets.Socket listener;
            if (v4) {
                listener = System.Threading.Interlocked.CompareExchange(ref _listener_v4, null, null);
            } else {
                listener = System.Threading.Interlocked.CompareExchange(ref _listener_v6, null, null);
            }
            if (listener == null)
                return false;
            try {
                var socket = listener.Accept();
                if (socket == null || socket == listener || !socket.Connected || socket.RemoteEndPoint == null)
                    return true;
                System.Threading.ThreadPool.QueueUserWorkItem(OnAccept, socket);
                return true;
            } catch {
                return true;
            }
        }
        void StartAccept_Body(object state) {
            if (state is System.Net.Sockets.Socket listener) {
                StartAccept_Body(listener);
            }
        }

        #endregion

        #region SetNatTraversal
        /// <summary>
        /// 设置是否允许NAT
        /// </summary>
        /// <param name="socket">socket对象，必须在Bind之后。</param>
        /// <param name="value">为true表示启用</param>
        public static void SetNatTraversal(System.Net.Sockets.Socket socket, bool value) {
            if (value) {
                SetIPProtectionLevel(socket, System.Net.Sockets.IPProtectionLevel.Unrestricted);
            } else {
                SetIPProtectionLevel(socket, System.Net.Sockets.IPProtectionLevel.EdgeRestricted);
            }
        }
        #endregion
        #region SetIPProtectionLevel
        /// <summary>
        /// 设置IP协议参数。
        /// </summary>
        /// <param name="socket">socket实例，必须在bind之后。</param>
        /// <param name="level">参数。</param>
        public static void SetIPProtectionLevel(System.Net.Sockets.Socket socket, System.Net.Sockets.IPProtectionLevel level) {
            if (level == System.Net.Sockets.IPProtectionLevel.Unspecified) {
                Throw.Argument("无效的level参数：" + level);
            }
            if (socket.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) {
#if net20 || net35
                socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, (System.Net.Sockets.SocketOptionName)23, (int)level);
#else
                socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6, System.Net.Sockets.SocketOptionName.IPProtectionLevel, (int)level);
#endif
            } else {
                if (socket.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) {
                    Throw.NotSupported("暂不支持此网络地址类型：" + socket.AddressFamily);
                }
#if net20 || net35
                socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, (System.Net.Sockets.SocketOptionName)23, (int)level);
#else
                socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.IPProtectionLevel, (int)level);
#endif
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            if (System.Threading.Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
                return;
            System.Threading.ThreadPool.QueueUserWorkItem(Dispose_Body);
        }
        void Dispose_Body(object state) {
            if (Running) {
                Stop();
                System.Threading.Thread.Sleep(100);
            }
            System.Threading.Interlocked.Exchange(ref _log, null);
            _serverSetting = null;
            _beginTime = null;
            _endTime = null;
            System.Threading.Interlocked.CompareExchange(ref _disposed, 2, 1);
        }
        #endregion

        #endregion

        #region events

        #region Starting
        /// <summary>
        /// 事件：准备启动。
        /// </summary>
        public event EventHandler Starting;
        #endregion
        #region Started
        /// <summary>
        /// 事件：启动完成（是否成功启动，需要判断Running。）
        /// </summary>
        public event EventHandler Started;
        #endregion
        #region Stopping
        /// <summary>
        /// 事件：准备停止。
        /// </summary>
        public event EventHandler Stopping;
        #endregion
        #region Stoped
        /// <summary>
        /// 事件：停止完成。
        /// </summary>
        public event EventHandler Stoped;
        #endregion

        #region Accepted
        /// <summary>
        /// 事件：客户端连接。
        /// </summary>
        public event TcpListenerAcceptHandler Accept;
        void OnAccept(object state) {
            if (state is System.Net.Sockets.Socket socket) {
                OnAccept(socket);
            }
        }
        void OnAccept(System.Net.Sockets.Socket socket) {
            Accept?.Invoke(this, socket);
        }

        #endregion

        #endregion


    }
    #region types
    /// <summary>
    /// TcpListener Accept 事件委托。
    /// </summary>
    /// <param name="sender">事件触发者。</param>
    /// <param name="socket">接入的socket客户端。</param>
    public delegate void TcpListenerAcceptHandler(object sender, System.Net.Sockets.Socket socket);
    #endregion

}