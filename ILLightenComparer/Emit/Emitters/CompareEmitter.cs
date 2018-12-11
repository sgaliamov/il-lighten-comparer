using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly ComparerContext _context;
        private readonly StackEmitter _stackEmitter = new StackEmitter();

        public CompareEmitter(ComparerContext context)
        {
            _context = context;
        }

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new ArgumentException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

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
            il.DefineLabel(out var gotoNextMember);
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;

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

                return il.Emit(OpCodes.Call, compareMethod)
                         .EmitReturnNotZero(gotoNextMember);
            }

            var contextCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, contextCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            var memberType = member.MemberType;
            var underlyingType = memberType.GetUnderlyingType();
            if (underlyingType.IsValueType || underlyingType.IsSealed)
            {
                var compareToMethod = memberType.GetUnderlyingCompareToMethod();

                return il.Emit(OpCodes.Call, compareToMethod).EmitReturnNotZero(gotoNextMember);
            }

            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(ICollectionAcceptor member, ILEmitter il)
        {
            throw new NotImplementedException();
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
