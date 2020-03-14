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

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityStaticMethodsEmitter : IComparerStaticMethodEmitter
    {
        private readonly IConfigurationProvider _configuration;
        private readonly EqualityResolver _resolver;

        public EqualityStaticMethodsEmitter(EqualityResolver resolver, IConfigurationProvider configuration)
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
                il.EmitArgumentsReferenceComparison();
            }

            if (IsDetectCycles(objectType)) {
                EmitCycleDetection(il);
            }

            _resolver
                .GetComparison(new ArgumentVariable(objectType))
                .Emit(il);
        }

        private bool IsCreateCycleDetectionSets(Type objectType) =>
            _configuration.Get(objectType).DetectCycles
            && !objectType.GetUnderlyingType().IsPrimitive()
            && !objectType.IsSealedComparable();

        private bool IsDetectCycles(Type objectType) =>
            objectType.IsClass
            && IsCreateCycleDetectionSets(objectType)
            && !objectType.ImplementsGeneric(typeof(IEnumerable<>));

        private static void EmitCycleDetection(ILEmitter il)
        {
            il.AreSame(
                LoadInteger(0),
                Or(LoadArgument(Arg.SetX)
                   | LoadArgument(Arg.X)
                   | LoadInteger(0)
                   | Call(CycleDetectionSet.TryAddMethod),
                   LoadArgument(Arg.SetY)
                   | LoadArgument(Arg.Y)
                   | LoadInteger(0)
                   | Call(CycleDetectionSet.TryAddMethod)))
            .Branch(OpCodes.Brfalse_S, out var next)
            .Sub(LoadArgument(Arg.SetX) | Call(CycleDetectionSet.GetCountProperty),
                 LoadArgument(Arg.SetY) | Call(CycleDetectionSet.GetCountProperty))
            .Return()
            .MarkLabel(next);
        }

    }
}
