using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityStaticMethodsEmitter : IComparerStaticMethodEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly EqualityResolver _resolver;

        public EqualityStaticMethodsEmitter(EqualityResolver resolver, IConfigurationProvider configuration)
        {
            _configuration = configuration;
            _resolver = resolver;
        }

        public void Build(Type objectType, MethodBuilder staticMethodBuilder)
        {
            switch (staticMethodBuilder.Name) {
                case nameof(Equals):
                    BuildEquals(objectType, staticMethodBuilder);
                    break;

                case nameof(GetHashCode):
                    BuildGetHashCode(objectType, staticMethodBuilder);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"{nameof(IEqualityComparer)} does not have method {staticMethodBuilder.Name}.");
            }
        }

        public bool IsCreateCycleDetectionSets(Type objectType) =>
            _configuration.Get(objectType).DetectCycles
            && !objectType.GetUnderlyingType().IsPrimitive()
            && !objectType.IsSealedEquatable(); // rely on provided implementation

        private void BuildGetHashCode(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            if (IsDetectCycles(objectType)) {
                EmitCycleDetectionForHasher(il);
            }

            _resolver.GetHasher(new ArgumentVariable(objectType)).Emit(il);
        }

        private void BuildEquals(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            var needReferenceComparison =
                 !objectType.IsPrimitive()
                 && !objectType.IsSealedEquatable()
                 && !objectType.ImplementsGeneric(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                EmitArgumentsReferenceComparison(il);
            }

            if (IsDetectCycles(objectType)) {
                EmitCycleDetectionForComparison(il);
            }

            _resolver.GetComparison(new ArgumentVariable(objectType)).Emit(il);
        }

        private bool IsDetectCycles(Type objectType) =>
            objectType.IsClass
            && IsCreateCycleDetectionSets(objectType)
            && !objectType.ImplementsGeneric(typeof(IEnumerable<>));

        private static void EmitCycleDetectionForComparison(ILEmitter il)
        {
            il.AreSame(
                LoadInteger(0),
                Or(Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetX), LoadArgument(Arg.X), LoadInteger(0)),
                   Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetY), LoadArgument(Arg.Y), LoadInteger(0))))
            .IfFalse_S(out var next)
            .Return(0)
            .MarkLabel(next);
        }

        private static void EmitCycleDetectionForHasher(ILEmitter il)
        {
            il.IfTrue_S(
                Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.Y), LoadArgument(Arg.X), LoadInteger(0)),
                out var next)
              .Return(0)
              .MarkLabel(next);
        }

        private static ILEmitter EmitArgumentsReferenceComparison(ILEmitter il) =>
            il.LoadArgument(Arg.X)
              .LoadArgument(Arg.Y)
              .IfNotEqual_Un_S(out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              .LoadArgument(Arg.X)
              .IfTrue_S(out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              .LoadArgument(Arg.Y)
              .IfTrue_S(out var next)
              .Return(0)
              .MarkLabel(next);
    }
}
