using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Comparer
{
    // todo: 3. unify with EqualsStaticMethodEmitter
    internal sealed class CompareStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly ComparisonResolver _resolver;

        public CompareStaticMethodEmitter(ComparisonResolver resolver) => _resolver = resolver;

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            var needReferenceComparison =
                 !objectType.IsValueType
                 && !objectType.IsSealedComparable() // ComparablesComparison do this check
                 && !objectType.ImplementsGeneric(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                il.EmitReferenceComparison(LoadArgument(Arg.X), LoadArgument(Arg.Y), Return(0));
            }

            if (detecCycles) {
                EmitCycleDetection(il);
            }

            var emitter = _resolver.GetComparisonEmitter(new ArgumentVariable(objectType));

            il.DefineLabel(out var exit)
              .Execute(emitter.Emit(exit))
              .Execute(emitter.EmitCheckForResult(exit))
              .MarkLabel(exit)
              .Return(0);
        }

        // no need detect cycle as flow goes outside context
        public bool NeedCreateCycleDetectionSets(Type objectType) => !objectType.IsSealedComparable();

        private static void EmitCycleDetection(ILEmitter il) => il
            .AreSame(
                LoadInteger(0),
                Or(Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetX), LoadArgument(Arg.X), LoadInteger(0)),
                   Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetY), LoadArgument(Arg.Y), LoadInteger(0))))
            .IfFalse_S(out var next)
            .Return(Sub(
                Call(CycleDetectionSet.GetCountProperty, LoadArgument(Arg.SetX)),
                Call(CycleDetectionSet.GetCountProperty, LoadArgument(Arg.SetY))))
            .MarkLabel(next);
    }
}
