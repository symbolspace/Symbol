using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Symbol.Logger
{
    /// <summary>
    /// 调试时间。
    /// </summary>
    public class DebugTime
    {
#if NET35
        private System.Collections.Generic.IDictionary<string, DebugTimeEntry> _list;
#else
        private System.Collections.Concurrent.ConcurrentDictionary<string, DebugTimeEntry> _list;
#endif
        private string _name;

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        /// <param name="name">日志名称。</param>
        public DebugTime(string name)
        {
#if NET35
            _list = new System.Collections.Generic.Dictionary<string, DebugTimeEntry>();
#else
            _list = new System.Collections.Concurrent.ConcurrentDictionary<string, DebugTimeEntry>();
#endif
            _name = name;
            Begin(_name + ".Total");
        }

        /// <summary>
        /// 清空所有记录。
        /// </summary>
        /// <returns></returns>
        public DebugTime Clear()
        {
            _list.Clear();
            return this;
        }

        /// <summary>
        /// 开始记时。
        /// </summary>
        /// <param name="name">标签名称。</param>
        /// <returns></returns>
        public DebugTime Begin(string name)
        {
            if (string.IsNullOrEmpty(name))
                return this;

#if NET35
            if(!_list.ContainsKey(name))
                _list.SetValue(name, CreateEntry(name));
#else
            _list.GetOrAdd(name, CreateEntry);
#endif
            return this;
        }
        /// <summary>
        /// 结束计时。
        /// </summary>
        /// <param name="name">标签名称。</param>
        /// <returns></returns>
        public DebugTime End(string name)
        {
            if (string.IsNullOrEmpty(name))
                return this;
#if NET35
            if (!_list.ContainsKey(name))
                _list.SetValue(name, CreateEntry(name));
            var entry= _list.GetValue(name);
#else
            var entry = _list.GetOrAdd(name, CreateEntry);
#endif
            entry.EndTime = DateTime.Now;

            return this;
        }
        /// <summary>
        /// 结束全部。
        /// </summary>
        /// <returns></returns>
        public DebugTime EndAll()
        {
            if (_list.Count == 0)
                return this;

            foreach (var key in LinqHelper.ToArray(_list.Keys))
            {
                if (_list.TryGetValue(key, out DebugTimeEntry entry) && entry.EndTime == null)
                {
                    entry.EndTime = DateTime.Now;
                }
            }
            End(_name + ".Total");

            return this;
        }

        /// <summary>
        /// 块操作。
        /// </summary>
        /// <param name="name">标签名称。</param>
        /// <param name="action">执行逻辑。</param>
        /// <returns></returns>
        public DebugTime Block(string name, Action action)
        {
            Begin(name);
            action?.Invoke();
            End(name);

            return this;
        }
        /// <summary>
        /// 块操作。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="name">标签名称。</param>
        /// <param name="func">执行逻辑。</param>
        /// <returns></returns>
        public T Block<T>(string name, Func<T> func)
        {
            Begin(name);
            T result = func.Invoke();
            End(name);

            return result;
        }

        /// <summary>
        /// 打印所有记录。
        /// </summary>
        /// <returns></returns>
        public string PrintAll()
        {
            var builder = new StringBuilder();
            builder.AppendLine()
                   .AppendLine(string.Empty.PadRight(50, '-'))
                   .Append("    ").Append(_name).AppendLine()
                   .AppendFormat("    Print Time {0:yyyy-MM-dd HH:mm:ss.fff}", DateTime.Now).AppendLine()
                   .AppendLine(string.Empty.PadRight(50, '-'))
                   .AppendLine("    Time\t\t\tBeginTime\t\t\t\tEndTime\t\t\t\t\tName")
                   .AppendLine(string.Empty.PadRight(100, '='));
            foreach (var entry in LinqHelper.OrderByDescending(_list.Values, p => p.Time))
            {
                builder.AppendFormat("    =>{0:hh\\:mm\\:ss\\.fff}\t{1:yyyy-MM-dd HH:mm:ss.fff}\t{2:yyyy-MM-dd HH:mm:ss.fff}\t{3}", entry.Time, entry.BeginTime, entry.EndTime ?? DateTime.Now, entry.Name).AppendLine();
            }
            builder.AppendLine();

            return builder.ToString();
        }
        /// <summary>
        /// 打印所有记录到文件。
        /// </summary>
        /// <returns></returns>
        public DebugTime PrintAllToFile()
        {
            var flag = Debugger.IsAttached ? "f5" : "exe";
            string file = $"{_name}-debugtime-{flag}.log";
            File.AppendAllText(file, PrintAll(), Encoding.UTF8);
            return this;
        }

        /// <summary>
        /// 打印所有记录到Logger。
        /// </summary>
        /// <returns></returns>
        public DebugTime PrintAllToLogger()
        {
            return PrintAllToLogger(LoggerProvider.Instance.GetLogger<DebugTime>());
        }
        /// <summary>
        /// 打印所有记录到Logger。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        /// <returns></returns>
        public DebugTime PrintAllToLogger(ILogger logger)
        {
            var content = PrintAll();
            if(logger!=null)
                LoggerExtensions.Debug(logger, content);
            return this;
        }

        /// <summary>
        /// 输出所有日志。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return PrintAll();
        }

        DebugTimeEntry CreateEntry(string name)
        {
            return new DebugTimeEntry()
            {
                Name = name,
                BeginTime = DateTime.Now,
            };
        }


        /// <summary>
        /// 调试时间项。
        /// </summary>
        [DebuggerDisplay("{Name}\t{Time}ms")]
        public class DebugTimeEntry
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 开始时间
            /// </summary>
            public DateTime BeginTime { get; set; }
            /// <summary>
            /// 结束时间
            /// </summary>
            public DateTime? EndTime { get; set; }
            /// <summary>
            /// 耗时
            /// </summary>
            public TimeSpan Time
            {
                get
                {
                    return (EndTime ?? DateTime.Now) - BeginTime;
                }
            }
        }
    }
}