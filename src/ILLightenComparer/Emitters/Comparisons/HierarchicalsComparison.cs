using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Builders;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class HierarchicalsComparison : IComparison
    {
        private readonly ComparerProvider _context;

        private HierarchicalsComparison(ComparerProvider context, IVariable variable)
        {
            _context = context;
            Variable = variable;
        }

        public IVariable Variable { get; }
        public bool PutsResultInStack => true;

        public ILEmitter Accept(ILEmitter il, Label _)
        {
            var variable = Variable;
            var variableType = variable.VariableType;

            il.LoadArgument(Arg.Context);
            variable.Load(il, Arg.X);
            variable.Load(il, Arg.Y);
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            var typeOfVariableCanBeChangedOnRuntime =
                !variableType.IsValueType && !variableType.IsSealed;

            if (typeOfVariableCanBeChangedOnRuntime) {
                return EmitCallForDelayedCompareMethod(il, variableType);
            }

            var compareMethod = _context.GetStaticCompareMethodInfo(variableType);

            return il.Call(compareMethod);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type type)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(type);

            return il.Call(delayedCompare);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static HierarchicalsComparison Create(ComparerProvider provider, IVariable variable)
        {
            if (variable.VariableType.IsHierarchical() && !(variable is ArgumentVariable)) {
                return new HierarchicalsComparison(provider, variable);
            }

            return null;
        }
    }
}
