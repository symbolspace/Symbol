/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对客户端已上载的单独文件的访问。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpPostedFile : System.IDisposable{
        /// <summary>
        /// 获取上载文件的大小（以字节为单位）。
        /// </summary>
        /// <returns>文件长度（以字节为单位）。</returns>
        int ContentLength { get; }
        /// <summary>
        /// 获取客户端发送的文件的 MIME 内容类型。比如png图片，可能会是image/png。
        /// </summary>
        /// <returns>上载文件的 MIME 内容类型。</returns>
        string ContentType { get; }
        /// <summary>
        /// 获取客户端上的文件的完全限定名称（需要注意的是，有些移动设备故意不提交文件名。）。
        /// </summary>
        /// <returns>客户端的文件的名称，包含目录路径。</returns>
        string FileName { get; }
        /// <summary>
        /// 获取一个 System.IO.Stream 对象，该对象指向一个上载文件，以准备读取该文件的内容。
        /// </summary>
        /// <returns>指向文件的 System.IO.Stream。</returns>
        System.IO.Stream InputStream { get; }

        /// <summary>
        /// 保存上载文件的内容。
        /// </summary>
        /// <param name="filename">保存的文件的名称（必须是绝对路径，不能是网址。）。</param>
        void SaveAs(string filename);
    }
}