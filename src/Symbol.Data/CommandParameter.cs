/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// IDbCommand参数类。
    /// </summary>
    public class CommandParameter {

        #region properties
        /// <summary>
        /// 获取或设置名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置真实数据类型。
        /// </summary>
        public System.Type RealType { get; set; }
        /// <summary>
        /// 获取或设置当前值。
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 获取或设置属性列表
        /// </summary>
        public Symbol.Collections.Generic.NameValueCollection<object> Properties { get; set; }
        #endregion

    }

}