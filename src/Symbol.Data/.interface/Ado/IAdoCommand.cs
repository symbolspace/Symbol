/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// ADO.NET 命令接口
    /// </summary>
    public interface IAdoCommand 
        : ICommand {
        /// <summary>
        /// 销毁DbCommand
        /// </summary>
        /// <param name="cache">DbCommandCache对象。</param>
        void DestroyDbCommand(AdoCommandCache cache);
    }

    /// <summary>
    /// ADO.NET Command 缓存对象。
    /// </summary>
    public class AdoCommandCache {
        private IDbCommand _dbCommand;
        private IAdoConnection _connection;

        /// <summary>
        /// 获取或设置DbCommand对象。
        /// </summary>
        public IDbCommand DbCommand {
            get { return ThreadHelper.InterlockedGet(ref _dbCommand); }
            set { ThreadHelper.InterlockedSet(ref _dbCommand, value); }
        }
        /// <summary>
        /// 获取或设置连接对象。
        /// </summary>
        public IAdoConnection Connection {
            get { return ThreadHelper.InterlockedGet(ref _connection); }
            set { ThreadHelper.InterlockedSet(ref _connection, value); }
        }


        /// <summary>
        /// 获取DbCommand对象。
        /// </summary>
        /// <param name="clear">是否获取后清空。</param>
        /// <returns>返回最新的值。</returns>
        public IDbCommand GetDbCommand(bool clear) {
            if (clear)
                return ThreadHelper.InterlockedSet(ref _dbCommand, null);
            return DbCommand;
        }
        /// <summary>
        /// 获取连接对象。
        /// </summary>
        /// <param name="clear">是否获取后清空。</param>
        /// <returns>返回最新的值。</returns>
        public IAdoConnection GetConnection(bool clear) {
            if (clear)
                return ThreadHelper.InterlockedSet(ref _connection, null);
            return Connection;
        }
    }

}