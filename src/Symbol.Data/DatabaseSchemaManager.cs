/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Reflection;

namespace Symbol.Data {
    /// <summary>
    /// 数据库架构管理。
    /// </summary>
    public class DatabaseSchemaManager : System.IDisposable {
        
        #region fields
        private System.Collections.Generic.List<DatabaseSchemaHandler> _list;
        private System.Collections.Generic.Dictionary<string, DatabaseSchemaHandler> _caches;
        private System.Collections.Generic.Dictionary<string, double> _classOrders;
        private System.Collections.Generic.Dictionary<string, AssemblyRef> _refs;
        private ILog _log;
        private bool _disposed;
        #endregion

        #region properties
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        public ILog Log {
            get { return _log; }
            set {
                _log = value ?? LogBase.Empty;
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建DatabaseSchemaManager实例。
        /// </summary>
        public DatabaseSchemaManager() {
            _list = new System.Collections.Generic.List<DatabaseSchemaHandler>();
            _caches = new System.Collections.Generic.Dictionary<string, DatabaseSchemaHandler>();
            _refs = new System.Collections.Generic.Dictionary<string, AssemblyRef>();
            _classOrders = new System.Collections.Generic.Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            _log = LogBase.Empty;
        }
        #endregion

        #region methods
        #region RegisterAppDomain
        /// <summary>
        /// 注册应用域（当前域）。
        /// </summary>
        public void RegisterAppDomain() {
            RegisterAppDomain(System.AppDomain.CurrentDomain);
        }
        /// <summary>
        /// 注册应用域（指定域）。
        /// </summary>
        /// <param name="appDomain">应用域对象。</param>
        public void RegisterAppDomain(System.AppDomain appDomain) {
            if (appDomain == null)
                return;
            foreach (System.Reflection.Assembly item in appDomain.GetAssemblies()) {
                RegisterAssembly(item);
            }
        }
        #endregion
        #region RegisterAssembly
        /// <summary>
        /// 注册程序集。
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<DatabaseSchemaHandler> RegisterAssembly(System.Reflection.Assembly assembly) {
            System.Collections.Generic.List<DatabaseSchemaHandler> list = new System.Collections.Generic.List<DatabaseSchemaHandler>();
            if (assembly == null 
                || assembly.FullName.StartsWith("System") 
                || assembly.FullName.StartsWith("mscorlib") 
                //|| assembly.IsDynamic
                ) {
                return list;
            }
            _log.Info("反射程序集：{0}", assembly.FullName);
            System.Type[] types;
            try {
                types = assembly.GetExportedTypes();
            } catch (System.Exception error) {
                _log.Warning("反射程序集失败：{0}\r\n{1}", assembly.FullName, LogBase.ExceptionToString(error));
                return list;
            }
            foreach (System.Type type in types) {
                DatabaseSchemaHandler item = RegisterType(type);
                if (item == null)
                    continue;
                list.Add(item);
            }
            return list;
        }
        #endregion
        #region RegisterType
        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DatabaseSchemaHandler RegisterType(System.Type type) {
            if (type == null || !type.IsClass || type.IsAbstract ||
                !TypeExtensions.IsInheritFrom(type, typeof(DatabaseSchemaHandler)))
                return null;
            DatabaseSchemaHandler handler = null;
            string key = type.FullName;
            if (_caches.TryGetValue(key, out handler)) {
                return handler;
            }
            string fullName = TypeExtensions.FullName2(type);
            try {
                _log.Info("创建实例：{0}", fullName);
                handler = (DatabaseSchemaHandler)FastWrapper.CreateInstance(type);
                _caches.Add(key, handler);
                _list.Add(handler);
                ClassOrder(handler.Attribute.TableName, handler.Attribute.Order);
                CacheRef(type.Assembly,handler);
                return handler;
            } catch(System.Exception error) {
                _log.Warning("创建实例失败：{0}\r\n{1}", fullName, LogBase.ExceptionToString(error));
                return null;
            }
        }
        #endregion
        #region CacheClassRef
        //void CacheClassRef(string className,double order,string[] references) {
        //    if (references != null && references.Length > 0) {
        //        foreach (string item in references) {
        //            order += ClassOrder(item) * 1000D;
        //        }
        //    }
        //    ClassOrder(className, order);
        //}
        double GetClassRef(string className, string[] references) {
            double order = 0D;
            if (references != null && references.Length > 0) {
                foreach (string item in references) {
                    int i = item.IndexOf('.');
                    if (i > -1) {
                        order += (TypeExtensions.Convert(item.Substring(i+1), ClassOrder(item)));
                    } else {
                        order += ClassOrder(item);
                    }
                }
                order *= 1000D;
                //order += ClassOrder(className);
            }
            return order;
        }
        double ClassOrder(string className) {
            double value;
            if (_classOrders.TryGetValue(className, out value))
                return value;
            return 0D;
        }
        void ClassOrder(string className, double order) {
            double value;
            if (_classOrders.TryGetValue(className, out value)) {
                if (value < order)
                    _classOrders[className] = order;
            } else {
                _classOrders.Add(className, value);
            }
        }
        #endregion
        #region CacheRef
        void CacheRef(System.Reflection.Assembly assembly, DatabaseSchemaHandler handler) {
            AssemblyRef item;
            string fullName = assembly.FullName;
            if (_refs.TryGetValue(fullName, out item)) {
                item.list.Add(handler);
                return;
            }
            item = new AssemblyRef() {
                fullName = assembly.FullName,
                refs = LinqHelper.ToArray(LinqHelper.Select(assembly.GetReferencedAssemblies(), p => p.FullName)),
                list = new System.Collections.Generic.List<DatabaseSchemaHandler>(),
            };
            item.list.Add(handler);
            _refs.Add(fullName, item);
        }
        #endregion
        #region Sort
        /// <summary>
        /// 排序并返回列表。
        /// </summary>
        public System.Collections.Generic.List<DatabaseSchemaHandler> Sort() {
            System.Collections.Generic.List<DatabaseSchemaHandler> list = new System.Collections.Generic.List<DatabaseSchemaHandler>();
            //foreach(AssemblyRef 
            System.Collections.Generic.Dictionary<string, SortEntry> refOrders = new System.Collections.Generic.Dictionary<string, SortEntry>();
            System.Collections.Generic.List<AssemblyRef> all = LinqHelper.ToList(_refs.Values);

            SortAction action = null;action = (p1, p2) => {
                foreach (string p11 in p1.refs) {
                    AssemblyRef p12;
                    if (_refs.TryGetValue(p11, out p12)) {
                        if (all.Remove(p12)) {
                            action(p12, p2 * 1000);
                        }
                    }
                }
                SortEntry p10 = new SortEntry() {
                    item = p1,
                    order = p2 * (refOrders.Count + 1)
                };
                refOrders.Add(p1.fullName, p10);
                return p10;
            };
            while (all.Count > 0) {
                AssemblyRef item = all[0];
                all.RemoveAt(0);
                action(item,-1);
            }
            SortFunc classOrderGetter = (p1) => p1.Attribute.Order + GetClassRef(p1.Attribute.TableName, p1.Attribute.References);

            foreach (SortEntry p1 in LinqHelper.OrderByDescending(refOrders.Values, p => p.order)) {
                list.AddRange(p1.item.SortList(classOrderGetter));
            }
            refOrders.Clear();
            refOrders = null;
            all = null;
            return list;
        }
        #endregion
        #region Process
        /// <summary>
        /// 数据库架构处理（内部开启事务，一旦有错误，自动回滚）。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Process(DatabaseSchemaContext context) {
            System.Collections.Generic.List<DatabaseSchemaHandler> list = Sort();
            context.Log.Info("数据库架构 {0}项", list.Count);
            context.Log.Info("数据库架构处理[{0}] 开始", context.DataContext.Connection.Database);
            bool success = true;
            int[] counts = new int[3];
            int t = Environment.TickCount;
            foreach (DatabaseSchemaHandler item in list.FindAll(p => p.Attribute.Type == DatabaseSchemaTypes.TableSpace)) {
                DatabaseSchemaProcessResults result = item.Process(context);
                counts[(int)result]++;
                if (result == DatabaseSchemaProcessResults.Error)
                    success = false;
            }
            using (context.DataContext.BeginTransaction()) {
                foreach (DatabaseSchemaHandler item in list.FindAll(p=>p.Attribute.Type!= DatabaseSchemaTypes.TableSpace)) {
                    DatabaseSchemaProcessResults result = item.Process(context);
                    counts[(int)result]++;
                    if (result == DatabaseSchemaProcessResults.Error)
                        success = false;
                }
                context.Log.Info("数据库架构处理[{0}] 成功：{1}，错误：{2}，忽略：{3}", context.DataContext.Connection.Database,
                    counts[0], counts[1], counts[2]);
                if (!success) {
                    context.Log.Info("数据库架构处理[{0}] 回滚", context.DataContext.Connection.Database);
                    context.DataContext.RollbackTransaction();
                } else {
                    context.Log.Info("数据库架构处理[{0}] 提交", context.DataContext.Connection.Database);
                    context.DataContext.CommitTransaction();
                }
            }
            t = Environment.TickCount - t;
            context.Log.Info("数据库架构处理[{0}] 完成，用时{1}ms", context.DataContext.Connection.Database, t);

            return success;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 
        /// </summary>
        ~DatabaseSchemaManager() {
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
            if (_refs != null) {
                _refs.Clear();
                _refs = null;
            }
            if (_list != null) {
                _list.Clear();
                _list = null;
            }
            if (_caches != null) {
                _caches.Clear();
                _caches = null;
            }
            if (_log != null) {
                _log.Dispose();
                _log = null;
            }
            GC.SuppressFinalize(this);
            _disposed = true;
        }
        #endregion

        #endregion

        #region types
        class AssemblyRef {
            public string fullName;
            public string[] refs;
            public System.Collections.Generic.List<DatabaseSchemaHandler> list;
            public System.Collections.Generic.IEnumerable<DatabaseSchemaHandler> SortList() {
                return LinqHelper.Where(list, p => p.Attribute.IsValid)
                                 .OrderBy(p => p.Attribute.Type)
                                 .ThenBy(p => p.Attribute.TableName)
                                 .ThenBy(p => p.Attribute.Order);
            }
            public System.Collections.Generic.IEnumerable<DatabaseSchemaHandler> SortList(SortFunc classOrderGetter) {
                return LinqHelper.Where(list, p => p.Attribute.IsValid)
                                 .OrderBy(p => p.Attribute.Type)
                                 .ThenBy(p => classOrderGetter(p))
                                 .ThenBy(p => p.Attribute.TableName);
            }
        }
        class SortEntry {
            public AssemblyRef item;
            public double order;
        }
        delegate SortEntry SortAction(AssemblyRef item, double order);
        delegate double SortFunc(DatabaseSchemaHandler item);
        #endregion
    }

}