/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;


/// <summary>
/// 日志存储接口
/// </summary>
public interface ILog : System.IDisposable {
    /// <summary>
    /// 日志名称
    /// </summary>
    string Name { get; }
    /// <summary>
    /// 输出一个空白行。
    /// </summary>
    /// <param name="level">级别，为空时将默认为{Empty}</param>
    void WriteLine(string level);
    /// <summary>
    /// 输出一行内容。
    /// </summary>
    /// <param name="level">级别，为空时将默认为{Empty}</param>
    /// <param name="message">消息</param>
    void WriteLine(string level, string message);
    /// <summary>
    /// 输出一行内容。
    /// </summary>
    /// <param name="level">级别，为空时将默认为{Empty}</param>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    void WriteLine(string level, string format, params object[] args);
    /// <summary>
    /// 输出一行内容（级别为{Debug}）。
    /// </summary>
    /// <param name="message">消息</param>
    void Debug(string message);
    /// <summary>
    /// 输出一行内容（级别为{Debug}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    void Debug(string format, params object[] args);
    /// <summary>
    /// 输出一行内容（级别为{Info}）。
    /// </summary>
    /// <param name="message">消息</param>
    void Info(string message);
    /// <summary>
    /// 输出一行内容（级别为{Info}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    void Info(string format, params object[] args);
    /// <summary>
    /// 输出一行内容（级别为{Warning}）。
    /// </summary>
    /// <param name="message">消息</param>
    void Warning(string message);
    /// <summary>
    /// 输出一行内容（级别为{Warning}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    void Warning(string format, params object[] args);
    /// <summary>
    /// 输出一行内容（级别为{Error}）。
    /// </summary>
    /// <param name="message">消息</param>
    void Error(string message);
    /// <summary>
    /// 输出一行内容（级别为{Error}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    void Error(string format, params object[] args);
    /// <summary>
    /// 输出一行内容（级别为{Error}）。
    /// </summary>
    /// <param name="error">异常对象</param>
    void Error(Exception error);
}

/// <summary>
/// 日志存储基类
/// </summary>
public class LogBase : ILog {

    #region fields
    private string _name;
    private Symbol.Collections.Generic.HashSet<string> _levels;
    private static readonly ILog _empty;
    /// <summary>
    /// 是否已释放。
    /// </summary>
    protected bool _disposed;
    #endregion

