/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System.Collections;
using System.Globalization;

namespace System {
    /// <summary>
    /// �й�ũ��
    /// </summary>
    public static class ChinaDate {

        #region fields

        private static readonly ChineseLunisolarCalendar china = new ChineseLunisolarCalendar();
        private static readonly Hashtable gHoliday = new Hashtable();
        private static readonly Hashtable nHoliday = new Hashtable();
        private static readonly string[] JQ = { "С��", "��", "����", "��ˮ", "����", "����", "����", "����", "����", "С��", "â��", "����", "С��", "����", "����", "����", "��¶", "���", "��¶", "˪��", "����", "Сѩ", "��ѩ", "����" };
        private static readonly int[] JQData = { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };
        private static readonly string yearTG = " ���ұ����켺�����ɹ�";
        private static readonly string yearSX = " ��ţ������������Ｆ����";
        private static readonly string yearDZ = " �ӳ���î������δ�����纥";

        #endregion

        #region cctor
        static ChinaDate() {
            //��������
            gHoliday.Add("0101", "Ԫ��");
            gHoliday.Add("0214", "���˽�");
            gHoliday.Add("0305", "�׷���");
            gHoliday.Add("0308", "��Ů��");
            gHoliday.Add("0312", "ֲ����");
            gHoliday.Add("0315", "��Ȩ��");
            gHoliday.Add("0401", "���˽�");
            gHoliday.Add("0501", "�Ͷ���");
            gHoliday.Add("0504", "�����");
            gHoliday.Add("0601", "��ͯ��");
            gHoliday.Add("0701", "������");
            gHoliday.Add("0801", "������");
            gHoliday.Add("0910", "��ʦ��");
            gHoliday.Add("1001", "�����");
            gHoliday.Add("1224", "ƽ��ҹ");
            gHoliday.Add("1225", "ʥ����");

            //ũ������
            nHoliday.Add("0101", "����");
            nHoliday.Add("0115", "Ԫ����");
            nHoliday.Add("0505", "�����");
            nHoliday.Add("0815", "�����");
            nHoliday.Add("0909", "������");
            nHoliday.Add("1208", "���˽�");
        }
        #endregion

        #region methods

        #region GetChinaDate
        /// <summary>
        /// ��ȡũ��
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetChinaDate(DateTime dt) {
            if (dt > china.MaxSupportedDateTime || dt < china.MinSupportedDateTime) {
                //���ڷ�Χ��1901 �� 2 �� 19 �� - 2101 �� 1 �� 28 ��
                Symbol.CommonException.ThrowArgumentOutOfRange("dt",string.Format("���ڳ�����Χ��������{0}��{1}֮�䣡", china.MinSupportedDateTime.ToString("yyyy-MM-dd"), china.MaxSupportedDateTime.ToString("yyyy-MM-dd")));
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
        /// ��ȡũ�����
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
        /// ��ȡũ����ţ����ࣺ����
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
        /// ��ȡũ���꣨��ɵ�֧��
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
        ///// ��ȡũ���£���ɵ�֧��
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
        /// ��ȡ�й�ũ����DateTime
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
        /// ��ȡ�������ڵ�DateTime
        /// </summary>
        /// <param name="dt">ũ��DateTime</param>
        /// <returns></returns>
        public static DateTime FromLunarDate(DateTime dt) {
            return china.ToDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
        #endregion

        #region GetMonth
        /// <summary>
        /// ��ȡũ���·�
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

            string szText = "�������������߰˾�ʮ";
            string strMonth = isLeapMonth ? "��" : "";
            if (iMonth <= 10) {
                strMonth += szText.Substring(iMonth - 1, 1);
            } else if (iMonth == 11) {
                strMonth += "ʮһ";
            } else {
                strMonth += "��";
            }
            return strMonth + "��";
        }
        #endregion

        #region GetDay
        /// <summary>
        /// ��ȡũ������
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDay(DateTime dt) {
            int iDay = china.GetDayOfMonth(dt);
            string szText1 = "��ʮإ��";
            string szText2 = "һ�����������߰˾�ʮ";
            string strDay;
            if (iDay == 20) {
                strDay = "��ʮ";
            } else if (iDay == 30) {
                strDay = "��ʮ";
            } else {
                strDay = szText1.Substring((iDay - 1) / 10, 1);
                strDay = strDay + szText2.Substring((iDay - 1) % 10, 1);
            }
            return strDay;
        }
        #endregion
        #region GetSolarTerm
        /// <summary>
        /// ��ȡ����
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
        /// ��ȡ��������
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
        /// ��ȡũ������
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
                strReturn = "��Ϧ";
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