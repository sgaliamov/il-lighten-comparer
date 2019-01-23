using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.v2.Comparisons;
using ILLightenComparer.Emit.v2.Variables;
using StringComparison = ILLightenComparer.Emit.v2.Comparisons.StringComparison;

namespace ILLightenComparer.Emit.v2
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

        private static readonly Func<IVariable, IVisitorsAcceptor>[] VariableConverters =
        {
            IntegralComparison.Create,
            StringComparison.Create,
            ComparableComparison.Create,
            HierarchicalComparison.Create
        };

        public ICompareEmitterAcceptor CreateArgumentComparison(Type argumentType)
        {
            var variable = new ArgumentVariable(argumentType);

            if (argumentType.IsNullable())
            {
                return NullableComparison.Create(variable);
            }

            var comparison = IntegralComparison.Create(variable) ?? (IVisitorsAcceptor)StringComparison.Create(variable);
            if (comparison != null)
            {
                return new VariableComparison(variable, comparison);
            }

            if (argumentType.GetUnderlyingType().IsSealedComparable())
            {
                return ComparableComparison.Create(variable);
            }

            return ArrayComparison.Create(variable) ?? EnumerableComparison.Create(variable);
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

        public IVisitorsAcceptor CreateArrayItemVariableComparison(
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

        public IVisitorsAcceptor CreateEnumerableItemVariableComparison(
            Type ownerType,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator)
        {
            var itemVariable = new EnumerableItemVariable(ownerType, xEnumerator, yEnumerator);

            return CreateVariableComparison(itemVariable);
        }

        public IVisitorsAcceptor CreateNullableVariableComparison(
            IVariable variable,
            LocalBuilder nullableX,
            LocalBuilder nullableY)
        {
            var itemVariable = new NullableVariable(variable.OwnerType, variable.VariableType, nullableX, nullableY);

            return CreateVariableComparison(itemVariable);
        }

        private static IVisitorsAcceptor CreateVariableComparison(IVariable itemVariable)
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
