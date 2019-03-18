using System;
using System.Linq;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;

namespace ILLightenComparer.Emitters
{
    internal sealed class Converter
    {
        private static readonly Func<IVariable, IComparison>[] ComparisonConverters =
        {
            NullableComparison.Create,
            IntegralsComparison.Create,
            ComparablesComparison.Create,
            StringsComparison.Create,
            MembersComparison.Create,
            HierarchicalsComparison.Create,
            ArraysComparison.Create,
            EnumerablesComparison.Create
        };

        private readonly Context _context;

        public Converter(Context context)
        {
            _context = context;
        }

        public IComparison CreateComparison(IVariable variable)
        {
            var hasCustomComparer = _context.HasCustomComparer(variable.VariableType);
            if (hasCustomComparer)
            {
                return new CustomComparison(variable);
            }

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
