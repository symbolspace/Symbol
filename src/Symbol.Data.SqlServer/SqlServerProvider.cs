/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SqlServer数据库提供者基类
    /// </summary>
    public abstract class SqlServerProvider : AdoProvider {


        #region ctor
        /// <summary>
        /// 创建 SqlServerProvider 的实例。
        /// </summary>
        public SqlServerProvider() {
        }
        #endregion

        #region IDatabaseProvider 成员
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public override IConnection CreateConnection(string connectionString) {
            CommonException.CheckArgumentNull(connectionString, "connectionString");
            return new SqlServerConnection(this, FastWrapper.CreateInstance<System.Data.SqlClient.SqlConnection>(connectionString), connectionString);
        }
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public override IConnection CreateConnection(object connectionOptions) {
            {
                if (connectionOptions is string connectionString) {
                    connectionString = connectionString.Trim();
                    if (connectionString.StartsWith("{")) {
                        connectionOptions = JSON.Parse(connectionString);
                        goto lb_Object;
                    }
                    return CreateConnection(connectionString);
                }
                if (connectionOptions is ConnectionOptions connectionOptions2) {
                    connectionOptions = connectionOptions2.ToObject();
                    goto lb_Object;
                }
            }
           lb_Object:
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            builder.MaxPoolSize = 1024;
            SetBuilderValue(builder, values, "host", "Data Source", p => {
                string p10 = p as string;
                if (string.IsNullOrEmpty(p10))
                    return p;
                if (p10.IndexOf(':') > -1) {
                    return p10.Replace(':', ',');
                }
                return p;
            });
            SetBuilderValue(builder, values, "port", "Data Source", p => {
                string p10 = TypeExtensions.Convert<string>(p);
                if (string.IsNullOrEmpty(p10))
                    return null;
                builder.DataSource += "," + p10;
                return null;
            });

            SetBuilderValue(builder, values, "name", "Initial Catalog");
            SetBuilderValue(builder, values, "account", "User ID");
            SetBuilderValue(builder, values, "password", "Password");
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in values) {
                //builder[item.Key] = item.Value;
                SetBuilderValue(builder, item.Key, item.Value);
            }
            if (!builder.PersistSecurityInfo && !builder.IntegratedSecurity) {
                builder.PersistSecurityInfo = true;
            }
            builder.MultipleActiveResultSets = true;
            return CreateConnection(builder.ConnectionString);
        }
      
        /// <summary>
        /// 创建方言。
        /// </summary>
        /// <returns>返回方言对象。</returns>
        public override IDialect CreateDialect() {
            return new SqlServerDialect();
        }

        #endregion
    }
}

