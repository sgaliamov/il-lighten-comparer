using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class CustomEqualityComparerTests
    {
        [Fact]
        public void After_change_custom_comparer_new_dynamic_comparer_should_be_created()
        {
            Test(() => {
                var x = _fixture.Create<Tuple<int, string>>();
                var y = _fixture.Create<Tuple<int, string>>();
                var expectedEqualsInt = x.Item1.Equals(y.Item1);
                var expectedEqualsString = x.Item2?.Equals(y.Item2) ?? y.Item2 == null;
                var expectedHashInt = HashCodeCombiner.Combine(x.Item1.GetHashCode(), 0);
                var expectedHashString = HashCodeCombiner.Combine(0, x.Item2?.GetHashCode() ?? 0);

                var builder = new ComparerBuilder(c => c.SetCustomEqualityComparer(
                    new CustomizableEqualityComparer<string>((__, _) => true, _ => 0)));
                var comparerForIntOnly = builder.GetEqualityComparer<Tuple<int, string>>();
                var comparerForStringOnly = builder
                    .Configure(c => c
                    .SetCustomEqualityComparer<string>(null)
                    .SetCustomEqualityComparer(new CustomizableEqualityComparer<int>((__, _) => true, _ => 0)))
                    .GetEqualityComparer<Tuple<int, string>>();

                comparerForIntOnly.Equals(x, y).Should().Be(expectedEqualsInt, "comparison is based on int field");
                comparerForStringOnly.Equals(x, y).Should().Be(expectedEqualsString, "comparison is based on string field");
                comparerForIntOnly.GetHashCode(x).Should().Be(expectedHashInt);
                comparerForStringOnly.GetHashCode(x).Should().Be(expectedHashString);
            });
        }

        [Fact]
        public void After_clean_custom_comparer_for_value_type_dynamic_comparer_should_be_created()
        {
            var x = _fixture.Create<SampleObject<SampleStruct<string>>>();
            var y = _fixture.Create<SampleObject<SampleStruct<string>>>();

            var reference = new SampleObjectEqualityComparer<SampleStruct<string>>(new SampleStructEqualityComparer<string>());
            var expectedEquals = reference.Equals(x, y);
            var expectedHash = reference.GetHashCode(x);
            var expectedCustomHash = HashCodeCombiner.Combine(0, 0);

            var builder = new ComparerBuilder(c => c.SetCustomEqualityComparer<SampleStructCustomEqualityComparer>());
            var comparerCustom = builder.GetEqualityComparer<SampleObject<SampleStruct<string>>>();
            var comparerDefault = builder.Configure(c => c
                .SetCustomEqualityComparer<SampleStruct<string>>(null))
                .GetEqualityComparer<SampleObject<SampleStruct<string>>>();

            comparerCustom.Equals(x, y).Should().BeTrue();
            comparerCustom.GetHashCode(x).Should().Be(expectedCustomHash);
            comparerDefault.Equals(x, y).Should().Be(expectedEquals);
            comparerDefault.GetHashCode(x).Should().Be(expectedHash);
        }

        [Fact]
        public void Custom_comparer_defined_as_a_type_should_be_used()
        {
            Test(() => {
                var x = _fixture.Create<SampleObject<SampleStruct<string>>>();
                var y = _fixture.Create<SampleObject<SampleStruct<string>>>();
                var expectedCustomHash = HashCodeCombiner.Combine(0, 0);

                var comparer = new ComparerBuilder(c => c
                    .SetCustomEqualityComparer<SampleStructCustomEqualityComparer>())
                    .GetEqualityComparer<SampleObject<SampleStruct<string>>>();

                comparer.Equals(x, y).Should().BeTrue();
                comparer.GetHashCode(x).Should().Be(expectedCustomHash);
            });
        }

        [Fact]
        public void Custom_comparer_should_be_used_for_collection_when_defined()
        {
            Test(() => {
                var x = _fixture.Create<SampleObject<int[]>>();
                var y = _fixture.Create<SampleObject<int[]>>();

                var referenceComparer = new SampleObjectEqualityComparer<int[]>(
                    new CustomizableEqualityComparer<int[]>(
                        (a, b) => (a is null && b is null) || !(a is null || b is null), _ => 0));
                var expected = referenceComparer.Equals(x, y);

                var comparer = new ComparerBuilder(c => c
                    .SetDefaultCollectionsOrderIgnoring(_fixture.Create<bool>())
                    .SetCustomEqualityComparer(new CustomizableEqualityComparer<int>((__, _) => true, _ => 0)))
                    .GetEqualityComparer<SampleObject<int[]>>();

                var actualEquals = comparer.Equals(x, y);
                var actualHash = comparer.GetHashCode(x);

                actualEquals.Should().Be(expected);
                actualHash.Should().Be(0);
            });
        }

        [Fact]
        public void Custom_comparer_should_be_used_for_structs()
        {
            Test(() => {
                var x = _fixture.Create<SampleStruct<string>>();
                var y = _fixture.Create<SampleStruct<string>>();

                var comparer = new ComparerBuilder(c => c
                    .SetCustomEqualityComparer<SampleStructCustomEqualityComparer>())
                    .GetEqualityComparer<SampleStruct<string>>();

                comparer.Equals(x, y).Should().BeTrue();
                comparer.GetHashCode(x).Should().Be(0);
            });
        }

        private static void Test(Action action) => Enumerable.Range(0, 1).AsParallel().ForAll(_ => action());

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        private sealed class SampleStructCustomEqualityComparer : CustomizableEqualityComparer<SampleStruct<string>>
        {
            public SampleStructCustomEqualityComparer() : base((__, _) => true, _ => 0) { }
        }
    }
}
