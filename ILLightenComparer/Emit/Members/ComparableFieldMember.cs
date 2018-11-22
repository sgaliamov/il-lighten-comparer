using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members.Base;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ComparableFieldMember : FieldMember
    {
        public ComparableFieldMember(FieldInfo fieldInfo, MethodInfo compareToMethod)
            : base(fieldInfo) =>
            CompareToMethod = compareToMethod;

        public MethodInfo CompareToMethod { get; }

        public override void Accept(IMemvberVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
