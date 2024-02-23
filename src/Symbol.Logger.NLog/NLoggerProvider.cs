using Symbol;
using Symbol.Logger;
using Symbol.Logger.NLog;

using NLogManager = NLog.LogManager;

[assembly: TypeImplementMap(typeof(ILoggerProvider), typeof(NLoggerProvider), "NLog")]

namespace Symbol.Logger.NLog
{
    /// <summary>
    /// 基于NLog的日志提供者
    /// </summary>
    class NLoggerProvider : LoggerProvider
    {
        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        public override string Name { get { return "NLog"; } }

        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="name">日志名称。</param>
        /// <returns>返回日志对象。</returns>
        public override ILogger GetLogger(string name)
        {
            Throw.CheckArgumentNull(name, nameof(name));
            return new NLogger(NLogManager.GetLogger(name));
        }
    }
}