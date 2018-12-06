/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对客户端上载文件的访问，并组织这些文件。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpFileCollection : System.Collections.ICollection, System.IDisposable {
        /// <summary>
        /// 获取一个字符串数组，该数组包含文件集合中所有成员的键（名称）。
        /// </summary>
        string[] AllKeys { get; }
        /// <summary>
        /// 获取实例中的所有键。
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Keys { get; }

        /// <summary>
        /// 获取具有指定数字索引的对象。
        /// </summary>
        /// <param name="index">要从文件集合中获取的项索引。</param>
        /// <returns>按 index 指定的 IHttpPostedFile。</returns>
        IHttpPostedFile this[int index] { get; }
        /// <summary>
        /// 从文件集合中获取具有指定名称的对象。
        /// </summary>
        /// <param name="name">要返回的项名称。</param>
        /// <returns>按 name 指定的 IHttpPostedFile。</returns>
        IHttpPostedFile this[string name] { get; }

        /// <summary>
        /// 返回具有指定数字索引的 IHttpPostedFile 成员名称。
        /// </summary>
        /// <param name="index">要返回的对象名索引。</param>
        /// <returns>按 index 指定的 IHttpPostedFile 成员的名称。</returns>
        string GetKey(int index);
    }
}