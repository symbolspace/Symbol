using System;

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 取字处理器辅助类
    /// </summary>
    public class CharRecognizerHelper {

        #region classes
        /// <summary>
        /// 取字处理器项
        /// </summary>
        public class RecognizerItem {
            /// <summary>
            /// 获取或设置名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 获取或设置显示名称
            /// </summary>
            public string DisplayName { get; set; }
            /// <summary>
            /// 获取或设置类型
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// 创建实例
            /// </summary>
            /// <returns>返回实例</returns>
            public ICharRecognizer CreateInstance() {
                return (ICharRecognizer)FastWrapper.CreateInstance(Type);
            }
        }
        #endregion

        #region fields
        private static readonly System.Collections.Generic.Dictionary<string, RecognizerItem> _handlers = null;
        #endregion

        #region properties
        /// <summary>
        /// 获取处理器列表。
        /// </summary>
        public static System.Collections.Generic.IEnumerable<RecognizerItem> Handlers {
            get { return _handlers.Values; }
        }
        #endregion

        #region cctor
        static CharRecognizerHelper() {
            _handlers = new System.Collections.Generic.Dictionary<string,RecognizerItem>(StringComparer.OrdinalIgnoreCase);
            Type baseType = typeof(ICharRecognizer);
            foreach (Type item in baseType.Assembly.GetTypes()) {
                if (item.IsAbstract || !item.IsPublic || !item.IsClass || !TypeExtensions.IsInheritFrom(item, baseType))
                    continue;
                string name = ConstAttributeExtensions.Const(item, "Name");
                string displayName = ConstAttributeExtensions.Const(item, "DisplayName");
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(displayName))
                    continue;
                _handlers.Add(name, new RecognizerItem() {
                    Name = name,
                    DisplayName = displayName,
                    Type = item,
                });
            }
        }
        #endregion


        #region methods

        #region CreateInstance
        /// <summary>
        /// 创建处理器实例。
        /// </summary>
        /// <param name="name">名称（唯一，英文）。</param>
        /// <returns>如果存在此处理器，返回它的实例；不存在时直接返回null。</returns>
        public static ICharRecognizer CreateInstance(string name) {
            if (_handlers.ContainsKey(name)) {
                return _handlers[name].CreateInstance();
            }
            return null;
        }
        #endregion

        #endregion
    }
}
