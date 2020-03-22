using System;
using System.Reflection;
using ILLightenComparer.Config;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Equality.Hashers
{
    internal sealed class StringHasher : IHasherEmitter
    {
        private static readonly MethodInfo GetHashCodeMethod = typeof(string).GetMethod(nameof(string.GetHashCode), new[] { typeof(StringComparison) });
        private readonly IConfigurationProvider _configuration;
        private readonly IVariable _variable;

        private StringHasher(IConfigurationProvider configuration, IVariable variable)
        {
            _configuration = configuration;
            _variable = variable;
        }

        public static StringHasher Create(IConfigurationProvider configuration, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringHasher(configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il)
        {
            var stringComparisonType = _configuration.Get(_variable.OwnerType).StringComparisonType;

            return il.IfFalse_S(_variable.Load(Arg.X), out var zero)
                     .Call(GetHashCodeMethod, _variable.Load(Arg.X), LoadInteger((int)stringComparisonType))
                     .GoTo(out var next)
                     .MarkLabel(zero)
                     .LoadInteger(0)
                     .MarkLabel(next);
        }
    }
}
