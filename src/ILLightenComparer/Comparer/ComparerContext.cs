using System;
using System.Collections.Generic;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparerContext : IComparerContext
    {
        private readonly GenericProvider _genericProvider;
        private readonly ComparersCollection _emittedComparers = new ComparersCollection();
        private readonly IConfigurationProvider _configurations;
        private readonly ComparersMethodsProvider _provider;

        public ComparerContext(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _provider = new ComparersMethodsProvider(_genericProvider);
            var resolver = new ComparisonResolver(_provider, _configurations);
            var staticMethodEmitter = new ComparerStaticMethodEmitter(resolver, _configurations);
            _genericProvider = new GenericProvider(
                typeof(IComparer<>),
                new GenericTypeBuilder(_configurations, staticMethodEmitter));
        }

        public IComparer<T> GetComparer<T>() =>
            _configurations.GetCustomComparer<T>()
           ?? (IComparer<T>)_emittedComparers.GetOrAdd(typeof(T), key => CreateInstance<T>(key));

        public int DelayedCompare<T>(T x, T y, CycleDetectionSet xSet, CycleDetectionSet ySet)
        {
            var comparer = _configurations.GetCustomComparer<T>();
            if (comparer != null) {
                return comparer.Compare(x, y);
            }

            if (!typeof(T).IsValueType) {
                if (x == null) { return y == null ? 0 : -1; }

                if (y == null) { return 1; }
            }

            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType) {
                throw new ArgumentException($"Argument types {xType} and {yType} are not matched.");
            }

            var compareMethod = _provider.GetCompiledStaticCompareMethod(xType);

            return compareMethod.InvokeCompare<IComparerContext, T, int>(xType, this, x, y, xSet, ySet);
        }

        private IComparer<T> CreateInstance<T>(Type key) => _genericProvider
            .EnsureComparerType(key)
            .CreateInstance<IComparerContext, IComparer<T>>(this);
    }

    internal interface IComparerContext : IComparerProvider, IContext
    {
        int DelayedCompare<T>(T x, T y, CycleDetectionSet xSet, CycleDetectionSet ySet);
    }
}
