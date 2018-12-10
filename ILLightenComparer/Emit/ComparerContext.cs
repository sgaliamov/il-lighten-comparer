using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using ILLightenComparer.Config;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    using Set = ConcurrentDictionary<object, byte>;

    public interface IComparerContext
    {
        int Compare<T>(T x, T y, Set xSet, Set ySet);
    }

    internal sealed class ComparerContext : IComparerContext
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ConcurrentDictionary<Type, Type> _comparerTypes = new ConcurrentDictionary<Type, Type>();
        private readonly ConfigurationBuilder _configurations;

        // ReSharper disable once NotAccessedField.Local // todo: implement EqualityComparerTypeBuilder
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, byte> _typeHeap = new ConcurrentDictionary<Type, byte>();

        public ComparerContext(ModuleBuilder moduleBuilder, ConfigurationBuilder configurations)
        {
            _moduleBuilder = moduleBuilder;
            _configurations = configurations;
            _comparerTypeBuilder = CreateComparerTypeBuilder(this);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(this, null);
        }

        // todo: cache delegates and benchmark ways
        public int Compare<T>(T x, T y, ConcurrentDictionary<object, byte> xSet, ConcurrentDictionary<object, byte> ySet)
        {
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
            if (!_typeHeap.TryAdd(objectType, 0))
            {
                return null;
            }

            var comparerType = _comparerTypes.GetOrAdd(
                objectType,
                t => _comparerTypeBuilder.Build(t));

            if (_typeHeap.TryRemove(objectType, out _))
            {
                return comparerType;
            }

            throw new InvalidOperationException("Comparison context is not valid.");
        }

        public Configuration GetConfiguration(Type type) => _configurations.GetConfiguration(type);

        public MethodInfo GetStaticCompareMethod(Type memberType)
        {
            // todo: technically we need only compare method, no need to build the type first.
            // it will help to eliminate in context compare method and generate more effective code.
            var comparerType = GetComparerType(memberType);

            return comparerType?.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);

        private int Compare<T>(Type type, T x, T y, ConcurrentDictionary<object, byte> xSet, ConcurrentDictionary<object, byte> ySet)
        {
            var compareMethod = EnsureStaticCompareMethod(type);

            if (typeof(T) != type)
            {
                // todo: benchmarks:
                // - direct Invoke;
                // - DynamicInvoke;
                // var genericType = typeof(Method.StaticMethodDelegate<>).MakeGenericType(type);
                // var @delegate = compareMethod.CreateDelegate(genericType);
                // return (int)@delegate.DynamicInvoke(this, x, y, hash);
                // - DynamicMethod;
                // - generate static class wrapper.
                return (int)compareMethod.Invoke(null, new object[] { this, x, y, xSet, ySet });
            }

            var compare = compareMethod.CreateDelegate<Method.StaticMethodDelegate<T>>();

            return compare(this, x, y, xSet, ySet);
        }

        private MethodInfo EnsureStaticCompareMethod(Type type)
        {
            // todo: find smart way to ensure compare method exists
            while (true)
            {
                var compareMethod = GetStaticCompareMethod(type);
                if (compareMethod != null)
                {
                    return compareMethod;
                }

                Thread.Yield();
            }
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
