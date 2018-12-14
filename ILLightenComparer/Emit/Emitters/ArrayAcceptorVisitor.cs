﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class ArrayAcceptorVisitor
    {
        private const int LocalX = 1;
        private const int LocalY = 2;
        private const int LocalCountX = 3;
        private const int LocalCountY = 4;
        private const int LocalDoneX = 5;
        private const int LocalDoneY = 6;
        private const int LocalIndex = 7;

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

            member.Load(_loader, il, Arg.X).Store(member.VariableType, LocalX, out var x);
            member.Load(_loader, il, Arg.Y).Store(member.VariableType, LocalY, out var y);

            EmitCheckMemberReferenceComparison(il, x, y, gotoNextMember);

            var (countX, countY) = EmitLoadCounts(il, member, x, y);

            EmitCheckForNegativeCount(il, countX, countY, member.VariableType);

            il.LoadConstant(0)
              .Store(typeof(int), LocalIndex, out var index)
              .DefineLabel(out var loopStart)
              .MarkLabel(loopStart);

            EmitCheckIfLoopsAreDone(il, index, countX, countY, gotoNextMember);

            EmitLoadValues(il, member, x, y, index);

            member.Accept(_callVisitor, il)
                  .DefineLabel(out var continueLoop)
                  .EmitReturnNotZero(continueLoop);

            il.MarkLabel(continueLoop)
              .LoadLocal(index)
              .LoadConstant(LocalX)
              .Emit(OpCodes.Add)
              .Store(index)
              .Branch(OpCodes.Br_S, loopStart)
              .MarkLabel(gotoNextMember);

            return il;
        }

        private static void EmitLoadValues(
            ILEmitter il,
            IArrayAcceptor member,
            LocalBuilder x,
            LocalBuilder y,
            LocalBuilder index)
        {
            il.LoadLocal(x)
              .LoadLocal(index)
              .Call(member.GetItemMethod);

            if (member.ElementType.IsValueType && !member.ElementType.IsSmallIntegral())
            {
                il.Store(member.ElementType, LocalX, out var item)
                  .LoadAddress(item);
            }

            il.LoadLocal(y)
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
              .Store(typeof(int), LocalDoneX, out var isDoneX)
              .LoadLocal(index)
              .LoadLocal(countY)
              .Emit(OpCodes.Ceq)
              .Store(typeof(int), LocalDoneY, out var isDoneY)
              .LoadLocal(isDoneX)
              .Branch(OpCodes.Brfalse_S, out var checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var returnM1)
              .Branch(OpCodes.Br_S, gotoNextMember)
              .MarkLabel(returnM1)
              .Return(-LocalX)
              .MarkLabel(checkIsDoneY)
              .LoadLocal(isDoneY)
              .Branch(OpCodes.Brfalse_S, out var loadValues)
              .Return(LocalX)
              .MarkLabel(loadValues);
        }

        private static (LocalBuilder countX, LocalBuilder countY) EmitLoadCounts(
            ILEmitter il,
            IArrayAcceptor member,
            LocalBuilder x,
            LocalBuilder y)
        {
            il.LoadLocal(x)
              .Call(member.GetLengthMethod)
              .Store(typeof(int), LocalCountX, out var countX)
              .LoadLocal(y)
              .Call(member.GetLengthMethod)
              .Store(typeof(int), LocalCountY, out var countY);

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

        private static void EmitCheckMemberReferenceComparison(
            ILEmitter il,
            LocalBuilder x,
            LocalBuilder y,
            Label gotoNextMember)
        {
            il.LoadLocal(x)
              .LoadLocal(y)
              .Branch(OpCodes.Bne_Un_S, out var checkX)
              .Branch(OpCodes.Br, gotoNextMember)
              .MarkLabel(checkX)
              .LoadLocal(x)
              .Branch(OpCodes.Brtrue_S, out var checkY)
              .Return(-LocalX)
              .MarkLabel(checkY)
              .LoadLocal(y)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(LocalX)
              .MarkLabel(next);
        }
    }
}
