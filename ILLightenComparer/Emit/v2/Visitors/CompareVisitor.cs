﻿using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;
using StringComparison = ILLightenComparer.Emit.v2.Comparisons.StringComparison;

namespace ILLightenComparer.Emit.v2.Visitors
{
    internal sealed class CompareVisitor
    {
        private readonly ComparerContext _context;

        public CompareVisitor(ComparerContext context)
        {
            _context = context;
        }

        public ILEmitter Visit(HierarchicalComparison comparison, ILEmitter il)
        {
            var underlyingType = comparison.Variable.VariableType.GetUnderlyingType();
            if (!underlyingType.IsValueType && !underlyingType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType);
            }

            var compareMethod = _context.GetStaticCompareMethod(underlyingType);

            return il.Emit(OpCodes.Call, compareMethod);
        }

        public ILEmitter Visit(ComparableComparison comparison, ILEmitter il)
        {
            var variableType = comparison.Variable.VariableType;
            var underlyingType = variableType.GetUnderlyingType();
            if (!underlyingType.IsSealedComparable())
            {
                return EmitCallForDelayedCompareMethod(il, underlyingType);
            }

            var compareToMethod = variableType.GetUnderlyingCompareToMethod()
                                  ?? throw new InvalidOperationException(
                                      $"{variableType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return il.Emit(OpCodes.Call, compareToMethod);
        }

        public ILEmitter Visit(IntegralComparison comparison, ILEmitter il)
        {
            if (!comparison.Variable.VariableType.GetUnderlyingType().IsSmallIntegral())
            {
                throw new InvalidOperationException(
                    $"Integral type is expected but: {comparison.Variable.VariableType.DisplayName()}.");
            }

            return il.Emit(OpCodes.Sub);
        }

        public ILEmitter Visit(StringComparison comparison, ILEmitter il)
        {
            var comparisonType = (int)_context
                                      .GetConfiguration(comparison.Variable.OwnerType)
                                      .StringComparisonType;

            return il.LoadConstant(comparisonType).Call(Method.StringCompare);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type underlyingType)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(underlyingType);

            return il.Emit(OpCodes.Call, delayedCompare);
        }
    }
}