using System.Globalization;
using AutoFixture;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class DomainCustomization : ICustomization
    {
        public void Customize(IFixture fixture) {
            AddGenerators(fixture);

            fixture.Customize(new SupportMutableValueTypesCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private static void AddGenerators(IFixture fixture) {
            fixture.Customizations.Add(new StringGenerator(() =>
                (fixture.Create<ushort>() % 4)
                .ToString(CultureInfo.InvariantCulture)));

            fixture.Customizations.Add(new CasualNullGenerator(0.1));
            fixture.Customizations.Add(new CustomNumericGenerator(-1, 1, 0.1));
        }
    }
}
