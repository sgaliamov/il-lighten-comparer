using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparerStaticMethodEmitter : IComparerStaticMethodEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly ComparisonResolver _resolver;

        public ComparerStaticMethodEmitter(ComparisonResolver resolver, IConfigurationProvider configuration)
        {
            _configuration = configuration;
            _resolver = resolver;
        }

        public void Build(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            var needReferenceComparison =
                 !objectType.IsValueType
                 && !objectType.IsSealedComparable() // rely on provided implementation
                 && !objectType.ImplementsGeneric(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                EmitArgumentsReferenceComparison(il);
            }

            if (IsDetectCycles(objectType)) {
                EmitCycleDetection(il);
            }

            _resolver.GetComparison(new ArgumentVariable(objectType)).Emit(il);
        }

        public bool IsCreateCycleDetectionSets(Type objectType) =>
            _configuration.Get(objectType).DetectCycles
            && !objectType.GetUnderlyingType().IsPrimitive()
            && !objectType.IsSealedComparable(); // rely on provided implementation

        private bool IsDetectCycles(Type objectType) =>
            objectType.IsClass
            && IsCreateCycleDetectionSets(objectType)
            && !objectType.ImplementsGeneric(typeof(IEnumerable<>));

        private static void EmitCycleDetection(ILEmitter il)
        {
            il.AreSame(
                LoadInteger(0),
                Or(Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetX), LoadArgument(Arg.X), LoadInteger(0)),
                   Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetY), LoadArgument(Arg.Y), LoadInteger(0))))
            .IfFalse_S(out var next)
            .Return(Sub(
                Call(CycleDetectionSet.GetCountProperty, LoadArgument(Arg.SetX)),
                Call(CycleDetectionSet.GetCountProperty, LoadArgument(Arg.SetY))))
            .MarkLabel(next);
        }

        private static ILEmitter EmitArgumentsReferenceComparison(ILEmitter il) =>
            il.LoadArgument(Arg.X)
              .LoadArgument(Arg.Y)
              .IfNotEqual_Un_S(out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              .LoadArgument(Arg.Y)
              .IfTrue_S(out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              .LoadArgument(Arg.X)
              .IfTrue_S(out var next)
              .Return(-1)
              .MarkLabel(next);
    }
}
