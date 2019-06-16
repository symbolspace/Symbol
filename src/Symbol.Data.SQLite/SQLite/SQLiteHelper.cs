/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Data;
using System.Reflection;

namespace Symbol.Data.SQLite {

    /// <summary>
    /// SQLite辅助类
    /// </summary>
    public class SQLiteHelper {

        #region fields
        private static readonly bool[] _isDebug;
        private static readonly bool[] _inited;
        private static readonly System.Reflection.Assembly[] _assemblies;
        private static readonly Symbol.Collections.Generic.NameValueCollection<FastWrapper> _types;
        private static readonly Symbol.Collections.Generic.HashSet<string> _typeNames;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置是否为调试模式（调式模式会生成dll文件。）
        /// </summary>
        public static bool IsDebug {
            get { return _isDebug[0]; }
            set { _isDebug[0] = value; }
        }
        #endregion


        #region cctor
        static SQLiteHelper() {
            _isDebug = new bool[] { false };
            _inited = new bool[] { false };
            _assemblies = new System.Reflection.Assembly[1];
            _types = new Collections.Generic.NameValueCollection<FastWrapper>();
            _typeNames = new Collections.Generic.HashSet<string>();

            LoadAssembly(typeof(System.Data.SQLite.SQLiteConnection).Assembly);
        }
        #endregion

        #region methods

        #region LoadAssembly
        /// <summary>
        /// 初始化程序集 System.Data.SQLite 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        internal static bool LoadAssembly(System.Reflection.Assembly assembly) {
            if (_assemblies[0] != null)
                return true;
            if (assembly == null || assembly.GetName().Name != "System.Data.SQLite")
                return false;
            if (_assemblies[0] != null)
                return true;
            _assemblies[0] = assembly;
            GetType("System.Data.SQLite.FunctionType");
            GetType("System.Data.SQLite.SQLiteFunctionAttribute");
            SQLiteConvert.Type = GetType("System.Data.SQLite.SQLiteConvert");
            GetType("System.Data.SQLite.SQLiteFunction");
            GetType("System.Data.SQLite.SQLiteFunctionEx");
            GetType("System.Data.SQLite.SQLiteConnectionStringBuilder");
            GetType("System.Data.SQLite.SQLiteConnection");
            GetType("System.Data.SQLite.CollationSequence");

            return true;
        }
        #endregion
        #region GetType
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="name">类型名称</param>
        /// <returns></returns>
        public static FastWrapper GetType(string name) {
            if (string.IsNullOrEmpty(name))
                return null;
            FastWrapper wrapper;
            if (_types.TryGetValue(name, out wrapper))
                return wrapper;
            if (_types.TryGetValue(name, out wrapper))
                return wrapper;
            try {
                System.Type type = _assemblies[0].GetType(name);
                if (type == null)
                    return null;
                wrapper = new FastWrapper(type, false);
                _types.Add(name, wrapper);
                return wrapper;
            } catch {
                return null;
            }
        }
        #endregion

