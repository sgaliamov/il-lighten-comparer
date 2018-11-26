using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class StringFiledMember : FieldMember, IComparableMember
    {
        public StringFiledMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public MethodInfo CompareToMethod { get; } = Constants.StringCompareMethod;

        public override void Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override void Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
