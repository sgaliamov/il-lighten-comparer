using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests.SimpleTypesTests
{
    public sealed class Tests
    {
        [Fact]
        public void Compare_Sample_Types_Directly()
        {
            foreach (var item in SampleTypes.Types)
            {
                var testMethod = typeof(Tests)
                                 .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                 .Single(x => x.Name == nameof(Test) && x.IsGenericMethodDefinition)
                                 .MakeGenericMethod(item.Key);

                testMethod.Invoke(this, new object[] { item.Value });
            }
        }

        private void Test<T>(IComparer<T> referenceComparer)
        {
            T Create()
            {
                if (!typeof(T).IsValueType && _random.NextDouble() < 0.1)
                {
                    return default;
                }

                return _fixture.Create<T>();
            }

            if (referenceComparer == null) { referenceComparer = Comparer<T>.Default; }

            var comparer = new ComparersBuilder().GetComparer<T>();

            for (var i = 0; i < 10; i++)
            {
                var x = Create();
                var y = Create();

                var expected = referenceComparer.Compare(x, y).Normalize();
                var actual = comparer.Compare(x, y).Normalize();

                var message = $"{typeof(T).DisplayName()} should be supported.\nx: {x},\ny: {y}";
                actual.Should().Be(expected, message);
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
        private readonly Random _random = new Random();
    }
}
