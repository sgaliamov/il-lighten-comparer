using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters.Builders
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly CompareEmitter _compareEmitter;
        private readonly IConfigurationProvider _configuration;

        public ComparerTypeBuilder(IContext context, IConfigurationProvider configuration)
        {
            _configuration = configuration;
            _compareEmitter = new CompareEmitter(context, configuration);
        }

        public Type Build(TypeBuilder comparerTypeBuilder, MethodBuilder staticCompareBuilder, Type objectType)
        {
            var contextField = comparerTypeBuilder.DefineField(
                "_context",
                typeof(IContext),
                FieldAttributes.InitOnly | FieldAttributes.Private);

            BuildConstructorAndFactoryMethod(comparerTypeBuilder, contextField);

            BuildStaticCompareMethod(objectType, staticCompareBuilder);

            BuildBasicCompareMethod(
                comparerTypeBuilder,
                staticCompareBuilder,
                contextField,
                objectType);

            BuildTypedCompareMethod(
                comparerTypeBuilder,
                staticCompareBuilder,
                contextField,
                objectType);

            return comparerTypeBuilder.CreateTypeInfo();
        }

        private void BuildStaticCompareMethod(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using (var il = staticMethodBuilder.CreateILEmitter())
            {
                if (!objectType.IsValueType
                    && !objectType.IsSealedComparable()
                    && !objectType.ImplementsGeneric(typeof(IEnumerable<>)))
                {
                    il.EmitArgumentsReferenceComparison();
                }

                if (IsDetectCycles(objectType))
                {
                    EmitCycleDetection(il);
                }

                _compareEmitter.Emit(objectType, il);
            }
        }

        private void BuildBasicCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var interfaceMethod = typeof(IComparer).GetMethod(MethodName.Compare);
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                if (objectType.IsValueType)
                {
                    il.EmitArgumentsReferenceComparison();
                }

                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(Arg.X)
                  .EmitCast(objectType)
                  .LoadArgument(Arg.Y)
                  .EmitCast(objectType);

                EmitStaticCompareMethodCall(il, staticCompareMethod, objectType);
            }
        }

        private void BuildTypedCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);
            var interfaceMethod = genericInterface.GetMethod(MethodName.Compare);
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Ldfld, contextField)
                  .LoadArgument(Arg.X)
                  .LoadArgument(Arg.Y);

                EmitStaticCompareMethodCall(il, staticCompareMethod, objectType);
            }
        }

        private void EmitStaticCompareMethodCall(ILEmitter il, MethodInfo staticCompareMethod, Type objectType)
        {
            if (!IsCreateCycleDetectionSets(objectType))
            {
                il.Emit(OpCodes.Ldnull)
                  .Emit(OpCodes.Ldnull)
                  .Call(staticCompareMethod)
                  .Return();

                return;
            }

            il.Emit(OpCodes.Newobj, Method.SetConstructor)
              .Emit(OpCodes.Newobj, Method.SetConstructor)
              .Call(staticCompareMethod)
              .Return();
        }

        private bool IsCreateCycleDetectionSets(Type objectType)
        {
            return _configuration.Get(objectType).DetectCycles
                   && !objectType.GetUnderlyingType().IsPrimitive()
                   && !objectType.IsSealedComparable();
        }

        private bool IsDetectCycles(Type objectType)
        {
            return objectType.IsClass
                   && IsCreateCycleDetectionSets(objectType)
                   && !objectType.ImplementsGeneric(typeof(IEnumerable<>));
        }

        private static void EmitCycleDetection(ILEmitter il)
        {
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.X)
              .LoadConstant(0)
              .Emit(OpCodes.Call, Method.SetAdd)
              .LoadArgument(Arg.SetY)
              .LoadArgument(Arg.Y)
              .LoadConstant(0)
              .Emit(OpCodes.Call, Method.SetAdd)
              .Emit(OpCodes.Or)
              .LoadConstant(0)
              .Emit(OpCodes.Ceq)
              .Branch(OpCodes.Brfalse_S, out var next)
              .LoadArgument(Arg.SetX)
              .Emit(OpCodes.Call, Method.SetGetCount)
              .LoadArgument(Arg.SetY)
              .Emit(OpCodes.Call, Method.SetGetCount)
              .Emit(OpCodes.Sub)
              .Return()
              .MarkLabel(next);
        }

        private static void BuildConstructorAndFactoryMethod(TypeBuilder typeBuilder, FieldInfo contextField)
        {
            var parameters = new[] { typeof(IContext) };

            var constructorInfo = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                parameters);

            using (var il = constructorInfo.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes))
                  .LoadArgument(Arg.This)
                  .LoadArgument(1)
                  .Emit(OpCodes.Stfld, contextField)
                  .Return();
            }

            var methodBuilder = typeBuilder.DefineStaticMethod(
                MethodName.CreateInstance,
                typeBuilder,
                parameters);

            using (var il = methodBuilder.CreateILEmitter())
            {
                il.LoadArgument(Arg.This)
                  .Emit(OpCodes.Newobj, constructorInfo)
                  .Return();
            }
        }
    }
}
