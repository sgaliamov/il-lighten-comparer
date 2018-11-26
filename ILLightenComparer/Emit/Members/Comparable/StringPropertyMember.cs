using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class StringPropertyMember : PropertyMember, IComparableMember
    {
        public StringPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public MethodInfo CompareToMethod { get; } = Constants.StringCompareMethod;

        public override void Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override void Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
