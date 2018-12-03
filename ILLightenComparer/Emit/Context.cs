using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit
{
    internal sealed class Context : IContext
    {
        private readonly ComparerTypeBuilder _comparerTypeBuilder;

        private readonly ConcurrentDictionary<Type, Lazy<TypeInfo>> _comparerTypes =
            new ConcurrentDictionary<Type, Lazy<TypeInfo>>();

        private readonly ConcurrentDictionary<Type, Configuration> _configurations =
            new ConcurrentDictionary<Type, Configuration>();

        // ReSharper disable once NotAccessedField.Local // todo: remove the comment
        private readonly EqualityComparerTypeBuilder _equalityComparerTypeBuilder;

        private readonly ModuleBuilder _moduleBuilder;

        private Configuration _defaultConfiguration = new Configuration(
            new HashSet<string>(),
            false,
            new string[0],
            StringComparison.Ordinal);

        public Context(ModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;
            _comparerTypeBuilder = CreateComparerTypeBuilder(this);
            _equalityComparerTypeBuilder = new EqualityComparerTypeBuilder(this, null);
        }

        // todo: cache delegates
        public int Compare<T>(T x, T y, HashSet<object> hash)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T); // todo: test with structs

            var compareMethod = GetStaticCompareMethod(type);

            var compare = compareMethod.CreateDelegate<Method.StaticMethodDelegate<T>>(); // todo: test with abstract class

            return compare(this, x, y, hash);
        }

        public void DefineConfiguration(Type type, ComparerSettings settings)
        {
            _configurations.AddOrUpdate(
                type,
                _ => _defaultConfiguration.Mutate(settings),
                (_, configuration) => configuration.Mutate(settings));
        }

        public void DefineDefaultConfiguration(ComparerSettings settings)
        {
            _defaultConfiguration = _defaultConfiguration.Mutate(settings);
        }

        public TypeInfo GetComparerType(Type objectType)
        {
            var lazy = _comparerTypes.GetOrAdd(
                objectType,
                type => new Lazy<TypeInfo>(() => _comparerTypeBuilder.Build(type)));

            return lazy.Value;
        }

        public MethodInfo GetStaticCompareMethod(Type memberType)
        {
            var comparerType = GetComparerType(memberType);

            return comparerType.GetMethod(
                MethodName.Compare,
                Method.StaticCompareMethodParameters(memberType));
        }

        public TypeBuilder DefineType(string name, params Type[] interfaceTypes) =>
            _moduleBuilder.DefineType(name, interfaceTypes);

        public Configuration GetConfiguration(Type type) =>
            _configurations.TryGetValue(type, out var configuration)
                ? configuration
                : _defaultConfiguration;

        private static ComparerTypeBuilder CreateComparerTypeBuilder(Context context)
        {
            Func<MemberInfo, IAcceptor>[] propertyFactories =
            {
                StringPropertyMember.Create,
                IntegralPropertyMember.Create,
                NullablePropertyMember.Create,
                BasicPropertyMember.Create,
                HierarchicalPropertyMember.Create
            };

            Func<MemberInfo, IAcceptor>[] fieldFactories =
            {
                StringFieldMember.Create,
                IntegralFieldMember.Create,
                NullableFieldMember.Create,
                BasicFieldMember.Create,
                HierarchicalFieldMember.Create
            };
            var converter = new MemberConverter(context, propertyFactories, fieldFactories);

            return new ComparerTypeBuilder(context, new MembersProvider(context, converter));
        }

        public struct Configuration
        {
            public readonly HashSet<string> IgnoredMembers;
            public readonly bool IncludeFields;
            public readonly string[] MembersOrder;
            public readonly StringComparison StringComparisonType;

            public Configuration Mutate(ComparerSettings settings) =>
                new Configuration(
                    settings.IgnoredMembers == null
                        ? IgnoredMembers
                        : new HashSet<string>(settings.IgnoredMembers),
                    settings.IncludeFields ?? IncludeFields,
                    settings.MembersOrder ?? MembersOrder,
                    settings.StringComparisonType ?? StringComparisonType);

            public Configuration(
                HashSet<string> ignoredMembers,
                bool includeFields,
                string[] membersOrder,
                StringComparison stringComparisonType)
            {
                IgnoredMembers = ignoredMembers ?? throw new ArgumentNullException(nameof(ignoredMembers));
                IncludeFields = includeFields;
                MembersOrder = membersOrder?.Distinct().ToArray()
                               ?? throw new ArgumentNullException(nameof(membersOrder));
                StringComparisonType = stringComparisonType;
            }
        }
    }

    public interface IContext
    {
        int Compare<T>(T x, T y, HashSet<object> hash);
    }
}
