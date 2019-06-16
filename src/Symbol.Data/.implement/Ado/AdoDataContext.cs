/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 数据上下文。
    /// </summary>
    public abstract class AdoDataContext : DataContext, IAdoDataContext {

        #region ctor
        /// <summary>
        /// 创建 AdoDataContext 的实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public AdoDataContext(IConnection connection)
            : base(connection) {
        }
        #endregion


        #region methods


        #endregion

    }


}