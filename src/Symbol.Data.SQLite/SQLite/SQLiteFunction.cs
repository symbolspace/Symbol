/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.SQLite {

    /// <summary>
    /// 扩展函数
    /// </summary>
    public abstract class SQLiteFunction {

        #region fields
        private SQLiteConvert _convert = null;
        private FastWrapper _fun = null;
        #endregion

        #region properties
        /// <summary>
        /// 获取转换器
        /// </summary>
        public SQLiteConvert SQLiteConvert {
            get {
                if (_convert == null) {
                    if (_fun == null)
                        return null;
                    object convert = _fun.Get("SQLiteConvert");
                    _convert = new SQLiteConvert(new FastWrapper(SQLiteConvert.Type.Type, false) { Instance = convert });
                }
                return _convert;
            }
        }
        #endregion

        #region ctor
        #endregion

        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ex"></param>
        public void Init(object o, bool ex) {
            SQLiteHelper.LoadAssembly(o.GetType().Assembly);
            _fun = new FastWrapper(SQLiteHelper.GetType(ex ? "System.Data.SQLite.SQLiteFunctionEx" : "System.Data.SQLite.SQLiteFunction").Type, false) { Instance = o };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public virtual int Compare(string param1, string param2) {
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextData"></param>
        /// <returns></returns>
        public virtual object Final(object contextData) {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual object Invoke(object[] args) {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="stepNumber"></param>
        /// <param name="contextData"></param>
        public virtual void Step(object[] args, int stepNumber, ref object contextData) {
        }
        #region GetCollationSequence
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected CollationSequence GetCollationSequence() {
            return new CollationSequence(_fun.Get("GetCollationSequence"));
        }
        #endregion

        #endregion
    }
    
}