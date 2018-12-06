/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// Http请求时，对返回的行为进行包装处理。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpResponse {
        /// <summary>
        /// 当前请求上下文实例，可以获取到 Request、Response、Session 等对象。
        /// </summary>
        IHttpContext Context { get; }

        /// <summary>
        /// 获取一个值，通过该值指示客户端是否仍连接在服务器上。
        /// </summary>
        /// <returns>如果客户端当前仍在连接，则为 true；否则为 false。</returns>
        bool IsClientConnected { get; }
        /// <summary>
        /// 获取请求客户端使用的 HTTP 版本。
        /// </summary>
        /// <returns>System.Version 用于标识 HTTP 的客户端版本。</returns>
        System.Version ProtocolVersion { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否将 HTTP 内容发送到客户端（相当于短暂的静默，不输出任何内容，但还在缓冲区中）。
        /// </summary>
        /// <returns>如果取消输出，则为 true；否则为 false。</returns>
        bool SuppressContent { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否缓冲输出并在处理完整个响应之后发送它（为true后，仅手动调用Flush或Close和End时才会输出。）。
        /// </summary>
        bool Buffer { get; set; }

        /// <summary>
        /// 获取或设置Cache-Control HTTP 标头（注意不会像ASP.NET那样乱套，你设置什么它就是什么）。
        /// </summary>
        string CacheControl { get; set; }
        /// <summary>
        /// 获取或设置返回给客户端的输出的 HTTP 状态代码。
        /// </summary>
        /// <returns>表示返回到客户端的 HTTP 输出状态的整数。默认值为 200 (OK)。有关有效状态代码的列表，请参见 Http Status Codes（Http状态代码）。</returns>
        int StatusCode { get; set; }
        /// <summary>
        /// 获取或设置返回给客户端的输出的 HTTP 状态字符串。
        /// </summary>
        /// <returns>一个字符串，描述返回给客户端的 HTTP 输出的状态。默认值为“OK”。有关有效状态代码的列表，请参见 Http Status Codes（Http状态代码）。</returns>
        string StatusDescription { get; set; }
        /// <summary>
        /// 获取或设置输出流的 HTTP 字符集。
        /// </summary>
        /// <returns>System.Text.Encoding 对象，包含与当前响应的字符集有关的信息。</returns>
        System.Text.Encoding ContentEncoding { get; set; }
        /// <summary>
        /// 获取或设置输出流的 HTTP MIME 类型（请别写上charset字样，设置字符集，请用ContentEncoding属性。）。
        /// </summary>
        /// <returns>输出流的 HTTP MIME 类型。默认值为“text/html”。</returns>
        string ContentType { get; set; }
        /// <summary>
        /// 获取或设置响应中包括的正文数据的字节数。
        /// </summary>
        /// <returns>响应的 Content-Length 标头的值。</returns>
        long ContentLength { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示服务器是否请求持久性连接。
        /// </summary>
        /// <returns>如果服务器请求持久性连接，则为 true；否则为 false。默认为 true。</returns>
        bool KeepAlive { get; set; }
        /// <summary>
        /// 获取或设置 HTTP Location 标头的值。
        /// </summary>
        /// <returns>System.String，包含要在 Location 标头中发送给客户端的绝对 URL。</returns>
        string RedirectLocation { get; set; }

        /// <summary>
        /// 获取响应标头的集合（会包括全局和本站点设置在配置文件中的标头），不用再像ASP.NET那样麻烦了。
        /// </summary>
        System.Collections.Specialized.NameValueCollection Headers { get; }
        /// <summary>
        /// 获取需要输出到客户端的Cookie集合，不用再像ASP.NET那样AppendCookie和SetCookie了。
        /// </summary>
        IHttpCookieCollection Cookies { get; }

        /// <summary>
        /// 获取或设置输出 HTTP 响应流的文本输出。
        /// </summary>
        /// <returns>支持到客户端的自定义输出的 System.IO.TextWriter 对象。</returns>
        System.IO.TextWriter Output { get; set; }
        /// <summary>
        /// 获取输出 HTTP 内容主体的二进制输出。
        /// </summary>
        /// <returns>表示输出 HTTP 内容主体的原始内容的 IO System.IO.Stream。</returns>
        System.IO.Stream OutputStream { get; }

        /// <summary>
        /// 将一个二进制字符串写入 HTTP 输出流。
        /// </summary>
        /// <param name="buffer">要写入输出流的字节。</param>
        void BinaryWrite(byte[] buffer);
        /// <summary>
        /// 清除缓冲区流中的所有内容输出。
        /// </summary>
        void Clear();
        /// <summary>
        /// 清除缓冲区流中的所有内容输出。
        /// </summary>
        void ClearContent();
        /// <summary>
        /// 清除缓冲区流中的所有头。
        /// </summary>
        void ClearHeaders();
        /// <summary>
        /// 将指定的标头和值添加到此响应的 HTTP 标头。
        /// </summary>
        /// <param name="name">要设置的 HTTP 标头的名称。</param>
        /// <param name="value">name 标头的值。</param>
        void AddHeader(string name, string value);
        /// <summary>
        /// 向随此响应发送的指定 HTTP 标头追加值。
        /// </summary>
        /// <param name="name">要追加 value 的 HTTP 标头的名称。</param>
        /// <param name="value">要追加到 name 标头的值。</param>
        void AppendHeader(string name, string value);
        /// <summary>
        /// 将指定的 Cookie 添加到此响应的 Cookie 集合。
        /// </summary>
        /// <param name="name">cookie名称。</param>
        /// <param name="value">cookie值。</param>
        void AppendCookie(string name, string value);
        /// <summary>
        /// 将指定的 Cookie 添加到此响应的 Cookie 集合。
        /// </summary>
        /// <param name="cookie">cookie。</param>
        void AppendCookie(IHttpCookie cookie);

        /// <summary>
        /// 将一个字符写入 HTTP 响应输出流（基本不用）。
        /// </summary>
        /// <param name="value">要写入 HTTP 输出流的字符。</param>
        void Write(char value);
        /// <summary>
        /// 将 System.Object 写入 HTTP 响应流（很少用）。
        /// </summary>
        /// <param name="value">要写入 HTTP 输出流的 System.Object。</param>
        void Write(object value);
        /// <summary>
        /// 将一个字符串写入 HTTP 响应输出流（最常用就是它）。
        /// </summary>
        /// <param name="value">要写入 HTTP 输出流的字符串。</param>
        void Write(string value);
        /// <summary>
        /// 将一个字符串写入 HTTP 响应输出流（string.Format用过吗？）。
        /// </summary>
        /// <param name="format">要写入 HTTP 输出流的字符串（为空或空字符串，不会进行Format行为）。</param>
        /// <param name="args">参于格式化的参数。</param>
        void Write(string format, params object[] args);
        /// <summary>
        /// 将一个字符数组写入 HTTP 响应输出流（基本用不上的它）。
        /// </summary>
        /// <param name="buffer">要写入的字符数组。</param>
        /// <param name="index">字符数组中开始进行写入的位置。</param>
        /// <param name="count">从 index 开始写入的字符数。</param>
        void Write(char[] buffer, int index, int count);
        /// <summary>
        /// 将指定文件的内容作为文件块直接写入 HTTP 响应输出流。
        /// </summary>
        /// <param name="filename">要写入 HTTP 输出的文件名（必须是绝对路径。）。</param>
        void WriteFile(string filename);
        /// <summary>
        /// 输出一个json对象（不格式化）。
        /// </summary>
        /// <param name="value">可序列化为json的对象。</param>
        void Json(object value);
        /// <summary>
        /// 输出一个json对象。
        /// </summary>
        /// <param name="value">可序列化为json的对象。</param>
        /// <param name="formated">是否格式化json文本。</param>
        void Json(object value, bool formated = false);
        /// <summary>
        /// 输出一段javascript脚本。
        /// </summary>
        /// <param name="script">脚本内容，为空或空文本时，自动输出为"//"。</param>
        void Javascript(string script);
        /// <summary>
        /// 输出一个javascript脚本（var name=json(value);）。
        /// </summary>
        /// <param name="name">变量名称，不能为空。</param>
        /// <param name="value">可序列化为json的对象。</param>
        void Javascript(string name, object value);

        /// <summary>
        /// 将请求重定向到新 URL 并指定该新 URL。
        /// </summary>
        /// <param name="url">目标位置。</param>
        void Redirect(string url);
        /// <summary>
        /// 重定向到一个Url
        /// </summary>
        /// <param name="url">可以是相对路径</param>
        /// <param name="message">跳转前要弹出的消息</param>
        void Redirect(string url, string message);
        /// <summary>
        /// 输出一段脚本，示意要弹出一个对话框，alert。
        /// </summary>
        /// <param name="message">要弹出的消息，如果为空或空字符串，将不输出脚本</param>
        /// <remarks> &lt;script&gt;alert(decodeURIComponent('%EC%20%3D'));&lt;/script&gt;</remarks>
        void Alert(string message);
        /// <summary>
        /// 输出一段脚本，示意要弹出一个对话框，alert。[延迟]
        /// </summary>
        /// <param name="message">要弹出的消息，如果为空或空字符串，将不输出脚本</param>
        /// <param name="delay">延迟时间，单位：毫秒</param>
        /// <remarks> &lt;script&gt;setTimeout(function(){alert(decodeURIComponent('%EC%20%3D'));},delay);&lt;/script&gt;</remarks>
        void Alert(string message, int delay);

        /// <summary>
        /// 向客户端发送当前所有缓冲的输出。
        /// </summary>
        void Flush();
        /// <summary>
        /// 关闭到客户端的套接字连接（不建议手动调用它，在关闭前会尝试输出缓冲中的数据）。
        /// </summary>
        void Close();
        /// <summary>
        /// 将当前所有缓冲的输出发送到客户端，停止该页的执行，并引发 IHttpApplication.EndRequest 事件。
        /// </summary>
        void End();


    }
}