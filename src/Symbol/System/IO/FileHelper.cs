/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace System.IO {
    /// <summary>
    /// File�����ࡣ
    /// </summary>
    public class FileHelper {

        #region fields
        private static readonly string[] _lengthStrings = new string[] { "B", "K", "M", "G", "T","P" };
        #endregion

        #region methods

        #region LengthToString
        /// <summary>
        /// �ļ�����ת��Ϊ����Ĵ�С�������磺1.23M
        /// </summary>
        /// <param name="length">�ļ�����</param>
        /// <returns>����������Ϣ��</returns>
        public static string LengthToString(long length) {
            return LengthToString(length, false);
        }
        /// <summary>
        /// �ļ�����ת��Ϊ����Ĵ�С�������磺1.23M
        /// </summary>
        /// <param name="length">�ļ�����</param>
        /// <param name="speed">Ϊtrueʱ����ʾ����һ���ٶ�ֵ������ĩβ����/S��</param>
        /// <returns>����������Ϣ��</returns>
        public static string LengthToString(long length, bool speed) {
            int tStart=0;
            double tMax= length;
            int tUnit=1024;
            while(tMax>=tUnit){
                tStart++;
                tMax/=tUnit;
                if (tStart == 3) tUnit = 1000;
            }
            string result= tMax.ToString("0.##");
            if (result.EndsWith("."))
                result= result.Substring(0,result.Length-1);
            result+=tStart>=_lengthStrings.Length? "?": _lengthStrings[tStart];
            if (speed)
                result+="/S";
            return result;
        }
        #endregion

        #region Scan
        /// <summary>
        /// ɨ���ļ��б������AppHepler.AppPath��
        /// </summary>
        /// <param name="path">�ļ�·����֧��*ƥ�䣬���·���÷ֺŸ�����������Чʱֱ�ӷ��ؿ��б�</param>
        /// <returns>����ƥ�䵽���ļ��б��ļ�·��Ϊ����·��</returns>
        public static System.Collections.Generic.List<string> Scan(string path) {
            return Scan(path, null);
        }
        /// <summary>
        /// ɨ���ļ��б�
        /// </summary>
        /// <param name="path">�ļ�·����֧��*ƥ�䣬���·���÷ֺŸ�����������Чʱֱ�ӷ��ؿ��б�</param>
        /// <param name="appPath">���Ŀ¼��Ĭ��ΪAppHelper.AppPath</param>
        /// <returns>����ƥ�䵽���ļ��б��ļ�·��Ϊ����·��</returns>
        public static System.Collections.Generic.List<string> Scan(string path, string appPath) {
            var list = new Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(appPath))
                appPath = AppHelper.AppPath;
            if (string.IsNullOrEmpty(path))
                return list;
            string[] paths = path.Split(';','��');
            foreach (string p in paths) {
                if (string.IsNullOrEmpty(p))
                    continue;
                if (p.IndexOf('*') > -1) {
                    try {
                        list.AddRange(Directory.GetFiles(appPath, p, System.IO.SearchOption.TopDirectoryOnly));
                    } catch (System.IO.DirectoryNotFoundException) {
                    }
                } else {
                    string p10;
                    if (p.IndexOf(':') == -1 && !p.StartsWith("~/")) {
                        p10 = System.IO.Path.Combine(appPath, p);
                    } else {
                        p10 = AppHelper.MapPath(p);
                    }
                    if (!System.IO.File.Exists(p10))
                        continue;
                    list.Add(p10);
                }
            }
            return list;
        }
        #endregion

        #endregion

    }
}