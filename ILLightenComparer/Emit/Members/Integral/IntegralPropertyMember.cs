using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Behavioural;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members.Integral
{
    internal sealed class IntegralPropertyMember : PropertyMember, IPropertyValues, IIntegralMember
    {
        public IntegralPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public override ILEmitter Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
