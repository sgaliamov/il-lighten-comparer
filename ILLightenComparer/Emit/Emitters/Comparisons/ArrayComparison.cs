using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class ArrayComparison : ICompareEmitterAcceptor
    {
        private ArrayComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            GetLengthMethod = variable.VariableType.GetPropertyGetter(MethodName.Length);
        }

        public MethodInfo GetLengthMethod { get; }

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
            var variableType = variable.VariableType;
            if (variableType.IsArray && variableType.GetArrayRank() == 1)
            {
                return new ArrayComparison(variable);
            }

            return null;
        }
    }
}
