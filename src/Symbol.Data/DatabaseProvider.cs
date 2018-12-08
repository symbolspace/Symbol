/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据库提供者基类
    /// </summary>
    public abstract class DatabaseProvider : IDatabaseProvider {

        #region IDatabaseProvider 成员
        /// <summary>
        /// 获取是否支持多查询。
        /// </summary>
        public virtual bool MultipleActiveResultSets {
            get {
                return false;
            }
        }
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public abstract IDbConnection CreateConnection(string connectionString);
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public abstract IDbConnection CreateConnection(object connectionOptions);

        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public abstract IDataContext CreateDataContext(IDbConnection connection);

        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据上下文。</returns>
        public virtual IDataContext CreateDataContext(string connectionString) {
            if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("{"))
                return CreateDataContext(JSON.Parse(connectionString));
            return CreateDataContext(CreateConnection(connectionString));
        }
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据上下文。</returns>
        public virtual IDataContext CreateDataContext(object connectionOptions) {
            return CreateDataContext(CreateConnection(connectionOptions));
        }

        #endregion

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

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        public abstract string PreName(string name);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        public virtual string PreName(string pairs, string spliter) {
            if (string.IsNullOrEmpty(pairs))
                return "";
            return PreName(pairs.Split(new string[] { spliter }, System.StringSplitOptions.RemoveEmptyEntries));
        }
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        public virtual string PreName(string[] pairs) {
            if (pairs == null || pairs.Length == 0)
                return "";

            for (int i = 0; i < pairs.Length; i++) {
                pairs[i] = PreName(pairs[i]);
            }
            return string.Join(".", pairs);
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
