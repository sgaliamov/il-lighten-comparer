using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality
{
    internal sealed class GetHashCodeStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly HasherResolver _resolver;

        public GetHashCodeStaticMethodEmitter(HasherResolver resolver) => _resolver = resolver;

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            var isCollection = objectType.ImplementsGeneric(typeof(IEnumerable<>));
            var needNullCheck = !objectType.IsValueType && !isCollection; // collections do null check anyway

            if (needNullCheck) {
                il.LoadArgument(Arg.Input)
                  .IfTrue_S(out var next)
                  .Return(0)
                  .MarkLabel(next);
            }

            if (detecCycles) {
                EmitCycleDetection(il);
            }

            _resolver.GetHasherEmitter(new ArgumentVariable(objectType)).Emit(il);

            if (detecCycles) {
                il.Call(CycleDetectionSet.RemoveMethod, LoadArgument(Arg.Input)).Pop();
            }

            il.Return();
        }

        public bool NeedCreateCycleDetectionSets(Type _) => true;

        private static void EmitCycleDetection(ILEmitter il) => il
            .IfTrue_S(
                Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.Y), LoadArgument(Arg.X), LoadInteger(0)),
                out var next)
            .Return(0) // todo: 3. return collection size
            .MarkLabel(next);
    }
}
