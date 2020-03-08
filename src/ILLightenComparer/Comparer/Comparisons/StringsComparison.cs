using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Reflection;
using ILLightenComparer.Variables;
using Illuminator;

namespace ILLightenComparer.Comparer.Comparisons
{
    internal sealed class StringsComparison : IComparison
    {
        private readonly IConfigurationProvider _configurations;
        private readonly IVariable _variable;

        private StringsComparison(IConfigurationProvider configurations, IVariable variable)
        {
            _configurations = configurations;
            _variable = variable;
        }

        public static StringsComparison Create(IConfigurationProvider configurations, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(configurations, variable);
            }

            return null;
        }

        public bool PutsResultInStack => true;

        public ILEmitter Emit(ILEmitter il, Label _)
        {
            _variable.Load(il, Arg.X);
            _variable.Load(il, Arg.Y);

            var stringComparisonType = _configurations.Get(_variable.OwnerType).StringComparisonType;

            return il.LoadInteger((int)stringComparisonType).Call(Method.StringCompare);
        }

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
