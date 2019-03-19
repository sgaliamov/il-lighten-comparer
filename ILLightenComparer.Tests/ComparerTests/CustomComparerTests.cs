using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class CustomComparerTests
    {
        [Fact]
        public void After_clean_custom_comparer_for_value_type_dynamic_comparer_should_be_created()
        {
            Test(() =>
            {
                var x = _fixture.Create<SampleObject<SampleStruct<string>>>();
                var y = _fixture.Create<SampleObject<SampleStruct<string>>>();

                var referenceComparer = new SampleObjectComparer<SampleStruct<string>>(new SampleStructComparer<string>());
                var expected = referenceComparer.Compare(x, y);

                var builder = new ComparerBuilder();
                var comparer = builder
                               .SetCustomComparer<SampleStructCustomComparer>()
                               .GetComparer<SampleObject<SampleStruct<string>>>();
                builder.SetCustomComparer<SampleStruct<string>>(null);

                comparer.Compare(x, y).Should().Be(expected);
            });
        }

        [Fact]
        public void Custom_comparer_defined_as_a_type_should_be_used()
        {
            Test(() =>
            {
                var x = _fixture.Create<SampleObject<SampleStruct<string>>>();
                var y = _fixture.Create<SampleObject<SampleStruct<string>>>();

                var comparer = new ComparerBuilder()
                               .SetCustomComparer<SampleStructCustomComparer>()
                               .GetComparer<SampleObject<SampleStruct<string>>>();

                comparer.Compare(x, y).Should().Be(0);
            });
        }

        [Fact]
        public void Custom_comparer_should_be_used_for_collection_when_defined()
        {
            Test(() =>
            {
                var x = _fixture.Create<SampleObject<int[]>>();
                var y = _fixture.Create<SampleObject<int[]>>();

                var referenceComparer = new SampleObjectComparer<int[]>(new CustomisableComparer<int[]>((a, b) =>
                {
                    if (a == b)
                    {
                        return 0;
                    }

                    if (ReferenceEquals(null, b))
                    {
                        return 1;
                    }

                    if (ReferenceEquals(null, a))
                    {
                        return -1;
                    }

                    return 0;
                }));
                var expected = referenceComparer.Compare(x, y);

                var comparer = new ComparerBuilder(c => c.DefaultIgnoreCollectionOrder(true))
                               .SetCustomComparer(new CustomisableComparer<int>((a, b) => 0))
                               .GetComparer<SampleObject<int[]>>();

                var actual = comparer.Compare(x, y);

                actual.Should().Be(expected, $"\nx: {x.ToJson()}\ny: {y.ToJson()}");
            });
        }

        [Fact]
        public void Custom_comparer_should_be_used_for_itself()
        {
            Test(() =>
            {
                var x = _fixture.Create<SampleStruct<string>>();
                var y = _fixture.Create<SampleStruct<string>>();

                var comparer = new ComparerBuilder()
                               .SetCustomComparer<SampleStructCustomComparer>()
                               .GetComparer<SampleStruct<string>>();

                comparer.Compare(x, y).Should().Be(0);
            });
        }

        [Fact]
        public void Custom_instance_comparer_for_primitive_member_should_be_used()
        {
            Test(() =>
            {
                var x = _fixture.Create<Tuple<int, string>>();
                var y = _fixture.Create<Tuple<int, string>>();
                var expected = x.Item1.CompareTo(y.Item1);

                var comparer = new ComparerBuilder()
                               .SetCustomComparer(new CustomisableComparer<string>((a, b) => 0))
                               .GetComparer<Tuple<int, string>>();

                comparer.Compare(x, y).Should().Be(expected);
            });
        }

        private static void Test(Action action)
        {
            Enumerable.Range(0, Constants.SmallCount)
                      .AsParallel()
                      .ForAll(_ => action());
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class SampleStructCustomComparer : CustomisableComparer<SampleStruct<string>>
        {
            // ReSharper disable once UnusedMember.Local UnusedMember.Global
            public SampleStructCustomComparer() : base((x, y) => 0) { }
        }
    }
}
