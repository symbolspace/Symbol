/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对会话状态值以及会话级别设置和生存期管理方法的访问。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IHttpSessionState : System.Collections.ICollection {
        /// <summary>
        /// 获取一个值，该值指示会话是否是与当前请求一起创建的。
        /// </summary>
        bool IsNewSession { get; }
        /// <summary>
        /// 获取会话的唯一标识符。
        /// </summary>
        string SessionID { get; }
        /// <summary>
        /// 获取并设置在会话状态提供程序终止会话之前各请求之间所允许的时间（以分钟为单位）。
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// 获取一个字符串数组，该数组包含此 Session 集合中的所有键（Session 名称）。
        /// </summary>
        string[] AllKeys { get; }
        /// <summary>
        /// 获取实例中的所有键。
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Keys { get; }

        /// <summary>
        /// 按数字索引获取或设置会话值。
        /// </summary>
        /// <param name="index">会话值的数字索引。</param>
        /// <returns>存储在指定索引处的会话状态值；如果该项不存在，则为 null。</returns>
        object this[int index] { get; set; }
        /// <summary>
        /// 按名称获取或设置会话值。
        /// </summary>
        /// <param name="name">会话值的键名。</param>
        /// <returns>具有指定名称的会话状态值；如果该项不存在，则为 null。</returns>
        object this[string name] { get; set; }

        /// <summary>
        /// 取消当前会话（实际效果是当前请求内还可以用（所有数据会清空），只是在下次请求时，它已经是新的了，原来的数据全没了。）。
        /// </summary>
        void Abandon();
        /// <summary>
        /// 从会话状态集合中移除所有的键和值。
        /// </summary>
        void Clear();
        /// <summary>
        /// 向会话状态集合添加一个新项。
        /// </summary>
        /// <param name="name">要添加到会话状态集合的项的名称。</param>
        /// <param name="value">要添加到会话状态集合的项的值。</param>
        void Add(string name, object value);
        /// <summary>
        /// 删除会话状态集合中的项。
        /// </summary>
        /// <param name="name">要从会话状态集合中删除的项的名称。</param>
        void Remove(string name);
        /// <summary>
        /// 删除会话状态集合中指定索引处的项。
        /// </summary>
        /// <param name="index">要从会话状态集合中移除的项的索引。</param>
        void RemoveAt(int index);
    }
}