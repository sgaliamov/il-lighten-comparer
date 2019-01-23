using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class IntegralComparison : IStaticComparison
    {
        private IntegralComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.LoadVariables(this, il, gotoNext);
        }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static IntegralComparison Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);

            return Create(variable);
        }

        public static IntegralComparison Create(IVariable variable)
        {
            if (variable.VariableType.GetUnderlyingType().IsSmallIntegral())
            {
                return new IntegralComparison(variable);
            }

            return null;
        }
    }
}
