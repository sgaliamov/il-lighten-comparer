using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly CollectionAcceptorVisitor _collectionAcceptorVisitor;
        private readonly ComparerContext _context;
        private readonly MemberLoader _loader = new MemberLoader();
        private readonly StackEmitter _stackEmitter;

        public CompareEmitter(ComparerContext context)
        {
            _context = context;
            _stackEmitter = new StackEmitter(_loader);
            _collectionAcceptorVisitor = new CollectionAcceptorVisitor(_loader);
        }

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return Visit(member, il, gotoNextMember);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            member.LoadMembers(_stackEmitter, gotoNextMember, il);

            return Visit(member, il, gotoNextMember);
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

        private ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var underlyingType = member.MemberType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType, gotoNextMember);
            }

            var compareMethod = _context.GetStaticCompareMethod(underlyingType);

            return il.Emit(OpCodes.Call, compareMethod).EmitReturnNotZero(gotoNextMember);
        }

        private ILEmitter Visit(IComparableAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.MemberType;
            var underlyingType = memberType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType, gotoNextMember);
            }

            var compareToMethod = memberType.GetUnderlyingCompareToMethod();

            return il.Emit(OpCodes.Call, compareToMethod).EmitReturnNotZero(gotoNextMember);
        }

        private ILEmitter Visit(
            IBasicAcceptor member,
            ILEmitter il,
            Label gotoNextMember)
        {
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new ArgumentException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Call(compareToMethod).EmitReturnNotZero(gotoNextMember);
        }

        private ILEmitter Visit(IIntegralAcceptor _, ILEmitter il, Label gotoNextMember)
        {
            return il.Emit(OpCodes.Sub).EmitReturnNotZero(gotoNextMember);
        }

        private ILEmitter Visit(IStringAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;

            return il.LoadConstant(comparisonType)
                     .Call(Method.StringCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(
            ILEmitter il,
            Type underlyingType,
            Label gotoNextMember)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }
    }
}
