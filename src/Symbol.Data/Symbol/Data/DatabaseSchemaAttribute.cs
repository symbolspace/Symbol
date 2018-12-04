/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Reflection;

namespace Symbol.Data {
    /// <summary>
    /// 数据库架构特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DatabaseSchemaAttribute : System.Attribute {

        #region fields
        private string _tableName;
        private double _order;
        private string _description;
        private string[] _references;
        private bool _isValid;
        private DatabaseSchemaTypes _type;
        private static readonly System.Collections.Generic.List<RegexEntry> _regexs;
        #endregion

        #region properties
        /// <summary>
        /// 获取表名。
        /// </summary>
        public string TableName { get { return _tableName; } }
        /// <summary>
        /// 获取优先级。
        /// </summary>
        public double Order { get { return _order; } }
        /// <summary>
        /// 获取描述。
        /// </summary>
        public string Description { get { return _description; } }
        /// <summary>
        /// 获取类型。
        /// </summary>
        public DatabaseSchemaTypes Type { get { return _type; } }
        /// <summary>
        /// 获取是否已验证。
        /// </summary>
        public bool IsValid { get { return _isValid; } }
        /// <summary>
        /// 获取或设置引用表名称列表（如果引用具体某个版本号，请在表名后面加点"."）。
        /// </summary>
        public string[] References {
            get { return _references; }
            set {
                _references = value ?? new string[0];
            }
        }
        /// <summary>
        /// 获取或设置引用表列表（多个用逗号隔开，如果引用具体某个版本号，请在表名后面加点"."）
        /// </summary>
        public string Reference {
            get { return string.Join(",", _references); }
            set {
                if (string.IsNullOrEmpty(value))
                    _references = new string[0];
                else
                    _references = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        #endregion

        #region cctor
        static DatabaseSchemaAttribute() {
            _regexs = new System.Collections.Generic.List<RegexEntry>();
            foreach (DatabaseSchemaTypes item in Enum.GetValues(typeof(DatabaseSchemaTypes))) {
                string p = EnumExtensions.GetProperty(item, "Prefix");
                if (p == "*")
                    continue;
                RegexEntry entry = new RegexEntry() {
                    regex = new System.Text.RegularExpressions.Regex(p, System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase),
                    type = item,
                };
                _regexs.Add(entry);
            }
        }
        #endregion
        #region ctor
        /// <summary>
        /// 创建DatabaseSchemaAttribute实例。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="order">优先级</param>
        /// <param name="description">描述</param>
        public DatabaseSchemaAttribute(string tableName, double order, string description)
            : this(tableName, order, description, null) {
        }
        /// <summary>
        /// 创建DatabaseSchemaAttribute实例。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="order">优先级</param>
        /// <param name="description">描述</param>
        /// <param name="references">引用表名称列表（如果引用具体某个版本号，请在表名后面加点"."）</param>
        public DatabaseSchemaAttribute(string tableName, double order, string description, params string[] references) {

            _tableName = tableName;
            _order = order;
            _description = description;
            _isValid = !string.IsNullOrEmpty(tableName);
            References = references;
            ParseType();
        }
        #endregion

        #region methods
        void ParseType() {
            if (!_isValid)
                return;
            foreach (RegexEntry entry in _regexs) {
                if (entry.regex.IsMatch(_tableName)) {
                    _type = entry.type;
                    return;
                }
            }
            _type = DatabaseSchemaTypes.Table;
            if (_order > 1)
                _type = DatabaseSchemaTypes.TableField;
        }
        #endregion

        #region types
        class RegexEntry {
            public System.Text.RegularExpressions.Regex regex;
            public DatabaseSchemaTypes type;
        }
        #endregion
    }


}