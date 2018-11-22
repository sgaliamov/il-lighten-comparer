using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members.Integral
{
    internal sealed class IntegralPropertyMember : PropertyMember, IIntegralMember
    {
        public IntegralPropertyMember(PropertyInfo propertyInfo, bool isInteger)
            : base(propertyInfo) => ConvertToLong = isInteger;

        public bool ConvertToLong { get; }

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
