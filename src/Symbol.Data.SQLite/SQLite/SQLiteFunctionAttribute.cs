/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.SQLite {

    /// <summary>
    /// 扩展函数特性类
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SQLiteFunctionAttribute : System.Attribute {

        #region properties
        /// <summary>
        /// 获取或设置参数个数。
        /// </summary>
        public int Arguments { get; set; }
        /// <summary>
        /// 获取或设置类型。
        /// </summary>
        public FunctionTypes Type { get; set; }
        /// <summary>
        /// 获取或设置名称。
        /// </summary>
        public string Name { get; set; }
        #endregion
    }
}