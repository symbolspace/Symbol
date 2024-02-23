using System;

namespace Symbol.Logger
{
    /// <summary>
    /// 抽象：日志对象提供者。
    /// </summary>
    public abstract class LoggerProvider : ILoggerProvider
    {
        private static readonly ILoggerProvider[] _instances = new ILoggerProvider[]
        {
            EmptyLoggerProvider.EmptyInstance,
        };

        static LoggerProvider()
        {
            TypeImplementMap.Scan("*.Logger.*.dll");
        }

        /// <summary>
        /// 获取全局静态实例。
        /// </summary>
        public static ILoggerProvider Instance
        {
            get { return _instances[0]; }
            set { _instances[0] = value ?? EmptyLoggerProvider.EmptyInstance; }
        }

        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="name">日志名称。</param>
        /// <returns>返回日志对象。</returns>
        public abstract ILogger GetLogger(string name);
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="type">类型定义。</param>
        /// <returns>返回日志对象。</returns>
        public virtual ILogger GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <returns>返回日志对象。</returns>
        public virtual ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T).FullName);
        }

        /// <summary>
        /// 设置提供者。
        /// </summary>
        /// <param name="name">提供者名称。</param>
        public static void SetProvider(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (name == Instance.Name)
                return;
            Instance = TypeImplementMap.GetTargetSingleton<ILoggerProvider>(name);
        }

        class EmptyLoggerProvider : LoggerProvider
        {
            public static readonly ILoggerProvider EmptyInstance = new EmptyLoggerProvider();

            public override string Name { get { return "empty"; } }

            public override ILogger GetLogger(string name)
            {
                return EmptyLogger.EmptyInstance;
            }
        }
        class EmptyLogger : ILogger
        {
            public static readonly ILogger EmptyInstance = new EmptyLogger();
            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log(LogLevel logLevel, string message, params object[] args)
            {

            }

            public void Log(LogLevel logLevel, Exception exception, string message, params object[] args)
            {

            }
        }
    }

}