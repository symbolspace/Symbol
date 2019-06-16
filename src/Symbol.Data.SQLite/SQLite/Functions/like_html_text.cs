/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 模糊匹配 html，并且只匹配纯文本，不按html代码来匹配。
    /// </summary>
    [SQLiteFunction(Name = "like_html_text", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class like_html_text : SQLiteFunction {
        public override object Invoke(object[] args) {
            string html = args[0] as string;
            string text = args[1] as string;
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text))
                return false;
            html = Symbol.Text.StringExtractHelper.ClearTag(html);
            if (string.IsNullOrEmpty(html))
                return false;
            return html.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }

#pragma warning restore CS1591
}