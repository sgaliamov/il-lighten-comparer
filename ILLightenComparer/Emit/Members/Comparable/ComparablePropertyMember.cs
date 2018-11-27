using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class ComparablePropertyMember : PropertyMember, IComparableMember
    {
        public ComparablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) =>
            CompareToMethod = propertyInfo
                              .PropertyType
                              .GetCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{propertyInfo.DisplayName()} does not have {MethodName.CompareTo} method.");

        public MethodInfo CompareToMethod { get; }

        public override ILEmitter Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
