/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Data;
using System.Reflection;

namespace Symbol.Data {
    /// <summary>
    /// IDbCommand的扩展类。
    /// </summary>
    public static class IDbCommandExtensions {

        #region methods

        #region NextParamName
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <param name="command"></param>
        /// <returns>返回下一个参数的名称。</returns>
        public static string NextParamName(
#if !net20
            this
#endif
            IDbCommand command) {
            return NextParamName(command, 1);
        }
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="offset">偏移多少个参数，可能用于预留。</param>
        /// <returns>返回下一个参数的名称。</returns>
        public static string NextParamName(
#if !net20
            this
#endif
            IDbCommand command, int offset) {
            return "@p" + (command.Parameters.Count + offset);
        }
        #endregion
        #region CreateParameter
        /// <summary>
        /// 创建查询参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public static IDbDataParameter CreateParameter(
#if !net20
            this
#endif
            IDbCommand command, string parameterName, object value) {
            IDbDataParameter px = value as IDbDataParameter;
            if (px != null)
                return px;
            CommandParameter p = value as CommandParameter;
            object value2 = p == null ? value : p.Value;

            IDbDataParameter result = command.CreateParameter();
            if (!string.IsNullOrEmpty(parameterName))
                result.ParameterName = parameterName;

            bool processed = false;
            if (value2 == null) {
                Type type2 = p == null ? null : p.RealType;
                if (type2 != null) {
                    result = CreateParameter(command, parameterName, TypeExtensions.DefaultValue(type2));
                    if (type2.IsArray) {
                        result.DbType = DbType.Binary;
                    }
                }
                result.Value = DBNull.Value;

                processed = true;
#if !netcore
            } else if (value2 is DateTime) {
                if (result is System.Data.OleDb.OleDbParameter) {
                    DateTime dateTime = (DateTime)value2;
                    result.Value = dateTime.AddMilliseconds(-dateTime.Millisecond).ToOADate();
                    ((System.Data.OleDb.OleDbParameter)result).OleDbType = System.Data.OleDb.OleDbType.Date;
                    result.DbType = DbType.Double;
                    processed = true;
                }
                //} else if (result.GetType().FullName == "System.Data.SQLite.SQLiteParameter") {
                //    DateTime dateTime = (DateTime)value2;
                //    //result.DbType = DbType.String;
                //    result.Value = dateTime.ToString("s");
                //}
#endif
            } else {
                System.Type type = value2.GetType();
#if netcore
                var typeInfo = type.GetTypeInfo();
#else
                var typeInfo = type;
#endif
                if (typeInfo.IsEnum) {
                    IDbDataParameter result2 = command.CreateParameter();
                    result2.ParameterName = result.ParameterName;
                    result2.Value = TypeExtensions.Convert<long>(value2);
                    result = result2;
                    processed = true;
                } else if (typeInfo.IsClass && type != typeof(string)) {
                    IDbDataParameter result2 = command.CreateParameter();
                    result2.ParameterName = result.ParameterName;
                    result2.Value = Symbol.Serialization.Json.ToString(value2, true);
                    result = result2;
                    if (command.GetType().Namespace.StartsWith("Npgsql")) {
                        FastWrapper.Set(result, "NpgsqlDbType", TypeExtensions.Convert("Json", NpgsqlDatabaseProvider.GetDbType()));
                    }
                    processed = true;
                }
                
            }
            if (p != null && p.Properties != null && p.Properties.Count > 0) {
                foreach (System.Collections.Generic.KeyValuePair<string, object> item in p.Properties) {
                    FastWrapper.Set(result, item.Key, item.Value);
                }
            }
            if (!processed){
                result.Value = value2;
                processed = true;
            }
            
            return result;
        }
        #endregion
        #region AddParameter
        /// <summary>
        /// 添加一个查询参数（无值）。
        /// </summary>
        /// <param name="command"></param>
        /// <returns>返回参数实例。</returns>
        public static IDbDataParameter AddParameter(
#if !net20
            this
#endif
            IDbCommand command) {
            IDbDataParameter result = command.CreateParameter();
            command.Parameters.Add(result);
            return result;
        }
        /// <summary>
        /// 添加一个查询参数（自动命名@p1 @p2）。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public static IDbDataParameter AddParameter(
#if !net20
            this
#endif
            IDbCommand command, object value) {
            IDbDataParameter result = CreateParameter(command,string.Empty, value);
            command.Parameters.Add(result);
            return result;
        }
        /// <summary>
        /// 添加一个查询参数。
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public static IDbDataParameter AddParameter(
#if !net20
            this
#endif
            IDbCommand command, string parameterName, object value) {
            IDbDataParameter result = CreateParameter(command, parameterName, value);
            command.Parameters.Add(result);
            return result;
        }
        #endregion

        #endregion

    }
}