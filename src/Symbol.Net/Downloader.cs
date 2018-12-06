/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Net;

namespace Symbol.Net {
    /// <summary>
    /// 下载器，基于HttpWebClient
    /// </summary>
    /// <remarks>本类中的所有提到的默认值，将指HttpWebClient的默认设置。注意每次请求，都将创建一个新的HttpWebClient。</remarks>
    public class Downloader : IDisposable {

        #region properties
        /// <summary>
        /// 编码格式，如gzip，如果设置为null将采用默认值。
        /// </summary>
        public DecompressionMethods? AutomaticDecompression { get; set; }
        /// <summary>
        /// 允许自动重定向，有时为了得到重定向地址上面的参数，会不需要自动重定向。如果设置为null将采用默认值。
        /// </summary>
        public bool? AllowAutoRedirect { get; set; }
        /// <summary>
        /// 发送100标头，有的网站限制不能用100标头，所以默认都是关闭的。
        /// </summary>
        public bool? Expect100Continue { get; set; }
        /// <summary>
        /// 自动追加 PostHeader：application/x-www-form-urlencoded。如果设置为null将采用默认值。
        /// </summary>
        public bool? AutoAppendPostHeader { get; set; }

        /// <summary>
        /// 重试次数，有时网络有一些小故障多试一两次会发现可以正常访问。默认为5次。
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// 允许抛出异常，默认为false
        /// </summary>
        public bool AllowThrow { get; set; }
        /// <summary>
        /// 超时时间，设置为0时，将采用默认值。
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 代理服务器
        /// </summary>
        public System.Net.IWebProxy Proxy { get; set; }
        /// <summary>
        /// 编码，当下载的文本内容无法确定编码时，将采用这个编码。设置为null将采用默认编码。
        /// </summary>
        public System.Text.Encoding Encoding { get; set; }
        /// <summary>
        /// 共享Cookies
        /// </summary>
        public System.Net.CookieContainer Cookies { get; set; }
        /// <summary>
        /// 下次请求时将采用的Headers
        /// </summary>
        public System.Net.WebHeaderCollection Headers { get; private set; }
        /// <summary>
        /// 获取或设置基础网址。
        /// </summary>
        public string BaseAddress { get; set; }
        /// <summary>
        /// 获取上次网络操作返回的Http头集合。
        /// </summary>
        public System.Net.WebHeaderCollection ResponseHeaders { get; private set; }

        #endregion

        #region methods

        #region ctor
        /// <summary>
        /// 创建 Downloader 的实例。
        /// </summary>
        public Downloader() {
            RetryCount = 5;
            AllowThrow = false;
            Headers = new WebHeaderCollection();
            Cookies = new CookieContainer();
        }
        #endregion

        #region CreateWebClient
        /// <summary>
        /// 创建HttpWebClient的实例。
        /// </summary>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回HttpWebClient的实例。</returns>
        protected HttpWebClient CreateWebClient(Uri refererUrl) {
            HttpWebClient result = new HttpWebClient();
            //result.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; CIBA; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            //result.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";
            //result.Headers["Accept"] = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            //result.Headers["Accept-Language"] = "zh-cn";
            result.BaseAddress = BaseAddress;
            if (Headers != null) {
                foreach (string item in Headers.AllKeys) {
                    result.Headers[item] = Headers[item];
                }
            }
            if (refererUrl != null) {
                result.Headers["Referer"] = refererUrl.AbsoluteUri;
            }
            if (AutomaticDecompression != null)
                result.AutomaticDecompression = AutomaticDecompression.Value;
            if (AllowAutoRedirect != null)
                result.AllowAutoRedirect = AllowAutoRedirect.Value;
            if (Expect100Continue != null)
                result.Expect100Continue = Expect100Continue.Value;
            if (AutoAppendPostHeader != null)
                result.AutoAppendPostHeader = AutoAppendPostHeader.Value;

            if (Timeout > 0)
                result.Timeout = Timeout;
            result.Cookies = Cookies;
            if (Proxy != null)
                result.Proxy = Proxy;
            if (Encoding != null)
                result.Encoding = Encoding;
            return result;
        }
        #endregion


