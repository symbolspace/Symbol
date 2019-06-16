/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.SQLite {

    /// <summary>
    /// 排序规则
    /// </summary>
    public class CollationSequence {
        #region fields
        private FastWrapper _wraaper;
        #endregion

        #region properties
        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name {
            get {
                return (string)_wraaper.Get("Name");
            }
        }
        /// <summary>
        /// 获取类型
        /// </summary>
        public CollationTypes Type {
            get {
                return TypeExtensions.Convert<CollationTypes>(_wraaper.Get("Type"));
            }
        }
        /// <summary>
        /// 获取编码
        /// </summary>
        public CollationEncodings Encoding {
            get {
                return TypeExtensions.Convert<CollationEncodings>(_wraaper.Get("Encoding"));
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public CollationSequence(object o) {
            SQLiteHelper.LoadAssembly(o.GetType().Assembly);
            _wraaper = new FastWrapper(SQLiteHelper.GetType("System.Data.SQLite.CollationSequence").Type, false) { Instance = o };
        }
        #endregion

        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public int Compare(string s1, string s2) {
            return (int)_wraaper.MethodInvoke("Compare", s1, s2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public int Compare(char[] c1, char[] c2) {
            return (int)_wraaper.MethodInvoke("Compare", c1, c2);
        }
        #endregion
    }

}