/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供创建和操作各 HTTP Cookie 的类型安全方法。
    /// </summary>
    public class HttpCookie :
        System.MarshalByRefObject,
        IHttpCookie {
        #region IHttpCookie 成员

        /// <summary>
        /// 获取或设置 Cookie 的名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置将此 Cookie 与其关联的域。
        /// </summary>
        /// <returns>要将此 Cookie 与其关联的域名。默认值为当前域。</returns>
        public string Domain { get; set; }
        /// <summary>
        /// 获取或设置要与当前 Cookie 一起传输的虚拟路径。
        /// </summary>
        /// <returns>要与此 Cookie 一起传输的虚拟路径。默认值为当前请求的路径。</returns>
        public string Path { get; set; }
        /// <summary>
        /// 获取或设置单个 Cookie 值。
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指定 Cookie 是否可通过客户端脚本访问。
        /// </summary>
        /// <returns>如果 Cookie 具有 HttpOnly 属性且不能通过客户端脚本访问，则为 true；否则为 false。默认值为 false。</returns>
        public bool HttpOnly { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否使用安全套接字层 (SSL)（即仅通过 HTTPS）传输 Cookie（目前没有用，未处理它的值。）。
        /// </summary>
        public bool Secure { get; set; }
        /// <summary>
        /// 获取或设置此 Cookie 的过期日期和时间。
        /// </summary>
        /// <returns>此 Cookie 的过期时间（在客户端）。</returns>
        public System.DateTime Expires { get; set; }

        #endregion
    }
}