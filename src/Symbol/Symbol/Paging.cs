/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol {

    /// <summary>
    /// 翻页参数（主要用于API）。
    /// </summary>
    [Const("翻页参数")]
    public class Paging {

        #region properties
        /// <summary>
        /// 获取或设置当前页码(0开始）。
        /// </summary>
        [Const("当前页码(0开始）")]
        public virtual int page { get; set; }
        /// <summary>
        /// 获取或设置每页行数，为0时不分页。
        /// </summary>
        [Const("每页行数，为0时不分页")]
        public virtual int size { get; set; }
        /// <summary>
        /// 获取或设置是否允许页码超出最大值。
        /// </summary>
        [Const("是否允许页码超出最大值")]
        public virtual bool outMax { get; set; }
        /// <summary>
        /// 获取或设置是否分页。
        /// </summary>
        [Const("是否分页")]
        public virtual bool paging { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建Paging实例。
        /// </summary>
        public Paging() {
            page = 0;
            size = 30;
            outMax = false;
            paging = true;
        }
        /// <summary>
        /// 创建Paging实例。
        /// </summary>
        /// <param name="page">当前页码(0开始）。</param>
        /// <param name="size">每页行数，为0时不分页。</param>
        /// <param name="outMax">是否允许页码超出最大值</param>
        public Paging(int page, int size = 30, bool outMax = false) {
            this.page = page;
            this.size = size;
            this.outMax = outMax;
            this.paging = size > 0;
        }
        #endregion

    }

}