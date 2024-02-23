using System;
using Symbol;
using Symbol.Logger;
using Symbol.Logger.Serilog;

using SerilogLog = Serilog.Log;

[assembly: TypeImplementMap(typeof(ILoggerProvider), typeof(SerilogLoggerProvider), "Serilog")]

namespace Symbol.Logger.Serilog
{
    /// <summary>
    /// 基于Serilog的日志提供者
    /// </summary>
    class SerilogLoggerProvider : LoggerProvider
    {
        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string Name { get { return "Serilog"; } }

        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="name">日志名称。</param>
        /// <returns>返回日志对象。</returns>
        public override ILogger GetLogger(string name)
        {
            Throw.CheckArgumentNull(name, nameof(name));
            return new SerilogLogger(SerilogLog.ForContext("SourceContext", name));
        }

        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <returns>返回日志对象。</returns>
        public override ILogger GetLogger<T>()
        {
            return new SerilogLogger(SerilogLog.ForContext<T>());
        }

        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="type">类型定义。</param>
        /// <returns>返回日志对象。</returns>
        public override ILogger GetLogger(Type type)
        {
            Throw.CheckArgumentNull(type, nameof(type));
            return new SerilogLogger(SerilogLog.ForContext(type));
        }
    }
}