using System;
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
            // todo: 1. null check
            if (detecCycles) {
                EmitCycleDetection(il);
            }

            _resolver
                .GetHasherEmitter(new ArgumentVariable(objectType))
                .Emit(il)
                .Return();
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
