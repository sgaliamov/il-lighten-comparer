using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class NullableComparison : ICompareEmitterAcceptor
    {
        private NullableComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IVariable Variable { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static NullableComparison Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);

            return Create(variable);
        }

        public static NullableComparison Create(IVariable variable)
        {
            if (variable.VariableType.IsNullable())
            {
                return new NullableComparison(variable);
            }

            return null;
        }
    }
}
