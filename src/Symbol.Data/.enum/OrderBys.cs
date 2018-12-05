/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 排序规则集。
    /// </summary>
    [Const("排序规则集")]
    public enum OrderBys {
        /// <summary>
        /// 升序/顺序，值越小越在前。
        /// </summary>
        [Const("升序")]
        [Const("Description", "升序/顺序，值越小越在前")]
        [Const("Keyword", "asc")]
        Ascing,
        /// <summary>
        /// 降序/逆序/倒序，值越大越在前。
        /// </summary>
        [Const("降序")]
        [Const("Description", "降序/逆序/倒序，值越大越在前")]
        [Const("Keyword", "desc")]
        Descing,
    }

}