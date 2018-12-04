/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 数据库架构类型集。
    /// </summary>
    public enum DatabaseSchemaTypes {
        /// <summary>
        /// 表空间
        /// </summary>
        [Const("表空间")]
        [Const("Prefix", "^tablespace\\.")]
        TableSpace = 1,
        /// <summary>
        /// 函数
        /// </summary>
        [Const("函数")]
        [Const("Prefix", "^fun\\.")]
        Function = 2,
        /// <summary>
        /// 表
        /// </summary>
        [Const("表")]
        [Const("Prefix", "*")]
        Table = 1024,
        /// <summary>
        /// 字段
        /// </summary>
        [Const("字段")]
        [Const("Prefix", "*")]
        TableField = 2048,

        /// <summary>
        /// 视图
        /// </summary>
        [Const("视图")]
        [Const("Prefix", "^view\\.")]
        Viewer = 8192,
        /// <summary>
        /// 行集
        /// </summary>
        [Const("行集")]
        [Const("Prefix", "^row\\.")]
        Rows = 16384,
    }


}