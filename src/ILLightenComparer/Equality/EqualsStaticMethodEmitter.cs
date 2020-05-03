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

namespace ILLightenComparer.Equality
{
    // todo: 3. unify with CompareStaticMethodEmitter
    internal sealed class EqualsStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly EqualityResolver _resolver;

        public EqualsStaticMethodEmitter(EqualityResolver resolver) => _resolver = resolver;

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            var needReferenceComparison =
                 !objectType.IsValueType
                 && !objectType.IsSealedEquatable()
                 && !objectType.ImplementsGeneric(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                il.EmitReferenceComparison(LoadArgument(Arg.X), LoadArgument(Arg.Y), Return(1));
            }

            if (detecCycles) {
                EmitCycleDetection(il);
            }

            var emitter = _resolver.GetComparisonEmitter(new ArgumentVariable(objectType));

            il.DefineLabel(out var exit)
              .Execute(emitter.Emit(exit))
              .Execute(emitter.EmitCheckForResult(exit))
              .MarkLabel(exit)
              .Return(1);
        }

        // no need detect cycle as flow goes outside context
        public bool NeedCreateCycleDetectionSets(Type objectType) => !objectType.IsSealedEquatable();

        private static void EmitCycleDetection(ILEmitter il) => il
            .AreSame(
                LoadInteger(0),
                Or(Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetX), LoadArgument(Arg.X), LoadInteger(0)),
                   Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.SetY), LoadArgument(Arg.Y), LoadInteger(0))))
            .IfFalse_S(out var next)
            .Return(0)
            .MarkLabel(next);
    }
}
