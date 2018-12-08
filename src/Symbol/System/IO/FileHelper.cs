/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace System.IO {
    /// <summary>
    /// File辅助类。
    /// </summary>
    public class FileHelper {

        #region fields
        private static readonly string[] _lengthStrings = new string[] { "B", "K", "M", "G", "T","P" };
        #endregion

        #region methods

        #region LengthToString
        /// <summary>
        /// 文件长度转换为精简的大小描述，如：1.23M
        /// </summary>
        /// <param name="length">文件长度</param>
        /// <returns>返因描述信息。</returns>
        public static string LengthToString(long length) {
            return LengthToString(length, false);
        }
        /// <summary>
        /// 文件长度转换为精简的大小描述，如：1.23M
        /// </summary>
        /// <param name="length">文件长度</param>
        /// <param name="speed">为true时，表示这是一个速度值，会在末尾加上/S。</param>
        /// <returns>返因描述信息。</returns>
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
        /// 扫描文件列表（相对于AppHepler.AppPath）
        /// </summary>
        /// <param name="path">文件路径，支持*匹配，多个路径用分号隔开，参数无效时直接反回空列表。</param>
        /// <returns>返回匹配到的文件列表，文件路径为绝对路径</returns>
        public static System.Collections.Generic.List<string> Scan(string path) {
            return Scan(path, null);
        }
        /// <summary>
        /// 扫描文件列表
        /// </summary>
        /// <param name="path">文件路径，支持*匹配，多个路径用分号隔开，参数无效时直接反回空列表。</param>
        /// <param name="appPath">相对目录，默认为AppHelper.AppPath</param>
        /// <returns>返回匹配到的文件列表，文件路径为绝对路径</returns>
        public static System.Collections.Generic.List<string> Scan(string path, string appPath) {
            var list = new Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(appPath))
                appPath = AppHelper.AppPath;
            if (string.IsNullOrEmpty(path))
                return list;
            string[] paths = path.Split(';','；');
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