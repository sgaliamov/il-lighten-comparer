using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        private static readonly HashSet<Type> SmallIntegralTypes = new HashSet<Type>(new[]
        {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort)
        });

        // todo: split on two collections and use IncludeFields setting
        private static readonly Converter[] Converters =
        {
            new Converter(GetPropertyType, IsString, info => new StringPropertyMember((PropertyInfo)info)),
            new Converter(GetPropertyType, IsIntegral, info => new IntegralPropertyMember((PropertyInfo)info)),
            new Converter(GetPropertyType, IsComparable, info => new ComparablePropertyMember((PropertyInfo)info)),
            new Converter(GetPropertyType, IsNullable, info => new NullablePropertyMember((PropertyInfo)info)),

            new Converter(GetFieldType, IsString, info => new StringFiledMember((FieldInfo)info)),
            new Converter(GetFieldType, IsIntegral, info => new IntegralFiledMember((FieldInfo)info)),
            new Converter(GetFieldType, IsComparable, info => new ComparableFieldMember((FieldInfo)info)),
            new Converter(GetFieldType, IsNullable, info => new NullableFieldMember((FieldInfo)info))
        };

        public IAcceptor Convert(MemberInfo memberInfo)
        {
            foreach (var converter in Converters)
            {
                var (info, memberType) = converter.Convert(memberInfo);
                if (info == null || memberType == null)
                {
                    continue;
                }

                if (converter.Condition(memberType))
                {
                    return converter.Factory(info);
                }
            }

            throw new NotSupportedException(
                $"{memberInfo.DisplayName()} is not supported.");
        }

        private static bool IsNullable(Type type) => type.IsNullable();

        private static (MemberInfo, Type) GetPropertyType(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return (propertyInfo, propertyInfo.PropertyType);
            }

            return default;
        }

        private static (MemberInfo, Type) GetFieldType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return (fieldInfo, fieldInfo.FieldType);
            }

            return default;
        }

        private static bool IsComparable(Type type) => 
            !type.IsNullable() && type.GetCompareToMethod() != null;

        private static bool IsString(Type type) => type == typeof(string);

        private static bool IsIntegral(Type type) =>
            !type.IsNullable() && SmallIntegralTypes.Contains(type.GetUnderlyingType());

        private sealed class Converter
        {
            public Converter(
                Func<MemberInfo, (MemberInfo, Type)> convert,
                Func<Type, bool> condition,
                Func<MemberInfo, IAcceptor> factory)
            {
                Convert = convert;
                Condition = condition;
                Factory = factory;
            }

            public Func<Type, bool> Condition { get; }
            public Func<MemberInfo, (MemberInfo, Type)> Convert { get; }
            public Func<MemberInfo, IAcceptor> Factory { get; }
        }
    }
}
