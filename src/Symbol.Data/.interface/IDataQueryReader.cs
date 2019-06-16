/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据查询读取器接口。
    /// </summary>
    public interface IDataQueryReader 
        : System.IDisposable {

        #region properties
        /// <summary>
        /// 获取命令对象。
        /// </summary>
        ICommand Command { get; }
        /// <summary>
        /// 获取当前查询命令语句。
        /// </summary>
        string CommandText { get;  }
        /// <summary>
        /// 获取连接对象。
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// 获取读取器是否已关闭。
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// 获取当前行的嵌套深度。
        /// </summary>
        /// <remarks>嵌套的级别，最外面的表的深度为零。</remarks>
        int Depth { get; }
        /// <summary>
        /// 获取读取器当前字段数量。
        /// </summary>
        int FieldCount { get; }


        /// <summary>
        /// 获取指定字段的值。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段的值，若字段不存在，则为空。</returns>
        object this[string name] { get; }
        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        object this[int index] { get; }

        #endregion


        #region methods

        /// <summary>
        /// 检测指定字段是否存在。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回false。</param>
        /// <returns>如果字段存在，则为true。</returns>
        bool Exists(string name);
        /// <summary>
        /// 获取指定字段当前从0开始的索引顺序。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回-1。</param>
        /// <returns>返回字段索引顺序，若字段不存在，则为-1。</returns>
        int GetIndex(string name);
        /// <summary>
        /// 获取指定索引的字段名称。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应的字段名称，若字段不存在，则为空。</returns>
        string GetName(int index);


        /// <summary>
        /// 获取指定字段的类型。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段类型，若字段不存在，则为空。</returns>
        System.Type GetType(string name);
        /// <summary>
        /// 获取指定索引对应字段的类型。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的类型，若字段不存在，则为空。</returns>
        System.Type GetType(int index);

        /// <summary>
        /// 获取指定字段的数据类型名称。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段数据类型名称，若字段不存在，则为空。</returns>
        string GetDataTypeName(string name);
        /// <summary>
        /// 获取指定索引对应字段的数据类型名称。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的数据类型名称，若字段不存在，则为空。</returns>
        string GetDataTypeName(int index);

        /// <summary>
        /// 获取指定字段的值。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <returns>返回字段的值，若字段不存在，则为空。</returns>
        object GetValue(string name);
        /// <summary>
        /// 获取指定字段的值。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回空。</param>
        /// <param name="type">目标类型，尝试转换为此类型，为空则保持原状。</param>
        /// <returns>返回字段的值，若字段不存在，则为空。</returns>
        object GetValue(string name, System.Type type);
        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        object GetValue(int index);
        /// <summary>
        /// 获取指定对应字段的值。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为空。</param>
        /// <param name="type">目标类型，尝试转换为此类型，为空则保持原状。</param>
        /// <returns>返回索引顺序对应字段的值，若字段不存在，则为空。</returns>
        object GetValue(int index, System.Type type);

        /// <summary>
        /// 检测指定字段的值是否为空、DBNull。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull，则为true。</returns>
        bool IsNullValue(string name);
        /// <summary>
        /// 检测指定索引顺序对应字段的值是否为空、DBNull。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull，则为true。</returns>
        bool IsNullValue(int index);
        /// <summary>
        /// 检测指定字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        bool IsNullOrEmpty(string name);
        /// <summary>
        /// 检测指定字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="name">字段名称，空或空字符串直接返回true。</param>
        /// <param name="trim">是否对文本进行trim操作。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        bool IsNullOrEmpty(string name, bool trim);
        /// <summary>
        /// 检测指定索引顺序对应字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为true。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        bool IsNullOrEmpty(int index);
        /// <summary>
        /// 检测指定索引顺序对应字段的值是否为空、DBNull、空字符串。
        /// </summary>
        /// <param name="index">从0开始的索引，小于0或超出有效值时，则为true。</param>
        /// <param name="trim">是否对文本进行trim操作。</param>
        /// <returns>返回检测结果，如果字段不存在或值为空、DBNull、空字符串，则为true。</returns>
        bool IsNullOrEmpty(int index, bool trim);


        /// <summary>
        /// 使读取器前进到下一个结果。
        /// </summary>
        /// <returns>如果存在更多结果集，则为true。</returns>
        /// <remarks>用于处理多个结果，默认情况下，数据读取器定位在第一个结果。</remarks>
        bool NextResult();
        /// <summary>
        /// 让读取器前进到下一条记录。
        /// </summary>
        /// <returns>如果存在更多的记录，则为true。</returns>
        bool Read();

        /// <summary>
        /// 关闭读取器。
        /// </summary>
        void Close();


        /// <summary>
        /// 映射为实体对象。
        /// </summary>
        /// <param name="type">类型，为空则为字典类型（<see cref="Collections.Generic.NameValueCollection{T}"/>）。</param>
        /// <returns>如果type为基础类型，并且与第0个字段类型相等，则返回该字段的值；否则为type对应的实体对象。</returns>
        object ToObject(System.Type type);


        #endregion
    }
}