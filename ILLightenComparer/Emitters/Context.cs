using System;
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
        private readonly IConfigurationProvider _configurations;
        private readonly ContextBuilder _contextBuilder;
        private readonly ComparersCollection _dynamicComparers = new ComparersCollection();

        public Context(IConfigurationProvider configurations)
        {
            _configurations = configurations;
            // todo: think about relations and shared responsibilities with `ContextBuilder`
            _contextBuilder = new ContextBuilder(this, configurations);
        }

        public IComparer<T> GetComparer<T>()
        {
            var comparer = _configurations.GetCustomComparer<T>();
            if (comparer != null)
            {
                return comparer;
            }

            return (IComparer<T>)_dynamicComparers.GetOrAdd(
                typeof(T),
                key => _contextBuilder.EnsureComparerType(key).CreateInstance<IContext, IComparer<T>>(this));
        }

        public int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            var comparer = _configurations.GetCustomComparer<T>();
            if (comparer != null)
            {
                return comparer.Compare(x, y);
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
