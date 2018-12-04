/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 节点类型集。
    /// </summary>
    [Const("节点类型集")]
    public enum NodeValueTypes {
        /// <summary>
        /// 空
        /// </summary>
        [Const("空")]
        Null,
        /// <summary>
        /// 数组
        /// </summary>
        [Const("数组")]
        Array,
        /// <summary>
        /// 字典
        /// </summary>
        [Const("字典")]
        Dictionary,
        /// <summary>
        /// 对象
        /// </summary>
        [Const("对象")]
        Object,
        /// <summary>
        /// 文本
        /// </summary>
        [Const("文本")]
        String,
        /// <summary>
        /// 布尔
        /// </summary>
        [Const("布尔")]
        Boolean,
        /// <summary>
        /// 数字
        /// </summary>
        [Const("数字")]
        Number,
        /// <summary>
        /// 日期
        /// </summary>
        [Const("日期")]
        DateTime,
        /// <summary>
        /// 时间
        /// </summary>
        [Const("时间")]
        TimeSpan,
        /// <summary>
        /// 标识
        /// </summary>
        [Const("标识")]
        Guid,
    }

}