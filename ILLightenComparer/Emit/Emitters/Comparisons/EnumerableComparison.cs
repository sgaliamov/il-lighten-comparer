using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class EnumerableComparison : ICompareEmitterAcceptor
    {
        private EnumerableComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            GetEnumeratorMethod = variable.VariableType.GetMethod(MethodName.GetEnumerator, Type.EmptyTypes)
                                  ?? throw new ArgumentException(nameof(variable));
        }

        public MethodInfo GetEnumeratorMethod { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public IVariable Variable { get; }

        public static EnumerableComparison Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);

            return Create(variable);
        }

        public static EnumerableComparison Create(IVariable variable)
        {
            var underlyingType = variable.VariableType.GetUnderlyingType();
            if (underlyingType.ImplementsGeneric(typeof(IEnumerable<>)))
            {
                return new EnumerableComparison(variable);
            }

            return null;
        }
    }
}
