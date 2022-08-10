#if netcore

using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.AspNetCore.Http {


    /// <summary>
    /// HttpRequest 扩展类。
    /// </summary>
    public static class HttpRequestExtension {

        /// <summary>
        /// 获取客户IP
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string GetClientUserIP(this HttpRequest httpRequest) {
            var ip = httpRequest.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip)) {
                if (httpRequest.HttpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ip = httpRequest.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                else
                    ip = httpRequest.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
            }

            return ip;
        }

        /// <summary>
        /// QueryString、Form、Cookies、ServerVariables，
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static System.Collections.Specialized.NameValueCollection Params(this HttpRequest httpRequest) {
            var list = new System.Collections.Specialized.NameValueCollection();
            foreach (var p in httpRequest.Query) {
                list[p.Key] = p.Value.ToString();
            }
            if (httpRequest.Method == "POST" 
                && httpRequest.HasFormContentType 
                && httpRequest.ContentLength>0
                && (
                     httpRequest.ContentType.IndexOf("form-urlencoded", System.StringComparison.OrdinalIgnoreCase)>-1
                  || httpRequest.ContentType.IndexOf("multi", System.StringComparison.OrdinalIgnoreCase) > -1
                )) {
                foreach (var p in httpRequest.Form) {
                    list[p.Key] = p.Value.ToString();
                }
            }
            if (httpRequest.Cookies != null) {
                foreach (var p in httpRequest.Cookies) {
                    list[p.Key] = p.Value;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取请求参数，Form[POST] Query Cookies Headers
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Param(this HttpRequest httpRequest, string name) {
            string value;
            if (httpRequest.Method == "POST"
                && httpRequest.HasFormContentType
                && httpRequest.ContentLength > 0
                && (
                     httpRequest.ContentType.IndexOf("form-urlencoded", System.StringComparison.OrdinalIgnoreCase) > -1
                  || httpRequest.ContentType.IndexOf("multi", System.StringComparison.OrdinalIgnoreCase) > -1
                )) {
                value = httpRequest.Form[name];
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            value = httpRequest.Query[name];
            if (!string.IsNullOrEmpty(value))
                return value;

            value = httpRequest.Cookies[name];
            if (!string.IsNullOrEmpty(value))
                return value;

            value = httpRequest.Headers[name];
            if (!string.IsNullOrEmpty(value))
                return value;

            return "";
        }

        #region 读取请求数据 BinaryRead
        /// <summary>
        /// 读取请求数据（从0开始读，读取长度为请求内容长度）。
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static async Task<byte[]> BinaryRead(this HttpRequest httpRequest) {
            if (httpRequest.ContentLength == null || httpRequest.ContentLength == 0)
                return new byte[0];
            var value = await BinaryRead(httpRequest, 0, (int)httpRequest.ContentLength.Value);
            return value.Item1;
        }
        /// <summary>
        /// 读取请求数据。
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="index">起始位置</param>
        /// <param name="length">读取长度</param>
        /// <returns>读取到的数据，实际读取长度</returns>
        public static async Task<(byte[],int)> BinaryRead(this HttpRequest httpRequest, int index, int length) {
            int realLength = 0;
            byte[] data = new byte[length];
            if (httpRequest.Body.CanSeek) {
                long original_posistion = httpRequest.Body.Position;
                httpRequest.Body.Position = index;
                realLength = await httpRequest.Body.ReadAsync(data, 0, length);
                httpRequest.Body.Position = original_posistion;
            } else {
                realLength = await httpRequest.Body.ReadAsync(data, 0, length);
            }
            return (data,realLength);
        }
        #endregion
    }

}
#endif