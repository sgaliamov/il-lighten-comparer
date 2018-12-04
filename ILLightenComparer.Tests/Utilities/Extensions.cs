using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class Extensions
    {
        private static readonly ConditionalWeakTable<object, object> VisitedObjects =
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
                        .BeEquivalentTo(otherCurrent);
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

        public static bool NotVisited<T>(this T obj) where T : class
        {
            return VisitedObjects.GetValue(obj, _ => null) == null;
        }

        public static void SetProcessing<T>(this T obj, bool value) where T : class
        {
            VisitedObjects.AddOrUpdate(obj, value ? new object() : null);
        }
    }
}
