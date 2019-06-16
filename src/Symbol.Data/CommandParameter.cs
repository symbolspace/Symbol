/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// IDbCommand参数类。
    /// </summary>
    public class CommandParameter {

        #region fields
        private Symbol.Collections.Generic.NameValueCollection<object> _properties;
        #endregion

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
        /// 获取或设置是否为输出参数。
        /// </summary>
        public bool IsOut { get; set; }
        /// <summary>
        /// 获取或设置是否为返回值。
        /// </summary>
        public bool IsReturn { get; set; }
        /// <summary>
        /// 获取或设置是否创建。
        /// </summary>
        public bool Created { get; set; }

        /// <summary>
        /// 获取或设置属性列表
        /// </summary>
        public Symbol.Collections.Generic.NameValueCollection<object> Properties {
            get {
                if (_properties == null)
                    _properties = new Symbol.Collections.Generic.NameValueCollection<object>();
                return _properties;
            }
            set { _properties = value; }
        }
        #endregion

        #region ctor

        #endregion

    }

}