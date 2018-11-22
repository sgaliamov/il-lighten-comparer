using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members.Base;

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
