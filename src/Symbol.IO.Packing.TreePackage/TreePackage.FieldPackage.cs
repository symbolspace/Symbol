/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Reflection;

namespace Symbol.IO.Packing {
    partial class TreePackage {

        class FieldPackage : ICustomPackage {
            public static readonly ICustomPackage Instance = new FieldPackage();


            #region ICustomPackage 成员

            public byte[] Save(object instance) {
                Type type = instance.GetType();
                TreePackage package = new TreePackage();
                package.Attributes.Add("Type", type.AssemblyQualifiedName);
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
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
                    CommonException.ThrowTypeMismatch("未能找到类型“" + (string)package.Attributes["Type"] + "”");
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
                            if (k.Equals(@params[i].Name, StringComparison.OrdinalIgnoreCase) || k.EndsWith(@params[i].Name, StringComparison.OrdinalIgnoreCase))
                                key = k;
                        }
                        if (string.IsNullOrEmpty(key))
                            return null;
                        args[i] = package[key];
                        package.Remove(key);
                    }
                } else {
                    args = new object[0];
                }
                object result = FastWrapper.CreateInstance(type, args);
                foreach (string key in package.Keys) {
                    FieldInfo field = type.GetField(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                        field.SetValue(result, TypeExtensions.Convert(package[key], field.FieldType));
                }
                return result;
            }
            #endregion
        }
    }
}