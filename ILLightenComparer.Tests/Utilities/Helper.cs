using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static ConditionalWeakTable<object,
            ConcurrentDictionary<string, object>> ObjectCache = new ConditionalWeakTable<object,
            ConcurrentDictionary<string, object>>();

        public static T GetOrAddValue<T>(this object obj, string name, Func<T> value)
        {
            var properties = ObjectCache.GetOrCreateValue(obj);

            return (T)properties.GetOrAdd(name, (key, x) => x, value());
        }

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

        public static string DisplayName(this MemberInfo memberInfo)
        {
            return $"{memberInfo}"
                   .Replace("\\, ILLightenComparer.Tests\\, Version=1.0.0.0\\, Culture=neutral\\, PublicKeyToken=null", "");
        }
    }
}
