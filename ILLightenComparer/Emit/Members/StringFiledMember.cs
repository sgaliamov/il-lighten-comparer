using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class StringFiledMember : ComparableFieldMember
    {
        public StringFiledMember(FieldInfo fieldInfo, MethodInfo compareToMethod) : base(fieldInfo, compareToMethod) { }

        public override void Accept(IMemvberVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
