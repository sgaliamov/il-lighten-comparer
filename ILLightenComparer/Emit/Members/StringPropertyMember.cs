using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class StringPropertyMember : PropertyMember
    {
        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
