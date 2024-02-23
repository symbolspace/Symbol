/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Web {
    /// <summary>
    /// 表示对 HttpListener 对象正在处理的请求的响应。
    /// </summary>
    public class HttpListenerResponse : IHttpListenerResponse {

        #region fields
        private string _statusDescription = null;
        private int _statusCode = 0;
        private System.Text.Encoding _contentEncoding = null;
        private long _contentLength;
        private HttpCookieCollection _cookies = null;
        private System.Collections.Specialized.NameValueCollection _headers = null;
        private System.IO.TextWriter _output = null;
        private System.IO.Stream _outputStream = null;
        private System.Version _protocolVersion;
        private bool _disposed = false;
        private bool _buffer = false;
        private bool _suppressContent = false;
        private bool _headerOutputed = false;
        private IHttpConnection _connection;
        private static readonly string _redirectFormat = "<script type=\"text/javascript\">window.location=decodeURIComponent('{0}');</script>";
        private static readonly string _alertFormat = "<script type=\"text/javascript\">alert(decodeURIComponent('{0}'));</script>";
        private static readonly string _alertFormatDelay = "<script type=\"text/javascript\">setTimeout(function(){{alert(decodeURIComponent('{0}'));}},{1});</script>";

        #endregion

        #region properties

        #region ContentEncoding
        /// <summary>
        /// 获取或设置输出流的 HTTP 字符集。
        /// </summary>
        /// <returns>System.Text.Encoding 对象，包含与当前响应的字符集有关的信息。</returns>
        public System.Text.Encoding ContentEncoding {
            get {
                return _contentEncoding;
            }
            set {
                _contentEncoding = value;
            }
        }
        #endregion
        #region ContentLength
        /// <summary>
        /// 获取或设置响应中包括的正文数据的字节数。
        /// </summary>
        /// <returns>响应的 Content-Length 标头的值。</returns>
        public long ContentLength {
            get {
                return _contentLength;
            }
            set {
                _contentLength = value;
                _headers["Content-Length"] = value.ToString();
            }
        }
        #endregion
        #region ContentType
        /// <summary>
        /// 获取或设置输出流的 HTTP MIME 类型（请别写上charset字样，设置字符集，请用ContentEncoding属性。）。
        /// </summary>
        /// <returns>输出流的 HTTP MIME 类型。默认值为“text/html”。</returns>
        public string ContentType {
            get {
                return _headers["Content-Type"];
            }
            set {
                _headers["Content-Type"] = value;
            }
        }
        #endregion
        #region Cookies
        /// <summary>
        /// 获取需要输出到客户端的Cookie集合，不用再像ASP.NET那样AppendCookie和SetCookie了。
        /// </summary>
        public IHttpCookieCollection Cookies {
            get {
                return _cookies;
            }
        }
        #endregion
        #region Headers
        /// <summary>
        /// 获取响应标头的集合（会包括全局和本站点设置在配置文件中的标头），不用再像ASP.NET那样麻烦了。
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Headers {
            get {
                return _headers;
            }
        }
        #endregion
        #region KeepAlive
        /// <summary>
        /// 获取或设置一个值，该值指示服务器是否请求持久性连接。
        /// </summary>
        /// <returns>如果服务器请求持久性连接，则为 true；否则为 false。默认为 true。</returns>
        public bool KeepAlive {
            get {
                return string.Equals(_headers["Connection"], "keep-alive", StringComparison.OrdinalIgnoreCase);
            }

            set {
                _headers["Connection"] = value ? "keep-alive" : "close";
            }
        }
        #endregion
        #region Buffer
        /// <summary>
        /// 获取或设置一个值，该值指示是否缓冲输出并在处理完整个响应之后发送它（为true后，仅手动调用Flush或Close和End时才会输出。）。
        /// </summary>
        public bool Buffer {
            get { return _buffer; }
            set { _buffer = value; }
        }
        #endregion
        #region SuppressContent
        /// <summary>
        /// 获取或设置一个值，该值指示是否将 HTTP 内容发送到客户端（相当于短暂的静默，不输出任何内容，但还在缓冲区中）。
        /// </summary>
        /// <returns>如果取消输出，则为 true；否则为 false。</returns>
        public bool SuppressContent {
            get { return _suppressContent; }
            set { _suppressContent = value; }
        }
        #endregion
        #region IsClientConnected
        /// <summary>
        /// 获取一个值，通过该值指示客户端是否仍连接在服务器上。
        /// </summary>
        /// <returns>如果客户端当前仍在连接，则为 true；否则为 false。</returns>
        public bool IsClientConnected {
            get {
                return _connection == null ? false : _connection.IsClientConnected;
            }
        }
        #endregion
        #region Output
        /// <summary>
        /// 获取或设置输出 HTTP 响应流的文本输出。
        /// </summary>
        /// <returns>支持到客户端的自定义输出的 System.IO.TextWriter 对象。</returns>
        public System.IO.TextWriter Output {
            get {
                return _output;
            }
            set {
                if (_output != null) {
                    HtmlWriter writer = _output as HtmlWriter;
                    if (writer != null)
                        writer.Dispose();
                }
                _output = value;
            }
        }
        #endregion
        #region CacheControl
        /// <summary>
        /// 获取或设置Cache-Control HTTP 标头（注意不会像ASP.NET那样乱套，你设置什么它就是什么）。
        /// </summary>
        public string CacheControl {
            get {
                return _headers["Cache-Control"];
            }
            set {
                _headers["Cache-Control"] = value;
            }
        }
        #endregion
        #region OutputStream
        /// <summary>
        /// 获取输出 HTTP 内容主体的二进制输出。
        /// </summary>
        /// <returns>表示输出 HTTP 内容主体的原始内容的 IO System.IO.Stream。</returns>
        public System.IO.Stream OutputStream {
            get {
                return _outputStream;
            }
        }
        #endregion
        #region ProtocolVersion
        /// <summary>
        /// 获取请求客户端使用的 HTTP 版本。
        /// </summary>
        /// <returns>System.Version 用于标识 HTTP 的客户端版本。</returns>
        public Version ProtocolVersion {
            get {
                return _protocolVersion;
            }
            set {
                _protocolVersion = value ?? new Version(1, 1);
            }
        }
        #endregion
        #region RedirectLocation
        /// <summary>
        /// 获取或设置 HTTP Location 标头的值。
        /// </summary>
        /// <returns>System.String，包含要在 Location 标头中发送给客户端的绝对 URL。</returns>
        public string RedirectLocation {
            get {
                return _headers["Location"];
            }

            set {
                _headers["Location"] = value;
            }
        }
        #endregion
        #region StatusCode
        /// <summary>
        /// 获取或设置返回给客户端的输出的 HTTP 状态代码。
        /// </summary>
        /// <returns>表示返回到客户端的 HTTP 输出状态的整数。默认值为 200 (OK)。有关有效状态代码的列表，请参见 Http Status Codes（Http状态代码）。</returns>
        public int StatusCode {
            get {
                return _statusCode;
            }
            set {
                _statusCode = value;
            }
        }
        #endregion
        #region StatusDescription
        /// <summary>
        /// 获取或设置返回给客户端的输出的 HTTP 状态字符串。
        /// </summary>
        /// <returns>一个字符串，描述返回给客户端的 HTTP 输出的状态。默认值为“OK”。有关有效状态代码的列表，请参见 Http Status Codes（Http状态代码）。</returns>
        public string StatusDescription {
            get {
                return _statusDescription;
            }
            set {
                _statusDescription = value;
            }
        }
        #endregion
        /// <summary>
        /// 获取或设置是否已处理。
        /// </summary>
        public bool Handled { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpListenerResponse实例。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="outputStream"></param>
        /// <param name="connection"></param>
        public HttpListenerResponse(IHttpListenerRequest request, System.IO.Stream outputStream, IHttpConnection connection) {
            _cookies = new HttpCookieCollection();
            _headers = new System.Collections.Specialized.NameValueCollection();
            _connection = connection;
            _protocolVersion = request.ProtocolVersion;
            _contentEncoding = request.ContentEncoding;
            _outputStream = outputStream;
            _output = new HtmlWriter(this);
            _buffer = true;
            Clear();
        }
        #endregion

        #region methods

        #region Clear
        /// <summary>
        /// 清除缓冲区流中的所有内容输出。
        /// </summary>
        public void Clear() {
            ClearHeaders();
            ClearContent();
        }
        #endregion
        #region ClearContent
        /// <summary>
        /// 清除缓冲区流中的所有内容输出。
        /// </summary>
        public void ClearContent() {
            HtmlWriter writer = _output as HtmlWriter;
            if (writer != null)
                writer.Clear();
        }
        #endregion
        #region ReplaceOutputToHtmlWriter
        void ReplaceOutputToHtmlWriter() {
            HtmlWriter writer = _output as HtmlWriter;
            if (writer == null) {
                _output = new HtmlWriter(this);
            }
        }
        #endregion
        #region ClearHeaders
        /// <summary>
        /// 清除缓冲区流中的所有头。
        /// </summary>
        public void ClearHeaders() {
            if (_headerOutputed)
                return;
            _contentLength = 0;
            _statusCode = 200;
            _statusDescription = "OK";
            _headers.Clear();
            ContentType = "text/html";
            _headers["Server"] = "Symbol.WebServer";
        }
        #endregion

        #region AddHeader
        /// <summary>
        /// 将指定的标头和值添加到此响应的 HTTP 标头。
        /// </summary>
        /// <param name="name">要设置的 HTTP 标头的名称。</param>
        /// <param name="value">name 标头的值。</param>
        public void AddHeader(string name, string value) {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                return;
            Headers.Add(name, value);
        }
        #endregion
        #region AppendHeader
        /// <summary>
        /// 向随此响应发送的指定 HTTP 标头追加值。
        /// </summary>
        /// <param name="name">要追加 value 的 HTTP 标头的名称。</param>
        /// <param name="value">要追加到 name 标头的值。</param>
        public void AppendHeader(string name, string value) {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                return;
            Headers.Add(name, value);
        }
        #endregion
        #region AppendCookie
        /// <summary>
        /// 将指定的 Cookie 添加到此响应的 Cookie 集合。
        /// </summary>
        /// <param name="cookie">cookie。</param>
        public void AppendCookie(IHttpCookie cookie) {
            if (cookie == null)
                return;
            Cookies.Add(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
        }
        /// <summary>
        /// 将指定的 Cookie 添加到此响应的 Cookie 集合。
        /// </summary>
        /// <param name="name">cookie名称。</param>
        /// <param name="value">cookie值。</param>
        public void AppendCookie(string name, string value) {
            if (string.IsNullOrEmpty(name))
                Cookies.Add(name, value);
        }
        #endregion
        #region BinaryWrite
        /// <summary>
        /// 将一个二进制字符串写入 HTTP 输出流。
        /// </summary>
        /// <param name="buffer">要写入输出流的字节。</param>
        public void BinaryWrite(byte[] buffer) {
            if (_outputStream != null) {
                //onsole.WriteLine("BinaryWrite.in length:{0}",buffer.Length);
                _output?.Flush();
                _outputStream.Write(buffer, 0, buffer.Length);
                Handled = true;
            }
        }
        #endregion
        #region Write
        /// <summary>
        /// 将一个字符写入 HTTP 响应输出流（基本不用）。
        /// </summary>
        /// <param name="value">要写入 HTTP 输出流的字符。</param>
        public void Write(char value) {
            if (_output == null)
                return;
            _output.Write(value);
            Handled = true;
        }
        /// <summary>
        /// 将 System.Object 写入 HTTP 响应流（很少用）。
        /// </summary>
        /// <param name="value">要写入 HTTP 输出流的 System.Object。</param>
        public void Write(object value) {
            if (_output == null)
                return;
            _output.Write(value);
            Handled = true;
        }
        /// <summary>
        /// 将一个字符串写入 HTTP 响应输出流（最常用就是它）。
        /// </summary>
        /// <param name="value">要写入 HTTP 输出流的字符串。</param>
        public void Write(string value) {
            if (_output == null)
                return;
            _output.Write(value);
            Handled = true;
        }
        /// <summary>
        /// 将一个字符串写入 HTTP 响应输出流（string.Format用过吗？）。
        /// </summary>
        /// <param name="format">要写入 HTTP 输出流的字符串（为空或空字符串，不会进行Format行为）。</param>
        /// <param name="args">参于格式化的参数。</param>
        public void Write(string format, params object[] args) {
            if (_output == null)
                return;
            if (string.IsNullOrEmpty(format))
                return;
            _output.Write(format, args);
            Handled = true;
        }
        /// <summary>
        /// 将一个字符数组写入 HTTP 响应输出流（基本用不上的它）。
        /// </summary>
        /// <param name="buffer">要写入的字符数组。</param>
        /// <param name="index">字符数组中开始进行写入的位置。</param>
        /// <param name="count">从 index 开始写入的字符数。</param>
        public void Write(char[] buffer, int index, int count) {
            if (_output == null)
                return;
            _output.Write(buffer, index, count);
            Handled = true;
        }
        #endregion
        #region Json
        /// <summary>
        /// 输出一个json对象（不格式化）。
        /// </summary>
        /// <param name="value">可序列化为json的对象。</param>
        public void Json(object value) {
            Json(value, false);
        }
        /// <summary>
        /// 输出一个json对象。
        /// </summary>
        /// <param name="value">可序列化为json的对象。</param>
        /// <param name="formated">是否格式化json文本。</param>
        public void Json(object value, bool formated = false) {
            Clear();
            ContentType = "application/json";
            ContentEncoding = System.Text.Encoding.UTF8;
            Write(Symbol.Serialization.Json.ToString(value, true, formated));
            Flush();
        }
        #endregion
        #region Javascript
        /// <summary>
        /// 输出一段javascript脚本。
        /// </summary>
        /// <param name="script">脚本内容，为空或空文本时，自动输出为"//"。</param>
        public void Javascript(string script) {
            Clear();
            ContentType = "application/x-javascript";
            ContentEncoding = System.Text.Encoding.UTF8;
            Write(string.IsNullOrEmpty(script) ? "//" : script);
            Flush();
        }
        /// <summary>
        /// 输出一个javascript脚本（var name=json(value);）。
        /// </summary>
        /// <param name="name">变量名称，不能为空。</param>
        /// <param name="value">可序列化为json的对象。</param>
        public void Javascript(string name, object value) {
            if (string.IsNullOrEmpty(name))
                name = "$var";
            Javascript(string.Format("var {0}={1};", name, Symbol.Serialization.Json.ToString(value)));
        }
        #endregion

        #region Redirect
        /// <summary>
        /// 将请求重定向到新 URL 并指定该新 URL。
        /// </summary>
        /// <param name="url">目标位置。</param>
        public void Redirect(string url) {
            Clear();
            ReplaceOutputToHtmlWriter();

            _statusCode = 302;
            _statusDescription = "Moved Temporarily";
            CacheControl = "no-cache, no-store, max-age=0, must-revalidate";
            _headers["Location"] = url;
            _output.Write("<html><head><title>302 Found</title></head><body bgcolor=\"white\"><body><a href=\"{0}\">here</a></body></html>", url);
        }
        /// <summary>
        /// 重定向到一个Url
        /// </summary>
        /// <param name="url">可以是相对路径</param>
        /// <param name="message">跳转前要弹出的消息</param>
        public void Redirect(string url, string message) {
            if (string.IsNullOrEmpty(message)) {
                Redirect(url);
                return;
            }
            Alert(message);
            Write(_redirectFormat, HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8));
        }
        #endregion
        #region Alert
        /// <summary>
        /// 输出一段脚本，示意要弹出一个对话框，alert。
        /// </summary>
        /// <param name="message">要弹出的消息，如果为空或空字符串，将不输出脚本</param>
        /// <remarks> &lt;script&gt;alert(decodeURIComponent('%EC%20%3D'));&lt;/script&gt;</remarks>
        public void Alert(string message) {
            if (string.IsNullOrEmpty(message)) {
                return;
            }
            Write(_alertFormat, HttpUtility.UrlEncode(message, System.Text.Encoding.UTF8));
        }
        /// <summary>
        /// 输出一段脚本，示意要弹出一个对话框，alert。[延迟]
        /// </summary>
        /// <param name="message">要弹出的消息，如果为空或空字符串，将不输出脚本</param>
        /// <param name="delay">延迟时间，单位：毫秒</param>
        /// <remarks> &lt;script&gt;setTimeout(function(){alert(decodeURIComponent('%EC%20%3D'));},delay);&lt;/script&gt;</remarks>
        public void Alert(string message, int delay) {
            if (string.IsNullOrEmpty(message)) {
                return;
            }
            if (delay < 1)
                Alert(message);
            else
                Write(_alertFormatDelay, HttpUtility.UrlEncode(message, System.Text.Encoding.UTF8), delay);
        }
        #endregion

        #region Abort
        /// <summary>
        /// 关闭到客户端的连接而不发送响应。
        /// </summary>
        public void Abort() {
            if (_connection != null) {
                _connection.Close();
                _connection = null;
            }
            Dispose(true);
        }
        #region Flush
        /// <summary>
        /// 向客户端发送当前所有缓冲的输出。
        /// </summary>
        public void Flush() {
            Handled = true;
            if (_output != null)
                _output.Flush();
            if (_outputStream != null)
                _outputStream.Flush();
        }
        #endregion
        #region Close
        /// <summary>
        /// 关闭到客户端的套接字连接（不建议手动调用它，在关闭前会尝试输出缓冲中的数据）。
        /// </summary>
        public void Close() {
            Handled = true;
            Dispose(true);
        }
        #endregion
        ///// <summary>
        ///// 将指定的字节数组发送到客户端，并释放此 HttpListenerResponse 实例占用的资源。
        ///// </summary>
        ///// <param name="responseEntity">包含要发送给客户端的响应的 System.Byte 数组。</param>
        ///// <param name="willBlock">如果要在刷新到客户端的流时阻止执行，则为 true；否则为 false。</param>
        //public void Close(byte[] responseEntity, bool willBlock) {
        //    throw new NotImplementedException();
        //}
        #endregion

        #region Dispose
        /// <summary>
        /// 
        /// </summary>
        ~HttpListenerResponse() {
            Dispose(false);
        }
        /// <summary>
        /// 释放占用的资源，如果未Abort会尝试输出数据。
        /// </summary>
        public void Dispose() {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            if (_disposed)
                return;
            if (_output != null) {
                if (_connection != null) {
                    HtmlWriter writer = _output as HtmlWriter;
                    if (writer != null)
                        writer.Flush(true);
                    else
                        _output.Flush();
                }
                _output.Dispose();
                _output = null;
            }
            if (_outputStream != null) {
                if (_connection != null) {
                    _outputStream.Flush();
                }
                _outputStream.Dispose();
                _outputStream = null;
            }
            if (_connection != null) {
                _connection.Close();
                _connection = null;
            }
            _contentEncoding = null;
            _cookies = null;
            _headers = null;
            _statusDescription = null;
            _protocolVersion = null;

            _disposed = true;

        }
        #endregion

        #endregion

        #region types
        class HtmlWriter : System.IO.TextWriter {
            private HttpListenerResponse _response = null;
            private System.Text.StringBuilder _builder = null;

            #region cctor
            static HtmlWriter() {
            }
            #endregion

            public HtmlWriter(HttpListenerResponse response) {
                _response = response;
                _builder = new System.Text.StringBuilder();
            }

            public override System.Text.Encoding Encoding {
                get { return _response.ContentEncoding; }
            }
            public void Clear() {
                _builder.Remove(0, _builder.Length);
            }
            public override void Write(char value) {
                _builder.Append(value);
                if (!_response.Buffer)
                    Flush();
            }

            public override void Write(string value) {
                if (value != null) {
                    _builder.Append(value);
                    if (!_response.Buffer)
                        Flush();
                }
            }
            public override void Write(char[] buffer, int index, int count) {
                _builder.Append(buffer, index, count);
                if (!_response.Buffer)
                    Flush();
            }
            public void Flush(bool isEnd) {
                if (_response == null || !_response.IsClientConnected || _response.SuppressContent)
                    return;
                byte[] buffer = null;
                //Console.WriteLine("HtmlWriter.Flush isEnd={0}", isEnd);
                if (_builder.Length > 0) {
                    //Console.WriteLine("  builder.Length>0");
                    buffer = _response.ContentEncoding.GetBytes(_builder.ToString());
                    _builder.Remove(0, _builder.Length);
                }
                if (!_response._headerOutputed) {
                    if (isEnd && string.IsNullOrEmpty(_response.Headers["Content-Length"])) {
                        _response.Headers["Content-Length"] = (buffer == null ? 0 : buffer.Length).ToString();
                    }
                    //Console.WriteLine("  !headerOutputed");
                    OutputHeader();
                }
                if (buffer == null || buffer.Length == 0) {
                    if (_response._outputStream != null) {
                        _response._outputStream.Flush();
                    }
                    return;
                }
                if (_response._outputStream != null) {
                    _response._outputStream.Write(buffer, 0, buffer.Length);
                    _response._outputStream.Flush();
                }
            }
            public override void Flush() {
                Flush(false);
            }
            private static readonly string[] _invalidHeaderNames = new string[] {
                    "Date",
                    "Set-Cookie",
                    "Content-Type",
                    //"Content-Length",
                };
            void OutputHeader(int contentLength = -1) {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.AppendFormat("HTTP/{0} {1} {2}\r\n", _response.ProtocolVersion.ToString(), _response._statusCode, _response._statusDescription);
                builder.AppendFormat("Date: {0}\r\n",DateTimeExtensions.ToGMT(DateTime.Now));
                bool findConnection = false;
                bool findContentLength = false;
                foreach (string key in _response._headers.AllKeys) {
                    if (Array.Exists(_invalidHeaderNames, p => string.Equals(p, key, StringComparison.OrdinalIgnoreCase)))
                        continue;
                    if (string.Equals(key, "Connection", StringComparison.OrdinalIgnoreCase))
                        findConnection = true;
                    else if (string.Equals(key, "Content-Length", StringComparison.OrdinalIgnoreCase))
                        findContentLength = true;
                    if (string.Equals(key, "Server", StringComparison.OrdinalIgnoreCase)) {
                        string value = _response._headers[key];
                        //if (!string.IsNullOrEmpty(value))
                        //    value = StringExtensions.Replace(value, "{version}", Host._version, true);
                        _response._headers[key] = value;
                    }
                    builder.AppendFormat("{0}: {1}\r\n", key, _response._headers[key]);
                }
                builder.AppendFormat("Content-Type: {0}; charset={1}\r\n", _response.ContentType, _response._contentEncoding.WebName.ToLower());
                if (contentLength != -1 && !findContentLength) {
                    builder.AppendFormat("Content-Length: {0}\r\n", contentLength);
                }

                if (_response._cookies.Count > 0) {
                    builder.Append("Set-Cookie: ");
                    foreach (string key in _response._cookies.AllKeys) {
                        IHttpCookie cookie = _response._cookies[key];
                        if (cookie == null)
                            continue;

                        builder.AppendFormat("{0}={1}; ", cookie.Name, cookie.Value);
                        if (!string.IsNullOrEmpty(cookie.Path))
                            builder.AppendFormat("path={0}; ", cookie.Path);
                        if (!string.IsNullOrEmpty(cookie.Domain))
                            builder.AppendFormat("domain={0}; ", cookie.Domain);
                        if (cookie.HttpOnly)
                            builder.Append("HttpOnly; ");
                        else
                            builder.AppendFormat("expires={0}; ",DateTimeExtensions.ToGMT( cookie.Expires,'-'));
                        builder.AppendLine();
                    }
                }
                if (!findConnection)
                    builder.Append("Connection: close\r\n");

                builder.Append("\r\n");
                _response._headerOutputed = true;
                if (!_response.IsClientConnected && _response._outputStream == null)
                    return;
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(builder.ToString());
                //Console.WriteLine("OutputHeader length:{0},string.length:{1}",buffer.Length,builder.Length);
                _response._outputStream.Write(buffer, 0, buffer.Length);
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    _response = null;
                    _builder = null;
                }
                base.Dispose(disposing);
            }
        }
        #endregion
    }

}