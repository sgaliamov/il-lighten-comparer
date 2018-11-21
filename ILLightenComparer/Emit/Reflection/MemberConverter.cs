using System;
using System.Reflection;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Members.Base;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        public Member Convert(MemberInfo memberInfo)
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
            if (property.PropertyType == typeof(string))
            {
                return new StringPropertyMember
                {
                    MemberType = property.PropertyType,
                    Name = property.Name,
                    OwnerType = property.DeclaringType,
                    GetterMethod = property.GetMethod
                };
            }

            var underlyingType = GetUnderlyingType(property.PropertyType);
            var compareToMethod = GetCompareToMethod(underlyingType);
            if (compareToMethod != null)
            {
                return new ComparablePropertyMember
                {
                    MemberType = underlyingType,
                    Name = property.Name,
                    GetterMethod = property.GetGetMethod(),
                    OwnerType = property.DeclaringType,
                    CompareToMethod = compareToMethod
                };
            }

            throw new NotSupportedException($"Property {property.DisplayName()} is not supported.");
        }

        private static Member Convert(FieldInfo field)
        {
            if (field.FieldType == typeof(string))
            {
                return new StringFiledMember
                {
                    MemberType = field.FieldType,
                    Name = field.Name,
                    OwnerType = field.DeclaringType,
                    FieldInfo = field
                };
            }

            var underlyingType = GetUnderlyingType(field.FieldType);
            var compareToMethod = GetCompareToMethod(underlyingType);
            if (compareToMethod != null)
            {
                return new ComparableFieldMember
                {
                    MemberType = underlyingType,
                    Name = field.Name,
                    OwnerType = field.DeclaringType,
                    FieldInfo = field,
                    CompareToMethod = compareToMethod
                };
            }

            throw new NotSupportedException($"Field {field.DisplayName()} is not supported.");
        }

        private static MethodInfo GetCompareToMethod(Type type) => type.GetMethod(
            nameof(IComparable.CompareTo),
            new[] { type });

        private static Type GetUnderlyingType(Type type) =>
            type.IsEnum
                ? Enum.GetUnderlyingType(type)
                : type;
    }
}
