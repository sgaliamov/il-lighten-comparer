using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Shared;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class StringsComparison : IComparisonEmitter
    {
        private static readonly MethodInfo StringCompare = typeof(string).GetMethod(
           nameof(string.Compare),
           new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private readonly IVariable _variable;
        private readonly int _stringComparisonType;

        private StringsComparison(IConfigurationProvider configuration, IVariable variable)
        {
            _variable = variable;
            _stringComparisonType = (int)configuration.Get(_variable.OwnerType).StringComparisonType;
        }

        public static StringsComparison Create(IConfigurationProvider configuration, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(configuration, variable);
            }

            return null;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            StringCompare,
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y),
            LoadInteger(_stringComparisonType));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
