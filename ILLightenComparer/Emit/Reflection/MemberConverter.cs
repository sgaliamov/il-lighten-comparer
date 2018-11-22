using System;
using System.Collections.Generic;
using System.Reflection;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Members.Comparable;
using ILLightenComparer.Emit.Members.Integral;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        private static readonly HashSet<Type> IntegralTypes = new HashSet<Type>(new[]
        {
            typeof(sbyte),
            typeof(byte),
            typeof(char),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint)
        });

        private static readonly HashSet<Type> IntegerTypes = new HashSet<Type>(new[]
        {
            typeof(int),
            typeof(uint)
        });

        private static readonly MethodInfo StringCompareMethod = typeof(string)
            .GetMethod(
                nameof(string.Compare),
                new[] { typeof(string), typeof(string), typeof(StringComparison) });

        public IMember Convert(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    return Convert(field);

                case PropertyInfo property:
                    return Convert(property);
            }

            throw new NotSupportedException(
                "Only fields and properties are supported. "
                + $"{memberInfo.MemberType}: {memberInfo.DisplayName()}");
        }

        private static Member Convert(PropertyInfo property)
        {
            if (IntegralTypes.Contains(property.PropertyType))
            {
                return new IntegralPropertyMember(property, IntegerTypes.Contains(property.PropertyType));
            }

            if (property.PropertyType == typeof(string))
            {
                return new StringPropertyMember(property, StringCompareMethod);
            }

            // todo: try compare enums as integral types
            var underlyingType = GetEnumUnderlyingType(property.PropertyType);
            var compareToMethod = GetCompareToMethod(underlyingType);
            if (compareToMethod != null)
            {
                return new ComparablePropertyMember(property, compareToMethod);
            }

            throw new NotSupportedException($"Property {property.DisplayName()} is not supported.");
        }

        private static Member Convert(FieldInfo field)
        {
            if (IntegralTypes.Contains(field.FieldType))
            {
                return new IntegralFiledMember(field, IntegerTypes.Contains(field.FieldType));
            }

            if (field.FieldType == typeof(string))
            {
                return new StringFiledMember(field, StringCompareMethod);
            }

            var underlyingType = GetEnumUnderlyingType(field.FieldType);
            var compareToMethod = GetCompareToMethod(underlyingType);
            if (compareToMethod != null)
            {
                return new ComparableFieldMember(field, compareToMethod);
            }

            throw new NotSupportedException($"Field {field.DisplayName()} is not supported.");
        }

        private static MethodInfo GetCompareToMethod(Type type) => type.GetMethod(
            nameof(IComparable.CompareTo),
            new[] { type });

        private static Type GetEnumUnderlyingType(Type type) =>
            type.IsEnum
                ? Enum.GetUnderlyingType(type)
                : type;
    }
}
