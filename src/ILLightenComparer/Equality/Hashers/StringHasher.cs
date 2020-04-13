using System;
using System.Reflection;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

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
            .Store(typeof(string), out var local)
            .IfFalse_S(LoadLocal(local), out var zero)
            .Call(GetHashCodeMethod, LoadLocal(local), LoadInteger(_stringComparison))
            .GoTo(out var next)
            .MarkLabel(zero)
            .LoadInteger(0)
            .MarkLabel(next);
    }
}
