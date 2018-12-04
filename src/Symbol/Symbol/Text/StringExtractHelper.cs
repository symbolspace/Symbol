/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Text {
    /// <summary>
    /// 文本提取辅助类。
    /// </summary>
    public class StringExtractHelper {

        #region fields
        private static readonly string _tagReplacePattern = "<[^>]*>";
        #endregion

        #region methods

        #region TagReplace
        /// <summary>
        /// 替换Html代码中的标签
        /// </summary>
        /// <param name="html">Html代码</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="replace">替换为，通常是string.Empty，用于移除指定的标签</param>
        /// <returns>返回替换后的Html代码</returns>
        public static string TagReplace(string html, string tagName, string replace) {
            if (string.IsNullOrEmpty(html))
                return html;
            return System.Text.RegularExpressions.Regex.Replace(html, "<" + tagName + "[^>]*>", replace, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        #endregion

        #region ClearTag
        /// <summary>
        /// 清除Html代码中的所有标签（清除<c>&amp;nbsp;</c>，清除首尾空白符）
        /// </summary>
        /// <param name="html">Html代码</param>
        /// <returns>返回处理后的Html代码</returns>
        public static string ClearTag(string html) {
            return ClearTag(html, true, true);
        }
        /// <summary>
        /// 清除Html代码中的所有标签
        /// </summary>
        /// <param name="html">Html代码</param>
        /// <param name="clearNbsp">清除<c>&amp;nbsp;</c></param>
        /// <param name="trim">清除首尾的空白符</param>
        /// <returns>返回处理后的Html代码</returns>
        public static string ClearTag(string html, bool clearNbsp, bool trim) {
            if (string.IsNullOrEmpty(html))
                return html;
            string result = System.Text.RegularExpressions.Regex.Replace(html, _tagReplacePattern, string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            if (result != null && clearNbsp)
                result = result.Replace("&nbsp;", string.Empty);
            if (result != null && trim)
                result = result.Trim();
            return result;
        }
        #endregion

        #region RulesStringsStartEnd
        /// <summary>
        /// 提取字符串列表,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <returns>返回提取的字符串列表</returns>
        public static System.Collections.Generic.IEnumerable<string> RulesStringsStartEnd(string text, string start, params string[] ends) {
            return RulesStringsStartEnd(text, start, ends, 0, false, false, false);
        }
        /// <summary>
        /// 提取字符串列表,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <returns>返回提取的字符串列表</returns>
        public static System.Collections.Generic.IEnumerable<string> RulesStringsStartEnd(string text, string start, int startIndex, params string[] ends) {
            return RulesStringsStartEnd(text, start, ends, startIndex, false, false, false);
        }
        /// <summary>
        /// 提取字符串列表,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="isCantionStart">是否包含查找字符串</param>
        /// <param name="isCantionEnd">是否包含结尾字符串</param>
        /// <param name="isCase">是否区分大小写</param>
        /// <returns>返回提取的字符串列表</returns>
        public static System.Collections.Generic.IEnumerable<string> RulesStringsStartEnd(string text, string start, string[] ends, int startIndex, bool isCantionStart, bool isCantionEnd, bool isCase) {
            int endIndex = 0;
            string lastText = StringsStartEnd(text, start, ends, endIndex, isCantionStart, isCantionEnd, isCase, out endIndex);
            while (endIndex > 0) {
                yield return lastText;
                lastText = StringsStartEnd(text, start, ends, endIndex, isCantionStart, isCantionEnd, isCase, out endIndex);
            }
        }
        #endregion

        #region StringsStartEnd
        /// <summary>
        /// 提取字符串,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <returns>返回提取的字符串</returns>
        public static string StringsStartEnd(string text, string start, params string[] ends) {
            return StringsStartEnd(text, start, ends, 0, false, false, false);
        }
        /// <summary>
        /// 提取字符串,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <returns>返回提取的字符串</returns>
        public static string StringsStartEnd(string text, string start, int startIndex, params string[] ends) {
            return StringsStartEnd(text, start, ends, startIndex, false, false, false);
        }
        /// <summary>
        /// 提取字符串,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="isCantionStart">是否包含查找字符串</param>
        /// <param name="isCantionEnd">是否包含结尾字符串</param>
        /// <param name="isCase">是否区分大小写</param>
        /// <returns>返回提取的字符串</returns>
        public static string StringsStartEnd(string text, string start, string[] ends, int startIndex, bool isCantionStart, bool isCantionEnd, bool isCase) {
            int i = 0;
            return StringsStartEnd(text, start, ends, startIndex, isCantionStart, isCantionEnd, isCase, out i);
        }
        /// <summary>
        /// 提取字符串,开始与结尾
        /// </summary>
        /// <param name="text">提取来源</param>
        /// <param name="start">查找字符串</param>
        /// <param name="ends">结尾字符串列表,以找到的第一个字符串为结尾</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="isCantionStart">是否包含查找字符串</param>
        /// <param name="isCantionEnd">是否包含结尾字符串</param>
        /// <param name="isCase">是否区分大小写</param>
        /// <param name="endIndex">结尾字符串停在位置[此输出可以用于遍历]</param>
        /// <returns>返回提取的字符串</returns>
        public static string StringsStartEnd(string text, string start, string[] ends, int startIndex, bool isCantionStart, bool isCantionEnd, bool isCase, out int endIndex) {
            int startIndex2 = StringIndexOf(text, ref start, startIndex, isCase);

            if (startIndex2 == -1) {
                endIndex = -1;
                return null;
            } else if (!string.IsNullOrEmpty(start) && !isCantionStart) {
                startIndex2 += start.Length;
            }

            endIndex = ends == null || ends.Length == 0 ? 0 : -1;
            bool b = isCantionStart && !string.IsNullOrEmpty(start);
            if (endIndex != 0) {
                foreach (string item in ends) {
                    string end = item;
                    int i = StringIndexOf(text, ref end, startIndex2 + (b ? start.Length : 0), isCase);
                    if (i != -1) {
                        if (i != 0 && isCantionEnd) {
                            i += end.Length;
                        }
                        endIndex = i;
                        break;
                    }
                }
            }
            if (endIndex == -1) {
                return null;
            } else if (endIndex == 0) {
                if (startIndex == 0) {
                    return null;
                } else {
                    endIndex = text.Length;
                }
            }

            if (startIndex2 == 0 && endIndex == text.Length) {
                return text;
            } else {
                return text.Substring(startIndex2, endIndex - startIndex2);
            }

        }
        #endregion

        #region StringIndexOf
        /// <summary>
        /// 获取一个字符串[支持"[*]"跳跃]在指定的字符串中的位置
        /// </summary>
        /// <param name="text"></param>
        /// <param name="find">查找来源</param>
        /// <param name="isCase">查找字符串[由于"[*]"可以跳跃部分字符串,在查找时此值会更改为实际找到的值]</param>
        /// <returns>返回位置,未找到时为 -1</returns>
        public static int StringIndexOf(string text, ref string find, bool isCase) {
            return StringIndexOf(text, ref find, 0, isCase);
        }
        /// <summary>
        /// 获取一个字符串[支持"[*]"跳跃]在指定的字符串中的位置[指定开始位置]
        /// </summary>
        /// <param name="text">查找来源</param>
        /// <param name="find">查找字符串[由于"[*]"可以跳跃部分字符串,在查找时此值会更改为实际找到的值]</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="isCase">区分大小写</param>
        /// <returns>返回位置,未找到时为 -1</returns>
        public static int StringIndexOf(string text, ref string find, int startIndex, bool isCase) {
            if (string.IsNullOrEmpty(text)) {
                return -1;
            }

            if (string.IsNullOrEmpty(find)) {
                return 0;
            }

            if (find.IndexOf("[*]") > 0) {
                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(text, @find.Replace("[*]", ".*"), isCase ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (string.IsNullOrEmpty(m.Value)) {
                    return -1;
                } else {
                    find = m.Value;
                }
            }

            return text.IndexOf(find, startIndex, isCase ? System.StringComparison.CurrentCulture : System.StringComparison.CurrentCultureIgnoreCase);
        }
        #endregion

        #endregion

    }
}