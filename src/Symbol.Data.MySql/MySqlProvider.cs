/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

[assembly: Symbol.Data.Provider("mysql", typeof(Symbol.Data.MySqlProvider))]
namespace Symbol.Data {

    /// <summary>
    /// Mysql数据库提供者(5.x)
    /// </summary>
    public class MySqlProvider : AdoProvider {

        #region fields
        private static readonly Symbol.Collections.Generic.NameValueCollection<System.Type> _types;
        #endregion

        #region cctor
        static MySqlProvider() {
            _types = new Collections.Generic.NameValueCollection<System.Type>();
#if netcore || net452
            _types.Add("MySql.Data.MySqlClient.MySqlConnection", typeof(MySql.Data.MySqlClient.MySqlConnection));
            _types.Add("MySql.Data.MySqlClient.MySqlConnectionStringBuilder", typeof(MySql.Data.MySqlClient.MySqlConnectionStringBuilder));
            _types.Add("MySql.Data.MySqlClient.MySqlDbType", typeof(MySql.Data.MySqlClient.MySqlDbType));
#else
            GetType("MySql.Data.MySqlClient.MySqlConnection");
            GetType("MySql.Data.MySqlClient.MySqlConnectionStringBuilder");
            GetType("MySql.Data.MySqlClient.MySqlDbType");
#endif
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建 MySqlProvider 的实例。
        /// </summary>
        public MySqlProvider() {
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
            System.Type type = GetConnectionType(true);
            return new AdoConnection(this, FastWrapper.CreateInstance<IDbConnection>(type, connectionString), connectionString);
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
            System.Data.Common.DbConnectionStringBuilder builder = FastWrapper.CreateInstance<System.Data.Common.DbConnectionStringBuilder>(GetType("MySql.Data.MySqlClient.MySqlConnectionStringBuilder", true));
            builder["pooling"] = true;
            //builder["MaxPoolSize"] = 1024;
            //builder["Port"] = 5432;
            builder["Charset"] = "utf8";
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            SetBuilderValue(builder, "SslMode", "None");
            SetBuilderValue(builder, values, "port", "Port");
            SetBuilderValue(builder, values, "host", "Data Source", p=> {
                string p10 = p as string;
                if (string.IsNullOrEmpty(p10))
                    return p;
                if (p10.IndexOf(':') > -1) {
                    string[] pair = p10.Split(':');
                    SetBuilderValue(builder, "Port", pair[1]);
                    return pair[0];
                }
                return p;
            });
            SetBuilderValue(builder, values, "name", "Database");
            SetBuilderValue(builder, values, "account", "User ID");
            SetBuilderValue(builder, values, "password", "Password");
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in values) {
                //builder[item.Key] = item.Value;
                SetBuilderValue(builder, item.Key, item.Value);
            }
            return CreateConnection(builder.ConnectionString);
        }
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public override IDataContext CreateDataContext(IConnection connection) {
            return new MySqlDataContext(connection);
        }

        /// <summary>
        /// 创建方言。
        /// </summary>
        /// <returns>返回方言对象。</returns>
        public override IDialect CreateDialect() {
            return new MySqlDialect();
        }

        #endregion

        #region methods

        #region GetType
        /// <summary>
        /// 获取 MySql.Data 类型
        /// </summary>
        /// <param name="typeFullName">类型全名。</param>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetType(string typeFullName, bool @throw = false) {
            if (string.IsNullOrEmpty(typeFullName))
                return null;
            System.Type type = _types[typeFullName];
            if (type == null) {
                string typeName = typeFullName + ", MySql.Data";
                type = FastWrapper.GetWarpperType(typeName, "MySql.Data.dll");
                if (type == null && @throw)
                    CommonException.ThrowTypeLoad(typeName);
                _types[typeFullName] = type;
            }
            return type;
        }
        #endregion

        #region GetConnectionType
        /// <summary>
        /// 获取 MySql.Data.MySqlClient.MySqlConnection 类型
        /// </summary>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetConnectionType(bool @throw=false) {
            return GetType("MySql.Data.MySqlClient.MySqlConnection", @throw);
        }
        #endregion
        #region GetDbType
        /// <summary>
        /// 获取 MySql.Data.MySqlClient.MySqlDbType 类型
        /// </summary>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetDbType(bool @throw=false) {
            return GetType("MySql.Data.MySqlClient.MySqlDbType", @throw);
        }
        #endregion

        #endregion

    }
}

