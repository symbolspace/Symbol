/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 命令参数列表
    /// </summary>
    public class PostgreSQLCommandParameterList : CommandParameterList {

        #region fields
        #endregion

        #region ctor
        /// <summary>
        /// 创建PostgreSQLCommandParameterList实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        public PostgreSQLCommandParameterList(IProvider provider)
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
            
            if (item.RealType.IsEnum) {
                //item.Properties["PostgreSQLDbType"] = TypeExtensions.Convert("Int64", PostgreSQLProvider.GetDbType());
                item.RealType = typeof(long);
                item.Value = TypeExtensions.Convert<long>(item.Value);
                return;
            }
            if (item.RealType.IsArray && item.RealType.GetElementType() == typeof(string)) {
                item.Properties["NpgsqlDbType"] = TypeExtensions.Convert("Array,Text", PostgreSQLProvider.GetDbType());
                return;
            }
            if (item.RealType.IsArray && item.RealType.GetElementType() != typeof(byte)) {
                item.Properties["NpgsqlDbType"] = TypeExtensions.Convert("Json", PostgreSQLProvider.GetDbType());
                item.Value = item.Value == null ? null : JSON.ToJSON(item.Value);
                return;
            }
            if (
                   TypeExtensions.IsAnonymousType(item.RealType) 
                || TypeExtensions.IsInheritFrom(item.RealType, typeof(System.Collections.Generic.IDictionary<string, object>))
                || (item.RealType.IsClass && !TypeExtensions.IsSystemBaseType(item.RealType))   
            ) {
                item.Properties["NpgsqlDbType"] = TypeExtensions.Convert("Json", PostgreSQLProvider.GetDbType());
                item.RealType = typeof(object);
                item.Value = item.Value == null ? null : JSON.ToJSON(item.Value);
                return;
            }

        }

        #endregion
    }

}

