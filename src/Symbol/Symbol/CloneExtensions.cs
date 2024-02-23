/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol;

/// <summary>
/// 克隆扩展类
/// </summary>
public static class CloneExtensions {

    #region methods

#if !netcore
    #region Clone
    /// <summary>
    /// 将一个对象进行内存克隆，完全一样的一个对象（二进制模式）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="value">需要克隆的对象，为null直接返回null。</param>
    /// <returns>返回克隆后的对象。</returns>
    public static T Clone<T>(
#if !net20
        this
#endif
        T value) {
        T result = default(T);
        if (value == null)
            return result;
        System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        formatter.Serialize(memoryStream, value);
        memoryStream.Position = 0;
        result = (T)formatter.Deserialize(memoryStream);

        return result;
    }
    #endregion
#endif

    #region CopyToNew
    /// <summary>
    /// 复制属性到另一个对象中（一个新对象）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="model">当前对象。</param>
    /// <param name="excludeProperties">需要排除的属性。</param>
    /// <returns>返回一个新的对象。</returns>
    public static T CopyToNew<T>(
#if !net20
        this
#endif
        object model, params string[] excludeProperties) where T : new() {
        T result = new T();
        CopyTo(model, result, excludeProperties);
        return result;
    }
    /// <summary>
    /// 复制属性到另一个对象中（一个新对象）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="model">当前对象。</param>
    /// <param name="predicate">过滤器。</param>
    /// <returns>返回一个新的对象。</returns>
    public static T CopyToNew<T>(
#if !net20
        this
#endif
        object model, System.Predicate<System.Reflection.PropertyInfo> predicate) where T : new() {
        T result = new T();
        CopyTo(model, result, predicate);
        return result;
    }

    #endregion
    #region CopyTo
    /// <summary>
    /// 复制属性到另一个对象中（可以不同类型）。
    /// </summary>
    /// <param name="model">当前对象。</param>
    /// <param name="toModel">目标对象。</param>
    /// <param name="excludeProperties">需要排除的属性。</param>
    public static void CopyTo(
#if !net20
        this
#endif
        object model, object toModel, params string[] excludeProperties) {
        System.Predicate<System.Reflection.PropertyInfo> predicate = null;
        if (excludeProperties != null && excludeProperties.Length > 0) {
            predicate = (p) => System.Array.IndexOf(excludeProperties, p.Name) == -1;
        }
        CopyTo(model, toModel, predicate);
    }
    /// <summary>
    /// 复制属性到另一个对象中（可以不同类型）。
    /// </summary>
    /// <param name="model">当前对象。</param>
    /// <param name="toModel">目标对象。</param>
    /// <param name="predicate">过滤器。</param>
    public static void CopyTo(
#if !net20
        this
#endif
        object model, object toModel, System.Predicate<System.Reflection.PropertyInfo> predicate) {
        Throw.CheckArgumentNull(model, "model");
        Throw.CheckArgumentNull(toModel, "toModel");
        
        System.Type type = model.GetType();
        System.Type toType = toModel.GetType();

        foreach (System.Reflection.PropertyInfo propertyInfo in type.GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) {
            if (!propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length>0)
                continue;
            System.Reflection.PropertyInfo toPropertyInfo = toType.GetProperty(propertyInfo.Name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (toPropertyInfo == null || !toPropertyInfo.CanWrite)
                continue;
            if (predicate != null && !predicate(propertyInfo))
                continue;
            toPropertyInfo.SetValue(toModel, ConvertExtensions.Convert(propertyInfo.GetValue(model, null), toPropertyInfo.PropertyType), null);
        }
    }
    #endregion

    #endregion

}
