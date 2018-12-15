using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Visitors.Comparisons;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class CompareVisitor
    {
        private readonly ComparerContext _context;

        public CompareVisitor(ComparerContext context)
        {
            _context = context;
        }

        public ILEmitter Visit(IHierarchicalComparison comparison, ILEmitter il)
        {
            return VisitHierarchical(il, comparison.Variable.VariableType);
        }

        public ILEmitter Visit(IComparableComparison comparison, ILEmitter il)
        {
            return VisitComparable(comparison.Variable.VariableType, il);
        }

        public ILEmitter Visit(IIntegralComparison comparison, ILEmitter il)
        {
            return VisitIntegral(il);
        }

        public ILEmitter Visit(IStringComparison comparison, ILEmitter il)
        {
            return VisitString(comparison.Variable.OwnerType, il);
        }

        public ILEmitter Visit(ICollectionComparison comparison, ILEmitter il)
        {
            var underlyingElementType = comparison.ElementType.GetUnderlyingType();

            switch (underlyingElementType.GetComparisonType())
            {
                case ComparisonType.Strings:
                    return VisitString(comparison.Variable.OwnerType, il);

                case ComparisonType.Integrals:
                    return VisitIntegral(il);

                case ComparisonType.Primitives:
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

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type underlyingType)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare);
        }
    }
}
