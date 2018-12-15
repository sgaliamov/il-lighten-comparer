using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Emitters.Visitors.Comparisons;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class StringComparison : IStringComparison
    {
        private StringComparison(IVariable variable)
        {
            Variable = variable;
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static StringComparison Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);
            if (variable.VariableType == typeof(string))
            {
                return new StringComparison(variable);
            }

            return null;
        }
    }
}
