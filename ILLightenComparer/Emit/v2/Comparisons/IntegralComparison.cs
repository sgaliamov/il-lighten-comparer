using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Variables.Members;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class IntegralComparison : IStaticComparison
    {
        private IntegralComparison(IVariable variable)
        { }

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
            var variable = MemberVariableFactory.Create(memberInfo);

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
