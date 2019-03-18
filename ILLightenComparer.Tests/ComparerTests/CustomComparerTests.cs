using System;
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
            var x = _fixture.Create<SampleObject<SampleStruct<string>>>();
            var y = _fixture.Create<SampleObject<SampleStruct<string>>>();

            var referenceComparer = new SampleObjectComparer<SampleStruct<string>>(new SampleStructComparer<string>());
            var expected = referenceComparer.Compare(x, y);

            var builder = new ComparerBuilder();
            var comparer = builder
                           .SetComparer<SampleStructCustomComparer>()
                           .GetComparer<SampleObject<SampleStruct<string>>>();
            builder.SetComparer<SampleStruct<string>>(null);

            comparer.Compare(x, y).Should().Be(expected);
        }

        [Fact]
        public void Custom_comparer_defined_as_a_type_should_be_used()
        {
            var x = _fixture.Create<SampleObject<SampleStruct<string>>>();
            var y = _fixture.Create<SampleObject<SampleStruct<string>>>();

            var comparer = new ComparerBuilder()
                           .SetComparer<SampleStructCustomComparer>()
                           .GetComparer<SampleObject<SampleStruct<string>>>();

            comparer.Compare(x, y).Should().Be(0);
        }

        [Fact]
        public void Custom_comparer_should_be_used_for_itself()
        {
            var x = _fixture.Create<SampleStruct<string>>();
            var y = _fixture.Create<SampleStruct<string>>();

            var comparer = new ComparerBuilder()
                           .SetComparer<SampleStructCustomComparer>()
                           .GetComparer<SampleStruct<string>>();

            comparer.Compare(x, y).Should().Be(0);
        }

        [Fact]
        public void Custom_instance_comparer_for_primitive_member_should_be_used()
        {
            var x = _fixture.Create<Tuple<int, string>>();
            var y = _fixture.Create<Tuple<int, string>>();
            var expected = x.Item1.CompareTo(y.Item1);

            var comparer = new ComparerBuilder()
                           .SetComparer(new CustomisableComparer<string>((a, b) => 0))
                           .GetComparer<Tuple<int, string>>();

            comparer.Compare(x, y).Should().Be(expected);
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
