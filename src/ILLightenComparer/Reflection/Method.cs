using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Comparer;
using ILLightenComparer.Shared;
using Illuminator.Extensions;

namespace ILLightenComparer.Reflection
{
    internal static class Method
    {
        public static readonly MethodInfo StringCompare = typeof(string).GetMethod(
            nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public static readonly ConstructorInfo ConcurrentSetConstructor =
            typeof(CycleDetectionSet).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo ConcurrentSetAddMethod =
            typeof(CycleDetectionSet).GetMethod(nameof(CycleDetectionSet.TryAdd), new[] { typeof(object), typeof(byte) });

        public static readonly MethodInfo ConcurrentSetGetCountProperty =
            typeof(CycleDetectionSet).GetProperty(nameof(CycleDetectionSet.Count))?.GetGetMethod();

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
                   .Where(x => x.Name == nameof(Array.Sort) && x.IsGenericMethodDefinition)
                   .Single(x => {
                       var parameters = x.GetParameters();
                       return parameters.Length == 1 && parameters[0].ParameterType.IsArray;
                   })
                   .MakeGenericMethod(elementType);
        }

        public static TResult InvokeCompare<TContext, TComparable, TResult>(
            this MethodInfo method,
            Type actualType,
            TContext context,
            TComparable x,
            TComparable y,
            CycleDetectionSet xSet,
            CycleDetectionSet ySet)
        {
            var isDeclaringTypeMatchedActualMemberType = typeof(TComparable) == actualType;
            if (!isDeclaringTypeMatchedActualMemberType) {
                // todo: 1. cache delegates and benchmark ways:
                // - direct Invoke;
                // - DynamicInvoke;
                // var genericType = typeof(Method.StaticMethodDelegate<>).MakeGenericType(type);
                // var @delegate = compareMethod.CreateDelegate(genericType);
                // return (int)@delegate.DynamicInvoke(this, x, y, hash);
                // - DynamicMethod;
                // - generate static class wrapper.

                return (TResult)method.Invoke(
                    null,
                    new object[] { context, x, y, xSet, ySet });
            }

            var compare = method.CreateDelegate<
                Func<TContext, TComparable, TComparable, CycleDetectionSet, CycleDetectionSet, TResult>>();

            return compare(context, x, y, xSet, ySet);
        }
    }
}
