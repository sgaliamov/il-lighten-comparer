using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members.Comparable;
using ILLightenComparer.Emit.Members.Integral;

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

        private static readonly Converter[] Converters =
        {
            new Converter(GetPropertyType, IsString, info => new StringPropertyMember((PropertyInfo)info)),
            new Converter(GetPropertyType, IsIntegral, info => new IntegralPropertyMember((PropertyInfo)info)),
            new Converter(GetPropertyType, IsComparable, info => new ComparablePropertyMember((PropertyInfo)info)),

            new Converter(GetFieldType, IsString, info => new StringFiledMember((FieldInfo)info)),
            new Converter(GetFieldType, IsIntegral, info => new IntegralFiledMember((FieldInfo)info)),
            new Converter(GetFieldType, IsComparable, info => new ComparableFieldMember((FieldInfo)info))
        };

        public IMember Convert(MemberInfo memberInfo)
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
                $"{memberInfo.MemberType} {memberInfo.DisplayName()} is not supported.");
        }

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

        private static bool IsComparable(Type type) => type.GetCompareToMethod() != null;

        private static bool IsString(Type type) => type == typeof(string);

        private static bool IsIntegral(Type type) => SmallIntegralTypes.Contains(type.GetUnderlyingType());

        private sealed class Converter
        {
            public Converter(
                Func<MemberInfo, (MemberInfo, Type)> convert,
                Func<Type, bool> condition,
                Func<MemberInfo, IMember> factory)
            {
                Convert = convert;
                Condition = condition;
                Factory = factory;
            }

            public Func<Type, bool> Condition { get; }
            public Func<MemberInfo, (MemberInfo, Type)> Convert { get; }
            public Func<MemberInfo, IMember> Factory { get; }
        }
    }
}
