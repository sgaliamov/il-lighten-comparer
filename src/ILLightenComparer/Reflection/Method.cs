using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Comparer;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Reflection
{
    internal static class Method
    {
        public delegate TOut StaticCompareMethodDelegate<in TComparable, in TContext, out TOut>(
            TContext context,
            TComparable x,
            TComparable y,
            ConcurrentSet<object> xSet,
            ConcurrentSet<object> ySet);

         public delegate int StaticHashMethodDelegate<in TComparable, in TContext, out TOut>(
            TContext context,
            TComparable comparable,
            ConcurrentSet<object> cycleDetectionSet);

        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo ConcurrentSetConstructor =
            typeof(ConcurrentSet<object>).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo ConcurrentSetAddMethod =
            typeof(ConcurrentSet<object>).GetMethod(nameof(ConcurrentSet<object>.TryAdd), new[] { typeof(object), typeof(byte) });

        public static readonly MethodInfo ConcurrentSetGetCountProperty =
            typeof(ConcurrentSet<object>).GetProperty(nameof(ConcurrentSet<object>.Count))?.GetGetMethod();

        public static MethodInfo DelayedCompare =
            typeof(IComparerContext).GetMethod(nameof(IComparerContext.DelayedCompare));

        public static MethodInfo GetComparer =
            typeof(IComparerProvider).GetMethod(nameof(IComparerProvider.GetComparer));

        public static MethodInfo MoveNext = typeof(IEnumerator)
            .GetMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);

        public static MethodInfo Dispose = typeof(IDisposable)
            .GetMethod(nameof(IDisposable.Dispose), Type.EmptyTypes);

        public static MethodInfo ToArray = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));

        public static MethodInfo GetArraySortWithComparer(Type elementType)
        {
            return typeof(Array)
                   .GetMethods(BindingFlags.Static | BindingFlags.Public)
                   .Where(x => x.Name == nameof(Array.Sort)
                            && x.IsGenericMethodDefinition)
                   .Single(x => {
                       var parameters = x.GetParameters();

                       return parameters.Length == 2
                              && parameters[0].ParameterType.IsArray
                              && parameters[1].ParameterType.IsGenericType
                              && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(IComparer<>);
                   })
                   .MakeGenericMethod(elementType);
        }

        public static MethodInfo GetArraySort(Type elementType)
        {
            return typeof(Array)
                   .GetMethods(BindingFlags.Static | BindingFlags.Public)
                   .Where(x => x.Name == nameof(Array.Sort)
                            && x.IsGenericMethodDefinition)
                   .Single(x => {
                       var parameters = x.GetParameters();
                       return parameters.Length == 1 && parameters[0].ParameterType.IsArray;
                   })
                   .MakeGenericMethod(elementType);
        }

        public static Type[] StaticCompareMethodParameters(Type objectType)
        {
            return new[] {
                typeof(IComparerContext),
                objectType,
                objectType,
                typeof(ConcurrentSet<object>),
                typeof(ConcurrentSet<object>)
            };
        }
    }
}
