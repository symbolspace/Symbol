/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.SQLite {

    /// <summary>
    /// 扩展函数类型集。
    /// </summary>
    public enum FunctionTypes {
        /// <summary>
        /// 执行函数。
        /// </summary>
        [Const("执行函数")]
        Scalar = 0,
        /// <summary>
        /// 聚合函数。
        /// </summary>
        [Const("聚合函数")]
        Aggregate = 1,
        /// <summary>
        /// 排序规则。
        /// </summary>
        [Const("排序规则")]
        Collation = 2,
    }

}