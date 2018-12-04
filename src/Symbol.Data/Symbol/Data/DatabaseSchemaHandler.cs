/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Reflection;

namespace Symbol.Data {
   
    /// <summary>
    /// 数据库架构处理对象。
    /// </summary>
    public abstract class DatabaseSchemaHandler {

        #region fields
        private DatabaseSchemaAttribute _attribute;
        #endregion

        #region properties
        /// <summary>
        /// 获取特性对象。
        /// </summary>
        public DatabaseSchemaAttribute Attribute { get { return _attribute; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建DatabaseSchemaHandler实例。
        /// </summary>
        public DatabaseSchemaHandler() {
            DatabaseSchemaAttribute[] attributes = (DatabaseSchemaAttribute[])this.GetType().GetCustomAttributes(typeof(DatabaseSchemaAttribute), true);
            if (attributes.Length > 0)
                _attribute = attributes[0];
            else
                _attribute = new DatabaseSchemaAttribute(null, -1, null);
        }
        #endregion

        #region methods

        #region Process
        /// <summary>
        /// 数据库架构处理。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        public DatabaseSchemaProcessResults Process(DatabaseSchemaContext context) {
            string fullName = TypeExtensions.FullName2(this.GetType());
            if (!_attribute.IsValid) {
                context.Log.Warning("{0}.Attribute.IsValid=false,[{1}]{2},{3}",fullName, _attribute.Order, _attribute.TableName, _attribute.Description);
                return DatabaseSchemaProcessResults.Ignore;
            }
            context.DataContext.ChangeDatabase();
            context.Log.Info("执行 [{0} {1}] {2} {3} {4}",
                EnumExtensions.ToName( _attribute.Type).PadRight(6, ' '),_attribute.Order.ToString().PadRight(8,' '),
                _attribute.TableName.PadRight(32, ' '), _attribute.Description.PadRight(32, ' '),
                fullName);
            DatabaseSchemaProcessResults result;
            try {
                result = OnProcess(context);
                context.Log.Info("     [{0}] {1}", EnumExtensions.ToName(result), fullName);
            } catch(System.Exception error) {
                result = DatabaseSchemaProcessResults.Error;
                context.Log.Info("     [{0}] {1}", EnumExtensions.ToName(result), fullName);
                context.Log.Error(LogBase.ExceptionToString(error));
            }
            return result;
        }
        #endregion
        #region OnProcess
        /// <summary>
        /// 数据库架构处理。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        /// <returns>返回处理结果。</returns>
        protected abstract DatabaseSchemaProcessResults OnProcess(DatabaseSchemaContext context);
        #endregion

        #endregion
    }

}