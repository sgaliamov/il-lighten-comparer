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

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            return VisitHierarchical(il, member.VariableType);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            return VisitComparable(member.VariableType, il);
        }

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il)
        {
            return VisitBasic(member.VariableType, il);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il)
        {
            return VisitIntegral(il);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            return VisitString(member.DeclaringType, il);
        }

        public ILEmitter Visit(IArrayAcceptor member, ILEmitter il)
        {
            var variableType = member.VariableType;
            var underlyingElementType = variableType.GetElementType().GetUnderlyingType();
            if (underlyingElementType == typeof(string))
            {
                return VisitString(member.DeclaringType, il);
            }

            if (underlyingElementType.IsSmallIntegral())
            {
                return VisitIntegral(il);
            }

            if (underlyingElementType.IsPrimitive())
            {
                return VisitBasic(underlyingElementType, il);
            }

            var isComparable = underlyingElementType.ImplementsGeneric(typeof(IComparable<>), underlyingElementType);
            if (isComparable)
            {
                return VisitComparable(underlyingElementType, il);
            }

            var isEnumerable = variableType.ImplementsGeneric(
                typeof(IEnumerable<>),
                underlyingElementType);
            if (isEnumerable)
            {
                throw new NotSupportedException($"Nested collections {variableType} are not supported.");
            }

            return VisitHierarchical(il, variableType);
        }

        private ILEmitter VisitString(Type declaringType, ILEmitter il)
        {
            var comparisonType = (int)_context.GetConfiguration(declaringType).StringComparisonType;

            return il.LoadConstant(comparisonType).Call(Method.StringCompare);
        }

        private ILEmitter VisitHierarchical(ILEmitter il, Type variableType)
        {
            var underlyingType = variableType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType);
            }

            var compareMethod = _context.GetStaticCompareMethod(underlyingType);

            return il.Emit(OpCodes.Call, compareMethod);
        }

        private static ILEmitter VisitComparable(Type memberType, ILEmitter il)
        {
            var underlyingType = memberType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType);
            }

            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new InvalidOperationException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Emit(OpCodes.Call, compareToMethod);
        }

        private static ILEmitter VisitIntegral(ILEmitter il)
        {
            return il.Emit(OpCodes.Sub);
        }

        private static ILEmitter VisitBasic(Type memberType, ILEmitter il)
        {
            var compareToMethod = memberType.GetUnderlyingCompareToMethod()
                                  ?? throw new InvalidOperationException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Call(compareToMethod);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type underlyingType)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare);
        }
    }
}
