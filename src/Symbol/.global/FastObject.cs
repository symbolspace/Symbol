/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Globalization;
using System.Reflection;

/// <summary>
/// 快速对象（反射调用）类。
/// </summary>
public class FastObject {

    #region fields
    private object _instance;
    private SaveAction _saveAction;
    private string _file;
    //private System.IO.FileSystemWatcher _fileSystemWatcher;
    //private bool _autoSaveReload;
    #endregion

    #region properties

    /// <summary>
    /// 获取或设置当前对象。
    /// </summary>
    public object Instance {
        get { return _instance; }
        set {
            _instance = value;
            if (value == null) {
                _instance = new Symbol.Collections.Generic.NameValueCollection<object>(System.StringComparer.Ordinal);
            }
        }
    }

    ///// <summary>
    ///// 获取或设置自动保存和重新载入（保存时自动禁止载入，载入时不保存）。
    ///// </summary>
    //public bool AutoSaveReload {
    //    get { return _autoSaveReload; }
    //    set {
    //        if (_autoSaveReload == value)
    //            return;
    //        if (value) {
    //            if (_fileSystemWatcher == null) {
    //            } else {
    //                _fileSystemWatcher.EnableRaisingEvents = value;
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 获取或设置文件。
    /// </summary>
    public string File {
        get { return _file; }
        set {
            if (!string.IsNullOrEmpty(_file) && _saveAction != null)
                _saveAction = null;
            _file = value;

        }
    }
    /// <summary>
    /// 获取或设置值
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <returns></returns>
    public object this[string path] {
        get {
            return Path(_instance, path);
        }
        set {
            Path(_instance, path, value);
        }
    }
    /// <summary>
    /// 获取或设置值
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="formatArgs">path为格式串是的参数列表。</param>
    /// <returns></returns>
    public object this[string path, params object[] formatArgs] {
        get {

            return Path(_instance, FormatPath(path, formatArgs));
        }
        set {
            Path(_instance, FormatPath(path, formatArgs), value);
        }
    }

    string FormatPath(string path, object[] formatArgs) {
        if (string.IsNullOrEmpty(path))
            return "";
        if (formatArgs != null && formatArgs.Length > 0)
            path = string.Format(path, formatArgs);
        return path;
    }

    #endregion

    #region ctor
    /// <summary>
    /// 创建FastObject实例（创建空白对象）。
    /// </summary>
    public FastObject() : this(null, null) {
    }
    /// <summary>
    /// 创建FastObject实例。
    /// </summary>
    /// <param name="file">文件或JSON，编码要求为UTF-8，文件不存在或格式不正确，不会报错。</param>
    public FastObject(string file) :this((object)file,null){
    }
    /// <summary>
    /// 创建FastObject实例。
    /// </summary>
    /// <param name="instance">当前对象，为null时，自动创建Dictionary</param>
    public FastObject(object instance):this(instance,null) {
    }

    /// <summary>
    /// 创建FastObject实例（创建空白对象）。
    /// </summary>
    /// <param name="saveAction">保存时的逻辑。</param>
    public FastObject(SaveAction saveAction) : this(null, saveAction) {
    }
    /// <summary>
    /// 创建FastObject实例。
    /// </summary>
    /// <param name="instance">当前对象，为null时，自动创建Dictionary</param>
    /// <param name="saveAction">保存时的逻辑。</param>
    public FastObject(object instance, SaveAction saveAction) {
        if (instance is string) {
            Instance = Load((string)instance);
        } else {
            Instance = instance;
        }
        _saveAction = saveAction;
    }
    #endregion

