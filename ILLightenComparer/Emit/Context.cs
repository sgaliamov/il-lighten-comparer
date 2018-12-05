using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    using ComparerTypes = ConcurrentDictionary<Type, Lazy<TypeInfo>>;
    using TypeHeap = ConcurrentDictionary<Type, byte>;

    internal sealed class Context : IContext
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ComparerTypes _comparerTypes = new ComparerTypes();
        private readonly ConfigurationBuilder _configurations;

        // ReSharper disable once NotAccessedField.Local // todo: implement EqualityComparerTypeBuilder
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly TypeHeap _typeHeap = new TypeHeap();

        public Context(ModuleBuilder moduleBuilder, ConfigurationBuilder configurations)
        {
            _moduleBuilder = moduleBuilder;
            _configurations = configurations;
            _comparerTypeBuilder = CreateComparerTypeBuilder(this);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(this, null);
        }

        // todo: cache delegates and benchmark ways
        public int Compare<T>(T x, T y, HashSet<object> hash)
        {
            if (x == null)
            {
                if (y == null) { return 0; }

                return -1;
            }

            if (y == null) { return 1; }

            var xType = x.GetType(); // todo: test with structs
            var yType = y.GetType();
            if (xType != yType)
            {
                throw new ArgumentException($"Argument types {xType} and {yType} are not matched.");
            }

            return Compare(xType, x, y, hash);
        }

        public TypeInfo GetComparerType(Type objectType)
        {
            if (!_typeHeap.TryAdd(objectType, 0)) { return null; }

            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                t => new Lazy<TypeInfo>(() => _comparerTypeBuilder.Build(t)));

            var comparerType = lazy.Value;
            if (_typeHeap.TryRemove(objectType, out _)) { return comparerType; }

            throw new InvalidOperationException("Comparison context is not valid.");
        }

        public Configuration GetConfiguration(Type type) => _configurations.GetConfiguration(type);

        public MethodInfo GetStaticCompareMethod(Type memberType)
        {
            var comparerType = GetComparerType(memberType);

            return comparerType?.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);

        private int Compare<T>(Type type, T x, T y, HashSet<object> hash)
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
                return (int)compareMethod.Invoke(null, new object[] { this, x, y, hash });
            }

            var compare = compareMethod.CreateDelegate<Method.StaticMethodDelegate<T>>();

            return compare(this, x, y, hash);
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

        private static ComparerTypeBuilder CreateComparerTypeBuilder(Context context)
        {
            Func<MemberInfo, IAcceptor>[] propertyFactories =
            {
                StringPropertyMember.Create,
                IntegralPropertyMember.Create,
                NullablePropertyMember.Create,
                BasicPropertyMember.Create,
                ComparablePropertyMember.Create,
                HierarchicalPropertyMember.Create
            };

            Func<MemberInfo, IAcceptor>[] fieldFactories =
            {
                StringFieldMember.Create,
                IntegralFieldMember.Create,
                NullableFieldMember.Create,
                BasicFieldMember.Create,
                ComparableFieldMember.Create,
                HierarchicalFieldMember.Create
            };
            var converter = new MemberConverter(context, propertyFactories, fieldFactories);

            return new ComparerTypeBuilder(context, new MembersProvider(context, converter));
        }
    }

    public interface IContext
    {
        int Compare<T>(T x, T y, HashSet<object> hash);
    }
}
