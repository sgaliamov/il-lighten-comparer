using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members.Base;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ComparablePropertyMember : PropertyMember
    {
        public ComparablePropertyMember(PropertyInfo propertyInfo, MethodInfo compareToMethod)
            : base(propertyInfo) =>
            CompareToMethod = compareToMethod;

        public MethodInfo CompareToMethod { get; }

        public override void Accept(IMemvberVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
