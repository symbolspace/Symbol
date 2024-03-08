using System;

namespace Symbol.Logger {
    class ConsoleLoggerProvider : LoggerProvider{
#if NET20 || NET35
        private System.Collections.Generic.IDictionary<string, ConsoleLogger> _list;
#else
        private System.Collections.Concurrent.ConcurrentDictionary<string, ConsoleLogger> _list;
#endif

        public override string Name { get { return "Console"; } }

        public ConsoleLoggerProvider()
        {
#if NET20 || NET35
            _list = new System.Collections.Generic.Dictionary<string, ConsoleLogger>();
#else
            _list = new System.Collections.Concurrent.ConcurrentDictionary<string, ConsoleLogger>();
#endif
        }
        public override ILogger GetLogger(string name) {
#if NET20 || NET35
            ConsoleLogger result = IDictionaryExtensions.GetValue(_list, name);
            if(result == null){
                lock(_list) {
                    result = IDictionaryExtensions.GetValue(_list, name);
                    if (result == null) {
                        result= CreateLogger(name);
                        IDictionaryExtensions.SetValue(_list, name, result);
                    }
                }
            }
            return result;
#else
            return _list.GetOrAdd(name, CreateLogger);
#endif
        }
        ConsoleLogger CreateLogger(string name) {
            return new ConsoleLogger(name);
        }

        class ConsoleLogger : ILogger {
            private string _name;
            public ConsoleLogger(string name) {
                _name = name;
            }
            public bool IsEnabled(LogLevel logLevel) {
                return true;
            }

            public void Log(LogLevel logLevel, string message, params object[] args) {
                Console.WriteLine($"[{_name}|{EnumExtensions.ToName(logLevel)}]{string.Format(message, args)}");
            }

            public void Log(LogLevel logLevel, Exception exception, string message, params object[] args) {
                Console.WriteLine($"[{_name}|{EnumExtensions.ToName(logLevel)}]{string.Format(message, args)}");
                if(exception!=null)
                    Console.WriteLine(exception);
            }
        }

    }
}