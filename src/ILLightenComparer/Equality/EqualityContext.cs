using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Comparer;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;
using Illuminator.Extensions;

namespace ILLightenComparer.Equality
{
    internal sealed class EqualityContext : IEqualityComparerContext
    {
        private readonly IConfigurationProvider _configurations;
        private readonly ComparersCollection _emittedComparers = new ComparersCollection();
        private readonly EqualityProvider _provider;

        public EqualityContext(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _provider = new EqualityProvider(configurations);
        }

        public bool DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            var comparer = _configurations.GetCustomEqualityComparer<T>();
            if (comparer != null) {
                return comparer.Equals(x, y);
            }

            if (!typeof(T).IsValueType) {
                if (x == null && y == null) { return true; }
                if (y == null || y == null) { return false; }
            }

            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType) {
                throw new ArgumentException($"Argument types {xType} and {yType} are not matched.");
            }

            var compareMethod = _provider.GetCompiledStaticCompareMethod(xType);

            return compareMethod.StaticCompare<IEqualityComparerContext, T, bool>(xType, this, x, y, xSet, ySet);
        }

        public int DelayedHash<T>(T comparable, ConcurrentSet<object> cycleDetectionSet)
        {
            var comparer = _configurations.GetCustomEqualityComparer<T>();
            if (comparer != null) {
                return comparer.GetHashCode(comparable);
            }

            if (!typeof(T).IsValueType && comparable == null) {
                return 0;
            }

            var actualType = comparable.GetType();

            var compareMethod = _provider.GetCompiledStaticCompareMethod(actualType);

            return GetHash(compareMethod, actualType, comparable, cycleDetectionSet);
        }

        private int GetHash<TComparable>(
            MethodInfo method,
            Type actualType,
            TComparable comparable,
            ConcurrentSet<object> cycleDetectionSet)
        {
            var isDeclaringTypeMatchedActualMemberType = typeof(TComparable) == actualType;
            if (!isDeclaringTypeMatchedActualMemberType) {
                return (int)method.Invoke(
                    null,
                    new object[] { this, comparable, cycleDetectionSet });
            }

            var compare = method.CreateDelegate<Func<IEqualityComparerContext, TComparable, ConcurrentSet<object>, int>>();

            return compare(this, comparable, cycleDetectionSet);
        }

        public IEqualityComparer<T> GetEqualityComparer<T>() =>
            _configurations.GetCustomEqualityComparer<T>()
               ?? (IEqualityComparer<T>)_emittedComparers.GetOrAdd(typeof(T),
                   key => _provider
                   .EnsureComparerType(key)
                   .CreateInstance<IEqualityComparerContext, IEqualityComparer<T>>(this));
    }

    internal interface IEqualityComparerContext : IEqualityComparerProvider
    {
        bool DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet);

        int DelayedHash<T>(T x, ConcurrentSet<object> xSet);
    }
}
