using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class StringPropertyMember : ComparablePropertyMember
    {
        public StringPropertyMember(PropertyInfo propertyInfo, MethodInfo compareToMethod)
            : base(propertyInfo, compareToMethod) { }

        public override void Accept(StackEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
