using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static ILLightenComparer.Shared.CycleDetectionSet;
using static Illuminator.Functional;

namespace ILLightenComparer.Comparer
{
    internal sealed class CompareStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly ComparisonResolver _resolver;

        public CompareStaticMethodEmitter(ComparisonResolver resolver) => _resolver = resolver;

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            il.DefineLabel(out var exit);

            var needReferenceComparison =
                !objectType.IsSealedComparable() // ComparablesComparison do this check
                && !objectType.ImplementsGenericInterface(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                if (!objectType.IsValueType) {
                    il.EmitReferenceComparison(LoadArgument(Arg.X), LoadArgument(Arg.Y), Return(0));
                } else if (objectType.IsNullable()) {
                    il.EmitCheckNullablesForValue(LoadArgumentAddress(Arg.X), LoadArgumentAddress(Arg.Y), objectType, exit);
                }
            }

            if (detecCycles) {
                EmitCycleDetection(il, objectType);
            }

            var emitter = _resolver.GetComparisonEmitter(new ArgumentVariable(objectType));

            emitter.Emit(il, exit);

            if (detecCycles) {
                il.Execute(Remove(Arg.SetX, Arg.X, objectType))
                  .Execute(Remove(Arg.SetY, Arg.Y, objectType));
            }

            il.Execute(emitter.EmitCheckForResult(exit))
              .MarkLabel(exit)
              .Return(0);
        }

        // no need detect cycle as flow goes outside context
        public bool NeedCreateCycleDetectionSets(Type objectType) => !objectType.IsSealedComparable();

        private static void EmitCycleDetection(ILEmitter il, Type objectType) => il
            .AreSame(LoadInteger(0), Or(TryAdd(Arg.SetX, Arg.X, objectType), TryAdd(Arg.SetY, Arg.Y, objectType)))
            .IfFalse_S(out var next)
            .Return(Sub(GetCount(Arg.SetX), GetCount(Arg.SetY)))
            .MarkLabel(next);
    }
}
