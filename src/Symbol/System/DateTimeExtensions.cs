/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace System {
    /// <summary>
    /// DateTime扩展类。
    /// </summary>
    public static class DateTimeExtensions {

        #region fields
        private static readonly System.Globalization.CultureInfo _en_us;
        #endregion

        #region cctor
        static DateTimeExtensions() {
#if netcore
            _en_us = new System.Globalization.CultureInfo("en-US");
#else
            _en_us = System.Globalization.CultureInfo.GetCultureInfo("en-US");
#endif
        }
        #endregion

        #region methods

        #region ToTimeString
        /// <summary>
        /// 输出简短时间描述文本
        /// </summary>
        /// <param name="value">时间</param>
        /// <param name="defaultValue">默认输出内容，如果时间不属于一个小范围时间，将采用此值。</param>
        /// <returns>返回描述文本：半天前、n 小时前、半小时前、n 分钟前、半分钟前、n 秒前、刚刚、昨天 18：39</returns>
        public static string ToTimeString(
#if !net20
            this 
#endif
            DateTime value, string defaultValue) {
            if (value.Date != DateTime.Today) {
                int days = (DateTime.Today - value.Date).Days;
                if (days == 1) {
                    return value.ToString("'昨天'HH:mm:ss");
                } else if (days == 2) {
                    return value.ToString("'前天'HH:mm:ss");
                } else if (days == -1) {
                    return value.ToString("'明天'HH:mm:ss");
                } else if (days == -2) {
                    return value.ToString("'后天'HH:mm:ss");
                } else if (days > 0 && days < 10) {
                    return days + value.ToString("'天前'HH:mm:ss");
                } else if (days < 0 && days > -10) {
                    return Math.Abs(days) + value.ToString("'天后'HH:mm:ss");
                } else {
                    return defaultValue;
                }
            } else {

                TimeSpan span = DateTime.Now - value;
                if (span.TotalHours >= 1) {
                    if (span.TotalHours >= 12)
                        return "半天前";
                    else
                        return span.TotalHours.ToString("0") + "小时前";
                } else if (span.TotalMinutes >= 1) {
                    if (span.TotalMinutes >= 30)
                        return "半小时前";
                    else
                        return span.TotalMinutes.ToString("0") + "分钟前";
                } else if (span.TotalSeconds >= 1) {
                    if (span.TotalSeconds >= 30)
                        return "半分钟前";
                    else
                        return span.TotalSeconds.ToString("0") + "秒前";
                } else {
                    return "刚刚";
                }
            }
        }
        #endregion

        #region ToHourNumber
        /// <summary>
        /// 将日期转换为数字
        /// </summary>
        /// <param name="value">需要转换的日期</param>
        /// <returns>返回转换后的数字，如：2012072801。</returns>
        public static int ToHourNumber(
#if !net20
this 
#endif
            DateTime value) {
            return value.Year * 1000000 + value.Month * 10000 + value.Day * 100 + value.Hour;
        }
        #endregion
        #region ToDayNumber
        /// <summary>
        /// 将日期转换为数字
        /// </summary>
        /// <param name="value">需要转换的日期</param>
        /// <returns>返回转换后的数字，如：20120728。</returns>
        public static int ToDayNumber(
#if !net20
            this 
#endif
            DateTime value) {
            return value.Year * 10000 + value.Month * 100 + value.Day;
        }
        #endregion
        #region ToMonthNumber
        /// <summary>
        /// 将日期转换为数字
        /// </summary>
        /// <param name="value">需要转换的日期</param>
        /// <returns>返回转换后的数字，如：201207。</returns>
        public static int ToMonthNumber(
#if !net20
            this 
#endif
            DateTime value) {
            return value.Year * 100 + value.Month;
        }
        #endregion

        #region WeekRange
        /// <summary>
        /// 获取所在周的区间
        /// </summary>
        /// <param name="value">指定的日期</param>
        /// <returns>返回所在周的周一和周日。</returns>
        public static DateTimeRange WeekRange(
#if !net20
            this 
#endif
            DateTime value) {
             
            DateTime week1Day = value;
            DateTime weekzDay = week1Day;
            switch (week1Day.DayOfWeek) {
                case System.DayOfWeek.Monday:
                    weekzDay = week1Day.AddDays(6);
                    break;
                case DayOfWeek.Tuesday:
                    weekzDay = week1Day.AddDays(5);
                    week1Day = week1Day.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    weekzDay = week1Day.AddDays(4);
                    week1Day = week1Day.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    weekzDay = week1Day.AddDays(3);
                    week1Day = week1Day.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    weekzDay = week1Day.AddDays(2);
                    week1Day = week1Day.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    weekzDay = week1Day.AddDays(1);
                    week1Day = week1Day.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    week1Day = week1Day.AddDays(-6);
                    break;
            }
            return new DateTimeRange() { Begin = week1Day, End = weekzDay };
        }
        #endregion

        #region ToGMT
        /// <summary>
        /// 输出GMT 标准时间格式：ddd, dd MMM yyyy HH:mm:ss 'GMT'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToGMT(
#if !net20
            this
#endif
            System.DateTime? value) {
            if (value == null)
                return "";
            return ToGMT(value.Value);
        }
        /// <summary>
        /// 输出GMT 标准时间格式：ddd, dd MMM yyyy HH:mm:ss 'GMT'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToGMT(
#if !net20
            this
#endif
            System.DateTime value) {
            return ToGMT(value, ' ');
        }
        /// <summary>
        /// 输出GMT 标准时间格式：ddd, dd MMM yyyy HH:mm:ss 'GMT'
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dateSpliter">日期分隔符，默认为“ ”。</param>
        /// <returns></returns>
        public static string ToGMT(
#if !net20
            this
#endif
            System.DateTime value, char dateSpliter=' ') {
            return value.ToUniversalTime().ToString(string.Format(@"ddd, dd{0}MMM{0}yyyy HH:mm:ss 'GMT'",dateSpliter), _en_us);
        }
        #endregion
        #region FromGMT
        /// <summary>
        /// 尝试从GMT标准时间转换：ddd, dd MMM yyyy HH:mm:ss 'GMT'。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>无效转换时返回null。</returns>
        public static System.DateTime? FromGTM(string value) {
            if (string.IsNullOrEmpty(value))
                return null;
            value = StringExtensions.CheckQuoted(value);
            DateTime result;
            if (DateTime.TryParse(value, _en_us, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
                return result;
            return null;
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 日期和时间区间
    /// </summary>
    public class DateTimeRange {

        /// <summary>
        /// 获取或设置起始日期
        /// </summary>
        public System.DateTime Begin { get; set; }
        /// <summary>
        /// 获取或设置结束日期
        /// </summary>
        public System.DateTime End { get; set; }

    }
}