using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly Context _context;
        private readonly StackEmitter _stackEmitter = new StackEmitter();

        public CompareEmitter(Context context) => _context = context;

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new ArgumentException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            il.DefineLabel(out var gotoNextMember);
            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .Call(compareToMethod)
                         .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);

            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .Emit(OpCodes.Sub)
                         .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;
            il.DefineLabel(out var gotoNextMember);

            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .LoadConstant(comparisonType)
                         .Call(Method.StringCompare)
                         .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il)
                  .LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);

            var underlyingType = member.MemberType.GetUnderlyingType();
            if (underlyingType.IsValueType || underlyingType.IsSealed)
            {
                var compareMethod = _context.GetStaticCompareMethod(underlyingType);
                if (compareMethod != null)
                {
                    return il.Emit(OpCodes.Call, compareMethod)
                             .EmitReturnNotZero(gotoNextMember);
                }
            }

            var contextCompare = Method.ContextCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, contextCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;
            var underlyingType = memberType.GetUnderlyingType();
            var compareToMethod = memberType.GetUnderlyingCompareToMethod();

            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il)
                  .Store(memberType, 1, out var y) // todo: not optimal local variables
                  .Store(memberType, 0, out var x);

            if (underlyingType.IsValueType || underlyingType.IsSealed)
            {
                if (memberType.IsValueType)
                {
                    il.LoadAddress(x)
                      .LoadLocal(y);
                }
                else
                {
                    il.LoadLocal(x)
                      .Branch(OpCodes.Brtrue_S, out var call)
                      .LoadLocal(y)
                      .Emit(OpCodes.Brfalse_S, gotoNextMember)
                      .Return(-1)
                      .MarkLabel(call)
                      .LoadLocal(x)
                      .LoadLocal(y);
                }

                return il.Emit(OpCodes.Call, compareToMethod).EmitReturnNotZero(gotoNextMember);
            }

            il.LoadArgument(Arg.Context)
              .LoadLocal(x)
              .LoadLocal(y)
              .LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            var contextCompare = Method.ContextCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, contextCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }

        public void EmitReferenceComparison(ILEmitter il)
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
