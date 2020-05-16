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
                 && !objectType.ImplementsGenericInterface(typeof(IEnumerable<>)); // collections do reference comparisons anyway

            if (needReferenceComparison) {
                il.EmitReferenceComparison(LoadArgument(Arg.X), LoadArgument(Arg.Y), Return(1));
            }

            if (detecCycles) {
                EmitCycleDetection(il, objectType);
            }

            var emitter = _resolver.GetComparisonEmitter(new ArgumentVariable(objectType));

            il.DefineLabel(out var exit)
              .Execute(emitter.Emit(exit));

            if (detecCycles) {
                il.Call(RemoveMethod, LoadArgument(Arg.SetX), LoadArgument(Arg.X));
                il.Call(RemoveMethod, LoadArgument(Arg.SetY), LoadArgument(Arg.Y));
            }

            il.Execute(emitter.EmitCheckForResult(exit))
              .MarkLabel(exit)
              .Return(1);
        }

        public bool NeedCreateCycleDetectionSets(Type objectType) => true;

        private static void EmitCycleDetection(ILEmitter il, Type objectType) => il
            .AreSame(LoadInteger(0), Or(TryAdd(Arg.SetX, Arg.X, objectType.IsValueType), TryAdd(Arg.SetY, Arg.Y, objectType.IsValueType)))
            .IfFalse_S(out var next)
            .Throw(New(Methods.ArgumentExceptionConstructor, LoadString($"Can't compare objects. Cycle is detected in {objectType.DisplayName()}.")))
            .MarkLabel(next);
    }
}
