using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Comparer
{
    internal sealed class ComparerContext : IComparerContext
    {
        private readonly IConfigurationProvider _configurations;
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ComparersCollection _emittedComparers = new ComparersCollection();
        private readonly GenericProvider _genericProvider;
        private readonly ComparerMethodProvider _provider;

        public ComparerContext(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _configurations = configurations;
            _genericProvider = new GenericProvider(typeof(IComparer<>), typeof(IComparerContext), BuiltType);
            _provider = new ComparerMethodProvider(_genericProvider);
            var resolver = new ComparisonResolver(_provider, _configurations);
            _comparerTypeBuilder = new ComparerTypeBuilder(resolver, _configurations);
        }

        private Type BuiltType(StaticMethodsInfo info, Type objectType)
        {
            var methodInfo = info.GetMethodInfo(MethodName.Compare);
            Debug.Assert(!info.IsCompiled(MethodName.Compare), "Not compiled method is expected.");

            return _comparerTypeBuilder.Build(
                (TypeBuilder)methodInfo.DeclaringType,
                (MethodBuilder)methodInfo,
                objectType);
        }

        public IComparer<T> GetComparer<T>() => _configurations.GetCustomComparer<T>()
           ?? (IComparer<T>)_emittedComparers.GetOrAdd(
               typeof(T),
               key => _genericProvider.EnsureComparerType(key).CreateInstance<IComparerContext, IComparer<T>>(this));

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
    }

    internal interface IComparerContext : IComparerProvider, IContex
    {
        int DelayedCompare<T>(T x, T y, CycleDetectionSet xSet, CycleDetectionSet ySet);
    }
}
