using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class StringsComparison : IComparisonEmitter
    {
        private static readonly MethodInfo StringCompare = typeof(string).GetMethod(
           nameof(string.Compare),
           new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private readonly IConfigurationProvider _configuration;
        private readonly IVariable _variable;

        private StringsComparison(IConfigurationProvider configuration, IVariable variable)
        {
            _configuration = configuration;
            _variable = variable;
        }

        public static StringsComparison Create(IConfigurationProvider configuration, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(configuration, variable);
            }

            return null;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            _variable.Load(il, Arg.X);
            _variable.Load(il, Arg.Y);

            var stringComparisonType = _configuration.Get(_variable.OwnerType).StringComparisonType;

            return il.LoadInteger((int)stringComparisonType).Call(StringCompare);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
