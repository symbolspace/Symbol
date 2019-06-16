/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 命令。
    /// </summary>
    public abstract class AdoCommand : Command, IAdoCommand {

        #region fields
        private System.Collections.Concurrent.ConcurrentQueue<AdoCommandCache> _list_commands;
        #endregion

        #region properties
       
        /// <summary>
        /// 获取或设置当前超时时间（秒，不会影响到DataContext）。
        /// </summary>
        public override int Timeout { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建AdoCommand实例。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        public AdoCommand(IDataContext dataContext)
            : base(dataContext) {
            _list_commands = new System.Collections.Concurrent.ConcurrentQueue<AdoCommandCache>();
        }
        #endregion

        #region methods

        #region CreateDbCommand
        /// <summary>
        /// 创建DbCommand对象。
        /// </summary>
        /// <returns>返回DbCommandCache对象。</returns>
        protected virtual AdoCommandCache CreateDbCommand() {
            return CreateDbCommand(Text);
        }
        /// <summary>
        /// 创建DbCommand对象。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回DbCommandCache对象。</returns>
        protected virtual AdoCommandCache CreateDbCommand(string commandText) {
            var connection = (IAdoConnection)DataContext?.Connections?.Take();
            if (connection == null)
                return null;
            IDbCommand dbCommand = null;
            try {
                connection.Open();
                dbCommand = connection.DbConnection.CreateCommand();
                var transcation = connection.DbTransaction;
                if (transcation != null)
                    dbCommand.Transaction = transcation;
                if (Timeout > 0)
                    dbCommand.CommandTimeout = Timeout;
                dbCommand.CommandText = commandText;

                FillDbCommandParameters(dbCommand);

                var cache = new AdoCommandCache() { DbCommand = dbCommand, Connection = connection };
                var list = ThreadHelper.InterlockedGet(ref _list_commands);
                if (list == null) {
                    DestroyDbCommand(cache);
                    return null;
                }
                list.Enqueue(cache);
                return cache;
            } catch {
                DestroyConnection(connection);
                DestroyDbCommand(dbCommand);
                throw;
            }
        }
        #endregion
        #region FillDbCommandParameters
        /// <summary>
        /// 填充DbCommand参数列表。
        /// </summary>
        /// <param name="dbCommand">DbCommand对象。</param>
        protected virtual void FillDbCommandParameters(System.Data.IDbCommand dbCommand) {

            bool autoParameterName = dbCommand.CommandText.IndexOf('?') > 0 
                                        && dbCommand.CommandText.IndexOf(
                                            DataContext?.Provider?.Dialect.ParameterNameGrammar("p"), 
                                            System.StringComparison.OrdinalIgnoreCase) == -1;
            var list = Parameters;
            if (list == null)
                return;
            if (autoParameterName) {
                string[] lines = dbCommand.CommandText.Split('?');
                for (int i = 0; i < list.Count; i++) {
                    var parameter = CreateDbCommandParameter(dbCommand, list[i]);
                    if (i < lines.Length) {
                        lines[i] += parameter.ParameterName;
                    }
                }
                dbCommand.CommandText = string.Join("", lines);
            } else {
                foreach (var item in list) {
                    dbCommand.Parameters.Add(CreateDbCommandParameter(dbCommand, item));
                }
            }
        }
        #endregion
        #region CreateDbCommandParameter
        /// <summary>
        /// 创建DbCommand参数。
        /// </summary>
        /// <param name="dbCommand">DbCommand对象。</param>
        /// <param name="commandParameter">命令参数对象。</param>
        /// <returns>返回ADO.NET命令参数对象。</returns>
        protected virtual System.Data.IDbDataParameter CreateDbCommandParameter(System.Data.IDbCommand dbCommand, CommandParameter commandParameter) {
            var result = dbCommand.CreateParameter();

            if (commandParameter.IsReturn) {
                result.Direction = ParameterDirection.ReturnValue;
            } else {
                result.ParameterName = commandParameter.Name;
                if (commandParameter.IsOut)
                    result.Direction = ParameterDirection.Output;
                if (commandParameter.Value == null) {
                    result.Value = System.DBNull.Value;
                    goto lb_Properties;
                }
                if (commandParameter.RealType.IsArray && commandParameter.RealType.GetElementType() == typeof(byte)) {
                    result.DbType = DbType.Binary;
                    result.Value = commandParameter.Value;
                    goto lb_Properties;
                }
                result.Value = commandParameter.Value;
            }
        lb_Properties:
            foreach (System.Collections.Generic.KeyValuePair<string, object> p in commandParameter.Properties) {
                FastWrapper.Set(result, p.Key, p.Value);
            }
            return result;

        }
        #endregion

        #region DestroyConnection
        /// <summary>
        /// 销毁连接
        /// </summary>
        /// <param name="connection">连接对象。</param>
        protected virtual void DestroyConnection(IAdoConnection connection) {
            if (connection == null)
                return;

            var connections = DataContext?.Connections;
            if (connections == null) {
                connection.Dispose();
            } else {
                connections.Push(connection);
            }
        }
        #endregion
        #region DestroyDbCommand
        /// <summary>
        /// 销毁DbCommand
        /// </summary>
        /// <param name="dbCommand">DbCommand对象。</param>
        protected virtual void DestroyDbCommand(IDbCommand dbCommand) {
            if (dbCommand == null)
                return;
            dbCommand.Cancel();
            dbCommand.Connection = null;
            dbCommand.Transaction = null;
            dbCommand.Dispose();
        }
        /// <summary>
        /// 销毁DbCommand
        /// </summary>
        /// <param name="cache">DbCommandCache对象。</param>
        public virtual void DestroyDbCommand(AdoCommandCache cache) {
            if (cache == null)
                return;
            DestroyDbCommand(cache.GetDbCommand(true));
            DestroyConnection(cache.GetConnection(true));
            cache.Connection = null;
            cache.DbCommand = null;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回查询结果。</returns>
        public override object ExecuteScalar(string commandText) {
            var dbCommandCache = CreateDbCommand(commandText);
            if (dbCommandCache == null)
                return null;
            try {
                dbCommandCache.DbCommand.UpdatedRowSource = UpdateRowSource.None;
                var result = ExecuteScalar(dbCommandCache);
                if (result is System.DBNull) {
                    return null;
                }
                return result;
            } finally {
                DestroyDbCommand(dbCommandCache);
            }
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="dbCommandCache">DbCommandCache对象。</param>
        /// <returns>返回查询结果。</returns>
        protected virtual object ExecuteScalar(AdoCommandCache dbCommandCache) {
            return dbCommandCache?.DbCommand?.ExecuteScalar();
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        public override int ExecuteNonQuery(string commandText) {
            var dbCommandCache = CreateDbCommand(commandText);
            if (dbCommandCache == null)
                return 0;
            try {
                dbCommandCache.DbCommand.UpdatedRowSource = UpdateRowSource.None;
                return ExecuteNonQuery(dbCommandCache);
            } finally {
                DestroyDbCommand(dbCommandCache);
            }
        }
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="dbCommandCache">DbCommandCache对象。</param>
        /// <returns>返回查询结果。</returns>
        protected virtual int ExecuteNonQuery(AdoCommandCache dbCommandCache) {
            return dbCommandCache?.DbCommand?.ExecuteNonQuery()??0;
        }
        #endregion

        #region ExecuteFunction
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <returns>返回此函数的执行结果</returns>
        public override object ExecuteFunction() {
            var list = Parameters;
            if (list == null)
                return null;
            var dbCommandCache = CreateDbCommand();
            if (dbCommandCache == null)
                return null;
            try {
                if (dbCommandCache.DbCommand.CommandText.IndexOf("select ", System.StringComparison.OrdinalIgnoreCase) == -1) {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    builder.AppendLine("select ")
                           .AppendFormat("   {0}\r\n", DataContext?.Provider?.Dialect.PreName(dbCommandCache.DbCommand.CommandText))
                           .AppendLine("    (");

                    for (int i = 0; i < list.Count; i++) {
                        builder.Append(i == 0 ? "     " : "    ,")
                               .AppendLine(list[i].Name);
                    }

                    builder.Append("    )");
                    dbCommandCache.DbCommand.CommandText = builder.ToString();
                }
                dbCommandCache.DbCommand.UpdatedRowSource = UpdateRowSource.None;
                var result = ExecuteScalar(dbCommandCache);
                if (result is System.DBNull) {
                    return null;
                }
                return result;
            } finally {
                DestroyDbCommand(dbCommandCache);
            }

        }
        #endregion

        #region ExecuteStoredProcedure
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <returns>返回存储过程的值。</returns>
        public override object ExecuteStoredProcedure() {
            var list = Parameters;
            if (list == null)
                return null;
            {
                var returnParameter = list.ReturnParameter;
                if (returnParameter == null) {
                    list.Add(new CommandParameter() { IsReturn = true });
                }
            }
            var dbCommandCache = CreateDbCommand();
            if (dbCommandCache == null)
                return null;
            try {
                dbCommandCache.DbCommand.CommandType = CommandType.StoredProcedure;
                dbCommandCache.DbCommand.UpdatedRowSource = UpdateRowSource.None;
                dbCommandCache.DbCommand.ExecuteNonQuery();
                object result = null;
                foreach (System.Data.IDbDataParameter item in dbCommandCache.DbCommand.Parameters) {
                    if (item.Direction == ParameterDirection.ReturnValue) {
                        list.ReturnParameter.Value = item.Value;
                        result = item.Value is System.DBNull ? null : item.Value;
                        continue;
                    }
                    if (item.Direction == ParameterDirection.Output) {
                        var commandParameter = list[item.ParameterName];
                        if (commandParameter != null)
                            commandParameter.Value = item.Value;
                        continue;
                    }
                }

                return result;
            } finally {
                DestroyDbCommand(dbCommandCache);
            }
        }
        #endregion

        #region ExecuteReader
        /// <summary>
        /// 执行查询。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回数据查询读取器。</returns>
        public override IDataQueryReader ExecuteReader(string commandText) {
            var list = Parameters;
            if (list == null)
                return null;
            {
                var returnParameter = list.ReturnParameter;
                if (returnParameter == null) {
                    list.Add(new CommandParameter() { IsReturn = true });
                }
            }
            var dbCommandCache = CreateDbCommand();
            if (dbCommandCache == null)
                return null;
            bool ok = false;
            try {
                //dbCommandCache.DbCommand.CommandType = CommandType.Text;
                //dbCommandCache.DbCommand.UpdatedRowSource = UpdateRowSource.None;
                dbCommandCache.DbCommand.CommandText = commandText;
                var dataReader=dbCommandCache.DbCommand.ExecuteReader();

                var result= CreateDataQueryReader(dataReader, dbCommandCache);
                ok = true;
                return result;
            } finally {
                if(!ok)
                    DestroyDbCommand(dbCommandCache);
            }
        }
        /// <summary>
        /// 创建ADO.NET 查询读取器实例。
        /// </summary>
        /// <param name="dataReader">ADO.NET DataReader对象。</param>
        /// <param name="commandCache">ADO.NET Command 缓存对象。</param>
        protected virtual AdoDataQueryReader CreateDataQueryReader(IDataReader dataReader, AdoCommandCache commandCache) {
            return new AdoDataQueryReader(dataReader, commandCache, this, commandCache.DbCommand.CommandText);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public override void Dispose() {
            var list = ThreadHelper.InterlockedSet(ref _list_commands, null);
            if (list != null) {
                while (list.TryDequeue(out AdoCommandCache dbCommandCache)) {
                    DestroyDbCommand(dbCommandCache);
                }
            }
            base.Dispose();
        }

        #endregion

        #endregion

    }


}