using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareCallVisitor
    {
        private readonly ComparerContext _context;

        public CompareCallVisitor(ComparerContext context)
        {
            _context = context;
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var underlyingType = member.MemberType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType, gotoNextMember);
            }

            var compareMethod = _context.GetStaticCompareMethod(underlyingType);

            return il.Emit(OpCodes.Call, compareMethod).EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il, Label gotoNextMember)
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

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new ArgumentException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Call(compareToMethod).EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            return il.Emit(OpCodes.Sub).EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;

            return il.LoadConstant(comparisonType)
                     .Call(Method.StringCompare)
                     .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(ICollectionAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            throw new NotImplementedException();
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
