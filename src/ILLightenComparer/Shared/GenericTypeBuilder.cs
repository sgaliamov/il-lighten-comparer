using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Reflection;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared
{
    internal sealed class GenericTypeBuilder
    {
        private readonly IComparerStaticMethodEmitter _staticMethodEmitter;

        public GenericTypeBuilder(IComparerStaticMethodEmitter staticMethodBuilder) => _staticMethodEmitter = staticMethodBuilder;

        public Type Build(StaticMethodsInfo methodsInfo)
        {
            var comparerTypeBuilder = methodsInfo.ComparerTypeBuilder;
            var contextField = comparerTypeBuilder.DefineField(
                "_context",
                typeof(IContext),
                FieldAttributes.InitOnly | FieldAttributes.Private);

            BuildConstructorAndFactoryMethod(comparerTypeBuilder, contextField);

            var names = methodsInfo
                .GetAllMethodBuilders()
                .Select(staticMethodBuilder => {
                    _staticMethodEmitter.Build(methodsInfo.ObjectType, staticMethodBuilder);

                    BuildInstanceMethod(
                        methodsInfo.TypedComparerInterface,
                        comparerTypeBuilder,
                        staticMethodBuilder,
                        contextField,
                        methodsInfo.ObjectType);

                    return staticMethodBuilder.Name;
                })
                .ToArray();

            var compiledComparerType = comparerTypeBuilder.CreateTypeInfo();

            foreach (var name in names) {
                var method = compiledComparerType.GetMethod(name, BindingFlags.Public | BindingFlags.Static);
                methodsInfo.SetCompiledMethod(method);
            }

            return compiledComparerType;
        }

        private void BuildInstanceMethod(
            Type typedComparerInterface,
            TypeBuilder typeBuilder,
            MethodBuilder staticMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var interfaceMethod = typedComparerInterface.GetMethod(staticMethod.Name);
            var parametersCount = interfaceMethod.GetParameters().Length;
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using var il = methodBuilder.CreateILEmitter();

            Enumerable.Range(0, parametersCount)
                .Aggregate(il
                .LoadArgument(Arg.This)
                .LoadField(contextField), (il, index) => il.LoadArgument((ushort)(index + 1)));

            EmitStaticMethodCall(il, objectType, staticMethod, parametersCount);
        }

        private void EmitStaticMethodCall(ILEmitter il, Type objectType, MethodBuilder staticMethod, int parametersCount)
        {
            // todo: 1. need other implementation for hasher.
            if (_staticMethodEmitter.IsCreateCycleDetectionSets(objectType)) {
                Enumerable.Range(0, parametersCount)
                    .Aggregate(il, (il, _) => il.New(CycleDetectionSet.DefaultConstructor))
                    .Call(staticMethod)
                    .Return();

                return;
            }

            Enumerable.Range(0, parametersCount)
               .Aggregate(il, (il, _) => il.LoadNull())
               .Call(staticMethod)
               .Return();
        }

        private static void BuildConstructorAndFactoryMethod(TypeBuilder typeBuilder, FieldInfo contextField)
        {
            var parameters = new[] { typeof(IContext) };

            var constructorInfo = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                parameters);

            using (var il = constructorInfo.CreateILEmitter()) {
                il.LoadArgument(Arg.This)
                  .Call(typeof(object).GetConstructor(Type.EmptyTypes))
                  .SetField(LoadArgument(Arg.This), LoadArgument(1), contextField)
                  .Return();
            }

            var methodBuilder = typeBuilder.DefineStaticMethod(
                MethodName.CreateInstance,
                typeBuilder,
                parameters);

            using (var il = methodBuilder.CreateILEmitter()) {
                il.LoadArgument(Arg.This)
                  .New(constructorInfo)
                  .Return();
            }
        }
    }
}
