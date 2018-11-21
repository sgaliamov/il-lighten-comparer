using System.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Members
{
    internal class PropertyMember : Member
    {
        public MethodInfo GetterMethod { get; set; }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
