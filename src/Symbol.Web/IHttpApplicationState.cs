/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 启用应用程序中多个会话和请求之间的全局信息共享（仅网站内）。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpApplicationState : System.Collections.ICollection {
        /// <summary>
        /// 获取集合中的访问键。
        /// </summary>
        string[] AllKeys { get; }
        /// <summary>
        /// 获取实例中的所有键。
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Keys { get; }

        /// <summary>
        /// 通过索引获取单个对象。
        /// </summary>
        /// <param name="index">集合中对象的数字索引。</param>
        /// <returns>index 所引用的对象。</returns>
        object this[int index] { get; set; }
        /// <summary>
        /// 通过名称获取单个对象的值。
        /// </summary>
        /// <param name="name">集合中的对象名。</param>
        /// <returns>name 所引用的对象。</returns>
        object this[string name] { get; set; }

        /// <summary>
        /// 通过索引获取对象名。
        /// </summary>
        /// <param name="index">应用程序状态对象的索引。</param>
        /// <returns>保存应用程序状态对象所使用的名称。</returns>
        string GetKey(int index);

        /// <summary>
        /// 从集合中移除所有对象。
        /// </summary>
        void Clear();
        /// <summary>
        /// 将新的对象添加到集合中。
        /// </summary>
        /// <param name="name">要添加到集合中的对象名。</param>
        /// <param name="value">对象的值。</param>
        void Add(string name, object value);

        /// <summary>
        /// 从集合中移除命名对象。
        /// </summary>
        /// <param name="name">要从集合中移除的对象名。</param>
        void Remove(string name);
        /// <summary>
        /// 按索引从集合中移除一个对象。
        /// </summary>
        /// <param name="index">要移除的项在集合中的位置。</param>
        void RemoveAt(int index);

        /// <summary>
        /// 锁定变量的访问以促进访问同步（目前没有加锁，与其容易堵死不如混乱着用吧）。
        /// </summary>
        void Lock();
        /// <summary>
        /// 取消锁定变量的访问以促进访问同步。
        /// </summary>
        void UnLock();
    }
}