    #region methods

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <returns>不存在返回null</returns>
    public object Get(string path) {
        return Path(_instance, path);
    }
    /// <summary>
    /// 获取值（包装对象）
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <returns>不存在返回null</returns>
    public FastObject GetWrapper(string path) {
        var value = Path(_instance, path);
        if (value == null)
            return null;
        return new FastObject(value);
    }
    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="formatArgs">path为格式串是的参数列表。</param>
    /// <returns>不存在返回null</returns>
    public object Get(string path, params object[] formatArgs) {
        return Path(_instance, FormatPath(path, formatArgs));
    }
    /// <summary>
    /// 获取值
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <returns></returns>
    public T Get<T>(string path) {
        return TypeExtensions.Convert<T>(Path(_instance, path));
    }
    /// <summary>
    /// 获取值
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="formatArgs">path为格式串是的参数列表。</param>
    /// <returns></returns>
    public T Get<T>(string path, params object[] formatArgs) {
        return TypeExtensions.Convert<T>(Path(_instance, FormatPath(path, formatArgs)));
    }
    /// <summary>
    /// 获取值
    /// </summary>
    /// <typeparam name="T">任意结构类型</typeparam>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="defaultValue">如果为null时的默认值</param>
    /// <returns></returns>
    public T Get<T>(string path, T defaultValue) where T : struct {
        return TypeExtensions.Convert<T>(Path(_instance, path), defaultValue);
    }
    /// <summary>
    /// 获取值
    /// </summary>
    /// <typeparam name="T">任意结构类型</typeparam>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="defaultValue">如果为null时的默认值</param>
    /// <param name="formatArgs">path为格式串是的参数列表。</param>
    /// <returns></returns>
    public T Get<T>(string path, T defaultValue, params object[] formatArgs) where T : struct {
        return TypeExtensions.Convert<T>(Path(_instance, FormatPath(path, formatArgs)), defaultValue);
    }
    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="value">值</param>
    public void Set(string path, object value) {
        Path(_instance, path, value);
    }
    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。</param>
    /// <param name="value">值</param>
    /// <param name="formatArgs">path为格式串是的参数列表。</param>
    public void Set(string path, object value, params object[] formatArgs) {
        Path(_instance, FormatPath(path, formatArgs), value);
    }

    #region Load
    object Load(string file) {
        file = file?.Trim();
        if (string.IsNullOrEmpty(file))
            return null;
        if (file[0] == '[' || file[0] == '{') {
            return Symbol.Serialization.Json.Parse(file);
        }
        file = AppHelper.MapPath(file);
        object o = Symbol.Serialization.Json.Parse(AppHelper.LoadTextFile(file, System.Text.Encoding.UTF8), true);
        _file = file;
        return o;
    }
    #endregion
    #region Save
    /// <summary>
    /// 保存保存对象
    /// </summary>
    /// <returns>返回是否保存成功。</returns>
    public bool Save() {
        bool ok = false;
        if (_saveAction != null)
            ok = _saveAction(_instance);
        if (!string.IsNullOrEmpty(_file)) {
            ok = Save(_file);
        }
        return ok;
    }
    /// <summary>
    /// 保存对象到文件（JSON，格式化）。
    /// </summary>
    /// <param name="file">文件路径。</param>
    /// <returns>返回是否保存成功。</returns>
    public bool Save(string file) {
        bool ok = Save_File(file);
        if (ok && _saveAction == null) {
            _file = file;
            _saveAction = Save_File;
        }
        return ok;
    }
    bool Save_File(object instance) {
        if (string.IsNullOrEmpty(_file))
            return false;
        try {
            if (System.IO.File.Exists(_file))
                System.IO.File.SetAttributes(_file, System.IO.FileAttributes.Normal);
            AppHelper.SaveTextFile(_file, Symbol.Serialization.Json.ToString(instance,true,true), System.Text.Encoding.UTF8);
            return System.IO.File.Exists(_file);
        } catch {
            return false;
        }
    }
    bool Save_File(string file) {
        if (string.IsNullOrEmpty(file))
            return false;
        try {
            if (System.IO.File.Exists(file))
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            AppHelper.SaveTextFile(file, Symbol.Serialization.Json.ToString(_instance, true, true), System.Text.Encoding.UTF8);
            return System.IO.File.Exists(file);
        } catch {
            return false;
        }
    }
    #endregion

