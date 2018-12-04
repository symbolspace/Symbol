/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 引用关系Entry { "name": { "source": "sourceField" }, "by": { "target": "targetField" } }
    /// </summary>
    public class ReferEntry {

        #region fields
        private string _name;
        private string _source;
        private string _sourceField;
        private string _target;
        private string _targetField;
        #endregion

        #region properties
        /// <summary>
        /// 获取当前的名称是否为 $this。
        /// </summary>
        public bool IsThis { get { return string.Equals(_name, "$this", StringComparison.OrdinalIgnoreCase); } }
        /// <summary>
        /// 获取当前源是否为$self。
        /// </summary>
        public bool IsSelf { get { return string.Equals(_source, "$self", StringComparison.OrdinalIgnoreCase); } }
        /// <summary>
        /// 获取名称。
        /// </summary>
        public string Name {
            get { return _name; }
            //set {
            //    CommonException.CheckArgumentNull(value, "value");
            //    _name = value;
            //}
        }

        /// <summary>
        /// 获取或设置源。
        /// </summary>
        public string Source {
            get { return _source; }
            set {
                CommonException.CheckArgumentNull(value, "value");
                _source = value;
            }
        }
        /// <summary>
        /// 获取或设置源字段。
        /// </summary>
        public string SourceField {
            get { return _sourceField; }
            set {
                CommonException.CheckArgumentNull(value, "value");
                _sourceField = value;
            }
        }

        /// <summary>
        /// 获取或设置目标。
        /// </summary>
        public string Target {
            get { return _target; }
            set {
                CommonException.CheckArgumentNull(value, "value");
                _target = value;
            }
        }
        /// <summary>
        /// 获取或设置目标字段。
        /// </summary>
        public string TargetField {
            get { return _targetField; }
            set {
                CommonException.CheckArgumentNull(value, "value");
                _targetField = value;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建ReferEntry实例 $this。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <param name="field">字段名称。</param>
        public ReferEntry(string name, string field) : this("$this", name, field, null, null) {
        }
        /// <summary>
        /// 创建ReferEntry实例
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="source">源。</param>
        /// <param name="sourceField">源字段。</param>
        /// <param name="target">目标。</param>
        /// <param name="targetField">目标字段。</param>
        public ReferEntry(string name, string source, string sourceField, string target, string targetField) {
            CommonException.CheckArgumentNull(name, "name");
            CommonException.CheckArgumentNull(source, "source");
            CommonException.CheckArgumentNull(sourceField, "sourceField");
            

            _name = name;
            _source = source;
            _sourceField = sourceField;

            if (!IsThis) {
                CommonException.CheckArgumentNull(target, "target");
                CommonException.CheckArgumentNull(targetField, "targetField");
                _target = target;
                _targetField = targetField;
            }
        }
        #endregion

        #region methods

        #region ToObject
        /// <summary>
        /// 输出数据结构。 { "name": { "source": "sourceField" }, "by": { "target": "targetField" } }
        /// </summary>
        /// <returns></returns>
        public object ToObject() {
            System.Collections.Generic.IDictionary<string, object> o = new System.Collections.Generic.Dictionary<string, object>();
            System.Collections.Generic.IDictionary<string, object> my = new System.Collections.Generic.Dictionary<string, object>();
            my.Add(_source, _sourceField);
            o.Add(_name, my);

            if (!IsThis) {
                System.Collections.Generic.IDictionary<string, object> by = new System.Collections.Generic.Dictionary<string, object>();
                by.Add(_target, _targetField);
                o.Add("by", by);
            }

            return o;
        }
        #endregion
        #region ToJson
        /// <summary>
        /// 输出JSON字符串。 { "name": { "source": "sourceField" }, "by": { "target": "targetField" } }
        /// </summary>
        /// <returns>返回JSON字符串。</returns>
        public string ToJson() {
            return ToJson(false);
        }
        /// <summary>
        /// 输出JSON字符串。 { "name": { "source": "sourceField" }, "by": { "target": "targetField" } }
        /// </summary>
        /// <param name="formated">是否格式化。</param>
        /// <returns>返回JSON字符串。</returns>
        public string ToJson(bool formated) {
            return Symbol.Serialization.Json.ToString(ToObject(), false, formated);
        }
        #endregion
        #region ToString
        /// <summary>
        /// 输出JSON字符串。 { "name": { "source": "sourceField" }, "by": { "target": "targetField" } }
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToJson();
        }
        #endregion

        #endregion

    }   

}