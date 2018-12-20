﻿using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using StringComparison = ILLightenComparer.Emit.Emitters.Comparisons.StringComparison;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class Converter
    {
        private static readonly Func<MemberInfo, ICompareEmitterAcceptor>[] MemberConverters =
        {
            NullableComparison.Create,
            IntegralComparison.Create,
            StringComparison.Create,
            ComparableComparison.Create,
            ArrayComparison.Create,
            EnumerableComparison.Create,
            HierarchicalComparison.Create
        };

        private static readonly Func<IVariable, IComparisonAcceptor>[] VariableConverters =
        {
            IntegralComparison.Create,
            StringComparison.Create,
            ComparableComparison.Create,
            HierarchicalComparison.Create
        };

        private static readonly Func<IVariable, IComparisonAcceptor>[] ArgumentConverters =
        {
            //NullableComparison.Create,
            IntegralComparison.Create,
            StringComparison.Create,
            //ComparableComparison.Create,
            //ArrayComparison.Create,
            //EnumerableComparison.Create
        };

        public ICompareEmitterAcceptor CreateArgumentComparison(Type argumentType)
        {
            var variable = new ArgumentVariable(argumentType);

            var comparison = ArgumentConverters
                             .Select(factory => factory(variable))
                             .FirstOrDefault(x => x != null);

            if (comparison == null)
            {
                return null;
            }

            return new VariableComparison(variable, comparison);
        }

        public ICompareEmitterAcceptor CreateMemberComparison(MemberInfo memberInfo)
        {
            var comparison = MemberConverters
                             .Select(factory => factory(memberInfo))
                             .FirstOrDefault(x => x != null);
            if (comparison == null)
            {
                throw new NotSupportedException($"{memberInfo.DisplayName()} is not supported.");
            }

            return comparison;
        }

        public IComparisonAcceptor CreateArrayItemComparison(
            IVariable variable,
            LocalBuilder xArray,
            LocalBuilder yArray,
            LocalBuilder index)
        {
            var itemVariable = new ArrayItemVariable(
                variable.VariableType,
                variable.OwnerType,
                xArray,
                yArray,
                index);

            return CreateVariableComparison(itemVariable);
        }

        public IComparisonAcceptor CreateEnumerableItemComparison(
            Type ownerType,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator)
        {
            var itemVariable = new EnumerableItemVariable(ownerType, xEnumerator, yEnumerator);

            return CreateVariableComparison(itemVariable);
        }

        public IComparisonAcceptor CreateNullableVariableComparison(
            IVariable variable,
            LocalBuilder nullableX,
            LocalBuilder nullableY)
        {
            var itemVariable = new NullableVariable(variable.OwnerType, variable.VariableType, nullableX, nullableY);

            return CreateVariableComparison(itemVariable);
        }

        private static IComparisonAcceptor CreateVariableComparison(IVariable itemVariable)
        {
            var comparison = VariableConverters
                             .Select(factory => factory(itemVariable))
                             .FirstOrDefault(x => x != null);

            if (comparison == null)
            {
                throw new NotSupportedException($"{itemVariable.VariableType.DisplayName()} is not supported.");
            }

            return new VariableComparison(itemVariable, comparison);
        }
    }
}
