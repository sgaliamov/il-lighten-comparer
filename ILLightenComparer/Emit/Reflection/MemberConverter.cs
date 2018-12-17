using System;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Extensions;
using StringComparison = ILLightenComparer.Emit.Emitters.Comparisons.StringComparison;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        private static readonly Func<MemberInfo, ICompareEmitterAcceptor>[] Factories =
        {
            IntegralComparison.Create,
            StringComparison.Create,
            ComparableComparison.Create,
            CollectionComparison.Create,
            HierarchicalComparison.Create
        };

        public ICompareEmitterAcceptor Convert(MemberInfo memberInfo)
        {
            var comparison = Factories
                             .Select(factory => factory(memberInfo))
                             .FirstOrDefault(x => x != null);

            if (comparison != null)
            {
                return comparison;
            }

            throw new NotSupportedException($"{memberInfo.DisplayName()} is not supported.");
        }
    }
}
