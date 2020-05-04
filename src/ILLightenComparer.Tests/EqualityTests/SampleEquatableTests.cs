using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace ILLightenComparer.Tests.EqualityTests
{
    public sealed class SampleEquatableTests
    {
        [Fact]
        public void Custom_IEquatable_implementation_is_not_used_yet()
        {
            var x = new TrueEquatable { Property = 1 };
            var y = new TrueEquatable { Property = 2 };

            var comparer = new ComparerBuilder().GetEqualityComparer<TrueEquatable>();

            comparer.Equals(x, y).Should().BeFalse();
        }

        public sealed class TrueEquatable : IEquatable<TrueEquatable>
        {
            public int Property { get; set; }

            public bool Equals([AllowNull] TrueEquatable other) => true;
        }
    }
}
