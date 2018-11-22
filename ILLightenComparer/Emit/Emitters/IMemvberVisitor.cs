using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Emitters
{
    internal interface IMemvberVisitor
    {
        void Visit(ComparableFieldMember member, ILEmitter il);
        void Visit(ComparablePropertyMember member, ILEmitter il);

        void Visit(StringFiledMember member, ILEmitter il);
        void Visit(StringPropertyMember member, ILEmitter il);
        
        void Visit(IntegralFiledMember member, ILEmitter il);
        void Visit(IntegralPropertyMember member, ILEmitter il);
    }
}
