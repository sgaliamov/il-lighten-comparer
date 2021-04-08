using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static ILLightenComparer.Shared.CycleDetectionSet;
using static ILLightenComparer.Extensions.Functions;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualsStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly EqualityResolver _resolver;

        public EqualsStaticMethodEmitter(EqualityResolver resolver) => _resolver = resolver;

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            il.DefineLabel(out var exit);

            var needReferenceComparison = !objectType.ImplementsGenericInterface(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                if (!objectType.IsValueType) {
                    il.EmitReferenceComparison(Ldarg(Arg.X), Ldarg(Arg.Y), Ret(1));
                } else if (objectType.IsNullable()) {
                    il.EmitCheckNullablesForValue(Ldarga(Arg.X), Ldarga(Arg.Y), objectType, exit);
                }
            }

            if (detecCycles) {
                EmitCycleDetection(il, objectType);
            }

            var emitter = _resolver.GetComparisonEmitter(new ArgumentVariable(objectType));

            emitter.Emit(il, exit);

            if (detecCycles) {
                il.Emit(Remove(Arg.SetX, Arg.X, objectType))
                  .Emit(Remove(Arg.SetY, Arg.Y, objectType));
            }

            il.Emit(emitter.EmitCheckForResult(exit))
              .MarkLabel(exit)
              .Ret(1);
        }

        public bool NeedCreateCycleDetectionSets(Type objectType) => true;

        private static void EmitCycleDetection(ILEmitter il, Type objectType) => il
            .Ceq(Ldc_I4(0), Or(TryAdd(Arg.SetX, Arg.X, objectType), TryAdd(Arg.SetY, Arg.Y, objectType)))
            .IfFalse_S(out var next)
            .Ret(Ceq(GetCount(Arg.SetX), GetCount(Arg.SetY)))
            .MarkLabel(next);
    }
}
