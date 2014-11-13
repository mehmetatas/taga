using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using Taga.Core.DynamicProxy;

namespace Taga.Core.ORM
{
    public class PocoMapper
    {
        private static readonly Hashtable Mappers = new Hashtable();

        public static IPocoMapper For<T>() where T : class, new()
        {
            return For(typeof(T));
        }

        public static IPocoMapper For(Type entityType)
        {
            if (Mappers.ContainsKey(entityType))
            {
                return Mappers[entityType] as IPocoMapper;
            }

            var mapper = CreateMapper(entityType);
            Mappers.Add(entityType, mapper);
            return mapper;
        }

        private static IPocoMapper CreateMapper(Type entityType)
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Taga.Core.Dynamix.Mappers"),
                AssemblyBuilderAccess.Run);

            var assemblyName = assemblyBuilder.GetName().Name;

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

            var typeName = entityType.Name + "Mapper";

            var interfaceType = typeof(IPocoMapper);

            var typeBuilder = moduleBuilder.DefineType(
                "Taga.Core.Dynamix.Mappers." + typeName,
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { interfaceType });

            var baseCtor = typeof(object).GetConstructor(Type.EmptyTypes);

            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                Type.EmptyTypes);

            var ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, baseCtor);
            ctorIL.Emit(OpCodes.Ret);

            var methodInfo = interfaceType.GetMethod("Map");

            // public override? ReturnType Method(arguments...)
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                methodInfo.ReturnType,
                methodInfo.GetParameterTypes());

            var methodIL = methodBuilder.GetILGenerator();

            var entityCtor = entityType.GetConstructor(Type.EmptyTypes);

            // var user = new User();
            var entity = methodIL.DeclareLocal(entityType);
            methodIL.Emit(OpCodes.Newobj, entityCtor);
            methodIL.Emit(OpCodes.Stloc, entity);

            var getValueMethod = typeof(IDataRecord).GetMethod("GetValue");

            // var dbnull = DbNull.Value;
            var dbnull = methodIL.DeclareLocal(typeof(DBNull));
            var dbnullValue = typeof(DBNull).GetField("Value", BindingFlags.Static | BindingFlags.Public);
            methodIL.Emit(OpCodes.Ldsfld, dbnullValue);
            methodIL.Emit(OpCodes.Stloc, dbnull);

            //var console = new Console(methodIL);
            var tmp = methodIL.DeclareLocal(typeof(object));

            var i = 0;
            foreach (var property in entityType.GetProperties())
            {
                var setter = property.GetSetMethod(true);

                var ifnull = methodIL.DefineLabel();
                var endif = methodIL.DefineLabel();

                // object tmp = dr.GetValue(i);
                methodIL.Emit(OpCodes.Ldarg_1);
                methodIL.Emit(OpCodes.Ldc_I4, i++);
                methodIL.Emit(OpCodes.Callvirt, getValueMethod);
                methodIL.Emit(OpCodes.Stloc, tmp);

                // if (tmp == dbnull)
                //      goto ifnull;
                methodIL.Emit(OpCodes.Ldloc, tmp);
                methodIL.Emit(OpCodes.Ldloc, dbnull);
                methodIL.Emit(OpCodes.Beq, ifnull);

                // user.Property = tmp;
                methodIL.Emit(OpCodes.Ldloc, entity);
                methodIL.Emit(OpCodes.Ldloc, tmp);

                //console.WriteLine(property.Name);
                //var tostring = typeof(object).GetMethod("ToString");
                //methodIL.Emit(OpCodes.Callvirt, tostring);
                //methodIL.Emit(OpCodes.Call, typeof(System.Console).GetMethod("WriteLine",
                //    new[] { typeof(string) }));
                //methodIL.Emit(OpCodes.Ldloc, tmp);

                if (property.PropertyType.IsValueType)
                {
                    if (property.PropertyType.IsGenericType &&
                        property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                    {
                        methodIL.Emit(OpCodes.Unbox_Any, property.PropertyType.GetGenericArguments()[0]);
                        var nullableCtor =
                            property.PropertyType.GetConstructor(new[] {property.PropertyType.GetGenericArguments()[0]});

                        methodIL.Emit(OpCodes.Newobj, nullableCtor);
                    }
                    else
                    {
                        methodIL.Emit(OpCodes.Unbox_Any, property.PropertyType);
                    }
                }
                methodIL.Emit(OpCodes.Callvirt, setter);

                // goto endif;
                methodIL.Emit(OpCodes.Br, endif);

                // ifnull:
                methodIL.MarkLabel(ifnull);
                // endIf:
                methodIL.MarkLabel(endif);
            }

            methodIL.Emit(OpCodes.Ldloc, entity);
            methodIL.Emit(OpCodes.Ret);

            var mapperType = typeBuilder.CreateType();

            return (IPocoMapper)Activator.CreateInstance(mapperType);
        }

        //private class Console
        //{
        //    private static readonly MethodInfo Cw = typeof (System.Console).GetMethod("Write",
        //        new[] {typeof (string)});

        //    private readonly ILGenerator _il;

        //    public Console(ILGenerator il)
        //    {
        //        _il = il;
        //    }

        //    public void WriteLine(string s)
        //    {
        //        _il.Emit(OpCodes.Ldstr, s + ": ");
        //        _il.Emit(OpCodes.Call, Cw);
        //    }
        //}
    }
}
