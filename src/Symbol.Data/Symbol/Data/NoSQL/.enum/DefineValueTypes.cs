/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 定义对象Entry 值类型集。
    /// </summary>
    public enum DefineValueTypes : int {
        /// <summary>
        /// 字段
        /// </summary>
        [Const("字段")]
        Field = 0,
        /// <summary>
        /// 行集
        /// </summary>
        [Const("行集")]
        Having = 1,
        /// <summary>
        /// 固定值
        /// </summary>
        [Const("固定值")]
        FixedValue = 2,
        /// <summary>
        /// 对象
        /// </summary>
        [Const("对象")]
        Object = 3,

    }
}