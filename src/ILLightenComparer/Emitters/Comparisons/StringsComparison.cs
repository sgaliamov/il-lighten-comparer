using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class StringsComparison : IComparison
    {
        private readonly IConfigurationProvider _configurations;

        private StringsComparison(IConfigurationProvider configurations, IVariable variable)
        {
            _configurations = configurations;
            Variable = variable;
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(ILEmitter il, Label _)
        {
            Variable.Load(il, Arg.X);
            Variable.Load(il, Arg.Y);

            var stringComparisonType = _configurations.Get(Variable.OwnerType).StringComparisonType;

            return il.LoadInteger((int)stringComparisonType).Call(Method.StringCompare);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static StringsComparison Create(IConfigurationProvider configurations, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(configurations, variable);
            }

            return null;
        }
    }
}
