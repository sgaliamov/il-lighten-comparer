using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests
{
    internal static class GenericTests
    {
        public static void Test<T>(IComparer<T> referenceComparer, int times)
        {
            var type = typeof(T);
            var random = new Random();
            var fixture = FixtureBuilder.GetInstance();

            T Create()
            {
                if ((!type.IsValueType || type.IsNullable()) && random.NextDouble() < 0.2)
                {
                    return default;
                }

                var result = fixture.Create<T>();
                if (result is IList list)
                {
                    var count = Math.Max(list.Count / 5, 1);
                    for (var i = 0; i < count; i++)
                    {
                        list[random.Next(list.Count)] = default;
                    }
                }

                return result;
            }

            if (referenceComparer == null) { referenceComparer = Comparer<T>.Default; }

            var typedComparer = new ComparersBuilder().GetComparer<T>();
            var basicComparer = new ComparersBuilder().GetComparer(type);

            for (var i = 0; i < times; i++)
            {
                var x = Create();
                var y = Create();

                var actual1 = typedComparer.Compare(x, y).Normalize();
                var actual2 = basicComparer.Compare(x, y).Normalize();
                var expected = referenceComparer.Compare(x, y).Normalize();

                var message = $"{type.DisplayName()} should be supported.\nx: {x},\ny: {y}";
                actual1.Should().Be(expected, message);
                actual2.Should().Be(expected, message);
            }
        }

        public static MethodInfo GetTestMethod()
        {
            return typeof(GenericTests)
                   .GetMethods(BindingFlags.Static | BindingFlags.Public)
                   .Single(x => x.Name == nameof(Test) && x.IsGenericMethodDefinition);
        }
    }
}
