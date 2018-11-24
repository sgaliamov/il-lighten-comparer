using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class ComparablePropertyMember : PropertyMember, IComparableMember
    {
        public ComparablePropertyMember(PropertyInfo propertyInfo, MethodInfo compareToMethod)
            : base(propertyInfo) =>
            CompareToMethod = compareToMethod;

        public MethodInfo CompareToMethod { get; }

        public override void Accept(StackEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }

        public override void Accept(CompareEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
