using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class ArrayAcceptorVisitor
    {
        private readonly CompareCallVisitor _callVisitor;
        private readonly MemberLoader _loader;

        public ArrayAcceptorVisitor(MemberLoader loader, CompareCallVisitor callVisitor)
        {
            _callVisitor = callVisitor;
            _loader = loader;
        }

        public ILEmitter Visit(IArrayAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);

            EmitCheckMemberReferenceComparison(il, member, gotoNextMember);

            var (countX, countY) = EmitLoadCounts(il, member);

            EmitCheckForNegativeCount(il, countX, countY, member.VariableType);

            il.LoadConstant(0)
              .Store(typeof(int), 3, out var index)
              .DefineLabel(out var loopStart);

            EmitCheckIfLoopsAreDone(il, index, countX, countY, gotoNextMember);

            EmitLoadValues(il, member, index);

            member.Accept(_callVisitor, il, gotoNextMember);

            il.LoadLocal(index)
              .LoadConstant(1)
              .Emit(OpCodes.Add)
              .Store(index)
              .Branch(OpCodes.Br_S, loopStart);

            return il;
        }

        private void EmitLoadValues(
            ILEmitter il,
            IArrayAcceptor member,
            LocalBuilder index)
        {
            member.Load(_loader, il, Arg.X)
                  .LoadLocal(index)
                  .Call(member.GetItemMethod);

            member.Load(_loader, il, Arg.Y)
                  .LoadLocal(index)
                  .Call(member.GetItemMethod);
        }

        private static void EmitCheckIfLoopsAreDone(
            ILEmitter il,
            LocalBuilder index,
            LocalBuilder countX,
            LocalBuilder countY,
            Label gotoNextMember)
        {
            il.LoadLocal(index)
              .LoadLocal(countX)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), 4, out var isDoneX)
              .LoadLocal(index)
              .LoadLocal(countY)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), 5, out var isDoneY)
              .LoadLocal(isDoneX)
              .Branch(OpCodes.Brfalse_S, out var checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Br_S, gotoNextMember)
              .MarkLabel(returnM1)
              .Return(-1)
              .MarkLabel(checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var loadValues)
              .Return(1)
              .MarkLabel(loadValues);
        }

        private (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(
            ILEmitter il,
            IArrayAcceptor member)
        {
            member.Load(_loader, il, Arg.X)
                  .Call(member.GetLengthMethod)
                  .Store(typeof(int), 0, out var countX);

            member.Load(_loader, il, Arg.Y)
                  .Call(member.GetLengthMethod)
                  .Store(typeof(int), 1, out var countY);

            return (countX, countY);
        }

        private static void EmitCheckForNegativeCount(
            ILEmitter il,
            LocalBuilder countX,
            LocalBuilder countY,
            MemberInfo memberType)
        {
            il.LoadConstant(0)
              .LoadLocal(countX)
              .Branch(OpCodes.Bgt_S, out var negativeException)
              .LoadConstant(0)
              .LoadLocal(countY)
              .Branch(OpCodes.Ble_S, out var loopInit)
              .MarkLabel(negativeException)
              .LoadString($"Collection {memberType.DisplayName()} has negative count of elements.")
              .Emit(OpCodes.Newobj, typeof(IndexOutOfRangeException).GetConstructor(new[] { typeof(string) }))
              .Emit(OpCodes.Throw)
              .MarkLabel(loopInit);
        }

        private void EmitCheckMemberReferenceComparison(ILEmitter il, IAcceptor member, Label gotoNextMember)
        {
            member.Load(_loader, il, Arg.X);
            member.Load(_loader, il, Arg.Y)
                  .Branch(OpCodes.Bne_Un_S, out var checkX)
                  .Branch(OpCodes.Br_S, gotoNextMember)
                  .MarkLabel(checkX);

            member.Load(_loader, il, Arg.X)
                  .Branch(OpCodes.Brtrue_S, out var checkY)
                  .Return(-1)
                  .MarkLabel(checkY);

            member.Load(_loader, il, Arg.Y)
                  .Branch(OpCodes.Brtrue_S, out var next)
                  .Return(1)
                  .MarkLabel(next);
        }
    }
}
