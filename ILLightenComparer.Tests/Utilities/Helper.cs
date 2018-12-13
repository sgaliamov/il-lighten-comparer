using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using FluentAssertions;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class Helper
    {
        private static int _counter;

        private static readonly ConditionalWeakTable<object, object> ObjectIds =
            new ConditionalWeakTable<object, object>();

        public static bool IsNullable(this Type type)
        {
            return type.IsValueType
                   && type.IsGenericType
                   && !type.IsGenericTypeDefinition
                   && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || ReferenceEquals(type, typeof(string))
                   || ReferenceEquals(type, typeof(decimal));
        }

        public static int CompareTo<T>(this IList<T> one, IList<T> other)
            where T : IComparable<T>
        {
            if (one == null)
            {
                if (other == null)
                {
                    return 0;
                }

                return -1;
            }

            if (other == null)
            {
                return 1;
            }

            if (one.Count < 0 || other.Count < 0)
            {
                throw new IndexOutOfRangeException($"Collection {one.GetType()} has negative count of elements.");
            }

            var i = 0;
            while (true)
            {
                var oneDone = i == one.Count;
                var otherDone = i == other.Count;
                if (oneDone)
                {
                    if (otherDone)
                    {
                        return 0;
                    }

                    return -1;
                }

                if (otherDone)
                {
                    return 1;
                }

                var compare = one[i].CompareTo(other[i]);
                if (compare != 0)
                {
                    return compare;
                }

                i++;
            }
        }

        public static int CompareTo<T>(this IEnumerable<T> one, IEnumerable<T> other)
            where T : IComparable<T>
        {
            if (one == null)
            {
                if (other == null)
                {
                    return 0;
                }

                return -1;
            }

            if (other == null)
            {
                return 1;
            }

            using (var enumeratorOne = one.GetEnumerator())
            using (var enumeratorOther = other.GetEnumerator())
            {
                while (true)
                {
                    var oneDone = !enumeratorOne.MoveNext();
                    var otherDone = !enumeratorOther.MoveNext();

                    if (oneDone)
                    {
                        if (otherDone)
                        {
                            return 0;
                        }

                        return -1;
                    }

                    if (otherDone)
                    {
                        return 1;
                    }

                    var oneCurrent = enumeratorOne.Current;
                    var otherCurrent = enumeratorOther.Current;

                    var compare = oneCurrent.CompareTo(otherCurrent);
                    if (compare != 0)
                    {
                        return compare;
                    }
                }
            }
        }

        public static void ShouldBeSameOrder<T>(this IEnumerable<T> one, IEnumerable<T> other)
        {
            using (var enumeratorOne = one.GetEnumerator())
            using (var enumeratorOther = other.GetEnumerator())
            {
                while (enumeratorOne.MoveNext() && enumeratorOther.MoveNext())
                {
                    var oneCurrent = enumeratorOne.Current;
                    var otherCurrent = enumeratorOther.Current;

                    oneCurrent
                        .Should()
                        .BeEquivalentTo(otherCurrent, options => options.ComparingByMembers<T>());
                }

                enumeratorOne.MoveNext().Should().BeFalse();
                enumeratorOther.MoveNext().Should().BeFalse();
            }
        }

        public static int Normalize(this int value)
        {
            if (value <= -1)
            {
                return -1;
            }

            if (value >= 1)
            {
                return 1;
            }

            return value;
        }

        public static int GetObjectId<T>(this T target) where T : class
        {
            return (int)ObjectIds.GetValue(target, _ => Interlocked.Increment(ref _counter));
        }

        public static void Parallel(ThreadStart action, int count)
        {
            var threads = Enumerable
                          .Range(0, count)
                          .Select(x => new Thread(action))
                          .ToArray();

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
