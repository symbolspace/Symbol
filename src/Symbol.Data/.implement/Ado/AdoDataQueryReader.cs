/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 数据查询读取器。
    /// </summary>
    public class AdoDataQueryReader : DataQueryReader, IAdoDataQueryReader {

        #region fields


        private System.Data.IDataReader _dataReader;
        private AdoCommandCache _commandCache;

        #endregion

        #region properties

        /// <summary>
        /// 获取Ado DataReader对象。
        /// </summary>
        public IDataReader DataReader { get { return ThreadHelper.InterlockedGet(ref _dataReader); } }
        /// <summary>
        /// 获取ADO.NET DbCommand对象。
        /// </summary>
        public IDbCommand DbCommand { get { return ThreadHelper.InterlockedGet(ref _commandCache)?.DbCommand; } }
        /// <summary>
        /// 获取读取器是否已关闭。
        /// </summary>
        public override bool IsClosed { get { return DataReader?.IsClosed ?? false; } }

        /// <summary>
        /// 获取当前行的嵌套深度。
        /// </summary>
        /// <remarks>嵌套的级别，最外面的表的深度为零。</remarks>
        public override int Depth { get { return DataReader?.Depth ?? 0; } }
        /// <summary>
        /// 获取读取器当前字段数量。
        /// </summary>
        public override int FieldCount { get { return DataReader?.FieldCount ?? 0; } }

        #endregion


        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="dataReader">ADO.NET DataReader对象。</param>
        /// <param name="commandCache">ADO.NET Command 缓存对象。</param>
        /// <param name="command">命令对象。</param>
        public AdoDataQueryReader(IDataReader dataReader, AdoCommandCache commandCache, ICommand command)
            : base(commandCache?.Connection, command){
            _dataReader = dataReader;
            _commandCache = commandCache;
            command?.DataContext?.DisposableObjects?.Add(dataReader);
        }
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="dataReader">ADO.NET DataReader对象。</param>
        /// <param name="commandCache">ADO.NET Command 缓存对象。</param>
        /// <param name="command">命令对象。</param>
        /// <param name="commandText">当前查询命令语句。</param>
        public AdoDataQueryReader(IDataReader dataReader, AdoCommandCache commandCache, ICommand command, string commandText) 
            :base(commandCache?.Connection, command,commandText){
            _dataReader = dataReader;
            _commandCache = commandCache;
            command?.DataContext?.DisposableObjects?.Add(dataReader);
        }
        #endregion

        #region methods


        /// <summary>
        /// 获取指定字段当前从0开始的索引顺序。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回-1。</param>
        /// <returns>返回字段索引顺序，若字段不存在，则为-1。</returns>
        public override int GetIndex(string name) {
            if (string.IsNullOrEmpty(name))
                return -1;
            return DataReader?.GetOrdinal(name) ?? -1;
        }
        /// <summary>
        /// 获取指定索引的字段名称。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应的字段名称，若字段不存在，则为空。</returns>
        public override string GetName(int index) {
            if (index < 0 || index > FieldCount - 1)
                return null;
            return DataReader?.GetName(index);
        }

        /// <summary>
        /// 获取指定索引对应字段的类型。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的类型，若字段不存在，则为空。</returns>
        public override System.Type GetType(int index) {
            if (index < 0 || index > FieldCount - 1)
                return null;
            return DataReader?.GetFieldType(index);
        }


        /// <summary>
        /// 获取指定索引对应字段的数据类型名称。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的数据类型名称，若字段不存在，则为空。</returns>
        public override string GetDataTypeName(int index) {
            if (index < 0 || index > FieldCount - 1)
                return null;
            return DataReader?.GetDataTypeName(index);
        }

        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        public override object GetValue(int index) {
            if (index < 0 || index > FieldCount - 1)
                return null;
            object value= DataReader?.GetValue(index);
            if (IsNullValue(value))
                return null;

            string dataTypeName = GetDataTypeName(index);
            if (string.Equals(dataTypeName, "char(1)", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(dataTypeName, "nchar(1)", StringComparison.OrdinalIgnoreCase)) {
                value = _dataReader.GetChar(index);
            } else if (GetType(index) == typeof(byte[]) && string.Equals(dataTypeName, "timestamp", System.StringComparison.OrdinalIgnoreCase)) {
                byte[] buffer = (byte[])value;
                System.Array.Reverse(buffer);
            }
            return value;
        }

        /// <summary>
        /// 使读取器前进到下一个结果。
        /// </summary>
        /// <returns>如果存在更多结果集，则为true。</returns>
        /// <remarks>用于处理多个结果，默认情况下，数据读取器定位在第一个结果。</remarks>
        public override bool NextResult() {
            return DataReader?.NextResult() ?? false;
        }
        /// <summary>
        /// 让读取器前进到下一条记录。
        /// </summary>
        /// <returns>如果存在更多的记录，则为true。</returns>
        public override bool Read() {
            return DataReader?.Read() ?? false;
        }

        /// <summary>
        /// 关闭读取器。
        /// </summary>
        public override void Close() {
            DataReader?.Close();
        }


        /// <summary>
        /// 尝试转换值。
        /// </summary>
        /// <param name="type">字段类型。</param>
        /// <param name="value">字段的值。</param>
        /// <param name="index">字段的索引值，从0开始。</param>
        /// <param name="targetType">目标类型。</param>
        /// <param name="target">输出转换结果。</param>
        /// <returns>返回尝试结果，为true表示成功。</returns>
        protected override bool TryConvertValue(Type type, object value, int index, Type targetType, out object target) {
            string dataTypeName = GetDataTypeName(index);
            bool isJson = targetType==typeof(object)
                          || string.Equals(dataTypeName, "jsonb", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(dataTypeName, "json", StringComparison.OrdinalIgnoreCase)
                          || (type==typeof(string) && !targetType.IsValueType && !targetType.IsEnum && targetType.IsClass && targetType!=typeof(string));
            if (isJson) {
                if (TryParseJSON(value, targetType, out object jsonObject)) {
                    target = jsonObject;
                    return true;
                }
            }

            return base.TryConvertValue(type, value, index, targetType, out target);
        }


        #region Dispose
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public override void Dispose() {
            var commandCache = ThreadHelper.InterlockedSet(ref _commandCache, null);
            commandCache?.DbCommand?.Cancel();
            (Command as IAdoCommand)?.DestroyDbCommand(commandCache);

            var dataReader=ThreadHelper.InterlockedSet(ref _dataReader, null);
            if (dataReader != null) {
                if (!dataReader.IsClosed)
                    try { dataReader.Close(); } catch { }
                dataReader.Dispose();
            }
            base.Dispose();
        }

        #endregion


        #endregion

    }


}