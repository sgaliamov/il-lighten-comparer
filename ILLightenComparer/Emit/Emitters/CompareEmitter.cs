using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly ArrayAcceptorVisitor _arrayAcceptorVisitor;
        private readonly CompareCallVisitor _callVisitor;
        private readonly MemberLoader _loader = new MemberLoader();
        private readonly StackEmitter _stackEmitter;

        public CompareEmitter(ComparerContext context)
        {
            _callVisitor = new CompareCallVisitor(context);
            _stackEmitter = new StackEmitter(_loader);
            _arrayAcceptorVisitor = new ArrayAcceptorVisitor(_loader, _callVisitor);
        }

        public ILEmitter Visit(IAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, il, gotoNextMember);

            return member.Accept(_callVisitor, il)
                         .EmitReturnNotZero(gotoNextMember)
                         .MarkLabel(gotoNextMember);
        }

        public ILEmitter Visit(IArrayAcceptor member, ILEmitter il)
        {
            return _arrayAcceptorVisitor.Visit(member, il);
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
