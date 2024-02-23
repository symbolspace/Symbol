using System;
using System.Threading;

namespace Symbol.Logger
{
    /// <summary>
    /// 信息：日志属性。
    /// </summary>
    public class LoggerProperty
    {
        private string _name;
        private int _count;
        private int _total;
        private int _avg;
        private int _lastTick;

        /// <summary>
        /// 获取属性名称。
        /// </summary>
        public string Name { get { return _name; } }


        /// <summary>
        /// 获取数量。
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// 获取总时间，单位：毫秒。
        /// </summary>
        public int Total
        {
            get
            {
                return _total;
            }
        }

        /// <summary>
        /// 创建对象实例。
        /// </summary>
        /// <param name="name">属性名称。</param>
        public LoggerProperty(string name)
        {
            _name = name;
            _count = 0;
            _total = 0;
            _avg = 0;
        }

        /// <summary>
        /// 开始。
        /// </summary>
        public void Begin()
        {
            Interlocked.Exchange(ref _lastTick, Environment.TickCount);
        }
        /// <summary>
        /// 结束。
        /// </summary>
        /// <returns>返回消耗时间，单位：毫秒。</returns>
        public int End()
        {
            var tick = Environment.TickCount;
            var lastTick = Interlocked.Exchange(ref _lastTick, tick);
            tick -= lastTick;
            int total = Interlocked.Add(ref _total, tick);
            int count = Interlocked.Increment(ref _count);
            int avg = total / count;
            Interlocked.Exchange(ref _avg, avg);
            return tick;
        }

    }
}