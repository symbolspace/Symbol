/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 查询命令where表达式逻辑操作符集。
    /// </summary>
    [Const("逻辑操作符集")]
    public enum WhereOperators {
        /// <summary>
        /// 无
        /// </summary>
        [Const("无")]
        None,
        /// <summary>
        /// 操作符 and 。
        /// </summary>
        [Const("并且")]
        And,
        /// <summary>
        /// 操作符 or 。
        /// </summary>
        [Const("或者")]
        Or,
    }


}