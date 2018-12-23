using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class ObjectExtensions
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

        public static int GetObjectId<T>(this T target) where T : class
        {
            return (int)ObjectIds.GetValue(target, _ => Interlocked.Increment(ref _counter));
        }
    }
}