    #region properties
    /// <summary>
    /// 获取默认日志对象。
    /// </summary>
    public static ILog Empty { get { return _empty; } }
    /// <summary>
    /// 获取名称。
    /// </summary>
    public string Name {
        get { return _name; }
        private set {
            Symbol.CommonException.CheckArgumentNull(value, "名称");
            if (value.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) > -1)
                Symbol.CommonException.ThrowArgument("名称包含无效字符");
            _name = value;
        }
    }
    /// <summary>
    /// 获取日志级别列表（默认为空列表，表示不过滤）。
    /// </summary>
    public Symbol.Collections.Generic.HashSet<string> Levels {
        get { return _levels; }
    }
    #endregion

    #region cctor
    static LogBase() {
        LogBase empty = new LogBase("Empty");
        empty._levels.Add("!");
        _empty = empty;
    }
    #endregion

    #region ctor
    /// <summary>
    /// 创建LogRepository实例。
    /// </summary>
    /// <param name="name">名称，不能包含无效文件名字符。</param>
    public LogBase(string name) {
        Name = name;
        _levels = new Symbol.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region methods

    #region OnWriteLine
    /// <summary>
    /// 输出一个一行信息。
    /// </summary>
    /// <param name="level">级别</param>
    /// <param name="message">消息</param>
    protected virtual void OnWriteLine(string level, string message) {
    }
    #endregion

    #region WriteLine
    /// <summary>
    /// 输出一个空白行。
    /// </summary>
    /// <param name="level">级别，为空时将默认为{Empty}</param>
    public void WriteLine(string level) {
        WriteLine(level, "");
    }
    /// <summary>
    /// 输出一行内容。
    /// </summary>
    /// <param name="level">级别，为空时将默认为{Empty}</param>
    /// <param name="message">消息</param>
    public virtual void WriteLine(string level, string message) {
        if (string.IsNullOrEmpty(level))
            level = "{Empty}";
        if (_levels.Count == 0 || _levels.Contains(level)) {
            OnWriteLine(level, message);
        }
    }
    /// <summary>
    /// 输出一行内容。
    /// </summary>
    /// <param name="level">级别，为空时将默认为{Empty}</param>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    public void WriteLine(string level, string format, params object[] args) {
        if (args != null && args.Length > 0)
            format = string.Format(format, args);
        WriteLine(level, format);
    }
    #endregion
    #region Debug
    /// <summary>
    /// 输出一行内容（级别为{Debug}）。
    /// </summary>
    /// <param name="message">消息</param>
    public void Debug(string message) {
        WriteLine("{Debug}", message);
    }
    /// <summary>
    /// 输出一行内容（级别为{Debug}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    public void Debug(string format, params object[] args) {
        WriteLine("{Debug}", format, args);
    }
    #endregion
    #region Info
    /// <summary>
    /// 输出一行内容（级别为{Info}）。
    /// </summary>
    /// <param name="message">消息</param>
    public void Info(string message) {
        WriteLine("{Info}", message);
    }
    /// <summary>
    /// 输出一行内容（级别为{Info}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    public void Info(string format, params object[] args) {
        WriteLine("{Info}", format, args);
    }
    #endregion
    #region Warning
    /// <summary>
    /// 输出一行内容（级别为{Warning}）。
    /// </summary>
    /// <param name="message">消息</param>
    public void Warning(string message) {
        WriteLine("{Warning}", message);
    }
    /// <summary>
    /// 输出一行内容（级别为{Warning}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    public void Warning(string format, params object[] args) {
        WriteLine("{Warning}", format, args);
    }
    #endregion
    #region Error
    /// <summary>
    /// 输出一行内容（级别为{Error}）。
    /// </summary>
    /// <param name="message">消息</param>
    public void Error(string message) {
        WriteLine("{Error}", message);
    }
    /// <summary>
    /// 输出一行内容（级别为{Error}）。
    /// </summary>
    /// <param name="format">带格式串的消息</param>
    /// <param name="args">用于格式串的参数列表。</param>
    public void Error(string format, params object[] args) {
        WriteLine("{Error}", format, args);
    }
    /// <summary>
    /// 输出一行内容（级别为{Error}）。
    /// </summary>
    /// <param name="error">异常对象</param>
    public void Error(Exception error) {
        Error(ExceptionToString(error));
    }
    #endregion

    #region GetName
    /// <summary>
    /// 获取名称（类型）。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetName(System.Type type) {
        if (type == null)
            return "";
        return TypeExtensions.FullName2(type).Replace('<', '[').Replace('>', ']').Replace('+', '.');
    }
    /// <summary>
    /// 获取名称（成员）。
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static string GetName(System.Reflection.MemberInfo memberInfo) {
        if (memberInfo == null)
            return "";
        return GetName(memberInfo.DeclaringType) + "." + memberInfo.Name;
    }
    #endregion

    #region Dispose
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
        if (this == _empty)
            return;
        if (!disposing)
            return;
        if (_disposed)
            return;
        _name = null;
        if (_levels != null) {
            _levels.Clear();
            _levels = null;
        }
        GC.SuppressFinalize(this);
        _disposed = true;
    }
    #endregion

    #region ExceptionToString
    /// <summary>
    /// 将异常转换为文本（Message、StackTrace、Data、InnerException）。
    /// </summary>
    /// <param name="error">当前异常。</param>
    /// <returns>返回详细的信息。</returns>
    public static string ExceptionToString(Exception error) {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        ExceptionToString(error, builder, 0);
        return builder.ToString();
    }
    /// <summary>
    /// 将异常转换为文本。
    /// </summary>
    /// <param name="error">当前异常。</param>
    /// <param name="builder">输出的文本缓冲区。</param>
    /// <param name="layer">层数，会因为层数会自动在左边追加缩近。</param>
    public static void ExceptionToString(Exception error, System.Text.StringBuilder builder, int layer) {
        if (error == null)
            return;
        string left = string.Empty;
        if (layer > 0)
            left = string.Empty.PadRight(layer * 2, '　');
        builder.Append(left).AppendLine(TypeExtensions.FullName2(error.GetType()));
        builder.Append(left).Append("Message:").AppendLine(error.Message);
        if (error.StackTrace != null)
            builder.Append(left).AppendLine("StackTrace:")
              .Append(left).Append(left).AppendLine(error.StackTrace.Replace("\r\n", left + "　　\r\n"));

        if (error.Data != null && error.Data.Count > 0) {
            builder.Append(left).AppendLine("Data:");
            int index = -1;
            foreach (object key in error.Data.Keys) {
                index++;
                object value = error.Data[key];
                builder.Append(left).Append(left)
                  .Append(index).Append(':').Append(key).Append('=')
                  .Append(error.Data[key]).AppendLine();
            }
        }
        foreach (System.Reflection.PropertyInfo propertyInfo in error.GetType().GetProperties()) {
            if (propertyInfo.DeclaringType == typeof(Exception)) {
                continue;
            }
            try {
                object value = propertyInfo.GetValue(error, new object[0]);
                builder.Append(left).Append(propertyInfo.Name).Append(':').AppendLine(value == null ? "" : value.ToString());
            } catch { }
        }
        if (error.InnerException != null) {
            builder.Append(left).AppendLine(string.Empty.PadRight(100, '-'));
            ExceptionToString(error.InnerException, builder, layer + 1);
        }
    }
    #endregion

    #endregion
}
/// <summary>
/// TextWriter日志存储，自动按天拆分文件。
/// </summary>
public class TextWriterLog : LogBase {

