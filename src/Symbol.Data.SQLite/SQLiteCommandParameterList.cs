/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SQLite 命令参数列表
    /// </summary>
    public class SQLiteCommandParameterList : CommandParameterList {

        #region fields
        #endregion

        #region ctor
        /// <summary>
        /// 创建SQLiteCommandParameterList实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        public SQLiteCommandParameterList(IProvider provider)
            : base(provider) {
        }
        #endregion


        #region methods

        /// <summary>
        /// 创建参数回调
        /// </summary>
        /// <param name="item">参数对象</param>
        protected override void OnCreate(CommandParameter item) {
            if (item.Value == null) {
                return;
            }
            if (item.Value.GetType() != item.RealType) {
                item.Value = TypeExtensions.Convert(item.Value, item.RealType);
            }
          
            if (item.RealType.IsArray && item.RealType.GetElementType() != typeof(byte)) {
                item.Value = item.Value == null ? null : JSON.ToJSON(item.Value);
                return;
            }
            if (
                   TypeExtensions.IsAnonymousType(item.RealType) 
                || TypeExtensions.IsInheritFrom(item.RealType, typeof(System.Collections.Generic.IDictionary<string, object>))
                || (item.RealType.IsClass && !TypeExtensions.IsSystemBaseType(item.RealType))   
            ) {
                item.RealType = typeof(object);
                item.Value = item.Value == null ? null : JSON.ToJSON(item.Value);
                return;
            }

        }

        #endregion
    }

}

