using AutoFixture;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class DomainCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new RandomNumericSequenceGenerator(-10, 10));
        }
    }
}
