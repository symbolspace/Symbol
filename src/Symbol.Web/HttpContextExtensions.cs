#if netcore
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Symbol;

namespace Microsoft.AspNetCore.Http {

    /// <summary>
    /// HttpContext 扩展类。
    /// </summary>
    public static class HttpContextExtensions {

        /// <summary>
        /// 获取Host对象。
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static IHostingEnvironment GetHost(this HttpContext httpContext) {
            return httpContext.RequestServices.GetService<IHostingEnvironment>();
        }
        /// <summary>
        /// 获取配置对象。
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FastObject GetConfig(this HttpContext httpContext, string file) {
            return System.IO.Path.Combine(httpContext.GetHost().ContentRootPath, file);
        }
        /// <summary>
        /// 获取客户IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientUserIP(this HttpContext context) {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip)) {
                if (context.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
                else
                    ip = context.Connection.RemoteIpAddress.MapToIPv6().ToString();
            }

            return ip;
        }
    }
    
   
}
#endif