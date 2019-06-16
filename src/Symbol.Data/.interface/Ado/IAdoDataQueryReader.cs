/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// ADO.NET 数据查询读取器接口。
    /// </summary>
    public interface IAdoDataQueryReader
        : IDataQueryReader
    {
        /// <summary>
        /// 获取Ado DataReader对象。
        /// </summary>
        IDataReader DataReader { get; }

        /// <summary>
        /// 获取ADO.NET DbCommand对象。
        /// </summary>
        IDbCommand DbCommand { get; }
    }
}