/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Collections.Generic;
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据上下文NoSQL接口。
    /// </summary>
    public interface IDataContextNoSQL {

        #region FindAll
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        IDataQuery<object> FindAll(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null);
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        IDataQuery<T> FindAll<T>(object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) where T : class;
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        IDataQuery<T> FindAll<T>(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null);
        #endregion
        #region Find
        /// <summary>
        /// 查询一条数据，默认类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        object Find(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null);
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        T Find<T>(object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) where T : class;
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        T Find<T>(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null);
        #endregion
        #region Exists
        /// <summary>
        /// 是否存在(select 1 xxxxx)
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        bool Exists(string collectionName, object condition = null);
        /// <summary>
        /// 是否存在(select 1 xxxxx)
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        bool Exists<TEntity>(object condition = null) where TEntity : class;
        #endregion
        #region Count
        /// <summary>
        /// 求数量
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        long Count(string collectionName, object condition = null);
        /// <summary>
        /// 求数量
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        long Count<TEntity>(object condition = null) where TEntity : class;
        #endregion
        #region Sum
        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Sum<TResult>(string collectionName, string field, object condition = null) where TResult : struct;
        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Sum<TEntity, TResult>(string field, object condition = null) where TEntity : class where TResult : struct;
        #endregion
        #region Min
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Min<TResult>(string collectionName, string field, object condition = null) ;
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Min<TEntity, TResult>(string field, object condition = null) where TEntity : class ;
        #endregion
        #region Max
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Max<TResult>(string collectionName, string field, object condition = null) ;
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Max<TEntity, TResult>(string field, object condition = null) where TEntity : class ;
        #endregion
        #region Average
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Average<TResult>(string collectionName, string field, object condition = null) where TResult : struct;
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        TResult Average<TEntity, TResult>(string field, object condition = null) where TEntity : class where TResult : struct;
        #endregion


        #region Insert
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert<TEntity>(TEntity model) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert<TEntity>(TEntity model, string[] removeFields) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert<TEntity>(TEntity model, InsertCommandBuilderFilter builderFilter) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert<TEntity>(object values) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert<TEntity>(object values, string[] removeFields) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert<TEntity>(object values, InsertCommandBuilderFilter builderFilter) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        TResult Insert<TEntity, TResult>(object values, string[] removeFields) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        TResult Insert<TEntity, TResult>(object values, InsertCommandBuilderFilter builderFilter) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert(string collectionName, object values);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert(string collectionName, object values, string[] removeFields);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        long Insert(string collectionName, object values, InsertCommandBuilderFilter builderFilter);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        TResult Insert<TResult>(string collectionName, object values, string[] removeFields);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        TResult Insert<TResult>(string collectionName, object values, InsertCommandBuilderFilter builderFilter);
        #endregion
        #region Update
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(TEntity model, object condition, CommandQueryFilterDelegate queryFilter = null) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(TEntity model, object condition, string[] removeFields) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(TEntity model, object condition, UpdateCommandBuilderFilter builderFilter) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(TEntity model, object condition, CommandQueryFilterDelegate queryFilter, UpdateCommandBuilderFilter builderFilter = null) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(object values, object condition, CommandQueryFilterDelegate queryFilter = null) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(object values, object condition, string[] removeFields) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(object values, object condition, UpdateCommandBuilderFilter builderFilter) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        int Update<TEntity>(object values, object condition, CommandQueryFilterDelegate queryFilter, UpdateCommandBuilderFilter builderFilter = null) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回更新条数。</returns>
        int Update(string collectionName, object values, object condition, CommandQueryFilterDelegate queryFilter = null);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回更新条数。</returns>
        int Update(string collectionName, object values, object condition, string[] removeFields);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        int Update(string collectionName, object values, object condition, UpdateCommandBuilderFilter builderFilter);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        int Update(string collectionName, object values, object condition, CommandQueryFilterDelegate queryFilter, UpdateCommandBuilderFilter builderFilter = null);
        #endregion
        #region Delete
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回删除条数。</returns>
        int Delete<TEntity>(object condition, CommandQueryFilterDelegate queryFilter = null) where TEntity : class;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回删除条数。</returns>
        int Delete(string collectionName, object condition, CommandQueryFilterDelegate queryFilter = null);
        #endregion

        #region TryInsert
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert<TEntity>(object values, string[] removeFields, DataContextExecuteCallback<long> callback = null) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert<TEntity>(object values, InsertCommandBuilderFilter builderFilter=null, DataContextExecuteCallback<long> callback = null) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert<TEntity, TResult>(object values, string[] removeFields, DataContextExecuteCallback<TResult> callback = null) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert<TEntity, TResult>(object values, InsertCommandBuilderFilter builderFilter=null, DataContextExecuteCallback<TResult> callback = null) where TEntity : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert(string collectionName, object values, string[] removeFields, DataContextExecuteCallback<long> callback = null);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert(string collectionName, object values, InsertCommandBuilderFilter builderFilter=null, DataContextExecuteCallback<long> callback = null);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert<TResult>(string collectionName, object values, string[] removeFields, DataContextExecuteCallback<TResult> callback = null);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryInsert<TResult>(string collectionName, object values, InsertCommandBuilderFilter builderFilter=null, DataContextExecuteCallback<TResult> callback = null);
        #endregion
        #region TryUpdate
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryUpdate<TEntity>(object values, object condition, string[] removeFields, DataContextExecuteCallback<int> callback = null) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryUpdate<TEntity>(object values, object condition, CommandQueryFilterDelegate queryFilter=null, UpdateCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<int> callback = null) where TEntity : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryUpdate(string collectionName, object values, object condition, string[] removeFields, DataContextExecuteCallback<int> callback = null);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryUpdate(string collectionName, object values, object condition, CommandQueryFilterDelegate queryFilter=null, UpdateCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<int> callback = null);
        #endregion
        #region TryDelete
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryDelete<TEntity>(object condition, CommandQueryFilterDelegate queryFilter = null, DataContextExecuteCallback<int> callback = null) where TEntity : class;
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        bool TryDelete(string collectionName, object condition, CommandQueryFilterDelegate queryFilter = null, DataContextExecuteCallback<int> callback = null);
        #endregion

    }
}