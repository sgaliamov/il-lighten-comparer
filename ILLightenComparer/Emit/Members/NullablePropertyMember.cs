using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class NullablePropertyMember : PropertyMember, INullableAcceptor, ITwoArgumentsProperty
    {
        public NullablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;

            CompareToMethod = propertyType
                                  .GetCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{propertyInfo.DisplayName()} does not have {MethodName.CompareTo} method.");

            HasValueMethod = propertyType
                             .GetProperty(MethodName.HasValue)
                             ?.GetGetMethod()
                             ?? throw new ArgumentException(
                                 $"{propertyInfo.DisplayName()} does not have {MethodName.HasValue} property.");

            GetValueMethod = propertyType
                             .GetProperty(MethodName.Value)
                             ?.GetGetMethod()
                             ?? throw new ArgumentException(
                                 $"{propertyInfo.DisplayName()} does not have {MethodName.Value} property.");
        }

        public MethodInfo GetValueMethod { get; }
        public MethodInfo HasValueMethod { get; }
        public MethodInfo CompareToMethod { get; }

        public ILEmitter Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