    #region fields
    private System.IO.TextWriter _writer;
    #endregion

    #region ctor
    /// <summary>
    /// 创建TextWriterLog实例（当前程序目录）。
    /// </summary>
    /// <param name="name">名称，不能包含无效文件名字符。</param>
    /// <param name="writer"></param>
    public TextWriterLog(string name, System.IO.TextWriter writer) : 
        base(name) {
        _writer = writer;
    }
    
    #endregion


    #region methods

    #region OnWriteLine
    /// <summary>
    /// 输出一个一行信息（时间 level&gt;消息）。
    /// </summary>
    /// <param name="level">级别</param>
    /// <param name="message">消息</param>
    protected override void OnWriteLine(string level, string message) {
        if (_writer == null)
            return;
        _writer.WriteLine("{0} {1}>{2}", System.DateTime.Now.ToString("HH:mm:ss.fffffff"), level, message);
        _writer.Flush();
    }
    #endregion

    #region Dispose
    /// <summary>
    /// 释放占用的资源。
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing) {
        if (!_disposed && disposing) {
            if (_writer != null) {
                _writer.Dispose();
                _writer = null;
            }
        }
        base.Dispose(disposing);
    }
    #endregion

    #endregion
}
/// <summary>
/// 控制台日志存储，自动按天拆分文件。
/// </summary>
public class ConsoleLog : LogBase {

    #region fields
    #endregion

    #region ctor
    /// <summary>
    /// 创建ConsoleLog实例（当前程序目录）。
    /// </summary>
    /// <param name="name">名称，不能包含无效文件名字符。</param>
    public ConsoleLog(string name) :
        base(name) {
    }

    #endregion


    #region methods

    #region ColorBlock

