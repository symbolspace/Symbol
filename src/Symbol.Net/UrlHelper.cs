/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

 using System;

namespace Symbol.Net {
    /// <summary>
    /// Url辅助类。
    /// </summary>
    public static class UrlHelper {

        #region methods

        #region IsAbsoluteUri
        /// <summary>
        /// 判断是否为绝对网址。
        /// </summary>
        /// <param name="url">需要检查的网址。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsAbsoluteUri(string url) {
            bool result = false;
            try {
                result = new Uri(url, UriKind.RelativeOrAbsolute).IsAbsoluteUri;
            } catch (UriFormatException) {
                try {
                    result = new Uri(HttpUtility.UrlDecode(url), UriKind.RelativeOrAbsolute).IsAbsoluteUri;
                } catch {

                }
            }
            return result;
        }
        #endregion

        #region GetAbsoluteUri
        /// <summary>
        /// 生成绝对网址。
        /// </summary>
        /// <param name="baseUri">基础网址。</param>
        /// <param name="relativeUri">相对网址，如果此为绝对网址，会忽略baseUri参数。</param>
        /// <returns>返回生成后的网址。</returns>
        public static Uri GetAbsoluteUri(Uri baseUri, string relativeUri) {
            if (string.IsNullOrEmpty(relativeUri))
                return baseUri;
            Uri result = null;
            try {
                result = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
            } catch (UriFormatException) {
                try {
                    result = new Uri(HttpUtility.UrlDecode(relativeUri), UriKind.RelativeOrAbsolute);
                } catch {
                    return null;
                }
            }
            if (result.IsAbsoluteUri) {
                return result;
            } else {
                return new Uri(baseUri, result);
            }

        }
        #endregion

        #endregion

    }

}