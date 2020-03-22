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
        private readonly MethodInfo _compareMethod;
        private readonly IVariable _variable;
        private readonly int _stringComparisonType;

        private StringsComparison(MethodInfo compareMethod, IConfigurationProvider configuration, IVariable variable)
        {
            _compareMethod = compareMethod;
            _variable = variable;
            _stringComparisonType = (int)configuration.Get(_variable.OwnerType).StringComparisonType;
        }

        public static StringsComparison Create(MethodInfo compareMethod, IConfigurationProvider configuration, IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(compareMethod, configuration, variable);
            }

            return null;
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _compareMethod,
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y),
            LoadInteger(_stringComparisonType));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
