using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Builders;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters
{
    /// <summary>
    ///     Provides access to cached comparers and comparison methods to compile types.
    /// </summary>
    internal sealed class Context : IComparerProvider, IContext
    {
        private readonly IConfigurationProvider _configurations;
        private readonly ContextBuilder _contextBuilder;
        private readonly ConcurrentDictionary<Type, IComparer> _dynamicComparers = new ConcurrentDictionary<Type, IComparer>();

        public Context(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            _contextBuilder = new ContextBuilder(configurations);
        }

        // todo: test - define configuration, get comparer, change configuration, get comparer, they should be different
        public IComparer GetComparer(Type objectType)
        {
            var configuration = _configurations.Get(objectType);
            var customComparer = configuration.GetComparer(objectType);
            if (customComparer != null)
            {
                return customComparer;
            }

            return _dynamicComparers.GetOrAdd(
                objectType,
                key => GetOrBuildComparerType(key).CreateInstance<IContext, IComparer>(this));
        }

        public IEqualityComparer GetEqualityComparer(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            throw new NotImplementedException();
        }

        // todo: cache delegates and benchmark ways
        public IComparer<T> GetComparer<T>()
        {
            var objectType = typeof(T);
        }

        public int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
#if DEBUG
            if (typeof(T).IsValueType)
            {
                throw new InvalidOperationException($"Unexpected value type {typeof(T)}.");
            }
#endif

            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType)
            {
                throw new ArgumentException($"Argument types {xType} and {yType} are not matched.");
            }

            return Compare(xType, x, y, xSet, ySet);
        }

        public MethodInfo GetStaticCompareMethod(Type type)
        {
            return GetOrStartBuild(type).CompareMethod;
        }

        private int Compare<T>(Type type, T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            var compareMethod = GetCompiledCompareMethod(type);

            var isDeclaringTypeMatchedActualMemberType = typeof(T) == type;
            if (!isDeclaringTypeMatchedActualMemberType)
            {
                // todo: benchmarks:
                // - direct Invoke;
                // - DynamicInvoke;
                // var genericType = typeof(Method.StaticMethodDelegate<>).MakeGenericType(type);
                // var @delegate = compareMethod.CreateDelegate(genericType);
                // return (int)@delegate.DynamicInvoke(this, x, y, hash);
                // - DynamicMethod;
                // - generate static class wrapper.

                return (int)compareMethod.Invoke(
                    null,
                    new object[] { this, x, y, xSet, ySet });
            }

            var compare = compareMethod.CreateDelegate<Method.StaticMethodDelegate<T>>();

            return compare(this, x, y, xSet, ySet);
        }
    }

    public interface IContext
    {
        IComparer<T> GetComparer<T>();
        int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet);
        MethodInfo GetStaticCompareMethod(Type type);
    }
}
