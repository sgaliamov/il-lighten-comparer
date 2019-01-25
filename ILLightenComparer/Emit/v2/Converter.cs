using System;
using System.Linq;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.v2.Comparisons;
using ILLightenComparer.Emit.v2.Sources;
using ILLightenComparer.Emit.v2.Variables;

namespace ILLightenComparer.Emit.v2
{
    internal sealed class Converter
    {
        private static readonly Func<IVariable, IComparison>[] ComparisonConverters =
        {
            IntegralsComparison.Create,
            StringsComparison.Create,
            NullableComparison.Create,
            ArrayComparison.Create,
            EnumerableComparison.Create,
            ComparablesComparison.Create,
            HierarchicalsComparison.Create
        };

        public IComparison CreateComparison(IVariable variable)
        {
            var comparison = ComparisonConverters
                             .Select(factory => factory(variable))
                             .FirstOrDefault(x => x != null);

            if (comparison == null)
            {
                throw new NotSupportedException($"{variable.VariableType.DisplayName()} is not supported.");
            }

            return comparison;
        }
    }
}
