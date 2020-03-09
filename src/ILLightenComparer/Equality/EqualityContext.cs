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
        private readonly EqualityMethodsProvider _provider;
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly GenericProvider _genericProvider;

        public EqualityContext(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _configurations = configurations;
            var resolver = new ComparisonResolver(null, _configurations);
            _comparerTypeBuilder = new ComparerTypeBuilder(resolver, _configurations);
            _genericProvider = new GenericProvider(typeof(IEqualityComparer<>), (_) => null);

            _provider = new EqualityMethodsProvider(_genericProvider);
        }

        public bool DelayedCompare<T>(T x, T y, CycleDetectionSet xSet, CycleDetectionSet ySet)
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

            var compareMethod = _provider.GetCompiledStaticEqualsMethod(xType);

            return compareMethod.InvokeCompare<IEqualityComparerContext, T, bool>(xType, this, x, y, xSet, ySet);
        }

        public int DelayedHash<T>(T comparable, CycleDetectionSet cycleDetectionSet)
        {
            var comparer = _configurations.GetCustomEqualityComparer<T>();
            if (comparer != null) {
                return comparer.GetHashCode(comparable);
            }

            if (!typeof(T).IsValueType && comparable == null) {
                return 0;
            }

            var actualType = comparable.GetType();

            var hashMethod = _provider.GetCompiledStaticHashMethod(actualType);

            return GetHash(hashMethod, actualType, comparable, cycleDetectionSet);
        }

        private int GetHash<TComparable>(
            MethodInfo method,
            Type actualType,
            TComparable comparable,
            CycleDetectionSet cycleDetectionSet)
        {
            var isDeclaringTypeMatchedActualMemberType = typeof(TComparable) == actualType;
            if (!isDeclaringTypeMatchedActualMemberType) {
                // todo: refactor after Method.InvokeCompare
                return (int)method.Invoke(
                    null,
                    new object[] { this, comparable, cycleDetectionSet });
            }

            var compare = method.CreateDelegate<Func<IEqualityComparerContext, TComparable, CycleDetectionSet, int>>();

            return compare(this, comparable, cycleDetectionSet);
        }

        public IEqualityComparer<T> GetEqualityComparer<T>() =>
            _configurations.GetCustomEqualityComparer<T>()
               ?? (IEqualityComparer<T>)_emittedComparers.GetOrAdd(typeof(T),
                   key => _genericProvider
                   .EnsureComparerType(key)
                   .CreateInstance<IEqualityComparerContext, IEqualityComparer<T>>(this));
    }

    internal interface IEqualityComparerContext : IEqualityComparerProvider, IContex
    {
        bool DelayedCompare<T>(T x, T y, CycleDetectionSet xSet, CycleDetectionSet ySet);

        int DelayedHash<T>(T x, CycleDetectionSet xSet);
    }
}
