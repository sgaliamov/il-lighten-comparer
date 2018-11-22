using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members.Base;

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
