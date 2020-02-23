﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Builders
{
    internal sealed class ComparerTypeBuilder
    {
        private readonly CompareEmitter _compareEmitter;
        private readonly IConfigurationProvider _configuration;

        public ComparerTypeBuilder(Context context, IConfigurationProvider configuration)
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

            BuildInstanceCompareMethod(
                comparerTypeBuilder,
                staticCompareBuilder,
                contextField,
                objectType);

            return comparerTypeBuilder.CreateTypeInfo();
        }

        private void BuildStaticCompareMethod(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            if (!objectType.IsValueType
                && !objectType.IsSealedComparable()
                && !objectType.ImplementsGeneric(typeof(IEnumerable<>))) {
                il.EmitArgumentsReferenceComparison();
            }

            if (IsDetectCycles(objectType)) {
                EmitCycleDetection(il);
            }

            _compareEmitter.Emit(objectType, il);
        }

        private void BuildInstanceCompareMethod(
            TypeBuilder typeBuilder,
            MethodInfo staticCompareMethod,
            FieldInfo contextField,
            Type objectType)
        {
            var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);
            var interfaceMethod = genericInterface.GetMethod(MethodName.Compare);
            var methodBuilder = typeBuilder.DefineInterfaceMethod(interfaceMethod);

            using var il = methodBuilder.CreateILEmitter();

            il.LoadArgument(Arg.This)
              .LoadField(contextField)
              .LoadArgument(Arg.X)
              .LoadArgument(Arg.Y);

            EmitStaticCompareMethodCall(il, staticCompareMethod, objectType);
        }

        private void EmitStaticCompareMethodCall(ILEmitter il, MethodInfo staticCompareMethod, Type objectType)
        {
            if (!IsCreateCycleDetectionSets(objectType)) {
                il.LoadNull()
                  .LoadNull()
                  .Call(staticCompareMethod)
                  .Return();

                return;
            }

            il.New(Method.ConcurrentSetConstructor)
              .New(Method.ConcurrentSetConstructor)
              .Call(staticCompareMethod)
              .Return();
        }

        private bool IsCreateCycleDetectionSets(Type objectType) =>
            _configuration.Get(objectType).DetectCycles
            && !objectType.GetUnderlyingType().IsPrimitive()
            && !objectType.IsSealedComparable();

        private bool IsDetectCycles(Type objectType) =>
            objectType.IsClass
            && IsCreateCycleDetectionSets(objectType)
            && !objectType.ImplementsGeneric(typeof(IEnumerable<>));

        private static void EmitCycleDetection(ILEmitter il)
        {
            il.AreSame(
                il => il.Or(
                    il => il
                        .LoadArgument(Arg.SetX)
                        .LoadArgument(Arg.X)
                        .LoadConstant(0)
                        .Call(Method.ConcurrentSetAddMethod),
                    il => il
                        .LoadArgument(Arg.SetY)
                        .LoadArgument(Arg.Y)
                        .LoadConstant(0)
                        .Call(Method.ConcurrentSetAddMethod)),
                il => il.LoadConstant(0))
            .Branch(OpCodes.Brfalse_S, out var next) // todo: Beq?
            .Sub(il => il.LoadArgument(Arg.SetX).Call(Method.ConcurrentSetGetCountProperty),
                 il => il.LoadArgument(Arg.SetY).Call(Method.ConcurrentSetGetCountProperty))
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

            using (var il = constructorInfo.CreateILEmitter()) {
                il.LoadArgument(Arg.This)
                  .Call(typeof(object).GetConstructor(Type.EmptyTypes))
                  .SetField(il => il.LoadArgument(Arg.This),
                            il => il.LoadArgument(1),
                            contextField)
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
