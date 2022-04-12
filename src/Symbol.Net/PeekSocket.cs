/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Symbol.Net {

    /// <summary>
    /// 连续的Socket，当数据接收到指定大小时就会触发Peeked事件。
    /// </summary>
    public class PeekSocket : IDisposable {
        #region fields
        private static readonly bool _supportSocketIOControlByCodeEnum;

        private Socket _socket;
        private bool _autoClose;
        private bool _started;
        private bool _speedEnabled;
        private int _bufferSize;
        private System.Timers.Timer _time_speed;
        //private System.Timers.Timer _time_receive;
        private bool _streaming = false;
        private Queue<byte> _rec_queue;
        //private System.Collections.Queue _rec_queue;
        private byte[] _rec_buffer;
        private int _rec_peekCount;
        private int _rec_secondLength;
        private SocketFlags _rec_flags;

       //private Queue<byte> _send_queue;
       //private byte[] _send_buffer;
        //private int _send_length;
        private int _send_secondLength;
        //private IAsyncResult _send_asyncResult;
        //private bool _send_busy = false;
        private SocketFlags _send_flags;
        private OnPeekedDelegate _onPeekedHandler = null;
        private NextPeekDelegate _nextPeekHandler = null;

        #endregion

        #region properties
        /// <summary>
        /// 获取当前系统是否支持Socket IO Control 
        /// </summary>
        public static bool SupportSocketIOControlByCodeEnum { get { return _supportSocketIOControlByCodeEnum; } }
        /// <summary>
        /// 获取本地终结点
        /// </summary>
        public System.Net.EndPoint LocalEndPoint {
            get { return _socket == null ? null : _socket.LocalEndPoint; }
        }
        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public System.Net.EndPoint RemoteEndPoint {
            get { return _socket == null ? null : _socket.RemoteEndPoint; }
        }

        /// <summary>
        /// 获取缓冲区大小（字节）。
        /// </summary>
        public int BufferSize {
            get { return _bufferSize; }
        }
        /// <summary>
        /// 获取当前是否已经开始。
        /// </summary>
        public bool Started {
            get {
                return _started;
            }
        }
        /// <summary>
        /// 获取订阅的下次数据缓冲长度。
        /// </summary>
        public int PeekCount {
            get { return _rec_peekCount; }
        }
        /// <summary>
        /// 获取或设置是否自动关闭Socket。
        /// </summary>
        public bool AutoClose {
            get { return _autoClose; }
            set { _autoClose = value; }
        }
        /// <summary>
        /// 获取或设置速度监控，不需要监控时请无开启，可能会浪费一些效率。
        /// </summary>
        public bool SpeedEnabled {
            get { return _speedEnabled; }
            set {
                if (_speedEnabled == value)
                    return;
                _speedEnabled = value;
                if (_time_speed !=null) {
                    if (value) {
                        _rec_secondLength = 0;
                        _send_secondLength = 0;
                    }
                    _time_speed.Enabled = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置发送时采用的SocketFlags。
        /// </summary>
        public SocketFlags SendFlags {
            get { return _send_flags; }
            set { _send_flags = value; }
        }
        /// <summary>
        /// 获取或设置接收时采用的SocketFlags。
        /// </summary>
        public SocketFlags ReceiveFlags {
            get { return _rec_flags; }
            set { _rec_flags = value; }
        }
        /// <summary>
        /// 获取发送队列中的字节数。
        /// </summary>
        public int SendQueueCount {
            //get { lock (_send_queue) { return _send_queue.Count; } }
            get { return 0;  }
        }
        /// <summary>
        /// 获取接收队列中已有的字节数。
        /// </summary>
        public int ReceiveQueueCount {
            get { 
                //lock (_rec_queue) { 
                    return _rec_queue.Count; 
                //} 
            }
            //get { return 0; }
        }
        /// <summary>
        /// 获取或设置当前为流模式。
        /// </summary>
        public bool Streaming {
            get { return _streaming; }
            set { _streaming = value; }
        }
        #endregion

        #region cctor
        static PeekSocket() {
            System.Net.Sockets.Socket socket = null;
            try {
                socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
#if net60
#pragma warning disable CA1416 // 验证平台兼容性
                socket.IOControl(System.Net.Sockets.IOControlCode.KeepAliveValues, null, null);
#pragma warning restore CA1416 // 验证平台兼容性
#endif
                _supportSocketIOControlByCodeEnum = true;
            } catch (System.NotSupportedException) {
                _supportSocketIOControlByCodeEnum = false;
            } catch (System.NotImplementedException) {
                _supportSocketIOControlByCodeEnum = false;
            } catch (System.Exception) {
                _supportSocketIOControlByCodeEnum = true;
            } finally {
#if !net35
                if (socket != null) {
                    socket.Dispose();
                }
#endif
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建PeekSocket实例
        /// </summary>
        /// <param name="socket">已经连接的Socket实例。</param>
        /// <param name="bufferSize">缓冲区大小，默认是8192字节。</param>
        public PeekSocket(Socket socket, int bufferSize=8192) {
            _socket = socket;
            if (bufferSize < 1)
                bufferSize = 8192;
            _socket.ReceiveBufferSize = _socket.SendBufferSize = bufferSize;

            _rec_queue = new Queue<byte>(bufferSize);
           //_rec_queue = System.Collections.Queue.Synchronized(new System.Collections.Queue(bufferSize));
            _rec_buffer = new byte[bufferSize];
            _rec_flags = SocketFlags.None;

            //_send_queue = new Queue<byte>(bufferSize);
            //_send_buffer = new byte[bufferSize];
            _send_flags = SocketFlags.None;

            _bufferSize = bufferSize;

            _time_speed = new System.Timers.Timer(1000D);
            _time_speed.Elapsed += new System.Timers.ElapsedEventHandler(_time_speed_Elapsed);

            //_time_receive = new System.Timers.Timer(200);
            //_time_receive.Elapsed += new System.Timers.ElapsedEventHandler(_time_receive_Elapsed);
            _onPeekedHandler = OnPeeked;
            _nextPeekHandler = NextPeek_Body;
        }

        //void _time_receive_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        //    CheckPeek();
        //}

        void _time_speed_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            //lock (_rec_queue) {
            int length1 = _rec_secondLength;// *2;
                _rec_secondLength = 0;
                OnReceiveSpeed(length1);
            //}
            //lock (_send_queue) {
                int length2 = _send_secondLength;// *2;
                _send_secondLength = 0;
                OnSendSpeed(length2);
            //}
        }
        #endregion

        #region methods

        #region Start
        /// <summary>
        /// 开始接收/发送操作。
        /// </summary>
        public void Start() {
            if (_started)
                return;
            if (_socket == null)
                return;
            if (_rec_queue == null) {
                return;
            }
            //lock (_rec_queue) {
                if (_started)
                    return;
                _started = true;
                _rec_secondLength = 0;
                _send_secondLength = 0;

                Receive();
                //Send();

                //_time_receive.Start();
            //}
        }
        #endregion

        #region Stop
        /// <summary>
        /// 停止接收/发送操作。
        /// </summary>
        public void Stop() {
            if (!_started)
                return;
            //lock (_rec_queue) {
                if (!_started)
                    return;
                _started = false;
                //_time_receive.Stop();
            //}
        }
        #endregion

        void Receive_Peek() {
            if (_rec_peekCount == 0)
                return;
            try {
                _socket.BeginReceive(_rec_buffer, 0, _bufferSize, _rec_flags | SocketFlags.Peek, Receive_Callback_Peek, null);
            } catch (System.ObjectDisposedException e) {
                OnError(new PeekSocketErrorEventArgs(false, true, e));
            } catch (Exception e) {
                bool cancel = e is ThreadAbortException;
                OnError(new PeekSocketErrorEventArgs(false, cancel, e));
            }
        }
        void Receive_Callback_Peek(IAsyncResult asyncResult) {
            if (_socket == null)
                return;
            int length = 0;
            try {
                length = _socket.EndReceive(asyncResult);
                if (length < _rec_peekCount) {
                    if (!_started)
                        return;
                    Receive_Peek();
                }
                byte[] buffer = new byte[_rec_peekCount];
                int length2 = _socket.Receive(buffer, _rec_peekCount, _rec_flags);
                OnReceiveSpeedLength(length2);
                //Console.WriteLine("peek " + _rec_peekCount);
                //OnPeeked(new PeekSocketPeekedEventArgs(buffer));
                _onPeekedHandler.BeginInvoke(new PeekSocketPeekedEventArgs(buffer), null, null);
            } catch (System.ObjectDisposedException e) {
                OnError(new PeekSocketErrorEventArgs(false, true, e));
            } catch (Exception e) {
                bool cancel = e is ThreadAbortException;
                OnError(new PeekSocketErrorEventArgs(false, cancel, e));
            }
        }
        void Receive() {
            try {
                byte[] buffer = new byte[_bufferSize];
                _socket.BeginReceive(buffer, 0, _bufferSize, _rec_flags, Receive_Callback, buffer);
            } catch (System.ObjectDisposedException e) {
                OnError(new PeekSocketErrorEventArgs(false, true, e));
            } catch (Exception e) {
                bool cancel = e is ThreadAbortException;
                OnError(new PeekSocketErrorEventArgs(false, cancel, e));
            }
        }
        void Receive_Callback(IAsyncResult asyncResult) {
            //Console.WriteLine("{0} peek rec callback from {1} peek:{2}",_socket.LocalEndPoint, _socket.RemoteEndPoint,_rec_peekCount);
            if (_rec_queue == null || _socket==null) {
                //Console.WriteLine("{0} peek rec callback from {1} _rec_queue==null", _socket.LocalEndPoint, _socket.RemoteEndPoint);
                return;
            }
            //lock (_rec_queue) {
            lock (_rec_queue) {
                int length = 0;
                byte[] buffer = asyncResult.AsyncState as byte[];
                try {
                    if (_socket == null)
                        return;
                    length = _socket.EndReceive(asyncResult);
                    
                //} catch (System.ObjectDisposedException) {
                //    Dispose();
                //    return;
                } catch (OutOfMemoryException) {
                    Dispose();
                    return;
                } catch (Exception e) {
                    bool cancel = e is ThreadAbortException;
                    OnError(new PeekSocketErrorEventArgs(false, cancel, e));
                    if (_socket == null || !_socket.Connected) {
                        //Console.WriteLine("{0} peek rec callback from {1} socket==null or !connected", _socket.LocalEndPoint, _socket.RemoteEndPoint);
                        return;
                    }
                    if (_started) {
                        Receive();
                        //Console.WriteLine("{0} peek rec callback from {1} catch(exception) started rec()", _socket.LocalEndPoint, _socket.RemoteEndPoint);
                        return;
                    }
                }
                if (length > 0) {
                    //Console.WriteLine("{0} peek rec callback from {1} length={2}", _socket.LocalEndPoint, _socket.RemoteEndPoint,length);
                    //QueueExtensions.AddRange(_rec_queue,_rec_buffer, 0, length);//追加到队列中
                    if (_streaming) {
                        byte[] buffer2 = new byte[length];
                        Array.Copy(buffer, 0, buffer2, 0, length);
                        OnReceiveSpeedLength(length);
                        OnDataArrived(new PeekSocketPeekedEventArgs(buffer2, true));
                    } else {
                        for (int i = 0; i < length; i++) {
                            _rec_queue.Enqueue(buffer[i]);
                        }
                        OnReceiveSpeedLength(length);//触发速度
                        CheckPeek();//检查订阅的数据长度
                    }

                    //已经停止了
                    if (!_started) {
                        //Console.WriteLine("{0} peek rec callback from {1} !started return", _socket.LocalEndPoint, _socket.RemoteEndPoint);
                        return;
                    }
                    Receive();
                } else {
                    if (_autoClose) {
                        _started = false;
                        _socket.Close();
                        //_socket.Dispose();
                        _socket = null;
                    }
                }
            }

        }
        private void OnReceiveSpeedLength(int length) {
            if(_speedEnabled)
                _rec_secondLength += length;
            //Console.WriteLine("rec={0},{1}", FileHelper.LengthToString(length),_queue.Count);
        }
        private void OnSendSpeedLength(int length) {
            if (_speedEnabled)
                _send_secondLength += length;
        }
        private void CheckPeek() {
            lock (_rec_queue) {
                if (_rec_peekCount == 0 || _rec_queue.Count < _rec_peekCount)
                    return;
                byte[] buffer = new byte[_rec_peekCount];
                for (int i = 0; i < _rec_peekCount; i++) {
                    buffer[i] = _rec_queue.Dequeue();
                    //buffer[i] = (byte)_rec_queue.Dequeue();
                }
                _rec_peekCount = 0;
                new Thread(() => {
                    OnPeeked(new PeekSocketPeekedEventArgs(buffer));
                }).Start();
                //buffer = null;
            }
            
        }

        #region NextPeek
        /// <summary>
        /// 订阅下次数据接收缓冲长度。
        /// </summary>
        /// <param name="count">需要缓冲的长度，必须大于0。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">count不能小于1。</exception>
        public void NextPeek(int count) {
            if (count < 1)
                CommonException.ThrowArgumentOutOfRange("count", "数量不能小于1。");
            _nextPeekHandler.BeginInvoke(count, null, null);
        }
        delegate void NextPeekDelegate(int count);
        void NextPeek_Body(int count) {
            lock (_rec_queue) {
                _rec_peekCount = count;
                CheckPeek();
                //Receive_Peek();
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// 发送数据，写入缓冲区，不一定马上就发送完成。
        /// </summary>
        /// <param name="buffer">需要发送的数据。</param>
        /// <param name="index">起始位置，从0开始。</param>
        /// <param name="count">追加数量，-1表示从index开始剩下的，反之是从index开始往后多少个成员。</param>
        public void Send(byte[] buffer, int index = 0, int count = -1) {
            //lock (_send_queue) {
                //QueueExtensions.AddRange(_send_queue,buffer, index, count);
                //Send();
                if (_socket == null)
                    return;
                try {
                    //_socket.Send(buffer, index, count, _send_flags);
                    _socket.BeginSend(buffer, index, count == -1 ? buffer.Length : count, _send_flags, Send_Callback_empty, null);
                } catch (System.ObjectDisposedException e) {
                    OnError(new PeekSocketErrorEventArgs(true, true, e));
                } catch (Exception error) {
                    bool cancel = error is ThreadAbortException;
                    OnError(new PeekSocketErrorEventArgs(true, cancel, error));
                } 
            //}
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="value">字节</param>
        public void Send(byte value) {
            //lock (_send_queue) {
                //_send_queue.Enqueue(value);
                //Send();
                if (_socket == null)
                    return;
                try {
                    byte[] buffer= new byte[]{ value};
                    //_send_busy = true;
                    _socket.BeginSend(buffer, 0, 1, _send_flags, Send_Callback_empty, null);
                    //_socket.Send(buffer, 0, 1, _send_flags);
                } catch (System.ObjectDisposedException e) {
                    OnError(new PeekSocketErrorEventArgs(true, true, e));
                } catch (Exception error) {
                    bool cancel = error is ThreadAbortException;
                    OnError(new PeekSocketErrorEventArgs(true, cancel, error));
                }
            //}
        }
        void Send_Callback_empty(IAsyncResult asyncResult) {
            if (_socket == null)
                return;
            try {
                int length = _socket.EndSend(asyncResult);
                OnSendSpeedLength(length);
                //_send_busy = false;
            } catch (System.ObjectDisposedException e) {
                OnError(new PeekSocketErrorEventArgs(true, true, e));
            } catch (Exception error) {
                bool cancel = error is ThreadAbortException;
                OnError(new PeekSocketErrorEventArgs(true, cancel, error));
            }
        }

        #endregion

        #endregion

        #region events

        #region Peeked
        /// <summary>
        /// 缓冲就绪，订阅的数据到了时触发。
        /// </summary>
        public event EventHandler<PeekSocketPeekedEventArgs> Peeked;
        delegate void OnPeekedDelegate(PeekSocketPeekedEventArgs e);
        /// <summary>
        /// 触发缓冲就绪事件。
        /// </summary>
        /// <param name="e">事件信息实例。</param>
        protected virtual void OnPeeked(PeekSocketPeekedEventArgs e) {
            using (e) {
                EventHandler<PeekSocketPeekedEventArgs> handler = Peeked;
                if (handler != null) {
                    //new Thread(() => { 
                    //    //Thread.Sleep(1); 
                    //    handler(this, e); 
                    //}) { IsBackground = true }.Start();
                    
                    handler(this, e);
                }
            }
        }
        #endregion

        #region DataArrived
        /// <summary>
        /// 数据到达时触发，仅当为Streaming时才会触发。
        /// </summary>
        public event EventHandler<PeekSocketPeekedEventArgs> DataArrived;
        /// <summary>
        /// 触发数据到达时触发。
        /// </summary>
        /// <param name="e">事件信息实例。</param>
        protected virtual void OnDataArrived(PeekSocketPeekedEventArgs e) {
            using (e) {
                EventHandler<PeekSocketPeekedEventArgs> handler = DataArrived;
                if (handler != null) {
                    //new Thread(() => { Thread.Sleep(1); handler(this, e); }) { IsBackground = true }.Start();
                    handler(this, e);
                }
            }
        }
        #endregion
        #region Error
        /// <summary>
        /// 错误事件，当发生错误时触发。
        /// </summary>
        public event EventHandler<PeekSocketErrorEventArgs> Error;
        /// <summary>
        /// 触发错误事件。
        /// </summary>
        /// <param name="e">事件信息实例。</param>
        protected virtual void OnError(PeekSocketErrorEventArgs e) {
            EventHandler<PeekSocketErrorEventArgs> handler = Error;
            if (handler != null) {
                handler(this, e);
            }
        }
        #endregion

        #region ReceiveSpeed
        /// <summary>
        /// 接收速度事件，每500毫秒触发一次。需要SpeedEnabled属性为true。
        /// </summary>
        public event PeekSocketSpeedHandler ReceiveSpeed;
        /// <summary>
        /// 触发接收速度事件。
        /// </summary>
        /// <param name="speed">1秒内发送的数据量。</param>
        protected virtual void OnReceiveSpeed(int speed) {
            PeekSocketSpeedHandler handler = ReceiveSpeed;
            if (handler != null) {
                //new Thread(() => {
                    handler(this, speed);
                //}).Start();
            }
        }
        #endregion

        #region SendSpeed
        /// <summary>
        /// 发送速度事件，每500毫秒触发一次。需要SpeedEnabled属性为true。
        /// </summary>
        public event PeekSocketSpeedHandler SendSpeed;
        /// <summary>
        /// 触发接收速度事件。
        /// </summary>
        /// <param name="speed">1秒内发送的数据量。</param>
        protected virtual void OnSendSpeed(int speed) {
            PeekSocketSpeedHandler handler = SendSpeed;
            if (handler != null) {
                //new Thread(() => {
                    handler(this, speed);
                //}).Start();
            }
        }
        #endregion

        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 析构 PeekSocket
        /// </summary>
        ~PeekSocket() {
            Dispose(false);
        }
        /// <summary>
        /// 释放PeekSocket所占用的资源。清空除列，关闭Socket。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放PeekSocket所占用的资源。
        /// </summary>
        /// <param name="dispsing">是否为强制释放。</param>
        protected virtual void Dispose(bool dispsing) {
            if (dispsing) {
                if (_rec_queue != null) {
                    Stop();
                    _rec_queue.Clear();
                    _rec_queue = null;
                }
                if (_socket != null) {
                    try { _socket.Shutdown(SocketShutdown.Both); } catch (System.Exception) { }
                    _socket.Close();
#if !net20 && !net35
                    _socket.Dispose();
#endif
                    _socket = null;
                    _socket = null;
                }
                if (_rec_buffer != null) {
                    _rec_buffer = null;
                }
                if (_time_speed != null) {
                    _time_speed.Enabled = false;
                    _time_speed.Dispose();
                    _time_speed = null;
                }

                GC.SuppressFinalize(this);
                //GC.Collect(0);
                //GC.Collect();
            }
        }
        #endregion
    }
   
  
    /// <summary>
    /// PeekSocket速度事件委托
    /// </summary>
    /// <param name="sender">PeekSocket实例。</param>
    /// <param name="speed">1秒内发送的数据量。</param>
    public delegate void PeekSocketSpeedHandler(PeekSocket sender,int speed);
}