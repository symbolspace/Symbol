using System;
using System.Collections.Generic;
using System.Reflection;

namespace Symbol;

/// <summary>
/// 实体属性设置器。
/// </summary>
public class EntityPropertySetter
{

#if NET20 || NET35
    private static readonly IDictionary<Type, IDictionary<string, EntityPropertySetter>> _list_entity_setter_global;
#else
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, IDictionary<string, EntityPropertySetter>> _list_entity_setter_global;
#endif
    static EntityPropertySetter()
    {
#if NET20 || NET35
        _list_entity_setter_global = new Dictionary<Type, IDictionary<string, EntityPropertySetter>>();
#else
        _list_entity_setter_global = new System.Collections.Concurrent.ConcurrentDictionary<Type, IDictionary<string, EntityPropertySetter>>();
#endif
    }


    /// <summary>
    /// 获取或设置属性名称。
    /// </summary>
    public string PropertyName { get; set; }
    /// <summary>
    /// 获取或设置属性类型。
    /// </summary>
    public Type PropertyType { get; set; }
    /// <summary>
    /// 获取或设置特性提供者。
    /// </summary>
    public ICustomAttributeProvider AttributeProvider { get; set; }
    /// <summary>
    /// 获取或设置是否为匿名类型。
    /// </summary>
    public bool IsAnonymousType { get; set; }
    /// <summary>
    /// 获取或设置属性设置器。
    /// </summary>
    public PropertySetter Setter { get; set; }

