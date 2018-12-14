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
            var underlyingElementType = member.ElementType.GetUnderlyingType();

            switch (underlyingElementType.GetComparisonType())
            {
                case ComparisonType.Strings:
                    return VisitString(member.DeclaringType, il);

                case ComparisonType.Integrals:
                    return VisitIntegral(il);

                case ComparisonType.Primitives:
                    return VisitBasic(underlyingElementType, il);

                case ComparisonType.Comparables:
                    return VisitComparable(underlyingElementType, il);

                case ComparisonType.Hierarchicals:
                    return VisitHierarchical(il, underlyingElementType);

                default:
                    throw new NotSupportedException($"{underlyingElementType.DisplayName()} is not supported.");
            }
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
