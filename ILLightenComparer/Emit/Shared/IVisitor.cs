using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Shared
{
    internal interface IVisitor
    {
        void Visit(ComparablePropertyMember member, ILEmitter il);
        void Visit(ComparableFieldMember member, ILEmitter il);
        void Visit(NestedObject member, ILEmitter il);
        void Visit(StringFiledMember member, ILEmitter il);
        void Visit(StringPropertyMember member, ILEmitter il);
    }
}
