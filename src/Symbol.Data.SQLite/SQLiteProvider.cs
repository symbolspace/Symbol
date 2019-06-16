/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

[assembly: Symbol.Data.Provider("sqlite", typeof(Symbol.Data.SQLiteProvider))]
[assembly: Symbol.Data.Provider("sqlite3", typeof(Symbol.Data.SQLiteProvider))]
namespace Symbol.Data {

    /// <summary>
    /// SQLite数据库提供者(3.x)
    /// </summary>
    public class SQLiteProvider : AdoProvider {

        #region ctor
        /// <summary>
        /// 创建 SQLiteProvider 的实例。
        /// </summary>
        public SQLiteProvider() {
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
            return new AdoConnection(this, SQLite.SQLiteHelper.CreateConnection(connectionString), connectionString);
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
            System.Data.Common.DbConnectionStringBuilder builder = SQLite.SQLiteHelper.CreateConnectionStringBuilder();
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            SetBuilderValue(builder, values, "host", "Data Source");
            SetBuilderValue(builder, values, "file", "Data Source");
            SetBuilderValue(builder, values, "name", "Data Source");
            SetBuilderValue(builder, values, "memory", "Data Source", p => {
                if (TypeExtensions.Convert(p, false))
                    return ":memory:";
                return p;
            });
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
            return new SQLiteDataContext(connection);
        }

        /// <summary>
        /// 创建方言。
        /// </summary>
        /// <returns>返回方言对象。</returns>
        public override IDialect CreateDialect() {
            return new SQLiteDialect();
        }

        #endregion

        #region methods

        #region GetType
        /// <summary>
        /// 获取 SQLite.Data 类型
        /// </summary>
        /// <param name="typeFullName">类型全名。</param>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetType(string typeFullName, bool @throw = false) {
            var type= SQLite.SQLiteHelper.GetType(typeFullName);
            if (type == null && @throw)
                CommonException.ThrowTypeLoad(typeFullName);
            return type.Type;
        }
        #endregion

        #region GetConnectionType
        /// <summary>
        /// 获取 SQLite.Data.SQLiteClient.SQLiteConnection 类型
        /// </summary>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetConnectionType(bool @throw=false) {
            return GetType("System.Data.SQLite.SQLiteConnection", @throw);
        }
        #endregion

        #endregion

    }
}

