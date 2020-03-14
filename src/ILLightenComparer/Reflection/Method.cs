using System;
using System.Reflection;
using ILLightenComparer.Comparer;
using ILLightenComparer.Equality;
using ILLightenComparer.Shared;
using Illuminator.Extensions;

namespace ILLightenComparer.Reflection
{
    internal static class Method
    {
        public static MethodInfo DelayedCompare =
            typeof(IComparerContext).GetMethod(nameof(IComparerContext.DelayedCompare));

        public static MethodInfo DelayedEquals =
           typeof(IEqualityComparerContext).GetMethod(nameof(IEqualityComparerContext.DelayedEquals));

        public static MethodInfo DelayedHash =
            typeof(IEqualityComparerContext).GetMethod(nameof(IEqualityComparerContext.DelayedHash));

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
