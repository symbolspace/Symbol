/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Net;

namespace Symbol.Net {
    /// <summary>
    /// Http操作类。
    /// </summary>
    public class HttpWebClient : System.Net.WebClient {


        #region properties
        /// <summary>
        /// 获取或设置 允许自动重定向（检测302标志）。
        /// </summary>
        public bool AllowAutoRedirect { get; set; }
        /// <summary>
        /// 获取或设置 发送100侦测头。
        /// </summary>
        public bool Expect100Continue { get; set; }
        /// <summary>
        /// 获取或设置 文本请求时的Accept头。
        /// </summary>
        public string StringAccept { get; set; }
        /// <summary>
        /// 获取或设置 数据请求时的Accept头。
        /// </summary>
        public string DataAccept { get; set; }
        /// <summary>
        /// 获取或设置 自动解码类型(gzip和deflate)。
        /// </summary>
        public System.Net.DecompressionMethods AutomaticDecompression { get; set; }
        /// <summary>
        /// 获取或设置 Cookies。
        /// </summary>
        public System.Net.CookieContainer Cookies { get; set; }
        /// <summary>
        /// 获取或设置 超时时间（秒）。
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 获取或设置 自动追加 PostHeader：application/x-www-form-urlencoded
        /// </summary>
        public bool AutoAppendPostHeader { get; set; }
        /// <summary>
        /// 获取或设置最大并发连接数（最大值建议不要超过1024，目前已设置为512）。
        /// </summary>
        public static int DefaultConnectionLimit {
            get { return ServicePointManager.DefaultConnectionLimit; }
            set { ServicePointManager.DefaultConnectionLimit = value; }
        }
        #endregion