        #region ExistsFunction
        /// <summary>
        /// 检测函数是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ExistsFunction(string name) {
            if (string.IsNullOrEmpty(name))
                return false;

            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteFunction");
            if (wrapper == null)
                return false;
            foreach (object item in (System.Collections.IEnumerable)wrapper.Get("_registeredFunctions")) {
                string name2 = (string)FastWrapper.Get(item, "Name");
                if (name == name2)
                    return true;
            }
            return false;
        }
        #endregion
        #region RegisterFunction
        /// <summary>
        /// 注册函数
        /// </summary>
        public static int RegisterFunction() {
            lock (_assemblies) {
                int count = 0;
                foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
                    count += RegisterFunction(assembly);
                }
                return count;
            }
        }
        /// <summary>
        /// 注册函数
        /// </summary>
        public static int RegisterFunction(System.Reflection.Assembly assembly) {
            if (assembly == null
#if !net20 && !net35
                || assembly.IsDynamic
#endif
                )
                return 0;
            lock (_assemblies) {
                int count = 0;
                foreach (System.Type type in assembly.GetExportedTypes()) {
                    if (RegisterFunction(type))
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// 注册函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool RegisterFunction(System.Type type) {
            if (type == null)
                return false;
            if (!type.IsClass || type.IsAbstract || !type.IsPublic)
                return false;
            if (type.Assembly.GetName().Name == "System.Data.SQLite") {
                if (!LoadAssembly(type.Assembly))
                    return false;
                GetType("System.Data.SQLite.SQLiteFunction").MethodInvoke("RegisterFunction", type);
                return true;
            }
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteFunction");
            if (!TypeExtensions.IsInheritFrom(type, typeof(SQLiteFunction)))
                return false;
            if (!_typeNames.Add(type.FullName))
                return true;
            System.Type newType = GenerateFunctionType(type);
            if (newType == null)
                return false;
            wrapper.MethodInvoke("RegisterFunction", newType);
            return true;
        }
        #region GenerateFunctionType
        static System.Type GenerateFunctionType(System.Type functionType) {
            SQLiteFunctionAttribute attribute = AttributeExtensions.GetCustomAttribute<SQLiteFunctionAttribute>(functionType);
            if (attribute == null)
                return null;
            bool ex = TypeExtensions.IsInheritFrom(functionType, typeof(SQLiteFunctionEx)) || attribute.Type == FunctionTypes.Collation;
            FastWrapper baseType = GetType(ex ? "System.Data.SQLite.SQLiteFunctionEx" : "System.Data.SQLite.SQLiteFunction");


            System.Reflection.AssemblyName assemblyName = new System.Reflection.AssemblyName(functionType.Namespace + ".DynamicClass_" + functionType.Name);
            System.Reflection.Emit.AssemblyBuilderAccess accemblyBuilderAccess =
#if netcore
                      System.Reflection.Emit.AssemblyBuilderAccess.Run;
#else
                IsDebug
                    ? System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave
                    : System.Reflection.Emit.AssemblyBuilderAccess.Run;
#endif
            System.Reflection.Emit.AssemblyBuilder assembly =
#if netcore
                System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, accemblyBuilderAccess);
#else
                System.AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, accemblyBuilderAccess);
#endif
#if !netcore
            bool canSave = (accemblyBuilderAccess == System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave || accemblyBuilderAccess == System.Reflection.Emit.AssemblyBuilderAccess.Save);
#endif
            System.Reflection.Emit.ModuleBuilder module =
#if netcore
                      assembly.DefineDynamicModule(assemblyName.Name);
#else

                canSave
                    ? assembly.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll")
                    : assembly.DefineDynamicModule(assemblyName.Name);//, n.Name + ".dll");
#endif
            System.Reflection.Emit.TypeBuilder type = module.DefineType(
                assemblyName.Name + ".DynamicClass",
                System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed | System.Reflection.TypeAttributes.AutoClass,
                baseType.Type,
                System.Type.EmptyTypes);

            {
                FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteFunctionAttribute");
                System.Reflection.PropertyInfo[] properties = new System.Reflection.PropertyInfo[] {
                    wrapper.Type.GetProperty("Name"),
                    wrapper.Type.GetProperty("Arguments"),
                    wrapper.Type.GetProperty("FuncType"),
                };
                System.Reflection.Emit.CustomAttributeBuilder attributeBuilder = new System.Reflection.Emit.CustomAttributeBuilder(wrapper.Type.GetConstructor(System.Type.EmptyTypes), new object[0],
                    properties, new object[] {
                        attribute.Name,
                        attribute.Arguments,
                        TypeExtensions.Convert(attribute.Type,GetType("System.Data.SQLite.FunctionType").Type),
                    });
                type.SetCustomAttribute(attributeBuilder);
            }
            System.Reflection.Emit.FieldBuilder _o = type.DefineField("_o", functionType, FieldAttributes.Private);

            {
                System.Reflection.Emit.ConstructorBuilder ctor = type.DefineConstructor(
                    System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.SpecialName | System.Reflection.MethodAttributes.RTSpecialName,
                    System.Reflection.CallingConventions.HasThis,
                    System.Type.EmptyTypes);
                System.Reflection.Emit.ILGenerator il = ctor.GetILGenerator();
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Call, baseType.Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, System.Type.EmptyTypes, new System.Reflection.ParameterModifier[0]));
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Newobj, functionType.GetConstructor(System.Type.EmptyTypes));
                il.Emit(System.Reflection.Emit.OpCodes.Stfld, _o);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Ldfld, _o);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                if (attribute.Type == FunctionTypes.Collation) {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
                } else {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
                }
                il.Emit(System.Reflection.Emit.OpCodes.Callvirt, functionType.GetMethod("Init", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new System.Type[] {
                    typeof(object),typeof(bool)
                }, null));
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ret);
            }
            CreateMethodDelegate createMethod = (methodInfo, action) => {
                System.Reflection.ParameterInfo[] parameters = methodInfo.GetParameters();
                System.Type[] parameterTypes = new System.Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++) {
                    parameterTypes[i] = parameters[i].ParameterType;
                }
                System.Reflection.Emit.MethodBuilder method = type.DefineMethod(methodInfo.Name, (methodInfo.Attributes | MethodAttributes.NewSlot) ^ MethodAttributes.NewSlot, methodInfo.CallingConvention, methodInfo.ReturnType, parameterTypes);
                for (int i = 0; i < parameters.Length; i++) {
                    System.Reflection.Emit.ParameterBuilder parameter = method.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
                    if (parameters[i].IsOptional) {
                        if (parameters[i].ParameterType.IsValueType && parameters[i].DefaultValue == null)
                            continue;
                        parameter.SetConstant(parameters[i].DefaultValue);
                    }
                }
                System.Reflection.Emit.ILGenerator il = method.GetILGenerator();
                bool hasReturn = (methodInfo.ReturnType != typeof(void));
                System.Reflection.Emit.LocalBuilder @return = null;
                if (hasReturn) {
                    @return = il.DeclareLocal(methodInfo.ReturnType);
                }
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Ldfld, _o);
                action(functionType.GetMethod(methodInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), method, il);
                il.Emit(System.Reflection.Emit.OpCodes.Ret);
            };
            if (attribute.Type == FunctionTypes.Scalar) {
                createMethod(baseType.Type.GetMethod("Invoke"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    System.Reflection.Emit.Label label = il.DefineLabel();
                    il.Emit(System.Reflection.Emit.OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                });
            } else if (attribute.Type == FunctionTypes.Collation) {
                createMethod(baseType.Type.GetMethod("Compare"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_2);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    System.Reflection.Emit.Label label = il.DefineLabel();
                    il.Emit(System.Reflection.Emit.OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                });
            } else {
                createMethod(baseType.Type.GetMethod("Final"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    System.Reflection.Emit.Label label = il.DefineLabel();
                    il.Emit(System.Reflection.Emit.OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                });
                createMethod(baseType.Type.GetMethod("Step"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_2);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_3);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Nop);
                });
            }
            {
                System.Reflection.MethodInfo methodInfo_base = baseType.Type.GetMethod("Dispose", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new System.Type[] { typeof(bool) }, null);
                createMethod(methodInfo_base, (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Nop);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Call, methodInfo_base);
                    il.Emit(System.Reflection.Emit.OpCodes.Nop);
                });
            }

