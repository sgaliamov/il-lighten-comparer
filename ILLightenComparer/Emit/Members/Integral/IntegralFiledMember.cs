using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members.Integral
{
    internal sealed class IntegralFiledMember : FieldMember, IFieldValues, IIntegralMember
    {
        public IntegralFiledMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public override ILEmitter Accept(StackEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
        public override ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);
    }
}
