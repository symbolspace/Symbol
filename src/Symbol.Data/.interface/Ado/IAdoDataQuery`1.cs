/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// ADO.NET 数据查询接口（泛型）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public interface IAdoDataQuery<T> 
        : IDataQuery<T> {

    }
}