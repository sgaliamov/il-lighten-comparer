using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
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
            // todo: think about relations and shared responsibilities with `ContextBuilder`
            _contextBuilder = new ContextBuilder(this, configurations);
        }

        public IComparer<T> GetComparer<T>()
        {
            if (_customComparers.TryGetValue(typeof(T), out var comparer) && comparer != null)
            {
                return (IComparer<T>)comparer;
            }

            return (IComparer<T>)_dynamicComparers.GetOrAdd(
                typeof(T),
                key => _contextBuilder.EnsureComparerType(key).CreateInstance<IContext, IComparer<T>>(this));
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            throw new NotImplementedException();
        }

        public int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            if (_customComparers.TryGetValue(typeof(T), out var comparer))
            {
                return ((IComparer<T>)comparer).Compare(x, y);
            }

            if (!typeof(T).IsValueType)
            {
                if (x == null) { return y == null ? 0 : -1; }

                if (y == null) { return 1; }
            }

            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType)
            {
                throw new ArgumentException($"Argument types {xType} and {yType} are not matched.");
            }

            return Compare(xType, x, y, xSet, ySet);
        }

        public void SetCustomComparer(Type type, object instance)
        {
            // todo: test - define configuration, get comparer, change configuration, get comparer, they should be different
            if (instance == null)
            {
                _customComparers.TryRemove(type, out _);
                return;
            }

            _customComparers.AddOrUpdate(type, key => instance, (key, _) => instance);
        }

        public bool HasCustomComparer(Type type)
        {
            return _customComparers.ContainsKey(type);
        }

        public MethodInfo GetStaticCompareMethod(Type type)
        {
            return _contextBuilder.GetStaticCompareMethod(type);
        }

        private int Compare<T>(Type type, T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            var compareMethod = _contextBuilder.GetCompiledCompareMethod(type);

            var isDeclaringTypeMatchedActualMemberType = typeof(T) == type;
            if (!isDeclaringTypeMatchedActualMemberType)
            {
                // todo: cache delegates and benchmark ways:
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
