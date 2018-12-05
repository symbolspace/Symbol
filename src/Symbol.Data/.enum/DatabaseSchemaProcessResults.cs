/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 数据库架构处理结果集。
    /// </summary>
    public enum DatabaseSchemaProcessResults {
        /// <summary>
        /// 成功
        /// </summary>
        [Const("成功")]
        Success,
        /// <summary>
        /// 错误
        /// </summary>
        [Const("错误")]
        Error,
        /// <summary>
        /// 忽略
        /// </summary>
        [Const("忽略")]
        Ignore,
    }


}