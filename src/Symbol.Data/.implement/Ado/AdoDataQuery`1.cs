/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 数据查询
    /// </summary>
    public class AdoDataQuery<T> : DataQuery<T>, IAdoDataQuery<T> {


        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="command">命令对象。</param>
        /// <param name="type">类型。</param>
        public AdoDataQuery(IDataContext dataContext, ICommand command, System.Type type) 
            :base(dataContext,command,type){
        }
        #endregion


    }

}