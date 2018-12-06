/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 节点值
    /// </summary>
    public class NodeValue {

        #region fields
        private object _value = null;
        private NodeValueTypes _type;
        private System.Type _valueType = null;
        private bool _isValue = false;
        private int _length = 0;
        private static readonly System.Collections.Generic.Dictionary<System.Type, NodeValueTypes> _types;
        #endregion

        #region properties
        /// <summary>
        /// 获取当前值。
        /// </summary>
        public object Value { get { return _value; } }
        /// <summary>
        /// 获取值的类型。
        /// </summary>
        public System.Type ValueType { get { return _valueType; } }
        /// <summary>
        /// 获取节点的类型。
        /// </summary>
        public NodeValueTypes Type { get { return _type; } }
        /// <summary>
        /// 获取是否为纯值。
        /// </summary>
        public bool IsValue { get { return _isValue; } }
        /// <summary>
        /// 获取集合长度（仅限集合类型）。
        /// </summary>
        public int Length { get { return _length; } }
        #region this[string path]
        /// <summary>
        /// 获取指定path的节点值。
        /// </summary>
        /// <param name="path">path规则。</param>
        /// <returns></returns>
        public NodeValue this[string path] {
            get {
                return new NodeValue(FastObject.Path(_value, path));
            }
        }
        #endregion
        #region this[int index]
        /// <summary>
        /// 获取指定索引的节点值。
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns></returns>
        public NodeValue this[int index] {
            get {
                return new NodeValue(FastObject.Path(_value, "[" + index + "]"));
            }
        }
        #endregion

        #endregion

        #region cctor
        static NodeValue() {
            _types = new System.Collections.Generic.Dictionary<System.Type, NodeValueTypes>();
            _types.Add(typeof(string), NodeValueTypes.String);
            _types.Add(typeof(bool), NodeValueTypes.Boolean);
            _types.Add(typeof(System.DateTime), NodeValueTypes.DateTime);
            _types.Add(typeof(System.TimeSpan), NodeValueTypes.TimeSpan);
            _types.Add(typeof(System.Guid), NodeValueTypes.Guid);
        }
        #endregion
        #region ctor
        /// <summary>
        /// 创建NodeValue实例。
        /// </summary>
        /// <param name="value">值。</param>
        public NodeValue(object value) {
            _value = value;
            _type = GetNodeType();
        }
        #endregion

        #region methods

        #region GetNodeType
        NodeValueTypes GetNodeType() {
            if (_value == null) {
                _valueType = typeof(object);
                return NodeValueTypes.Null;
            }
            _valueType = _value.GetType();
            NodeValueTypes nodeType;
            if (_types.TryGetValue(_valueType, out nodeType)) {
                _isValue = true;
                return nodeType;
            }
            if (_valueType.IsValueType) {
                _isValue = true;
                return NodeValueTypes.Number;
            }
            if (_valueType.IsArray) {
                _length = ((System.Array)_value).Length;
                return NodeValueTypes.Array;
            }
            if (TypeExtensions.IsAnonymousType(_valueType))
                return NodeValueTypes.Object;

            if (TypeExtensions.IsInheritFrom(_valueType, typeof(System.Collections.IDictionary))
                || TypeExtensions.IsInheritFrom(_valueType, typeof(System.Collections.Generic.IDictionary<string, object>)))
                return NodeValueTypes.Dictionary;
            if (TypeExtensions.IsInheritFrom(_valueType, typeof(System.Collections.IList))
                 || TypeExtensions.IsInheritFrom(_valueType, typeof(System.Collections.Generic.IList<object>))) {
                _length = TypeExtensions.Convert<int>(FastWrapper.Get(_value, "Count"), 0);
                return NodeValueTypes.Array;
            }
            if (_valueType.IsGenericType) {
                if (_valueType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>)) {
                    _length = TypeExtensions.Convert<int>(FastWrapper.Get(_value, "Count"), 0);
                    return NodeValueTypes.Array;
                }
                if (_valueType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.Dictionary<,>))
                    return NodeValueTypes.Dictionary;
            }
            return NodeValueTypes.Object;
        }
        #endregion

        #region As
        /// <summary>
        /// 尝试转换
        /// </summary>
        /// <param name="value">如果为NodeValue直接返回，反之包装。</param>
        /// <returns></returns>
        public static NodeValue As(object value) {
            NodeValue nodeValue = value as NodeValue;
            return nodeValue ?? new NodeValue(value);
        }
        #endregion

        #endregion

    }

}