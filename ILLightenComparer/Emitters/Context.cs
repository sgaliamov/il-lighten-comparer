using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Builders;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters
{
    /// <summary>
    ///     Provides access to cached comparers and comparison methods to compile types.
    /// </summary>
    internal sealed class Context : IComparerProvider, IContext
    {
        private readonly ContextBuilder _contextBuilder;

        private readonly ConcurrentDictionary<Type, object> _customComparers = new ConcurrentDictionary<Type, object>();

        /// <summary>
        ///     <see cref="object" /> is IComparer&lt;<see cref="Type" />&gt;
        /// </summary>
        private readonly ConcurrentDictionary<Type, object> _dynamicComparers = new ConcurrentDictionary<Type, object>();

        public Context(IConfigurationProvider configurations)
        {
            _contextBuilder = new ContextBuilder(configurations);
        }

        public IComparer<T> GetComparer<T>()
        {
            return (IComparer<T>)_dynamicComparers.GetOrAdd(
                typeof(T),
                key => _contextBuilder.EnsureComparerType(key).CreateInstance<IContext, IComparer<T>>(this));
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            throw new NotImplementedException();
        }

        // todo: maybe possible use only GetComparer<T> method for delayed comparison,
        // as we could access static method via instance object
        public int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            if (_customComparers.TryGetValue(typeof(T), out var comparer))
            {
                return ((IComparer<T>)comparer).Compare(x, y);
            }

            #if DEBUG
            if (typeof(T).IsValueType)
            {
                throw new InvalidOperationException($"Unexpected value type {typeof(T)}.");
            }
            #endif

            if (x == null) { return y == null ? 0 : -1; }

            if (y == null) { return 1; }

            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType)
            {
                throw new ArgumentException($"Argument types {xType} and {yType} are not matched.");
            }

            return Compare(xType, x, y, xSet, ySet);
        }

        public void SetComparer(Type type, object instance)
        {
            // todo: test - define configuration, get comparer, change configuration, get comparer, they should be different
            if (instance == null)
            {
                _customComparers.TryRemove(type, out _);
            }

            _customComparers.AddOrUpdate(type, key => instance, (key, _) => instance);
        }

        // todo: cache delegates and benchmark ways
        private int Compare<T>(Type type, T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            var compareMethod = _contextBuilder.GetCompiledCompareMethod(type);

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
        int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet);
    }
}
