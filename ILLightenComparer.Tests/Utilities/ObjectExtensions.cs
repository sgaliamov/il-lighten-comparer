using System;
using System.Collections.Concurrent;
using System.Linq;
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

        public static T GetOrAddProperty<T, TTarget>(this TTarget obj, string name, Func<T> value)
            where TTarget : class
        {
            var properties = ObjectCache.GetOrCreateValue(obj);

            return (T)properties.GetOrAdd(name, (key, x) => x, value());
        }

        public static int GetObjectId<T>(this T target) where T : class
        {
            return (int)ObjectIds.GetValue(target, _ => Interlocked.Increment(ref _counter));
        }

        public static Type GetGenericInterface(this Type type, Type generic)
        {
            if (!generic.IsGenericType)
            {
                throw new ArgumentException($"{generic.DisplayName()} should be generic type.", nameof(generic));
            }

            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == generic)
            {
                return type;
            }

            return type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && generic == t.GetGenericTypeDefinition());
        }
    }
}