    #region Length
    /// <summary>
    /// 获取集合的长度（自适应Array.Length或collection.Count）
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。不传表示当前对象。</param>
    /// <returns></returns>
    public int Length(string path=null) {
        if (string.IsNullOrEmpty(path))
            path = "length";
        else
            path += ".length";
        return Get(path, 0);
    }
    #endregion
    #region IsEnumerable
    /// <summary>
    /// 是否可以转换为可枚举对象。
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。不传表示当前对象。</param>
    /// <returns></returns>
    public bool IsEnumerable(string path = null) {
        if (string.IsNullOrEmpty(path))
            return _instance as System.Collections.IEnumerable != null;
        else
            return Get(path) as System.Collections.IEnumerable != null;
    }
    #endregion
    #region AsEnumerable
    /// <summary>
    /// 强制转换为可枚举对象，单对象自动变成数组，如果都不成功返回new object[0]
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。不传表示当前对象。</param>
    /// <returns></returns>
    public System.Collections.IEnumerable AsEnumerable(string path = null) {
        var value = string.IsNullOrEmpty(path) ? _instance : Get(path);
        var list = value as System.Collections.IEnumerable;
        if (list == null) {
            if (value == null) {
                list = new object[0];
            } else {
                list = new object[] { value };
            }
        }
        return list;
    }
    /// <summary>
    /// 强制转换为可枚举对象，单对象自动变成数组，如果都不成功返回空白序列
    /// </summary>
    /// <param name="path">key,支持深度path，如a.b[1] 表示成员a的b成员中的第2个元素。不传表示当前对象。</param>
    /// <returns></returns>
    public System.Collections.Generic.IEnumerable<FastObject> AsEnumerableWrapper(string path = null) {
        return LinqHelper.Select(AsEnumerable(path), p => new FastObject(p));
    }

    #endregion


    #region ToJson
    /// <summary>
    /// 将对象输出为Json文本。
    /// </summary>
    /// <returns>返回对象的json文本。</returns>
    public string ToJson() {
        return ToJson(false);
    }
    /// <summary>
    /// 将对象输出为Json文本。
    /// </summary>
    /// <param name="formated">是否格式化json</param>
    /// <returns>返回对象的json文本。</returns>
    public string ToJson(bool formated) {
        return Symbol.Serialization.Json.ToString(_instance, true, formated);
    }
    #endregion
    #region ToString
    /// <summary>
    /// 返回当前对象的json文本。
    /// </summary>
    /// <returns>返回对象的json文本。</returns>
    public override string ToString() {
        return ToJson();
    }
    #endregion


    #region implicit
    /// <summary>
    /// 解析 json
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator FastObject(string value) {
        return new FastObject(value);
    }
    #endregion
    #region explicit
    /// <summary>
    /// 转为json文本
    /// </summary>
    /// <param name="value"></param>
    public static explicit operator string(FastObject value) {
        return value == null ? "null" : value.ToJson();
    }
    #endregion


    #region Mapper

