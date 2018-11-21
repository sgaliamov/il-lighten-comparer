using System.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class FieldMember : Member
    {
        public FieldInfo FieldInfo { get; set; }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
