/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Collections;
using System.Globalization;

namespace System {
    /// <summary>
    /// 中国农历
    /// </summary>
    public static class ChinaDate {

        #region fields

        private static readonly ChineseLunisolarCalendar china = new ChineseLunisolarCalendar();
        private static readonly Hashtable gHoliday = new Hashtable();
        private static readonly Hashtable nHoliday = new Hashtable();
        private static readonly string[] JQ = { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };
        private static readonly int[] JQData = { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };
        private static readonly string yearTG = " 甲乙丙丁戊己庚辛壬癸";
        private static readonly string yearSX = " 鼠牛虎兔龙蛇马羊猴鸡狗猪";
        private static readonly string yearDZ = " 子丑寅卯辰巳午未申酉戌亥";

        #endregion

        #region cctor
        static ChinaDate() {
            //公历节日
            gHoliday.Add("0101", "元旦");
            gHoliday.Add("0214", "情人节");
            gHoliday.Add("0305", "雷锋日");
            gHoliday.Add("0308", "妇女节");
            gHoliday.Add("0312", "植树节");
            gHoliday.Add("0315", "消权日");
            gHoliday.Add("0401", "愚人节");
            gHoliday.Add("0501", "劳动节");
            gHoliday.Add("0504", "青年节");
            gHoliday.Add("0601", "儿童节");
            gHoliday.Add("0701", "建党节");
            gHoliday.Add("0801", "建军节");
            gHoliday.Add("0910", "教师节");
            gHoliday.Add("1001", "国庆节");
            gHoliday.Add("1224", "平安夜");
            gHoliday.Add("1225", "圣诞节");

            //农历节日
            nHoliday.Add("0101", "春节");
            nHoliday.Add("0115", "元宵节");
            nHoliday.Add("0505", "端午节");
            nHoliday.Add("0815", "中秋节");
            nHoliday.Add("0909", "重阳节");
            nHoliday.Add("1208", "腊八节");
        }
        #endregion

        #region methods

        #region GetChinaDate
        /// <summary>
        /// 获取农历
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetChinaDate(DateTime dt) {
            if (dt > china.MaxSupportedDateTime || dt < china.MinSupportedDateTime) {
                //日期范围：1901 年 2 月 19 日 - 2101 年 1 月 28 日
                Symbol.CommonException.ThrowArgumentOutOfRange("dt",string.Format("日期超出范围！必须在{0}到{1}之间！", china.MinSupportedDateTime.ToString("yyyy-MM-dd"), china.MaxSupportedDateTime.ToString("yyyy-MM-dd")));
            }
            string str = string.Format("{0} {1}{2}", GetYear(dt), GetMonth(dt), GetDay(dt));
            string strJQ = GetSolarTerm(dt);
            if (strJQ != "") {
                str += " (" + strJQ + ")";
            }
            string strHoliday = GetHoliday(dt);
            if (strHoliday != "") {
                str += " " + strHoliday;
            }
            string strChinaHoliday = GetChinaHoliday(dt);
            if (strChinaHoliday != "") {
                str += " " + strChinaHoliday;
            }

            return str;
        }
        #endregion

        #region GetYear
        /// <summary>
        /// 获取农历年份
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetYear(DateTime dt) {
            int yearIndex = china.GetSexagenaryYear(dt);
            int year = china.GetYear(dt);
            int yTG = china.GetCelestialStem(yearIndex);
            int yDZ = china.GetTerrestrialBranch(yearIndex);

            string str = string.Format("[{1}]{2}{3}{0}", year, yearSX[yDZ], yearTG[yTG], yearDZ[yDZ]);
            return str;
        }
        #endregion
        #region GetYearNumber
        /// <summary>
        /// 获取农历年号（属相：龙）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetYearNumber(DateTime dt) {
            int yearIndex = china.GetSexagenaryYear(dt);
            int yDZ = china.GetTerrestrialBranch(yearIndex);
            return yearSX[yDZ].ToString();
        }
        #endregion

