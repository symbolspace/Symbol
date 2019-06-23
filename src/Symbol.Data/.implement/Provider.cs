/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.Data {

    /// <summary>
    /// 数据库提供者基类
    /// </summary>
    public abstract class Provider : IProvider {

        #region fields
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, IProvider> _list_cache;

        private IDialect _dialect;
        #endregion

        #region properties

        /// <summary>
        /// 获取方言对象。
        /// </summary>
        public IDialect Dialect { get { return _dialect; } }

        #endregion

        #region cctor
        static Provider() {
            _list_cache = new System.Collections.Concurrent.ConcurrentDictionary<string, IProvider>(System.StringComparer.OrdinalIgnoreCase);
            
            foreach (string file in System.IO.FileHelper.Scan("Symbol.Data.*.dll", AppHelper.AppPath)) {
                try
                {
                    //var deps = Microsoft.Extensions.DependencyModel.DependencyContext;

#if netcore
                    var assemblyName = System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(file);
                    var assembly = System.Reflection.Assembly.Load(assemblyName);
#else
                    var assembly = System.Reflection.Assembly.LoadFrom(file);
#endif
                    var attributes = AttributeExtensions.GetCustomAttributes<ProviderAttribute>(assembly, true);
                    foreach (var item in attributes)
                    {
                        if (string.IsNullOrEmpty(item.Name) || item.Type == null)
                            continue;
                        _list_cache.TryAdd(item.Name, FastWrapper.CreateInstance<IProvider>(item.Type, new object[0]));
                    }
                }
                catch (System.Exception error)
                {
                    System.Console.WriteLine(file);
                    System.Console.WriteLine(error);
                }
            }
#if netcore
            {
                var deps = Microsoft.Extensions.DependencyModel.DependencyContext.Default;
                foreach (var p in deps.CompileLibraries) {
                    if (!p.Name.StartsWith("Symbol.Data.", System.StringComparison.OrdinalIgnoreCase))
                        continue;
                    var assemblyName = new System.Reflection.AssemblyName($"{p.Name}, Version={p.Version}");
                    var assembly = System.Reflection.Assembly.Load(assemblyName);
                    Register(assembly);
                }
            }
#endif
            //var type = FastWrapper.GetWarpperType("Symbol.Data.SqlServerProvider, Symbol.Data.SqlServer");
            //Console.WriteLine(FastWrapper.GetWarpperType("Symbol.Data.SqlServerProvider, Symbol.Data.SqlServer"));
            //Register(type.Assembly);
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建DatabaseProvider实例。
        /// </summary>
        public Provider() {
            _dialect = CreateDialect();
        }
        #endregion

        #region methods

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="assembly">程序集实例。</param>
        public static void Register(System.Reflection.Assembly assembly) {
            if (assembly == null)
                return;
            var attributes = AttributeExtensions.GetCustomAttributes<ProviderAttribute>(assembly, true);
            foreach (var item in attributes) {
                if (string.IsNullOrEmpty(item.Name) || item.Type == null)
                    continue;
                _list_cache.TryAdd(item.Name, FastWrapper.CreateInstance<IProvider>(item.Type, new object[0]));
            }
        }

        /// <summary>
        /// 获取指定提供者。
        /// </summary>
        /// <param name="name">提供者名称，为空或空字符串，则返回空。</param>
        /// <returns>返回提供者实例，若名称不存在，则返回空。</returns>
        public static IProvider Get(string name) {
            if (!string.IsNullOrEmpty(name) && _list_cache.TryGetValue(name, out IProvider result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public abstract IConnection CreateConnection(string connectionString);
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数，兼容string/object/ConnectionOptions。</param>
        /// <returns>返回数据库连接。</returns>
        public abstract IConnection CreateConnection(object connectionOptions);
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public virtual IConnection CreateConnection(ConnectionOptions connectionOptions) {
            if (connectionOptions == null)
                return null;
            return CreateConnection(connectionOptions.ToObject());
        }

        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public abstract IDataContext CreateDataContext(IConnection connection);

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
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据上下文。</returns>
        public virtual IDataContext CreateDataContext(ConnectionOptions connectionOptions) {
            return CreateDataContext(CreateConnection(connectionOptions));
        }
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="type">数据库提供者名称。</param>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据上下文。</returns>
        public static IDataContext CreateDataContext(string type, object connectionOptions) {
            CommonException.CheckArgumentNull(type, "type");
            var provider = Get(type);
            if (provider == null)
                CommonException.ThrowNotSupported($"未找到数据提供者“{type}”");

            return provider.CreateDataContext(connectionOptions);
        }
        /// <summary>
        /// 创建方言。
        /// </summary>
        /// <returns>返回方言对象。</returns>
        public abstract IDialect CreateDialect();

        #endregion
    }

}
