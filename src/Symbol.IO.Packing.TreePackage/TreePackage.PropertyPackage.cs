/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Reflection;

namespace Symbol.IO.Packing {
    partial class TreePackage {

        class PropertyPackage: ICustomPackage {
            public static readonly ICustomPackage Instance = new PropertyPackage();


            #region ICustomPackage 成员

            public byte[] Save(object instance) {
                Type type = instance.GetType();
                TreePackage package = new TreePackage();
                package.Attributes.Add("Type", type.AssemblyQualifiedName);
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic| BindingFlags.Instance)) {
                    if (!property.CanRead)
                        continue;
                    if (AttributeExtensions.IsDefined<NonPackageAttribute>(property) || AttributeExtensions.IsDefined<Formatting.IgnoreAttribute>(property))
                        continue;
                    package.Add(property.Name, property.GetValue(instance, new object[0]));
                }
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                    if (AttributeExtensions.IsDefined<NonPackageAttribute>(field) || AttributeExtensions.IsDefined<Formatting.IgnoreAttribute>(field))
                        continue;
                    package.Add(field.Name, field.GetValue(instance));
                }
                return package.Save();
            }

            public object Load(byte[] buffer) {
                TreePackage package = TreePackage.Load(buffer);
                if (!package.Attributes.ContainsKey("Type"))
                    return null;
                Type type = FastWrapper.GetWarpperType((string)package.Attributes["Type"]);
                if (type == null)
                    throw new TypeMismatchException("未能找到类型“" + (string)package.Attributes["Type"] + "”");
                //bool isAnonymousType = type.IsAnonymousType();
                //if (isAnonymousType) {
                //    ConstructorInfo ctorX = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic)[0];
                //    ParameterInfo[] @paramsX = ctorX.GetParameters();
                //    object[] argsX = new object[@paramsX.Length];
                //    for (int i = 0; i < @paramsX.Length; i++) {
                //        argsX[i] = package[@paramsX[i].Name];
                //    }
                //    return FastWrapper.CreateInstance(type, argsX);
                //}
                ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance);
                object[] args = null;
                if (ctors.Length > 0) {
                    ConstructorInfo ctor = ctors[0];
                    ParameterInfo[] @params = ctor.GetParameters();
                    args = new object[@params.Length];
                    for (int i = 0; i < @params.Length; i++) {
                        string key = null;
                        foreach (string k in package.Keys) {
                            if (k.Equals(@params[i].Name, StringComparison.OrdinalIgnoreCase))
                                key = k;
                        }
                        args[i] = package[key];
                        package.Remove(key);
                    }
                } else {
                    args = new object[0];
                }
                object result = FastWrapper.CreateInstance(type, args);
                foreach (string key in package.Keys) {
                    PropertyInfo property = type.GetProperty(key, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (property == null) {
                        FieldInfo field = type.GetField(key, BindingFlags.Public | BindingFlags.Instance);
                        if (field != null)
                            field.SetValue(result, package[key]);
                        continue;
                    }

                    //try {
                        if (property.CanWrite) {
                            property.SetValue(result, TypeExtensions.Convert(package[key],property.PropertyType), null);
                        }
                    //} catch(Exception e){
                    //    throw new Exception(string.Format("key={0},name={1},type:{2},{3}", key, property.Name, type.FullName,e.Message));
                    //}
                }
                return result;
            }
            //object ConvertValue(Type valueType, object value, Entry entry) {
            //    if (entry.ArrayType == PackageArrayTypes.None)
            //        return TypeExtensions.Convert(value, valueType);

            //    Type simpleType = null;

            //    if (entry.ArrayType == PackageArrayTypes.NameValueCollection) {
            //        simpleType = typeof(System.Collections.Specialized.NameValueCollection);
            //        if (valueType == simpleType)
            //            return value;
                    

            //    } else if (entry.ArrayType == PackageArrayTypes.Dictionary) {
            //        simpleType = typeof(System.Collections.Generic.Dictionary<,>);
            //    } else if (entry.ArrayType == PackageArrayTypes.Object_List) {
            //        simpleType = typeof(System.Collections.Generic.List<object>);
            //    } else if (entry.ArrayType == PackageArrayTypes.T_List) {
            //        simpleType = typeof(System.Collections.Generic.List<>);
            //    }
            //    return value;

            //}
            #endregion
        }
    }
}