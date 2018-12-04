/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 匹配操作符集
    /// </summary>
    [Const("匹配操作符集")]
    public enum MatchOpertaors {
        /// <summary>
        /// ==
        /// </summary>
        [Const("等于")]
        [Const("Keyword", "=")]
        Equals,
        /// <summary>
        /// &lt;&gt;
        /// </summary>
        [Const("不等于")]
        [Const("Keyword", "<>")]
        NotEquals,
        /// <summary>
        /// &gt;
        /// </summary>
        [Const("大于")]
        [Const("Keyword", ">")]
        GreaterThan,
        /// <summary>
        /// &gt;=
        /// </summary>
        [Const("大于等于")]
        [Const("Keyword", ">=")]
        GreaterThanEquals,
        /// <summary>
        /// &lt;
        /// </summary>
        [Const("小于")]
        [Const("Keyword", "<")]
        LessThan,
        /// <summary>
        /// &lt;=
        /// </summary>
        [Const("小于等于")]
        [Const("Keyword", "<=")]
        LessThanEquals,
    }

}