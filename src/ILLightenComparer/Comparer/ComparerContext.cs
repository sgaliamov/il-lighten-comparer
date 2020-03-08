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
        private readonly IConfigurationProvider _configurations;
        private readonly ComparersCollection _emittedComparers = new ComparersCollection();
        private readonly ComparerProvider _provider;

        public ComparerContext(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _provider = new ComparerProvider(configurations);
        }

        public IComparer<T> GetComparer<T>() => _configurations.GetCustomComparer<T>()
           ?? (IComparer<T>)_emittedComparers.GetOrAdd(
               typeof(T),
               key => _provider.EnsureComparerType(key).CreateInstance<IComparerContext, IComparer<T>>(this));

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

            return compareMethod.StaticCompare<IComparerContext, T, int>(xType, this, x, y, xSet, ySet);
        }
    }

    internal interface IComparerContext : IComparerProvider
    {
        int DelayedCompare<T>(T x, T y, CycleDetectionSet xSet, CycleDetectionSet ySet);
    }
}
