using System;
using System.Reflection.Emit;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Reflection;
using Illuminator;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class CustomComparison : IComparison
    {
        public CustomComparison(IVariable variable) => Variable = variable;

        public bool PutsResultInStack => true;
        public IVariable Variable { get; }

        public ILEmitter Accept(ILEmitter il, Label _)
        {
            il.LoadArgument(Arg.Context);
            Variable.Load(il, Arg.X);
            Variable.Load(il, Arg.Y);
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            return EmitCallForDelayedCompareMethod(il, Variable.VariableType);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type type)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(type);

            return il.Call(delayedCompare);
        }
    }
}
