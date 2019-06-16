/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.SQLite {
    /// <summary>
    /// 排序规则类型集
    /// </summary>
    public enum CollationTypes {
        /// <summary>
        /// 自定义
        /// </summary>
        Custom,
        /// <summary>
        /// 二进制
        /// </summary>
        Binary,
        /// <summary>
        /// 未定义
        /// </summary>
        NoCase,
        /// <summary>
        /// 反序
        /// </summary>
        Reverse
    }
}