    /// <summary>
    /// 获取属性清单。
    /// </summary>
    /// <param name="type">实体类型，不能为空。</param>
    /// <returns>返回属性清单，不会返回null。</returns>
    public static IDictionary<string, EntityPropertySetter> GetProperties(Type type)
    {
        Throw.CheckArgumentNull(type, nameof(type));
#if NET20 || NET35
        if (!_list_entity_setter_global.TryGetValue(type, out IDictionary<string, EntityPropertySetter> result))
        {
            lock (_list_entity_setter_global)
            {
                if (!_list_entity_setter_global.TryGetValue(type, out result))
                {
                    result = ScanEntityProperties(type);
                    _list_entity_setter_global.Add(type, result);
                }
            }
        }
        return result;
#else
        return _list_entity_setter_global.GetOrAdd(type, ScanEntityProperties);
#endif
    }
    static IDictionary<string, EntityPropertySetter> ScanEntityProperties(Type type)
    {
        var list = new Dictionary<string, EntityPropertySetter>();
        var isAnonymousType = TypeExtensions.IsAnonymousType(type);
        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.NonPublic))
        {
            var setter = BuildEntityPropertySetter_Property(propertyInfo, isAnonymousType);
            if (setter == null)
                continue;
            setter.IsAnonymousType = isAnonymousType;
            IDictionaryExtensions.SetValue(list, setter.PropertyName, setter);
            IDictionaryExtensions.SetValue(list, $"{setter.PropertyName.ToUpper()}|Upper", setter);
        }
        if (!isAnonymousType)
        {
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.NonPublic))
            {
                var setter = BuildEntityPropertySetter_Field(fieldInfo);
                if (setter == null)
                    continue;
                if (list.ContainsKey(setter.PropertyName))
                    continue;
                setter.IsAnonymousType = isAnonymousType;
                IDictionaryExtensions.SetValue(list, setter.PropertyName, setter);
                IDictionaryExtensions.SetValue(list,$"{setter.PropertyName.ToUpper()}|Upper", setter);
            }
            foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Instance))
            {
                var setters = BuildEntityPropertySetter_Method(methodInfo);
                if (setters == null)
                    continue;
                foreach(var setter in setters)
                {
                    if (list.ContainsKey(setter.PropertyName))
                        continue;
                    setter.IsAnonymousType = isAnonymousType;
                    IDictionaryExtensions.SetValue(list, setter.PropertyName, setter);
                    IDictionaryExtensions.SetValue(list, $"{setter.PropertyName.ToUpper()}|Upper", setter);
                }
            }
        }
        if(type.BaseType!=null && type.BaseType != typeof(object))
        {
            var list_base = GetProperties(type.BaseType);
            foreach(var setter in list_base.Values)
            {
                if (list.ContainsKey(setter.PropertyName))
                    continue;
                IDictionaryExtensions.SetValue(list, setter.PropertyName, setter);
                IDictionaryExtensions.SetValue(list, $"{setter.PropertyName.ToUpper()}|Upper", setter);
            }
        }
        return list;
    }
    static EntityPropertySetter BuildEntityPropertySetter_Property(PropertyInfo propertyInfo, bool isAnonymousType)
    {
        if (propertyInfo.CanWrite)
        {
            return new EntityPropertySetter()
            {
                PropertyName = propertyInfo.Name,
                PropertyType = propertyInfo.PropertyType,
                AttributeProvider = propertyInfo,
#if NET20 || NET35 || NET40
                Setter = (instance, value) => propertyInfo.SetValue(instance, value, null)
#else
                Setter = propertyInfo.SetValue,
#endif
            };
        }
        if (isAnonymousType)
        {
            var args = propertyInfo.DeclaringType.GetConstructors()[0].GetParameters();
            int argIndex = 0;
            ParameterInfo arg = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Name == propertyInfo.Name)
                {
                    argIndex = i;
                    arg = args[i];
                    break;
                }
            }
            return new EntityPropertySetter()
            {
                PropertyName = propertyInfo.Name,
                PropertyType = arg.ParameterType,
                AttributeProvider = arg,
                Setter = (instance, value) => ((object[])instance)[argIndex] = value,
            };
        }
        return null;
    }
    static EntityPropertySetter BuildEntityPropertySetter_Field(FieldInfo fieldInfo)
    {
        if (fieldInfo.IsInitOnly)
            return null;
        return new EntityPropertySetter()
        {
            PropertyName = fieldInfo.Name,
            PropertyType = fieldInfo.FieldType,
            AttributeProvider = fieldInfo,
            Setter = fieldInfo.SetValue
        };
    }
    static List<EntityPropertySetter> BuildEntityPropertySetter_Method(MethodInfo methodInfo)
    {
        var attributes = AttributeExtensions.GetCustomAttributes<PropertySetMethodAttribute>(methodInfo);
        if (attributes.Count==0)
            return null;
        var list = new List<EntityPropertySetter>();
        foreach(var attribute in attributes)
        {
            var args = methodInfo.GetParameters();
            ParameterInfo arg = null;
            PropertySetter setter = null;
            if (methodInfo.IsStatic)
            {
                arg = args[1];
                setter = (instance, value) => methodInfo.Invoke(null, new object[] { instance, value });
            }
            else
            {
                arg = args[0];
                setter = (instance, value) => methodInfo.Invoke(instance, new object[] { value });
            }
            list.Add(new EntityPropertySetter()
            {
                PropertyName = attribute.Name,
                PropertyType = arg.ParameterType,
                AttributeProvider = arg,
                Setter = setter
            });
        }
        return list;
    }

    /// <summary>
    /// 设置属性值。
    /// </summary>
    /// <param name="instance">对象实例，不能为空。</param>
    /// <param name="values">承载属性数据的集合，为空不会抛出异常，可以为JSON文本、JSON对象、匿名对象、字典。</param>
    /// <returns>返回成功设置的属性数量。</returns>
    public static int SetValues(object instance, object values)
    {
        return SetValues(instance, FastObject.As(values));
    }
    /// <summary>
    /// 设置属性值。
    /// </summary>
    /// <param name="instance">对象实例，不能为空。</param>
    /// <param name="values">承载属性数据的集合，为空不会抛出异常。</param>
    /// <returns>返回成功设置的属性数量。</returns>
    public static int SetValues(object instance, IDictionary<string, object> values)
    {
        Throw.CheckArgumentNull(instance, nameof(instance));
        if (values == null || values.Count == 0)
            return 0;
        var properties = GetProperties(instance.GetType());
        int count = 0;
        foreach(var item in values)
        {
            var property = IDictionaryExtensions.GetValue(properties, item.Key);
            if(property == null)
            {
                property = IDictionaryExtensions.GetValue(properties, $"{item.Key.ToUpper()}|Upper");
            }
            if (property == null)
                continue;
            property.Setter(instance, ConvertExtensions.Convert(item.Value, property.PropertyType));
            count++;
        }
        return count;
    }
}
/// <summary>
/// 委托：属性设置器
/// </summary>
/// <param name="instance">对象实例，不能为空。</param>
/// <param name="value">属性值。</param>
public delegate void PropertySetter(object instance, object value);