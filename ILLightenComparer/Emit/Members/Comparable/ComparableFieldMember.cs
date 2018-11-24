using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class ComparableFieldMember : FieldMember, IComparableMember
    {
        public ComparableFieldMember(FieldInfo fieldInfo) : base(fieldInfo)
        {
            ComparableType = fieldInfo.FieldType.GetUnderlyingType();
            CompareToMethod = ComparableType.GetCompareToMethod();
        }

        public Type ComparableType { get; }
        public MethodInfo CompareToMethod { get; }

        public override void Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override void Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
