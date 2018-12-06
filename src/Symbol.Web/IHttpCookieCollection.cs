/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供操作 HTTP Cookie 的类型安全方法。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpCookieCollection : System.Collections.ICollection {
        /// <summary>
        /// 获取一个字符串数组，该数组包含此 Cookie 集合中的所有键（Cookie 名称）。
        /// </summary>
        string[] AllKeys { get; }
        /// <summary>
        /// 获取实例中的所有键。
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Keys { get; }
        /// <summary>
        /// 从 Cookie 集合中获取具有指定数字索引的 Cookie。
        /// </summary>
        /// <param name="index">要从集合中检索的 Cookie 索引。</param>
        /// <returns>按 index 指定的 IHttpCookie。</returns>
        IHttpCookie this[int index] { get; }
        /// <summary>
        /// 从 Cookie 集合中获取具有指定名称的 Cookie。
        /// </summary>
        /// <param name="name">要检索的 Cookie 名称。</param>
        /// <returns>由 name 指定的 IHttpCookie。</returns>
        IHttpCookie this[string name] { get; }
        /// <summary>
        /// 获取具有指定数字索引的 Cookie 值（简化操作，直接获取值，未找到就是null。）。
        /// </summary>
        /// <param name="index">要从集合中检索的 Cookie 索引。</param>
        /// <returns>按 index 指定的 IHttpCookie 的Value。</returns>
        string GetValue(int index);
        /// <summary>
        /// 获取具有指定名称的 Cookie 值（简化操作，直接获取值，未找到就是null。）。
        /// </summary>
        /// <param name="name">要检索的 Cookie 名称。</param>
        /// <returns>由 name 指定的 IHttpCookie 的Value。</returns>
        string GetValue(string name);
        /// <summary>
        /// 返回指定数字索引处的 Cookie 键（名称）。
        /// </summary>
        /// <param name="index">要从集合中检索的键索引。</param>
        /// <returns>按 index 指定的 Cookie 的名称。</returns>
        string GetKey(int index);
        /// <summary>
        /// 将指定的 Cookie 添加到此 Cookie 集合中。
        /// </summary>
        /// <param name="name">Cookie 名称。</param>
        /// <param name="value">Cookie 值。</param>
        /// <param name="path">路径，默认为 / ，如果设置为指定路径，将只有访问指定的路径时，客户端才会发送此Cookie。</param>
        /// <param name="domain">Cookie 限定的域名，一旦设置，只有访问此域名才会发送此Cookie。</param>
        /// <returns>返回创建的 IHttpCookie 。</returns>
        IHttpCookie Add(string name, string value, string path = "/", string domain = null);
        /// <summary>
        /// 从集合中移除具有指定名称的 Cookie。
        /// </summary>
        /// <param name="name">要从集合中移除的 Cookie 名称。</param>
        void Remove(string name);
        /// <summary>
        /// 从集合中移除具有数字索引的 Cookie。
        /// </summary>
        /// <param name="index">要从集合中移除的 Cookie 索引。</param>
        void RemoveAt(int index);
        /// <summary>
        /// 清除 Cookie 集合中的所有 Cookie。
        /// </summary>
        void Clear();

    }
}