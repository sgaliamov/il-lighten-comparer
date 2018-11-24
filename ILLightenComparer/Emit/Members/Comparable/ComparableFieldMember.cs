using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members.Comparable
{
    internal sealed class ComparableFieldMember : FieldMember, IComparableMember
    {
        public ComparableFieldMember(FieldInfo fieldInfo, MethodInfo compareToMethod)
            : base(fieldInfo) =>
            CompareToMethod = compareToMethod;

        public MethodInfo CompareToMethod { get; }

        public override void Accept(StackEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }

        public override void Accept(CompareEmitter visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
