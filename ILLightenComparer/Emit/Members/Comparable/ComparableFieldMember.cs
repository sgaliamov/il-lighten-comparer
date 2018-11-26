using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class ComparableFieldMember : FieldMember, IComparableMember
    {
        public ComparableFieldMember(FieldInfo fieldInfo) : base(fieldInfo) =>
            CompareToMethod = fieldInfo
                              .FieldType
                              .GetUnderlyingType()
                              .GetCompareToMethod()
                              ?? throw new ArgumentException(
                                  $"{fieldInfo.DisplayName()} does not have {MethodName.CompareTo} method.");

        public MethodInfo CompareToMethod { get; }

        public override void Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override void Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
