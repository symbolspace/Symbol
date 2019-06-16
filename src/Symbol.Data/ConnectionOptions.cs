/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 连接参数。
    /// </summary>
    public class ConnectionOptions {

        #region fields
        private Symbol.Collections.Generic.NameValueCollection<object> _vars;
        private string _host;
        private int _port;
        private string _name;
        private string _account;
        private string _password;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置连接主机地址。
        /// </summary>
        public string Host {
            get { return _host; }
            set {
                _host = value;
                _vars["Host"] = value;
            }
        }
        /// <summary>
        /// 获取或设置连接端口。
        /// </summary>
        public int Port {
            get { return _port; }
            set {
                _port = value;
                _vars["Port"] = value;
            }
        }
        /// <summary>
        /// 获取或设置数据库名称。
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                _name = value;
                _vars["Name"] = value;
            }
        }
        /// <summary>
        /// 获取或设置登录账号。
        /// </summary>
        public string Account {
            get { return _account; }
            set {
                _account = value;
                _vars["Account"] = value;
            }
        }
        /// <summary>
        /// 获取或设置登录密码。
        /// </summary>
        public string Password {
            get { return _password; }
            set {
                _password = value;
                _vars["Password"] = value;
            }
        }
        /// <summary>
        /// 获取或设置指定键对应的值。
        /// </summary>
        /// <param name="key">键，null或empty忽略。</param>
        /// <returns>返回指定键对应的值。</returns>
        public object this[string key] {
            get { return Get(key); }
            set { Set(key, value); }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建ConnectionOptions实例。
        /// </summary>
        public ConnectionOptions() {
            _vars = new Collections.Generic.NameValueCollection<object>();
        }
        #endregion

        #region methods

        #region Get
        /// <summary>
        /// 获取指定键对应的值。
        /// </summary>
        /// <param name="key">键，null或empty忽略。</param>
        /// <returns>返回指定键对应的值。</returns>
        public object Get(string key) {
            if (string.IsNullOrEmpty(key))
                return null;
            return _vars[key];
        }
        #endregion
        #region Set
        /// <summary>
        /// 设置指定键对应的值。
        /// </summary>
        /// <param name="key">键，null或empty忽略。</param>
        /// <param name="value">值。</param>
        public void Set(string key, object value) {
            if (string.IsNullOrEmpty(key))
                return;
            switch (key.ToLower()) {
                case "host":
                    Host = TypeExtensions.Convert<string>(value);
                    break;
                case "port":
                    Port = TypeExtensions.Convert<int>(value, 0);
                    break;
                case "name":
                    Name = TypeExtensions.Convert<string>(value);
                    break;
                case "account":
                    Account = TypeExtensions.Convert<string>(value);
                    break;
                case "password":
                    Password = TypeExtensions.Convert<string>(value);
                    break;
                default:
                    _vars[key] = value;
                    break;
            }
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 将所有键值输出为一个对象。
        /// </summary>
        /// <returns>返回所有键值。</returns>
        public object ToObject() {
            return _vars;
        }
        #endregion

        #endregion

    }

}