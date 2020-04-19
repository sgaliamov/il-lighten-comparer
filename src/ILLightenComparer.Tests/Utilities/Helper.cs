using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Execution;
using ILLightenComparer.Tests.Comparers;
using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.Utilities
{
    public sealed class CycleDetectionSet : ConcurrentDictionary<object, byte> { }

    internal static class Helper
    {
        public static void ShouldBeSameOrder<T>(this IEnumerable<T> one, IEnumerable<T> other)
        {
            using var enumeratorOne = one.GetEnumerator();
            using var enumeratorOther = other.GetEnumerator();

            while (enumeratorOne.MoveNext() && enumeratorOther.MoveNext()) {
                var oneCurrent = enumeratorOne.Current;
                var otherCurrent = enumeratorOther.Current;

                oneCurrent.ShouldBeEquals(otherCurrent);
            }

            using (new AssertionScope()) {
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

        public static void Parallel(ThreadStart action) => Parallel(action, Environment.ProcessorCount * 4);

        public static void Parallel(ThreadStart action, int count)
        {
            var threads = Enumerable
                          .Range(0, count)
                          .Select(_ => new Thread(action))
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

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection?.Any() != true;

        public static IEqualityComparer CreateNullableEqualityComparer(Type type, IEqualityComparer valueComparer)
        {
            var nullableComparerType = typeof(NullableEqualityComparer<>).MakeGenericType(type);

            return (IEqualityComparer)Activator.CreateInstance(nullableComparerType, valueComparer);
        }

        public static string ToStringEx<T>(this T value)
        {
            return value != null && value is IEnumerable enumerable
                ? string.Join(", ", enumerable.Cast<object>().ToArray())
                : value?.ToString() ?? "null";
        }

        public static object[] UnfoldArrays(this object[] objects) => objects.SelectMany(ObjectToArray).ToArray();

        public static object[] ObjectToArray(this object item) => item switch
        {
            string str => new[] { str },
            object[] array => UnfoldArrays(array),
            IEnumerable<object> enumerable => UnfoldArrays(enumerable.ToArray()),
            IEnumerable enumerable => UnfoldArrays(enumerable.Cast<object>().ToArray()),
            _ => new[] { item },
        };

        public static IEnumerable<T> RandomNulls<T>(this IEnumerable<T> collection, double probability = .2)
        {
            foreach (var item in collection) {
                if (ThreadSafeRandom.NextDouble() < probability) {
                    yield return default;
                }

                yield return item;
            }
        }

        //public static unsafe long GetAddress<T>(this T value)
        //{
        //    if (value is null) {
        //        return 0;
        //    }

        //    var reference = __makeref(value);

        //    return (long)*(IntPtr*)(&reference);
        //}
    }
}
