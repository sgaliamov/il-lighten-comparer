using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members.Base;

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
