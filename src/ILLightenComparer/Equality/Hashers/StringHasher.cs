using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.FunctionalExtensions;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class StringHasher : IHasherEmitter
    {
        private static readonly MethodInfo GetHashCodeMethod = typeof(string).GetMethod(
            nameof(string.GetHashCode),
            new[] { typeof(StringComparison) });

        private readonly IVariable _variable;
        private readonly int _stringComparison;

        private StringHasher(StringComparison stringComparison, IVariable variable)
        {
            _variable = variable;
            _stringComparison = (int)stringComparison;
        }

        public static StringHasher Create(IConfigurationProvider configuration, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                var stringComparisonType = configuration.Get(variable.OwnerType).StringComparisonType;
                return new StringHasher(stringComparisonType, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il) => _variable
            .Load(il, Arg.Input)
            .Stloc(typeof(string), out var local)
            .Brfalse_S(LoadLocal(local), out var zero)
            .Call(GetHashCodeMethod, LoadLocal(local), Ldc_I4(_stringComparison))
            .Br_S(out var next)
            .MarkLabel(zero)
            .LoadInteger(0)
            .MarkLabel(next);

        public ILEmitter Emit(ILEmitter il, LocalBuilder _) => Emit(il);
    }
}
