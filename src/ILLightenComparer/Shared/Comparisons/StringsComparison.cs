using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functions;
using ILLightenComparer.Extensions;

namespace ILLightenComparer.Shared.Comparisons
{
    internal sealed class StringsComparison : IComparisonEmitter
    {
        private readonly MethodInfo _compareMethod;
        private readonly EmitterDelegate _checkForIntermediateResultEmitter;
        private readonly IVariable _variable;
        private readonly int _stringComparisonType;

        private StringsComparison(
            MethodInfo compareMethod,
            EmitterDelegate checkForIntermediateResultEmitter,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            _compareMethod = compareMethod;
            _checkForIntermediateResultEmitter = checkForIntermediateResultEmitter;
            _variable = variable;
            _stringComparisonType = (int)configuration.Get(_variable.OwnerType).StringComparisonType;
        }

        public static StringsComparison Create(
            MethodInfo compareMethod,
            EmitterDelegate checkForIntermediateResultEmitter,
            IConfigurationProvider configuration,
            IVariable variable)
        {
            if (variable.VariableType == typeof(string)) {
                return new StringsComparison(compareMethod, checkForIntermediateResultEmitter, configuration, variable);
            }

            return null;
        }

        public ILEmitter Emit(ILEmitter il, Label _) =>
            il.CallMethod(
                _compareMethod,
                new[] { _variable.VariableType, _variable.VariableType, typeof(int) },
                _variable.Load(Arg.X),
                _variable.Load(Arg.Y),
                Ldc_I4(_stringComparisonType));

        public ILEmitter EmitCheckForResult(ILEmitter il, Label next) => _checkForIntermediateResultEmitter(il, next);
    }
}
