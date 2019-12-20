#if netcore

using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http {


    /// <summary>
    /// HttpResponse 扩展类。
    /// </summary>
    public static class HttpResponseExtensions {

        /// <summary>
        /// 输出JSON { code, message, data }
        /// </summary>
        /// <param name="response"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task JsonAsync(this HttpResponse response, int code, string message, object data = null) {
            await response.JsonAsync(new { code, message, data });
        }

        /// <summary>
        /// 输出JSON
        /// </summary>
        /// <param name="response"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static async Task JsonAsync(this HttpResponse response, object json) {
            response.ContentType = "application/json";
            response.StatusCode = 200;
            await response.WriteAsync(JSON.ToJSON(json));
        }


        /// <summary>
        /// 输出Html
        /// </summary>
        /// <param name="response"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static async Task HtmlAsync(this HttpResponse response, string html) {
            response.ContentType = "text/html";
            response.StatusCode = 200;
            await response.WriteAsync(html);
        }
    }
}
#endif