    #endregion
    #region OnWriteLine
    /// <summary>
    /// 输出一个一行信息（时间 level&gt;消息）。
    /// </summary>
    /// <param name="level">级别</param>
    /// <param name="message">消息</param>
    protected override void OnWriteLine(string level, string message) {
        System.ConsoleColor foreColor = ConsoleColor.White;
        System.ConsoleColor backColor = ConsoleColor.Black;
        switch (level.ToLower()) {
            case "{emtpy}":
                foreColor = ConsoleColor.Gray;
                break;
            case "{info}":
                //foreColor = ConsoleColor.Gray;
                break;
            case "{debug}":
                foreColor = ConsoleColor.Green;
                break;
            case "{warning}":
                foreColor = ConsoleColor.Yellow;
                break;
            case "{error}":
                foreColor = ConsoleColor.Red;
                break;
        }
        System.DateTime now=System.DateTime.Now;
        //lock (System.Console.Out) {
            System.ConsoleColor foreColor2 = System.Console.ForegroundColor;
            System.ConsoleColor backColor2 = System.Console.BackgroundColor;

            System.Console.ForegroundColor = foreColor;
            System.Console.BackgroundColor = backColor;
            System.Console.WriteLine("{0} {1}\r\n{2}", now.ToString("HH:mm:ss.fffffff"), Name, message);
            System.Console.ForegroundColor = foreColor2;
            System.Console.BackgroundColor = backColor2;
        //}

    }
    #endregion
    
    #endregion
}
#if !net20 && missing
/// <summary>
/// 控制台日志存储扩展类。
/// </summary>
public static class ConsoleLogExtenstions {

    #region methods

    /// <summary>
    /// 创建日志存储对象（类型，当前程序目录）。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ILog Console(this System.Type type) {
        return new ConsoleLog(LogBase.GetName(type));
    }
    /// <summary>
    /// 创建日志存储对象（成员，当前程序目录）。
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static ILog Console(this System.Reflection.MemberInfo memberInfo) {
        return new ConsoleLog(LogBase.GetName(memberInfo));
    }

    #endregion
}
#endif

/// <summary>
/// 文件日志存储，自动按天拆分文件。
/// </summary>
public class FileLog : LogBase {

    #region fields
    private string _path;
    private string _file;
    private System.IO.TextWriter _writer;
    private System.DateTime _lastDate;
    private System.Collections.Concurrent.ConcurrentQueue<string> _list;
    private int _doing;
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, FileLog> _caches;
    #endregion

