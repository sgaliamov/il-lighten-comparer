using System;
using System.Collections.Generic;
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
            return VisitHierarchical(il, gotoNextMember, member.VariableType);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            return VisitComparable(member.VariableType, il, gotoNextMember);
        }

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            return VisitBasic(member.VariableType, il, gotoNextMember);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            return VisitIntegral(il, gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            return VisitString(member.DeclaringType, il, gotoNextMember);
        }

        public ILEmitter Visit(IArrayAcceptor member, ILEmitter il, Label gotoNextMember)
        {
            var variableType = member.VariableType;
            var underlyingType = variableType.GetElementType().GetUnderlyingType();
            if (underlyingType == typeof(string))
            {
                return VisitString(member.DeclaringType, il, gotoNextMember);
            }

            if (underlyingType.IsSmallIntegral())
            {
                return VisitIntegral(il, gotoNextMember);
            }

            if (underlyingType.IsPrimitive())
            {
                return VisitBasic(variableType, il, gotoNextMember);
            }

            var isComparable = underlyingType.ImplementsGeneric(typeof(IComparable<>), underlyingType);
            if (isComparable)
            {
                return VisitComparable(variableType, il, gotoNextMember);
            }

            var isEnumerable = variableType.ImplementsGeneric(typeof(IEnumerable<>), underlyingType);
            if (isEnumerable)
            {
                throw new NotSupportedException($"Nested collections {variableType} are not supported.");
            }

            return VisitHierarchical(il, gotoNextMember, variableType);
        }

        private ILEmitter VisitHierarchical(ILEmitter il, Label gotoNextMember, Type variableType)
        {
            var underlyingType = variableType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType, gotoNextMember);
            }

            var compareMethod = _context.GetStaticCompareMethod(underlyingType);

            return il.Emit(OpCodes.Call, compareMethod);
        }

        private static ILEmitter VisitComparable(Type memberType, ILEmitter il, Label gotoNextMember)
        {
            var underlyingType = memberType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType, gotoNextMember);
            }

            var compareToMethod = memberType.GetUnderlyingCompareToMethod();

            return il.Emit(OpCodes.Call, compareToMethod);
        }

        private static ILEmitter VisitIntegral(ILEmitter il, Label gotoNextMember)
        {
            return il.Emit(OpCodes.Sub);
        }

        private ILEmitter VisitString(Type declaringType, ILEmitter il, Label gotoNextMember)
        {
            var comparisonType = (int)_context.GetConfiguration(declaringType).StringComparisonType;

            return il.LoadConstant(comparisonType)
                     .Call(Method.StringCompare)
                ;
        }

        private static ILEmitter VisitBasic(Type memberType, ILEmitter il, Label gotoNextMember)
        {
            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new ArgumentException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Call(compareToMethod);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(
            ILEmitter il,
            Type underlyingType,
            Label gotoNextMember)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare);
        }
    }
}
