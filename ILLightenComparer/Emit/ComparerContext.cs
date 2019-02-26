using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit
{
    using Builds = ConcurrentDictionary<Type, Lazy<BuildInfo>>;
    using ComparerTypes = ConcurrentDictionary<Type, Lazy<Type>>;

    public interface IComparerContext
    {
        int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet);
        IComparer<T> GetComparer<T>();
    }

    internal sealed class ComparerContext : IComparerContext, IComparerProvider
    {
        private readonly Builds _builds = new Builds();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ComparerTypes _comparerTypes = new ComparerTypes();
        private readonly IConfigurationProvider _configurations;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, IComparer> _comparers = new ConcurrentDictionary<Type, IComparer>();

        public ComparerContext(IConfigurationProvider configurations)
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("ILLightenComparer"),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = assembly.DefineDynamicModule("ILLightenComparer.dll");

            _configurations = configurations;

            _comparerTypeBuilder = new ComparerTypeBuilder(this);
        }

        // todo: cache delegates and benchmark ways
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

        // todo: move comparer instances comparison to context
        public IComparer GetComparer(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IComparer<T> GetComparer<T>()
        {
            var comparerType = GetOrBuildComparerType(typeof(T));

            return comparerType.CreateInstance<IComparerContext, IComparer<T>>(this);
        }

        public IEqualityComparer GetEqualityComparer(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            throw new NotImplementedException();
        }

        public Type GetOrBuildComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                key => new Lazy<Type>(() =>
                {
                    var buildInfo = GetOrStartBuild(key);

                    var result = _comparerTypeBuilder.Build(
                        (TypeBuilder)buildInfo.Method.DeclaringType,
                        (MethodBuilder)buildInfo.Method,
                        buildInfo.ObjectType);

                    var method = result.GetMethod(MethodName.Compare, Method.StaticCompareMethodParameters(key));

                    buildInfo.FinalizeBuild(method);

                    return result;
                }));

            var comparerType = lazy.Value;

            FinalizeStartedBuilds();

            return comparerType;
        }

        public Configuration GetConfiguration(Type type)
        {
            return _configurations.GetConfiguration(type);
        }

        internal MethodInfo GetStaticCompareMethod(Type type)
        {
            return GetOrStartBuild(type).Method;
        }

        private void FinalizeStartedBuilds()
        {
            var builds = _builds.ToArray().Select(x => x.Value).ToArray();

            foreach (var item in builds)
            {
                if (item.Value.Compiled)
                {
                    continue;
                }

                GetOrBuildComparerType(item.Value.ObjectType);
            }
        }

        private MethodInfo GetCompiledCompareMethod(Type memberType)
        {
            var comparerType = GetOrBuildComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        private BuildInfo GetOrStartBuild(Type objectType)
        {
            var lazy = _builds.GetOrAdd(objectType,
                key => new Lazy<BuildInfo>(() =>
                {
                    var basicInterface = typeof(IComparer);
                    var genericInterface = typeof(IComparer<>).MakeGenericType(key);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{key.FullName}.DynamicComparer",
                        basicInterface,
                        genericInterface);

                    var staticCompareMethodBuilder = typeBuilder.DefineStaticMethod(
                        MethodName.Compare,
                        typeof(int),
                        Method.StaticCompareMethodParameters(key));

                    return new BuildInfo(key, staticCompareMethodBuilder);
                }));

            return lazy.Value;
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
}
