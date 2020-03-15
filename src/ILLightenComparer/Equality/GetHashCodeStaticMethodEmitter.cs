using System;
using System.Reflection.Emit;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality
{
    internal sealed class GetHashCodeStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly EqualityResolver _resolver;

        public GetHashCodeStaticMethodEmitter(EqualityResolver resolver) => _resolver = resolver;

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            if (detecCycles) {
                EmitCycleDetection(il);
            }

            _resolver.GetHasher(new ArgumentVariable(objectType)).Emit(il);
        }

        public bool NeedCreateCycleDetectionSets(Type objectType) => true;

        private static void EmitCycleDetection(ILEmitter il) => il
            .IfTrue_S(
                Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.Y), LoadArgument(Arg.X), LoadInteger(0)),
                out var next)
            .Return(0)
            .MarkLabel(next);
    }
}
