using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class StringFiledMember : FieldMember, IComparableMember
    {
        public StringFiledMember(FieldInfo fieldInfo)
            : base(fieldInfo) { }

        public override void Accept(StackEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }

        public override void Accept(CompareEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }

        public MethodInfo CompareToMethod { get; } = Constants.StringCompareMethod;
    }
}
