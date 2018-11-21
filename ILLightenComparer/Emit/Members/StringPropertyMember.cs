using System.Reflection;
using ILLightenComparer.Emit.Members.Base;
using ILLightenComparer.Emit.Visitors;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class StringPropertyMember : PropertyMember
    {
        public StringPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
