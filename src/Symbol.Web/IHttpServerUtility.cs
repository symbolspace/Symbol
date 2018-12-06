/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供用于处理 Web 请求的 Helper 方法。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpServerUtility {
        /// <summary>
        /// 获取服务器的计算机名称（注意这不是客户端计算机名称）。
        /// </summary>
        /// <returns>本地计算机的名称。</returns>
        string MachineName { get; }
        /// <summary>
        /// 返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径。
        /// </summary>
        /// <param name="path">Web 服务器的虚拟路径。</param>
        /// <returns>与 path 相对应的物理文件路径。</returns>
        string MapPath(string path);
        /// <summary>
        /// 对字符串进行 URL 编码，并返回已编码的字符串。
        /// </summary>
        /// <param name="s">要进行 URL 编码的文本。</param>
        /// <returns>URL 编码的文本。</returns>
        string UrlEncode(string s);
        /// <summary>
        /// 对字符串进行 URL 编码，并将结果输出发送到 System.IO.TextWriter 输出流。
        /// </summary>
        /// <param name="s">要编码的文本字符串。</param>
        /// <param name="output">System.IO.TextWriter 输出包含已编码字符串的流。</param>
        void UrlEncode(string s, System.IO.TextWriter output);
        /// <summary>
        /// 对字符串进行 URL 解码并返回已解码的字符串。
        /// </summary>
        /// <param name="s">要解码的文本字符串。</param>
        /// <returns>已解码的文本。</returns>
        string UrlDecode(string s);
        /// <summary>
        /// 对在 URL 中接收的 HTML 字符串进行解码，并将结果输出发送到 System.IO.TextWriter 输出流。
        /// </summary>
        /// <param name="s">要解码的 HTML 字符串。</param>
        /// <param name="output">System.IO.TextWriter 输出包含已解码字符串的流。</param>
        void UrlDecode(string s, System.IO.TextWriter output);

        /// <summary>
        /// 对字符串进行 HTML 编码并返回已编码的字符串。
        /// </summary>
        /// <param name="s">要编码的文本字符串。</param>
        /// <returns>HTML 已编码的文本。</returns>
        string HtmlEncode(string s);
        /// <summary>
        /// 对字符串进行 HTML 编码，并将结果输出发送到 System.IO.TextWriter 输出流。
        /// </summary>
        /// <param name="s">要编码的字符串。</param>
        /// <param name="output">System.IO.TextWriter 输出包含已编码字符串的流。</param>
        void HtmlEncode(string s, System.IO.TextWriter output);
        /// <summary>
        /// 对 HTML 编码的字符串进行解码，并返回已解码的字符串。
        /// </summary>
        /// <param name="s">要解码的 HTML 字符串。</param>
        /// <returns>已解码的文本。</returns>
        string HtmlDecode(string s);
        /// <summary>
        /// 对 HTML 编码的字符串进行解码，并将结果输出发送到 System.IO.TextWriter 输出流。
        /// </summary>
        /// <param name="s">要解码的 HTML 字符串。</param>
        /// <param name="output">System.IO.TextWriter 输出包含已解码字符串的流。</param>
        void HtmlDecode(string s, System.IO.TextWriter output);
    }
}