using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

namespace ILLightenComparer.Tests.Utilities
{
    internal static class ObjectExtensions
    {
        private static int _counter;

        private static readonly ConditionalWeakTable<object, object> ObjectIds = new ConditionalWeakTable<object, object>();

        public static ConditionalWeakTable<object, ConcurrentDictionary<string, object>> ObjectCache =
            new ConditionalWeakTable<object, ConcurrentDictionary<string, object>>();

        public static T GetOrAddProperty<T, TTarget>(this TTarget obj, string name, Func<T> value)
            where TTarget : class
        {
            var properties = ObjectCache.GetOrCreateValue(obj);

            return (T)properties.GetOrAdd(name, (_, x) => x, value());
        }

        public static int GetObjectId<T>(this T target) where T : class => (int)ObjectIds.GetValue(target, _ => Interlocked.Increment(ref _counter));

        public static string ToJson<T>(this T target) => JsonConvert.SerializeObject(target);

        public static Type GetGenericInterface(this Type type, Type generic)
        {
            if (!generic.IsGenericType) {
                throw new ArgumentException($"{generic.DisplayName()} should be generic type.", nameof(generic));
            }

            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == generic) {
                return type;
            }

            return Array.Find(type.GetInterfaces(), t => t.IsGenericType && generic == t.GetGenericTypeDefinition());
        }

        public static MethodInfo GetGenericMethod(this IReflect fromType, string name, BindingFlags bindingFlags)
        {
            return fromType
                .GetMethods(bindingFlags)
                .Single(x => x.Name == name && x.IsGenericMethodDefinition);
        }

        public static TItem[] UnfoldArrays<TItem>(this TItem[] objects) => objects.SelectMany(x => ObjectToArray(x)).Cast<TItem>().ToArray();

        public static object[] ObjectToArray(this object item) => item switch
        {
            string str => new[] { str },
            object[] array => UnfoldArrays(array),
            IEnumerable<object> enumerable => UnfoldArrays(enumerable.ToArray()),
            IEnumerable enumerable => UnfoldArrays(enumerable.Cast<object>().ToArray()),
            _ => new[] { item },
        };
    }
}
