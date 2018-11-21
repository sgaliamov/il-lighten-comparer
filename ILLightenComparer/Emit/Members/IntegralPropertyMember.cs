using System.Reflection;
using ILLightenComparer.Emit.Members.Base;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class IntegralPropertyMember : PropertyMember
    {
        public IntegralPropertyMember(PropertyInfo propertyInfo, bool isInteger)
            : base(propertyInfo) => IsInteger = isInteger;

        public bool IsInteger { get; }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
