using System.Globalization;
using AutoFixture;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class DomainCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            AddGenerators(fixture);

            fixture.Customize(new SupportMutableValueTypesCustomization());
        }

        private static void AddGenerators(IFixture fixture)
        {
            fixture.Customizations.Add(new StringGenerator(() =>
                (fixture.Create<ushort>() % 6)
                .ToString(CultureInfo.InvariantCulture)));

            fixture.Customizations.Add(new RandomNumericSequenceGenerator(-5, 5));
        }
    }
}
