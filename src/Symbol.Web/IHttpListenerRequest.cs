/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    
    /// <summary>
    /// 描述传入 HttpListener 对象的 HTTP 请求。
    /// </summary>
    public interface IHttpListenerRequest : System.IDisposable {
        /// <summary>
        /// 获取请求客户端使用的 HTTP 版本。
        /// </summary>
        /// <returns>System.Version 用于标识 HTTP 的客户端版本。</returns>
        System.Version ProtocolVersion { get; }
        /// <summary>
        /// 获取客户端使用的 HTTP 数据传输方法（如 GET、POST 或 HEAD），始终为大写字母。
        /// </summary>
        /// <returns>客户端使用的 HTTP 数据传输方法。</returns>
        string HttpMethod { get; }
        /// <summary>
        /// 获取客户端支持的 MIME 接受类型的字符串数组。["*/*","image/png"]
        /// </summary>
        /// <returns>客户端支持的 MIME 接受类型的字符串数组。</returns>
        string[] AcceptTypes { get; }
        /// <summary>
        /// 获取当前请求所采用的字符集，如果客户端未指定，默认是UTF8。
        /// </summary>
        /// <returns>表示客户端的字符集的 System.Text.Encoding 对象。</returns>
        System.Text.Encoding ContentEncoding { get; }
        /// <summary>
        /// 获取客户端发送的内容长度（以字节计）。
        /// </summary>
        /// <returns>客户端发送的内容的长度（以字节为单位）。</returns>
        long ContentLength { get; }
        /// <summary>
        /// 获取传入请求的 MIME 内容类型，默认GET没有此值，POST时，普通的表单为：application/x-www-form-urlencoded，带文件上传的为：multipart/form-data; boundary=--------xxxxxx，一些特殊的网络请求，可能会传成 text/xml。
        /// </summary>
        /// <returns>表示传入请求的 MIME 内容类型的字符串，例如，“text/html”。其他常见 MIME 类型包括“audio.wav”、“image/gif”和“application/pdf”。</returns>
        string ContentType { get; }
        /// <summary>
        /// 获取一个 System.Boolean 值，该值指示请求是否有关联的正文数据。
        /// </summary>
        /// <returns>如果请求有关联的正文数据，则为 true；否则为 false。</returns>
        bool HasEntityBody { get; }
        /// <summary>
        /// 获取客户端请求的 URL 信息（不包括主机和端口）。
        /// </summary>
        /// <returns>System.String 包含此请求的原始 URL。</returns>
        string RawUrl { get; }
        /// <summary>
        /// 获取有关当前请求的 URL 的信息。
        /// </summary>
        /// <returns>包含有关当前请求的 URL 的信息的 System.Uri 对象。</returns>
        System.Uri Url { get; }
        /// <summary>
        /// 获取有关客户端上次请求的 URL 的信息，该请求链接到当前的 URL。错误的数据将直接是null。
        /// </summary>
        /// <returns>一个 System.Uri 对象。</returns>
        System.Uri UrlReferrer { get; }
        /// <summary>
        /// 获取客户端浏览器的原始用户代理信息。
        /// </summary>
        /// <returns>客户端浏览器的原始用户代理信息。</returns>
        string UserAgent { get; }
        /// <summary>
        /// 获取远程客户端的 IP 主机地址。
        /// </summary>
        /// <returns>远程客户端的 IP 地址。</returns>
        string UserHostAddress { get; }
        /// <summary>
        /// 获取远程客户端的 DNS 名称（目前直接返回的是客户端的IP）。
        /// </summary>
        /// <returns>远程客户端的 DNS 名称。</returns>
        string UserHostName { get; }
        /// <summary>
        /// 获取客户端语言首选项的字符串数组（暂时未排序，按照客户端提交的顺序；常见的是 zh-CN）。
        /// </summary>
        string[] UserLanguages { get; }
        /// <summary>
        /// 获取 System.Boolean 值，该值指示该请求是否来自本地计算机。
        /// </summary>
        /// <returns>如果发出请求的计算机就是提供该请求的 HttpListener 对象所在的计算机，则为 true；否则为 false。</returns>
        bool IsLocal { get; }
        /// <summary>
        /// 获取一个 System.Boolean 值，该值指示用来发送请求的 TCP 连接是否使用安全套接字层 (SSL) 协议。
        /// </summary>
        /// <returns>如果 TCP 连接使用的是 SSL，则为 true；否则为 false。</returns>
        bool IsSecureConnection { get; }
        /// <summary>
        /// 获取一个 System.Boolean 值，该值指示客户端是否请求持续型连接。
        /// </summary>
        /// <returns>如果连接应保持打开状态，则为 true；否则为 false。</returns>
        bool KeepAlive { get; }
        /// <summary>
        /// 获取请求被定向到的服务器 IP 地址和端口号。
        /// </summary>
        /// <returns>System.Net.IPEndPoint 表示请求被发送到的 IP 地址。</returns>
        System.Net.IPEndPoint LocalEndPoint { get; }
        /// <summary>
        /// 获取发出请求的客户端 IP 地址和端口号。
        /// </summary>
        /// <returns>System.Net.IPEndPoint 表示发出请求的 IP 地址和端口号。</returns>
        System.Net.IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 获取 HTTP 头集合。
        /// </summary>
        System.Collections.Specialized.NameValueCollection Headers { get; }
        /// <summary>
        /// 获取客户端发送的 Cookie 的集合（改进了传统的ASP.NET用法，实际上在Request时，也只能得到名称与值，其它的属性是没有的）。
        /// </summary>
        System.Collections.Specialized.NameValueCollection Cookies { get; }
        /// <summary>
        /// 获取 HTTP 查询字符串变量集合（网址上 ? 后面的数据。）。
        /// </summary>
        System.Collections.Specialized.NameValueCollection QueryString { get; }
        /// <summary>
        /// 获取窗体变量集合，需要注意的是，仅当POST时才会有真正有数据，其它情况其实是空白的集合；另外POST中的数据不是正常可以识别的时，它也是空白的集合，不会报错。
        /// </summary>
        /// <returns>表示窗体变量集合的 System.Collections.Specialized.NameValueCollection。</returns>
        System.Collections.Specialized.NameValueCollection Form { get; }
        /// <summary>
        /// 获取 Web 服务器变量的集合（目前它是一个空集合，没有任何参数，并且不调用它，不会创建它的。）。
        /// </summary>
        System.Collections.Specialized.NameValueCollection ServerVariables { get; }
        /// <summary>
        /// 获取一个组合参数的集合，它组成的顺序为：QueryString、Form、Cookies、ServerVariables，需要注意，不调用时它不会去组合，不占资源的。
        /// </summary>
        System.Collections.Specialized.NameValueCollection Params { get; }
        /// <summary>
        /// 获取采用多部分 MIME 格式的由客户端上载的文件的集合（仅当POST文件时才有，并且不调用它 或 Form 或 this[key]，是不会解析的，即不占用资源）。
        /// </summary>
        IHttpFileCollection Files { get; }
        /// <summary>
        /// 获取组合值，注意它不是调用的Params，寻找顺序：QueryString、Form（仅POST时）、Cookies、ServerVariables。
        /// </summary>
        /// <param name="key">要获取的集合成员的名称。</param>
        /// <returns>如果未找到指定的 key，则返回 null。</returns>
        string this[string key] { get; }


        /// <summary>
        /// 获取包含正文数据的流，这些数据由客户端发送。
        /// </summary>
        /// <returns>一个可读的 System.IO.Stream 对象，该对象包含客户端在请求正文中发送的字节。如果没有随请求发送任何数据，则此属性返回 System.IO.Stream.Null。</returns>
        System.IO.Stream InputStream { get; }




        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取（读取所有可用数据，并且不还原流位置）。
        /// </summary>
        /// <returns>字节数组。</returns>
        byte[] BinaryRead();
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取（读取所有可用数据）。
        /// </summary>
        /// <returns>字节数组。</returns>
        byte[] BinaryRead(bool resetPosition);
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取（不还原流位置）。
        /// </summary>
        /// <param name="count">要读取的字节数，超出有效范围时仅读取有效的部分。</param>
        /// <returns>字节数组。</returns>
        byte[] BinaryRead(int count);
        /// <summary>
        /// 执行对当前输入流进行指定字节数的二进制读取。
        /// </summary>
        /// <param name="count">要读取的字节数，超出有效范围时仅读取有效的部分。</param>
        /// <param name="resetPosition">是否还原读取之前的流位置。</param>
        /// <returns>字节数组。</returns>
        byte[] BinaryRead(int count, bool resetPosition);
    }
}