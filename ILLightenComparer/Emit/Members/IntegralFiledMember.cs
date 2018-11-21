using System.Reflection;
using ILLightenComparer.Emit.Members.Base;
using ILLightenComparer.Emit.Visitors;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class IntegralFiledMember : FieldMember
    {
        public IntegralFiledMember(FieldInfo fieldInfo, bool isInteger) 
            : base(fieldInfo) => IsInteger = isInteger;

        public bool IsInteger { get; }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
