using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using ILLightenComparer.Tests.ComparerTests.Comparers;

namespace ILLightenComparer.Tests.Utilities
{
    public sealed class CycleDetectionSet : ConcurrentDictionary<object, byte> { }

    internal static class Helper
    {
        public static void ShouldBeSameOrder<T>(this IEnumerable<T> one, IEnumerable<T> other)
        {
            using (var enumeratorOne = one.GetEnumerator())
            using (var enumeratorOther = other.GetEnumerator()) {
                while (enumeratorOne.MoveNext() && enumeratorOther.MoveNext()) {
                    var oneCurrent = enumeratorOne.Current;
                    var otherCurrent = enumeratorOther.Current;

                    oneCurrent.ShouldBeEquals(otherCurrent);
                }

                enumeratorOne.MoveNext().Should().BeFalse();
                enumeratorOther.MoveNext().Should().BeFalse();
            }
        }

        public static void ShouldBeEquals<T>(this T x, T y)
        {
            if (typeof(T).IsPrimitive() || typeof(T).IsNullable()) {
                x.Should().BeEquivalentTo(y, options => options.WithStrictOrdering());
            } else {
                x.Should().BeEquivalentTo(y, options => options.ComparingByMembers<T>().WithStrictOrdering());
            }
        }

        public static int Normalize(this int value)
        {
            if (value <= -1) {
                return -1;
            }

            if (value >= 1) {
                return 1;
            }

            return value;
        }

        public static void Parallel(ThreadStart action) => Parallel(action, Environment.ProcessorCount * 10);

        public static void Parallel(ThreadStart action, int count)
        {
            var threads = Enumerable
                          .Range(0, count)
                          .Select(x => new Thread(action))
                          .ToArray();

            foreach (var thread in threads) {
                thread.Start();
            }

            foreach (var thread in threads) {
                thread.Join();
            }
        }

        public static IComparer CreateNullableComparer(Type type, IComparer valueComparer)
        {
            var nullableComparerType = typeof(NullableComparer<>).MakeGenericType(type);

            return (IComparer)Activator.CreateInstance(nullableComparerType, valueComparer);
        }
    }
}
