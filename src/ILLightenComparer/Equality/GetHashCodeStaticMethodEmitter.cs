using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static ILLightenComparer.Shared.CycleDetectionSet;
using static ILLightenComparer.Extensions.Functions;

namespace ILLightenComparer.Equality
{
    internal sealed class GetHashCodeStaticMethodEmitter : IStaticMethodEmitter
    {
        private readonly HasherResolver _resolver;
        private static readonly MethodInfo _getHashCodeMethod = typeof(int).GetMethod(nameof(GetHashCode));

        public GetHashCodeStaticMethodEmitter(HasherResolver resolver) => _resolver = resolver;
        static GetHashCodeStaticMethodEmitter() { }

        public void Build(Type objectType, bool detecCycles, MethodBuilder staticMethodBuilder)
        {
            using var il = staticMethodBuilder.CreateILEmitter();

            var isCollection = objectType.ImplementsGenericInterface(typeof(IEnumerable<>));
            var needNullCheck = !objectType.IsValueType && !isCollection; // collections do null check anyway

            if (needNullCheck) {
                il.LoadArgument(Arg.Input)
                  .Brtrue_S(out var next)
                  .Ret(0)
                  .MarkLabel(next);
            }

            if (detecCycles) {
                EmitCycleDetection(il, objectType);
            }

            _resolver.GetHasherEmitter(new ArgumentVariable(objectType)).Emit(il);

            if (detecCycles) {
                il.Emit(Remove(Arg.CycleSet, Arg.Input, objectType));
            }

            il.Ret();
        }

        public bool NeedCreateCycleDetectionSets(Type _) => true;

        private static void EmitCycleDetection(ILEmitter il, Type objectType) =>
            il.Brtrue_S(TryAdd(Arg.CycleSet, Arg.Input, objectType), out var next)
              .Emit(GetCount(Arg.CycleSet))
              .Stloc(typeof(int), out var count)
              .Ret(CallMethod(_getHashCodeMethod, LoadCaller(count)))
              .MarkLabel(next);
    }
}