        #region GetYearTerrestrialBranch
        /// <summary>
        /// 获取农历年（天干地支）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetYearTerrestrialBranch(DateTime dt) {
            int yearIndex = china.GetSexagenaryYear(dt);
            int yTG = china.GetCelestialStem(yearIndex);
            int yDZ = china.GetTerrestrialBranch(yearIndex);
            return string.Format("{0}{1}", yearTG[yTG], yearDZ[yDZ]);
        }
        #endregion
        #region GetMonthTerrestrialBranch
        ///// <summary>
        ///// 获取农历月（天干地支）
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static string GetMonthTerrestrialBranch(DateTime dt) {
        //    int yearIndex = china.GetSexagenaryYear(dt);
        //    int yTG = china.GetCelestialStem(yearIndex);
        //    int yDZ = china.GetTerrestrialBranch(yearIndex);
        //    return string.Format("{0}{0}", yearTG[yTG], yearDZ[yDZ]);
        //}
        #endregion

        #region GetLunarDate
        /// <summary>
        /// 获取中国农历的DateTime
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetLunarDate(DateTime dt) {
            int year = china.GetYear(dt);
            int iMonth = china.GetMonth(dt);
#if netcore
            int leapMonth = china.GetLeapMonth(year,china.GetEra(dt));
#else
            int leapMonth = china.GetLeapMonth(year);
#endif
            if (leapMonth > 0) {
                if (iMonth > leapMonth)
                    iMonth--;
            }
            return new DateTime(china.GetYear(dt), iMonth, china.GetDayOfMonth(dt), china.GetHour(dt), china.GetMinute(dt), china.GetSecond(dt), (int)china.GetMilliseconds(dt));
        }
        #endregion
        #region FromLunarDate
        /// <summary>
        /// 获取阳历日期的DateTime
        /// </summary>
        /// <param name="dt">农历DateTime</param>
        /// <returns></returns>
        public static DateTime FromLunarDate(DateTime dt) {
            return china.ToDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
        #endregion

        #region GetMonth
        /// <summary>
        /// 获取农历月份
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetMonth(DateTime dt) {
            int year = china.GetYear(dt);
            int iMonth = china.GetMonth(dt);
#if netcore
            int leapMonth = china.GetLeapMonth(year,china.GetEra(dt));
#else
            int leapMonth = china.GetLeapMonth(year);
#endif
            bool isLeapMonth = iMonth == leapMonth;
            if (leapMonth != 0 && iMonth >= leapMonth) {
                iMonth--;
            }

            string szText = "正二三四五六七八九十";
            string strMonth = isLeapMonth ? "闰" : "";
            if (iMonth <= 10) {
                strMonth += szText.Substring(iMonth - 1, 1);
            } else if (iMonth == 11) {
                strMonth += "十一";
            } else {
                strMonth += "腊";
            }
            return strMonth + "月";
        }
        #endregion

        #region GetDay
        /// <summary>
        /// 获取农历日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDay(DateTime dt) {
            int iDay = china.GetDayOfMonth(dt);
            string szText1 = "初十廿三";
            string szText2 = "一二三四五六七八九十";
            string strDay;
            if (iDay == 20) {
                strDay = "二十";
            } else if (iDay == 30) {
                strDay = "三十";
            } else {
                strDay = szText1.Substring((iDay - 1) / 10, 1);
                strDay = strDay + szText2.Substring((iDay - 1) % 10, 1);
            }
            return strDay;
        }
        #endregion
        #region GetSolarTerm
        /// <summary>
        /// 获取节气
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetSolarTerm(DateTime dt) {
            DateTime dtBase = new DateTime(1900, 1, 6, 2, 5, 0);
            DateTime dtNew;
            double num;
            int y;
            string strReturn = "";

            y = dt.Year;
            for (int i = 1; i <= 24; i++) {
                num = 525948.76 * (y - 1900) + JQData[i - 1];
                //num = 525944 * (y - 1900) + JQData[i - 1];
                dtNew = dtBase.AddMinutes(num);
                if (dtNew.DayOfYear == dt.DayOfYear) {
                    strReturn = JQ[i - 1];
                }
            }

            return strReturn;
        }
        #endregion
        #region GetHoliday
        /// <summary>
        /// 获取公历节日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetHoliday(DateTime dt) {
            string strReturn = "";
            object g = gHoliday[dt.Month.ToString("00") + dt.Day.ToString("00")];
            if (g != null) {
                strReturn = g.ToString();
            }

            return strReturn;
        }
        #endregion
        #region GetChinaHoliday
        /// <summary>
        /// 获取农历节日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetChinaHoliday(DateTime dt) {
            string strReturn = "";
            int year = china.GetYear(dt);
            int iMonth = china.GetMonth(dt);
#if netcore
            int leapMonth = china.GetLeapMonth(year,china.GetEra(dt));
#else
            int leapMonth = china.GetLeapMonth(year);
#endif
            int iDay = china.GetDayOfMonth(dt);
            if (china.GetDayOfYear(dt) == china.GetDaysInYear(year)) {
                strReturn = "除夕";
            } else if (leapMonth != iMonth) {
                if (leapMonth != 0 && iMonth >= leapMonth) {
                    iMonth--;
                }
                object n = nHoliday[iMonth.ToString("00") + iDay.ToString("00")];
                if (n != null) {
                    if (strReturn == "") {
                        strReturn = n.ToString();
                    } else {
                        strReturn += " " + n.ToString();
                    }
                }
            }

            return strReturn;
        }
        #endregion

        #endregion

    }

}