#if netcore20
            var result = type.CreateTypeInfo();
#else
            var result = type.CreateType();
#endif
#if !netcore
            if (canSave) {
                assembly.Save(assemblyName.Name + ".dll");
            }
#endif
            return result;
        }
        delegate void CreateMethodDelegate(System.Reflection.MethodInfo methodInfo, MethodBuilderAction action);
        delegate void MethodBuilderAction(System.Reflection.MethodInfo methodInfo, System.Reflection.Emit.MethodBuilder method, System.Reflection.Emit.ILGenerator il);
        #endregion
        #endregion

        #region CreateFile
        /// <summary>
        /// 创建数据库文件。
        /// </summary>
        /// <param name="file">文件位置，如果已经存在将不会创建，请手动删除。</param>
        public static bool CreateFile(string file) {
            if (string.IsNullOrEmpty(file) || string.Equals(file, ":memory:", System.StringComparison.OrdinalIgnoreCase))
                return false;
            if (System.IO.File.Exists(file))
                return true;
            try {
                System.IO.File.WriteAllBytes(file, new byte[0]);
                return true;
            } catch {
                return false;
            }
        }
        #endregion
        #region CreateConnectionStringBuilder
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnectionStringBuilder
        /// </summary>
        /// <returns></returns>
        public static System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder() {
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteConnectionStringBuilder");
            if (wrapper == null)
                return null;
            System.Data.Common.DbConnectionStringBuilder builder = FastWrapper.CreateInstance(wrapper.Type) as System.Data.Common.DbConnectionStringBuilder;
            FastWrapper.Set(builder, "Pooling", true);
            FastWrapper.Set(builder, "FailIfMissing", false);
            builder["journal mode"] = "Off";
            return builder;
        }
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnectionStringBuilder
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString) {
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteConnectionStringBuilder");
            if (wrapper == null)
                return null;
            System.Data.Common.DbConnectionStringBuilder builder;
            if (string.IsNullOrEmpty(connectionString)) {
                builder = FastWrapper.CreateInstance(wrapper.Type) as System.Data.Common.DbConnectionStringBuilder;
            } else {
                builder = FastWrapper.CreateInstance(wrapper.Type, connectionString) as System.Data.Common.DbConnectionStringBuilder;
                string file = (string)FastWrapper.Get(builder, "DataSource");
                if (!string.IsNullOrEmpty(file) && !file.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
                    CreateFile(file);
            }
            FastWrapper.Set(builder, "Pooling", true);
            FastWrapper.Set(builder, "FailIfMissing", false);
            builder["journal mode"] = "Off";

            return builder;
        }
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnectionStringBuilder
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder(string file, string password) {
            System.Data.Common.DbConnectionStringBuilder builder = CreateConnectionStringBuilder();
            if (!string.IsNullOrEmpty(file)) {
                CreateFile(file);
                FastWrapper.Set(builder, "DataSource", file);
            }
            FastWrapper.Set(builder, "Pooling", true);
            FastWrapper.Set(builder, "FailIfMissing", false);
            builder["journal mode"] = "Off";
            if (!string.IsNullOrEmpty(password)) {
                FastWrapper.Set(builder, "Password", password);
            }
            return builder;
        }
        #endregion
        #region CreateConnection
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnection
        /// </summary>
        /// <returns></returns>
        public static IDbConnection CreateConnection() {
            return CreateConnection(null);
        }
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnection
        /// </summary>
        /// <param name="connectionString">文件或连接字符串</param>
        /// <returns></returns>
        public static IDbConnection CreateConnection(string connectionString) {
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteConnection");
            if (wrapper == null)
                return null;
            if (!_inited[0]) {
                lock (_inited) {
                    if (!_inited[0]) {
                        _inited[0] = true;
                        RegisterFunction();
                    }
                }
            }
            if (string.IsNullOrEmpty(connectionString)) {
                //connectionString = AppHelper.AppFile + ".db";
                return FastWrapper.CreateInstance(wrapper.Type) as IDbConnection;
            }
            bool byFile = (System.IO.File.Exists(connectionString) || connectionString.IndexOf('=') == -1);
            System.Data.Common.DbConnectionStringBuilder builder;
            if (byFile) {
                builder = CreateConnectionStringBuilder(connectionString, null);
            } else {
                builder = CreateConnectionStringBuilder(connectionString);
            }
            if (builder == null)
                return null;
            return FastWrapper.CreateInstance(wrapper.Type, builder.ConnectionString) as IDbConnection;
        }
        #endregion

        #endregion

    }


}