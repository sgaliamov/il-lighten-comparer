using System;
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
            IntegralComparison.Create,
            StringComparison.Create,
            ComparableComparison.Create,
            ArrayComparison.Create,
            EnumerableComparison.Create,
            NullableComparison.Create,
            HierarchicalComparison.Create
        };

        private static readonly Func<IVariable, IComparisonAcceptor>[] VariableConverters =
        {
            IntegralComparison.Create,
            StringComparison.Create,
            ComparableComparison.Create,
            HierarchicalComparison.Create
        };

        public ICompareEmitterAcceptor Convert(MemberInfo memberInfo)
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
            var itemVariable = ArrayItemVariable.Create(
                variable.VariableType,
                variable.OwnerType,
                xArray,
                yArray,
                index);
            if (itemVariable == null)
            {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not array.");
            }

            return CreateCollectionItemComparison(itemVariable);
        }

        public IComparisonAcceptor CreateEnumerableItemComparison(
            Type ownerType,
            LocalBuilder xEnumerator,
            LocalBuilder yEnumerator)
        {
            var itemVariable = new EnumerableItemVariable(ownerType, xEnumerator, yEnumerator);

            return CreateCollectionItemComparison(itemVariable);
        }

        public IComparisonAcceptor CreateNullableVariableComparison(
            IVariable variable,
            LocalBuilder nullableX,
            LocalBuilder nullableY)
        {
            throw new NotImplementedException();
        }

        private static IComparisonAcceptor CreateCollectionItemComparison(IVariable itemVariable)
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
