#if netcore
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace System {

    /// <summary>
    /// ServiceProvider 扩展类。
    /// </summary>
    public static class ServiceProviderExtensions {

        /// <summary>
        /// 获取Host对象。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IHostingEnvironment GetHost(this IServiceProvider serviceProvider) {
            return serviceProvider.GetService<IHostingEnvironment>();
        }

        /// <summary>
        /// 映射路径（相对Host的ContentRootPath）。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="path">相对路径，若以/或\开头将直接返回。</param>
        /// <returns></returns>
        /// <remarks>所有返回值，均处理为<see cref="System.IO.Path.GetFullPath(string)"/>。</remarks>
        public static string MapPath(this IServiceProvider serviceProvider, string path) {
            return MapPath(serviceProvider, path, true);
        }
        /// <summary>
        /// 映射路径。
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="path">相对路径，若以/或\开头将直接返回。</param>
        /// <param name="forContentPath">为true时相对Host的ContentRootPath，反之则相对WebRoot。</param>
        /// <returns></returns>
        /// <remarks>所有返回值，均处理为<see cref="System.IO.Path.GetFullPath(string)"/>。</remarks>
        public static string MapPath(this IServiceProvider serviceProvider, string path, bool forContentPath) {
            if (!string.IsNullOrEmpty(path) && (path[0] == '/' || path[0] == '\\'))
                return System.IO.Path.GetFullPath(path);
            var host = serviceProvider.GetHost();

            string rootPath = forContentPath ? host.ContentRootPath : host.WebRootPath;
            if (rootPath == null)
                rootPath = !forContentPath ? host.ContentRootPath : host.WebRootPath;

            if (string.IsNullOrEmpty(path))
                return rootPath;
            if (path.StartsWith("~/"))
                path = path.Substring(2);

            return System.IO.Path.GetFullPath(System.IO.Path.Combine(rootPath, path));
        }

        /// <summary>
        /// 映射路径（相对Host的ContentRootPath）。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path">相对路径，若以/或\开头将直接返回。</param>
        /// <returns></returns>
        /// <remarks>所有返回值，均处理为<see cref="System.IO.Path.GetFullPath(string)"/>。</remarks>
        public static string MapPath(this IHostingEnvironment host, string path) {
            return MapPath(host, path, true);
        }
        /// <summary>
        /// 映射路径。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path">相对路径，若以/或\开头将直接返回。</param>
        /// <param name="forContentPath">为true时相对Host的ContentRootPath，反之则相对WebRoot。</param>
        /// <returns></returns>
        /// <remarks>所有返回值，均处理为<see cref="System.IO.Path.GetFullPath(string)"/>。</remarks>
        public static string MapPath(this IHostingEnvironment host, string path, bool forContentPath) {
            if (!string.IsNullOrEmpty(path) && (path[0] == '/' || path[0] == '\\'))
                return System.IO.Path.GetFullPath(path);

            string rootPath = forContentPath ? host.ContentRootPath : host.WebRootPath;
            if (rootPath == null)
                rootPath = !forContentPath ? host.ContentRootPath : host.WebRootPath;

            if (string.IsNullOrEmpty(path))
                return rootPath;
            if (path.StartsWith("~/"))
                path = path.Substring(2);

            return System.IO.Path.GetFullPath(System.IO.Path.Combine(rootPath, path));
        }
    }


}
#endif