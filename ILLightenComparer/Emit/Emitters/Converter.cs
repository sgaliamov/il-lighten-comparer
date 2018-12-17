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
            CollectionComparison.Create,
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

        public IComparisonAcceptor CreateArrayItemComparison(IVariable variable, LocalBuilder index)
        {
            var itemVariable = ArrayItemVariable.Create(variable.VariableType, variable.OwnerType, index);
            if (itemVariable == null)
            {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not array.");
            }

            var comparison = VariableConverters
                             .Select(factory => factory(itemVariable))
                             .FirstOrDefault(x => x != null);

            if (comparison == null)
            {
                throw new NotSupportedException($"{itemVariable.VariableType.DisplayName()} is not supported.");
            }

            return new ArrayItemComparison(itemVariable, comparison);
        }
    }
}
