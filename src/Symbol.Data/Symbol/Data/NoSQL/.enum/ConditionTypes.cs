/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 规则类型集
    /// </summary>
    [Const("规则类型集")]
    public enum ConditionTypes {
        /// <summary>
        /// 根，没有实际作用
        /// </summary>
        [Const("根")]
        Root,
        /// <summary>
        /// 逻辑操作
        /// </summary>
        [Const("逻辑操作")]
        Logical,
        /// <summary>
        /// 字段
        /// </summary>
        [Const("字段")]
        Field,
    }
}