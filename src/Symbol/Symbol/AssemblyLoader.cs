/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Symbol;
using Symbol.Formatting;

namespace Symbol {

    /// <summary>
    /// 程序集加载器。
    /// </summary>
    public class AssemblyLoader {

        #region 加载程序集文件
        /// <summary>
        /// 加载程序集文件（加载失败返回空）。
        /// </summary>
        /// <param name="file">程序集文件，文件不存在时则返回空。</param>
        /// <returns>返回成功加载的程序集对象。</returns>
        public static System.Reflection.Assembly Load(string file) {
            return Load(file, false);
        }
        /// <summary>
        /// 加载程序集文件。
        /// </summary>
        /// <param name="file">程序集文件，文件不存在时则返回空。</param>
        /// <param name="throwError">是否抛出异常</param>
        /// <returns>返回成功加载的程序集对象。</returns>
        public static System.Reflection.Assembly Load(string file, bool throwError) {
            if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
                return null;
            try {

#if netcore
                var assemblyName = System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(file);
                return System.Reflection.Assembly.Load(assemblyName);
#else
                return System.Reflection.Assembly.LoadFrom(file);
#endif
            } catch (System.Exception) {
                if (throwError)
                    throw;
                return null;
            }
        }

        #endregion

        #region 获取全局程序集
        /// <summary>
        /// 获取全局程序集（基于yield return）。
        /// </summary>
        /// <returns>返回匹配的程序集集合。</returns>
        /// <remarks>.net framework: System.AppDomain.CurrentDomain.GetAssemblies(), .net core: DependencyContext.Default.CompileLibraries</remarks>
        public static System.Collections.Generic.IEnumerable<System.Reflection.Assembly> GetAssemblies() {
            return GetAssemblies(null);
        }
        /// <summary>
        /// 获取全局程序集（基于yield return）。
        /// </summary>
        /// <param name="predicate">程序集匹配委托(string assemblyName, string version)，为空表示所有程序集。</param>
        /// <returns>返回匹配的程序集集合。</returns>
        /// <remarks>.net framework: System.AppDomain.CurrentDomain.GetAssemblies(), .net core: DependencyContext.Default.CompileLibraries</remarks>
        public static System.Collections.Generic.IEnumerable<System.Reflection.Assembly> GetAssemblies(AssemblyPredicate predicate) {
#if netcore
            {
                var deps = Microsoft.Extensions.DependencyModel.DependencyContext.Default;
                foreach (var p in deps.CompileLibraries) {
                    if (predicate == null || predicate(p.Name, p.Version)) {
                        Assembly assembly = null;
                        try {
                            var assemblyName = new System.Reflection.AssemblyName($"{p.Name}, Version={p.Version}");
                            assembly = System.Reflection.Assembly.Load(assemblyName);
                        } catch (System.Exception) {
                            continue;
                        }
                        if(assembly!=null)
                            yield return assembly;
                    }
                }
                yield break;
            }
#else
            {
                foreach (var p in System.AppDomain.CurrentDomain.GetAssemblies()) {
                    if (predicate == null || predicate(p.GetName().Name, p.GetName().Version.ToString()))
                        yield return p;
                }
                yield break;
            }
#endif

        }

        #endregion

        #region types
        /// <summary>
        /// 程序集匹配委托。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <param name="version">程序集版本。</param>
        /// <returns>返回匹配结果。</returns>
        public delegate bool AssemblyPredicate(string assemblyName, string version);
        #endregion
    }

}
