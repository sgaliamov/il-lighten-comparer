using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality
{
    internal sealed class GetHashCodeStaticMethodEmitter : IComparerStaticMethodEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly EqualityResolver _resolver;

        public GetHashCodeStaticMethodEmitter(EqualityResolver resolver, IConfigurationProvider configuration)
        {
            _configuration = configuration;
            _resolver = resolver;
        }

        public void Build(Type objectType, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            if (IsDetectCycles(objectType)) {
                EmitCycleDetection(il);
            }

            _resolver.GetHasher(new ArgumentVariable(objectType)).Emit(il);
        }

        public bool IsCreateCycleDetectionSets(Type objectType) =>
            _configuration.Get(objectType).DetectCycles
            && !objectType.IsPrimitive();

        private bool IsDetectCycles(Type objectType) =>
            objectType.IsClass
            && IsCreateCycleDetectionSets(objectType)
            && !objectType.ImplementsGeneric(typeof(IEnumerable<>));

        private static void EmitCycleDetection(ILEmitter il)
        {
            il.IfTrue_S(
                Call(CycleDetectionSet.TryAddMethod, LoadArgument(Arg.Y), LoadArgument(Arg.X), LoadInteger(0)),
                out var next)
              .Return(0)
              .MarkLabel(next);
        }
    }
}
