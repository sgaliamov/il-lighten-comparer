using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly CompareCallVisitor _callVisitor;
        private readonly CollectionAcceptorVisitor _collectionAcceptorVisitor;
        private readonly MemberLoader _loader = new MemberLoader();
        private readonly StackEmitter _stackEmitter;

        public CompareEmitter(ComparerContext context)
        {
            _callVisitor = new CompareCallVisitor(context);
            _stackEmitter = new StackEmitter(_loader);
            _collectionAcceptorVisitor = new CollectionAcceptorVisitor(_loader);
        }

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return _callVisitor.Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return _callVisitor.Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return _callVisitor.Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return _callVisitor.Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return _callVisitor.Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(ICollectionAcceptor member, ILEmitter il)
        {
            return _collectionAcceptorVisitor.Visit(member, il);
        }

        public void EmitCheckArgumentsReferenceComparison(ILEmitter il)
        {
            il.LoadArgument(Arg.X) // x == y
              .LoadArgument(Arg.Y)
              .Branch(OpCodes.Bne_Un_S, out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              // y != null
              .LoadArgument(Arg.Y)
              .Branch(OpCodes.Brtrue_S, out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              // x != null
              .LoadArgument(Arg.X)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(-1)
              .MarkLabel(next);
        }
    }
}
