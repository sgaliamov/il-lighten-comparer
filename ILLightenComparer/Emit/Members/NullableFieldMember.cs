using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class NullableFieldMember : FieldMember, INullableAcceptor, ITwoArgumentsField
    {
        public NullableFieldMember(FieldInfo fieldInfo) : base(fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;

            CompareToMethod = fieldType
                                  .GetCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{fieldInfo.DisplayName()} does not have {MethodName.CompareTo} method.");

            HasValueMethod = fieldType
                             .GetProperty(MethodName.HasValue)
                             ?.GetGetMethod()
                             ?? throw new ArgumentException(
                                 $"{fieldInfo.DisplayName()} does not have {MethodName.HasValue} field.");

            GetValueMethod = fieldType
                             .GetProperty(MethodName.Value)
                             ?.GetGetMethod()
                             ?? throw new ArgumentException(
                                 $"{fieldInfo.DisplayName()} does not have {MethodName.Value} field.");
        }

        public MethodInfo GetValueMethod { get; }
        public MethodInfo HasValueMethod { get; }
        public MethodInfo CompareToMethod { get; }

        public ILEmitter Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