        #region cctor
        static HttpWebClient() {
            SetUseUnsafeHeaderParsing();
            try { DefaultConnectionLimit = 512; } catch { }
            try { ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult); } catch { }
            try { ServicePointManager.SecurityProtocol =
                          (SecurityProtocolType)48      //Ssl3 
                        | (SecurityProtocolType)192     //Tls
                        | (SecurityProtocolType)768     //Tls11
                        | (SecurityProtocolType)3072    //Tls12
                        ;
            } catch { }
        }
        static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors) {
            return true;
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建 HttpWebClient 的实例。
        /// </summary>
#if NET6_0 || NET7_0 || NET8_0
#pragma warning disable SYSLIB0014 // 类型或成员已过时
#endif
        public HttpWebClient() {
            StringAccept = "*/*";
            DataAccept = "*/*";
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            Expect100Continue = false;
            AllowAutoRedirect = true;
            Cookies = new CookieContainer();
            Timeout = 100;
            Proxy = null;// GlobalProxySelection.GetEmptyWebProxy();
            Headers = new WebHeaderCollection();
            UseDefaultCredentials = false;
            Headers["Accept"] = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*";
            Headers["Accept-Language"] = "zh-cn";
            Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";

            AutoAppendPostHeader = true;
            Credentials = CredentialCache.DefaultCredentials;
            //Headers.Add( HttpRequestHeader.KeepAlive,"TRUE");
        }
#if NET6_0 || NET7_0 || NET8_0
#pragma warning restore SYSLIB0014 // 类型或成员已过时
#endif
        #endregion

        #region methods

        #region SetUseUnsafeHeaderParsing
        private static void SetUseUnsafeHeaderParsing() {
            System.Type type = FastWrapper.GetWarpperType("System.Net.Configuration.SettingsSectionInternal,System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            if (type == null)
                return;
            object instance = FastWrapper.Get(type, "Section");
            if (instance == null)
                return;
            FastWrapper.Set(instance, "useUnsafeHeaderParsing", true);
        }
        #endregion

        #region GetWebRequest
        /// <summary>
        /// 为指定资源返回一个 System.Net.WebRequest 对象。
        /// </summary>
        /// <param name="address">一个 System.Uri，用于标识要请求的资源。</param>
        /// <returns>一个新的 System.Net.WebRequest 对象，用于指定的资源。</returns>
        protected override WebRequest GetWebRequest(System.Uri address) {
            string ifModifiedSince = null;
            try {
                ifModifiedSince = base.Headers[System.Net.HttpRequestHeader.IfModifiedSince];
                base.Headers.Remove(System.Net.HttpRequestHeader.IfModifiedSince);
            } catch {
            }
            //string userAgent = Headers["User-Agent"];
            //Headers.Remove("User-Agent");
            //string referer = Headers["Referer"];
            //Headers.Remove("Referer");
            WebRequest request = base.GetWebRequest(address);
            request.Proxy = Proxy;
            request.Timeout = this.Timeout * 1000;
            //Headers["User-Agent"] = userAgent;
            //Headers["Referer"] = referer;
            if (request is HttpWebRequest) {
                HttpWebRequest request2 = (HttpWebRequest)request;
                
                //if (!string.IsNullOrEmpty(userAgent)) {
                //    request2.UserAgent = userAgent;
                //}
                //if (!string.IsNullOrEmpty(referer)) {
                //    request2.Referer = referer;
                //}
                //request2.Accept = "";
                //request2.ContentType
                if (!string.IsNullOrEmpty(ifModifiedSince)) {
                    request2.IfModifiedSince = DateTime.ParseExact(
                                                                    ifModifiedSince,
                                                                    @"ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                                                    System.Globalization.CultureInfo.GetCultureInfo("en-US")
                                                                   );
                }
                request2.AutomaticDecompression = this.AutomaticDecompression;
                request2.ServicePoint.Expect100Continue = this.Expect100Continue;
                request2.AllowAutoRedirect = this.AllowAutoRedirect;
                request2.CookieContainer = this.Cookies;
                //request2.KeepAlive = true;
            }
            if (AutoAppendPostHeader) {
                if (string.Equals(request.Method, "POST", StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(request.ContentType))
                    request.ContentType = "application/x-www-form-urlencoded";
            }

            return request;
        }
        #endregion

        #region DownloadString
        /// <summary>
        /// 下载文本（网页）
        /// </summary>
        /// <param name="address">网址。</param>
        /// <returns>返回下载到的内容。</returns>
        public new string DownloadString(string address) {
            Headers["Accept"] = StringAccept;
            return BufferToString(base.DownloadData(address));
        }
        /// <summary>
        /// 下载文本（网页）
        /// </summary>
        /// <param name="address">网址。</param>
        /// <returns>返回下载到的内容。</returns>
        public new string DownloadString(Uri address) {
            Headers["Accept"] = StringAccept;
            return BufferToString(base.DownloadData(address));
        }
        #endregion

        #region DownloadStringAsync
        /// <summary>
        /// 下载文本（网页，异步）
        /// </summary>
        /// <param name="address">网址。</param>
        public void DownloadStringAsync(string address) {
            this.DownloadStringAsync(new Uri(address));
        }
        /// <summary>
        /// 下载文本（网页，异步）
        /// </summary>
        /// <param name="address">网址。</param>
        public new void DownloadStringAsync(Uri address) {
            DownloadStringAsync(address, null);
        }
        /// <summary>
        /// 下载文本（网页，异步）
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="userToken">额外的数据，方便完成时调用。</param>
        public void DownloadStringAsync(string address, object userToken) {
            this.DownloadStringAsync(new Uri(address), userToken);
        }
        /// <summary>
        /// 下载文本（网页，异步）
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="userToken">额外的数据，方便完成时调用。</param>
        public new void DownloadStringAsync(Uri address, object userToken) {
            Headers["Accept"] = StringAccept;
            base.DownloadDataAsync(address, new DownloadStringUserToken(userToken));
        }
        #endregion
        #region DownloadDataAsync
        /// <summary>
        /// 下载数据（异步）
        /// </summary>
        /// <param name="address">网址。</param>
        public void DownloadDataAsync(string address) {
            Headers["Accept"] = DataAccept;
            base.DownloadDataAsync(new Uri(address));
        }
        /// <summary>
        /// 下载数据（异步）
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="userToken">额外的数据，方便完成时调用。</param>
        public void DownloadDataAsync(string address, object userToken) {
            Headers["Accept"] = DataAccept;
            base.DownloadDataAsync(new Uri(address), userToken);

        }

        #endregion

        #region OnDownloadDataCompleted
        /// <summary>
        /// 引发 System.Net.WebClient.DownloadDataCompleted 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Net.DownloadDataCompletedEventArgs 对象。</param>
        protected override void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e) {
            if (!(e.UserState is DownloadStringUserToken)) {
                base.OnDownloadDataCompleted(e);
                return;
            }
            string result = null;
            if (!e.Cancelled && e.Error == null && e.Result != null) {
                try {
                    result = BufferToString(e.Result);
                } catch {
                }
            }
            DownloadStringCompletedEventArgs e2 =
                new DownloadStringCompletedEventArgs(
                                                        result,
                                                        e.Error,
                                                        e.Cancelled,
                                                        ((DownloadStringUserToken)e.UserState).UserToken
                                                     );
            OnDownloadStringCompleted(e2);
        }
        #endregion

        #region DownloadFile
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        public new void DownloadFile(string address, string fileName) {
            DownloadFile(new Uri(address), fileName);
        }
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        public new void DownloadFile(Uri address, string fileName) {
            Headers["Accept"] = DataAccept;
            //if (this.CacheFiles.Contains(address.AbsoluteUri))
            //    return;
            //try
            //{
            base.DownloadFile(address, fileName);

            //if (string.IsNullOrEmpty(address.Query))
            //    this.CacheFiles.Add(address.AbsoluteUri);
            //}
            //catch
            //{
            //}
        }
        #endregion

        #region GetEncoding
        /// <summary>
        /// 获取数据中包含的编码。
        /// </summary>
        /// <param name="buffer">需要转换的数据。</param>
        /// <returns>返回获取的编码，未找到将返回Encoding属性。</returns>
        private System.Text.Encoding GetEncoding(byte[] buffer) {
            string encoding = null;
            System.Text.Encoding result = null;
            string contentType = this.ResponseHeaders == null ? null : this.ResponseHeaders["Content-Type"];
            if (contentType != null) {
                int charsetIndex = contentType.IndexOf("charset=", StringComparison.CurrentCultureIgnoreCase);
                if (charsetIndex != -1)
                    encoding = contentType.Substring(charsetIndex + 8);
            }
            if (!string.IsNullOrEmpty(encoding)) {
                if (string.Equals(encoding, "utf8", StringComparison.OrdinalIgnoreCase))
                    encoding = "utf-8";
                return System.Text.Encoding.GetEncoding(encoding);
            }
            if (result == null) {
                result = GetBufferEncoding(buffer);
            }

            if (result == null)
                return this.Encoding;
            else
                return result;
        }
        #endregion
        #region GetBufferEncoding
        /// <summary>
        /// 获取数据中包含的编码。
        /// </summary>
        /// <param name="buffer">需要转换的数据。</param>
        /// <returns>返回获取的编码，未找到将返回null。</returns>
        public static System.Text.Encoding GetBufferEncoding(byte[] buffer) {
            string encoding = null;
            try {
                using (var memoryStream = new System.IO.MemoryStream(buffer)) {
                    using (var streamReader = new System.IO.StreamReader(memoryStream, System.Text.Encoding.ASCII)) {
                        string pageCode = streamReader.ReadToEnd();
                        System.Text.RegularExpressions.Regex regexCharset = new System.Text.RegularExpressions.Regex("\\s*;\\s*charset=(?<charset>[\\w-]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (regexCharset.IsMatch(pageCode)) {
                            encoding = regexCharset.Match(pageCode).Groups["charset"].Value;
                        } else {
                            regexCharset = new System.Text.RegularExpressions.Regex("\\s*charset=['\"]*(?<charset>[\\w-]*)['\"]*\\s*", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (regexCharset.IsMatch(pageCode)) {
                                encoding = regexCharset.Match(pageCode).Groups["charset"].Value;
                            }
                        }
                    }
                }
            } catch {
            }
            if (string.IsNullOrEmpty(encoding))
                return null;
            if (string.Equals(encoding, "utf8", StringComparison.OrdinalIgnoreCase))
                encoding = "utf-8";
            return System.Text.Encoding.GetEncoding(encoding);
        }
        #endregion
        #region BufferToString
        /// <summary>
        /// 将数据转为文本（自动检测数据中包含的编码信息）。
        /// </summary>
        /// <param name="buffer">需要转换的数据。</param>
        /// <returns>返回转换后的文本。</returns>
        public string BufferToString(byte[] buffer) {
            System.Text.Encoding encoding = GetEncoding(buffer);
            return BufferToString(buffer, encoding);
        }
        /// <summary>
        /// 将数据转为文本。
        /// </summary>
        /// <param name="buffer">需要转换的数据。</param>
        /// <param name="encoding">指定数据的编码。</param>
        /// <returns>返回转换后的文本。</returns>
        public static string BufferToString(byte[] buffer, System.Text.Encoding encoding) {
            if (encoding == null)
                encoding = GetBufferEncoding(buffer);

            if (encoding == null)
#if NETDNX
                encoding = HttpUtility.GB2312??HttpUtility.GBK??System.Text.Encoding.UTF8;
#else
                encoding = System.Text.Encoding.Default;
#endif
                return encoding.GetString(buffer);
        }
        #endregion

        #endregion

        #region events
        /// <summary>
        /// 在异步资源下载操作完成时发生。
        /// </summary>
        public new event EventHandler<DownloadStringCompletedEventArgs> DownloadStringCompleted;
        /// <summary>
        /// 引发 System.Net.WebClient.DownloadStringCompleted 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Net.DownloadStringCompletedEventArgs 对象。</param>
        protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e) {
            if (this.DownloadStringCompleted != null) {
                this.DownloadStringCompleted(this, e);
            }
        }

        #endregion

        #region classes
        class DownloadStringUserToken {
            public object UserToken {
                get;
                private set;
            }

            public DownloadStringUserToken(object userToken) {
                UserToken = userToken;
            }
        }
        /// <summary>
        /// 下载文本完成事件消息类。
        /// </summary>
        public class DownloadStringCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
            private string m_Result;
            /// <summary>
            /// 返回内容。
            /// </summary>
            public string Result {
                get {
                    base.RaiseExceptionIfNecessary();
                    return this.m_Result;
                }
            }

            internal DownloadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken)
                : base(exception, cancelled, userToken) {
                this.m_Result = result;
            }
        }
        #endregion
    }
}