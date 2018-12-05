/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 数据库表字段信息。
    /// </summary>
    public class DatabaseTableField {
        /// <summary>
        /// 获取或设置字段是否存在。
        /// </summary>
        public bool Exists { get; set; }
        /// <summary>
        /// 获取或设置表名称。
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 获取或设置字段名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置字段数据类型。
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 获取或设置字段在表中的位置。
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 获取或设置字段是否允许为空值（NULL）。
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// 获取或设置字段是否为主键。
        /// </summary>
        public bool IsPrimary { get; set; }
        /// <summary>
        /// 获取或设置字段是否为自增。
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 获取或设置字段长度（不是字节）。
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 获取或设置字段小数位数。
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 获取或设置字段默认值。
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 获取或设置字段描述。
        /// </summary>
        public string Description { get; set; }
    }

}