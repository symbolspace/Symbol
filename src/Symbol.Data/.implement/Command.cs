/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 命令基类
    /// </summary>
    public abstract class Command : ICommand {

        #region fields
        private IDataContext _dataContext;
        private ICommandParameterList _parameters;
        #endregion

        #region properties
        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return ThreadHelper.InterlockedGet(ref _dataContext); } }

        /// <summary>
        /// 获取参数列表。
        /// </summary>
        public ICommandParameterList Parameters { get { return ThreadHelper.InterlockedGet(ref _parameters); } }
        
        /// <summary>
        /// 获取或设置当前查询命令语句。
        /// </summary>
        public virtual string Text { get; set; }


        /// <summary>
        /// 获取或设置当前超时时间（秒，不会影响到DataContext）。
        /// </summary>
        public abstract int Timeout { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建CommandParameterList实例。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        public Command(IDataContext dataContext) {
            _dataContext = dataContext;
            dataContext?.DisposableObjects.Add(this);
            _parameters = CreateCommandParameterList();
        }
        #endregion

        #region methods

        /// <summary>
        /// 创建参数列表。
        /// </summary>
        /// <returns></returns>
        protected abstract ICommandParameterList CreateCommandParameterList();

        #region ExecuteScalar
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <returns>返回查询结果。</returns>
        public virtual object ExecuteScalar() {
            return ExecuteScalar(Text);
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回查询结果。</returns>
        public abstract object ExecuteScalar(string commandText);
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <returns>返回查询结果。</returns>
        public virtual T ExecuteScalar<T>() {
            object value = ExecuteScalar();
            if (value == null || value is System.DBNull) {
                return default(T);
            }
            return TypeExtensions.Convert<T>(value);
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回查询结果。</returns>
        public virtual T ExecuteScalar<T>(string commandText) {
            object value = ExecuteScalar(commandText);
            if (value == null || value is System.DBNull) {
                return default(T);
            }
            return TypeExtensions.Convert<T>(value);
        }

        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回查询结果。</returns>
        public virtual T ExecuteScalar<T>(T defaultValue) where T : struct {
            object value = ExecuteScalar();
            if (value == null || value is System.DBNull) {
                return default(T);
            }
            return TypeExtensions.Convert<T>(value, defaultValue);
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="commandText">命令文本</param>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回查询结果。</returns>
        public virtual T ExecuteScalar<T>(string commandText, T defaultValue) where T : struct {
            object value = ExecuteScalar(commandText);
            if (value == null || value is System.DBNull) {
                return default(T);
            }
            return TypeExtensions.Convert<T>(value, defaultValue);
        }
        #endregion

        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        public virtual int ExecuteNonQuery() {
            return ExecuteNonQuery(Text);
        }
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        public abstract int ExecuteNonQuery(string commandText);

        /// <summary>
        /// 调用函数
        /// </summary>
        /// <returns>返回此函数的执行结果</returns>
        public abstract object ExecuteFunction();
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <returns>返回此函数的执行结果</returns>
        public virtual T ExecuteFunction<T>() {
            object value = ExecuteFunction();
            if (value == null || value is System.DBNull) {
                return default(T);
            }
            return TypeExtensions.Convert<T>(value);
        }
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回此函数的执行结果</returns>
        public virtual T ExecuteFunction<T>(T defaultValue) where T : struct {
            object value = ExecuteFunction();
            if (value == null || value is System.DBNull) {
                return default(T);
            }
            return TypeExtensions.Convert<T>(value, defaultValue);
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <returns>返回存储过程的值。</returns>
        public abstract object ExecuteStoredProcedure();


        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns>返回数据查询读取器。</returns>
        public virtual IDataQueryReader ExecuteReader() {
            return ExecuteReader(Text);
        }
        /// <summary>
        /// 执行查询。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回数据查询读取器。</returns>
        public abstract IDataQueryReader ExecuteReader(string commandText);

        #region Dispose
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public virtual void Dispose() {
            ThreadHelper.InterlockedSet(ref _parameters, null)?.Dispose();
            ThreadHelper.InterlockedSet(ref _dataContext, null);
            Text = null;
        }

        #endregion


        #endregion

    }


}