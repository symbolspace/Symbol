/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Reflection;

namespace Symbol.Data {
   

    /// <summary>
    /// 数据库架构上下文。
    /// </summary>
    public class DatabaseSchemaContext :System.IDisposable {

        #region fields
        private Symbol.Data.IDataContext _dataContext;
        private string _databaseName;
        private ILog _log;
        private Symbol.Collections.Generic.NameValueCollection<object> _vars;
        private bool _disposed;
        #endregion

        #region properties
        /// <summary>
        /// 获取DataContext对象。
        /// </summary>
        public Symbol.Data.IDataContext DataContext { get { return _dataContext; } }
        /// <summary>
        /// 获取变量列表。
        /// </summary>
        public Symbol.Collections.Generic.NameValueCollection<object> Vars {
            get {
                return _vars;
            }
        }
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        public ILog Log {
            get { return _log; }
            set {
                _log = value ?? LogBase.Empty;
                _vars["log"] = _log;
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建DatabaseSchemaContext实例。
        /// </summary>
        /// <param name="dataContext">DataContext对象</param>
        public DatabaseSchemaContext(Symbol.Data.IDataContext dataContext) {
            _dataContext = dataContext;
            _databaseName = dataContext.Connection.DatabaseName;
            _log = LogBase.Empty;
            _vars = new Symbol.Collections.Generic.NameValueCollection<object>();
            _vars.Add("log", _log);
            _vars.Add("dataContext", _dataContext);
            _vars.Add("databaseName", _databaseName);
        }
        #endregion

        #region methods


        #region ChangeDatabase
        /// <summary>
        /// 变更当前数据库（默认）。
        /// </summary>
        public void ChangeDatabase() {
            ChangeDatabase(_databaseName);
        }
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        public void ChangeDatabase(string database) {
            if (string.IsNullOrEmpty(_databaseName) || _dataContext == null)
                return;
            _dataContext.ChangeDatabase(database);
            //if (_dataContext.Connection.State != System.Data.ConnectionState.Open)
            //    _dataContext.Connection.Open();
            //_dataContext.Connection.ChangeDatabase(database);
        }
        #endregion

        #region ExecuteBlockQuery
        /// <summary>
        /// 批量执行命令
        /// </summary>
        /// <param name="command">命令（SQL）。</param>
        /// <param name="mulitFlag">多段命令分隔符。</param>
        /// <param name="changeDatabase">切换数据库标志。</param>
        public void ExecuteBlockQuery(string command, string mulitFlag = "GO", string changeDatabase = "use ") {
            if (string.IsNullOrEmpty(command))
                return;
            if (string.IsNullOrEmpty(mulitFlag))
                mulitFlag = "GO";
            if (_dataContext == null)
                return;
            _dataContext.ExecuteBlockQuery(command, mulitFlag, changeDatabase);
            //string[] lines = command.Split(new string[] { mulitFlag }, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < lines.Length; i++) {
            //    if (string.IsNullOrEmpty(lines[i]))
            //        continue;
            //    if (lines[i].StartsWith(changeDatabase, StringComparison.OrdinalIgnoreCase)) {
            //        OnExecuteNonQuery_ChangeDatabase(lines[i], changeDatabase);
            //        _dataContext.ExecuteNonQuery("--" + lines[i]);
            //    } else {
            //        _dataContext.ExecuteNonQuery(lines[i]);
            //    }
            //}
        }
        //void OnExecuteNonQuery_ChangeDatabase(string line, string changeDatabase) {
        //    int i = line.IndexOf(changeDatabase, StringComparison.OrdinalIgnoreCase);
        //    i += changeDatabase.Length;
        //    int j = line.IndexOf(';', i);
        //    if (j == -1) {
        //        j = line.IndexOf(']', i);
        //    }
        //    if (j == -1) {
        //        j = line.IndexOf('"', i + 1);
        //    }
        //    if (j == -1) {
        //        j = line.IndexOf('\r', i);
        //    }
        //    if (j == -1) {
        //        j = line.IndexOf('\n', i);
        //    }
        //    if (j == -1) {
        //        j = line.Length;
        //        //return;
        //    }
        //    string name = line.Substring(i, j - i);
        //    name = name.Trim(' ', ';', '[', ']', '"', '\r', '\n').Trim();
        //    if (string.Equals(name, "{db.name}", StringComparison.OrdinalIgnoreCase))
        //        name = _databaseName;
        //    ChangeDatabase(name);
        //}
        #endregion

        #region Dispose
        /// <summary>
        /// 
        /// </summary>
        ~DatabaseSchemaContext() {
            Dispose(false);
        }
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing)
                return;
            if (_disposed)
                return;
            if (_vars != null) {
                _vars.Clear();
                _vars = null;
            }
            if (_dataContext != null) {
                _dataContext.Dispose();
                _dataContext = null;
            }
            _databaseName = null;
            if (_log != null) {
                _log.Dispose();
                _log = null;
            }
            GC.SuppressFinalize(this);
            _disposed = true;
        }
        #endregion

        #endregion

    }

}