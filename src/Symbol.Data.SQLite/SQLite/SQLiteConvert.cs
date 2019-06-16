/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.SQLite {

    /// <summary>
    /// SQLite转换器
    /// </summary>
    public class SQLiteConvert {
        #region fields
        private FastWrapper _wrapper;
        private static readonly FastWrapper[] _types = new FastWrapper[1];
        #endregion
        #region properties
        internal static FastWrapper Type { get { return _types[0]; } set { _types[0] = value; } }
        #endregion
        #region ctor
        internal SQLiteConvert(FastWrapper wrapper) {
            _wrapper = wrapper;
        }
        #endregion
        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Split(string source, char separator) {
            return _types[0] == null ? new string[0] : (string[])_types[0].MethodInvoke("Split", source, separator);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ToBoolean(object source) {
            return _types[0] == null ? false : (bool)_types[0].MethodInvoke("ToBoolean", source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ToBoolean(string source) {
            return _types[0] == null ? false : (bool)_types[0].MethodInvoke("ToBoolean", source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceText"></param>
        /// <returns></returns>
        public static byte[] ToUTF8(string sourceText) {
            return _types[0] == null ? new byte[0] : (byte[])_types[0].MethodInvoke("ToUTF8", sourceText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativestring"></param>
        /// <param name="nativestringlen"></param>
        /// <returns></returns>
        public static string UTF8ToString(System.IntPtr nativestring, int nativestringlen) {
            return _types[0] == null ? "" : (string)_types[0].MethodInvoke("UTF8ToString", nativestring, nativestringlen);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="julianDay"></param>
        /// <returns></returns>
        public System.DateTime ToDateTime(double julianDay) {
            return _wrapper == null ? System.DateTime.Now : (System.DateTime)_wrapper.MethodInvoke("ToDateTime", julianDay);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateText"></param>
        /// <returns></returns>
        public System.DateTime ToDateTime(string dateText) {
            return _wrapper == null ? System.DateTime.Now : (System.DateTime)_wrapper.MethodInvoke("ToDateTime", dateText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ToJulianDay(System.DateTime value) {
            return _wrapper == null ? 0D : (double)_wrapper.MethodInvoke("ToJulianDay", value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public string ToString(System.DateTime dateValue) {
            return _wrapper == null ? "" : (string)_wrapper.MethodInvoke("ToString", dateValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativestring"></param>
        /// <param name="nativestringlen"></param>
        /// <returns></returns>
        public virtual string ToString(System.IntPtr nativestring, int nativestringlen) {
            return _wrapper == null ? "" : (string)_wrapper.MethodInvoke("ToString", nativestring, nativestringlen);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeValue"></param>
        /// <returns></returns>
        public byte[] ToUTF8(System.DateTime dateTimeValue) {
            return _wrapper == null ? new byte[0] : (byte[])_wrapper.MethodInvoke("ToUTF8", dateTimeValue);
        }
        #endregion
    }

}