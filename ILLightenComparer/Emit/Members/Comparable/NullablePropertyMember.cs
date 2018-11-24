using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class NullablePropertyMember : PropertyMember, IComparableMember
    {
        public NullablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;

            ComparableType = propertyType.GetUnderlyingType();

            CompareToMethod = ComparableType.GetCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{propertyInfo.DisplayName()} does not have {MethodName.CompareTo} method.");

            HasValueMethod = propertyType.GetProperty(MethodName.HasValue)?.GetGetMethod()
                             ?? throw new ArgumentException(
                                 $"{propertyInfo.DisplayName()} does not have {MethodName.HasValue} property.");

            GetValueMethod = propertyType.GetProperty(MethodName.Value)?.GetGetMethod()
                             ?? throw new ArgumentException(
                                 $"{propertyInfo.DisplayName()} does not have {MethodName.Value} property.");
        }

        public Type ComparableType { get; }
        public MethodInfo GetValueMethod { get; }
        public MethodInfo HasValueMethod { get; }
        public MethodInfo CompareToMethod { get; }

        public override void Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override void Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
