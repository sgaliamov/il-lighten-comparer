using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Shared
{
    internal interface IVisitor
    {
        void Visit(PropertyMember member, ILEmitter il);
        void Visit(FieldMember member, ILEmitter il);
        void Visit(NestedObject member, ILEmitter il);
        void Visit(StringFiledMember member, ILEmitter il);
        void Visit(StringPropertyMember member, ILEmitter il);
    }
}
