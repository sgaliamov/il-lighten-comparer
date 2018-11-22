using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class StringPropertyMember : ComparablePropertyMember
    {
        public StringPropertyMember(PropertyInfo propertyInfo, MethodInfo compareToMethod) : base(propertyInfo,
            compareToMethod) { }

        public override void Accept(IMemvberVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