    #region Path
    /// <summary>
    /// path，快速获取值。
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="path">操作路径。字典key:"aa";数组索引:[0];组合使用:"data.items[0].name"。</param>
    /// <returns>返回最终的值。</returns>
    public static object Path(object instance, string path) {
        if (instance == null)
            return null;
        if (string.IsNullOrEmpty(path))
            return null;
        PathNode[] nodes = ParsePath(path);
        PathMapper mapper = new PathMapper(instance, nodes);
        if (mapper.Map(false)) {
            return nodes[nodes.Length - 1].value;
        } else {
            return null;
        }
    }
    delegate void PathSetValueAction(object parent, object lastTarget, string lastKey, object value);
    /// <summary>
    /// path，快速设置值。
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="path">操作路径。字典key:"aa";数组索引:[0];组合使用:"data.items[0].name"。</param>
    /// <param name="value">要设置的值</param>
    /// <returns>返回是否操作成功。</returns>
    public static bool Path(object instance, string path, object value) {
        if (instance == null)
            return false;
        if (string.IsNullOrEmpty(path))
            return false;
        try {
            string[] paths = path.Split('.');
            object parent = null;
            object lastTarget = instance;
            string lastKey = null;
            bool lastAllowNull = false;
            PathSetValueAction setMethod = null;

            for (int i = 0; i < paths.Length; i++) {
                lastAllowNull = false;
                string p = paths[i];
                if (string.IsNullOrEmpty(p))
                    return false;
                int p10 = p.IndexOf('[');
                string p11 = null;
                if (p10 > -1) {
                    p11 = p.Substring(p10 + 1, p.Length - 2 - p10);
                    p = p.Substring(0, p10);
                }
                //JsonObject j;
                if (!string.IsNullOrEmpty(p)) {
                    if (System.Text.RegularExpressions.Regex.IsMatch(p, "^[0-9a-zA-Z]+$")) {
                        System.Collections.Generic.IList<object> list = lastTarget as System.Collections.Generic.IList<object>;
                        if (list != null) {
                            parent = lastTarget;
                            try {
                                if (string.Equals("add", p, System.StringComparison.OrdinalIgnoreCase)
                                    || string.Equals("push", p, System.StringComparison.OrdinalIgnoreCase)) {
                                    lastTarget = null;
                                } else {
                                    lastTarget = list[TypeExtensions.Convert<int>(p, 0)];
                                }
                            } catch { lastTarget = null; }
                            lastKey = p;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                //((System.Collections.Generic.IList<object>)p1)[TypeExtensions.Convert<int>(p3, 0)] = p4;
                                System.Collections.Generic.IList<object> p10_list = (System.Collections.Generic.IList<object>)p1;
                                if (string.Equals("add", p3, System.StringComparison.OrdinalIgnoreCase)
                                    || string.Equals("push", p3, System.StringComparison.OrdinalIgnoreCase)) {
                                    p10_list.Add(p4);
                                    return;
                                }
                                int p11_list = TypeExtensions.Convert<int>(p3, 0);
                                if ((p10_list.Count - 1) < p11_list) {
                                    p10_list.Add(p4);
                                    return;
                                }
                                p10_list[p11_list] = p4;
                            };
                            goto lb_Index;
                        }
                    }
                    {
                        System.Collections.Generic.IDictionary<string, object> dic = lastTarget as System.Collections.Generic.IDictionary<string, object>;
                        if (dic != null) {
                            parent = lastTarget;
                            if (!dic.TryGetValue(p, out lastTarget)) {
                                lastTarget = null;
                            }
                            //lastTarget = dic[p];
                            lastKey = p;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                //((System.Collections.Generic.IDictionary<string, object>)p1)[(string)p3] = p4;
                                System.Collections.Generic.IDictionary<string, object> p10_list = (System.Collections.Generic.IDictionary<string, object>)p1;
                                object p11_v;
                                if (p10_list.TryGetValue(p3, out p11_v))
                                    p10_list[p3] = p4;
                                else
                                    p10_list.Add(p3, p4);
                            };
                            goto lb_Index;
                        }
                    }
                    {
                        System.Collections.IDictionary dic = lastTarget as System.Collections.IDictionary;
                        if (dic != null) {
                            parent = lastTarget;
                            try {
                                lastTarget = dic[p];
                            } catch { lastTarget = null; }
                            lastKey = p;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                System.Collections.IDictionary p10_list = (System.Collections.IDictionary)p1;
                                if (p10_list.Contains(p3))
                                    p10_list[p3] = p4;
                                else
                                    p10_list.Add(p3, p4);
                                //((System.Collections.IDictionary)p1)[(string)p3] = p4;
                            };
                            goto lb_Index;
                        }
                    }
                    {
                        if (string.Equals(p, "length", System.StringComparison.OrdinalIgnoreCase)) {
                            setMethod = null;
                            lastKey = null;
                            try {
                                parent = lastTarget;
                                lastTarget = TypeExtensions.Get(lastTarget, p);
                            } catch {
                                parent = lastTarget;
                                lastTarget = TypeExtensions.Get(lastTarget, p == "length" ? "Length" : "length");
                            }

                        } else {
                            lastTarget = TypeExtensions.Get(lastTarget, p);
                            lastKey = p;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => TypeExtensions.Set(p1, (string)p3, p4);
                        }
                        goto lb_Index;
                    }
                }
            lb_Index:
                if (lastTarget == null && !lastAllowNull)
                    return false;
                if (string.IsNullOrEmpty(p11))
                    continue;
                if (p11.StartsWith("\"")) {
                    p11 = p11.Substring(1, p11.Length - 2);
                    {
                        System.Collections.Generic.IDictionary<string, object> dic = lastTarget as System.Collections.Generic.IDictionary<string, object>;
                        if (dic != null) {
                            parent = lastTarget;
                            if (!dic.TryGetValue(p11, out lastTarget))
                                lastTarget = null;
                            //lastTarget = dic[p11];
                            lastKey = p11;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                //((System.Collections.Generic.IDictionary<string, object>)p1)[(string)p3] = p4;
                                System.Collections.Generic.IDictionary<string, object> p10_list = (System.Collections.Generic.IDictionary<string, object>)p1;
                                object p11_v;
                                if (p10_list.TryGetValue(p3, out p11_v))
                                    p10_list[p3] = p4;
                                else
                                    p10_list.Add(p3, p4);
                            };
                            continue;
                        }
                    }
                    {
                        System.Collections.IDictionary dic = lastTarget as System.Collections.IDictionary;
                        if (dic != null) {
                            parent = lastTarget;
                            try {
                                lastTarget = dic[p11];
                            } catch { lastTarget = null; }
                            lastKey = p11;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                System.Collections.IDictionary p10_list = (System.Collections.IDictionary)p1;
                                if (p10_list.Contains(p3))
                                    p10_list[p3] = p4;
                                else
                                    p10_list.Add(p3, p4);
                                //((System.Collections.IDictionary)p1)[(string)p3] = p4;
                            };
                            continue;
                        }
                    }
                    {
                        parent = lastTarget;
                        lastTarget = TypeExtensions.Get(lastTarget, p11);
                        lastKey = p11;
                        lastAllowNull = true;
                        setMethod = (p1, p2, p3, p4) => TypeExtensions.Set(p1, (string)p3, p4);
                        continue;
                    }
                } else {

                    {
                        System.Collections.Generic.IList<object> list = lastTarget as System.Collections.Generic.IList<object>;
                        if (list != null) {
                            parent = lastTarget;
                            try {
                                lastTarget = list[TypeExtensions.Convert<int>(p11, 0)];
                            } catch { lastTarget = null; }
                            lastKey = p11;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                //((System.Collections.Generic.IList<object>)p1)[TypeExtensions.Convert<int>(p3, 0)] = p4;
                                System.Collections.Generic.IList<object> p10_list = (System.Collections.Generic.IList<object>)p1;
                                if (string.Equals("add", p3, System.StringComparison.OrdinalIgnoreCase)
                                    || string.Equals("push", p3, System.StringComparison.OrdinalIgnoreCase)) {
                                    p10_list.Add(p4);
                                    return;
                                }
                                int p11_list = TypeExtensions.Convert<int>(p3, 0);
                                if ((p10_list.Count - 1) < p11_list) {
                                    p10_list.Add(p4);
                                    return;
                                }
                                p10_list[p11_list] = p4;
                            };
                            continue;
                        }
                    }
                    {
                        System.Collections.IList list = lastTarget as System.Collections.IList;
                        if (list != null) {
                            parent = lastTarget;
                            try {
                                lastTarget = list[TypeExtensions.Convert<int>(p11, 0)];
                            } catch { lastTarget = null; }
                            lastKey = p11;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => {
                                System.Collections.IList p10_list = (System.Collections.IList)p1;
                                if (string.Equals("add", p3, System.StringComparison.OrdinalIgnoreCase)
                                    || string.Equals("push", p3, System.StringComparison.OrdinalIgnoreCase)) {
                                    p10_list.Add(p4);
                                    return;
                                }
                                int p11_list = TypeExtensions.Convert<int>(p3, 0);
                                if ((p10_list.Count - 1) < p11_list) {
                                    p10_list.Add(p4);
                                    return;
                                }
                                p10_list[p11_list] = p4;
                                //((System.Collections.IList)p1)[TypeExtensions.Convert<int>(p3, 0)] = p4;
                            };
                            continue;
                        }
                    }
                    {
                        System.Array list = lastTarget as System.Array;
                        if (list != null) {
                            parent = lastTarget;
                            lastTarget = list.GetValue(TypeExtensions.Convert<int>(p11, 0));
                            lastKey = p11;
                            lastAllowNull = true;
                            setMethod = (p1, p2, p3, p4) => ((System.Array)p1).SetValue(p4, TypeExtensions.Convert<int>(p3, 0));
                            continue;
                        }
                    }
                }
                continue;
            }
            if (setMethod != null) {
                setMethod(parent, lastTarget, lastKey, value);
                return true;
            }
            return false;
        } catch {
            return false;
        }
    }
    #endregion

    #region ParsePath
    static PathNode[] ParsePath(string path) {
        if (string.IsNullOrEmpty(path))
            return PathNode.empty;
        PathParser parser = new PathParser(path);
        System.Collections.Generic.List<PathNode> list = parser.Parse();
        return list == null ? PathNode.empty : list.ToArray();
    }
    #endregion

    #endregion

    #endregion


    #region types
    /// <summary>
    /// 保存Action委托。
    /// </summary>
    /// <param name="instance">当前对象。</param>
    public delegate bool SaveAction(object instance);

    [System.Diagnostics.DebuggerDisplay("{key} {type}")]
    class PathNode {

        #region fields
        public static readonly PathNode[] empty = new PathNode[0];

        public PathNode parent;
        public string key;
        public PathNodeTypes type;
        public object model;
        public object value = null;

        #endregion

    }
    enum PathNodeTypes {
        Index,
        Property,
        MethodInvoke
    }

    class PathParser {

        #region fields
        private string _path;
        private bool _nodeBegin;
        private int _nodeIndex;
        private int _nodeLastIndex;
        private PathNodeTypes _type;
        private bool _indexBlock = false;
        private System.Collections.Generic.List<PathNode> _list;
        #endregion

        #region ctor
        public PathParser(string path) {
            _path = path;
        }
        #endregion


        #region methods

        #region CreateNode
        PathNode CreateNode(string key, PathNodeTypes type) {
            if (type == PathNodeTypes.Property && key.EndsWith("()")) {
                type = PathNodeTypes.MethodInvoke;
                key = key.Substring(0, key.Length - 2);
            }
            return new PathNode() {
                type = type,
                key = key,
            };
        }
        #endregion
        #region BeginNode
        void BeginNode(int i, PathNodeTypes type) {
            if (_nodeBegin) {
                EndNode(_nodeLastIndex);
            }
            _nodeBegin = true;
            _nodeIndex = i;
            _nodeLastIndex = -1;
            _type = type;
        }
        #endregion
        #region EndNode
        PathNode EndNode(int i) {
            if (!_nodeBegin)
                return null;
            if (i == -1 || _nodeIndex == -1 || _nodeIndex > _path.Length - 1) {
                //_ended = true;
                return null;
            }
            string key = _path.Substring(_nodeIndex, i - _nodeIndex + 1);
            if (string.IsNullOrEmpty(key)) {
                _nodeBegin = false;
                _nodeIndex = -1;
                _nodeLastIndex = -1;
                //_ended = true;
                return null;
            }
            if (key.StartsWith("\"") || key.StartsWith("'"))
                key = key.Substring(1);
            if (key.EndsWith("\"") || key.EndsWith("'"))
                key = key.Substring(0, key.Length - 1);

            PathNode node = CreateNode(key, _type);
            _nodeBegin = false;
            _nodeIndex = -1;
            _nodeLastIndex = -1;
            if (node != null) {
                if (_list.Count > 0)
                    node.parent = _list[_list.Count - 1];
                _list.Add(node);
            }
            return node;
        }
        #endregion
        #region Parse
        public System.Collections.Generic.List<PathNode> Parse() {
            if (string.IsNullOrEmpty(_path))
                return null;
            System.Collections.Generic.List<PathNode> list = _list = new System.Collections.Generic.List<PathNode>();
            _nodeBegin = false;
            _nodeIndex = -1;
            _nodeLastIndex = -1;

            for (int i = 0; i < _path.Length; i++) {
                char c = _path[i];
                switch (c) {
                    case '.': {
                            if (_indexBlock)
                                break;
                            EndNode(_nodeLastIndex);
                            BeginNode(i + 1, PathNodeTypes.Property);
                        }
                        break;
                    case '[': {
                            EndNode(_nodeLastIndex);
                            BeginNode(i + 1, PathNodeTypes.Index);
                            _indexBlock = true;
                        }
                        break;
                    case ']': {
                            EndNode(_nodeLastIndex);
                            _indexBlock = false;
                        }
                        break;
                    default: {
                            if (i == 0) {
                                BeginNode(i, PathNodeTypes.Property);
                                _nodeLastIndex = i;
                            } else if (_nodeBegin) {
                                if (i < _nodeIndex)
                                    break;
                                _nodeLastIndex = i;
                            }
                        }
                        break;
                }
            }
            if (_nodeBegin) {
                EndNode(_nodeLastIndex);
            }
            _list = null;
            _path = null;
            return list;
        }
        #endregion

        #endregion
    }
    class PathMapper {
        #region fields
        private PathNode[] _nodes;
        private object _model;
        #endregion

        #region ctor
        public PathMapper(object model, PathNode[] nodes) {
            _model = model;
            _nodes = nodes;
            _nodes[0].model = model;
        }
        #endregion

        #region methods
        void End() {
            _nodes = null;
            _model = null;
        }
        public bool Map(bool autoCreate) {
            for (int i = 0; i < _nodes.Length; i++) {
                PathNode node = _nodes[i];
                if (i > 0)
                    node.model = _nodes[i - 1].value;
                //int t = Environment.TickCount;
                if (node.type == PathNodeTypes.Property) {
                    //if (!Map_Property(node)) {
                    //    Map_Index(node);
                    //}
                    if (!Map_Index(node)) {
                        Map_Property(node);
                    }
                } else if (node.type == PathNodeTypes.Index) {
                    if (!Map_Index(node)) {
                        Map_Property(node);
                    }
                } else if (node.type == PathNodeTypes.MethodInvoke) {
                    Map_MethodInvoke(node);
                }
                //t = Environment.TickCount - t ;
                //Console.WriteLine("i:{0}ms, {1} {2}", t, node.key,node.type);
                if (node.value == null) {
                    if (autoCreate) {
                        //create object
                        End();
                        return false;
                    } else {
                        End();
                        return false;
                    }
                }

            }
            End();
            return true;
        }
        bool Map_Property(PathNode node) {
            if (string.Equals(node.key, "length", System.StringComparison.OrdinalIgnoreCase)) {
                if (
                      TryProperty(node, node.key == "length" ? "Length" : "length")
                   || TryProperty(node, node.key)
                   || TryProperty(node, "Count")
                   || TryProperty(node, "count")
                )
                    return true;
            }
            return TryProperty(node, node.key)
                || TryField(node, node.key);
        }
        bool Map_Index_Dictionary(PathNode node) {
            if (
                        TryIndex<System.Collections.Generic.IDictionary<string, object>>(node, node.key, TryIndex_IDictionary_String)
                )
                return true;
            if (
                    TryIndex<System.Collections.IDictionary>(node, node.key, TryIndex_IDictionary)
            )
                return true;
            if (
                    TryIndex<System.Collections.Specialized.NameValueCollection>(node, node.key, TryIndex_NameValueCollection)
            )
                return true;
            return false;
        }
        bool Map_Index(PathNode node) {
            //System.Collections.IList

            if (System.Text.RegularExpressions.Regex.IsMatch(node.key, "^[0-9]+$")) {
                int index = TypeExtensions.Convert<int>(node.key);
                if (
                       TryIndex<System.Collections.Generic.IList<object>>(node, index, TryIndex_IList_T)
                )
                    return true;
                if (
                       TryIndex<System.Collections.IList>(node, index, TryIndex_IList))
                    return true;
                if (
                       TryIndex<System.Array>(node, index, TryIndex_Array)
                )
                    return true;
                if (
                       TryIndex<System.Collections.Generic.IList<object>>(node, index, TryIndex_IList_T)
                )
                    return true;
                if (
                       TryIndex<System.Collections.Specialized.NameValueCollection>(node, index, TryIndex_NameValueCollection)
                )
                    return true;

                if (Map_Index_Dictionary(node))
                    return true;
                if (
                       TryIndex<System.Collections.IEnumerable>(node, index, TryIndex_IEnumerable)
                )
                    return true;

            }
            if (Map_Index_Dictionary(node))
                return true;
            if (string.Equals(node.key, "length", System.StringComparison.OrdinalIgnoreCase)) {
                if (
                      TryProperty(node, node.key == "length" ? "Length" : "length")
                   || TryProperty(node, node.key)
                   || TryProperty(node, "Count")
                   || TryProperty(node, "count")
                )
                    return true;
            }
            return false;
        }
        #region TryProperty
        bool TryProperty(PathNode node, string name) {
            try {
                System.Reflection.PropertyInfo propertyInfo = FastWrapper.GetProperty(node.model.GetType(), name);
                if (propertyInfo == null)
                    return false;
                node.value = propertyInfo.GetValue(node.model, new object[0]);// TypeExtensions.Get(node.model, property);
                return true;
            } catch {
                node.value = null;
                return false;
            }
        }
        #endregion
        #region TryField
        bool TryField(PathNode node, string name) {
            try {
                System.Reflection.FieldInfo fieldInfo = FastWrapper.GetField(node.model.GetType(), name);
                if (fieldInfo == null)
                    return false;
                node.value = fieldInfo.GetValue(node.model);// TypeExtensions.Get(node.model, property);
                return true;
            } catch {
                node.value = null;
                return false;
            }
        }
        #endregion
        #region TryIndex (PathNode,int,IndexGetter<T>)
        bool TryIndex<T>(PathNode node, int index, IndexGetter<T> getter) where T : class {
            var list = node.model as T;
            if (list == null)
                return false;
            return getter(node, list, index);
        }
        #endregion
        #region TryIndex_IList_T
        bool TryIndex_IList_T(PathNode node, System.Collections.Generic.IList<object> list, int index) {
            if (index < 0 || index > list.Count - 1) {
                node.value = null;
                return false;
            } else {
                node.value = list[index];
                return true;
            }
        }
        #endregion
        #region TryIndex_IList
        bool TryIndex_IList(PathNode node, System.Collections.IList list, int index) {
            if (index < 0 || index > list.Count - 1) {
                node.value = null;
                return false;
            } else {
                node.value = list[index];
                return true;
            }
        }
        #endregion
        #region TryIndex_Array
        bool TryIndex_Array(PathNode node, System.Array list, int index) {
            if (index < 0 || index > list.Length - 1) {
                node.value = null;
                return false;
            } else {
                node.value = list.GetValue(index);
                return true;
            }
        }
        #endregion
        #region TryIndex_IEnumerable
        bool TryIndex_IEnumerable(PathNode node, System.Collections.IEnumerable list, int index) {
            if (index < 0) {
                node.value = null;
                return false;
            } else {
                int n = -1;
                foreach (object value in list) {
                    n++;
                    if (n == index) {
                        node.value = value;
                        break;
                    }
                }
                return n > -1;
            }
        }
        #endregion
        #region TryIndex (PathNode,int,IndexGetterString<T>)
        bool TryIndex<T>(PathNode node, string index, IndexGetterString<T> getter) where T : class {
            var list = node.model as T;
            if (list == null)
                return false;
            return getter(node, list, index);
        }
        #endregion
        #region TryIndex_IDictionary_String
        bool TryIndex_IDictionary_String(PathNode node, System.Collections.Generic.IDictionary<string, object> list, string index) {
            object value;
            if (list.TryGetValue(index, out value)) {
                node.value = value;
            } else {
                node.value = null;
                return false;
            }
            return true;
        }
        #endregion
        #region TryIndex_IDictionary
        bool TryIndex_IDictionary(PathNode node, System.Collections.IDictionary list, string index) {
            if (list.Contains(index)) {
                node.value = list[index];
            } else {
                node.value = null;
                return false;
            }
            return true;
        }
        #endregion
        #region TryIndex_NameValueCollection
        bool TryIndex_NameValueCollection(PathNode node, System.Collections.Specialized.NameValueCollection list, int index) {
            if (index < 0 || index > list.Count - 1) {
                node.value = null;
                return false;
            } else {
                node.value = list[index];
                return true;
            }
        }
        bool TryIndex_NameValueCollection(PathNode node, System.Collections.Specialized.NameValueCollection list, string key) {
            if (string.IsNullOrEmpty(key)) {
                node.value = null;
                return false;
            }
            node.value = list[key];
            if (node.value == null)
                return false;
            return true;
        }
        #endregion

        void Map_MethodInvoke(PathNode node) {
            try {
                node.value = FastWrapper.MethodInvoke(node.model.GetType(), node.key, node.model, new object[0]);
            } catch { }

        }

        #endregion

        #region types
        delegate bool IndexGetter<T>(PathNode node, T source, int index);
        delegate bool IndexGetterString<T>(PathNode node, T source, string index);
        #endregion

    }


    #endregion


}
#if !net20
/// <summary>
/// 对象深度路径扩展类
/// </summary>
public static class FastObjectPathExtensions {
    #region Path
    /// <summary>
    /// path，快速获取值。
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="path">操作路径。字典key:"aa";数组索引:[0];组合使用:"data.items[0].name"。</param>
    /// <returns>返回最终的值。</returns>
    public static object Path(this object instance, string path) {
        return FastObject.Path(instance, path);
    }
    /// <summary>
    /// path，快速设置值。
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="path">操作路径。字典key:"aa";数组索引:[0];组合使用:"data.items[0].name"。</param>
    /// <param name="value">要设置的值</param>
    /// <returns>返回是否操作成功。</returns>
    public static bool Path(this object instance, string path, object value) {
        return FastObject.Path(instance, path, value);
    }


    #endregion
}
#endif

