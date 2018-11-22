using System.Reflection;
using ILLightenComparer.Emit.Emitters;

namespace ILLightenComparer.Emit.Members.Integral
{
    internal sealed class IntegralFiledMember : FieldMember, IIntegralMember
    {
        public IntegralFiledMember(FieldInfo fieldInfo, bool isInteger)
            : base(fieldInfo) => ConvertToLong = isInteger;

        public bool ConvertToLong { get; }

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
