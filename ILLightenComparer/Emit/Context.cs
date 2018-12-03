using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    using ComparerTypes = ConcurrentDictionary<Type, Lazy<TypeInfo>>;

    internal sealed class Context : IContext
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;
        private readonly ComparerTypes _comparerTypes = new ComparerTypes();
        private readonly ConfigurationBuilder _configurations;

        // ReSharper disable once NotAccessedField.Local // todo: remove the comment
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        public Context(ModuleBuilder moduleBuilder, ConfigurationBuilder configurations)
        {
            _moduleBuilder = moduleBuilder;
            _configurations = configurations;
            _comparerTypeBuilder = CreateComparerTypeBuilder(this);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(this, null);
        }

        // todo: cache delegates
        public int Compare<T>(T x, T y, HashSet<object> hash)
        {
            var type = x?.GetType() ?? y?.GetType(); // todo: test with structs
            if (type == null)
            {
                return 0;
            }

            var compareMethod = GetStaticCompareMethod(type);

            if (typeof(T) != type)
            {
                //return (int)compareMethod.Invoke(null, new object[] { this, x, y, hash });
                var genericType = typeof(Method.StaticMethodDelegate<>).MakeGenericType(type);
                var @delegate = compareMethod.CreateDelegate(genericType);

                return (int)@delegate.DynamicInvoke(this, x, y, hash);
            }

            var compare = compareMethod.CreateDelegate<Method.StaticMethodDelegate<T>>();

            return compare(this, x, y, hash);
        }

        public TypeInfo GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                type => new Lazy<TypeInfo>(() => _comparerTypeBuilder.Build(type)));

            return lazy.Value;
        }

        public Configuration GetConfiguration(Type type) => _configurations.GetConfiguration(type);

        public MethodInfo GetStaticCompareMethod(Type memberType)
        {
            var comparerType = GetComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);

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
