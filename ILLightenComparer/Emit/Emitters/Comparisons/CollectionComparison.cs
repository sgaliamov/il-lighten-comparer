using System.Reflection;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class CollectionComparison : ICompareEmitterAcceptor
    {
        private CollectionComparison(IVariable variable)
        {
            Variable = variable;
            GetLengthMethod = variable.VariableType.GetPropertyGetter(MethodName.ArrayLength);
        }

        public MethodInfo GetLengthMethod { get; }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public IVariable Variable { get; }

        public static CollectionComparison Create(MemberInfo memberInfo)
        {
            var variable = VariableFactory.Create(memberInfo);

            var underlyingType = variable.VariableType.GetUnderlyingType();
            if (underlyingType.IsArray && underlyingType.GetArrayRank() == 1)
            {
                return new CollectionComparison(variable);
            }

            return null;
        }
    }
}
