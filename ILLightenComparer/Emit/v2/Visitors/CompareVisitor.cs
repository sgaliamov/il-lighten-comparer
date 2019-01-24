using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;
using ILLightenComparer.Emit.v2.Visitors.Collection;

namespace ILLightenComparer.Emit.v2.Visitors
{
    internal sealed class CompareVisitor
    {
        private readonly ComparerContext _context;
        private readonly ArrayVisitor _arrayVisitor;
        private readonly EnumerableVisitor _enumerableVisitor;

        public CompareVisitor(ComparerContext context)
        {
            _context = context;
            _arrayVisitor = new ArrayVisitor(context, _variablesVisitor, _compareVisitor, _loader, converter);
            _enumerableVisitor = new EnumerableVisitor(context, _variablesVisitor, _compareVisitor, _loader, converter);
        }

        public ILEmitter Visit(HierarchicalsComparison comparison, ILEmitter il)
        {
            var underlyingType = comparison.VariableType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType);
            }

            var compareMethod = _context.GetStaticCompareMethod(underlyingType);

            return il.Emit(OpCodes.Call, compareMethod);
        }

        public ILEmitter Visit(ComparablesComparison comparison, ILEmitter il)
        {
            var variableType = comparison.VariableType;
            var underlyingType = variableType.GetUnderlyingType();
            if (!underlyingType.IsSealedComparable()) // todo: if object implements IComparable, then it should be used anyway?
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType);
            }

            var compareToMethod = variableType.GetUnderlyingCompareToMethod()
                                  ?? throw new InvalidOperationException(
                                      $"{variableType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Emit(OpCodes.Call, compareToMethod);
        }

        public ILEmitter Visit(IntegralsComparison comparison, ILEmitter il)
        {
            if (!comparison.VariableType.GetUnderlyingType().IsIntegral())
            {
                throw new InvalidOperationException(
                    $"Integral type is expected but: {comparison.VariableType.DisplayName()}.");
            }

            return il.Emit(OpCodes.Sub);
        }

        public ILEmitter Visit(StringsComparison comparison, ILEmitter il)
        {
            return il.LoadConstant(comparison.StringComparisonType).Call(Method.StringCompare);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type underlyingType)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare);
        }
    }
}
