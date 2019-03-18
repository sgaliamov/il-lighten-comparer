using System;
using System.Linq;
using ILLightenComparer.Config;
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

        //private readonly IConfigurationProvider _configurations;

        //public Converter(IConfigurationProvider configurations)
        //{
        //    _configurations = configurations;
        //}

        public IComparison CreateComparison(IVariable variable)
        {
            //var configuration = _configurations.Get(variable.OwnerType);
            //var comparer = configuration.GetComparer(variable.VariableType);
            //if (comparer != null)
            //{
            //    return new CustomComparison(variable);
            //}

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
