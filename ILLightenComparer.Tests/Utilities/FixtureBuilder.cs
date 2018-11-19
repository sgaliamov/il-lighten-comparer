using System;
using System.Threading;
using AutoFixture;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class FixtureBuilder
    {
        private static readonly Lazy<Fixture> Fixture = new Lazy<Fixture>(
            () =>
            {
                var f = new Fixture { RepeatCount = 2 };

                f.Customize(new DomainCustomization());
                f.Behaviors.Add(new OmitOnRecursionBehavior());

                return f;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static Fixture GetInstance() => Fixture.Value;
    }
}
