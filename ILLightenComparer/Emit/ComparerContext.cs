using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit
{
    public interface IComparerContext
    {
        int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet);
    }

    internal sealed class ComparerContext : IComparerContext
    {
        private readonly ConcurrentDictionary<Type, Lazy<BuildInfo>> _buildScope = new ConcurrentDictionary<Type, Lazy<BuildInfo>>();
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, Lazy<Type>> _comparerTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        private readonly ConfigurationBuilder _configurationBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        public ComparerContext(ModuleBuilder moduleBuilder, ConfigurationBuilder configurationBuilder)
        {
            _moduleBuilder = moduleBuilder;
            _configurationBuilder = configurationBuilder;
            _comparerTypeBuilder = CreateComparerTypeBuilder(this);
        }

        // todo: cache delegates and benchmark ways
        public int DelayedCompare<T>(T x, T y, ConcurrentSet<object> xSet, ConcurrentSet<object> ySet)
        {
            // todo: do not check types when T is struct
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

        public Type GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                type => new Lazy<Type>(() =>
                {
                    var buildInfo = EnqueueBuild(type);

                    var result = _comparerTypeBuilder.Build(
                        (TypeBuilder)buildInfo.ComparerType,
                        (MethodBuilder)buildInfo.CompareMethod,
                        buildInfo.ObjectType);

                    buildInfo.ComparerType = result;
                    buildInfo.CompareMethod = result.GetMethod(
                        MethodName.Compare,
                        Method.StaticCompareMethodParameters(type));
                    buildInfo.Compiled = true;

                    return result;
                }));

            var comparerType = lazy.Value;

            foreach (var item in _buildScope)
            {
                if (item.Value.Value.Compiled)
                {
                    continue;
                }

                GetComparerType(item.Value.Value.ObjectType);
            }

            return comparerType;
        }

        public Configuration GetConfiguration(Type type) => _configurationBuilder.GetConfiguration(type);

        public MethodInfo GetStaticCompareMethod(Type memberType) => EnqueueBuild(memberType).CompareMethod;

        private MethodInfo GetCompiledCompareMethod(Type memberType)
        {
            var comparerType = GetComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        private BuildInfo EnqueueBuild(Type objectType)
        {
            var lazy = _buildScope.GetOrAdd(objectType,
                type => new Lazy<BuildInfo>(() =>
                {
                    var basicInterface = typeof(IComparer);
                    var genericInterface = typeof(IComparer<>).MakeGenericType(objectType);

                    var typeBuilder = _moduleBuilder.DefineType(
                        $"{objectType.FullName}.DynamicComparer",
                        basicInterface,
                        genericInterface);

                    var staticCompareMethodBuilder = typeBuilder.DefineStaticMethod(
                        MethodName.Compare,
                        typeof(int),
                        Method.StaticCompareMethodParameters(objectType));

                    return new BuildInfo(type, typeBuilder, staticCompareMethodBuilder);
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

        private static ComparerTypeBuilder CreateComparerTypeBuilder(ComparerContext context)
        {
            Func<MemberInfo, IAcceptor>[] propertyFactories =
            {
                StringPropertyMember.Create,
                IntegralPropertyMember.Create,
                BasicPropertyMember.Create,
                ComparablePropertyMember.Create,
                HierarchicalPropertyMember.Create
            };

            Func<MemberInfo, IAcceptor>[] fieldFactories =
            {
                StringFieldMember.Create,
                IntegralFieldMember.Create,
                BasicFieldMember.Create,
                ComparableFieldMember.Create,
                HierarchicalFieldMember.Create
            };
            var converter = new MemberConverter(context, propertyFactories, fieldFactories);

            return new ComparerTypeBuilder(context, new MembersProvider(context, converter));
        }
    }
}
