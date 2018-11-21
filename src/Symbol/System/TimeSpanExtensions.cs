/*  
 *  作者：符号空间
 *  ＱＱ：3239078649
 *  主页：http://www.afuhao.com/
 */

namespace System {
    /// <summary>
    /// TimeSpan扩展类。
    /// </summary>
    public static class TimeSpanExtensions {

        #region methods

        #region ToTimeString
        /// <summary>
        /// 输出为时间文本。
        /// </summary>
        /// <param name="time">当前值。</param>
        /// <returns>返回时间文本，格式为：n天 00:00:00.3333，天数大于1才会有n天，末尾的是毫秒值。</returns>
        public static string ToTimeString(
#if !net20
            this 
#endif
            TimeSpan time) {
            return ToTimeString(time, true);
        }
        /// <summary>
        /// 输出为时间文本。
        /// </summary>
        /// <param name="time">当前值。</param>
        /// <param name="showMillseconds">是否显示末尾的毫秒值。</param>
        /// <returns>返回时间文本，格式为：n天 00:00:00.3333，天数大于1才会有n天，末尾的是毫秒值。</returns>
        public static string ToTimeString(
#if !net20
            this 
#endif
            TimeSpan time,bool showMillseconds) {
            string result= string.Format(
                                    "{0}{1:00}:{2:00}:{3:00}",
                                    time.Days > 0 ? time.Days + "天 " : null,
                                    time.Hours,
                                    time.Minutes,
                                    time.Seconds);
            if (showMillseconds && time.Milliseconds>0){
                result+="."+time.Milliseconds;
            }
            return result;
        }
        #endregion

        #endregion

    }
}