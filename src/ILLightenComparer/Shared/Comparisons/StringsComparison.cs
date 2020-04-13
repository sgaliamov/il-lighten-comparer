using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Abstractions;
using ILLightenComparer.Config;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

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

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _compareMethod,
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y),
            LoadInteger(_stringComparisonType));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();

        public ILEmitter EmitCheckForIntermediateResult(ILEmitter il, Label next) => _checkForIntermediateResultEmitter(il, next);
    }
}
