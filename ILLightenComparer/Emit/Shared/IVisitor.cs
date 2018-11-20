using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Shared
{
    internal interface IVisitor
    {
        void Visit(ComparableProperty info, ILEmitter il);
        void Visit(ComparableField info, ILEmitter il);
        void Visit(NestedObject info, ILEmitter il);
    }
}