    #region cctor
    static FileLog() {
        _caches = new System.Collections.Concurrent.ConcurrentDictionary<string, FileLog>(StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region ctor
    /// <summary>
    /// 创建FileLogRepository实例（当前程序目录）。
    /// </summary>
    /// <param name="name">名称，不能包含无效文件名字符。</param>
    public FileLog(string name) : this(name, AppHelper.MapPath("Log")) {
    }
    /// <summary>
    /// 创建FileLogRepository实例。
    /// </summary>
    /// <param name="name">名称，不能包含无效文件名字符。</param>
    /// <param name="path">路径</param>
    public FileLog(string name, string path) : base(name) {
        AppHelper.CreateDirectory(path);
        _path = System.IO.Path.GetFullPath(path);
        _lastDate = System.DateTime.Today;
        _list = new System.Collections.Concurrent.ConcurrentQueue<string>();
    }
    #endregion


    #region methods

    #region CreateWriter
    System.IO.TextWriter CreateWriter() {
        System.IO.TextWriter writer = null;
        if (_writer == null || _lastDate.Date != System.DateTime.Today) {
            _lastDate = System.DateTime.Now;
            if (_writer != null) {
                try { _writer.Flush(); } catch { }
                try { _writer.Dispose(); } catch { }
                _writer = null;
            }
            string name = string.Format($"{System.DateTime.Now.ToString("yyyy-MM-dd")}{System.IO.Path.DirectorySeparatorChar}{Name}-{System.DateTime.Now.ToString("HHmmss")}.log");
            _file = System.IO.Path.Combine(_path, name);
            AppHelper.CreateDirectory(System.IO.Path.GetDirectoryName(_file));
            writer = new System.IO.StreamWriter(CreateFile(_file, true, true), System.Text.Encoding.UTF8, 1024);
            _writer = writer;
        } else {
            writer = _writer;
        }
        return writer;

    }
    System.IO.Stream CreateFile(string path, bool append, bool checkHost) {
        return new System.IO.FileStream(path,
            append ? System.IO.FileMode.Append : System.IO.FileMode.Create,
            System.IO.FileAccess.Write, System.IO.FileShare.Read, 0x1000,
            System.IO.FileOptions.SequentialScan);
    }
    #endregion

    #region OnWriteLine
    /// <summary>
    /// 输出一个一行信息（时间 level&gt;消息）。
    /// </summary>
    /// <param name="level">级别</param>
    /// <param name="message">消息</param>
    protected override void OnWriteLine(string level, string message) {
        message = string.Format("{0} {1}>{2}", System.DateTime.Now.ToString("HH:mm:ss.fffffff"), level, message);
        var list = System.Threading.Interlocked.CompareExchange(ref _list, null, null);
        if (list == null)
            return;
        list.Enqueue(message);
        Flush();
    }
    void Flush() {
        System.Threading.ThreadPool.QueueUserWorkItem(FlushBody, null);
    }
    void FlushBody(object state) {
        if (System.Threading.Interlocked.CompareExchange(ref _doing, 1, 0) == 1)
            return;
        var list = System.Threading.Interlocked.CompareExchange(ref _list, null, null);
        if (list == null || list.IsEmpty)
            goto lb_End;
        try {
            var writer = CreateWriter();
            if (writer == null)
                goto lb_End;

            while (list.TryDequeue(out string message)) {
                writer.WriteLine(message);
            }
            writer.Flush();
        } catch {
            goto lb_End;
        }

    lb_End:
        System.Threading.Interlocked.CompareExchange(ref _doing, 0, 1);
    }
    #endregion

    #region Create
    static FileLog Create(string name, string path) {
        if (string.IsNullOrEmpty(path))
            path = AppHelper.MapPath("Log");
        string key = name + "|" + path;
        return _caches.GetOrAdd(key, (p) => {
            return new FileLog(name, path);
        });
    }
    /// <summary>
    /// 创建日志存储对象（类型，当前程序目录）。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static FileLog Create(System.Type type) {
        if (type == null)
            return null;
        return Create(GetName(type), null);
    }
    /// <summary>
    /// 创建日志存储对象（类型）。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static FileLog Create(System.Type type, string path) {
        if (type == null)
            return null;
        return Create(GetName(type), path);
    }
    /// <summary>
    /// 创建日志存储对象（成员，当前程序目录）。
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static FileLog Create(System.Reflection.MemberInfo memberInfo) {
        if (memberInfo == null)
            return null;
        return Create(GetName(memberInfo), null);
    }
    /// <summary>
    /// 创建日志存储对象（成员）。
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static FileLog Create(System.Reflection.MemberInfo memberInfo, string path) {
        if (memberInfo == null)
            return null;
        return Create(GetName(memberInfo), path);
    }
    #endregion
    #region Dispose
    /// <summary>
    /// 释放占用的资源。
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing) {
        if (!_disposed && disposing) {
            Flush();
            System.Threading.Interlocked.Exchange(ref _list, null);
            var writer = System.Threading.Interlocked.Exchange(ref _writer, null);
            if (writer != null) {
                try { writer.Dispose(); } catch { }
            }
            string key = Name + "|" + _path;
            _caches.TryRemove(key, out FileLog removed);
        }
        base.Dispose(disposing);
    }
    #endregion

    #endregion
}
#if !net20
/// <summary>
/// 文件日志存储扩展类。
/// </summary>
public static class FileLogExtenstions {

    #region methods

    /// <summary>
    /// 创建日志存储对象（类型，当前程序目录）。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static FileLog FileLog(this System.Type type) {
        return global::FileLog.Create(type);
    }
    /// <summary>
    /// 创建日志存储对象（类型）。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static FileLog FileLog(this System.Type type, string path) {
        return global::FileLog.Create(type, path);
    }
    /// <summary>
    /// 创建日志存储对象（成员，当前程序目录）。
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static FileLog FileLog(this System.Reflection.MemberInfo memberInfo) {
        return global::FileLog.Create(memberInfo);
    }
    /// <summary>
    /// 创建日志存储对象（成员）。
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static FileLog FileLog(this System.Reflection.MemberInfo memberInfo, string path) {
        return global::FileLog.Create(memberInfo, path);
    }

    #endregion
}
#endif
