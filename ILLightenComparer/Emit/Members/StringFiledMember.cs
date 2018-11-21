using System.Reflection;
using ILLightenComparer.Emit.Members.Base;
using ILLightenComparer.Emit.Visitors;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class StringFiledMember : FieldMember
    {
        public StringFiledMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
