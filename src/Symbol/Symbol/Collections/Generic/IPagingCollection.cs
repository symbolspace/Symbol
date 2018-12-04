/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Collections.Generic {

    /// <summary>
    /// 翻页适配器
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <remarks>遍历这个对象，就是当前页的所有行集。</remarks>
    public interface IPagingCollection<T> : System.Collections.Generic.IEnumerable<T> {
        /// <summary>
        /// 当前页码(0开始）
        /// </summary>
        int CurrentPageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        int ItemPerPage { get; set; }
        /// <summary>
        /// 是否允许页码超出最大值（默认允许）
        /// </summary>
        bool AllowPageOutOfMax { get; set; }
        /// <summary>
        /// 总行数
        /// </summary>
        int TotalCount { get; }
        /// <summary>
        /// 总页数
        /// </summary>
        int PageCount { get; }
    }
}