using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class EnumerableComparison : ICompareEmitterAcceptor
    {
        private EnumerableComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));

            GetEnumeratorMethod = variable.VariableType.GetMethod(MethodName.GetEnumerator, Type.EmptyTypes)
                                  ?? throw new ArgumentException(nameof(variable));

            ElementType = variable.VariableType.GetGenericArguments().FirstOrDefault()
                          ?? throw new ArgumentException(nameof(variable));

            EnumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);
        }

        public Type ElementType { get; }
        public Type EnumeratorType { get; }
        public MethodInfo GetEnumeratorMethod { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public IVariable Variable { get; }

        public static ICompareEmitterAcceptor Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);

            return Create(variable);
        }

        public static ICompareEmitterAcceptor Create(IVariable variable)
        {
            if (variable.VariableType.ImplementsGeneric(typeof(IEnumerable<>)))
            {
                return new EnumerableComparison(variable);
            }

            return null;
        }
    }
}
