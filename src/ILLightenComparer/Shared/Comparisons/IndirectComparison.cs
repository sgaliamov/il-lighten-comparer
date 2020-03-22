using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Extensions;
using ILLightenComparer.Variables;
using Illuminator;
using static Illuminator.Functional;

namespace ILLightenComparer.Shared.Comparisons
{
    /// <summary>
    ///     Delegates comparison to static method or delayed compare method in context.
    /// </summary>
    internal sealed class IndirectComparison : IComparisonEmitter
    {
        private readonly MethodInfo _method;
        private readonly IVariable _variable;

        private IndirectComparison(IVariable variable, MethodInfo method)
        {
            _variable = variable;
            _method = method;
        }

        public static IndirectComparison Create(
            Func<Type, MethodInfo> staticMethodFactory,
            MethodInfo genericDelayedMethod,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            if (!variableType.IsHierarchical() || (variable is ArgumentVariable)) {
                return null;
            }

            var typeOfVariableCanBeChangedOnRuntime = !variableType.IsSealedType();
            var compareMethod = typeOfVariableCanBeChangedOnRuntime
                ? genericDelayedMethod.MakeGenericMethod(variableType)
                : staticMethodFactory(variableType);

            return new IndirectComparison(variable, compareMethod);
        }

        public static IndirectComparison Create(
            MethodInfo genericDelayedMethod,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            var compareMethod = genericDelayedMethod.MakeGenericMethod(variableType);

            return new IndirectComparison(variable, compareMethod);
        }

        public bool PutsResultInStack { get; } = true;

        public ILEmitter Emit(ILEmitter il, Label _) => il.Call(
            _method,
            LoadArgument(Arg.Context),
            _variable.Load(Arg.X),
            _variable.Load(Arg.Y),
            LoadArgument(Arg.SetX),
            LoadArgument(Arg.SetY));

        public ILEmitter Emit(ILEmitter il) => Emit(il, default).Return();
    }
}
