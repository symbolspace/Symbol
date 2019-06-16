/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data {
    /// <summary>
    /// 数据库提供者特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class ProviderAttribute : System.Attribute {

        #region fields
        private string _name;
        private System.Type _type;
        #endregion

        #region properties
        /// <summary>
        /// 获取名称。
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// 获取类型。
        /// </summary>
        public System.Type Type { get { return _type; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="type">类型。</param>
        public ProviderAttribute(string name, System.Type type) {
            _name = name;
            _type = type;
        }
        #endregion
    }


}