using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class Helper
    {
        public static void ShouldBeSameOrder<T>(this IEnumerable<T> one, IEnumerable<T> other)
        {
            using (var enumeratorOne = one.GetEnumerator())
            using (var enumeratorOther = other.GetEnumerator())
            {
                while (enumeratorOne.MoveNext() && enumeratorOther.MoveNext())
                {
                    var oneCurrent = enumeratorOne.Current;
                    var otherCurrent = enumeratorOther.Current;

                    if (typeof(T).IsPrimitive())
                    {
                        oneCurrent
                            .Should()
                            .BeEquivalentTo(otherCurrent, options => options);
                    }
                    else
                    {
                        oneCurrent
                            .Should()
                            .BeEquivalentTo(otherCurrent, options => options.ComparingByMembers<T>());
                    }
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
