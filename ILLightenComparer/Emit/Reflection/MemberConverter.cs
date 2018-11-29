using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        private static readonly Converter[] PropertyConverters =
        {
            new Converter(IsString, info => new StringPropertyMember((PropertyInfo)info)),

            new Converter(
                TypeExtensions.IsSmallIntegral,
                info => new IntegralPropertyMember((PropertyInfo)info)),

            new Converter(
                TypeExtensions.IsNullable,
                info => new NullablePropertyMember((PropertyInfo)info)),

            new Converter(
                TypeExtensions.IsBasic,
                info => new BasicPropertyMember((PropertyInfo)info)),

            new Converter(_ => true, info => new HierarchicalPropertyMember((PropertyInfo)info))
        };

        private static readonly Converter[] FieldConverters =
        {
            new Converter(IsString, info => new StringFiledMember((FieldInfo)info)),

            new Converter(
                TypeExtensions.IsSmallIntegral,
                info => new IntegralFiledMember((FieldInfo)info)),

            new Converter(
                TypeExtensions.IsNullable,
                info => new NullableFieldMember((FieldInfo)info)),

            new Converter(TypeExtensions.IsBasic, info => new BasicFieldMember((FieldInfo)info)),

            new Converter(_ => true, info => new HierarchicalFieldMember((FieldInfo)info))
        };

        private readonly Context _context;

        public MemberConverter(Context context) => _context = context;

        public IAcceptor Convert(MemberInfo memberInfo)
        {
            var acceptor = Convert(memberInfo, GetPropertyType(memberInfo), PropertyConverters);

            var includeFields = _context.GetConfiguration(memberInfo.DeclaringType).IncludeFields;
            if (acceptor == null && includeFields)
            {
                acceptor = Convert(memberInfo, GetFieldType(memberInfo), FieldConverters);
            }

            return acceptor ?? throw new NotSupportedException($"{memberInfo.DisplayName()} is not supported.");
        }

        private static IAcceptor Convert(
            MemberInfo memberInfo,
            Type memberType,
            IEnumerable<Converter> converters)
        {
            if (memberType == null)
            {
                return null;
            }

            return converters
                   .Where(converter => converter.Condition(memberType))
                   .Select(converter => converter.Factory(memberInfo))
                   .FirstOrDefault();
        }

        private static Type GetPropertyType(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo propertyInfo ? propertyInfo.PropertyType : default;

        private static Type GetFieldType(MemberInfo memberInfo) =>
            memberInfo is FieldInfo fieldInfo ? fieldInfo.FieldType : default;

        private static bool IsString(Type type) => type == typeof(string);

        private sealed class Converter
        {
            public Converter(
                Func<Type, bool> condition,
                Func<MemberInfo, IAcceptor> factory)
            {
                Condition = condition;
                Factory = factory;
            }

            public Func<Type, bool> Condition { get; }
            public Func<MemberInfo, IAcceptor> Factory { get; }
        }
    }
}
