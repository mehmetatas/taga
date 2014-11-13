using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Core.Dynamix
{
    public static class PocoBuilder
    {
        private static readonly ModuleBuilder ModuleBuilder;

        static PocoBuilder()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Taga.Core.Dynamix.Pocos"),
                AssemblyBuilderAccess.Run);

            var assemblyName = assemblyBuilder.GetName().Name;

            ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        public static Type BuildPoco(IDictionary<string, Type> properties)
        {
            var typeName = "_" + Guid.NewGuid().ToString().Replace("-", "_");

            var typeBuilder = ModuleBuilder.DefineType(
                "Taga.Core.Dynamix.Pocos." + typeName,
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object));

            var baseCtor = typeof(object).GetConstructor(Type.EmptyTypes);

            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                Type.EmptyTypes);

            var ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, baseCtor);
            ctorIL.Emit(OpCodes.Ret);

            ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                properties.Values.ToArray());

            ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, baseCtor);

            var argIndex = 1;

            foreach (var kv in properties)
            {
                var propName = kv.Key;
                var propType = kv.Value;

                var backingField = typeBuilder.DefineField("_" + propName, propType, FieldAttributes.Private);

                var property = typeBuilder.DefineProperty(propName, PropertyAttributes.HasDefault, propType, null);
                
                //Getter
                var getterMethod = typeBuilder.DefineMethod(
                    "get_" + propName, 
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, 
                    propType, 
                    Type.EmptyTypes);

                var getterIL = getterMethod.GetILGenerator();

                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, backingField);
                getterIL.Emit(OpCodes.Ret);

                //Setter
                var setterMethod = typeBuilder.DefineMethod(
                    "set_" + propName, 
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, 
                    null, 
                    new [] { propType });

                var setterIL = setterMethod.GetILGenerator();

                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, backingField);
                setterIL.Emit(OpCodes.Ret);

                property.SetGetMethod(getterMethod);
                property.SetSetMethod(setterMethod);

                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Ldarg_S, argIndex);
                ctorIL.Emit(OpCodes.Call, setterMethod);

                argIndex++;
            }

            ctorIL.Emit(OpCodes.Ret);

            return typeBuilder.CreateType();
        }
    }
}