        #region DownloadString
        /// <summary>
        /// 下载文本（网页）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <returns>返回获取到的文本内容。</returns>
        public string DownloadString(string address) {
            return DownloadString(address, (Uri)null);
        }
        /// <summary>
        /// 下载文本（网页）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的文本内容。</returns>
        public string DownloadString(string address, string refererUrl) {
            return DownloadString(new Uri(address, UriKind.RelativeOrAbsolute), string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 下载文本（网页）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的文本内容。</returns>
        public string DownloadString(string address, Uri refererUrl) {
            return DownloadString(new Uri(address, UriKind.RelativeOrAbsolute), refererUrl);
        }
        /// <summary>
        /// 下载文本（网页）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <returns>返回获取到的文本内容。</returns>
        public string DownloadString(Uri address) {
            return DownloadString(address, (Uri)null);
        }
        /// <summary>
        /// 下载文本（网页）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的文本内容。</returns>
        public string DownloadString(Uri address, string refererUrl) {
            return DownloadString(address, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 下载文本（网页）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的文本内容。</returns>
        public string DownloadString(Uri address, Uri refererUrl) {
            lock (this) {
                using (HttpWebClient webclient = CreateWebClient(refererUrl)) {
                    string result = DownloadStringInternal(address, webclient, 0, null);
                    ResponseHeaders = webclient.ResponseHeaders;
                    return result;
                }
            }
        }
        private string DownloadStringInternal(Uri address, HttpWebClient webclient, int count, Exception e) {
            if (count > RetryCount) {
                if (e != null)
                    throw e;
                return string.Empty;
            }

            try {
                return webclient.DownloadString(address);
            } catch (WebException ex) {
                return DownloadStringInternal(address, webclient, count + 1, ex);
            }
        }
        #endregion
        #region DownloadData
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="address">网址。</param>
        /// <returns>返回获取到的数据。</returns>
        public byte[] DownloadData(string address) {
            return DownloadData(address, (Uri)null);
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的数据。</returns>
        public byte[] DownloadData(string address, string refererUrl) {
            return DownloadData(new Uri(address, UriKind.RelativeOrAbsolute), string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的数据。</returns>
        public byte[] DownloadData(string address, Uri refererUrl) {
            return DownloadData(new Uri(address, UriKind.RelativeOrAbsolute), refererUrl);
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="address">网址。</param>
        /// <returns>返回获取到的数据。</returns>
        public byte[] DownloadData(Uri address) {
            return DownloadData(address, (Uri)null);
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的数据。</returns>
        public byte[] DownloadData(Uri address, string refererUrl) {
            return DownloadData(address, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回获取到的数据。</returns>
        public byte[] DownloadData(Uri address, Uri refererUrl) {
            lock (this) {
                using (HttpWebClient webclient = CreateWebClient(refererUrl)) {
                    byte[] result = DownloadDataInternal(address, webclient, 0, null);
                    ResponseHeaders = webclient.ResponseHeaders;
                    return result;
                }
            }
        }
        private byte[] DownloadDataInternal(Uri address, HttpWebClient webclient, int count, Exception e) {
            if (count > RetryCount) {
                if (e != null)
                    throw e;
                return new byte[0];
            }

            try {
                return webclient.DownloadData(address);
            } catch (WebException ex) {
                return DownloadDataInternal(address, webclient, count + 1, ex);
            }
        }
        #endregion
        #region DownloadFile
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        public void DownloadFile(string address, string fileName) {
            DownloadFile(address, fileName, (Uri)null);
        }
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        public void DownloadFile(string address, string fileName, string refererUrl) {
            DownloadFile(new Uri(address), fileName, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        public void DownloadFile(string address, string fileName, Uri refererUrl) {
            DownloadFile(new Uri(address), fileName, refererUrl);
        }
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        public void DownloadFile(Uri address, string fileName) {
            DownloadFile(address, fileName, (Uri)null);
        }
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        public void DownloadFile(Uri address, string fileName, string refererUrl) {
            DownloadFile(address, fileName, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地保存文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        public void DownloadFile(Uri address, string fileName, Uri refererUrl) {
            lock (this) {
                using (HttpWebClient webclient = CreateWebClient(refererUrl)) {
                    DownloadFileInternal(address, fileName, webclient, 0, null);
                    ResponseHeaders = webclient.ResponseHeaders;
                }
            }
        }
        private void DownloadFileInternal(Uri address, string fileName, HttpWebClient webclient, int count, Exception e) {
            if (count > RetryCount) {
                if (e != null)
                    throw e;
                return;
            }

            try {
                webclient.DownloadFile(address, fileName);
            } catch (WebException ex) {
                DownloadFileInternal(address, fileName, webclient, count + 1, ex);
            }
        }
        #endregion

        #region UploadString
        /// <summary>
        /// 上传数据（Post文本，普通表单）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadString(string address, string data) {
            return UploadString(new Uri(address, UriKind.RelativeOrAbsolute), data);
        }
        /// <summary>
        /// 上传数据（Post文本，普通表单）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadString(string address, string data, string refererUrl) {
            return UploadString(new Uri(address, UriKind.RelativeOrAbsolute), data, refererUrl);
        }
        /// <summary>
        /// 上传数据（Post文本，普通表单）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadString(string address, string data, Uri refererUrl) {
            return UploadString(new Uri(address, UriKind.RelativeOrAbsolute), data, refererUrl);
        }
        /// <summary>
        /// 上传数据（Post文本，普通表单）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadString(Uri address, string data) {
            return UploadString(address, data, (Uri)null);
        }
        /// <summary>
        /// 上传数据（Post文本，普通表单）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadString(Uri address, string data, string refererUrl) {
            return UploadString(address, data, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 上传数据（Post文本，普通表单）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadString(Uri address, string data, Uri refererUrl) {
            lock (this) {
                using (HttpWebClient webclient = CreateWebClient(refererUrl)) {
                    string result = UploadStringInternal(address, data, webclient, 0, null);
                    ResponseHeaders = webclient.ResponseHeaders;
                    return result;
                }
            }
        }
        private string UploadStringInternal(Uri address, string data, HttpWebClient webclient, int count, Exception e) {
            if (count > RetryCount) {
                if (e != null)
                    throw e;
                return string.Empty;
            }

            try {
                return webclient.UploadString(address, data);
            } catch (WebException ex) {
                return UploadStringInternal(address, data, webclient, count + 1, ex);
            }
        }
        #endregion
        #region UploadValues
        Uri ToUri(object address) {
            Uri uri = address as Uri;
            if (uri != null)
                return uri;
            string url = address as string;
            if (url == null)
                return null;
            return new Uri(url, UriKind.RelativeOrAbsolute);
        }
        /// <summary>
        /// 数据拼接（value自动转码，默认为utf-8）
        /// </summary>
        /// <param name="values">实体/匿名类/key-value</param>
        /// <param name="valueSpliter">key/value之间的字符</param>
        /// <param name="pairSpliter">多个pair之间的字符</param>
        /// <param name="ignoreEmptyValue">是否忽略空或空字符串</param>
        /// <returns>返回文本</returns>
        public string ToData(object values,string valueSpliter="=",string pairSpliter="&", bool ignoreEmptyValue=true) {

            var values2 = Symbol.Collections.Generic.NameValueCollection<object>.As(values);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            bool first = true;
            foreach (var item in values2) {
                string key = item.Key;
                if (string.IsNullOrEmpty(key))
                    continue;
                string value=null;
                if (item.Value != null) {
                    if (item.Value.GetType() == typeof(string)) {
                        value = (string)item.Value;
                    } else {
                        value = Symbol.Serialization.Json.ToString(item.Value);
                    }
                }
                if (ignoreEmptyValue && string.IsNullOrEmpty(value))
                    continue;
                if (first) {
                    first = false;
                } else {
                    builder.Append(pairSpliter);
                }
                builder.Append(key).Append(valueSpliter).Append(HttpUtility.UrlEncode(value, Encoding == null ? System.Text.Encoding.UTF8 : Encoding));
            }
            return builder.ToString();
        }
        /// <summary>
        /// 上传数据（Post文本，key/value）。
        /// </summary>
        /// <param name="address">网址，自动识别System.Uri和System.String。</param>
        /// <param name="values">要上传的数据，匿名对象/key-value/实体；header Content-Type 为 application/json 时，自动序列化为JSON文本。</param>
        /// <param name="refererUrl">来路（引用页面），自动识别System.Uri和System.String。</param>
        /// <returns>返回上传后获取到的内容。</returns>
        public string UploadValues(object address, object values, object refererUrl=null) {
            if (string.Equals(Headers["Content-Type"], "application/json", StringComparison.OrdinalIgnoreCase)) {
                return UploadString(ToUri(address), Symbol.Serialization.Json.ToString(values), ToUri(refererUrl));
            } else {
                return UploadString(ToUri(address), ToData(values), ToUri(refererUrl));
            }
        }

        #endregion
        #region UploadFile
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地文件位置。</param>
        /// <returns>返回上传之后获取到的数据。</returns>
        public byte[] UploadFile(string address, string fileName) {
            return UploadFile(address, fileName, (Uri)null);
        }
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地文件位置。</param>
        /// <returns>返回上传之后获取到的数据。</returns>
        public byte[] UploadFile(Uri address, string fileName) {
            return UploadFile(address, fileName, (Uri)null);
        }
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传之后获取到的数据。</returns>
        public byte[] UploadFile(string address, string fileName, string refererUrl) {
            return UploadFile(new Uri(address, UriKind.RelativeOrAbsolute), fileName, refererUrl);
        }
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传之后获取到的数据。</returns>
        public byte[] UploadFile(string address, string fileName, Uri refererUrl) {
            return UploadFile(new Uri(address, UriKind.RelativeOrAbsolute), fileName, refererUrl);
        }
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传之后获取到的数据。</returns>
        public byte[] UploadFile(Uri address, string fileName, string refererUrl) {
            return UploadFile(address, fileName, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="fileName">本地文件位置。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传之后获取到的数据。</returns>
        public byte[] UploadFile(Uri address, string fileName, Uri refererUrl) {
            lock (this) {
                using (HttpWebClient webclient = CreateWebClient(refererUrl)) {
                    byte[] result = UploadFileInternal(address, fileName, webclient, 0, null);
                    ResponseHeaders = webclient.ResponseHeaders;
                    return result;
                }
            }
        }
        private byte[] UploadFileInternal(Uri address, string fileName, HttpWebClient webclient, int count, Exception e) {
            if (count > RetryCount) {
                if (e != null)
                    throw e;
                return new byte[0];
            }

            try {
                return webclient.UploadFile(address, fileName);
            } catch (WebException ex) {
                return UploadFileInternal(address, fileName, webclient, count + 1, ex);
            }
        }

        #endregion
        #region UploadData
        /// <summary>
        /// 上传数据（自定义上传方式）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <returns>返回上传后获取到的数据。</returns>
        public byte[] UploadData(string address, byte[] data) {
            return UploadData(address, data, (Uri)null);
        }
        /// <summary>
        /// 上传数据（自定义上传方式）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <returns>返回上传后获取到的数据。</returns>
        public byte[] UploadData(Uri address, byte[] data) {
            return UploadData(address, data, (Uri)null);
        }
        /// <summary>
        /// 上传数据（自定义上传方式）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的数据。</returns>
        public byte[] UploadData(string address, byte[] data, string refererUrl) {
            return UploadData(new Uri(address, UriKind.RelativeOrAbsolute), data, refererUrl);
        }
        /// <summary>
        /// 上传数据（自定义上传方式）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的数据。</returns>
        public byte[] UploadData(string address, byte[] data, Uri refererUrl) {
            return UploadData(new Uri(address, UriKind.RelativeOrAbsolute), data, refererUrl);
        }
        /// <summary>
        /// 上传数据（自定义上传方式）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的数据。</returns>
        public byte[] UploadData(Uri address, byte[] data, string refererUrl) {
            return UploadData(address, data, string.IsNullOrEmpty(refererUrl) ? null : new Uri(refererUrl));
        }
        /// <summary>
        /// 上传数据（自定义上传方式）。
        /// </summary>
        /// <param name="address">网址。</param>
        /// <param name="data">要上传的数据。</param>
        /// <param name="refererUrl">来路（引用页面）。</param>
        /// <returns>返回上传后获取到的数据。</returns>
        public byte[] UploadData(Uri address, byte[] data, Uri refererUrl) {
            lock (this) {
                using (HttpWebClient webclient = CreateWebClient(refererUrl)) {
                    byte[] result = UploadDataInternal(address, data, webclient, 0, null);
                    ResponseHeaders = webclient.ResponseHeaders;
                    return result;
                }
            }
        }
        private byte[] UploadDataInternal(Uri address, byte[] data, HttpWebClient webclient, int count, Exception e) {
            if (count > RetryCount) {
                if (e != null)
                    throw e;
                return new byte[0];
            }

            try {
                return webclient.UploadData(address, data);
            } catch (WebException ex) {
                return UploadDataInternal(address, data, webclient, count + 1, ex);
            }
        }

        #endregion

        
        #region IDisposable 成员
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        /// <param name="disposing">是否为终结。</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Proxy = null;
                Headers = null;
                Cookies = null;
                Encoding = null;
            }
        }

        #endregion

        #endregion
    }
}