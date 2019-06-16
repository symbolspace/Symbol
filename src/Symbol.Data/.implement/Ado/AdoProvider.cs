/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 提供者。
    /// </summary>
    public abstract class AdoProvider : Provider, IAdoProvider {

        #region methods

        #region SetBuilderValue
        /// <summary>
        /// 设置构造器的参数。
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="values">参数列表。</param>
        /// <param name="name">标准命名。</param>
        /// <param name="aliasName">别名。</param>
        /// <param name="filter">过滤器。</param>
        protected void SetBuilderValue(System.Data.Common.DbConnectionStringBuilder builder, Collections.Generic.NameValueCollection<object> values, string name, string aliasName, ValueFilter filter = null) {
            if (SetBuilderValue(builder, aliasName, values[name], filter))
                values.Remove(name);
        }
        /// <summary>
        /// 设置构造器的参数。
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值，为null时，自动跳过。</param>
        /// <param name="filter">过滤器。</param>
        /// <returns>返回是否操作成功。</returns>
        protected bool SetBuilderValue(System.Data.Common.DbConnectionStringBuilder builder, string name, object value, ValueFilter filter = null) {
            if (value == null)
                return false;
            try {
                if (filter != null) {
                    value = filter(value);
                    if (value == null)
                        return false;
                }
                if (value is string) {
                    if (string.IsNullOrEmpty((string)value))
                        return false;
                }
                if (name.IndexOf(' ') > -1) {
                    builder[name] = value;
                } else {
                    FastWrapper.Set(builder, name, value, System.Reflection.BindingFlags.IgnoreCase);
                }
                return true;
            } catch (System.Exception) {
                return false;
            }
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// 数据过滤器。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected delegate object ValueFilter(object value);

        #endregion
    }


}