/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

/// <summary>
/// 应用程序辅助类
/// </summary>
/// <remarks>主要做一些跟当前应用程序相关的操作，包括文件和目录的操作。</remarks>
public class AppHelper
#if !netcore
    : MarshalByRefObject
#endif
    {

    #region fields
    private static readonly bool _isWindows;
    #endregion

    #region properties

    #region AppPath
    private static string _appPath;
    /// <summary>
    /// 当前应用程序位置（目录）
    /// </summary>
    public static string AppPath {
        get {
            return _appPath;
        }

    }
    #endregion
    #region AppFile
    private static string _appFile;
    /// <summary>
    /// 当前应用程序主文件路径（Windows应用程序为exe的路径，ASP.NET应用程序为主dll的路径）
    /// </summary>
    public static string AppFile {
        get {
            return _appFile;
        }
    }
    #endregion
    #region Assembly
    private static System.Reflection.Assembly _assembly;
    /// <summary>
    /// 当前应用程序的主程序集
    /// </summary>
    public static System.Reflection.Assembly Assembly {
        get {
            return _assembly;
        }
    }
    #endregion

    #endregion

    #region cctor
    static AppHelper() {
        if (_assembly == null)
            _assembly = System.Reflection.Assembly.GetEntryAssembly();
#if !netcore
        if (_assembly == null)
            _assembly = System.Reflection.Assembly.GetCallingAssembly();
#endif
        _isWindows = System.IO.Path.DirectorySeparatorChar == '\\';
        if (!string.IsNullOrEmpty(_assembly.CodeBase)) {
            _appFile = _assembly.CodeBase.Replace("file:///", "").Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
        } else {
            _appFile = _assembly.Location;
        }
        if (!_isWindows) {
            if (_appFile[0] != System.IO.Path.DirectorySeparatorChar) {
                _appFile = System.IO.Path.DirectorySeparatorChar + _appFile;
            }
        }
        _appPath = System.IO.Path.GetDirectoryName(AppFile);
    }
    #endregion

    #region methods

    #region MapPath
    /// <summary>
    /// 映射路径，用于将相对路径变为绝对路径，相对于AppPath。
    /// </summary>
    /// <param name="path">需要处理的路径，支持~/方式，自动识别是否为绝对路径。</param>
    /// <returns>返回处理后的路径</returns>
    /// <remarks>用过ASP.NET的Server.MapPath应该不陌生。</remarks>
    /// <seealso cref="AppHelper.CreateDirectory(string,bool)"/>
    public static string MapPath(string path) {
        if (string.IsNullOrEmpty(path))
            return AppPath;
        if (path.StartsWith("~"))
            return (AppPath + path.Substring(1).Replace("/", "\\")).Replace("\\\\",System.IO.Path.DirectorySeparatorChar.ToString());
        if (path.IndexOf(':') > -1)
            return path;
        return System.IO.Path.Combine(AppPath, path);
    }
    #endregion

    #region LoadTextFile
    /// <summary>
    /// 加载文本文件
    /// </summary>
    /// <param name="path">文件位置（支持相对路径）</param>
    /// <returns>返回文件内容，文件不存时直接返回<c>string.Empty</c>。</returns>
    public static string LoadTextFile(string path) {
        return LoadTextFile(path, null);
    }
    /// <summary>
    /// 加载文本文件
    /// </summary>
    /// <param name="path">文件位置（支持相对路径）</param>
    /// <param name="encoding">文件编码</param>
    /// <returns>返回文件内容，文件不存时直接返回string.Empty。</returns>
    public static string LoadTextFile(string path, System.Text.Encoding encoding) {
        string file = MapPath(path);
        return ThreadHelper.ParallelLock("file", path).Block(() => {
            if (!System.IO.File.Exists(file))
                return string.Empty;
            if (encoding == null) {
                return System.IO.File.ReadAllText(file);
            } else {
                return System.IO.File.ReadAllText(file, encoding);
            }
        });
    }
    #endregion
    #region SaveTextFile
    /// <summary>
    /// 保存内容到文本文件
    /// </summary>
    /// <param name="path">文件位置（支持相对路径，自动创建目录）</param>
    /// <param name="contents">文本内容</param>
    public static void SaveTextFile(string path, string contents) {
        SaveTextFile(path, contents, false, null);
    }
    /// <summary>
    /// 保存内容到文本文件
    /// </summary>
    /// <param name="path">文件位置（支持相对路径，自动创建目录）</param>
    /// <param name="contents">文本内容</param>
    /// <param name="encoding">文件编码</param>
    public static void SaveTextFile(string path, string contents, System.Text.Encoding encoding) {
        SaveTextFile(path, contents, false, encoding);
    }
    /// <summary>
    /// 保存内容到文本文件
    /// </summary>
    /// <param name="path">文件位置（支持相对路径，自动创建目录）</param>
    /// <param name="contents">文本内容</param>
    /// <param name="isAppend">是否为追加模式</param>
    public static void SaveTextFile(string path, string contents, bool isAppend) {
        SaveTextFile(path, contents, isAppend, null);
    }
    /// <summary>
    /// 保存内容到文本文件
    /// </summary>
    /// <param name="path">文件位置（支持相对路径，自动创建目录）</param>
    /// <param name="contents">文本内容</param>
    /// <param name="isAppend">是否为追加模式</param>
    /// <param name="encoding">文件编码</param>
    public static void SaveTextFile(string path, string contents, bool isAppend, System.Text.Encoding encoding) {
        string file = MapPath(path);
        ThreadHelper.ParallelLock("file", path).Block(() => {
            string directory = System.IO.Path.GetDirectoryName(file);
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            if (isAppend) {
                if (encoding == null)
                    System.IO.File.AppendAllText(file, contents);
                else
                    System.IO.File.AppendAllText(file, contents, encoding);
            } else {
                if (encoding == null)
                    System.IO.File.WriteAllText(file, contents);
                else
                    System.IO.File.WriteAllText(file, contents, encoding);
            }
        });
    }
    #endregion

    #region CopyFile
    /// <summary>
    /// 复制文件（复制前会自动删除目标文件）
    /// </summary>
    /// <param name="sourceFilename">源文件位置（支持相对路径）</param>
    /// <param name="destFilename">目标文件位置（支持相对路径,自动创建目录）</param>
    /// <remarks>关于自动删除文件，请参考 <see cref="DeleteFile"/> 。</remarks>
    public static void CopyFile(string sourceFilename, string destFilename) {
        DeleteFile(destFilename);
        System.IO.File.Copy(sourceFilename, destFilename);
    }
    #endregion
    #region DeleteFile
    /// <summary>
    /// 删除文件（只读文件、隐藏文件、系统文件都可以删除）
    /// </summary>
    /// <param name="path">文件位置（支持相对路径）</param>
    /// <remarks>文件不存在直接结束操作，如果权限不足的操作会报异常。</remarks>
    public static void DeleteFile(string path) {
        path = MapPath(path);
        ThreadHelper.ParallelLock("file", path).Block(() => {
            if (!System.IO.File.Exists(path))
                return;
            System.IO.File.SetAttributes(path, System.IO.FileAttributes.Normal);
            System.IO.File.Delete(path);
        });
    }
    #endregion

    #region CreateDirectory
    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path">路径</param>
    public static void CreateDirectory(string path) {
        CreateDirectory(path, true);
    }
    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="isMapPath">是否为一个相对路径，如果是相对路径将会自动映射。</param>
    public static void CreateDirectory(string path, bool isMapPath) {
        if (isMapPath)
            path = MapPath(path);
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
    }
    #endregion
    #region CopyDirectory
    /// <summary>
    /// 复制目录
    /// </summary>
    /// <param name="sourceDirName">源目录位置（支持相对路径）</param>
    /// <param name="destDirName">目标目录位置（支持相对路径,自动创建目录）</param>
    /// <param name="copySubDirs">是否复制子目录（包括子目录中的文件）</param>
    /// <remarks>如果目标目录中存在同样的文件名，将会强制替换。</remarks>
    /// <exception cref="System.IO.DirectoryNotFoundException">源目录位置不存在</exception>
    public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs) {
        sourceDirName = MapPath(sourceDirName);
        destDirName = MapPath(destDirName);

        if (!System.IO.Directory.Exists(sourceDirName))
            throw new System.IO.DirectoryNotFoundException(sourceDirName);
        if (!System.IO.Directory.Exists(destDirName))
            CreateDirectory(destDirName);

        System.IO.FileInfo[] files = new System.IO.DirectoryInfo(sourceDirName).GetFiles();
        foreach (System.IO.FileInfo file in files) {
            string temppath = System.IO.Path.Combine(destDirName, file.Name);
            CopyFile(file.FullName, temppath);
        }

        if (copySubDirs) {
            System.IO.DirectoryInfo[] dirs = new System.IO.DirectoryInfo(sourceDirName).GetDirectories();
            foreach (System.IO.DirectoryInfo subdir in dirs) {
                string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
    #endregion
    #region DeleteDirectory
    /// <summary>
    /// 删除目录（包括目录自己）
    /// </summary>
    /// <param name="path">目录位置（支持相对路径）</param>
    /// <remarks>目录不存在直接结束操作。</remarks>
    public static void DeleteDirectory(string path) {
        DeleteDirectory(path, true);
    }
    /// <summary>
    /// 删除目录
    /// </summary>
    /// <param name="path">目录位置（支持相对路径）</param>
    /// <param name="deleteSelf">是否删除目录本身，如果不删除等同清空目录。</param>
    /// <remarks>目录不存在直接结束操作。</remarks>
    public static void DeleteDirectory(string path, bool deleteSelf) {
        path = MapPath(path);
        if (!System.IO.Directory.Exists(path))
            return;
        if (deleteSelf) {
            System.IO.Directory.Delete(path,true);
            return;
        }
        string[] dirs = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string item in dirs) {
            DeleteDirectory(item,true);
        }
        string[] files = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string item in files) {
            DeleteFile(item);
        }
        //System.Threading.Thread.Sleep(100);
        //if (deleteSelf) {
        //    System.IO.Directory.Delete(path);
        //    System.Threading.Thread.Sleep(200);
        //}
    }
    #endregion

    #region GetFiles
    /// <summary>
    /// 返回指定目录中文件的名称，该目录与指定搜索模式匹配并使用某个值确定是否在子目录中搜索。
    /// </summary>
    /// <param name="path">要搜索的目录，不存在不报错。</param>
    /// <param name="searchPattern">搜索字符串。</param>
    /// <param name="searchOption">指定搜索操作应包括所有子目录还是仅包括当前目录。</param>
    /// <returns>文件列表。</returns>
    public static System.Collections.Generic.List<string> GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption) {
        var list = new System.Collections.Generic.List<string>();
        if (string.IsNullOrEmpty(path))
            return list;
        path = MapPath(path);
        if (!System.IO.Directory.Exists(path))
            return list;
        try {
            list.AddRange(System.IO.Directory.GetFiles(path, searchPattern, System.IO.SearchOption.TopDirectoryOnly));
        } catch { }
        if (searchOption == System.IO.SearchOption.TopDirectoryOnly)
            return list;
        foreach (string dir in System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly)) {
            try {
                list.AddRange(System.IO.Directory.GetFiles(dir, searchPattern, searchOption));
            } catch {
            }
        }
        return list;
    }
    #endregion

    #endregion
}