using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using FluentAssertions;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class Extensions
    {
        private static int _counter;

        private static readonly ConditionalWeakTable<object, object> ObjectIds =
            new ConditionalWeakTable<object, object>();

        public static bool IsNullable(this Type type) =>
            type.IsValueType
            && type.IsGenericType
            && !type.IsGenericTypeDefinition
            && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));

        public static bool IsPrimitive(this Type type) =>
            type.IsPrimitive
            || type.IsEnum
            || ReferenceEquals(type, typeof(string))
            || ReferenceEquals(type, typeof(decimal));

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
    }
}
