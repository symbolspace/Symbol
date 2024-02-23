/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 描述传入 HttpListener 对象的 HTTP 请求。
    /// </summary>
    public class HttpListenerRequest : IHttpListenerRequest {

        #region fields
        private System.Collections.Specialized.NameValueCollection _headers;
        private System.Collections.Specialized.NameValueCollection _form = null;
        private System.Collections.Specialized.NameValueCollection _cookies = null;
        private System.Collections.Specialized.NameValueCollection _serverVariables = null;
        private System.Collections.Specialized.NameValueCollection _params = null;
        private System.Collections.Specialized.NameValueCollection _queryString = null;
        private IHttpFileCollection _files = null;
        private System.IO.Stream _inputStream;

        private string[] _acceptTypes = null;
        private string _userAgent = null;
        private string[] _userLanguages = null;
        private System.Text.Encoding _contentEncoding = null;
        private long _contentLength = -1;
        private string _contentType = null;
        private string _rawUrl;
        private System.Uri _url = null;
        private System.Uri _urlReferrer = null;
        private bool _hasEntityBody;
        private bool _isSecureConnection;
        private bool _isLocal;
        private string _httpMethod;
        private System.Version _protocolVersion;
        private bool? _keepAlive;
        private string _userHostAddress;
        private string _userHostName;
        private System.Net.IPEndPoint _localEndPoint;
        private System.Net.IPEndPoint _remoteEndPoint;
        private bool _disposed;
        private BinaryReadHandlerDelgate _binaryReadHandler;
        #endregion

        #region properties

        #region AcceptTypes
        /// <summary>
        /// 获取客户端支持的 MIME 接受类型的字符串数组。["*/*","image/png"]
        /// </summary>
        /// <returns>客户端支持的 MIME 接受类型的字符串数组。</returns>
        public string[] AcceptTypes {
            get {
                if (_acceptTypes == null) {
                    string value = _headers["Accept"];
                    if (!string.IsNullOrEmpty(value)) {
                        _acceptTypes = value.Split(',');
                        for (int i = 0; i < _acceptTypes.Length; i++) {
                            _acceptTypes[i] = _acceptTypes[i].Trim();
                        }
                    }
                    if (_acceptTypes == null || _acceptTypes.Length == 0)
                        _acceptTypes = new string[] { "*/*" };
                }
                return _acceptTypes;
            }
        }
        #endregion
        #region ContentEncoding
        /// <summary>
        /// 获取当前请求所采用的字符集，如果客户端未指定，默认是UTF8。
        /// </summary>
        /// <returns>表示客户端的字符集的 System.Text.Encoding 对象。</returns>
        public System.Text.Encoding ContentEncoding {
            get {
                if (_contentEncoding == null) {
                    string contentType = _headers["Content-Type"];
                    if (contentType == null) {
                        _contentEncoding = System.Text.Encoding.UTF8;
                    } else {
                        string[] strArray = contentType.ToLower().Split(new char[] { ';', '=', ' ' });
                        bool flag = false;
                        System.Text.Encoding result = null;
                        foreach (string str2 in strArray) {
                            if (str2 == "charset") {
                                flag = true;
                            } else if (flag) {
                                try {
                                    result = System.Text.Encoding.GetEncoding(str2);
                                } catch {
                                }
                                break;
                            }
                        }
                        if (result == null)
                            result = System.Text.Encoding.UTF8;
                        _contentEncoding = result;
                    }
                }
                return _contentEncoding;
            }
            set {
                _contentEncoding = value;
            }
        }
        #endregion
        #region ContentLength
        /// <summary>
        /// 获取客户端发送的内容长度（以字节计）。
        /// </summary>
        /// <returns>客户端发送的内容的长度（以字节为单位）。</returns>
        public long ContentLength {
            get {
                if (_contentLength == -1) {
                    string value = _headers["Content-Length"];
                    long result;
                    if (!string.IsNullOrEmpty(value) && long.TryParse(value, out result)) {
                        _contentLength = result;
                    }
                    if (_contentLength < 0L) {
                        _contentLength = 0L;
                    }
                }
                return _contentLength;
            }
        }
        #endregion
        #region ContentType
        /// <summary>
        /// 获取传入请求的 MIME 内容类型，默认GET没有此值，POST时，普通的表单为：application/x-www-form-urlencoded，带文件上传的为：multipart/form-data; boundary=--------xxxxxx，一些特殊的网络请求，可能会传成 text/xml。
        /// </summary>
        /// <returns>表示传入请求的 MIME 内容类型的字符串，例如，“text/html”。其他常见 MIME 类型包括“audio.wav”、“image/gif”和“application/pdf”。</returns>
        public string ContentType {
            get {
                if (_contentType == null) {
                    _contentType = _headers["Content-Type"];
                    if (_contentType == null)
                        _contentType = "";
                    //else
                        //_contentType = _contentType.Split(';')[0];
                }

                return _contentType;
            }
        }
        #endregion
        #region HasEntityBody
        /// <summary>
        /// 获取一个 System.Boolean 值，该值指示请求是否有关联的正文数据。
        /// </summary>
        /// <returns>如果请求有关联的正文数据，则为 true；否则为 false。</returns>
        public bool HasEntityBody {
            get {
                return _hasEntityBody;
            }
            set {
                _hasEntityBody = value;
            }
        }
        #endregion
        #region Cookies
        /// <summary>
        /// 获取客户端发送的 Cookie 的集合（改进了传统的ASP.NET用法，实际上在Request时，也只能得到名称与值，其它的属性是没有的）。
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Cookies {
            get {
                if (_cookies == null) {
                    _cookies = new System.Collections.Specialized.NameValueCollection();
                }
                return _cookies;
            }
        }
        #endregion
        #region Headers
        /// <summary>
        /// 获取 HTTP 头集合。
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Headers {
            get {
                return _headers;
            }
        }
        #endregion
        #region HttpMethod
        /// <summary>
        /// 获取或设置客户端使用的 HTTP 数据传输方法（如 GET、POST 或 HEAD），始终为大写字母。
        /// </summary>
        /// <returns>客户端使用的 HTTP 数据传输方法。</returns>
        public string HttpMethod {
            get {
                return _httpMethod;
            }
            set {
                _httpMethod = value;
            }
        }
        #endregion
        #region InputStream
        /// <summary>
        /// 获取或设置包含正文数据的流，这些数据由客户端发送。
        /// </summary>
        /// <returns>一个可读的 System.IO.Stream 对象，该对象包含客户端在请求正文中发送的字节。如果没有随请求发送任何数据，则此属性返回 System.IO.Stream.Null。</returns>
        public System.IO.Stream InputStream {
            get {
                return _inputStream;
            }
            set {
                _inputStream = value;
            }
        }
        #endregion
        #region IsLocal
        /// <summary>
        /// 获取或设置 System.Boolean 值，该值指示该请求是否来自本地计算机。
        /// </summary>
        /// <returns>如果发出请求的计算机就是提供该请求的 HttpListener 对象所在的计算机，则为 true；否则为 false。</returns>
        public bool IsLocal {
            get {
                return _isLocal;
            }
            set {
                _isLocal = value;
            }
        }
        #endregion
        #region IsSecureConnection
        /// <summary>
        /// 获取或设置一个 System.Boolean 值，该值指示用来发送请求的 TCP 连接是否使用安全套接字层 (SSL) 协议。
        /// </summary>
        /// <returns>如果 TCP 连接使用的是 SSL，则为 true；否则为 false。</returns>
        public bool IsSecureConnection {
            get {
                return _isSecureConnection;
            }
            set {
                _isSecureConnection = value;
            }
        }
        #endregion
        #region KeepAlive
        /// <summary>
        /// 获取一个 System.Boolean 值，该值指示客户端是否请求持续型连接。
        /// </summary>
        /// <returns>如果连接应保持打开状态，则为 true；否则为 false。</returns>
        public bool KeepAlive {
            get {
                if (_keepAlive == null)
                    _keepAlive = string.Equals(_headers["Connection"], "keep-alive", System.StringComparison.OrdinalIgnoreCase);
                return _keepAlive.Value;
            }
        }
        #endregion
        #region ProtocolVersion
        /// <summary>
        /// 获取请求客户端使用的 HTTP 版本。
        /// </summary>
        /// <returns>System.Version 用于标识 HTTP 的客户端版本。</returns>
        public System.Version ProtocolVersion {
            get {
                return _protocolVersion;
            }
            set {
                _protocolVersion = value;
            }
        }
        #endregion
        #region QueryString
        /// <summary>
        /// 获取 HTTP 查询字符串变量集合（网址上 ? 后面的数据。）。
        /// </summary>
        public System.Collections.Specialized.NameValueCollection QueryString {
            get {
                if (_queryString == null) {
                    string query = Url.Query;
                    if (!string.IsNullOrEmpty(query) && query.StartsWith("?"))
                        query = query.Substring(1);
                    _queryString = ParseNameValues(query, ContentEncoding);
                }
                return _queryString;
            }
            set {
                _queryString = value;
            }
        }
        #endregion
        #region Form
        /// <summary>
        /// 获取窗体变量集合，需要注意的是，仅当POST时才会有真正有数据，其它情况其实是空白的集合；另外POST中的数据不是正常可以识别的时，它也是空白的集合，不会报错。
        /// </summary>
        /// <returns>表示窗体变量集合的 System.Collections.Specialized.NameValueCollection。</returns>
        public System.Collections.Specialized.NameValueCollection Form {
            get {
                if (_form == null) {
                    ParseForm();
                }
                return _form;
            }
            set {
                _form = value;
            }
        }
        #endregion
        #region ServerVariables
        /// <summary>
        /// 获取 Web 服务器变量的集合（目前它是一个空集合，没有任何参数，并且不调用它，不会创建它的。）。
        /// </summary>
        public System.Collections.Specialized.NameValueCollection ServerVariables {
            get {
                if (_serverVariables == null) {
                    _serverVariables = new System.Collections.Specialized.NameValueCollection();
                }
                return _serverVariables;
            }
        }
        #endregion
        #region Params
        /// <summary>
        /// 获取一个组合参数的集合，它组成的顺序为：QueryString、Form、Cookies、ServerVariables，需要注意，不调用时它不会去组合，不占资源的。
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Params {
            get {
                if (_params == null) {
                    _params = new System.Collections.Specialized.NameValueCollection();
                    _params.Add(QueryString);
                    if (HttpMethod == "POST") {
                        foreach (var key in Form.AllKeys) {
                            _params.Set(key, Form[key]);
                        }
                    }
                    if (_serverVariables != null) {
                        foreach (var key in _serverVariables.AllKeys) {
                            _params.Set(key, _serverVariables[key]);
                        }
                    }
                    if (_cookies != null) {
                        foreach (var key in _cookies.AllKeys) {
                            _params.Set(key, _cookies[key]);
                        }
                    }
                }
                return _params;
            }
        }
        #endregion
        #region Files
        /// <summary>
        /// 获取采用多部分 MIME 格式的由客户端上载的文件的集合（仅当POST文件时才有，并且不调用它 或 Form 或 this[key]，是不会解析的，即不占用资源）。
        /// </summary>
        public IHttpFileCollection Files {
            get {
                if (_files == null) {
                    ParseForm();
                    if (_files == null)
                        _files = new HttpFileCollection();
                }
                return _files;
            }
            set {
                _files = value;
            }
        }
        #endregion
        #region this[string key]
        /// <summary>
        /// 获取组合值，注意它不是调用的Params，寻找顺序：Form（仅POST时）、QueryString、Cookies、ServerVariables。
        /// </summary>
        /// <param name="key">要获取的集合成员的名称。</param>
        /// <returns>如果未找到指定的 key，则返回 null。</returns>
        public string this[string key] {
            get {
                string value = null;
                if (HttpMethod == "POST") {
                    value = Form[key];
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
                value = QueryString[key];
                if (!string.IsNullOrEmpty(value))
                    return value;

                if (_cookies != null) {
                    value = _cookies[key];
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }

                if (_serverVariables != null) {
                    value = _serverVariables[key];
                    if (string.IsNullOrEmpty(value))
                        return value;
                }
                return null;
                //return ServerVariables[key];
            }
        }
        #endregion
        #region RawUrl
        /// <summary>
        /// 获取客户端请求的 URL 信息（不包括主机和端口）。
        /// </summary>
        /// <returns>System.String 包含此请求的原始 URL。</returns>
        public string RawUrl {
            get {
                return _rawUrl;
            }
            set {
                _rawUrl = value;
            }
        }
        #endregion
        #region LocalEndPoint
        /// <summary>
        /// 获取请求被定向到的服务器 IP 地址和端口号。
        /// </summary>
        /// <returns>System.Net.IPEndPoint 表示请求被发送到的 IP 地址。</returns>
        public System.Net.IPEndPoint LocalEndPoint {
            get {
                return _localEndPoint;
            }
            set {
                _localEndPoint = value;
            }
        }
        #endregion
        #region RemoteEndPoint
        /// <summary>
        /// 获取发出请求的客户端 IP 地址和端口号。
        /// </summary>
        /// <returns>System.Net.IPEndPoint 表示发出请求的 IP 地址和端口号。</returns>
        public System.Net.IPEndPoint RemoteEndPoint {
            get {
                return _remoteEndPoint;
            }
            set {
                _remoteEndPoint = value;
            }
        }
        #endregion
        #region Url
        /// <summary>
        /// 获取有关当前请求的 URL 的信息。
        /// </summary>
        /// <returns>包含有关当前请求的 URL 的信息的 System.Uri 对象。</returns>
        public System.Uri Url {
            get {
                return _url;
            }
            set {
                _url = value;
            }
        }
        #endregion
        #region UrlReferrer
        /// <summary>
        /// 获取有关当前请求的 URL 的信息。
        /// </summary>
        /// <returns>包含有关当前请求的 URL 的信息的 System.Uri 对象。</returns>
        public System.Uri UrlReferrer {
            get {
                return _urlReferrer;
            }
            set {
                _urlReferrer = value;
            }
        }
        #endregion
        #region UserAgent
        /// <summary>
        /// 获取客户端浏览器的原始用户代理信息。
        /// </summary>
        /// <returns>客户端浏览器的原始用户代理信息。</returns>
        public string UserAgent {
            get {
                if (_userAgent == null) {
                    _userAgent = _headers["User-Agent"];
                }
                return _userAgent;
            }
        }
        #endregion
        #region UserHostAddress
        /// <summary>
        /// 获取远程客户端的 IP 主机地址。
        /// </summary>
        /// <returns>远程客户端的 IP 地址。</returns>
        public string UserHostAddress {
            get {
                return _userHostAddress;
            }
            set {
                _userHostAddress = value;
            }
        }
        #endregion
        #region UserHostName
        /// <summary>
        /// 获取远程客户端的 DNS 名称（目前直接返回的是客户端的IP）。
        /// </summary>
        /// <returns>远程客户端的 DNS 名称。</returns>
        public string UserHostName {
            get {
                return _userHostName;
            }
            set {
                _userHostName = value;
            }
        }
        #endregion
        #region UserLanguages
        /// <summary>
        /// 获取客户端语言首选项的字符串数组（暂时未排序，按照客户端提交的顺序；常见的是 zh-CN）。
        /// </summary>
        public string[] UserLanguages {
            get {
                if (_userLanguages == null) {
                    string value = _headers["Accept-Language"];
                    if (!string.IsNullOrEmpty(value)) {
                        _userLanguages = value.Split(',');
                        for (int i = 0; i < _userLanguages.Length; i++) {
                            _userLanguages[i] = _userLanguages[i].Trim();
                        }
                    }
                    if (_userLanguages == null || _userLanguages.Length == 0)
                        _userLanguages = new string[] { "en-us", "en" };
                }
                return _userLanguages;
            }
        }
        #endregion

        /// <summary>
        /// 获取或设置二进制读取委托。
        /// </summary>
        public BinaryReadHandlerDelgate BinaryReadHandler {
            get { return _binaryReadHandler; }
            set { _binaryReadHandler = value; }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpListenerRequest实例。
        /// </summary>
        public HttpListenerRequest() {
            _headers = new System.Collections.Specialized.NameValueCollection();
        }
        #endregion

        #region methods

        #region ParseNameValues
        System.Collections.Specialized.NameValueCollection ParseNameValues(string content, System.Text.Encoding encoding, string separator = "&") {
            System.Collections.Specialized.NameValueCollection result = new System.Collections.Specialized.NameValueCollection();
            if (string.IsNullOrEmpty(content))
                return result;
            foreach (string item in content.Split(new string[] { separator }, System.StringSplitOptions.None)) {
                string key = string.Empty;
                string value = string.Empty;
                if (item.IndexOf('=') == -1) {
                    value = item;
                } else {
                    string[] array = item.Split('=');
                    key = HttpUtility.UrlDecode(array[0], encoding);
                    value = array[1];
                }
                result.Add(key, HttpUtility.UrlDecode(value, encoding));
            }
            return result;
        }
        #endregion
        #region ParseForm
        void ParseForm() {

            if (HttpMethod == "POST") {
                //if contentType
                string contentType = ContentType;
                if (string.IsNullOrEmpty(contentType) || contentType.StartsWith("application/x-www-form-urlencoded", System.StringComparison.OrdinalIgnoreCase)) {
                    try {
                        byte[] buffer = ReadFormData();
                        string text = ContentEncoding.GetString(buffer);
                        System.Text.Encoding encoding = GetCharset(contentType);
                        _form = ParseNameValues(text, encoding == null ? _contentEncoding : encoding);
                    } catch {
                    }
                } else if (contentType.StartsWith("multipart/form-data;", System.StringComparison.OrdinalIgnoreCase)) {
                    try {
                        byte[] buffer = ReadFormData();
                        System.Text.Encoding encoding = GetCharset(contentType);
                        ParseMultiPart(contentType, buffer, encoding == null ? _contentEncoding : encoding);
                    } catch {
                    }
                }
            }
            if (_form == null)
                _form = new System.Collections.Specialized.NameValueCollection();
        }
        #endregion
        #region ParseMultiPart
        void ParseMultiPart(string contentType, byte[] buffer, System.Text.Encoding encoding) {
            int boundaryIndex = contentType.IndexOf("boundary=", System.StringComparison.OrdinalIgnoreCase);
            if (boundaryIndex == -1) {
                return;
            }
            _form = new System.Collections.Specialized.NameValueCollection();
            var files = new HttpFileCollection();
            _files = files;

            string boundary = contentType.Substring(boundaryIndex + "boundary=".Length);
            byte[] spliter = System.Text.Encoding.ASCII.GetBytes(boundary + "\r\nContent-Disposition: form-data; name=\"");
            //byte[] endFlags = System.Text.Encoding.ASCII.GetBytes(boundary + "--");
            bool ended = false;
            int index = 0;
            int endIndex = -1;

            while (!ended && index < buffer.Length - 1) {
                index += spliter.Length + 2;
                endIndex = BinarySearch(buffer, index, 34);
                if (endIndex == -1)
                    break;
                string name = encoding.GetString(buffer, index, endIndex - index);
                index = endIndex + 1;
                bool isFile = false;
                string filename = null;
                string contentTypeHeader = null;
                while (true) {
                    if (buffer[index] == 59) {//; 还有其它值
                        index += 2;
                        endIndex = BinarySearch(buffer, index, 61);//=
                        if (endIndex == -1) {
                            ended = true;
                            break;
                        }
                        string name2 = encoding.GetString(buffer, index, endIndex - index);
                        index = endIndex + 2;//="
                        endIndex = BinarySearch(buffer, index, 34);
                        if (endIndex == -1) {
                            ended = true;
                            break;
                        }
                        string value2 = encoding.GetString(buffer, index, endIndex - index);
                        index = endIndex + 1;//\r
                                             //Console.WriteLine("{0}=[{1}]", name2, value2);
                        if (string.Equals(name2, "filename", System.StringComparison.OrdinalIgnoreCase)) {
                            isFile = true;
                            filename = value2;
                        }
                    } else {
                        break;
                    }
                }
                if (ended)
                    break;
                index++;//\n

                if (buffer[index + 1] != 13) {//headers
                    index++;
                    endIndex = BinarySearch(buffer, index, 58);//:
                    if (endIndex == -1) {
                        break;
                    }
                    string headerName = System.Text.Encoding.ASCII.GetString(buffer, index, endIndex - index);
                    index = endIndex + 2;
                    endIndex = BinarySearch(buffer, index, 13);//\r
                    if (endIndex == -1) {
                        break;
                    }
                    string headerValue = System.Text.Encoding.ASCII.GetString(buffer, index, endIndex - index);
                    index = endIndex + 1;//\r
                    if (string.Equals(headerName, "Content-Type", System.StringComparison.OrdinalIgnoreCase)) {
                        contentTypeHeader = headerValue;
                    }
                }
                index += 3;
                if (isFile) {
                    endIndex = BinarySearch(buffer, index, 13, new byte[] { 13, 10, 45, 45, 45, 45 });
                } else {
                    endIndex = BinarySearch(buffer, index, 13, new byte[] { 13, 10 });
                }
                if (endIndex == -1) {
                    break;
                }
                int valueLength = endIndex - index;
                if (isFile) {
                    _form.Add(name, filename);
                    System.IO.MemoryStream inputStream = null;
                    if (valueLength > 0) {
                        inputStream = new System.IO.MemoryStream(valueLength);
                        inputStream.Position = 0;
                        inputStream.Write(buffer, index, valueLength);
                        inputStream.Position = 0;
                    }
                    if (string.IsNullOrEmpty(contentType))
                        contentType = "application/octet-stream";
                    HttpPostedFile file = new HttpPostedFile(inputStream, filename, valueLength, contentTypeHeader);
                    files.Add(name, file);
                } else {
                    byte[] valueBuffer = new byte[valueLength];
                    System.Array.Copy(buffer, index, valueBuffer, 0, valueLength);
                    _form.Add(name, encoding.GetString(valueBuffer));
                }
                index = endIndex + 2;
            }
        }
        #endregion
        #region BinarySearch
        int BinarySearch(byte[] buffer, int index, byte nextChar) {
            for (int i = index; i < buffer.Length; i++) {
                if (buffer[i] == nextChar) {
                    return i;
                }
            }
            return -1;
        }
        int BinarySearch(byte[] buffer, int index, byte nextChar, byte[] nextSpliter) {
            for (int i = index; i < buffer.Length; i++) {
                if (buffer[i] == nextChar) {
                    bool ok = true;
                    for (int j = 1; j < nextSpliter.Length; j++) {
                        if (buffer[i + j] != nextSpliter[j]) {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                        return i;
                }
            }
            return -1;
        }
        #endregion
        #region ReadFormData
        byte[] ReadFormData() {
            return BinaryRead(true);
        }
        #endregion
        #region BinaryRead
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取（读取所有可用数据，并且不还原流位置）。
        /// </summary>
        /// <returns>字节数组。</returns>
        public byte[] BinaryRead() {
            return BinaryRead((int)ContentLength, false);
        }
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取（读取所有可用数据）。
        /// </summary>
        /// <returns>字节数组。</returns>
        public byte[] BinaryRead(bool resetPosition) {
            return BinaryRead((int)ContentLength, resetPosition);
        }
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取（不还原流位置）。
        /// </summary>
        /// <param name="count">要读取的字节数，超出有效范围时仅读取有效的部分。</param>
        /// <returns>字节数组。</returns>
        public byte[] BinaryRead(int count) {
            return BinaryRead(count, false);
        }
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取。
        /// </summary>
        /// <param name="count">要读取的字节数，超出有效范围时仅读取有效的部分。</param>
        /// <param name="resetPosition">是否还原读取之前的流位置。</param>
        /// <returns>字节数组。</returns>
        public byte[] BinaryRead(int count, bool resetPosition) {
            if (count<1 || ContentLength < 1 || !HasEntityBody) {
                return new byte[0];
            }
            if (_binaryReadHandler != null) {
                return _binaryReadHandler(count, resetPosition);
            }
            int length = (int)(_inputStream.Length - _inputStream.Position);
            if (count > length)
                count = length;
            if (resetPosition) {
                long oriIndex = _inputStream.Position;
                try {
                    byte[] buffer = new byte[count];
                    _inputStream.Read(buffer, 0, count);
                    return buffer;
                } finally {
                    _inputStream.Position = oriIndex;
                }
            } else {
                byte[] buffer = new byte[count];
                _inputStream.Read(buffer, 0, count);
                return buffer;
            }
        }
        #endregion
        #region GetCharset
        /// <summary>
        /// 尝试解析编码。
        /// </summary>
        /// <param name="value">包含编码的文本。</param>
        /// <returns>不存在将返回null。</returns>
        public static System.Text.Encoding GetCharset(string value) {
            if (string.IsNullOrEmpty(value))
                return null;
            string[] strArray = value.ToLower().Split(new char[] { ';', '=', ' ' });
            bool flag = false;
            System.Text.Encoding result = null;
            foreach (string str2 in strArray) {
                if (str2 == "charset") {
                    flag = true;
                } else if (flag) {
                    try {
                        result = System.Text.Encoding.GetEncoding(str2);
                    } catch {
                    }
                    break;
                }
            }
            return result;
        }
        #endregion
        #region Dispose
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            if (_disposed)
                return;
            if (_headers != null) {
                _headers.Clear();
                _headers = null;
            }
            if (_cookies != null) {
                _cookies.Clear();
                _cookies = null;
            }
            if (_queryString != null) {
                _queryString.Clear();
                _queryString = null;
            }
            if (_form != null) {
                _form.Clear();
                _form = null;
            }
            if (_serverVariables != null) {
                _serverVariables.Clear();
                _serverVariables = null;
            }
            if (_params != null) {
                _params.Clear();
                _params = null;
            }
            if (_files != null) {
                _files.Dispose();
                _files = null;
            }
            if (_inputStream != null) {
                try { _inputStream.Dispose(); } catch { }
                _inputStream = null;
            }
            _acceptTypes = null;
            _userAgent = null;
            _userLanguages = null;
            _contentEncoding = null;
            _contentType = null;
            _rawUrl = null;
            _url = null;
            _urlReferrer = null;
            _httpMethod = null;
            _protocolVersion = null;
            _localEndPoint = null;
            _remoteEndPoint = null;
            _userHostAddress = null;
            _userHostName = null;
            _binaryReadHandler = null;

            _disposed = true;

        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取。
        /// </summary>
        /// <param name="count">要读取的字节数，超出有效范围时仅读取有效的部分。</param>
        /// <param name="resetPosition">是否还原读取之前的流位置。</param>
        /// <returns>字节数组。</returns>
        public delegate byte[] BinaryReadHandlerDelgate(int count, bool resetPosition);
        #endregion

    }
}