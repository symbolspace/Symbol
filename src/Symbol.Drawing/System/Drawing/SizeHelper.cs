/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.Drawing {
    /// <summary>
    /// Size辅助类。
    /// </summary>
    public class SizeHelper {

        #region methods

        #region Parse
        /// <summary>
        /// 从字符串中解析Size。
        /// </summary>
        /// <param name="value">格式化字符串，可以为32,33、32x33、32*33、32.33、32×33、32|33。</param>
        /// <returns></returns>
        public static Size Parse(string value) {
            if (string.IsNullOrEmpty(value))
                return Size.Empty;
            string[] values = value.Split(',', '，', '×', 'x', '*', '.', '|');
            if (values.Length == 0)
                return Size.Empty;
            Size result = new Size();
            if (values.Length > 0)
                result.Width = TypeExtensions.Convert<int>(values[0], 0);
            if (values.Length > 1)
                result.Height = TypeExtensions.Convert<int>(values[1],0);
            return result;
        }
        #endregion

        #endregion
    }
}