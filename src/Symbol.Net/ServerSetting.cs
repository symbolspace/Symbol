/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Net {
    /// <summary>
    /// 服务端设置项
    /// </summary>
    public class ServerSetting {

        #region fields
        private bool _readonly;
        private bool _allowNatTraversal;
        private System.Net.Sockets.LingerOption _lingerState;
        private bool _noDelay;
        private int _maxConnectionCount;
        private int _receiveBufferSize;
        private int _sendBufferSize;
        private int _timeout;
        private int _port;
        private System.Net.IPAddress _ip;
        private System.Security.Cryptography.X509Certificates.X509Certificate _certificate;

        #endregion

        #region properties
        /// <summary>
        /// 获取或设置是否为只读（为只读时忽略其它属性设置）。
        /// </summary>
        public bool Readonly {
            get { return _readonly; }
            set { _readonly = value; }
        }
        /// <summary>
        /// 获取或设置是否允许NAT。
        /// </summary>
        public bool AllowNatTraversal {
            get { return _allowNatTraversal; }
            set {
                if (_readonly)
                    return;
                _allowNatTraversal = value;
            }
        }
        //X509Certificate2
        /// <summary>
        /// 获取或设置NoDelay。
        /// </summary>
        public bool NoDelay {
            get { return _noDelay; }
            set {
                if (_readonly)
                    return;
                _noDelay = value;
            }
        }
        /// <summary>
        /// 获取或设置LingerState。
        /// </summary>
        public System.Net.Sockets.LingerOption LingerState {
            get { return _lingerState; }
            set {
                if (_readonly)
                    return;
                _lingerState = value;
            }
        }
        /// <summary>
        /// 获取或设置最大连接数。
        /// </summary>
        public int MaxConnection {
            get { return _maxConnectionCount; }
            set {
                if (_readonly)
                    return;
                _maxConnectionCount = value;
            }
        }
        /// <summary>
        /// 获取或设置接收缓冲区大小。
        /// </summary>
        public int ReceiveBufferSize {
            get { return _receiveBufferSize; }
            set {
                if (_readonly)
                    return;
                _receiveBufferSize = value;
            }
        }
        /// <summary>
        /// 获取或设置发送缓冲区大小。
        /// </summary>
        public int SendBufferSize {
            get { return _sendBufferSize; }
            set {
                if (_readonly)
                    return;
                _sendBufferSize = value;
            }
        }
        /// <summary>
        /// 获取或设置超时时间（秒）。
        /// </summary>
        public int Timeout {
            get { return _timeout; }
            set {
                if (_readonly)
                    return;
                _timeout = value;
            }
        }
        /// <summary>
        /// 获取或设置端口。
        /// </summary>
        public int Port {
            get { return _port; }
            set {
                if (_readonly)
                    return;
                _port = value;
            }
        }
        /// <summary>
        /// 获取或设置绑定IP（为空时自适应IPV4/IPV6）。
        /// </summary>
        public System.Net.IPAddress IP {
            get { return _ip; }
            set {
                if (_readonly)
                    return;
                _ip = value;
            }
        }
        /// <summary>
        /// 获取或设置证书对象。
        /// </summary>
        public System.Security.Cryptography.X509Certificates.X509Certificate Certificate {
            get { return _certificate; }
            set {
                if (_readonly)
                    return;
                _certificate = value;
            }
        }

        #endregion

        #region methods

        #region CreateDefault
        /// <summary>
        /// 创建默认设置对象（连接数1000，发送16K，接收32K，超时300秒）。
        /// </summary>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public static ServerSetting CreateDefault(int port) {
            ServerSetting setting = new ServerSetting();
            setting.AllowNatTraversal = true;
            setting.NoDelay = true;
            setting.LingerState = new System.Net.Sockets.LingerOption(false, 0);
            setting.MaxConnection = int.MaxValue;
            setting.SendBufferSize = 16384;
            setting.ReceiveBufferSize = 32768;//85000
            setting.Timeout = 300;
            setting.IP = null;
            setting.Port = port;
            //setting.Readonly = true;
            return setting;
        }
        #endregion

        #region LoadPfx
        /// <summary>
        /// 加载证书文件（*.pfx）。
        /// </summary>
        /// <param name="file">文件。</param>
        /// <param name="password">密钥。</param>
        public void LoadPfx(string file, string password) {
            byte[] data = null;
            if (!string.IsNullOrEmpty(file)){
                file = AppHelper.MapPath(file);
                if (System.IO.File.Exists(file))
                    data = System.IO.File.ReadAllBytes(file);
            }
            LoadPfx(data, password);
        }
        /// <summary>
        /// 加载证书文件（*.pfx）。
        /// </summary>
        /// <param name="data">文件数据。</param>
        /// <param name="password">密钥。</param>
        public void LoadPfx(byte[] data, string password) {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(password)) {
                Certificate = null;
                return;
            }
            Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(data, password);
        }
        #endregion

        #endregion

    }



}
